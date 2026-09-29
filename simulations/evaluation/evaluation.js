/* Evaluation simulation (Module 7, lessons 07.3, 07.4 and 07.5). Classic script; depends on ../common/sim.js. */
(function () {
  'use strict';
  var S = window.Sim;
  var el = S.el, $ = S.$;
  var NS = 'http://www.w3.org/2000/svg';

  var PRESETS = ['biased-judge', 'one-run', 'paired'];
  var NOTES = {
    'biased-judge': ['A biased judge distorts A vs B (lesson 07.3)',
      'System A truly passes 80%, system B 30%. Judge v1 (“thorough, detailed”) is close to a length detector: on the calibration set TPR = 0.583 and FPR = 0.500, so it reports about 57% and 53% for systems fifty points apart. Switch to judge v2 (TPR 0.917, FPR 0.083): about 75% and 33%. Then press “B becomes verbose” and watch v1 rank the worse system first.'],
    'one-run': ['Trials vs interval width (lesson 07.4)',
      'The 07.1 “one run”: 14/20 has a 95% Wilson interval of 48%–85% — not wrong, just nearly uninformative. Run the same eval again and again to see how much k moves, change the number of trials to see the width shrink (20 trials ≈ ±18 points), and use the task × trial panel to see why past 3–5 trials per task you add tasks, not trials.'],
    'paired': ['Paired vs unpaired comparison (lesson 07.5)',
      'Both arms run the same 20 tasks × 5 trials and B has no real effect, like the illustrative skill v1 vs v2 comparison (−1.0 point; unpaired by task −19.7 to +17.7, paired −11.3 to +9.3). Tasks differ far more from each other than the arms do; pairing each task with itself cancels that. Re-run a few times, then run 200 experiments to compare the average widths.']
  };
  var preset = S.preset(PRESETS, 'biased-judge');

  /* ---------- small helpers ---------- */
  function svgEl(w, h, label) {
    var svg = document.createElementNS(NS, 'svg');
    svg.setAttribute('viewBox', '0 0 ' + w + ' ' + h);
    svg.setAttribute('role', 'img');
    svg.setAttribute('aria-label', label);
    svg.add = function (tag, a, text) {
      var e = document.createElementNS(NS, tag);
      Object.keys(a).forEach(function (k) { e.setAttribute(k, a[k]); });
      if (text !== undefined) e.textContent = text;
      svg.appendChild(e);
      return e;
    };
    return svg;
  }
  function bindRange(id, fmt, cb) {
    var inp = $('#' + id), out = $('#' + id + '-out');
    var show = function () { out.textContent = fmt(+inp.value); };
    inp.addEventListener('input', function () { show(); cb(); });
    show();
    return inp;
  }
  function setRange(inp, v) { inp.value = v; inp.dispatchEvent(new Event('input')); }
  function nextSeed(s) { return (s * 1103515245 + 12345) % 2147483647 || 7; }
  function binom(rng, n, p) { var k = 0; for (var i = 0; i < n; i++) if (rng() < p) k++; return k; }
  function pts(x) { return (x >= 0 ? '+' : '−') + Math.abs(x * 100).toFixed(1) + ' pts'; }
  function pct0(x) { return Math.round(x * 100) + '%'; }
  function stats(box, rows) {
    S.clear(box);
    rows.forEach(function (r) { box.appendChild(el('div', {}, [el('div', { class: 'k', text: r[0] }), el('div', { class: 'stat ' + (r[2] || ''), text: r[1] })])); });
  }
  function table(tbl, head, rows) {
    S.clear(tbl);
    tbl.appendChild(el('thead', {}, [el('tr', {}, head.map(function (h, i) { return el('th', { class: i ? 'num' : '', text: h }); }))]));
    var tb = el('tbody');
    rows.forEach(function (r) { tb.appendChild(el('tr', {}, r.map(function (c, i) { return el('td', { class: i ? 'num' : '', text: c }); }))); });
    tbl.appendChild(tb);
  }

  /* Horizontal interval chart. rows: {label, lo, hi, pt, cls, mark} */
  function ivChart(box, rows, o) {
    var W = Math.max(300, Math.min(760, box.clientWidth || 640)), lw = Math.min(160, Math.round(W * 0.42)), rh = 28, top = 8, H = top + rows.length * rh + 26;
    var X = function (v) { return lw + (W - lw - 14) * (v - o.min) / (o.max - o.min); };
    var svg = svgEl(W, H, o.label);
    o.ticks.forEach(function (t) {
      svg.add('line', { x1: X(t), x2: X(t), y1: top, y2: H - 22, class: 'grid-line' });
      svg.add('text', { x: X(t), y: H - 8, 'text-anchor': 'middle' }, o.fmt(t));
    });
    if (o.zero !== undefined) svg.add('line', { x1: X(o.zero), x2: X(o.zero), y1: top, y2: H - 22, class: 'zero' });
    rows.forEach(function (r, i) {
      var y = top + i * rh + rh / 2;
      svg.add('text', { x: lw - 8, y: y + 4, 'text-anchor': 'end' }, r.label);
      svg.add('line', { x1: X(Math.max(o.min, r.lo)), x2: X(Math.min(o.max, r.hi)), y1: y, y2: y, class: 'iv ' + (r.cls || 'a') });
      if (r.pt !== undefined) svg.add('circle', { cx: X(r.pt), cy: y, r: 4, class: 'pt' });
      if (r.mark !== undefined) svg.add('line', { x1: X(r.mark), x2: X(r.mark), y1: y - 10, y2: y + 10, class: 'truth' });
    });
    if (o.truth !== undefined) svg.add('line', { x1: X(o.truth), x2: X(o.truth), y1: top, y2: H - 22, class: 'truth' });
    S.clear(box).appendChild(svg);
  }

  /* =================== 07.3 Biased judge =================== */
  var JUDGES = {
    v1: { rl: 1, rs: 1 / 6, wl: 1, ws: 0 },
    v2: { rl: 1, rs: 5 / 6, wl: 1 / 6, ws: 0 },
    human: { rl: 1, rs: 1, wl: 0, ws: 0 }
  };
  var jSeed = S.seedFromQuery(73);
  var J = {};
  ['rl', 'rs', 'wl', 'ws'].forEach(function (k) { J[k] = bindRange('j-' + k, function (v) { return v.toFixed(2); }, updateJudge); });
  var sys = {
    ap: bindRange('a-p', pct0, updateJudge), al: bindRange('a-l', pct0, updateJudge),
    bp: bindRange('b-p', pct0, updateJudge), bl: bindRange('b-l', pct0, updateJudge),
    n: bindRange('j-n', String, updateJudge)
  };
  var judgeSel = $('#judge');
  function loadJudge() {
    var j = JUDGES[judgeSel.value];
    $('#judge-probs').disabled = !j;
    if (j) ['rl', 'rs', 'wl', 'ws'].forEach(function (k) { J[k].value = j[k]; $('#j-' + k + '-out').textContent = j[k].toFixed(2); });
    updateJudge();
  }
  judgeSel.addEventListener('input', loadJudge);
  $('#verbose-b').addEventListener('click', function () { setRange(sys.bl, 0.9); });
  $('#j-rerun').addEventListener('click', function () { jSeed = nextSeed(jSeed); updateJudge(); });

  function updateJudge() {
    var j = { rl: +J.rl.value, rs: +J.rs.value, wl: +J.wl.value, ws: +J.ws.value };
    var TPR = (j.rl + j.rs) / 2, FPR = (j.wl + j.ws) / 2;
    var N = +sys.n.value;
    var rng = S.rng(jSeed);
    var systems = [['A', +sys.ap.value, +sys.al.value, 'a'], ['B', +sys.bp.value, +sys.bl.value, 'b']].map(function (s) {
      var p = s[1], l = s[2];
      var q = p * (l * j.rl + (1 - l) * j.rs) + (1 - p) * (l * j.wl + (1 - l) * j.ws);
      var k = 0;
      for (var i = 0; i < N; i++) {
        var right = rng() < p, long = rng() < l;
        var pp = right ? (long ? j.rl : j.rs) : (long ? j.wl : j.ws);
        if (rng() < pp) k++;
      }
      var ci = S.wilson(k, N);
      var sig = TPR - FPR;
      var corr = sig > 0.05 ? S.clamp((k / N - FPR) / sig, 0, 1) : null;
      return { name: s[0], p: p, l: l, q: q, k: k, ci: ci, corr: corr, cls: s[3] };
    });
    table($('#judge-table'), ['System', 'True rate', 'Judge expects', 'Judge measured (95% Wilson)', 'Corrected'],
      systems.map(function (s) {
        return [s.name + ' (' + pct0(s.l) + ' long)', pct0(s.p), pct0(s.q), s.k + '/' + N + ' = ' + pct0(s.k / N) + ' [' + pct0(s.ci[0]) + ', ' + pct0(s.ci[1]) + ']', s.corr === null ? 'no signal' : pct0(s.corr) + (TPR - FPR < 0.3 ? ' (unreliable: signal ' + (TPR - FPR).toFixed(2) + ')' : '')];
      }));
    ivChart($('#judge-chart'), systems.map(function (s) {
      return { label: s.name + ': judge (dash = truth)', lo: s.ci[0], hi: s.ci[1], pt: s.k / N, cls: s.cls, mark: s.p };
    }), { min: 0, max: 1, ticks: [0, 0.25, 0.5, 0.75, 1], fmt: pct0, label: 'Judge-measured pass rate with 95% interval for A and B; dashed marks show the true rates.' });
    var A = systems[0], B = systems[1];
    var trueGap = A.p - B.p, judgeGap = A.k / N - B.k / N;
    var v = $('#judge-verdict');
    S.clear(v);
    var overlap = !(A.ci[0] > B.ci[1] || B.ci[0] > A.ci[1]);
    var cls, msg;
    if (Math.abs(trueGap) < 0.005) { cls = 'warn'; msg = 'The systems are truly equal; any gap the judge shows is noise or bias.'; }
    else if (Math.sign(judgeGap) !== Math.sign(trueGap) && !overlap) { cls = 'bad'; msg = 'The judge ranks the worse system first, and its intervals do not even overlap. A “clear win” for the wrong system.'; }
    else if (overlap) { cls = 'warn'; msg = 'The judge cannot separate the systems at this sample size, although they truly differ by ' + Math.abs(Math.round(trueGap * 100)) + ' points.'; }
    else { cls = 'good'; msg = 'The judge ranks the systems correctly.'; }
    v.appendChild(el('span', { class: 'badge ' + cls, text: 'True gap A − B: ' + pts(trueGap) + ' · judge gap: ' + pts(judgeGap) }));
    v.appendChild(document.createTextNode(' ' + msg));

    // calibration set: 6 answers per cell
    var c = { rl: Math.round(6 * j.rl), rs: Math.round(6 * j.rs), wl: Math.round(6 * j.wl), ws: Math.round(6 * j.ws) };
    var TP = c.rl + c.rs, FN = 12 - TP, FP = c.wl + c.ws, TN = 12 - FP;
    var prec = TP + FP ? TP / (TP + FP) : 0, rec = TP / 12, acc = (TP + TN) / 24;
    var pe = ((TP + FP) / 24) * 0.5 + ((FN + TN) / 24) * 0.5;
    var kappa = pe < 1 ? (acc - pe) / (1 - pe) : 0;
    var bias = ((c.rl - c.rs) + (c.wl - c.ws)) / 12;
    var m = $('#matrix');
    S.clear(m);
    m.appendChild(el('thead', {}, [el('tr', {}, [el('th', { text: '' }), el('th', { text: 'Human: pass' }), el('th', { text: 'Human: fail' })])]));
    m.appendChild(el('tbody', {}, [
      el('tr', {}, [el('th', { text: 'Grader: pass' }), el('td', { text: 'TP ' + TP }), el('td', { text: 'FP ' + FP })]),
      el('tr', {}, [el('th', { text: 'Grader: fail' }), el('td', { text: 'FN ' + FN }), el('td', { text: 'TN ' + TN })])
    ]));
    stats($('#calib-stats'), [
      ['Precision', prec.toFixed(3), prec >= 0.9 ? 'good' : 'bad'],
      ['Recall (TPR)', rec.toFixed(3), rec >= 0.8 ? 'good' : 'bad'],
      ['FPR', (FP / 12).toFixed(3)],
      ['Cohen’s kappa', kappa.toFixed(3)],
      ['TPR − FPR (signal)', (TPR - FPR).toFixed(2)],
      ['Length bias', Math.round(bias * 100) + ' pts', Math.abs(bias) > 0.2 ? 'bad' : '']
    ]);
    var cv = S.clear($('#calib-verdict'));
    var ok = prec >= 0.9 && rec >= 0.8;
    cv.appendChild(el('span', { class: 'badge ' + (ok ? 'good' : 'bad'), text: ok ? 'CALIBRATED' : 'NOT CALIBRATED' }));
    cv.appendChild(document.createTextNode(' needs precision ≥ 0.9 and recall ≥ 0.8.' +
      (Math.abs(bias) > 0.2 ? ' LENGTH BIAS: at equal human labels, long answers pass ' + Math.round(bias * 100) + ' points more often than short ones.' : '') +
      ' Aggregate pass rate on this set: ' + pct0((TP + FP) / 24) + ' vs human 50% — check the matrix, not the average.'));
  }

  /* =================== 07.4 Trials vs interval width =================== */
  var NS_TRIALS = [5, 10, 20, 30, 50, 100, 200, 300, 400];
  var tSeed = S.seedFromQuery(1407);
  var tRng = S.rng(tSeed);
  var runs = [];
  var tn = bindRange('t-n', function (v) { return String(NS_TRIALS[v]); }, updateTrials);
  var tp = bindRange('t-p', pct0, updateTrials);
  var th = bindRange('t-h', function (v) { return '±' + Math.round(v * 100) + ' pts'; }, updateTrials);
  var reveal = $('#t-reveal');
  reveal.addEventListener('change', function () { updateTrials(); });
  $('#t-p-out').hidden = true;
  reveal.addEventListener('change', function () { $('#t-p-out').hidden = !reveal.checked; });
  function runOnce() {
    var n = NS_TRIALS[+tn.value];
    var k = binom(tRng, n, +tp.value);
    runs.push({ k: k, n: n, ci: S.wilson(k, n), label: 'run ' + (runs.length + 1) });
  }
  $('#t-run').addEventListener('click', function () { runOnce(); updateTrials(); });
  $('#t-run20').addEventListener('click', function () { for (var i = 0; i < 20; i++) runOnce(); updateTrials(); });
  $('#t-clear').addEventListener('click', function () { runs = []; updateTrials(); });

  function updateTrials() {
    var truth = +tp.value;
    var shown = runs.slice(-16);
    var box = $('#t-chart');
    if (!shown.length) { S.clear(box).appendChild(el('p', { class: 'muted', text: 'No runs yet. Press “Run the eval once”.' })); }
    else {
      ivChart(box, shown.map(function (r) {
        var miss = reveal.checked && (truth < r.ci[0] || truth > r.ci[1]);
        return { label: r.label + ': ' + r.k + '/' + r.n, lo: r.ci[0], hi: r.ci[1], pt: r.k / r.n, cls: miss ? 'miss' : 'hit' };
      }), { min: 0, max: 1, ticks: [0, 0.25, 0.5, 0.75, 1], fmt: pct0, truth: reveal.checked ? truth : undefined,
        label: 'Wilson 95% intervals of the last ' + shown.length + ' runs' + (reveal.checked ? ', with the true rate as a dashed line' : '') + '.' });
    }
    var sum = S.clear($('#t-summary'));
    var last = runs[runs.length - 1];
    if (last) {
      sum.appendChild(document.createTextNode('Last run: ' + last.k + '/' + last.n + ' = ' + pct0(last.k / last.n) + ', 95% Wilson [' + pct0(last.ci[0]) + ', ' + pct0(last.ci[1]) + '], width ' + Math.round((last.ci[1] - last.ci[0]) * 100) + ' pts.'));
      if (runs.length > 1) {
        var ks = runs.filter(function (r) { return r.n === last.n; }).map(function (r) { return r.k / r.n; });
        if (ks.length > 1) sum.appendChild(document.createTextNode(' Across ' + ks.length + ' runs of ' + last.n + ' trials the observed rate ranged from ' + pct0(Math.min.apply(null, ks)) + ' to ' + pct0(Math.max.apply(null, ks)) + '.'));
      }
      if (reveal.checked) {
        var cover = runs.filter(function (r) { return truth >= r.ci[0] && truth <= r.ci[1]; }).length;
        sum.appendChild(document.createTextNode(' ' + cover + ' of ' + runs.length + ' intervals contain the true rate of ' + pct0(truth) + ' (about 95% expected).'));
      }
    }
    var p = last ? last.k / last.n : 0.7;
    p = S.clamp(p, 0.02, 0.98);
    table($('#t-widths'), ['Trials', '95% Wilson at ' + pct0(p), 'Width', '± half'],
      NS_TRIALS.map(function (n) {
        var ci = S.wilson(Math.round(p * n), n);
        return [String(n), '[' + pct0(ci[0]) + ', ' + pct0(ci[1]) + ']', Math.round((ci[1] - ci[0]) * 100) + ' pts', '±' + Math.round((ci[1] - ci[0]) * 50)];
      }));
    var h = +th.value;
    var need = Math.ceil(1.96 * 1.96 * p * (1 - p) / (h * h));
    $('#t-need').textContent = 'n ≈ z²·p(1−p)/h² = 3.84 × ' + p.toFixed(2) + ' × ' + (1 - p).toFixed(2) + ' / ' + h.toFixed(2) + '² ≈ ' + need + ' trials';
  }

  /* clustering */
  var cSeed = S.seedFromQuery(2411);
  var cT = bindRange('c-T', String, updateCluster);
  var cm = bindRange('c-m', String, updateCluster);
  var cs = bindRange('c-s', function (v) { return v.toFixed(1); }, updateCluster);
  $('#c-pseudo').addEventListener('click', function () { cT.value = 4; cm.value = 25; $('#c-T-out').textContent = '4'; $('#c-m-out').textContent = '25'; updateCluster(); });
  $('#c-wide').addEventListener('click', function () { cT.value = 24; cm.value = 5; $('#c-T-out').textContent = '24'; $('#c-m-out').textContent = '5'; updateCluster(); });
  $('#c-rerun').addEventListener('click', function () { cSeed = nextSeed(cSeed); updateCluster(); });
  var CENTER = S.logit(0.8);

  function popMean(sigma) {
    var r = S.rng(99), s = 0, N = 4000;
    for (var i = 0; i < N; i++) s += S.logistic(CENTER + sigma * r.normal());
    return s / N;
  }

  function updateCluster() {
    var T = +cT.value, m = +cm.value, sigma = +cs.value;
    var rng = S.rng(cSeed + T * 101 + m * 7);
    var rates = [], k = 0;
    for (var i = 0; i < T; i++) {
      var p = S.logistic(CENTER + sigma * rng.normal());
      var ki = binom(rng, m, p);
      k += ki; rates.push(ki / m);
    }
    var n = T * m, ph = k / n;
    var w = S.wilson(k, n);
    var mean = S.mean(rates), se = S.sd(rates) / Math.sqrt(T), tc = S.tcrit(T - 1);
    var cl = [Math.max(0, mean - tc * se), Math.min(1, mean + tc * se)];
    var naiveSe = Math.sqrt(ph * (1 - ph) / n);
    var truth = popMean(sigma);
    ivChart($('#c-chart'), [
      { label: 'Naive Wilson (per trial)', lo: w[0], hi: w[1], pt: ph, cls: 'a' },
      { label: 'Task-clustered (t)', lo: cl[0], hi: cl[1], pt: mean, cls: 'b' }
    ], { min: 0, max: 1, ticks: [0, 0.25, 0.5, 0.75, 1], fmt: pct0, truth: truth,
      label: 'Two 95% intervals from the same ' + T + ' tasks times ' + m + ' trials; dashed line is the pass rate over all tasks you care about.' });
    stats($('#c-stats'), [
      ['Trials', T + ' × ' + m + ' = ' + n],
      ['Pass rate', pct0(ph)],
      ['Naive per-trial SE', naiveSe.toFixed(3)],
      ['Task-clustered SE', se.toFixed(3)],
      ['Ratio', naiveSe > 0 ? (se / naiveSe).toFixed(1) + '×' : '—']
    ]);
    var note = 'Naive: ' + pct0(w[0]) + '–' + pct0(w[1]) + '. Task-clustered: ' + pct0(cl[0]) + '–' + pct0(cl[1]) + '. The dashed line is the rate over all tasks you care about (' + pct0(truth) + ').';
    if (T < 10) note += ' With fewer than about 10 tasks the clustered interval is itself rough: treat it as a warning, not a precise bound.';
    if (m > 5) note += ' Past 3–5 trials per task, more trials barely shrink the honest interval: add tasks.';
    $('#c-note').textContent = note;
  }

  /* =================== 07.5 Paired vs unpaired =================== */
  var pSeed = S.seedFromQuery(1705);
  var P = {
    T: bindRange('p-T', String, updatePaired), m: bindRange('p-m', String, updatePaired),
    s: bindRange('p-s', function (v) { return v.toFixed(1); }, updatePaired),
    e: bindRange('p-e', function (v) { return (v >= 0 ? '+' : '−') + Math.abs(Math.round(v * 100)) + ' pts'; }, updatePaired),
    x: bindRange('p-x', function (v) { return v.toFixed(1); }, updatePaired)
  };
  var PCENTER = S.logit(0.55);
  $('#p-aa').addEventListener('click', function () { P.e.value = 0; P.x.value = 0; $('#p-e-out').textContent = '+0 pts'; $('#p-x-out').textContent = '0.0'; pSeed = nextSeed(pSeed); updatePaired(); });
  $('#p-rerun').addEventListener('click', function () { pSeed = nextSeed(pSeed); updatePaired(); });
  $('#p-many').addEventListener('click', runMany);

  function experiment(rng, T, m, sigma, eff, tau) {
    var a = [], b = [], ka = 0, kb = 0, trueD = [];
    for (var i = 0; i < T; i++) {
      var pa = S.logistic(PCENTER + sigma * rng.normal());
      var pb = S.clamp(pa + eff, 0.005, 0.995);
      pb = S.logistic(S.logit(pb) + tau * rng.normal());
      var xa = binom(rng, m, pa), xb = binom(rng, m, pb);
      ka += xa; kb += xb;
      a.push(xa / m); b.push(xb / m); trueD.push(pb - pa);
    }
    var d = b.map(function (v, i) { return v - a[i]; });
    var dbar = S.mean(d);
    var sePaired = S.sd(d) / Math.sqrt(T);
    var tP = S.tcrit(T - 1);
    var seUn = Math.sqrt(Math.pow(S.sd(a), 2) / T + Math.pow(S.sd(b), 2) / T);
    var tU = S.tcrit(2 * T - 2);
    var n = T * m, pA = ka / n, pB = kb / n;
    var seN = Math.sqrt(pA * (1 - pA) / n + pB * (1 - pB) / n);
    return {
      a: a, b: b, pA: pA, pB: pB, diff: dbar, trueEff: S.mean(trueD),
      better: d.filter(function (x) { return x > 0; }).length, worse: d.filter(function (x) { return x < 0; }).length,
      methods: [
        { name: 'Paired (by task)', se: sePaired, lo: dbar - tP * sePaired, hi: dbar + tP * sePaired, cls: 'a' },
        { name: 'Unpaired by task', se: seUn, lo: dbar - tU * seUn, hi: dbar + tU * seUn, cls: 'b' },
        { name: 'Naive per trial', se: seN, lo: (pB - pA) - 1.96 * seN, hi: (pB - pA) + 1.96 * seN, cls: 'c' }
      ]
    };
  }

  function cfg() { return { T: +P.T.value, m: +P.m.value, s: +P.s.value, e: +P.e.value, x: +P.x.value }; }

  function updatePaired() {
    var c = cfg();
    var ex = experiment(S.rng(pSeed), c.T, c.m, c.s, c.e, c.x);
    var lim = Math.max(0.3, Math.max.apply(null, ex.methods.map(function (m) { return Math.max(Math.abs(m.lo), Math.abs(m.hi)); })) * 1.1);
    lim = Math.min(1, Math.ceil(lim * 10) / 10);
    var ticks = [-lim, -lim / 2, 0, lim / 2, lim];
    ivChart($('#p-chart'), ex.methods.map(function (m) { return { label: m.name, lo: m.lo, hi: m.hi, pt: ex.diff, cls: m.cls }; }),
      { min: -lim, max: lim, ticks: ticks, zero: 0, fmt: function (v) { return (v > 0 ? '+' : v < 0 ? '−' : '') + Math.abs(Math.round(v * 100)); },
        label: 'B minus A with three 95% intervals; the vertical line is zero.' });
    table($('#p-table'), ['Method', 'B − A', '95% CI', 'Width', 'Excludes 0?'],
      ex.methods.map(function (m) {
        return [m.name, pts(m.name === 'Naive per trial' ? ex.pB - ex.pA : ex.diff), '[' + pts(m.lo) + ', ' + pts(m.hi) + ']', Math.round((m.hi - m.lo) * 100) + ' pts', (m.lo > 0 || m.hi < 0) ? 'yes' : 'no'];
      }));
    var pm = ex.methods[0];
    var v = S.clear($('#p-verdict'));
    var excl = pm.lo > 0 || pm.hi < 0;
    v.appendChild(el('span', { class: 'badge ' + (excl ? (c.e === 0 && c.x === 0 ? 'bad' : 'good') : 'warn'),
      text: excl ? (pm.lo > 0 ? 'B is better' : 'B is worse') + ': the paired interval excludes 0' : 'No detectable difference at this sample size' }));
    v.appendChild(document.createTextNode(' ' + c.T + ' paired tasks: B better on ' + ex.better + ', worse on ' + ex.worse + ', equal on ' + (c.T - ex.better - ex.worse) +
      '. Pass rate A ' + pct0(ex.pA) + ' → B ' + pct0(ex.pB) + '. True effect on these tasks: ' + pts(ex.trueEff) + '.' +
      (excl && c.e === 0 && c.x === 0 ? ' This is an A/A run: an interval that excludes 0 happens about 1 time in 20 by chance; if it keeps happening, something other than the treatment is moving results.' : '')));
    renderTasks(ex);
  }

  function renderTasks(ex) {
    var T = ex.a.length, idx = ex.a.map(function (_, i) { return i; }).sort(function (i, j) { return ex.a[i] - ex.a[j]; });
    var box0 = $('#p-tasks'), W = Math.max(300, Math.min(760, box0.clientWidth || 640)), H = 170, pl = 36, pb = 22, pt = 8;
    var X = function (i) { return pl + (W - pl - 10) * (i + 0.5) / T; };
    var Y = function (v) { return pt + (H - pt - pb) * (1 - v); };
    var svg = svgEl(W, H, 'Per-task pass rates in arm A and arm B, sorted by A.');
    [0, 0.5, 1].forEach(function (v) { svg.add('line', { x1: pl, x2: W - 10, y1: Y(v), y2: Y(v), class: 'grid-line' }); svg.add('text', { x: pl - 4, y: Y(v) + 4, 'text-anchor': 'end' }, pct0(v)); });
    idx.forEach(function (i, r) {
      svg.add('line', { x1: X(r), x2: X(r), y1: Y(ex.a[i]), y2: Y(ex.b[i]), class: 'link' });
      svg.add('circle', { cx: X(r), cy: Y(ex.a[i]), r: 4, class: 'dot-a' });
      svg.add('circle', { cx: X(r), cy: Y(ex.b[i]), r: 4, class: 'dot-b' });
    });
    svg.add('text', { x: pl, y: H - 6 }, 'hardest task for A');
    svg.add('text', { x: W - 10, y: H - 6, 'text-anchor': 'end' }, 'easiest');
    var box = S.clear($('#p-tasks'));
    box.appendChild(svg);
    box.appendChild(el('ul', { class: 'legend' }, [
      el('li', {}, [el('i', { class: 'sw', style: 'background:var(--c1)' }), 'Arm A']),
      el('li', {}, [el('i', { class: 'sw', style: 'background:var(--c2)' }), 'Arm B'])
    ]));
  }

  function runMany() {
    var c = cfg(), rng = S.rng(pSeed * 31 + 5), K = 200;
    var agg = [0, 1, 2].map(function () { return { w: 0, excl: 0, cover: 0 }; });
    var names;
    for (var i = 0; i < K; i++) {
      var ex = experiment(rng, c.T, c.m, c.s, c.e, c.x);
      names = ex.methods.map(function (m) { return m.name; });
      ex.methods.forEach(function (m, j) {
        agg[j].w += (m.hi - m.lo);
        if (m.lo > 0 || m.hi < 0) agg[j].excl++;
        if (m.lo <= ex.trueEff && ex.trueEff <= m.hi) agg[j].cover++;
      });
    }
    var box = S.clear($('#p-many-out'));
    var tbl = el('table');
    table(tbl, ['Method', 'Average width', 'Intervals excluding 0', 'Contain the true effect'],
      agg.map(function (a, j) { return [names[j], Math.round(a.w / K * 100) + ' pts', Math.round(a.excl / K * 100) + '%', Math.round(a.cover / K * 100) + '%']; }));
    box.appendChild(el('div', { class: 'scroll-x' }, [tbl]));
    box.appendChild(el('p', { class: 'muted', text: (c.e === 0
      ? 'With no true effect, “excluding 0” is a false alarm; about 5% is expected for an honest interval. '
      : 'With a true effect, “excluding 0” is the power to detect it. ') +
      'Paired and unpaired use the same data; the paired interval is narrower whenever difficulty is shared across arms. The naive per-trial interval treats every trial as independent.' }));
  }

  /* =================== tabs and preset =================== */
  var TABS = ['judge', 'trials', 'paired'];
  function selectTab(name) {
    TABS.forEach(function (n) {
      var t = $('#tab-' + n);
      t.setAttribute('aria-selected', n === name ? 'true' : 'false');
      t.tabIndex = n === name ? 0 : -1;
      $('#view-' + n).hidden = n !== name;
    });
  }
  TABS.forEach(function (n) { $('#tab-' + n).addEventListener('click', function () { selectTab(n); }); });
  $('.tabs').addEventListener('keydown', function (e) {
    if (e.key !== 'ArrowRight' && e.key !== 'ArrowLeft') return;
    var cur = TABS.filter(function (n) { return $('#tab-' + n).getAttribute('aria-selected') === 'true'; })[0];
    var i = (TABS.indexOf(cur) + (e.key === 'ArrowRight' ? 1 : TABS.length - 1)) % TABS.length;
    selectTab(TABS[i]); $('#tab-' + TABS[i]).focus();
  });

  var note = NOTES[preset];
  $('#preset-note').appendChild(el('strong', { text: 'Preset: ' + note[0] }));
  $('#preset-note').appendChild(document.createTextNode(note[1]));

  var resizeTimer = null, lastW = window.innerWidth;
  window.addEventListener('resize', function () {
    if (window.innerWidth === lastW) return;
    lastW = window.innerWidth;
    clearTimeout(resizeTimer);
    resizeTimer = setTimeout(function () { updateJudge(); updateTrials(); updateCluster(); updatePaired(); }, 150);
  });
  TABS.forEach(function (n) { $('#tab-' + n).addEventListener('click', function () { updateJudge(); updateTrials(); updateCluster(); updatePaired(); }); });

  selectTab({ 'biased-judge': 'judge', 'one-run': 'trials', 'paired': 'paired' }[preset]);
  judgeSel.value = 'v1';
  loadJudge();
  if (preset === 'one-run') runs.push({ k: 14, n: 20, ci: S.wilson(14, 20), label: '07.1 one run' });
  updateTrials();
  updateCluster();
  updatePaired();
})();
