/* Context-budget simulation (Module 4, lessons 04.1 and 04.3). Classic script; depends on ../common/sim.js. */
(function () {
  'use strict';
  var S = window.Sim;
  var el = S.el;

  var PRESETS = ['bloated-rules', 'irrelevant-docs'];
  var NOTES = {
    'bloated-rules': ['Irrelevant material vs usable budget and fact recall (lesson 04.1)',
      'The lab’s bloated layer: 807 always-loaded lines, 7,028 tokens — only 3.5% of a 200k window, leaving B = 167,772 at turn 1. Judged by window share you would never fix it. Now watch the other two numbers: recall of the one line that matters (line 24 of .claude/rules/sql.md) and the monthly cost of re-sending R. Press “Turn 30” to see H eat the window, then switch to the grounded layer.'],
    'irrelevant-docs': ['Irrelevant documents vs recall of the facts that matter (lesson 04.3)',
      'The grounded layer plus eight true but irrelevant documents pasted in (a Kubernetes guide, a React style guide, a coffee-machine policy…). They barely dent the window, yet recall drops from the first distractor on, the agent starts proposing tools that do not exist here, and facts buried in the middle suffer most. Drag the document count to 0 and back, and move the facts to the middle or the end.']
  };
  var preset = S.preset(PRESETS, 'bloated-rules');

  var FILES = [
    { id: 'claude', name: 'CLAUDE.md', note: 'root instructions', lines: 306, tokens: 2785, irr: 2300, facts: ['dapper', 'money'] },
    { id: 'arch', name: 'docs/ai/architecture-full.md', note: '@import in CLAUDE.md', lines: 164, tokens: 1410, irr: 1410, facts: [] },
    { id: 'coding', name: 'docs/ai/coding-standards.md', note: '@import in CLAUDE.md', lines: 205, tokens: 1729, irr: 1729, facts: [] },
    { id: 'handbook', name: 'docs/ai/team-handbook.md', note: '@import in CLAUDE.md', lines: 96, tokens: 726, irr: 726, facts: [] },
    { id: 'sql', name: '.claude/rules/sql.md', note: 'rule without paths:', lines: 36, tokens: 378, irr: 150, facts: ['undo', 'money'] }
  ];
  var GROUNDED = { lines: 34, tokens: 531, irr: 0 };
  var FACTS = [
    { id: 'undo', text: 'Every V### migration gets a U### undo script', bloated: '.claude/rules/sql.md, line 24' },
    { id: 'dapper', text: 'New data access uses Dapper; SqlHelper only in Legacy/', bloated: 'CLAUDE.md' },
    { id: 'money', text: 'Money is decimal(19,4) in SQL Server and decimal in C#', bloated: 'CLAUDE.md and .claude/rules/sql.md (said twice)' }
  ];
  var DOC_TOKENS = 700;
  var DOCS = ['Kubernetes + Helm deployment guide', 'React component style guide', 'npm package release handbook',
    'Office coffee-machine policy', 'Java logging standard', 'iOS app onboarding', 'Terraform module conventions',
    'Python notebook etiquette', 'Figma naming guide', 'Go service template', 'Android release checklist',
    'Data-science glossary', 'Kafka topic naming', 'Vue migration notes', 'Travel expense policy',
    'Rust crate guidelines', 'Marketing tone of voice', 'Spark job tuning', 'Angular testing guide', 'Holiday calendar'];
  var RUNS = 20;
  var MSG = 100;

  var $ = S.$;
  var ui = {
    W: $('#window'), layer: $('#layer'), files: $('#files'), docs: $('#docs'), pos: $('#pos'), hist: $('#hist'),
    S: $('#S'), T: $('#T'), O: $('#O'), n: $('#n'), sessions: $('#sessions'), price: $('#price'), xaxis: $('#xaxis')
  };
  var seed = S.seedFromQuery(404);
  var fileBoxes = {};

  FILES.forEach(function (f) {
    var cb = el('input', { type: 'checkbox', id: 'f-' + f.id, checked: true });
    fileBoxes[f.id] = cb;
    ui.files.appendChild(el('label', { class: 'check' }, [cb, el('span', {}, [el('code', { text: f.name }), el('small', { text: f.note + ' · ' + f.lines + ' lines · ' + S.int(f.tokens) + ' tokens' })])]));
    cb.addEventListener('change', update);
  });

  function num(inp, d) { var v = parseFloat(inp.value); return isFinite(v) && v >= 0 ? v : d; }

  /* ---------- the model ---------- */
  function config(over) {
    var c = {
      W: +ui.W.value, layer: ui.layer.value, docs: +ui.docs.value, pos: ui.pos.value, H: +ui.hist.value,
      S: num(ui.S, 4200), T: num(ui.T, 1000), O: num(ui.O, 20000),
      files: {}
    };
    FILES.forEach(function (f) { c.files[f.id] = fileBoxes[f.id].checked; });
    if (over) Object.keys(over).forEach(function (k) { c[k] = over[k]; });
    return c;
  }

  function derive(c) {
    var R = 0, irr = 0, lines = 0, present = {};
    if (c.layer === 'grounded') {
      R = GROUNDED.tokens; irr = GROUNDED.irr; lines = GROUNDED.lines;
      FACTS.forEach(function (f) { present[f.id] = true; });
    } else {
      FILES.forEach(function (f) {
        if (!c.files[f.id]) return;
        R += f.tokens; irr += f.irr; lines += f.lines;
        f.facts.forEach(function (id) { present[id] = true; });
      });
    }
    var D = c.docs * DOC_TOKENS;
    var Rtot = R + D, irrTot = irr + D;
    var L = c.S + c.T + Rtot + c.H + MSG;
    var off = { start: 0.05, middle: 0.5, end: 0.95 }[c.pos];
    var span = Rtot + c.H + MSG; // the material the model reads after its own instructions
    var depth = off * Rtot / span;
    var B = c.W - c.S - Rtot - c.T - c.H - c.O;
    var recall = {};
    FACTS.forEach(function (f) {
      if (!present[f.id]) { recall[f.id] = 0; return; }
      var distraction = (irrTot > 0 ? 0.95 : 1) * Math.exp(-irrTot / 40000);
      var rot = Math.exp(-L / 800000);
      var middle = 1 - 0.35 * Math.sin(Math.PI * depth) * Math.min(1, span / 15000);
      recall[f.id] = 0.97 * distraction * rot * middle;
    });
    var offTopic = 0.02 + 0.5 * (1 - Math.exp(-irrTot / 15000));
    var avg = S.mean(FACTS.map(function (f) { return recall[f.id]; }));
    return { R: R, Rirr: irr, lines: lines, D: D, Rtot: Rtot, irrTot: irrTot, L: L, depth: depth, B: B, recall: recall, present: present, offTopic: offTopic, avg: avg };
  }

  /* ---------- rendering ---------- */
  function update() {
    ui.files.disabled = ui.layer.value !== 'bloated';
    $('#docs-out').textContent = ui.docs.value + ' × ' + DOC_TOKENS + ' tokens';
    $('#docs-list').textContent = +ui.docs.value ? DOCS.slice(0, +ui.docs.value).join(' · ') : 'None.';
    $('#hist-out').textContent = S.int(+ui.hist.value);
    $('#seed').textContent = String(seed);
    var c = config(), d = derive(c);
    renderBar(c, d);
    renderFacts(c, d);
    renderChart(c, d);
    renderCost(c, d);
  }

  function renderBar(c, d) {
    var parts = [
      ['S', 'System prompt S', c.S],
      ['T', 'Tool definitions T', c.T],
      ['Rrel', 'R: rules that matter', d.R - d.Rirr],
      ['Rirr', 'R: irrelevant rules', d.Rirr],
      ['D', 'R: imported irrelevant docs', d.D],
      ['H', 'History H', c.H],
      ['O', 'Reserved output O', c.O],
      ['B', 'Left for the task B', Math.max(0, d.B)]
    ];
    var total = Math.max(c.W, parts.reduce(function (a, p) { return a + p[2]; }, 0));
    var bar = S.clear($('#bar')), leg = S.clear($('#legend'));
    bar.setAttribute('aria-label', parts.map(function (p) { return p[1] + ' ' + S.int(p[2]); }).join(', '));
    parts.forEach(function (p) {
      if (p[2] <= 0) return;
      var w = 100 * p[2] / total;
      bar.appendChild(el('span', { class: 't-' + p[0], style: 'width:' + w + '%;min-width:2px', title: p[1] + ': ' + S.int(p[2]) }));
      leg.appendChild(el('li', {}, [el('i', { class: 'sw t-' + p[0] }), p[1] + ' ' + S.int(p[2])]));
    });
    $('#equation').textContent = 'B = W − S − R − T − H − O = ' + S.int(c.W) + ' − ' + S.int(c.S) + ' − ' + S.int(d.Rtot) +
      ' − ' + S.int(c.T) + ' − ' + S.int(c.H) + ' − ' + S.int(c.O) + ' = ' + S.int(d.B);
    var st = S.clear($('#budget-stats'));
    [['R always loaded', S.int(d.Rtot) + ' tokens'],
      ['R share of W', S.pct(d.Rtot / c.W, 1)],
      ['B left for the task', S.int(d.B)],
      ['B share of W', S.pct(d.B / c.W, 1)]].forEach(function (r) {
      st.appendChild(el('div', {}, [el('div', { class: 'k', text: r[0] }), el('div', { class: 'stat' + (r[0].indexOf('B') === 0 && d.B < 0 ? ' bad' : ''), text: r[1] })]));
    });
    var note;
    if (d.B < 0) note = 'Over the window: the harness must compact or drop history, or the request fails.';
    else if (c.H >= 60000) note = 'At this point in the session H is the term eating the window; R is ' + S.pct(d.Rtot / c.W, 1) + ' of W but is still re-sent on every request and still competes for attention. B ≈ ' + Math.max(0, Math.round(d.B / 8000)) + ' medium C# files.';
    else if (d.irrTot > 0) note = 'R is only ' + S.pct(d.Rtot / c.W, 1) + ' of W. Judged by window share you would never fix it — which is why the module measures recall and pass rate, not just tokens.';
    else note = 'Every always-loaded line is about this repository and this work. R is ' + S.pct(d.Rtot / c.W, 1) + ' of W.';
    if (c.layer === 'bloated') note += ' Layer: ' + d.lines + ' always-loaded lines' + (d.lines > 300 ? ' (course cap 300).' : '.');
    $('#budget-note').textContent = note;
  }

  function renderFacts(c, d) {
    var rng = S.rng(seed * 7919 + c.docs * 31 + Math.round(c.H / 1000) + (c.layer === 'grounded' ? 5 : 0) + ({ start: 1, middle: 2, end: 3 })[c.pos]);
    var box = S.clear($('#facts'));
    FACTS.forEach(function (f) {
      var p = d.recall[f.id];
      var hits = 0, dots = el('div', { class: 'dots', 'aria-hidden': 'true' });
      for (var i = 0; i < RUNS; i++) { var h = rng.bern(p); if (h) hits++; dots.appendChild(el('i', { class: h ? 'hit' : 'miss' })); }
      var src = !d.present[f.id] ? 'Not in context: the file that holds it is not loaded (insufficient context).'
        : c.layer === 'grounded' ? 'In the grounded layer' : 'In ' + f.bloated;
      box.appendChild(el('div', { class: 'fact' }, [
        el('div', { class: 'row' }, [el('strong', { text: f.text }), el('span', { class: 'mono', text: S.pct(p) + ' · used in ' + hits + '/' + RUNS + ' runs' })]),
        el('div', { class: 'src', text: src }),
        el('div', { class: 'meter' }, [el('span', { style: 'width:' + (100 * p) + '%' })]),
        dots
      ]));
    });
    var offHits = 0, offDots = el('div', { class: 'dots', 'aria-hidden': 'true' });
    for (var j = 0; j < RUNS; j++) { var o = rng.bern(d.offTopic); if (o) offHits++; offDots.appendChild(el('i', { class: o ? 'off' : 'hit' })); }
    box.appendChild(el('div', { class: 'fact' }, [
      el('div', { class: 'row' }, [el('strong', { text: 'Symptom: proposes a technology that does not exist here (npm, Helm)' }), el('span', { class: 'mono', text: S.pct(d.offTopic) + ' · ' + offHits + '/' + RUNS + ' runs' })]),
      el('div', { class: 'src', text: 'Driven by irrelevant tokens in the layer: ' + S.int(d.irrTot) + '.' }),
      offDots
    ]));
  }

  function renderChart(c, d) {
    var x = ui.xaxis.value;
    var series, xs, xLabel, cur;
    if (x === 'hist') {
      xs = []; for (var h = 0; h <= 180000; h += 10000) xs.push(h);
      xLabel = function (v) { return Math.round(v / 1000) + 'k'; };
      series = [
        { cls: 'l1', name: 'Current setup', f: function (v) { return derive(config({ H: v })).avg; } },
        { cls: 'l2', name: 'Grounded layer, no irrelevant docs', f: function (v) { return derive(config({ H: v, layer: 'grounded', docs: 0 })).avg; } }
      ];
      cur = c.H;
      $('#chart-title').textContent = 'Expected recall vs history H';
    } else {
      xs = []; for (var k = 0; k <= 20; k++) xs.push(k);
      xLabel = function (v) { return String(v); };
      series = ['start', 'middle', 'end'].map(function (p, i) {
        return { cls: 'l' + (i + 1), name: 'Facts ' + ({ start: 'at the top', middle: 'in the middle', end: 'at the end' })[p], f: function (v) { return derive(config({ docs: v, pos: p })).avg; } };
      });
      cur = c.docs;
      $('#chart-title').textContent = 'Expected recall vs irrelevant documents';
    }
    var Wd = Math.max(300, Math.min(760, $('#chart').clientWidth || 640)), Ht = 220, pl = 40, pr = 10, pt = 10, pb = 28;
    var xmax = xs[xs.length - 1];
    var X = function (v) { return pl + (Wd - pl - pr) * v / xmax; };
    var Y = function (v) { return pt + (Ht - pt - pb) * (1 - v); };
    var ns = 'http://www.w3.org/2000/svg';
    var svg = document.createElementNS(ns, 'svg');
    svg.setAttribute('viewBox', '0 0 ' + Wd + ' ' + Ht);
    svg.setAttribute('role', 'img');
    function add(tag, a, text) { var e = document.createElementNS(ns, tag); Object.keys(a).forEach(function (k) { e.setAttribute(k, a[k]); }); if (text !== undefined) e.textContent = text; svg.appendChild(e); return e; }
    [0, 0.25, 0.5, 0.75, 1].forEach(function (v) {
      add('line', { x1: pl, x2: Wd - pr, y1: Y(v), y2: Y(v), class: 'grid-line' });
      add('text', { x: pl - 4, y: Y(v) + 4, 'text-anchor': 'end' }, Math.round(v * 100) + '%');
    });
    [0, 0.5, 1].forEach(function (f) { var v = xs[Math.round(f * (xs.length - 1))]; add('text', { x: X(v), y: Ht - 8, 'text-anchor': f === 0 ? 'start' : f === 1 ? 'end' : 'middle' }, xLabel(v)); });
    var desc = [];
    series.forEach(function (s) {
      var pts = xs.map(function (v) { return X(v) + ',' + Y(s.f(v)); }).join(' ');
      add('polyline', { points: pts, class: s.cls });
      desc.push(s.name + ': ' + S.pct(s.f(xs[0])) + ' at ' + xLabel(xs[0]) + ' to ' + S.pct(s.f(xmax)) + ' at ' + xLabel(xmax));
    });
    add('circle', { cx: X(cur), cy: Y(d.avg), r: 5, class: 'now' });
    svg.setAttribute('aria-label', 'Average recall of the three facts. ' + desc.join('; ') + '. Current point ' + S.pct(d.avg) + '.');
    S.clear($('#chart')).appendChild(svg);
    var leg = S.clear($('#chart-legend'));
    series.forEach(function (s) { leg.appendChild(el('li', {}, [el('i', { class: 'sw', style: 'background:var(--c' + s.cls.slice(1) + ')' }), s.name])); });
    leg.appendChild(el('li', {}, [el('i', { class: 'sw', style: 'background:var(--ink);border-radius:50%' }), 'Current: ' + S.pct(d.avg)]));
  }

  function renderCost(c, d) {
    var n = Math.max(1, num(ui.n, 10)), sess = num(ui.sessions, 1440), price = num(ui.price, 3);
    function cost(R) {
      return { tokens: R * n * sess, unc: R * n * sess * price / 1e6, cached: price * sess * (1.25 * R + 0.1 * R * (n - 1)) / 1e6 };
    }
    var cur = cost(d.Rtot), ref = cost(GROUNDED.tokens);
    var st = S.clear($('#cost-stats'));
    [['R re-sent per month', (cur.tokens / 1e6).toFixed(1) + 'M tokens'],
      ['Uncached', S.money(cur.unc) + '/month'],
      ['Warm cache', S.money(cur.cached) + '/month'],
      ['Grounded layer (531)', S.money(ref.unc) + ' / ' + S.money(ref.cached)]].forEach(function (r) {
      st.appendChild(el('div', {}, [el('div', { class: 'k', text: r[0] }), el('div', { class: 'stat', text: r[1] })]));
    });
  }

  /* ---------- events and preset ---------- */
  [ui.W, ui.layer, ui.docs, ui.pos, ui.hist, ui.S, ui.T, ui.O, ui.n, ui.sessions, ui.price, ui.xaxis].forEach(function (x) { x.addEventListener('input', update); });
  $('#turn1').addEventListener('click', function () { ui.hist.value = 0; update(); });
  $('#turn30').addEventListener('click', function () { ui.hist.value = 120000; update(); });
  $('#rerun').addEventListener('click', function () { seed = (seed * 1103515245 + 12345) % 2147483647 || 7; update(); });

  var lastW = window.innerWidth, rt = null;
  window.addEventListener('resize', function () {
    if (window.innerWidth === lastW) return;
    lastW = window.innerWidth; clearTimeout(rt); rt = setTimeout(update, 150);
  });

  var note = NOTES[preset];
  $('#preset-note').appendChild(el('strong', { text: 'Preset: ' + note[0] }));
  $('#preset-note').appendChild(document.createTextNode(note[1]));
  if (preset === 'irrelevant-docs') {
    ui.layer.value = 'grounded'; ui.docs.value = 8; ui.pos.value = 'middle'; ui.xaxis.value = 'docs';
  } else {
    ui.layer.value = 'bloated'; ui.docs.value = 0; ui.pos.value = 'end'; ui.xaxis.value = 'hist';
  }
  update();
})();
