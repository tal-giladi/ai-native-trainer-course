/*
 * Multi-agent simulator (Module 10). Classic script; depends on ../common/sim.js (window.Sim).
 * Expected values port AgentTeam estimate (labs/module-10/tools/AgentTeam) exactly.
 */
(function () {
  'use strict';
  var Sim = window.Sim;
  var $ = Sim.$, el = Sim.el;

  /* Illustrative inputs, identical to labs/module-10/pipelines/*.json */
  var TASKS = {
    code: { label: 'Code task', single: { p: 0.60, cost: 0.30, sec: 180 }, planner: { p: 0.95, cost: 0.06, sec: 45 },
      worker: { p: 0.65, pbad: 0.20, pfix: 0.60, dmg: 0.10, cost: 0.25, sec: 150 }, reviewer: { r: 0.70, f: 0.15, cost: 0.07, sec: 40 }, R: 3 },
    qa: { label: 'Question task', single: { p: 0.88, cost: 0.05, sec: 25 }, planner: { p: 0.97, cost: 0.03, sec: 20 },
      worker: { p: 0.88, pbad: 0.50, pfix: 0.50, dmg: 0.30, cost: 0.04, sec: 20 }, reviewer: { r: 0.40, f: 0.15, cost: 0.03, sec: 15 }, R: 2 }
  };

  var EXTRA = { k: 3, stageP: 0.90, tokens: 30, price: 3, a: 0.6, loopR: 3, budget: 1.00, round1: 0.21, roundN: 0.15,
    coupling: 0.35, indep: 0.05, votes: 3, testRecall: 0.9, n: 120 };

  var PARAMS = [
    { g: 'Single agent', k: 'single.p', label: 'success p', min: 0.05, max: 0.99, step: 0.01, f: 'p' },
    { g: 'Single agent', k: 'single.cost', label: 'cost per task ($)', min: 0.01, max: 1, step: 0.01, f: '$' },
    { g: 'Single agent', k: 'single.sec', label: 'seconds', min: 5, max: 400, step: 5, f: 's' },
    { g: 'Planner', k: 'planner.p', label: 'plan sound', min: 0.5, max: 1, step: 0.01, f: 'p' },
    { g: 'Planner', k: 'planner.cost', label: 'cost per call ($)', min: 0, max: 0.5, step: 0.01, f: '$' },
    { g: 'Planner', k: 'planner.sec', label: 'seconds', min: 0, max: 200, step: 5, f: 's' },
    { g: 'Worker', k: 'worker.p', label: 'success on a sound plan', min: 0.05, max: 0.99, step: 0.01, f: 'p' },
    { g: 'Worker', k: 'worker.pbad', label: 'success after a bad plan', min: 0, max: 0.99, step: 0.01, f: 'p' },
    { g: 'Worker', k: 'worker.pfix', label: 'p_fix: flagged wrong result gets fixed', min: 0, max: 1, step: 0.01, f: 'p' },
    { g: 'Worker', k: 'worker.dmg', label: 'd: rework breaks a right result', min: 0, max: 1, step: 0.01, f: 'p' },
    { g: 'Worker', k: 'worker.cost', label: 'cost per call ($)', min: 0, max: 1, step: 0.01, f: '$' },
    { g: 'Worker', k: 'worker.sec', label: 'seconds', min: 5, max: 400, step: 5, f: 's' },
    { g: 'Reviewer', k: 'reviewer.r', label: 'r: recall on wrong results', min: 0, max: 1, step: 0.01, f: 'p' },
    { g: 'Reviewer', k: 'reviewer.f', label: 'f: false alarm on right results', min: 0, max: 1, step: 0.01, f: 'p' },
    { g: 'Reviewer', k: 'reviewer.cost', label: 'cost per call ($)', min: 0, max: 0.5, step: 0.01, f: '$' },
    { g: 'Reviewer', k: 'reviewer.sec', label: 'seconds', min: 0, max: 200, step: 5, f: 's' },
    { g: 'Loop', k: 'R', label: 'max review rounds', min: 1, max: 6, step: 1, f: 'i' },
    { g: 'Topologies', v: 'topologies', k: 'x.coupling', label: 'specialists: chance the parts disagree on an implicit decision', min: 0, max: 0.9, step: 0.01, f: 'p' },
    { g: 'Topologies', v: 'topologies', k: 'x.indep', label: 'parallel sectioning: same, for truly independent parts', min: 0, max: 0.9, step: 0.01, f: 'p' },
    { g: 'Topologies', v: 'topologies', k: 'x.votes', label: 'voting: attempts', min: 1, max: 8, step: 1, f: 'i' },
    { g: 'Topologies', v: 'topologies', k: 'x.testRecall', label: 'voting: tests catch a wrong attempt', min: 0, max: 1, step: 0.01, f: 'p' },
    { g: 'Chain', v: 'compounding', k: 'x.k', label: 'stages (agents) in the chain', min: 1, max: 7, step: 1, f: 'i' },
    { g: 'Chain', v: 'compounding', k: 'x.stageP', label: 'success per stage', min: 0.5, max: 1, step: 0.01, f: 'p' },
    { g: 'Chain', v: 'compounding', k: 'x.tokens', label: 'input tokens per call (thousands)', min: 5, max: 200, step: 5, f: 'i' },
    { g: 'Chain', v: 'compounding', k: 'x.price', label: 'price per million input tokens ($)', min: 0.5, max: 15, step: 0.5, f: '$' },
    { g: 'Loop cost', v: 'compounding', k: 'x.a', label: 'a: reviewer approves a round', min: 0, max: 1, step: 0.01, f: 'p' },
    { g: 'Loop cost', v: 'compounding', k: 'x.loopR', label: 'R: round cap', min: 1, max: 10, step: 1, f: 'i' },
    { g: 'Loop cost', v: 'compounding', k: 'x.budget', label: 'hard budget per task ($)', min: 0.2, max: 3, step: 0.05, f: '$' },
    { g: 'Simulated runs', v: 'review-loop', k: 'x.n', label: 'tasks to simulate', min: 20, max: 500, step: 10, f: 'i' }
  ];

  var PRESETS = {
    topologies: { label: 'Topologies', task: 'code',
      title: 'Topologies side by side (10.1)',
      text: 'Six shapes on one task, from the same per-agent numbers. Extra agents buy a mechanism — isolation, parallelism, an independent check, specialization — and every one of them costs calls. Raise the coupling for specialists and watch them fall below one agent; switch to the question task and watch every multi-agent shape lose.' },
    compounding: { label: 'Compounding', task: 'code',
      title: 'Reliability and cost compounding (10.3)',
      text: 'A chain succeeds only if every stage does: P = Πpᵢ. Cost scales with calls, not agents, and a review loop turns one agent into several calls. Slide the chain to 7 stages — the vendor pipeline — and compare with one agent.' },
    'review-loop': { label: 'Review loop vs one agent', task: 'code',
      title: 'Review loop vs one agent (10.4)',
      text: 'The reviewer changes success by rescues minus damage: (1−p)·r·p_fix − p·f·d. On the code task it gains about 22 points at 1.9× cost; switch to the question task — a strong single agent, a weak reviewer — and the same pipeline loses about 2 points at 2.4× cost. Simulated runs show the stop reasons and the p90 tail.' }
  };
  var ORDER = ['topologies', 'compounding', 'review-loop'];

  var P = null, X = null, view = 'topologies', task = 'code', seed = 10, traceIdx = 0;

  /* SVG namespace read from the parser, so the source holds no URL. */
  function SVG_NS() { var d = document.createElement('div'); d.innerHTML = '<svg></svg>'; return d.firstChild.namespaceURI; }

  function clone(o) { return JSON.parse(JSON.stringify(o)); }
  function get(k) { var s = k.split('.'); if (s[0] === 'x') return X[s[1]]; if (s.length === 1) return P[s[0]]; return P[s[0]][s[1]]; }
  function set(k, v) { var s = k.split('.'); if (s[0] === 'x') X[s[1]] = v; else if (s.length === 1) P[s[0]] = v; else P[s[0]][s[1]] = v; }
  function fmt(v, f) {
    if (f === 'p') return v.toFixed(2);
    if (f === '$') return '$' + v.toFixed(2);
    if (f === 's') return v + ' s';
    return String(v);
  }
  function money(x) { return '$' + x.toFixed(3); }
  function secs(x) { return Math.round(x) + ' s'; }

  /* ---------- expected values: exact port of AgentTeam estimate ---------- */
  function estimate(p) {
    var pp = p.planner.p, pw = p.worker.p, pbad = p.worker.pbad, pfix = p.worker.pfix, dmg = p.worker.dmg;
    var r = p.reviewer.r, f = p.reviewer.f;
    var cp = p.planner.cost, cw = p.worker.cost, cr = p.reviewer.cost;
    var tp = p.planner.sec, tw = p.worker.sec, trv = p.reviewer.sec;
    var R = p.R;
    var q1 = pp * pw + (1 - pp) * pbad;
    var c = q1, w = 1 - q1, success = 0, escapedWrong = 0, atMax = 0, cost = cp + cw, time = tp + tw, rounds = 1;
    for (var k = 1; ; k++) {
      var alive = c + w;
      cost += alive * cr; time += alive * trv;
      success += c * (1 - f); escapedWrong += w * (1 - r);
      var fc = c * f, fwr = w * r;
      if ((R > 0 && k >= R) || k >= 50) { success += fc; atMax += fc + fwr; break; }
      var again = fc + fwr;
      if (again < 1e-12) break;
      cost += again * (cp + cw); time += again * (tp + tw); rounds += again;
      c = fc * (1 - dmg) + fwr * pfix; w = again - c;
    }
    return {
      single: { p: p.single.p, cost: p.single.cost, time: p.single.sec },
      chain: { p: q1, cost: cp + cw, time: tp + tw },
      pwr: { p: success, cost: cost, time: time, rounds: rounds, atMax: atMax, escapedWrong: escapedWrong }
    };
  }

  /* ---------- topologies ---------- */
  function topologies() {
    var e = estimate(P);
    var pp = P.planner.p, pw = P.worker.p, pbad = P.worker.pbad;
    var part = Math.sqrt(pw), partBad = Math.sqrt(pbad);   // two halves; together as hard as the whole
    var cA = 0.6, cB = 0.5;                                 // halves' share of worker time and cost
    function split(conflict) {
      var ok = pp * part * part + (1 - pp) * partBad * partBad;
      return { p: ok * (1 - conflict), cost: P.planner.cost + (cA + cB) * P.worker.cost, time: P.planner.sec + Math.max(cA, cB) * P.worker.sec };
    }
    var ps = P.single.p, t = X.testRecall, k = X.votes;
    var fail = (1 - ps) * t, vote = 0;
    for (var j = 0; j < k; j++) vote += ps * Math.pow(fail, j);
    var hier = (function () {
      var ok = pp * pp * part * part + (1 - pp * pp) * partBad * partBad;
      return { p: ok * (1 - X.indep), cost: 2 * P.planner.cost + (cA + cB) * P.worker.cost, time: 2 * P.planner.sec + Math.max(cA, cB) * P.worker.sec };
    })();
    return [
      { id: 1, name: 'Single agent', buys: 'none needed', r: e.single, shape: ['agent'] },
      { id: 2, name: 'Chain: planner → worker', buys: 'isolation, a reviewable plan', r: e.chain, shape: ['planner', 'worker'] },
      { id: 3, name: 'Review loop: planner → worker ↔ reviewer (max ' + P.R + ')', buys: 'independent check', r: e.pwr, shape: ['planner', 'worker', 'reviewer'] },
      { id: 4, name: 'Specialists: coder ∥ tester (shared names)', buys: 'specialization', r: split(X.coupling), shape: ['router', 'coder ∥ tester'] },
      { id: 5, name: 'Parallel: sectioning (independent parts)', buys: 'parallelism', r: split(X.indep), shape: ['split', 'A ∥ B', 'merge'] },
      { id: '5b', name: 'Parallel: voting (' + k + ' attempts, tests pick)', buys: 'extra compute, not collaboration', r: { p: vote, cost: k * P.single.cost, time: P.single.sec }, shape: [k + ' × agent', 'tests pick'] },
      { id: 6, name: 'Hierarchical: lead → sub-lead → workers', buys: 'isolation at scale', r: hier, shape: ['lead', 'sub-lead', 'A ∥ B'] }
    ];
  }

  /* ---------- Monte Carlo of the review loop, same rules as estimate ---------- */
  function simulate(n, s) {
    var rng = Sim.rng(s), out = [];
    for (var i = 0; i < n; i++) {
      var spans = [], t = 0, cost = 0, round = 1;
      function span(role, sec, c, verdict) { spans.push({ role: role, round: round, start: t, dur: sec, cost: c, verdict: verdict || '' }); t += sec; cost += c; }
      var sound = rng() < P.planner.p;
      span('planner', P.planner.sec, P.planner.cost, sound ? 'plan sound' : 'plan leaves a decision open');
      var correct = rng() < (sound ? P.worker.p : P.worker.pbad);
      span('worker', P.worker.sec, P.worker.cost, correct ? 'result right' : 'result wrong');
      var stop, flaggedEver = false, firstWrong = !correct;
      for (;;) {
        var flag = correct ? rng() < P.reviewer.f : rng() < P.reviewer.r;
        span('reviewer', P.reviewer.sec, P.reviewer.cost, flag ? 'CHANGES' : 'APPROVE');
        if (!flag) { stop = 'approved'; break; }
        flaggedEver = true;
        if (round >= P.R) { stop = 'max_rounds'; break; }
        round++;
        span('planner', P.planner.sec, P.planner.cost, 'replan');
        correct = correct ? rng() >= P.worker.dmg : rng() < P.worker.pfix;
        span('worker', P.worker.sec, P.worker.cost, correct ? 'rework right' : 'rework wrong');
      }
      var approvedWrong = stop === 'approved' && !correct;
      out.push({ i: i + 1, spans: spans, cost: cost, wall: t, rounds: round, stop: stop, pass: correct, approvedWrong: approvedWrong,
        reworkPass: flaggedEver && correct && stop === 'approved', firstWrong: firstWrong });
    }
    return out;
  }

  function quantile(a, q) {
    var s = a.slice().sort(function (x, y) { return x - y; });
    if (!s.length) return 0;
    var pos = (s.length - 1) * q, lo = Math.floor(pos), hi = Math.ceil(pos);
    return s[lo] + (s[hi] - s[lo]) * (pos - lo);
  }

  /* ---------- UI: parameters ---------- */
  function buildParams() {
    var box = Sim.clear($('#params'));
    var groups = {};
    PARAMS.forEach(function (d) {
      if (d.v && d.v !== view) return;
      if (!groups[d.g]) {
        var open = d.v ? true : (view === 'review-loop' || d.g === 'Worker' || d.g === 'Reviewer');
        groups[d.g] = el('details', { class: 'pgroup', open: open ? true : null }, [el('summary', { text: d.g })]);
        box.appendChild(groups[d.g]);
      }
      var id = 'p-' + d.k.replace('.', '-');
      var out = el('output', { for: id, text: fmt(get(d.k), d.f) });
      var inp = el('input', { type: 'range', id: id, min: d.min, max: d.max, step: d.step, value: get(d.k) });
      inp.addEventListener('input', function () {
        var v = parseFloat(inp.value);
        set(d.k, v);
        out.textContent = fmt(v, d.f);
        render();
      });
      groups[d.g].appendChild(el('div', { class: 'field' }, [el('div', { class: 'row' }, [el('label', { for: id, text: d.label }), out]), inp]));
    });
  }

  function loadTask(t) {
    task = t;
    var src = TASKS[t];
    P = { single: clone(src.single), planner: clone(src.planner), worker: clone(src.worker), reviewer: clone(src.reviewer), R: src.R };
    $('#task').value = t;
  }

  function setView(v, fromPreset) {
    view = v;
    Sim.$$('#presets button').forEach(function (b) { b.setAttribute('aria-pressed', String(b.dataset.preset === v)); });
    var p = PRESETS[v], note = Sim.clear($('#note'));
    note.appendChild(el('strong', { text: p.title }));
    note.appendChild(el('span', { text: p.text }));
    if (fromPreset) { loadTask(p.task); X = clone(EXTRA); }
    buildParams();
    render();
  }

  /* ---------- rendering helpers ---------- */
  function table(headers, rows, numFrom) {
    var t = el('table');
    var tr = el('tr');
    headers.forEach(function (h, i) { tr.appendChild(el('th', { scope: 'col', class: i >= (numFrom || 1) ? 'num' : '', text: h })); });
    t.appendChild(el('thead', {}, [tr]));
    var tb = el('tbody');
    rows.forEach(function (r) {
      var row = el('tr', r.cls ? { class: r.cls } : null);
      r.cells.forEach(function (c, i) {
        var td = el('td', { class: i >= (numFrom || 1) ? 'num' : '' });
        if (typeof c === 'string') td.textContent = c; else td.appendChild(c);
        row.appendChild(td);
      });
      tb.appendChild(row);
    });
    t.appendChild(tb);
    return el('div', { class: 'scroll-x' }, [t]);
  }

  function stat(k, v, cls, sub) {
    return el('div', {}, [el('div', { class: 'k', text: k }), el('div', { class: 'stat ' + (cls || ''), text: v }), sub ? el('div', { class: 'muted small', text: sub }) : null]);
  }

  function delta(x, base) {
    var d = (x - base) * 100;
    return el('span', { class: d > 0.05 ? 'good' : d < -0.05 ? 'bad' : '', text: (d >= 0 ? '+' : '−') + Math.abs(d).toFixed(1) + ' pts' });
  }

  /* Horizontal bars: rows [{label, value, max, cls}] */
  function bars(rows, fmtv, title) {
    var h = rows.length * 26 + 10, W = 560, L = 190;
    var ns = SVG_NS();
    var svg = document.createElementNS(ns, 'svg');
    svg.setAttribute('viewBox', '0 0 ' + W + ' ' + h);
    svg.setAttribute('role', 'img');
    svg.setAttribute('aria-label', title);
    var max = Math.max.apply(null, rows.map(function (r) { return r.max || r.value; }).concat([1e-9]));
    rows.forEach(function (r, i) {
      var y = 6 + i * 26;
      var tx = document.createElementNS(ns, 'text'); tx.setAttribute('x', L - 6); tx.setAttribute('y', y + 14); tx.setAttribute('text-anchor', 'end'); tx.textContent = r.label; svg.appendChild(tx);
      var bw = Math.max(1, (W - L - 70) * r.value / max);
      var rect = document.createElementNS(ns, 'rect'); rect.setAttribute('x', L); rect.setAttribute('y', y + 2); rect.setAttribute('width', bw); rect.setAttribute('height', 16);
      rect.setAttribute('rx', 3); rect.setAttribute('class', 'bar ' + (r.cls || '')); svg.appendChild(rect);
      var v = document.createElementNS(ns, 'text'); v.setAttribute('x', L + bw + 6); v.setAttribute('y', y + 14); v.textContent = fmtv(r.value); svg.appendChild(v);
    });
    return svg;
  }

  /* ---------- views ---------- */
  function renderTopologies(box) {
    var rows = topologies(), base = rows[0].r;
    box.appendChild(el('h2', { text: 'Same task, seven shapes — ' + TASKS[task].label.toLowerCase() }));
    box.appendChild(table(['Topology', 'Buys', 'Success', 'vs single', 'Exp. cost', 'Latency', 'Cost / success'], rows.map(function (t) {
      var shape = el('div', { class: 'shape' }, t.shape.map(function (s, i) { return el('span', { class: 'chip', text: s }); }));
      return { cls: t.id === 1 ? 'base' : '', cells: [el('div', {}, [el('strong', { text: t.id + ' ' + t.name }), shape]), t.buys,
        Sim.pct(t.r.p, 1), t.id === 1 ? '—' : delta(t.r.p, base.p), money(t.r.cost), secs(t.r.time), money(t.r.cost / Math.max(t.r.p, 1e-9))] };
    }), 2));
    box.appendChild(el('h3', { text: 'Cost per successful task' }));
    box.appendChild(bars(rows.map(function (t) { return { label: t.id + ' ' + t.name.split(':')[0].split(' (')[0], value: t.r.cost / Math.max(t.r.p, 1e-9), cls: t.id === 1 ? 'b1' : 'b2' }; }), money, 'Cost per successful task by topology'));
    var best = rows.slice().sort(function (a, b) { return b.r.p - a.r.p; })[0];
    box.appendChild(el('p', { class: 'muted', text: 'Highest success here: ' + best.name + ' (' + Sim.pct(best.r.p, 1) + '). ' +
      'Specialists and sectioning split one job in two; each half is modelled as √p so that together they are as hard as the whole, then multiplied by the chance the halves agree on names, signatures and files. Voting is ' +
      X.votes + ' single-agent attempts in parallel with tests picking the first that passes — it costs ' + X.votes + '× and is not collaboration (10.4).' }));
  }

  function renderCompounding(box) {
    var k = X.k, p = X.stageP;
    box.appendChild(el('h2', { text: 'Failure compounds: P = Πpᵢ' }));
    var chainP = Math.pow(p, k), costPer = X.tokens * 1000 * X.price / 1e6;
    var st = el('div', { class: 'stats' }, [
      stat(k + '-stage chain succeeds', Sim.pct(chainP, 1), chainP < P.single.p ? 'bad' : 'good', p.toFixed(2) + '^' + k),
      stat('One agent (single p)', Sim.pct(P.single.p, 1)),
      stat('Expected input cost', money(k * costPer), k + ' calls × ' + X.tokens + 'k × $' + X.price.toFixed(2) + '/M'),
      stat('Planner × worker', Sim.pct(P.planner.p * P.worker.p, 1), P.planner.p.toFixed(2) + ' × ' + P.worker.p.toFixed(2))
    ]);
    box.appendChild(st);
    var rows = [];
    for (var j = 1; j <= 7; j++) rows.push({ label: j + (j === 1 ? ' stage' : ' stages') + (j === 7 ? ' (vendor pipeline)' : ''), value: Math.pow(p, j), max: 1, cls: j === k ? 'b2' : 'b1' });
    box.appendChild(el('h3', { text: 'All stages succeed, by chain length (p = ' + p.toFixed(2) + ' each)' }));
    box.appendChild(bars(rows, function (v) { return Sim.pct(v, 1); }, 'Chain success by number of stages'));
    box.appendChild(el('p', { class: 'muted', text: 'A chain beats one agent only if the extra stages raise the later stages\' success above what one agent achieves alone. ' +
      'Planner ' + P.planner.p.toFixed(2) + ' × worker ' + P.worker.p.toFixed(2) + ' = ' + (P.planner.p * P.worker.p).toFixed(2) + ' vs one agent ' + P.single.p.toFixed(2) + '.' }));

    var a = X.a, R = X.loopR;
    var exp = a > 0 ? (1 - Math.pow(1 - a, R)) / a : R;
    var unc = a > 0 ? 1 / a : Infinity;
    var byBudget = X.budget >= X.round1 ? 1 + Math.floor((X.budget - X.round1 + 1e-9) / X.roundN) : 0;
    box.appendChild(el('h3', { text: 'Review-loop rounds: E[rounds] = (1 − (1 − a)^R) / a' }));
    box.appendChild(el('div', { class: 'stats' }, [
      stat('Expected rounds (cap ' + R + ')', exp.toFixed(2)),
      stat('Uncapped', isFinite(unc) ? unc.toFixed(2) : '∞ — never ends', isFinite(unc) ? '' : 'bad'),
      stat('Rounds the budget allows', String(byBudget), '$' + X.round1.toFixed(2) + ' first round, $' + X.roundN.toFixed(2) + ' each further (T18)'),
      stat('Stop that fires first', a === 0 ? (R <= byBudget ? 'round cap' : 'budget') : (R <= byBudget ? 'round cap' : 'budget'), a === 0 ? 'a reviewer that never approves' : '')
    ]));
    box.appendChild(el('p', { class: 'muted', text: 'Per call the reviewer is cheap; the loop is not. Its cost is set by a, which you do not control directly. Keep three stops: a round cap, a hard budget, and an oscillation rule that escalates to a person.' }));
  }

  var simCache = null;
  function renderReview(box) {
    var e = estimate(P);
    box.appendChild(el('h2', { text: TASKS[task].label + ': single agent vs planner → worker → reviewer' }));
    box.appendChild(table(['Design', 'Success', 'Exp. cost', 'Exp. latency', 'Cost / success'], [
      { cls: 'base', cells: ['single agent', Sim.fix(e.single.p, 3), money(e.single.cost), secs(e.single.time), money(e.single.cost / e.single.p)] },
      { cells: ['planner → worker', Sim.fix(e.chain.p, 3), money(e.chain.cost), secs(e.chain.time), money(e.chain.cost / e.chain.p)] },
      { cells: ['planner → worker → reviewer (max ' + P.R + ')', Sim.fix(e.pwr.p, 3), money(e.pwr.cost), secs(e.pwr.time), money(e.pwr.cost / e.pwr.p)] }
    ]));
    var d = (e.pwr.p - e.single.p) * 100;
    box.appendChild(el('p', {}, [
      el('strong', { class: d >= 0 ? 'good' : 'bad', text: 'Pipeline vs single agent: ' + (d >= 0 ? '+' : '−') + Math.abs(d).toFixed(1) + ' pts, cost ×' + (e.pwr.cost / e.single.cost).toFixed(2) + ', latency ×' + (e.pwr.time / e.single.time).toFixed(2) + '.' }),
      el('span', { class: 'muted', text: ' Expected rounds ' + e.pwr.rounds.toFixed(2) + '; P(stopped at max rounds) ' + e.pwr.atMax.toFixed(3) + '; P(reviewer approved a wrong result) ' + e.pwr.escapedWrong.toFixed(3) + '.' })
    ]));

    var p = P.worker.p, r = P.reviewer.r, f = P.reviewer.f, dd = P.worker.dmg, pf = P.worker.pfix;
    var rescue = (1 - p) * r * pf, damage = p * f * dd;
    box.appendChild(el('h3', { text: 'One review round: rescues minus damage' }));
    box.appendChild(el('div', { class: 'stats' }, [
      stat('Rescues (1−p)·r·p_fix', '+' + (rescue * 100).toFixed(1) + ' pts', 'good'),
      stat('Damage p·f·d', '−' + (damage * 100).toFixed(1) + ' pts', 'bad'),
      stat('Net', ((rescue - damage) >= 0 ? '+' : '−') + Math.abs((rescue - damage) * 100).toFixed(1) + ' pts', rescue >= damage ? 'good' : 'bad', 'P₁ = ' + (p * (1 - f) + p * f * (1 - dd) + (1 - p) * r * pf).toFixed(3) + ' from p = ' + p.toFixed(2))
    ]));

    var runs = simulate(X.n, seed);
    simCache = runs;
    var passes = runs.filter(function (x) { return x.pass; }).length;
    var wi = Sim.wilson(passes, runs.length);
    var calls = { planner: 0, worker: 0, reviewer: 0 }, costBy = { planner: 0, worker: 0, reviewer: 0 };
    runs.forEach(function (x) { x.spans.forEach(function (s) { calls[s.role]++; costBy[s.role] += s.cost; }); });
    var changes = runs.reduce(function (s, x) { return s + x.spans.filter(function (y) { return y.verdict === 'CHANGES'; }).length; }, 0);
    var totalCost = costBy.planner + costBy.worker + costBy.reviewer;
    var costs = runs.map(function (x) { return x.cost; }), walls = runs.map(function (x) { return x.wall; });

    box.appendChild(el('h3', { text: 'Simulated runs: ' + runs.length + ' tasks (seed ' + seed + ')' }));
    box.appendChild(el('div', { class: 'controls' }, [el('button', { type: 'button', class: 'primary', text: 'Re-run (new seed)', onclick: function () { seed = seed * 31 % 99991 + 7; render(); } })]));
    box.appendChild(flow(calls, changes, runs));
    box.appendChild(el('div', { class: 'stats' }, [
      stat('Observed success', passes + '/' + runs.length + ' = ' + Sim.pct(passes / runs.length), '', '95% Wilson [' + Sim.pct(wi[0]) + ', ' + Sim.pct(wi[1]) + '] · expected ' + Sim.pct(e.pwr.p, 1)),
      stat('Cost per trace', 'median ' + money(quantile(costs, 0.5)), '', 'p90 ' + money(quantile(costs, 0.9)) + ' · max ' + money(Math.max.apply(null, costs))),
      stat('Wall-clock', 'median ' + secs(quantile(walls, 0.5)), '', 'p90 ' + secs(quantile(walls, 0.9))),
      stat('Reviewer approved a failing result', String(runs.filter(function (x) { return x.approvedWrong; }).length), 'bad', 'rework after a finding ended in a pass: ' + runs.filter(function (x) { return x.reworkPass; }).length)
    ]));
    var stops = [['approved in round 1', function (x) { return x.stop === 'approved' && x.rounds === 1; }],
      ['approved after rework', function (x) { return x.stop === 'approved' && x.rounds > 1; }],
      ['stopped at max rounds', function (x) { return x.stop === 'max_rounds'; }]];
    box.appendChild(table(['Stop reason', 'Traces', 'Pass', 'Fail'], stops.map(function (s) {
      var g = runs.filter(s[1]); var ps = g.filter(function (x) { return x.pass; }).length;
      return { cells: [s[0], String(g.length), String(ps), String(g.length - ps)] };
    })));
    box.appendChild(table(['Role', 'Calls', 'Share of cost'], ['worker', 'reviewer', 'planner'].map(function (r) {
      return { cells: [r, String(calls[r]), Sim.pct(costBy[r] / Math.max(totalCost, 1e-9))] };
    })));

    var sel = el('select', { id: 'trace-pick' });
    runs.slice(0, Math.min(runs.length, 200)).forEach(function (x) {
      sel.appendChild(el('option', { value: String(x.i - 1), text: 'task ' + x.i + ' · ' + x.stop + ' · ' + (x.pass ? 'pass' : 'fail') + ' · ' + x.rounds + ' round(s)' }));
    });
    traceIdx = Math.min(traceIdx, runs.length - 1);
    sel.value = String(traceIdx);
    var tbox = el('div');
    sel.addEventListener('change', function () { traceIdx = parseInt(sel.value, 10); drawTrace(tbox, runs[traceIdx]); });
    box.appendChild(el('h3', { text: 'Read one trace' }));
    box.appendChild(el('div', { class: 'controls' }, [el('label', { for: 'trace-pick', text: 'Trace' }), sel,
      el('button', { type: 'button', text: 'Next failing trace', onclick: function () {
        for (var j = 1; j <= runs.length; j++) { var c = (traceIdx + j) % runs.length; if (!runs[c].pass) { traceIdx = c; sel.value = String(c); drawTrace(tbox, runs[c]); return; } }
      } })]));
    box.appendChild(tbox);
    drawTrace(tbox, runs[traceIdx]);
  }

  function drawTrace(box, x) {
    Sim.clear(box);
    box.appendChild(el('p', { class: 'mono small', text: 'task ' + x.i + '  stop: ' + x.stop + '  rounds: ' + x.rounds + '  calls: ' + x.spans.length + '  cost: ' + money(x.cost) + '  wall: ' + secs(x.wall) + '  outcome: ' + (x.pass ? 'pass' : 'fail') }));
    box.appendChild(table(['start s', 'dur s', 'span', 'round', 'cost $', 'verdict / detail'], x.spans.map(function (s) {
      return { cls: s.verdict === 'CHANGES' ? 'warnrow' : '', cells: [String(Math.round(s.start)), String(Math.round(s.dur)), 'invoke_agent ' + s.role, String(s.round), s.cost.toFixed(4), s.verdict] };
    }), 0));
  }

  /* Message-flow diagram with call counts from the simulated runs. */
  function flow(calls, changes, runs) {
    var ns = SVG_NS();
    var svg = document.createElementNS(ns, 'svg');
    svg.setAttribute('viewBox', '0 0 640 170');
    svg.setAttribute('role', 'img');
    var approved = runs.filter(function (x) { return x.stop === 'approved'; }).length, maxed = runs.length - approved;
    svg.setAttribute('aria-label', 'Message flow: planner ' + calls.planner + ' calls, worker ' + calls.worker + ', reviewer ' + calls.reviewer + ', changes requested ' + changes + ' times, approved ' + approved + ', stopped at max rounds ' + maxed);
    function add(tag, attrs, text) { var n = document.createElementNS(ns, tag); Object.keys(attrs).forEach(function (k) { n.setAttribute(k, attrs[k]); }); if (text !== undefined) n.textContent = text; svg.appendChild(n); return n; }
    var defs = add('defs', {});
    var m = document.createElementNS(ns, 'marker');
    [['id', 'arr'], ['viewBox', '0 0 10 10'], ['refX', '9'], ['refY', '5'], ['markerWidth', '7'], ['markerHeight', '7'], ['orient', 'auto-start-reverse']].forEach(function (a) { m.setAttribute(a[0], a[1]); });
    var mp = document.createElementNS(ns, 'path'); mp.setAttribute('d', 'M0,0 L10,5 L0,10 z'); mp.setAttribute('class', 'arrowhead'); m.appendChild(mp); defs.appendChild(m);
    var boxes = [['planner', 20], ['worker', 190], ['reviewer', 360]];
    boxes.forEach(function (b) {
      add('rect', { x: b[1], y: 60, width: 120, height: 50, rx: 8, class: 'fnode' });
      add('text', { x: b[1] + 60, y: 82, 'text-anchor': 'middle', class: 'fl' }, b[0]);
      add('text', { x: b[1] + 60, y: 99, 'text-anchor': 'middle', class: 'fs' }, calls[b[0]] + ' calls');
    });
    add('line', { x1: 140, y1: 85, x2: 188, y2: 85, class: 'fedge', 'marker-end': 'url(#arr)' });
    add('line', { x1: 310, y1: 85, x2: 358, y2: 85, class: 'fedge', 'marker-end': 'url(#arr)' });
    add('path', { d: 'M420,110 C420,150 250,150 250,112', class: 'fedge back', 'marker-end': 'url(#arr)', fill: 'none' });
    add('text', { x: 335, y: 160, 'text-anchor': 'middle', class: 'fs' }, 'changes requested × ' + changes + ' (replan + rework)');
    add('rect', { x: 530, y: 30, width: 100, height: 40, rx: 8, class: 'fnode ok' });
    add('text', { x: 580, y: 55, 'text-anchor': 'middle', class: 'fs' }, 'approved ' + approved);
    add('rect', { x: 530, y: 100, width: 100, height: 40, rx: 8, class: 'fnode stop' });
    add('text', { x: 580, y: 125, 'text-anchor': 'middle', class: 'fs' }, 'max rounds ' + maxed);
    add('line', { x1: 480, y1: 78, x2: 528, y2: 55, class: 'fedge', 'marker-end': 'url(#arr)' });
    add('line', { x1: 480, y1: 95, x2: 528, y2: 118, class: 'fedge', 'marker-end': 'url(#arr)' });
    add('text', { x: 20, y: 30, class: 'fs' }, runs.length + ' tasks in');
    return svg;
  }

  function render() {
    var box = Sim.clear($('#view'));
    if (view === 'topologies') renderTopologies(box);
    else if (view === 'compounding') renderCompounding(box);
    else renderReview(box);
  }

  function init() {
    var tabs = $('#presets');
    ORDER.forEach(function (k) {
      tabs.appendChild(el('button', { type: 'button', 'data-preset': k, 'aria-pressed': 'false', text: PRESETS[k].label, onclick: function () { setView(k, true); } }));
    });
    $('#task').addEventListener('change', function () { loadTask(this.value); buildParams(); render(); });
    $('#reset').addEventListener('click', function () { loadTask(task); X = clone(EXTRA); buildParams(); render(); });
    seed = Sim.seedFromQuery(10);
    var pre = Sim.preset(ORDER, 'review-loop');
    var q = Sim.params().task;
    setView(pre, true);
    if (q === 'qa' || q === 'code') { loadTask(q); buildParams(); render(); }
  }

  // Exposed for headless checks only.
  window.MultiAgentSim = { estimate: estimate, TASKS: TASKS, _load: function (t) { loadTask.call(null, t); }, _P: function () { return P; },
    _topo: function (t) { var src = TASKS[t]; P = { single: clone(src.single), planner: clone(src.planner), worker: clone(src.worker), reviewer: clone(src.reviewer), R: src.R }; X = clone(EXTRA); return topologies(); },
    _sim: function (t, n, s) { var src = TASKS[t]; P = { single: clone(src.single), planner: clone(src.planner), worker: clone(src.worker), reviewer: clone(src.reviewer), R: src.R }; X = clone(EXTRA); return simulate(n, s); } };

  if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', init); else init();
})();
