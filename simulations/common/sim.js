/*
 * Shared helpers for the course simulations.
 * Classic script (no ES module) so pages open from file:// and in a sandboxed iframe.
 * Exposes one global: window.Sim. No network, no cookies; storage is optional.
 */
(function () {
  'use strict';
  var Sim = {};

  Sim.LABEL = 'Educational model — not real model behaviour';

  /* ---------- URL query ---------- */
  Sim.params = function () {
    var out = {};
    var q = '';
    try { q = window.location.search || ''; } catch (e) { q = ''; }
    q.replace(/^\?/, '').split('&').forEach(function (pair) {
      if (!pair) return;
      var i = pair.indexOf('=');
      var k = i < 0 ? pair : pair.slice(0, i);
      var v = i < 0 ? '' : pair.slice(i + 1);
      try { out[decodeURIComponent(k)] = decodeURIComponent(v.replace(/\+/g, ' ')); } catch (e) { /* ignore bad pair */ }
    });
    return out;
  };

  /* Returns the preset from ?preset=, or the default when missing/unknown. */
  Sim.preset = function (valid, def) {
    var p = Sim.params().preset;
    return valid.indexOf(p) >= 0 ? p : def;
  };

  Sim.seedFromQuery = function (def) {
    var s = parseInt(Sim.params().seed, 10);
    return isFinite(s) ? s : def;
  };

  /* ---------- seeded RNG (mulberry32) ---------- */
  Sim.rng = function (seed) {
    var a = (seed >>> 0) || 1;
    var f = function () {
      a = (a + 0x6D2B79F5) | 0;
      var t = Math.imul(a ^ (a >>> 15), 1 | a);
      t = (t + Math.imul(t ^ (t >>> 7), 61 | t)) ^ t;
      return ((t ^ (t >>> 14)) >>> 0) / 4294967296;
    };
    f.normal = function () {
      var u = 0, v = 0;
      while (u === 0) u = f();
      while (v === 0) v = f();
      return Math.sqrt(-2 * Math.log(u)) * Math.cos(2 * Math.PI * v);
    };
    f.bern = function (p) { return f() < p; };
    f.int = function (lo, hi) { return lo + Math.floor(f() * (hi - lo + 1)); };
    return f;
  };

  /* ---------- optional storage ---------- */
  Sim.store = {
    get: function (k) { try { return window.localStorage.getItem(k); } catch (e) { return null; } },
    set: function (k, v) { try { window.localStorage.setItem(k, v); } catch (e) { /* optional */ } }
  };

  /* ---------- DOM ---------- */
  Sim.$ = function (sel, root) { return (root || document).querySelector(sel); };
  Sim.$$ = function (sel, root) { return Array.prototype.slice.call((root || document).querySelectorAll(sel)); };

  Sim.el = function (tag, attrs, children) {
    var n = document.createElement(tag);
    if (attrs) Object.keys(attrs).forEach(function (k) {
      var v = attrs[k];
      if (v === null || v === undefined || v === false) return;
      if (k === 'class') n.className = v;
      else if (k === 'text') n.textContent = v;
      else if (k === 'html') n.innerHTML = v;
      else if (k.indexOf('on') === 0 && typeof v === 'function') n.addEventListener(k.slice(2), v);
      else n.setAttribute(k, v === true ? '' : v);
    });
    (children || []).forEach(function (c) {
      if (c === null || c === undefined) return;
      n.appendChild(typeof c === 'string' ? document.createTextNode(c) : c);
    });
    return n;
  };

  Sim.clear = function (n) { while (n.firstChild) n.removeChild(n.firstChild); return n; };

  /* Inserts the mandatory label at the top of <body> if the page did not include it. */
  Sim.label = function () {
    if (Sim.$('.sim-label')) return;
    document.body.insertBefore(Sim.el('p', { class: 'sim-label', role: 'note', text: Sim.LABEL }), document.body.firstChild);
  };

  /* ---------- number formatting ---------- */
  Sim.int = function (n) {
    var s = String(Math.round(Math.abs(n)));
    s = s.replace(/\B(?=(\d{3})+(?!\d))/g, ',');
    return (n < 0 ? '−' : '') + s;
  };
  Sim.fix = function (n, d) { return (n < 0 ? '−' : '') + Math.abs(n).toFixed(d); };
  Sim.pct = function (x, d) { return Sim.fix(x * 100, d === undefined ? 0 : d) + '%'; };
  Sim.money = function (x) { return '$' + Math.abs(x).toFixed(2).replace(/\B(?=(\d{3})+(?!\d))/g, ','); };

  /* ---------- statistics ---------- */
  Sim.logistic = function (x) { return 1 / (1 + Math.exp(-x)); };
  Sim.logit = function (p) { p = Math.min(0.999, Math.max(0.001, p)); return Math.log(p / (1 - p)); };
  Sim.clamp = function (x, lo, hi) { return Math.min(hi, Math.max(lo, x)); };
  Sim.mean = function (a) { return a.length ? a.reduce(function (s, x) { return s + x; }, 0) / a.length : 0; };
  Sim.sd = function (a) {
    if (a.length < 2) return 0;
    var m = Sim.mean(a);
    return Math.sqrt(a.reduce(function (s, x) { return s + (x - m) * (x - m); }, 0) / (a.length - 1));
  };

  /* Wilson score interval, 95% by default. Returns [lo, hi]. */
  Sim.wilson = function (k, n, z) {
    z = z || 1.96;
    if (n <= 0) return [0, 1];
    var p = k / n, z2 = z * z;
    var den = 1 + z2 / n;
    var centre = (p + z2 / (2 * n)) / den;
    var half = z * Math.sqrt(p * (1 - p) / n + z2 / (4 * n * n)) / den;
    return [Math.max(0, centre - half), Math.min(1, centre + half)];
  };

  /* Two-sided 95% t critical value for df degrees of freedom. */
  var T95 = [0, 12.706, 4.303, 3.182, 2.776, 2.571, 2.447, 2.365, 2.306, 2.262, 2.228, 2.201, 2.179, 2.160, 2.145,
    2.131, 2.120, 2.110, 2.101, 2.093, 2.086, 2.080, 2.074, 2.069, 2.064, 2.060, 2.056, 2.052, 2.048, 2.045, 2.042];
  Sim.tcrit = function (df) {
    df = Math.floor(df);
    if (df < 1) return NaN;
    if (df <= 30) return T95[df];
    if (df <= 40) return 2.042 + (2.021 - 2.042) * (df - 30) / 10;
    if (df <= 60) return 2.021 + (2.000 - 2.021) * (df - 40) / 20;
    if (df <= 120) return 2.000 + (1.980 - 2.000) * (df - 60) / 60;
    return 1.96 + 2.4 / df;
  };

  window.Sim = Sim;
})();
