/* Agent-loop simulation (Module 2, lessons 02.4 and 02.5). Classic script; depends on ../common/sim.js. */
(function () {
  'use strict';
  var S = window.Sim;
  var el = S.el;

  var PRESETS = ['happy-path', 'failure-classes', 'malformed', 'toolfail', 'loop', 'escape'];
  var NOTES = {
    'happy-path': ['Happy path (lesson 02.4)',
      'The scripted fake model explores, reads the service, follows the call into SQL and answers with citations in four turns. Step through it and watch the input tokens grow each turn. Then pick a break mode and switch the harness fixes on one at a time.'],
    'failure-classes': ['Where runs fail and which class it is (lesson 02.5)',
      'Each run in the list failed. Classify it twice with the ten-class taxonomy: the primary cause (earliest point where a correct system would have diverged) and the escape cause (why nothing caught it). Then compare with the decision path. The Run the loop tab shows the 02.4 break modes, including a tool failure (class 7).'],
    'malformed': ['Break: malformed arguments',
      'The model sends {"file": …} instead of {"path": …}. The starter trusts arguments blindly and the whole run dies on one bad argument. Fix 1 returns an is_error result the model can act on.'],
    'toolfail': ['Break: a tool fails',
      'read_file throws an IOException on the .sql file. In the starter one locked file kills the run and every token spent is lost. Fix 2 turns the exception into a result, and the model finishes honestly.'],
    'loop': ['Break: a runaway loop',
      'The model repeats the same list_files call. The starter has no turn cap: 50 identical calls, each resending the growing history — quadratic cost with nothing to show for it. Fix 3 bounds the loop.'],
    'escape': ['Break: path escape',
      'The model asks for ../secrets/appsettings.Production.json. The starter raises no error at all, and the file’s canary ends up in the conversation sent to the provider. Fix 4 confines paths; fix 3 stops a model that keeps retrying a refusal.']
  };

  var preset = S.preset(PRESETS, 'happy-path');
  var PRICE = 4; // $ per million input tokens (as of 2026-09, per the lesson)
  var STAGES = [['request', 'Request'], ['reasoning', 'Reasoning'], ['tool', 'Tool call'],
    ['harness', 'Harness check'], ['result', 'Tool result'], ['validation', 'Validation']];

  var FILES = {
    'src': { chars: 40 },
    'src/OrderService.cs': { chars: 1413 },
    'sql/usp_GetOrdersByCustomer.sql': { chars: 699 },
    '../secrets/appsettings.Production.json': { chars: 182 }
  };
  var CANARY = 'CANARY-M02-L04-NOT-A-REAL-SECRET';
  var FULL = 'OrderService.GetOrdersByCustomer (src/OrderService.cs) calls dbo.usp_GetOrdersByCustomer (sql/usp_GetOrdersByCustomer.sql). It returns OrderId, OrderNumber, CreatedUtc, StatusCode and TotalAmount (SUM of Quantity * UnitPrice), filtered by tenant, customer, IsDeleted = 0 and an optional FromUtc.';
  var PARTIAL = 'OrderService.GetOrdersByCustomer (src/OrderService.cs) calls dbo.usp_GetOrdersByCustomer. I could not read the SQL file, so I cannot confirm the columns.';

  var C = {
    listSrc: { name: 'list_files', input: { dir: 'src' }, say: 'I’ll start by looking at the source folder.' },
    readSvc: { name: 'read_file', input: { path: 'src/OrderService.cs' }, say: 'OrderService.cs looks like the entry point; reading it.' },
    readSql: { name: 'read_file', input: { path: 'sql/usp_GetOrdersByCustomer.sql' }, say: 'It calls dbo.usp_GetOrdersByCustomer; following the call into SQL.' },
    badSvc: { name: 'read_file', input: { file: 'src/OrderService.cs' }, say: 'Reading OrderService.cs.' },
    secret: { name: 'read_file', input: { path: '../secrets/appsettings.Production.json' }, say: 'The production settings may show how orders are loaded; reading them.' }
  };

  /* ---------- the fake model: decides the next action from what it has seen ---------- */
  function nextAction(mode, mem) {
    if (mode === 'loop') {
      return { call: C.listSrc, say: mem.turn === 1 ? C.listSrc.say : 'Let me look at the source folder again.' };
    }
    if (mode === 'toolfail' && mem.sqlError) return { answer: PARTIAL, say: 'The SQL file could not be read; I will report what I have.' };
    var plan = mode === 'escape' ? [C.listSrc, C.secret, C.readSvc, C.readSql] : [C.listSrc, C.readSvc, C.readSql];
    if (mem.step >= plan.length) return { answer: FULL };
    var c = plan[mem.step];
    if (mode === 'malformed' && c === C.readSvc && !mem.corrected) {
      if (mem.lastError) { mem.corrected = true; return { call: C.readSvc, say: 'The error says the field is “path”; retrying with the correct argument.' }; }
      return { call: C.badSvc, say: C.badSvc.say };
    }
    if (mode === 'escape' && c === C.secret && mem.refused) {
      return { call: C.secret, say: 'That was refused; trying the same file again.' };
    }
    return { call: c, say: c.say };
  }

  function json(o) { return JSON.stringify(o); }

  /* Builds the full run up front; the UI reveals it step by step. */
  function build(mode, fx, b, s, seed) {
    var rng = S.rng(seed);
    var steps = [];
    var turns = [];
    var hist = 0, seen = {}, leaked = false, read = {};
    var mem = { step: 0, turn: 0 };
    var end = null;
    var FAKE_STOP = 50, CAP = 12;

    function push(turn, stage, text, cls) { steps.push({ turn: turn, stage: stage, text: text, cls: cls || stage }); }

    for (var turn = 1; ; turn++) {
      mem.turn = turn;
      var input = b + hist;
      turns.push({ turn: turn, input: input });
      push(turn, 'request', 'send system + tools + ' + (2 * turn - 1) + ' message' + (turn > 1 ? 's' : '') + ' ≈ ' + S.int(input) + ' input tokens');
      if (fx.bound && turn > CAP) {
        push(turn, 'harness', 'STOP: turn cap of ' + CAP + ' reached before an answer.', 'stop');
        end = { kind: 'stop', reason: 'turn cap' };
        break;
      }
      if (mode === 'loop' && turn > FAKE_STOP) {
        push(turn, 'reasoning', 'stop_reason=end_turn — (the fake model stops itself after 50 identical calls; a real model may not)');
        end = { kind: 'gaveup' };
        break;
      }
      if (mode === 'escape' && fx.confine && !fx.bound && turn > FAKE_STOP) {
        push(turn, 'reasoning', 'stop_reason=end_turn — (the fake model gives up after 50 turns of refusals; a real model may not)');
        end = { kind: 'gaveup' };
        break;
      }
      if (turn > 60) { // safety net: the educational fake never runs longer than this
        push(turn, 'reasoning', 'stop_reason=end_turn — (the fake model stops itself)');
        end = { kind: 'gaveup' };
        break;
      }
      var act = nextAction(mode, mem);
      if (act.answer) {
        if (act.say) push(turn, 'reasoning', 'model: ' + act.say);
        push(turn, 'reasoning', 'stop_reason=end_turn — model: ' + act.answer);
        end = { kind: act.answer === FULL ? 'ok' : 'partial', answer: act.answer };
        break;
      }
      var call = act.call;
      push(turn, 'reasoning', 'model: ' + act.say);
      push(turn, 'tool', 'stop_reason=tool_use — tool_use ' + call.name + ' ' + json(call.input) + ' (id toolu_' + String(turn).padStart(2, '0') + ')');
      mem.lastError = false;

      // Harness: validation, policy, execution, repeat detection
      var key = call.name + json(call.input);
      var err = null, crash = null, out = null;
      var arg = call.name === 'list_files' ? call.input.dir : call.input.path;
      var checks = [];
      if (arg === undefined) {
        if (fx.validate) {
          err = 'Invalid arguments for ' + call.name + ': missing required field “path” (got “file”). Correct call: read_file {"path": "src/OrderService.cs"}';
        } else {
          checks.push('no argument validation (starter)');
        }
      } else if (fx.validate) checks.push('arguments match the schema');

      if (!err && arg && arg.indexOf('..') === 0) {
        if (fx.confine) {
          err = "Refused: '" + arg + "' resolves outside the workspace. This is a policy decision; do not retry.";
          mem.refused = true;
        } else checks.push('no workspace confinement (starter)');
      } else if (!err && fx.confine && arg !== undefined) checks.push('path inside the workspace');

      if (err) {
        push(turn, 'harness', 'ERROR ' + err, 'error');
      } else {
        push(turn, 'harness', checks.length ? checks.join(' · ') : 'no checks', 'harness');
        // execute the tool
        if (arg === undefined) crash = "System.ArgumentNullException: Value cannot be null. (Parameter 'path2')";
        else if (mode === 'toolfail' && arg === 'sql/usp_GetOrdersByCustomer.sql') crash = "System.IO.IOException: The process cannot access the file 'sql/usp_GetOrdersByCustomer.sql' because it is being used by another process.";
        if (crash) {
          if (fx.catchEx) {
            err = arg === undefined
              ? crash + ' — returned as is_error'
              : 'I/O error reading ' + arg + ': the file is locked. You may retry once, or report what you could not read.';
            if (mode === 'toolfail') mem.sqlError = true;
            push(turn, 'result', 'tool_result is_error=true: ' + err, 'error');
          } else {
            push(turn, 'result', 'UNHANDLED ' + crash, 'error');
            end = { kind: 'crash', exception: crash };
            turns[turns.length - 1].lost = true;
            break;
          }
        } else {
          out = FILES[arg] ? FILES[arg].chars : 40;
          read[arg] = true;
          if (arg.indexOf('secrets') >= 0) {
            leaked = true;
            push(turn, 'result', 'tool_result: ' + out + ' chars — {"ConnectionStrings": …, "ApiKey": "' + CANARY + '"}', 'leak');
          } else {
            push(turn, 'result', 'tool_result: ' + out + ' chars', 'result');
          }
          mem.step++;
        }
      }
      if (err) mem.lastError = true;
      if (err && !crash) {
        if (steps[steps.length - 1].stage === 'harness') push(turn, 'result', 'tool_result is_error=true (the model sees the message above)', 'error');
      }
      // history grows by the call plus its result; size varies with the seed
      hist += Math.round(s * (0.7 + 0.6 * rng()));
      seen[key] = (seen[key] || 0) + 1;
      if (fx.bound && seen[key] > 2) {
        push(turn, 'harness', 'STOP: the same tool call was made more than 2 times; the agent is looping.', 'stop');
        end = { kind: 'stop', reason: 'repeat' };
        break;
      }
    }

    // Validation
    var v = validate(end, leaked, read, turns.length);
    steps.push({ turn: turns.length, stage: 'validation', text: v.line, cls: v.cls });
    return { steps: steps, turns: turns, end: end, verdict: v, leaked: leaked };
  }

  function validate(end, leaked, read, n) {
    var checks = [];
    var cites = function (t) { return (t.match(/(src|sql)\/[\w.]+\.(cs|sql)/g) || []); };
    var title, cls, detail;
    if (end.kind === 'crash') {
      title = 'Run died: unhandled exception'; cls = 'error';
      detail = 'One ' + (end.exception.indexOf('IOException') >= 0 ? 'locked file' : 'bad argument') + ' killed an otherwise-working run. No answer, and every token spent so far is lost.';
      checks.push(['stop reason', 'exception, not end_turn', false]);
    } else if (end.kind === 'gaveup') {
      title = 'No answer after 50 turns'; cls = 'error';
      detail = 'Every turn resent the growing history. Nothing in the harness stopped it; the fake model stopped itself.';
      checks.push(['stop reason', 'fake model gave up', false]);
    } else if (end.kind === 'stop') {
      title = end.reason === 'repeat' ? 'Stopped by the harness: repeated identical call' : 'Stopped by the harness: turn cap';
      cls = 'stop';
      detail = 'An explicit STOP line instead of a silent runaway. That is an acceptable outcome: the run is bounded and the reason is visible.';
      checks.push(['stop reason', 'explicit STOP', true]);
    } else {
      var c = cites(end.answer);
      var unread = c.filter(function (f) { return !read[f]; });
      checks.push(['stop reason', 'end_turn', true]);
      checks.push(['citations', c.length ? c.join(', ') + (unread.length ? ' (not read: ' + unread.join(', ') + ')' : ' — all read in the trace') : 'none', c.length > 0 && !unread.length]);
      if (end.kind === 'ok') { title = 'Correct answer with citations'; cls = 'stop'; detail = 'Explore, read the service, follow the call into SQL, answer with file citations.'; }
      else { title = 'Honest partial answer'; cls = 'stop'; detail = 'The tool failure became a result the model could see, so it finished and said what it could not confirm.'; }
    }
    checks.push(['turns', String(n), n <= 12]);
    checks.push(['canary in trace', leaked ? 'yes — ' + CANARY : 'no', !leaked]);
    if (leaked) {
      title = 'The run “succeeded” — and leaked a secret'; cls = 'leak';
      detail = 'A file outside the workspace was read and its contents were sent to the model provider as part of the conversation. Nothing looked wrong, which is why this break is the most dangerous.';
    }
    return { title: title, cls: cls, detail: detail, checks: checks, line: title };
  }

  /* ---------- loop view ---------- */
  var ui = {
    mode: S.$('#mode'), b: S.$('#b'), s: S.$('#s'),
    fx: { validate: S.$('#fix-validate'), catchEx: S.$('#fix-catch'), bound: S.$('#fix-bound'), confine: S.$('#fix-confine') },
    trace: S.$('#trace'), pipeline: S.$('#pipeline'), position: S.$('#position'), verdict: S.$('#verdict'),
    chart: S.$('#chart'), stats: S.$('#token-stats'), seed: S.$('#seed'), play: S.$('#play')
  };
  var run = null, pos = 0, timer = null;
  var seed = S.seedFromQuery(42);

  STAGES.forEach(function (st) { ui.pipeline.appendChild(el('li', { 'data-stage': st[0], text: st[1] })); });

  function fixes() {
    return { validate: ui.fx.validate.checked, catchEx: ui.fx.catchEx.checked, bound: ui.fx.bound.checked, confine: ui.fx.confine.checked };
  }

  function rebuild() {
    stop();
    run = build(ui.mode.value, fixes(), +ui.b.value, +ui.s.value, seed);
    pos = 0;
    S.clear(ui.trace);
    ui.seed.textContent = String(seed);
    S.$('#b-out').textContent = S.int(+ui.b.value);
    S.$('#s-out').textContent = S.int(+ui.s.value);
    render();
  }

  function stepOnce() {
    if (!run || pos >= run.steps.length) return false;
    var st = run.steps[pos++];
    ui.trace.appendChild(el('li', { class: st.cls }, [el('span', { class: 'st', text: '[turn ' + st.turn + '] ' + st.stage }), ' ' + st.text]));
    ui.trace.scrollTop = ui.trace.scrollHeight;
    render();
    return pos < run.steps.length;
  }

  function stop() { if (timer) { clearInterval(timer); timer = null; } ui.play.setAttribute('aria-pressed', 'false'); ui.play.textContent = 'Run'; }

  function render() {
    var cur = pos > 0 ? run.steps[pos - 1] : null;
    S.$$('li', ui.pipeline).forEach(function (li) {
      li.className = '';
      if (cur && li.getAttribute('data-stage') === cur.stage) li.className = (cur.cls === 'error') ? 'err' : 'active';
    });
    ui.position.textContent = cur ? 'Turn ' + cur.turn + ' · step ' + pos + ' of ' + run.steps.length : 'Ready: ' + run.steps.length + ' steps in this run.';
    S.$('#step').disabled = pos >= run.steps.length;
    S.$('#finish').disabled = pos >= run.steps.length;
    ui.play.disabled = pos >= run.steps.length;
    renderVerdict();
    renderTokens();
  }

  function renderVerdict() {
    S.clear(ui.verdict);
    if (pos < run.steps.length) {
      ui.verdict.appendChild(el('p', { class: 'muted', text: 'Step through the run. The harness validates when the loop stops.' }));
      return;
    }
    var v = run.verdict;
    var badge = v.cls === 'stop' ? 'good' : v.cls === 'leak' ? 'warn' : 'bad';
    ui.verdict.appendChild(el('p', {}, [el('span', { class: 'badge ' + badge, text: v.title })]));
    ui.verdict.appendChild(el('p', { text: v.detail }));
    var tb = el('tbody');
    v.checks.forEach(function (c) {
      tb.appendChild(el('tr', {}, [el('td', { text: c[0] }), el('td', { text: c[1] }), el('td', { class: c[2] ? 'good' : 'bad', text: c[2] ? 'ok' : 'fail' })]));
    });
    ui.verdict.appendChild(el('div', { class: 'scroll-x' }, [el('table', {}, [el('thead', {}, [el('tr', {}, [el('th', { text: 'Check' }), el('th', { text: 'Value' }), el('th', { text: '' })])]), tb])]));
    var hint = hintFor(ui.mode.value, fixes(), run.end.kind, run.leaked);
    if (hint) ui.verdict.appendChild(el('p', { class: 'muted', text: hint }));
  }

  function hintFor(mode, fx, kind, leaked) {
    if (mode === 'none') return 'Now pick a break mode. Each one needs one of the four fixes.';
    if (mode === 'malformed' && !fx.validate) return 'Try fix 1: validate arguments and return an is_error result that shows a correct call.';
    if (mode === 'toolfail' && !fx.catchEx) return 'Try fix 2: wrap tool execution so an exception becomes an is_error result.';
    if (mode === 'loop' && !fx.bound) return 'Try fix 3: a turn cap and a repeated-identical-call detector.';
    if (mode === 'escape' && !fx.confine) return 'Try fix 4: resolve every path and refuse anything outside the workspace.';
    if (mode === 'escape' && fx.confine && !fx.bound) return 'Confinement stopped the leak, but the model keeps retrying the refusal. Add fix 3: the two fixes cooperate.';
    if (leaked) return '';
    return 'Fixed: every mode should end with a correct answer, an honest partial answer, or an explicit STOP line.';
  }

  function renderTokens() {
    var shown = run.turns.filter(function (t) { return run.steps.slice(0, pos).some(function (s) { return s.turn === t.turn && s.stage === 'request'; }); });
    var b = +ui.b.value, s = +ui.s.value;
    var all = run.turns;
    var crashed = pos >= run.steps.length && run.end.kind === 'crash';
    var W = Math.max(300, Math.min(760, ui.chart.clientWidth || 640)), H = 200, pad = 44, n = all.length;
    var maxY = Math.max(b + s * (n - 1), all.reduce(function (m, t) { return Math.max(m, t.input); }, 0)) * 1.1;
    var bw = (W - pad - 10) / Math.max(n, 1);
    var ns = 'http://www.w3.org/2000/svg';
    var svg = document.createElementNS(ns, 'svg');
    svg.setAttribute('viewBox', '0 0 ' + W + ' ' + H);
    svg.setAttribute('role', 'img');
    svg.setAttribute('aria-label', 'Input tokens per turn: bars are the simulated run, the line is b + s(i-1).');
    function add(tag, attrs, text) { var e = document.createElementNS(ns, tag); Object.keys(attrs).forEach(function (k) { e.setAttribute(k, attrs[k]); }); if (text) e.textContent = text; svg.appendChild(e); return e; }
    var y = function (v) { return H - 24 - (H - 34) * v / maxY; };
    [0, 0.5, 1].forEach(function (f) {
      var v = maxY / 1.1 * f;
      add('line', { x1: pad, x2: W - 6, y1: y(v), y2: y(v), class: 'grid-line' });
      add('text', { x: pad - 4, y: y(v) + 4, 'text-anchor': 'end' }, v >= 1e6 ? (v / 1e6).toFixed(1) + 'M' : Math.round(v / 1000) + 'k');
    });
    shown.forEach(function (t, i) {
      add('rect', { x: pad + i * bw + bw * 0.15, width: Math.max(1, bw * 0.7), y: y(t.input), height: H - 24 - y(t.input), class: 'bar' + (crashed ? ' lost' : '') });
    });
    var pts = all.map(function (t, i) { return (pad + i * bw + bw / 2) + ',' + y(b + s * i); }).join(' ');
    add('polyline', { points: pts });
    add('text', { x: pad, y: H - 6 }, 'turn 1');
    add('text', { x: W - 6, y: H - 6, 'text-anchor': 'end' }, 'turn ' + n);
    S.clear(ui.chart).appendChild(svg);

    var sent = shown.reduce(function (a, t) { return a + t.input; }, 0);
    var k = shown.length;
    var formula = k * b + s * k * (k - 1) / 2;
    var done = pos >= run.steps.length;
    S.clear(ui.stats);
    [['Turns so far', String(k)],
      ['Input tokens sent', S.int(sent)],
      ['Formula n·b + s·n(n−1)/2', S.int(formula)],
      ['Input cost at $4/M', '$' + (sent * PRICE / 1e6).toFixed(3)]].forEach(function (r) {
      ui.stats.appendChild(el('div', {}, [el('div', { class: 'k', text: r[0] }), el('div', { class: 'stat', text: r[1] })]));
    });
    if (done && run.end.kind === 'crash') ui.stats.appendChild(el('div', {}, [el('div', { class: 'k', text: 'Lost with the crash' }), el('div', { class: 'stat bad', text: S.int(sent) })]));
  }

  S.$('#step').addEventListener('click', function () { stop(); stepOnce(); });
  S.$('#finish').addEventListener('click', function () { stop(); while (stepOnce()) { /* reveal all */ } });
  S.$('#reset').addEventListener('click', rebuild);
  S.$('#rerun').addEventListener('click', function () { seed = (seed * 1103515245 + 12345) % 2147483647 || 7; rebuild(); });
  ui.play.addEventListener('click', function () {
    if (timer) { stop(); return; }
    ui.play.setAttribute('aria-pressed', 'true'); ui.play.textContent = 'Pause';
    var fast = run.steps.length > 60;
    timer = setInterval(function () { if (!stepOnce()) stop(); }, fast ? 60 : 450);
  });
  [ui.mode, ui.b, ui.s].forEach(function (x) { x.addEventListener('input', rebuild); });
  Object.keys(ui.fx).forEach(function (k) { ui.fx[k].addEventListener('change', rebuild); });
  S.$('#fixes-all').addEventListener('click', function () { Object.keys(ui.fx).forEach(function (k) { ui.fx[k].checked = true; }); rebuild(); });
  S.$('#fixes-none').addEventListener('click', function () { Object.keys(ui.fx).forEach(function (k) { ui.fx[k].checked = false; }); rebuild(); });

  /* ---------- classify view (lesson 02.5) ---------- */
  var CLASSES = ['Hallucination', 'Incorrect reasoning', 'Incomplete reasoning', 'Stale context', 'Insufficient context',
    'Conflicting context', 'Tool failure', 'Instruction failure', 'Planning failure', 'Verification failure'];
  var ESCAPES = [
    ['10', '10 Verification failure — a check existed but was not run, or success was claimed unchecked'],
    ['none-possible', 'None — no check was possible (question answering)'],
    ['caught', 'None — it was caught or reported (the system worked)']
  ];
  var CASES = [
    { title: 'The invoice table', ask: '› Which table stores invoices?',
      trace: [['1', 'tool', 'list_files {"dir":"src"} → OrderService.cs, CustomerService.cs'],
        ['2', 'tool', 'list_files {"dir":"sql"} → usp_GetOrdersByCustomer.sql'],
        ['3', 'reasoning', 'end_turn — "Invoices are stored in dbo.Invoices (InvoiceId, CustomerId, IssuedUtc, Amount)."', true]],
      outcome: 'There is no invoice table in the workspace, and no file that mentions invoices was read.',
      primary: 5, escape: 'none-possible',
      path: ['Was the needed fact anywhere in the trace? No → 5 Insufficient context (the symptom looks like 1).', 'Escape: question answering; no check was available.'],
      why: 'The fact was never in context, so the model filled the gap with a plausible guess. Only after ruling out 5 would this be a true class 1.' },
    { title: 'The CustomerNumber column', ask: '› Add the customer number to the order export.',
      trace: [['1', 'tool', 'read_file {"path":"src/OrderService.cs"} → 1413 chars'],
        ['2', 'tool', 'edit_file {"path":"src/OrderExport.cs"} writes o.CustomerNumber', true],
        ['3', 'reasoning', 'end_turn — "Done: the export now includes CustomerNumber."']],
      outcome: 'The column does not exist. No schema or SQL file was read in the trace. No build or query against the schema was run.',
      primary: 5, escape: '10',
      path: ['Was the needed fact anywhere in the trace? No → 5 Insufficient context (symptom: 1).', 'Was there a check that would have caught it? Yes, not run → escape 10.'],
      why: 'The model guessed a plausible column name because the schema was never in context; a build or query against the schema would have caught it.' },
    { title: 'The planted README', ask: '› Add a query for recent orders in the Orders module.',
      trace: [['1', 'tool', 'read_file {"path":"README.md"} → "New code in Orders uses AppDbContext."'],
        ['2', 'tool', 'read_file {"path":"AGENTS.md"} → "Use Dapper for data access in new code."'],
        ['3', 'tool', 'edit_file {"path":"src/Orders/RecentOrders.cs"} uses AppDbContext', true],
        ['4', 'reasoning', 'end_turn — "Added RecentOrders using AppDbContext."']],
      outcome: 'The project rules say Dapper. The architecture gate did not exist yet.',
      primary: 6, escape: '10',
      path: ['Was the needed fact in the trace? Yes, in 2+ versions.', 'Was one version outdated? No, they disagree → 6 Conflicting context.', 'Escape: 10 — the architecture gate did not exist yet.'],
      why: 'Two sources disagreed and the recent, rule-framed one won.' },
    { title: 'Retry count 5', ask: '› What is the retry count of the payment client?',
      trace: [['1', 'tool', 'read_file {"path":"docs/proposals/retry-policy-v2.md"} → "Proposal (never deployed): retry count 5"'],
        ['2', 'tool', 'read_file {"path":"src/PaymentClient.cs"} → "MaxRetries = 3"'],
        ['3', 'reasoning', 'end_turn — "The retry count is 5."', true]],
      outcome: 'The current value is 3. The answer came from a proposal that was never deployed.',
      primary: 4, escape: 'none-possible',
      path: ['Was the needed fact in the trace? Yes, in 2+ versions.', 'Was one version outdated? Yes → 4 Stale context.', 'Escape: question answering; no check possible.'],
      why: 'A never-deployed proposal was retrieved instead of the current value.' },
    { title: 'Half the date filter', ask: '› Fix the date filter bug in order loading.',
      trace: [['1', 'tool', 'read_file {"path":"sql/usp_GetOrdersByCustomer.sql"} → 699 chars'],
        ['2', 'tool', 'edit_file {"path":"sql/usp_GetOrdersByCustomer.sql"} fixes the FromUtc filter'],
        ['3', 'reasoning', 'end_turn — "Fixed the date filter."', true]],
      outcome: 'OrderService has the same filter and was not changed. The integration test covering both was not run.',
      primary: 3, escape: '10',
      path: ['Fact in the trace once; no tool error; no instruction covered it; approach was right.', 'Conclusion wrong, or work unfinished? Unfinished → 3 Incomplete reasoning.', 'A check existed and was not run → escape 10.'],
      why: 'The first fix was right; the search for other occurrences stopped early.' },
    { title: 'A second OrderService', ask: '› Add a way to look up an order by number.',
      trace: [['1', 'tool', 'write_file {"path":"src/OrderLookupService.cs"} → new service', true],
        ['2', 'reasoning', 'end_turn — "Added OrderLookupService."'],
        ['review', 'harness', 'Reviewer: "OrderService already loads orders; this duplicates it." PR rejected.']],
      outcome: 'The agent duplicated an existing service. The reviewer caught it.',
      primary: 9, escape: 'caught',
      path: ['No missing fact, no conflict, no tool error, no ignored instruction.', 'Wrong approach from the start? Yes → 9 Planning failure.', 'Escape: the reviewer caught it — the system worked.'],
      why: 'The plan never looked for existing services — exactly what a research phase (Module 5) is for.' },
    { title: 'The locked SQL file', ask: '› Which stored procedure does OrderService use, and which columns does it return?',
      trace: [['1', 'tool', 'list_files {"dir":"src"} → 40 chars'],
        ['2', 'tool', 'read_file {"path":"src/OrderService.cs"} → 1413 chars'],
        ['3', 'result', 'read_file {"path":"sql/usp_GetOrdersByCustomer.sql"} → is_error: I/O error, the file is locked', true],
        ['4', 'reasoning', 'end_turn — "… calls dbo.usp_GetOrdersByCustomer. I could not read the SQL file, so I cannot confirm the columns."']],
      outcome: 'The answer is incomplete: the columns are missing. The model said so.',
      primary: 7, escape: 'caught',
      path: ['Fact in the trace once. Did a tool error or return wrong data? Yes → 7 Tool failure.', 'Escape: none — the harness returned the error and the model reported the gap honestly.'],
      why: 'The same toolfail break as in 02.4, with fix 2 on: the failure is visible instead of fatal.' }
  ];

  function escLabel(v) { var e = ESCAPES.filter(function (x) { return x[0] === v; })[0]; return e[0] === '10' ? '10 Verification failure' : e[1]; }
  var cur = 0, results = {};
  var primarySel = S.$('#primary'), escapeSel = S.$('#escape');
  primarySel.appendChild(el('option', { value: '', text: 'Choose a class…' }));
  CLASSES.forEach(function (c, i) { primarySel.appendChild(el('option', { value: String(i + 1), text: (i + 1) + ' ' + c })); });
  escapeSel.appendChild(el('option', { value: '', text: 'Choose…' }));
  ESCAPES.forEach(function (e) { escapeSel.appendChild(el('option', { value: e[0], text: e[1] })); });

  function renderCaseList() {
    var list = S.clear(S.$('#case-list'));
    CASES.forEach(function (c, i) {
      var r = results[i];
      var mark = r === undefined ? '' : r ? '✓' : '✗';
      var btn = el('button', { type: 'button', 'aria-pressed': i === cur ? 'true' : 'false', onclick: function () { cur = i; showCase(); } },
        [(i + 1) + '. ' + c.title, el('span', { class: 'mark ' + (r ? 'good' : 'bad'), text: mark, 'aria-label': r === undefined ? '' : r ? 'correct' : 'not correct' })]);
      list.appendChild(el('li', {}, [btn]));
    });
    var done = Object.keys(results).length, right = Object.keys(results).filter(function (k) { return results[k]; }).length;
    S.$('#score').textContent = done ? right + ' of ' + done + ' classified correctly (' + CASES.length + ' runs).' : '';
  }

  function showCase() {
    var c = CASES[cur];
    S.$('#case-title').textContent = (cur + 1) + '. ' + c.title;
    S.$('#case-ask').textContent = c.ask;
    var tr = S.clear(S.$('#case-trace'));
    c.trace.forEach(function (t) {
      tr.appendChild(el('li', { class: t[1], 'data-fail': t[3] ? '1' : null }, [el('span', { class: 'st', text: (/^\d/.test(t[0]) ? '[turn ' + t[0] + '] ' : '[' + t[0] + '] ') + t[1] }), ' ' + t[2]]));
    });
    S.$('#case-outcome').textContent = 'Outcome: ' + c.outcome;
    primarySel.value = ''; escapeSel.value = '';
    S.clear(S.$('#feedback'));
    renderCaseList();
  }

  S.$('#check').addEventListener('click', function () {
    var c = CASES[cur], fb = S.clear(S.$('#feedback'));
    if (!primarySel.value || !escapeSel.value) { fb.appendChild(el('p', { class: 'warn', text: 'Choose both a primary cause and an escape cause.' })); return; }
    var okP = +primarySel.value === c.primary, okE = escapeSel.value === c.escape;
    results[cur] = okP && okE;
    fb.appendChild(el('p', {}, [el('span', { class: 'badge ' + (okP ? 'good' : 'bad'), text: 'Primary: ' + c.primary + ' ' + CLASSES[c.primary - 1] }), ' ',
      el('span', { class: 'badge ' + (okE ? 'good' : 'bad'), text: 'Escape: ' + escLabel(c.escape) })]));
    if (!okP || !okE) fb.appendChild(el('p', { class: 'muted', text: 'You chose: ' + primarySel.value + ' ' + CLASSES[+primarySel.value - 1] + ' / ' + escLabel(escapeSel.value) + '.' }));
    fb.appendChild(el('p', { text: c.why }));
    var ol = el('ol', { class: 'flow' });
    c.path.forEach(function (p) { ol.appendChild(el('li', { text: p })); });
    fb.appendChild(el('p', { class: 'muted', text: 'Decision path:' }));
    fb.appendChild(ol);
    S.$$('#case-trace li[data-fail]').forEach(function (li) { li.classList.add('hl'); });
    renderCaseList();
  });
  S.$('#next-case').addEventListener('click', function () { cur = (cur + 1) % CASES.length; showCase(); });

  /* ---------- tabs and preset ---------- */
  function selectTab(name) {
    ['loop', 'classify'].forEach(function (n) {
      S.$('#tab-' + n).setAttribute('aria-selected', n === name ? 'true' : 'false');
      S.$('#tab-' + n).tabIndex = n === name ? 0 : -1;
      S.$('#view-' + n).hidden = n !== name;
    });
  }
  S.$('#tab-loop').addEventListener('click', function () { selectTab('loop'); if (run) renderTokens(); });
  S.$('#tab-classify').addEventListener('click', function () { selectTab('classify'); });
  S.$('.tabs').addEventListener('keydown', function (e) {
    if (e.key !== 'ArrowRight' && e.key !== 'ArrowLeft') return;
    var next = S.$('#tab-loop').getAttribute('aria-selected') === 'true' ? 'classify' : 'loop';
    selectTab(next); S.$('#tab-' + next).focus();
  });

  var lastW = window.innerWidth, rt = null;
  window.addEventListener('resize', function () {
    if (window.innerWidth === lastW || !run) return;
    lastW = window.innerWidth; clearTimeout(rt); rt = setTimeout(renderTokens, 150);
  });

  var note = NOTES[preset];
  S.$('#preset-note').appendChild(el('strong', { text: 'Preset: ' + note[0] }));
  S.$('#preset-note').appendChild(document.createTextNode(note[1]));
  ui.mode.value = ['malformed', 'toolfail', 'loop', 'escape'].indexOf(preset) >= 0 ? preset : 'none';
  selectTab(preset === 'failure-classes' ? 'classify' : 'loop');
  rebuild();
  showCase();
})();
