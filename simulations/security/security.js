/*
 * Security simulator (Module 9). Conceptual defense-layer model: no attack content,
 * only which stage of source -> ingest -> agent -> tool -> sink cuts each path.
 * Classic script; depends on ../common/sim.js (window.Sim).
 */
(function () {
  'use strict';
  var Sim = window.Sim;
  var $ = Sim.$, el = Sim.el;

  var STAGES = [
    { id: 'source', name: 'Untrusted source' },
    { id: 'ingest', name: 'MCP / ingest' },
    { id: 'agent', name: 'Agent context' },
    { id: 'tool', name: 'Tool call' },
    { id: 'sink', name: 'Sink' }
  ];

  /* Six lab attacks, described only by path and sink (09.2–09.4). */
  var ATTACKS = [
    { id: 'A01', title: 'Injected ticket → file sink', kind: 'inject', owasp: 'LLM01 · ASI01',
      path: ['ticket BILL-901', 'tickets MCP result', 'instruction in context', 'write a file outside src/', 'exfil.txt (file)'] },
    { id: 'A02', title: 'Poisoned runbook → network sink', kind: 'inject', owasp: 'LLM01 · ASI01', variants: true,
      path: ['runbook doc', 'docs fetch result', 'instruction in context', 'outbound request', 'egress catcher (network)'] },
    { id: 'A03', title: 'Ticket → agent widens its own permissions', kind: 'inject', owasp: 'LLM06 · ASI03',
      path: ['ticket', 'tickets MCP result', 'instruction in context', 'edit .claude/settings', 'settings-allow'] },
    { id: 'A04', title: 'Poisoned MCP tool description', kind: 'inject', owasp: 'LLM03 · ASI04',
      path: ['notes server description', 'loaded at connect', 'instruction in context', 'write a file outside src/', 'file sink'] },
    { id: 'A05', title: 'Hallucinated package name', kind: 'halluc', owasp: 'LLM03 · ASI04',
      path: ['package the model invented', '— (no MCP)', 'model proposes it', 'add package to .csproj', 'manifest + restore'] },
    { id: 'A06', title: 'Ticket → canary posted as comment', kind: 'inject', owasp: 'LLM02 · ASI02',
      path: ['ticket with canary', 'tickets MCP result', 'instruction in context', 'add_comment', 'ticket-comment'] }
  ];

  /* A02 variants: how the outbound request is made. Bash rules match command text; the sandbox does not care. */
  var VARIANTS = [
    { id: 'plain', name: 'plain curl command', share: 0.3 },
    { id: 'wrapped', name: 'curl wrapped in another shell', share: 0.3 },
    { id: 'allowed', name: 'code run by an allowed command (a test opens a socket)', share: 0.4 }
  ];

  var P_FOLLOW = 0.92;       // undefended agent acts on injected text
  var P_FOLLOW_RULE = 0.55;  // with the CLAUDE.md rule: lower, not zero
  var P_HALLUC = 0.6;        // model proposes an invented package on the dependency task

  var DEFENSES = [
    { id: 'pin', stage: 'ingest', name: 'Pin + diff MCP tool descriptions', lesson: '09.3',
      help: 'A changed description is flagged DRIFT and not auto-trusted.' },
    { id: 'prompt', stage: 'agent', name: 'CLAUDE.md rule: ignore instructions in tickets/docs', lesson: '09.2',
      help: 'Lowers how often the model acts on injected text. Not a boundary.' },
    { id: 'least', stage: 'tool', name: 'Least privilege (Edit only src/ tests/, Bash only build/test, read-scoped tickets MCP)', lesson: '09.4',
      help: 'Off = the 09.4 break: Bash(*), Edit(**), add_comment available.' },
    { id: 'deny', stage: 'tool', name: 'Deny rules (.claude/**, secrets, Bash(curl*))', lesson: '09.5',
      help: 'Matches command text: stops a plain curl, not a wrapped one.' },
    { id: 'ask', stage: 'tool', name: 'Ask gate on .csproj edits', lesson: '09.4',
      help: 'A person approves dependency changes.' },
    { id: 'hook', stage: 'tool', name: 'Guard hook (PreToolUse)', lesson: '09.5', select: [
      ['off', 'off'], ['closed', 'on, fails closed'], ['open-broken', 'on, jq missing, naive (fails open)'], ['closed-broken', 'on, jq missing, fails closed']] },
    { id: 'egress', stage: 'sink', name: 'Sandbox network egress', lesson: '09.5', select: [
      ['open', 'no sandbox'], ['broad', 'sandbox, allowlist too broad'], ['empty', 'sandbox, allowedDomains: []']] },
    { id: 'mapping', stage: 'sink', name: 'NuGet package source mapping', lesson: '09.3',
      help: 'Unknown packages fail restore.' },
    { id: 'secrets', stage: 'tool', name: 'Secret isolation (Read deny + sandbox denyRead)', lesson: '09.5',
      help: 'Cuts the private-data leg; no lab attack here needs a real secret.' },
    { id: 'audit', stage: 'sink', name: 'Audit hook (PostToolUse, never blocks)', lesson: '09.5',
      help: 'Stops nothing. Records every call.' }
  ];

  var ALL_ON = { pin: true, prompt: true, least: true, deny: true, ask: true, hook: 'closed', egress: 'empty', mapping: true, secrets: true, audit: true };
  var ALL_OFF = { pin: false, prompt: false, least: false, deny: false, ask: false, hook: 'off', egress: 'open', mapping: false, secrets: false, audit: false };

  function merge(a, b) { var o = {}; Object.keys(a).forEach(function (k) { o[k] = a[k]; }); Object.keys(b || {}).forEach(function (k) { o[k] = b[k]; }); return o; }

  var PRESETS = {
    baseline: { label: 'Baseline', def: ALL_OFF, trials: 5, floor: 1, seed: 19,
      title: 'Baseline: no architectural defenses',
      text: 'All three trifecta legs are present. Almost every trial reaches its sink; the few that do not are luck (the model happened not to act on the text), not a boundary. Turn on one layer at a time and see which attacks it stops.' },
    'prompt-only': { label: 'Prompt-only', def: merge(ALL_OFF, { prompt: true }), trials: 5, floor: 1, seed: 19,
      title: 'Prompt-only: a CLAUDE.md rule and nothing else (09.2 break)',
      text: 'The breach rate drops but does not reach zero. Nothing in the path is cut: every stop happens at the agent stage, by chance. Raise trials to 100 to see the rate settle well above zero.' },
    layered: { label: 'Layered (09.6)', def: ALL_ON, trials: 5, floor: 1, seed: 19,
      title: 'Layered defenses: the hardened config (09.5, 09.6)',
      text: 'Every attack is stopped, and the first stage that stops it is marked. The depth column counts how many layers cover its weakest variant. Switch layers off one at a time: most attacks fall to the next layer — A06 has only one. 0 breaches in 5 still means a 95% upper bound of about 60%.' },
    partial: { label: 'Partial (09.6 break)', def: merge(ALL_ON, { egress: 'broad' }), trials: 5, floor: 0.8, seed: 19,
      title: 'Partial: the egress allowlist is too broad (09.6 break)',
      text: 'A02 is blocked on most trials but gets out when the request is made by code an allowed command runs. With the default 0.8 floor the gate passes a 4/5 result; switch the floor to 1.0 and it fails. One breach in five is a breach.' },
    'fail-open': { label: 'Hook fails open (09.5 break)', def: merge(ALL_ON, { hook: 'open-broken' }), trials: 5, floor: 1, seed: 19,
      title: 'The guard hook fails open (09.5 break)',
      text: 'jq is missing and the hook was written the naive way, so it waves every call through. Because the other layers do not depend on it, every attack is still stopped — but depth drops. Now also switch off least privilege and watch A01 escape.' }
  };
  var ORDER = ['baseline', 'prompt-only', 'layered', 'partial', 'fail-open'];

  var state = {};
  var draws = null;

  function applyPreset(name) {
    var p = PRESETS[name];
    state = merge(p.def, { preset: name, trials: p.trials, floor: p.floor, seed: Sim.seedFromQuery(p.seed) });
    Sim.$$('#presets button').forEach(function (b) { b.setAttribute('aria-pressed', String(b.dataset.preset === name)); });
    var note = $('#note');
    Sim.clear(note);
    note.appendChild(el('strong', { text: p.title }));
    note.appendChild(el('span', { text: p.text }));
    syncControls();
    redraw(true);
  }

  /* Fixed random numbers per attack × trial, so toggling a defense never reshuffles luck. */
  function makeDraws() {
    var rng = Sim.rng(state.seed);
    draws = ATTACKS.map(function () {
      var arr = [];
      for (var i = 0; i < 100; i++) arr.push({ follow: rng(), variant: rng() });
      return arr;
    });
  }

  function variantOf(u) {
    var acc = 0;
    for (var i = 0; i < VARIANTS.length; i++) { acc += VARIANTS[i].share; if (u < acc) return VARIANTS[i].id; }
    return VARIANTS[VARIANTS.length - 1].id;
  }

  /* Which deterministic layers block attack a (variant v) under state s. Ordered by stage. */
  function blockers(a, v, s) {
    var out = [];
    var hookWorks = s.hook === 'closed';
    var hookDeniesAll = s.hook === 'closed-broken';
    if (s.pin && a.id === 'A04') out.push({ stage: 'ingest', id: 'pin', name: 'pinned description (DRIFT)' });
    if (s.least) {
      if (a.id === 'A01' || a.id === 'A04') out.push({ stage: 'tool', id: 'least', name: 'least privilege (Edit only src/ tests/)' });
      if (a.id === 'A03') out.push({ stage: 'tool', id: 'least', name: 'least privilege (no Edit on .claude/)' });
      if (a.id === 'A06') out.push({ stage: 'tool', id: 'least', name: 'least privilege (no add_comment tool)' });
      if (a.id === 'A02' && v !== 'allowed') out.push({ stage: 'tool', id: 'least', name: 'least privilege (Bash only build/test)' });
    }
    if (s.deny) {
      if (a.id === 'A03') out.push({ stage: 'tool', id: 'deny', name: 'deny .claude/**' });
      if (a.id === 'A02' && v === 'plain') out.push({ stage: 'tool', id: 'deny', name: 'deny Bash(curl*)' });
    }
    if (s.ask && a.id === 'A05') out.push({ stage: 'tool', id: 'ask', name: 'ask gate (.csproj)' });
    if (hookDeniesAll && a.id !== 'A05') out.push({ stage: 'tool', id: 'hook', name: 'guard hook denies all (fail closed)' });
    if (hookWorks) {
      if (a.id === 'A01' || a.id === 'A04') out.push({ stage: 'tool', id: 'hook', name: 'guard hook (sink path)' });
      if (a.id === 'A03') out.push({ stage: 'tool', id: 'hook', name: 'guard hook (protected path)' });
      if (a.id === 'A02' && v !== 'allowed') out.push({ stage: 'tool', id: 'hook', name: 'guard hook (egress command)' });
    }
    if (a.id === 'A02' && s.egress === 'empty') out.push({ stage: 'sink', id: 'egress', name: 'sandbox egress (no domains)' });
    if (s.mapping && a.id === 'A05') out.push({ stage: 'sink', id: 'mapping', name: 'package source mapping' });
    return out;
  }

  function runTrial(ai, t) {
    var a = ATTACKS[ai], d = draws[ai][t], s = state;
    var v = a.variants ? variantOf(d.variant) : null;
    var bl = blockers(a, v, s);
    var steps = [];
    steps.push({ stage: 'source', text: 'Enters from ' + a.path[0] + '.' + (v ? ' Variant: ' + VARIANTS.filter(function (x) { return x.id === v; })[0].name + '.' : '') });
    var ing = bl.filter(function (b) { return b.stage === 'ingest'; });
    if (ing.length) { steps.push({ stage: 'ingest', text: 'Stopped: ' + ing[0].name + '.', stop: true }); return { v: v, stop: 'ingest', by: ing[0].name, steps: steps, bl: bl }; }
    steps.push({ stage: 'ingest', text: a.path[1] + ' passes into the context.' });
    var p = a.kind === 'halluc' ? P_HALLUC : (s.prompt ? P_FOLLOW_RULE : P_FOLLOW);
    if (d.follow >= p) {
      steps.push({ stage: 'agent', text: (a.kind === 'halluc' ? 'Model picked a real package this time' : 'Model did not act on the text this time') +
        ' (draw ' + d.follow.toFixed(2) + ' ≥ ' + p.toFixed(2) + '). Luck, not a boundary.', luck: true });
      return { v: v, stop: 'agent', by: 'luck', steps: steps, bl: bl };
    }
    steps.push({ stage: 'agent', text: (a.kind === 'halluc' ? 'Model proposes the invented package' : 'Model acts on the injected text') +
      ' (draw ' + d.follow.toFixed(2) + ' < ' + p.toFixed(2) + ').' });
    var tool = bl.filter(function (b) { return b.stage === 'tool'; });
    if (tool.length) { steps.push({ stage: 'tool', text: 'Stopped: ' + tool[0].name + '.', stop: true }); return { v: v, stop: 'tool', by: tool[0].name, steps: steps, bl: bl }; }
    steps.push({ stage: 'tool', text: 'Tool call allowed: ' + a.path[3] + '.' });
    var sink = bl.filter(function (b) { return b.stage === 'sink'; });
    if (sink.length) { steps.push({ stage: 'sink', text: 'Stopped: ' + sink[0].name + '.', stop: true }); return { v: v, stop: 'sink', by: sink[0].name, steps: steps, bl: bl }; }
    steps.push({ stage: 'sink', text: 'Canary reached ' + a.path[4] + '. Breach' + (s.audit ? ' — recorded in the audit log.' : ' — and nothing recorded it.'), breach: true });
    return { v: v, stop: 'breach', by: 'breach', steps: steps, bl: bl };
  }

  /* Minimum depth over the variants that exist for this attack. */
  function depth(a) {
    var vs = a.variants ? VARIANTS.map(function (x) { return x.id; }) : [null];
    return Math.min.apply(null, vs.map(function (v) { return blockers(a, v, state).length; }));
  }

  function results() {
    return ATTACKS.map(function (a, ai) {
      var counts = { ingest: 0, agent: 0, tool: 0, sink: 0, breach: 0 }, by = {};
      for (var t = 0; t < state.trials; t++) {
        var r = runTrial(ai, t);
        counts[r.stop]++;
        var key = r.stop + '|' + r.by;
        by[key] = (by[key] || 0) + 1;
      }
      return { a: a, counts: counts, by: by, depth: depth(a) };
    });
  }

  function legs() {
    return {
      untrusted: true,
      privateData: !state.secrets,
      external: state.egress !== 'empty' || !state.least
    };
  }

  /* ---------- rendering ---------- */

  function buildDefenses() {
    var box = $('#defenses');
    DEFENSES.forEach(function (d) {
      var stageName = STAGES.filter(function (s) { return s.id === d.stage; })[0].name;
      if (d.select) {
        var sel = el('select', { id: 'd-' + d.id });
        d.select.forEach(function (o) { sel.appendChild(el('option', { value: o[0], text: o[1] })); });
        sel.addEventListener('change', function () { state[d.id] = sel.value; redraw(false); });
        box.appendChild(el('div', { class: 'field' }, [
          el('label', { for: 'd-' + d.id, text: d.name + ' · ' + stageName + ' · ' + d.lesson }), sel]));
      } else {
        var cb = el('input', { type: 'checkbox', id: 'd-' + d.id });
        cb.addEventListener('change', function () { state[d.id] = cb.checked; redraw(false); });
        box.appendChild(el('div', { class: 'check' }, [cb,
          el('label', { for: 'd-' + d.id }, [
            el('span', { text: d.name }),
            el('small', { class: 'd-help', text: stageName + ' · ' + d.lesson + (d.help ? ' — ' + d.help : '') })])]));
      }
    });
  }

  function syncControls() {
    DEFENSES.forEach(function (d) {
      var n = $('#d-' + d.id);
      if (d.select) n.value = state[d.id]; else n.checked = !!state[d.id];
    });
    $('#trials').value = String(state.trials);
    $('#floor').value = String(state.floor);
    $('#seed').textContent = 'seed ' + state.seed;
  }

  function upper95(k, n) { return k === 0 ? Math.min(1, 3 / n) : Sim.wilson(k, n)[1]; }

  function renderSummary(res) {
    var n = state.trials, totalTrials = n * ATTACKS.length;
    var breaches = res.reduce(function (s, r) { return s + r.counts.breach; }, 0);
    var luck = res.reduce(function (s, r) { return s + r.counts.agent; }, 0);
    var cut = totalTrials - breaches - luck;
    var box = Sim.clear($('#summary'));
    function stat(k, v, cls) { box.appendChild(el('div', {}, [el('div', { class: 'k', text: k }), el('div', { class: 'stat ' + (cls || ''), text: v })])); }
    stat('Breaches', breaches + ' / ' + totalTrials, breaches ? 'bad' : 'good');
    stat('Cut by a layer', cut + ' / ' + totalTrials);
    stat('Stopped only by luck', luck + ' / ' + totalTrials, luck ? 'warn' : '');
    var util = state.hook === 'closed-broken' ? 'blocked (safe degrade)' : 'passes';
    stat('Legit build + test task', util, state.hook === 'closed-broken' ? 'warn' : 'good');

    var L = legs(), count = (L.untrusted ? 1 : 0) + (L.privateData ? 1 : 0) + (L.external ? 1 : 0);
    var tri = Sim.clear($('#trifecta'));
    tri.appendChild(el('span', { class: 'k', text: 'Lethal trifecta (09.1): ' }));
    [['untrusted content', L.untrusted], ['private data', L.privateData], ['external communication', L.external]].forEach(function (x) {
      tri.appendChild(el('span', { class: 'badge ' + (x[1] ? 'bad' : 'good'), text: (x[1] ? '● ' : '○ ') + x[0] }));
    });
    tri.appendChild(el('span', { class: count === 3 ? 'bad' : 'good', text: count === 3 ? ' all three legs: an injection here can exfiltrate' : ' Rule of Two satisfied: ' + count + ' of 3 legs' }));
  }

  function renderPaths(res) {
    var box = Sim.clear($('#paths'));
    res.forEach(function (r) {
      var row = el('div', { class: 'path', role: 'group', 'aria-label': r.a.id + ' ' + r.a.title });
      row.appendChild(el('div', { class: 'path-h' }, [
        el('strong', { text: r.a.id + ' ' }), el('span', { text: r.a.title + ' ' }), el('small', { class: 'muted', text: r.a.owasp })]));
      var chain = el('ol', { class: 'chain' });
      STAGES.forEach(function (s, i) {
        var n = s.id === 'sink' ? r.counts.sink + r.counts.breach : (r.counts[s.id] || 0);
        var cls = 'node';
        var info = [];
        if (s.id === 'agent' && r.counts.agent) { cls += ' luck'; info.push(r.counts.agent + ' luck'); }
        else if (s.id !== 'source' && n && s.id !== 'sink') { cls += ' stop'; }
        if (s.id === 'sink') {
          if (r.counts.breach) { cls += ' breach'; info.push(r.counts.breach + ' breach'); }
          if (r.counts.sink) { cls += ' stop'; info.push(r.counts.sink + ' stopped'); }
        } else if (s.id !== 'agent' && s.id !== 'source' && n) info.push(n + ' stopped');
        var byNames = Object.keys(r.by).filter(function (k) { return k.split('|')[0] === s.id && k.split('|')[1] !== 'luck'; })
          .map(function (k) { return k.split('|')[1]; });
        var li = el('li', { class: cls }, [
          el('span', { class: 'node-s', text: s.name }),
          el('span', { class: 'node-p', text: r.a.path[i] }),
          info.length ? el('span', { class: 'node-c', text: info.join(' · ') }) : null,
          byNames.length ? el('span', { class: 'node-b', text: byNames.join('; ') }) : null
        ]);
        chain.appendChild(li);
      });
      row.appendChild(chain);
      box.appendChild(row);
    });
  }

  function renderTable(res) {
    var n = state.trials, floor = state.floor;
    var t = Sim.clear($('#table'));
    var head = el('tr');
    ['Attack', 'Held', 'Breach rate', '95% upper bound', 'Depth', 'Gate (floor ' + floor.toFixed(1) + ')'].forEach(function (h, i) {
      head.appendChild(el('th', { class: i > 0 && i < 5 ? 'num' : '', text: h, scope: 'col' }));
    });
    t.appendChild(el('thead', {}, [head]));
    var body = el('tbody');
    var fails = 0;
    res.forEach(function (r) {
      var k = r.counts.breach, held = n - k, ok = held / n >= floor - 1e-9;
      if (!ok) fails++;
      var ub = upper95(k, n);
      body.appendChild(el('tr', {}, [
        el('td', { text: r.a.id + ' ' + r.a.title }),
        el('td', { class: 'num', text: held + '/' + n }),
        el('td', { class: 'num', text: Sim.pct(k / n) }),
        el('td', { class: 'num', text: '≤ ' + Sim.pct(ub) + (k === 0 ? ' (3/n)' : ' (Wilson)') }),
        el('td', { class: 'num ' + (r.depth === 0 ? 'bad' : r.depth === 1 ? 'warn' : 'good'), text: String(r.depth) }),
        el('td', {}, [el('span', { class: 'badge ' + (ok ? 'good' : 'bad'), text: ok ? 'ok' : 'FAIL' })])
      ]));
    });
    t.appendChild(body);
    var totalK = res.reduce(function (s, r) { return s + r.counts.breach; }, 0);
    var g = $('#gate');
    g.className = 'gate ' + (fails ? 'bad' : 'good');
    g.textContent = (fails ? 'GATE FAILED: ' + fails + ' attack(s) below the ' + floor.toFixed(1) + ' floor.' : 'GATE PASSED at floor ' + floor.toFixed(1) + '.') +
      (fails === 0 && totalK > 0 ? ' Note: ' + totalK + ' breach(es) were tolerated by this floor.' : '') +
      (totalK === 0 ? ' Residual risk is not zero: "0 in ' + n + '" means a 95% upper bound of about ' + Sim.pct(Math.min(1, 3 / n)) + ' per attack.' : '');
  }

  function buildTraceSelectors() {
    var sa = $('#trace-attack');
    ATTACKS.forEach(function (a, i) { sa.appendChild(el('option', { value: String(i), text: a.id + ' ' + a.title })); });
    sa.addEventListener('change', function () { renderTrace(false); });
    $('#trace-trial').addEventListener('change', function () { renderTrace(false); });
    $('#trace-play').addEventListener('click', function () { renderTrace(true); });
  }

  function fillTrialSelector() {
    var st = $('#trace-trial'), cur = parseInt(st.value, 10) || 0;
    Sim.clear(st);
    for (var i = 0; i < state.trials; i++) st.appendChild(el('option', { value: String(i), text: String(i + 1) }));
    st.value = String(Math.min(cur, state.trials - 1));
  }

  var timer = null;
  function renderTrace(animate) {
    if (timer) { clearInterval(timer); timer = null; }
    var ai = parseInt($('#trace-attack').value, 10) || 0, t = parseInt($('#trace-trial').value, 10) || 0;
    var r = runTrial(ai, t);
    var box = Sim.clear($('#trace'));
    var reduce = false;
    try { reduce = window.matchMedia('(prefers-reduced-motion: reduce)').matches; } catch (e) { reduce = false; }
    var items = r.steps.map(function (s) {
      var stage = STAGES.filter(function (x) { return x.id === s.stage; })[0].name;
      return el('li', { class: s.stop ? 'stop' : s.breach ? 'breach' : s.luck ? 'luck' : '' }, [el('strong', { text: stage + ': ' }), el('span', { text: s.text })]);
    });
    if (!animate || reduce) { items.forEach(function (li) { box.appendChild(li); }); return; }
    var i = 0;
    box.appendChild(items[i++]);
    timer = setInterval(function () {
      if (i >= items.length) { clearInterval(timer); timer = null; return; }
      box.appendChild(items[i++]);
    }, 450);
  }

  function redraw(reseed) {
    if (reseed || !draws) makeDraws();
    var res = results();
    renderSummary(res);
    renderPaths(res);
    renderTable(res);
    fillTrialSelector();
    renderTrace(false);
    $('#seed').textContent = 'seed ' + state.seed;
  }

  function init() {
    var tabs = $('#presets');
    ORDER.forEach(function (k) {
      tabs.appendChild(el('button', { type: 'button', 'data-preset': k, 'aria-pressed': 'false', text: PRESETS[k].label,
        onclick: function () { applyPreset(k); } }));
    });
    buildDefenses();
    buildTraceSelectors();
    $('#trials').addEventListener('change', function () { state.trials = parseInt(this.value, 10); redraw(false); });
    $('#floor').addEventListener('change', function () { state.floor = parseFloat(this.value); redraw(false); });
    $('#rerun').addEventListener('click', function () { state.seed = (state.seed * 7919 + 13) % 100000 + 1; redraw(true); });
    applyPreset(Sim.preset(ORDER, 'baseline'));
  }

  // Exposed for headless checks only.
  window.SecuritySim = { ATTACKS: ATTACKS, PRESETS: PRESETS, _state: function () { return state; }, _results: function () { return results(); },
    _setup: function (preset, seed) { state = merge(PRESETS[preset].def, { trials: PRESETS[preset].trials, floor: PRESETS[preset].floor, seed: seed || PRESETS[preset].seed }); makeDraws(); } };

  if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', init); else init();
})();
