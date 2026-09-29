/* Docsify shell for the AI-Native Trainer course.
 * Conveniences only (outline §19.8): front-matter header, quiz engine for *.quiz.yaml,
 * localStorage progress + resume, simulation embeds, GitHub alert callouts, KaTeX, mermaid.
 * Content files stay plain markdown/YAML and never depend on this file. */
(function () {
  var KEY = 'ai-native-trainer-progress-v1';

  function load() {
    try { return JSON.parse(localStorage.getItem(KEY)) || { done: {}, quiz: {}, last: null }; }
    catch (e) { return { done: {}, quiz: {}, last: null }; }
  }
  function save(s) { try { localStorage.setItem(KEY, JSON.stringify(s)); } catch (e) {} }
  function esc(s) {
    return String(s).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
  }
  function yamlParse(text) { return window.jsyaml ? window.jsyaml.load(text) : null; }

  var current = { fm: null, file: null };

  function splitFrontMatter(md) {
    var m = /^---\n([\s\S]*?)\n---\n/.exec(md.replace(/\r\n/g, '\n'));
    if (!m) return { fm: null, body: md };
    var fm = null;
    try { fm = yamlParse(m[1]); } catch (e) {}
    return { fm: fm, body: md.replace(/\r\n/g, '\n').slice(m[0].length) };
  }

  function headerHtml(fm) {
    var obj = (fm.objectives || []).map(function (o) { return '<li>' + esc(o) + '</li>'; }).join('');
    var pre = (fm.prerequisites || []).map(function (id) {
      var p = String(id).split('.');
      return '<a href="#/lessons/module-' + p[0] + '/lesson-' + ('0' + p[1]).slice(-2) + '">' + esc(id) + '</a>';
    }).join(', ') || 'none';
    return '<div class="lesson-meta">' +
      '<div class="lesson-meta-row"><span>⏱ ' + esc(fm.minutes) + ' min read</span>' +
      '<span>🛠 ' + esc(fm.practice_minutes) + ' min practice</span>' +
      '<span class="vol vol-' + esc(fm.volatility) + '">' + esc(fm.volatility) + '</span>' +
      '<span>Prerequisites: ' + pre + '</span>' +
      '<span>Verified ' + esc(fm.last_verified) + '</span></div>' +
      (obj ? '<details open><summary>Objectives</summary><ul>' + obj + '</ul></details>' : '') +
      '</div>';
  }

  function alerts(md) {
    var kinds = { NOTE: 'note', TIP: 'tip', IMPORTANT: 'important', WARNING: 'warning', CAUTION: 'caution' };
    return md.replace(/^> \[!(NOTE|TIP|IMPORTANT|WARNING|CAUTION)\]\s*\n((?:>.*(?:\n|$))*)/gm, function (_, k, rest) {
      var inner = rest.replace(/^> ?/gm, '');
      return '<div class="gh-alert gh-' + kinds[k] + '"><p class="gh-title">' + k.charAt(0) + k.slice(1).toLowerCase() + '</p>\n\n' + inner + '\n</div>\n';
    });
  }

  // Shield math from the markdown parser.
  var stash = [];
  var TOKEN = /(```[\s\S]*?```|~~~[\s\S]*?~~~)|(`[^`\n]*`)|(\$\$[\s\S]+?\$\$)|(\$(?!\s)[^$\n]+?(?<![\s\\])\$)/g;
  function shieldMath(md) {
    stash = [];
    return md.replace(TOKEN, function (m, fence, code) {
      if (fence || code) return m;
      stash.push(m);
      return 'KATEXSTASH' + (stash.length - 1) + 'END';
    });
  }
  function unshieldMath(html) {
    return html.replace(/KATEXSTASH(\d+)END/g, function (_, i) {
      return stash[+i].replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;');
    });
  }

  function quizUrlFor(file) {
    if (!file) return null;
    if (/lessons\/module-\d\d\/lesson-\d\d\.md$/.test(file)) return file.replace(/\.md$/, '.quiz.yaml');
    if (/assessments\/module-\d\d-quiz\.md$/.test(file)) return file.replace(/\.md$/, '.quiz.yaml');
    return null;
  }

  function renderQuiz(container, questions, quizId) {
    var state = load();
    var wrap = document.createElement('section');
    wrap.className = 'quiz';
    wrap.innerHTML = '<h2 id="knowledge-check">Knowledge check</h2>';
    questions.forEach(function (q, qi) {
      var div = document.createElement('div');
      div.className = 'quiz-q';
      var name = quizId + '-' + q.id;
      div.innerHTML = '<p class="quiz-stem"><strong>' + (qi + 1) + '.</strong> ' + esc(q.question) + '</p>' +
        q.options.map(function (o, oi) {
          return '<label class="quiz-opt"><input type="radio" name="' + esc(name) + '" value="' + oi + '"> ' + esc(o) + '</label>';
        }).join('') + '<div class="quiz-expl" hidden></div>';
      wrap.appendChild(div);
    });
    var btn = document.createElement('button');
    btn.className = 'quiz-submit';
    btn.textContent = 'Submit answers';
    var result = document.createElement('p');
    result.className = 'quiz-result';
    btn.addEventListener('click', function () {
      var right = 0;
      questions.forEach(function (q, qi) {
        var div = wrap.querySelectorAll('.quiz-q')[qi];
        var picked = div.querySelector('input:checked');
        var ok = picked && +picked.value === q.correct;
        if (ok) right++;
        div.classList.remove('ok', 'bad');
        div.classList.add(ok ? 'ok' : 'bad');
        var ex = div.querySelector('.quiz-expl');
        ex.hidden = false;
        ex.innerHTML = (ok ? '✅ Correct. ' : '❌ Correct answer: <em>' + esc(q.options[q.correct]) + '</em>. ') + esc(q.explanation);
      });
      var pct = Math.round(100 * right / questions.length);
      result.textContent = right + ' / ' + questions.length + ' (' + pct + '%) — ' + (pct >= 70 ? 'passed' : 'below the 70% pass mark; review and retry');
      var s = load();
      s.quiz[quizId] = Math.max(s.quiz[quizId] || 0, pct);
      save(s);
      if (window.renderMathInElement) window.renderMathInElement(wrap, { delimiters: [{ left: '$', right: '$', display: false }], throwOnError: false });
    });
    wrap.appendChild(btn);
    wrap.appendChild(result);
    if (state.quiz[quizId] != null) {
      var prev = document.createElement('p');
      prev.className = 'quiz-prev';
      prev.textContent = 'Best previous score: ' + state.quiz[quizId] + '%';
      wrap.appendChild(prev);
    }
    container.appendChild(wrap);
    if (window.renderMathInElement) window.renderMathInElement(wrap, { delimiters: [{ left: '$', right: '$', display: false }], throwOnError: false });
  }

  function progressBar(container, file) {
    if (!/lessons\/module-\d\d\/lesson-\d\d\.md$/.test(file)) return;
    var s = load();
    var bar = document.createElement('div');
    bar.className = 'progress-bar';
    var done = !!s.done[file];
    bar.innerHTML = '<button class="mark-done">' + (done ? '✅ Completed — click to undo' : 'Mark lesson complete') + '</button>';
    bar.querySelector('button').addEventListener('click', function () {
      var st = load();
      if (st.done[file]) delete st.done[file]; else st.done[file] = new Date().toISOString().slice(0, 10);
      save(st);
      this.textContent = st.done[file] ? '✅ Completed — click to undo' : 'Mark lesson complete';
      decorateSidebar();
    });
    container.appendChild(bar);
  }

  function decorateSidebar() {
    var s = load();
    document.querySelectorAll('.sidebar-nav a').forEach(function (a) {
      var href = (a.getAttribute('href') || '').replace(/^#\//, '');
      if (/lessons\/module-/.test(href)) {
        var f = href.replace(/\?.*$/, '') + (/\.md$/.test(href) ? '' : '.md');
        a.classList.toggle('lesson-done', !!s.done[f]);
      }
    });
  }

  function resumeBox(container) {
    var s = load();
    var n = Object.keys(s.done).length;
    var div = document.createElement('div');
    div.className = 'resume-box';
    div.innerHTML = '<p><strong>Your progress:</strong> ' + n + ' / 101 lessons completed (stored in this browser only).</p>' +
      (s.last ? '<p><a href="#/' + esc(s.last.replace(/\.md$/, '')) + '">▶ Resume where you stopped</a></p>' : '');
    container.insertBefore(div, container.firstChild);
  }

  function embedSimulations(root) {
    root.querySelectorAll('a').forEach(function (a) {
      var text = a.textContent || '';
      if (text.indexOf('Simulation:') !== 0) return;
      var href = a.getAttribute('href') || '';
      var src = href.replace(/^#\//, '');
      // Resolve relative to the current page
      var base = (current.file || '').replace(/[^/]*$/, '');
      if (!/^https?:/.test(src) && src.indexOf('simulations/') !== 0) {
        var parts = (base + src).split('/'), outp = [];
        parts.forEach(function (p) { if (p === '..') outp.pop(); else if (p !== '.') outp.push(p); });
        src = outp.join('/');
      }
      var frame = document.createElement('iframe');
      frame.src = src;
      frame.className = 'sim-embed';
      frame.setAttribute('sandbox', 'allow-scripts');
      frame.setAttribute('title', text);
      frame.setAttribute('loading', 'lazy');
      var p = a.closest('p') || a;
      var box = document.createElement('div');
      box.className = 'sim-box';
      var link = document.createElement('a');
      link.href = src; link.target = '_blank'; link.rel = 'noopener';
      link.textContent = text + ' (open full screen)';
      box.appendChild(link);
      box.appendChild(frame);
      p.replaceWith(box);
    });
  }

  window.CourseShellPlugin = function (hook, vm) {
    hook.beforeEach(function (md) {
      current.file = vm.route.file;
      var parts = splitFrontMatter(md);
      current.fm = parts.fm;
      var body = alerts(parts.body);
      if (parts.fm && parts.fm.id) {
        // Put the header right after the H1.
        body = body.replace(/^(# .+\n)/m,'$1\nLESSONMETAHEADER\n');
      }
      return shieldMath(body);
    });
    hook.afterEach(function (html) {
      html = unshieldMath(html);
      if (current.fm && current.fm.id) html = html.replace(/<p>LESSONMETAHEADER<\/p>|LESSONMETAHEADER/, headerHtml(current.fm));
      return html;
    });
    hook.doneEach(function () {
      var section = document.querySelector('.markdown-section');
      var file = vm.route.file;
      if (!section) return;
      if (/lessons\/module-\d\d\/lesson-\d\d\.md$/.test(file)) {
        var s = load(); s.last = file; save(s);
      }
      if (file === 'README.md') resumeBox(section);
      embedSimulations(section);
      var qurl = quizUrlFor(file);
      if (qurl) {
        fetch(qurl).then(function (r) { return r.ok ? r.text() : null; }).then(function (t) {
          if (!t) return;
          var qs = yamlParse(t);
          if (Array.isArray(qs) && qs.length) renderQuiz(section, qs, file);
          progressBar(section, file);
        }).catch(function () { progressBar(section, file); });
      }
      if (window.renderMathInElement) {
        window.renderMathInElement(section, {
          delimiters: [{ left: '$$', right: '$$', display: true }, { left: '$', right: '$', display: false }],
          ignoredTags: ['script', 'noscript', 'style', 'textarea', 'pre', 'code'],
          throwOnError: false
        });
      }
      if (window.mermaid) { try { window.mermaid.run({ querySelector: '.markdown-section .mermaid' }); } catch (e) {} }
      decorateSidebar();
    });
    hook.ready(decorateSidebar);
  };
})();
