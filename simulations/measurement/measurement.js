/*
 * Measurement simulator (Module 13). Classic script; depends on ../common/sim.js (window.Sim).
 * Ticket experiment: difficulty mix, self-selection, outliers, sample size.
 * Team pilot: selection on an extreme quarter and regression to the mean.
 */
(function () {
  'use strict';
  var Sim = window.Sim;
  var $ = Sim.$, el = Sim.el;

  var SIZES = ['S', 'M', 'L'];
  var GEO = { S: 5, M: 16, L: 47 };                  // 13.1: 5 h vs 16 h vs 47 h
  var SELF = { S: 0.74, M: 0.46, L: 0.33 };          // 13.3 break: agent chosen for 74% of S, 33% of L
  var QUARTERS = ['2025-Q4', '2026-Q1', '2026-Q2', '2026-Q3'];
  var TEAMS = ['Forms', 'Billing', 'Search', 'Identity', 'Payments', 'Reports', 'Mobile', 'Platform', 'Notify', 'Catalog', 'Ledger', 'Portal'];
  var BOOT = 1000, PERM = 1000;

  var TICKET_PARAMS = [
    { k: 'n', label: 'Tickets in the experiment', min: 12, max: 400, step: 4, f: 'i' },
    { k: 'effect', label: 'True effect of the agent (time ratio)', min: 0.5, max: 1.3, step: 0.01, f: 'ratio' },
    { k: 'sd', label: 'Within-size spread, SD of ln(hours)', min: 0.1, max: 1, step: 0.05, f: 'x2' },
    { k: 'mixS', label: 'Share of S tickets', min: 0, max: 1, step: 0.01, f: 'pct' },
    { k: 'mixL', label: 'Share of L tickets (M = the rest)', min: 0, max: 1, step: 0.01, f: 'pct' },
    { k: 'assign', label: 'Who decides which ticket gets the agent', select: [['random', 'coin, blocked by size (13.3)'], ['self', 'developer chooses (self-selected)']] },
    { k: 'outliers', label: 'Tickets blocked for days (170 h, like BILL-503)', min: 0, max: 3, step: 1, f: 'i' },
    { k: 'outArm', label: 'Blocked tickets land in', select: [['A', 'manual arm'], ['B', 'agent arm'], ['any', 'either arm, at random']] },
    { k: 'threshold', label: 'Minimum effect worth acting on', min: 0, max: 0.3, step: 0.01, f: 'pct' }
  ];
  var TEAM_PARAMS = [
    { k: 'r', label: 'r: quarter-to-quarter correlation across teams', min: 0.1, max: 0.95, step: 0.05, f: 'x2' },
    { k: 'tsd', label: 'Total spread, SD of ln(team cycle time)', min: 0.1, max: 0.8, step: 0.05, f: 'x2' },
    { k: 'teffect', label: 'True effect of the pilot (time ratio)', min: 0.5, max: 1.3, step: 0.01, f: 'ratio' },
    { k: 'pick', label: 'How the pilot team is chosen', select: [['worst', 'worst team in 2026-Q2'], ['random', 'at random']] },
    { k: 'baseline', label: 'Baseline for the comparison', select: [['q2', '2026-Q2 only (the quarter that got it selected)'], ['earlier', 'mean of 2025-Q4 and 2026-Q1']] }
  ];

  var BASE = { view: 'tickets', n: 96, effect: 0.84, sd: 0.35, mixS: 0.42, mixL: 0.17, assign: 'random', outliers: 0, outArm: 'A', threshold: 0.10,
    r: 0.45, tsd: 0.35, teffect: 1.0, pick: 'worst', baseline: 'q2', seed: 13 };
  function merge(a, b) { var o = {}; Object.keys(a).forEach(function (k) { o[k] = a[k]; }); Object.keys(b || {}).forEach(function (k) { o[k] = b[k]; }); return o; }

  var PRESETS = {
    randomized: { label: 'Randomized', p: { seed: 29 },
      title: 'A randomized comparison (13.3, 13.5)',
      text: '96 tickets, arms assigned by a coin within each size, true effect −16%. The naive and stratified analyses agree on direction; stratifying by the blocks you randomized within makes the interval much narrower.' },
    'difficulty-mix': { label: 'Difficulty mix', p: { assign: 'self', n: 120, effect: 1.0, mixS: 0.48, mixL: 0.18, seed: 14 },
      title: 'Difficulty mix and self-selection (13.3 break)',
      text: 'Developers chose the agent mostly for small tickets. The true effect is exactly zero, yet the unstratified comparison shows a large "speed-up". The balance table gives it away; the stratified analysis compares like with like. Switch "who decides" to the coin and the illusion disappears.' },
    'selection-bias': { label: 'Selection bias', p: { view: 'teams', teffect: 1.0, pick: 'worst', baseline: 'q2', seed: 6 },
      title: 'Selection bias and regression to the mean (13.4 break)',
      text: 'Twelve teams, no effect at all. The pilot goes to the team with the worst 2026-Q2, and next quarter it "improves" — because its bad quarter was partly luck. Switch the baseline to the two earlier quarters, or pick the team at random, and the improvement vanishes. Raise r and it shrinks.' },
    outliers: { label: 'Outliers', p: { outliers: 1, outArm: 'A', seed: 29 },
      title: 'One blocked ticket and the mean (13.5 break)',
      text: 'One manual ticket waited five days for a sign-off: 170 h. It drags the raw manual mean up and inflates the t-test\'s variance at the same time — so "26% faster" and "not significant" can both come from one slide. The log-scale, stratified estimate barely moves. Add or remove blocked tickets and move them between arms.' },
    'sample-size': { label: 'Sample size', p: { n: 24, effect: 1.0, seed: 8 },
      title: 'Sample size and apparent improvement (13.3, 13.6)',
      text: 'Twelve tickets per arm and no true effect — yet this first sample shows a sizeable "improvement". Press "Run 200 experiments": small experiments regularly show 20–30% "improvements" by chance. Raise the ticket count and watch the spread narrow. The minimum detectable effect at 80% power is shown next to it — with 20 per arm it is about 27%.' }
  };
  var ORDER = ['randomized', 'difficulty-mix', 'outliers', 'sample-size', 'selection-bias'];

  var S = {}, current = 'randomized', batch = null, teamBatch = null;

  /* ---------- maths ---------- */
  function mean(a) { return Sim.mean(a); }
  function sd(a) { return Sim.sd(a); }
  function lgamma(x) {
    var c = [76.18009172947146, -86.50532032941677, 24.01409824083091, -1.231739572450155, 0.1208650973866179e-2, -0.5395239384953e-5];
    var y = x, tmp = x + 5.5; tmp -= (x + 0.5) * Math.log(tmp);
    var ser = 1.000000000190015;
    for (var j = 0; j < 6; j++) ser += c[j] / ++y;
    return -tmp + Math.log(2.5066282746310005 * ser / x);
  }
  function betacf(a, b, x) {
    var qab = a + b, qap = a + 1, qam = a - 1, c = 1, d = 1 - qab * x / qap;
    if (Math.abs(d) < 1e-30) d = 1e-30;
    d = 1 / d; var h = d;
    for (var m = 1; m <= 200; m++) {
      var m2 = 2 * m, aa = m * (b - m) * x / ((qam + m2) * (a + m2));
      d = 1 + aa * d; if (Math.abs(d) < 1e-30) d = 1e-30; c = 1 + aa / c; if (Math.abs(c) < 1e-30) c = 1e-30; d = 1 / d; h *= d * c;
      aa = -(a + m) * (qab + m) * x / ((a + m2) * (qap + m2));
      d = 1 + aa * d; if (Math.abs(d) < 1e-30) d = 1e-30; c = 1 + aa / c; if (Math.abs(c) < 1e-30) c = 1e-30; d = 1 / d;
      var del = d * c; h *= del;
      if (Math.abs(del - 1) < 3e-12) break;
    }
    return h;
  }
  function ibeta(x, a, b) {
    if (x <= 0) return 0; if (x >= 1) return 1;
    var bt = Math.exp(lgamma(a + b) - lgamma(a) - lgamma(b) + a * Math.log(x) + b * Math.log(1 - x));
    return x < (a + 1) / (a + b + 2) ? bt * betacf(a, b, x) / a : 1 - bt * betacf(b, a, 1 - x) / b;
  }
  function tTwoSided(t, df) { return ibeta(df / (df + t * t), df / 2, 0.5); }
  function erfc(x) {
    var z = Math.abs(x), t = 1 / (1 + 0.5 * z);
    var r = t * Math.exp(-z * z - 1.26551223 + t * (1.00002368 + t * (0.37409196 + t * (0.09678418 + t * (-0.18628806 + t * (0.27886807 + t * (-1.13520398 + t * (1.48851587 + t * (-0.82215223 + t * 0.17087277)))))))));
    return x >= 0 ? r : 2 - r;
  }
  function chiP(x, df) { return df === 2 ? Math.exp(-x / 2) : df === 1 ? erfc(Math.sqrt(x / 2)) : 1; }
  function quantile(a, q) {
    var s = a.slice().sort(function (x, y) { return x - y; });
    if (!s.length) return NaN;
    var pos = (s.length - 1) * q, lo = Math.floor(pos), hi = Math.ceil(pos);
    return s[lo] + (s[hi] - s[lo]) * (pos - lo);
  }
  function median(a) { return quantile(a, 0.5); }

  /* ---------- ticket data ---------- */
  function counts(n) {
    var mS = Math.max(0, Math.min(1, S.mixS)), mL = Math.max(0, Math.min(1 - mS, S.mixL));
    var nS = Math.round(n * mS), nL = Math.round(n * mL), nM = Math.max(0, n - nS - nL);
    return { S: nS, M: nM, L: nL };
  }

  function makeTickets(rng, s) {
    var c = counts(s.n), rows = [], id = 500;
    SIZES.forEach(function (z) {
      var arms = [];
      if (s.assign === 'random') {
        for (var i = 0; i < c[z]; i++) arms.push(i < Math.floor(c[z] / 2) ? 'A' : 'B');
        if (c[z] % 2) arms[c[z] - 1] = rng() < 0.5 ? 'A' : 'B';
        for (var j = arms.length - 1; j > 0; j--) { var k = Math.floor(rng() * (j + 1)); var t = arms[j]; arms[j] = arms[k]; arms[k] = t; }
      } else {
        for (var q = 0; q < c[z]; q++) arms.push(rng() < SELF[z] ? 'B' : 'A');
      }
      arms.forEach(function (arm) {
        var h = GEO[z] * Math.exp(s.sd * rng.normal()) * (arm === 'B' ? s.effect : 1);
        rows.push({ id: 'BILL-' + (id++), size: z, arm: arm, hours: h, outlier: false });
      });
    });
    // Blocked tickets: an L ticket in the chosen arm waits days for a sign-off.
    for (var o = 0; o < s.outliers; o++) {
      var want = s.outArm === 'any' ? (rng() < 0.5 ? 'A' : 'B') : s.outArm;
      var pool = rows.filter(function (r) { return !r.outlier && r.arm === want; });
      var pref = pool.filter(function (r) { return r.size === 'L'; });
      var pick = (pref.length ? pref : pool);
      if (!pick.length) break;
      var r = pick[Math.floor(rng() * pick.length)];
      r.hours = 170; r.outlier = true;
    }
    return rows;
  }

  function byCell(rows) {
    var cell = {};
    SIZES.forEach(function (z) { cell[z] = { A: [], B: [] }; });
    rows.forEach(function (r) { cell[r.size][r.arm].push(Math.log(r.hours)); });
    return cell;
  }

  /* Stratified mean log difference, weighted by stratum size (only strata with both arms). */
  function stratEst(cell) {
    var num = 0, w = 0, se2 = 0;
    SIZES.forEach(function (z) {
      var a = cell[z].A, b = cell[z].B;
      if (!a.length || !b.length) return;
      var n = a.length + b.length;
      num += n * (mean(b) - mean(a)); w += n;
      var va = a.length > 1 ? sd(a) * sd(a) : 0, vb = b.length > 1 ? sd(b) * sd(b) : 0;
      se2 += n * n * (va / a.length + vb / b.length);
    });
    return w ? { d: num / w, se: Math.sqrt(se2) / w } : { d: NaN, se: NaN };
  }

  function pooledLogs(cell, arm) { return [].concat(cell.S[arm], cell.M[arm], cell.L[arm]); }

  function resample(a, rng) { var o = new Array(a.length); for (var i = 0; i < a.length; i++) o[i] = a[(rng() * a.length) | 0]; return o; }

  function analyse(rows, rng, full) {
    var cell = byCell(rows);
    var A = rows.filter(function (r) { return r.arm === 'A'; }), B = rows.filter(function (r) { return r.arm === 'B'; });
    var hA = A.map(function (r) { return r.hours; }), hB = B.map(function (r) { return r.hours; });
    var lA = pooledLogs(cell, 'A'), lB = pooledLogs(cell, 'B');
    var out = { nA: A.length, nB: B.length };
    out.rawA = mean(hA); out.rawB = mean(hB); out.rawRatio = out.rawB / out.rawA;
    var vA = sd(hA) * sd(hA) / Math.max(1, hA.length), vB = sd(hB) * sd(hB) / Math.max(1, hB.length);
    out.welchT = (out.rawB - out.rawA) / Math.sqrt(vA + vB);
    out.welchDf = (vA + vB) * (vA + vB) / (vA * vA / Math.max(1, hA.length - 1) + vB * vB / Math.max(1, hB.length - 1));
    out.welchP = isFinite(out.welchT) && out.welchDf > 0 ? tTwoSided(out.welchT, out.welchDf) : NaN;
    out.medA = median(hA); out.medB = median(hB);
    out.naive = mean(lB) - mean(lA);
    var st = stratEst(cell);
    out.strat = st.d; out.stratSe = st.se;
    out.maxH = Math.max.apply(null, hA.concat(hB));
    if (!full) return out;
    var bn = [], bs = [];
    for (var b = 0; b < BOOT; b++) {
      var c2 = {};
      SIZES.forEach(function (z) { c2[z] = { A: resample(cell[z].A, rng), B: resample(cell[z].B, rng) }; });
      bs.push(stratEst(c2).d);
      bn.push(mean(resample(lB, rng)) - mean(resample(lA, rng)));
    }
    out.naiveCI = [quantile(bn, 0.025), quantile(bn, 0.975)];
    out.stratCI = [quantile(bs, 0.025), quantile(bs, 0.975)];
    var cnt = 0, obs = Math.abs(out.strat);
    for (var p = 0; p < PERM; p++) {
      var c3 = {};
      SIZES.forEach(function (z) {
        var all = cell[z].A.concat(cell[z].B), na = cell[z].A.length;
        for (var j = all.length - 1; j > 0; j--) { var k = (rng() * (j + 1)) | 0; var t = all[j]; all[j] = all[k]; all[k] = t; }
        c3[z] = { A: all.slice(0, na), B: all.slice(na) };
      });
      if (Math.abs(stratEst(c3).d) >= obs - 1e-12) cnt++;
    }
    out.permP = (cnt + 1) / (PERM + 1);
    var cntN = 0, obsN = Math.abs(out.naive), pool = lA.concat(lB), nA = lA.length;
    for (var q = 0; q < PERM; q++) {
      for (var j2 = pool.length - 1; j2 > 0; j2--) { var k2 = (rng() * (j2 + 1)) | 0; var t2 = pool[j2]; pool[j2] = pool[k2]; pool[k2] = t2; }
      if (Math.abs(mean(pool.slice(nA)) - mean(pool.slice(0, nA))) >= obsN - 1e-12) cntN++;
    }
    out.naiveP = (cntN + 1) / (PERM + 1);
    out.cell = cell;
    return out;
  }

  function balance(rows) {
    var t = {}, tot = { A: 0, B: 0 }, N = rows.length;
    SIZES.forEach(function (z) { t[z] = { A: 0, B: 0 }; });
    rows.forEach(function (r) { t[r.size][r.arm]++; tot[r.arm]++; });
    var chi = 0, df = -1;
    SIZES.forEach(function (z) {
      var rowN = t[z].A + t[z].B; if (!rowN) return; df++;
      ['A', 'B'].forEach(function (a) { var e = rowN * tot[a] / N; if (e > 0) chi += (t[z][a] - e) * (t[z][a] - e) / e; });
    });
    if (!tot.A || !tot.B) df = 0;
    return { t: t, tot: tot, chi: chi, df: Math.max(0, df), p: df > 0 ? chiP(chi, df) : 1 };
  }

  function mde(nPerArm, s) { if (nPerArm < 2) return NaN; var d = Math.sqrt(2 * 7.849 * s * s / nPerArm); return 1 - Math.exp(-d); }

  /* ---------- team data ---------- */
  function makeTeams(rng, s) {
    var sb = s.tsd * Math.sqrt(s.r), sw = s.tsd * Math.sqrt(1 - s.r), mu = Math.log(24);
    var teams = TEAMS.map(function (name) {
      var lvl = mu + sb * rng.normal();
      return { name: name, lvl: lvl, q: QUARTERS.map(function () { return lvl + sw * rng.normal(); }) };
    });
    var idx;
    if (s.pick === 'worst') { idx = 0; teams.forEach(function (t, i) { if (t.q[2] > teams[idx].q[2]) idx = i; }); }
    else idx = Math.floor(rng() * teams.length);
    teams[idx].q[3] += Math.log(s.teffect);
    // The piloted team is always called Forms, as in the 13.4 claim.
    var nm = teams[idx].name; teams[idx].name = teams[0].name; teams[0].name = nm;
    return { teams: teams, idx: idx };
  }

  function did(teams, ti, base, excl) {
    function pre(t) { return base === 'q2' ? t.q[2] : (t.q[0] + t.q[1]) / 2; }
    var others = teams.filter(function (t, i) { return i !== ti && i !== excl; });
    var ch = teams[ti].q[3] - pre(teams[ti]);
    var oc = mean(others.map(function (t) { return t.q[3] - pre(t); }));
    return { own: ch, others: oc, did: ch - oc };
  }

  function teamAnalysis(d, s) {
    var main = did(d.teams, d.idx, s.baseline, -1);
    var plac = [];
    d.teams.forEach(function (t, i) { if (i !== d.idx) plac.push(did(d.teams, i, s.baseline, d.idx).did); });
    var c = plac.filter(function (x) { return Math.abs(x) >= Math.abs(main.did) - 1e-12; }).length;
    var rank = 1 + d.teams.filter(function (t, i) { return i !== d.idx && t.q[2] > d.teams[d.idx].q[2]; }).length;
    var mu = mean(d.teams.map(function (t) { return t.q[2]; }));
    return { main: main, plac: plac, c: c, p: (c + 1) / (plac.length + 1), rank: rank, mu: mu,
      expected: mu + s.r * (d.teams[d.idx].q[2] - mu) };
  }

  /* ---------- formatting ---------- */
  function pctChange(logd) { var v = (Math.exp(logd) - 1) * 100; return (v >= 0 ? '+' : '−') + Math.abs(v).toFixed(0) + '%'; }
  function ci(lo, hi) { return '[' + pctChange(lo) + ', ' + pctChange(hi) + ']'; }
  function fmtP(p) { return !isFinite(p) ? 'p —' : p < 0.001 ? 'p < 0.001' : 'p = ' + p.toFixed(p < 0.01 ? 4 : 3); }
  function sgn(v) { return (v >= 0 ? '+' : '−') + Math.abs(Math.round(v)) + '%'; }
  function fmtParam(v, f) {
    if (f === 'pct') return Math.round(v * 100) + '%';
    if (f === 'ratio') return v.toFixed(2) + ' (' + ((v - 1) * 100 >= 0 ? '+' : '−') + Math.abs((v - 1) * 100).toFixed(0) + '%)';
    if (f === 'x2') return v.toFixed(2);
    return String(v);
  }
  function stat(k, v, cls, sub) {
    return el('div', {}, [el('div', { class: 'k', text: k }), el('div', { class: 'stat ' + (cls || ''), text: v }), sub ? el('div', { class: 'muted small', text: sub }) : null]);
  }
  function tableEl(headers, rows, numFrom) {
    var t = el('table'), tr = el('tr');
    headers.forEach(function (h, i) { tr.appendChild(el('th', { scope: 'col', class: i >= numFrom ? 'num' : '', text: h })); });
    t.appendChild(el('thead', {}, [tr]));
    var tb = el('tbody');
    rows.forEach(function (r) {
      var row = el('tr', r.cls ? { class: r.cls } : null);
      r.cells.forEach(function (c, i) { row.appendChild(el('td', { class: i >= numFrom ? 'num' : '', text: c })); });
      tb.appendChild(row);
    });
    t.appendChild(tb);
    return el('div', { class: 'scroll-x' }, [t]);
  }

  /* ---------- SVG ---------- */
  var NS = null;
  function svgNS() { if (!NS) { var d = document.createElement('div'); d.innerHTML = '<svg></svg>'; NS = d.firstChild.namespaceURI; } return NS; }
  function svg(w, h, label) {
    var s = document.createElementNS(svgNS(), 'svg');
    s.setAttribute('viewBox', '0 0 ' + w + ' ' + h); s.setAttribute('role', 'img'); s.setAttribute('aria-label', label);
    return s;
  }
  function add(p, tag, attrs, text) {
    var n = document.createElementNS(svgNS(), tag);
    Object.keys(attrs).forEach(function (k) { n.setAttribute(k, attrs[k]); });
    if (text !== undefined) n.textContent = text;
    p.appendChild(n); return n;
  }

  /* Strip plot of hours (log axis) by arm, coloured by size. */
  function strip(rows, rng) {
    var W = 600, H = 150, L = 70, R = 16;
    var s = svg(W, H, 'Ticket hours by arm on a log axis, coloured by size');
    var lo = Math.log(1), hi = Math.log(300);
    function x(h) { return L + (W - L - R) * (Math.log(Math.max(1, Math.min(300, h))) - lo) / (hi - lo); }
    [1, 3, 10, 30, 100, 300].forEach(function (v) {
      add(s, 'line', { x1: x(v), x2: x(v), y1: 10, y2: H - 22, class: 'grid-line' });
      add(s, 'text', { x: x(v), y: H - 8, 'text-anchor': 'middle' }, v + ' h');
    });
    [['A', 'manual', 45], ['B', 'agent', 100]].forEach(function (a) {
      add(s, 'text', { x: L - 8, y: a[2] + 4, 'text-anchor': 'end' }, a[1]);
      var hs = rows.filter(function (r) { return r.arm === a[0]; });
      hs.forEach(function (r) {
        add(s, 'circle', { cx: x(r.hours), cy: a[2] + (rng() - 0.5) * 30, r: r.outlier ? 5 : 3.2, class: 'dot sz-' + r.size + (r.outlier ? ' out' : '') });
      });
      if (hs.length) {
        var g = Math.exp(mean(hs.map(function (r) { return Math.log(r.hours); })));
        add(s, 'line', { x1: x(g), x2: x(g), y1: a[2] - 20, y2: a[2] + 20, class: 'gmean' });
      }
    });
    return s;
  }

  /* Forest plot of estimates (% change) with intervals. */
  function forest(items, truth) {
    var W = 600, H = 34 + items.length * 30, L = 210, R = 20;
    var s = svg(W, H, 'Estimates of the agent effect with 95% intervals');
    var lo = -70, hi = 50;
    items.forEach(function (it) { if (it.ci) { lo = Math.min(lo, it.ci[0] - 5); hi = Math.max(hi, it.ci[1] + 5); } lo = Math.min(lo, it.v - 5); hi = Math.max(hi, it.v + 5); });
    lo = Math.max(lo, -95); hi = Math.min(hi, 200);
    function x(v) { return L + (W - L - R) * (Math.max(lo, Math.min(hi, v)) - lo) / (hi - lo); }
    add(s, 'line', { x1: x(0), x2: x(0), y1: 6, y2: H - 20, class: 'axis' });
    add(s, 'text', { x: x(0), y: H - 6, 'text-anchor': 'middle' }, '0%');
    add(s, 'line', { x1: x(truth), x2: x(truth), y1: 6, y2: H - 20, class: 'truth' });
    add(s, 'text', { x: x(truth), y: H - 6, 'text-anchor': truth < 0 ? 'end' : 'start', class: 'truth-t' }, 'true ' + (truth >= 0 ? '+' : '−') + Math.abs(truth).toFixed(0) + '%');
    items.forEach(function (it, i) {
      var y = 22 + i * 30;
      add(s, 'text', { x: L - 8, y: y + 4, 'text-anchor': 'end' }, it.label);
      if (it.ci) add(s, 'line', { x1: x(it.ci[0]), x2: x(it.ci[1]), y1: y, y2: y, class: 'ci ' + (it.cls || '') });
      add(s, 'circle', { cx: x(it.v), cy: y, r: 5, class: 'est ' + (it.cls || '') });
      add(s, 'text', { x: Math.min(W - 40, x(it.v) + 8), y: y - 8 }, (it.v >= 0 ? '+' : '−') + Math.abs(it.v).toFixed(0) + '%');
    });
    return s;
  }

  function histogram(series, label, truth) {
    var W = 600, H = 170, L = 40, R = 12, T = 10, Bm = 30;
    var s = svg(W, H, label);
    var lo = -60, hi = 60, bins = 30, bw = (hi - lo) / bins;
    var hs = series.map(function (sr) {
      var c = new Array(bins).fill(0);
      sr.values.forEach(function (v) { var b = Math.floor((Math.max(lo, Math.min(hi - 1e-9, v)) - lo) / bw); c[b]++; });
      return c;
    });
    var max = Math.max.apply(null, hs.map(function (c) { return Math.max.apply(null, c); }).concat([1]));
    function x(v) { return L + (W - L - R) * (v - lo) / (hi - lo); }
    [-60, -40, -20, 0, 20, 40, 60].forEach(function (v) {
      add(s, 'line', { x1: x(v), x2: x(v), y1: T, y2: H - Bm, class: v === 0 ? 'axis' : 'grid-line' });
      add(s, 'text', { x: x(v), y: H - Bm + 14, 'text-anchor': 'middle' }, (v > 0 ? '+' : v < 0 ? '−' : '') + Math.abs(v) + '%');
    });
    hs.forEach(function (c, k) {
      c.forEach(function (n, b) {
        if (!n) return;
        var h = (H - T - Bm) * n / max;
        add(s, 'rect', { x: x(lo + b * bw) + (k ? bw * 0.0 : 0) + 1, y: H - Bm - h, width: Math.max(1, (W - L - R) / bins - 2), height: h, class: 'hbar ' + series[k].cls });
      });
    });
    add(s, 'line', { x1: x(truth), x2: x(truth), y1: T, y2: H - Bm, class: 'truth' });
    add(s, 'text', { x: W - R, y: H - 4, 'text-anchor': 'end' }, 'estimated change in cycle time');
    return s;
  }

  /* ---------- views ---------- */
  function renderTickets(box) {
    var rng = Sim.rng(S.seed);
    var rows = makeTickets(rng, S);
    var bal = balance(rows);
    var a = analyse(rows, Sim.rng(S.seed + 1), true);
    var truth = (S.effect - 1) * 100;
    box.appendChild(el('h2', { text: rows.length + ' tickets: ' + a.nA + ' manual (A), ' + a.nB + ' with the agent (B)' }));

    var thr = S.threshold, sci = a.stratCI;
    var hiPct = Math.exp(sci[1]) - 1, loPct = Math.exp(sci[0]) - 1, verdict, vcls;
    if (a.nA < 2 || a.nB < 2 || !isFinite(a.strat)) { verdict = 'not enough tickets in both arms'; vcls = 'warn'; }
    else if (hiPct <= -thr) { verdict = 'real and large enough to act on'; vcls = 'good'; }
    else if (loPct >= thr) { verdict = 'real slow-down beyond the threshold'; vcls = 'bad'; }
    else if (loPct > -thr && hiPct < thr) { verdict = 'too small to matter'; vcls = ''; }
    else verdict = (hiPct < 0 ? 'faster, but may be under the threshold' : loPct > 0 ? 'slower' : 'inconclusive at this size'), vcls = 'warn';

    box.appendChild(el('div', { class: 'stats' }, [
      stat('True effect (you set it)', (truth >= 0 ? '+' : '−') + Math.abs(truth).toFixed(0) + '%'),
      stat('Naive: medians', pctChange(Math.log(a.medB / a.medA)), '', a.medA.toFixed(1) + ' h → ' + a.medB.toFixed(1) + ' h'),
      stat('Pre-registered: stratified', pctChange(a.strat), vcls, '95% CI ' + ci(sci[0], sci[1]) + ' · ' + fmtP(a.permP)),
      stat('Reading vs ±' + Math.round(thr * 100) + '% threshold', verdict, vcls)
    ]));

    box.appendChild(forest([
      { label: 'raw means (Welch t, ' + fmtP(a.welchP) + ')', v: (a.rawRatio - 1) * 100, cls: 'naive' },
      { label: 'log, unstratified (' + fmtP(a.naiveP) + ')', v: (Math.exp(a.naive) - 1) * 100, ci: a.naiveCI.map(function (d) { return (Math.exp(d) - 1) * 100; }), cls: 'naive' },
      { label: 'log, stratified by size (' + fmtP(a.permP) + ')', v: (Math.exp(a.strat) - 1) * 100, ci: sci.map(function (d) { return (Math.exp(d) - 1) * 100; }), cls: 'primary' }
    ], truth));

    box.appendChild(el('h3', { text: 'Every ticket' }));
    box.appendChild(strip(rows, Sim.rng(S.seed + 2)));
    box.appendChild(el('p', { class: 'muted legend' }, [
      el('span', { class: 'sw sz-S' }), ' S ', el('span', { class: 'sw sz-M' }), ' M ', el('span', { class: 'sw sz-L' }), ' L · bar = geometric mean of the arm · large dot = blocked ticket']));

    box.appendChild(el('h3', { text: 'Balance and strata' }));
    var srows = SIZES.map(function (z) {
      var A = a.cell[z].A, B = a.cell[z].B;
      var gA = A.length ? Math.exp(mean(A)) : NaN, gB = B.length ? Math.exp(mean(B)) : NaN;
      var share = bal.t[z].A + bal.t[z].B ? bal.t[z].B / (bal.t[z].A + bal.t[z].B) : NaN;
      return { cells: [z, String(A.length), String(B.length), isFinite(share) ? Math.round(share * 100) + '%' : '—',
        isFinite(gA) ? gA.toFixed(1) : '—', isFinite(gB) ? gB.toFixed(1) : '—', isFinite(gA) && isFinite(gB) ? (gB / gA).toFixed(2) : '—'] };
    });
    box.appendChild(tableEl(['size', 'nA', 'nB', 'share agent', 'geo A (h)', 'geo B (h)', 'B/A'], srows, 1));
    box.appendChild(el('p', { class: bal.p < 0.05 ? 'bad' : 'muted', text: 'chi-square ' + bal.chi.toFixed(2) + ' on ' + bal.df + ' df, ' + fmtP(bal.p) +
      (bal.p < 0.05 ? ' → arms are NOT balanced on size: compare within strata or randomize.' : ' → no evidence of imbalance on size.') }));
    box.appendChild(el('p', { class: 'muted small', text: 'Raw means: A ' + a.rawA.toFixed(1) + ' h, B ' + a.rawB.toFixed(1) + ' h (' + pctChange(Math.log(a.rawRatio)) + '). Largest single value ' + a.maxH.toFixed(0) + ' h. ' +
      'Minimum detectable reduction at 80% power with ' + Math.floor(rows.length / 2) + ' per arm and SD ' + S.sd.toFixed(2) + ': about ' + Math.round(mde(Math.floor(rows.length / 2), S.sd) * 100) + '%.' }));

    box.appendChild(el('h3', { text: 'Run 200 experiments like this one' }));
    box.appendChild(el('p', { class: 'muted', text: 'Same settings, 200 new samples. How far does the apparent improvement swing, and how often would each analysis mislead you?' }));
    box.appendChild(el('button', { type: 'button', class: 'primary', text: batch ? 'Run again' : 'Run 200 experiments', onclick: function () { batch = runBatch(); render(); } }));
    if (batch && batch.key === key()) {
      box.appendChild(histogram([{ values: batch.naive, cls: 'naive' }, { values: batch.strat, cls: 'primary' }], 'Distribution of estimates across 200 experiments', truth));
      box.appendChild(el('p', { class: 'muted legend' }, [el('span', { class: 'sw naive' }), ' log, unstratified  ', el('span', { class: 'sw primary' }), ' log, stratified by size (pre-registered)']));
      function share(arr, f) { return Math.round(100 * arr.filter(f).length / arr.length) + '%'; }
      box.appendChild(tableEl(['Analysis', 'Middle 90% of estimates', 'Showed ≥ 20% faster', 'Interval excluded 0'], [
        { cells: ['log, unstratified', sgn(quantile(batch.naive, 0.05)) + ' to ' + sgn(quantile(batch.naive, 0.95)), share(batch.naive, function (v) { return v <= -20; }), share(batch.naiveSig, function (v) { return v; })] },
        { cells: ['log, stratified by size', sgn(quantile(batch.strat, 0.05)) + ' to ' + sgn(quantile(batch.strat, 0.95)), share(batch.strat, function (v) { return v <= -20; }), share(batch.stratSig, function (v) { return v; })] }
      ], 1));
      box.appendChild(el('p', { class: 'muted small', text: S.effect === 1 ? 'The true effect is zero here, so every "interval excluded 0" is a false positive; with a sound design it should be near 5%.' :
        'The true effect is ' + Math.round(truth) + '%; "interval excluded 0" is the power of each analysis at this size.' }));
    }
  }

  function key() { return JSON.stringify([S.n, S.effect, S.sd, S.mixS, S.mixL, S.assign, S.outliers, S.outArm, S.seed, S.r, S.tsd, S.teffect, S.pick, S.baseline]); }

  function runBatch() {
    var rng = Sim.rng(S.seed * 7 + 3), naive = [], strat = [], naiveSig = [], stratSig = [];
    for (var i = 0; i < 200; i++) {
      var rows = makeTickets(rng, S);
      var a = analyse(rows, rng, false);
      if (!isFinite(a.strat) || !isFinite(a.naive)) continue;
      var cell = byCell(rows), lA = pooledLogs(cell, 'A'), lB = pooledLogs(cell, 'B');
      var seN = Math.sqrt(sd(lA) * sd(lA) / lA.length + sd(lB) * sd(lB) / lB.length);
      naive.push((Math.exp(a.naive) - 1) * 100);
      strat.push((Math.exp(a.strat) - 1) * 100);
      naiveSig.push(Math.abs(a.naive) > 1.96 * seN);
      stratSig.push(Math.abs(a.strat) > 1.96 * a.stratSe);
    }
    return { key: key(), naive: naive, strat: strat, naiveSig: naiveSig, stratSig: stratSig };
  }

  function renderTeams(box) {
    var d = makeTeams(Sim.rng(S.seed), S);
    var an = teamAnalysis(d, S);
    var pilot = d.teams[d.idx];
    box.appendChild(el('h2', { text: '12 teams, 4 quarters — pilot: ' + pilot.name }));
    var own = an.main.own, truth = (S.teffect - 1) * 100;
    box.appendChild(el('div', { class: 'stats' }, [
      stat('True effect of the pilot', (truth >= 0 ? '+' : '−') + Math.abs(truth).toFixed(0) + '%'),
      stat(pilot.name + ' before → after', pctChange(own), '', (S.baseline === 'q2' ? '2026-Q2 ' + Math.exp(pilot.q[2]).toFixed(1) : 'Q4–Q1 ' + Math.exp((pilot.q[0] + pilot.q[1]) / 2).toFixed(1)) + ' h → ' + Math.exp(pilot.q[3]).toFixed(1) + ' h'),
      stat('Difference-in-differences', pctChange(an.main.did), an.main.did < -0.1 ? 'warn' : '', 'others moved ' + pctChange(an.main.others)),
      stat('Placebo p', an.p.toFixed(2), '', an.c + ' of ' + an.plac.length + ' placebo teams at least as extreme')
    ]));
    box.appendChild(el('p', { class: an.rank === 1 ? 'bad' : 'muted', text: 'Selection check: ' + pilot.name + ' ranked ' + an.rank + ' of 12 on 2026-Q2 cycle time' +
      (an.rank === 1 ? ' — the most extreme value; expect regression to the mean.' : '.') +
      ' With r = ' + S.r.toFixed(2) + ', E[next] = μ + r·(x − μ) predicts ' + Math.exp(an.expected).toFixed(1) + ' h from ' + Math.exp(pilot.q[2]).toFixed(1) + ' h with no pilot at all (org mean ' + Math.exp(an.mu).toFixed(1) + ' h).' }));

    var W = 600, H = 220, L = 50, R = 90, T = 10, B = 26;
    var all = []; d.teams.forEach(function (t) { t.q.forEach(function (v) { all.push(Math.exp(v)); }); });
    var ymax = Math.max.apply(null, all) * 1.08, ymin = 0;
    var s = svg(W, H, 'Cycle time per team per quarter; the pilot team is highlighted');
    function x(i) { return L + (W - L - R) * i / 3; }
    function y(v) { return T + (H - T - B) * (1 - (v - ymin) / (ymax - ymin)); }
    [0, 10, 20, 30, 40, 50, 60, 70, 80].forEach(function (v) { if (v > ymax) return; add(s, 'line', { x1: L, x2: W - R, y1: y(v), y2: y(v), class: 'grid-line' }); add(s, 'text', { x: L - 6, y: y(v) + 4, 'text-anchor': 'end' }, v + ' h'); });
    QUARTERS.forEach(function (q, i) { add(s, 'text', { x: x(i), y: H - 8, 'text-anchor': 'middle' }, q); });
    add(s, 'line', { x1: (x(2) + x(3)) / 2, x2: (x(2) + x(3)) / 2, y1: T, y2: H - B, class: 'axis dashed' });
    add(s, 'text', { x: (x(2) + x(3)) / 2 + 4, y: T + 10 }, 'pilot starts');
    d.teams.forEach(function (t, i) {
      var pts = t.q.map(function (v, j) { return x(j) + ',' + y(Math.exp(v)); }).join(' ');
      add(s, 'polyline', { points: pts, class: i === d.idx ? 'team pilot' : 'team' });
      if (i === d.idx) add(s, 'text', { x: x(3) + 6, y: y(Math.exp(t.q[3])) + 4, class: 'pilot-t' }, t.name);
    });
    box.appendChild(s);

    box.appendChild(el('h3', { text: 'Run 500 pilots like this one' }));
    box.appendChild(el('button', { type: 'button', class: 'primary', text: 'Run 500 pilots', onclick: function () { teamBatch = runTeamBatch(); render(); } }));
    if (teamBatch && teamBatch.key === key()) {
      function share(arr, f) { return Math.round(100 * arr.filter(f).length / arr.length) + '%'; }
      box.appendChild(histogram([{ values: teamBatch.q2, cls: 'naive' }, { values: teamBatch.earlier, cls: 'primary' }], 'Distribution of DiD estimates across 500 pilots', truth));
      box.appendChild(el('p', { class: 'muted legend' }, [el('span', { class: 'sw naive' }), ' baseline 2026-Q2  ', el('span', { class: 'sw primary' }), ' baseline mean of the two earlier quarters']));
      box.appendChild(tableEl(['Baseline', 'Median DiD', 'Showed ≥ 20% faster', 'Beat every placebo (p = 0.08)'], [
        { cells: ['2026-Q2 only', sgn(quantile(teamBatch.q2, 0.5)), share(teamBatch.q2, function (v) { return v <= -20; }), share(teamBatch.q2p, function (v) { return v; })] },
        { cells: ['mean of 2025-Q4 and 2026-Q1', sgn(quantile(teamBatch.earlier, 0.5)), share(teamBatch.earlier, function (v) { return v <= -20; }), share(teamBatch.earp, function (v) { return v; })] }
      ], 1));
    }
  }

  function runTeamBatch() {
    var rng = Sim.rng(S.seed * 11 + 5), q2 = [], earlier = [], q2p = [], earp = [];
    for (var i = 0; i < 500; i++) {
      var d = makeTeams(rng, S);
      var a1 = teamAnalysis(d, merge(S, { baseline: 'q2' })), a2 = teamAnalysis(d, merge(S, { baseline: 'earlier' }));
      q2.push((Math.exp(a1.main.did) - 1) * 100); earlier.push((Math.exp(a2.main.did) - 1) * 100);
      q2p.push(a1.c === 0); earp.push(a2.c === 0);
    }
    return { key: key(), q2: q2, earlier: earlier, q2p: q2p, earp: earp };
  }

  /* ---------- controls ---------- */
  function buildParams() {
    var box = Sim.clear($('#params'));
    var list = S.view === 'teams' ? TEAM_PARAMS : TICKET_PARAMS;
    list.forEach(function (d) {
      var id = 'm-' + d.k;
      if (d.select) {
        var sel = el('select', { id: id });
        d.select.forEach(function (o) { sel.appendChild(el('option', { value: o[0], text: o[1] })); });
        sel.value = S[d.k];
        sel.addEventListener('change', function () { S[d.k] = sel.value; render(); });
        box.appendChild(el('div', { class: 'field' }, [el('label', { for: id, text: d.label }), sel]));
        return;
      }
      var out = el('output', { for: id, text: fmtParam(S[d.k], d.f) });
      var inp = el('input', { type: 'range', id: id, min: d.min, max: d.max, step: d.step, value: S[d.k] });
      inp.addEventListener('change', function () { render(); });
      inp.addEventListener('input', function () { S[d.k] = parseFloat(inp.value); out.textContent = fmtParam(S[d.k], d.f); });
      box.appendChild(el('div', { class: 'field' }, [el('div', { class: 'row' }, [el('label', { for: id, text: d.label }), out]), inp]));
    });
  }

  function applyPreset(name) {
    current = name;
    var p = PRESETS[name];
    S = merge(BASE, p.p);
    S.seed = Sim.seedFromQuery(S.seed);
    batch = null; teamBatch = null;
    Sim.$$('#presets button').forEach(function (b) { b.setAttribute('aria-pressed', String(b.dataset.preset === name)); });
    var note = Sim.clear($('#note'));
    note.appendChild(el('strong', { text: p.title }));
    note.appendChild(el('span', { text: p.text }));
    buildParams();
    render();
  }

  function render() {
    $('#seed').textContent = 'seed ' + S.seed;
    var box = Sim.clear($('#view'));
    if (S.view === 'teams') renderTeams(box); else renderTickets(box);
  }

  function init() {
    var tabs = $('#presets');
    ORDER.forEach(function (k) {
      tabs.appendChild(el('button', { type: 'button', 'data-preset': k, 'aria-pressed': 'false', text: PRESETS[k].label, onclick: function () { applyPreset(k); } }));
    });
    $('#rerun').addEventListener('click', function () { S.seed = (S.seed * 7919 + 17) % 100000 + 1; batch = null; teamBatch = null; render(); });
    applyPreset(Sim.preset(ORDER, 'randomized'));
  }

  // Exposed for headless checks only.
  window.MeasurementSim = { PRESETS: PRESETS,
    _tickets: function (preset, seed) { S = merge(BASE, PRESETS[preset].p); if (seed) S.seed = seed; var rows = makeTickets(Sim.rng(S.seed), S); return { rows: rows, a: analyse(rows, Sim.rng(S.seed + 1), true), bal: balance(rows) }; },
    _batch: function (preset) { S = merge(BASE, PRESETS[preset].p); return runBatch(); },
    _teams: function (preset, seed) { S = merge(BASE, PRESETS[preset].p); if (seed) S.seed = seed; var d = makeTeams(Sim.rng(S.seed), S); return { d: d, an: teamAnalysis(d, S) }; },
    _teamBatch: function (preset) { S = merge(BASE, PRESETS[preset].p); return runTeamBatch(); },
    mde: mde, tTwoSided: tTwoSided };

  if (document.readyState === 'loading') document.addEventListener('DOMContentLoaded', init); else init();
})();
