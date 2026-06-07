// Advanced particle system for hits, crits and special effects with pooling and audio
(function () {
    const THEME_COLORS = {
        fire: ['#ffae34', '#ff4d4d', '#ffd27a'],
        ice: ['#9be7ff', '#6fd3ff', '#cfefff'],
        shadow: ['#a68cff', '#6f49ff', '#3b1b5b'],
        nature: ['#8cff7a', '#46b94b', '#d6ffd6'],
        arcane: ['#ffd6ff', '#ff9bff', '#d1b3ff'],
        earth: ['#d4b48a', '#8c6b3a', '#f1e0c7'],
        default: ['#ffffff']
    };

    window.ArenaEffects = {
        init: initCanvas,
        spawnParticles: spawnParticles,
        screenShake: screenShake,
        slowMotionPulse: slowMotionPulse,
        playSfx: playSfx,
        setTheme: setTheme
    };

    let canvas, ctx, particlePool = [], active = [], running = false;
    let audioCtx = null, sfxBuffers = {};
    let currentTheme = 'default';
    let enableParticles = true;
    let enableSfx = true;

    function initCanvas() {
        canvas = document.getElementById('arenaEffectsCanvas') || document.createElement('canvas');
        canvas.id = 'arenaEffectsCanvas';
        canvas.style.position = 'absolute';
        canvas.style.left = '0';
        canvas.style.top = '0';
        canvas.style.pointerEvents = 'none';
        canvas.style.zIndex = 9998;
        if (!canvas.parentElement) document.body.appendChild(canvas);
        resize();
        window.addEventListener('resize', resize);
        running = true;
        requestAnimationFrame(loop);

        // Init WebAudio
        try { audioCtx = new (window.AudioContext || window.webkitAudioContext)(); } catch { audioCtx = null; }
        loadDefaultSfx();

        // Read persisted settings from localStorage
        try {
            const p = localStorage.getItem('arena.particles');
            const s = localStorage.getItem('arena.sfx');
            enableParticles = p === null ? enableParticles : (p === '1');
            enableSfx = s === null ? enableSfx : (s === '1');
        } catch { }
    }

    function setTheme(theme) { currentTheme = theme || 'default'; }

    async function loadDefaultSfx() {
        if (!audioCtx) return;
        const urls = {
            hit: '/audio/hit.wav',
            crit: '/audio/crit.wav',
            victory: '/audio/victory.wav',
            defeat: '/audio/defeat.wav'
        };

        for (const k of Object.keys(urls)) {
            try {
                const resp = await fetch(urls[k]);
                if (!resp.ok) continue;
                const arr = await resp.arrayBuffer();
                sfxBuffers[k] = await audioCtx.decodeAudioData(arr.slice(0));
            } catch { /* ignore */ }
        }
    }

    function playSfx(name, volume = 0.6) {
        if (!audioCtx || !sfxBuffers[name]) return;
        const src = audioCtx.createBufferSource();
        src.buffer = sfxBuffers[name];
        const gain = audioCtx.createGain();
        gain.gain.value = volume;
        src.connect(gain).connect(audioCtx.destination);
        src.start(0);
    }

    function resize() {
        if (!canvas) return;
        canvas.width = document.documentElement.clientWidth;
        canvas.height = document.documentElement.clientHeight;
        ctx = canvas.getContext('2d');
    }

    function borrowParticle() {
        return particlePool.length ? particlePool.pop() : {};
    }

    function returnParticle(p) { particlePool.push(p); }

    function spawnParticles(x, y, themeOrColor = null, count = 12, life = 800) {
        if (!enableParticles) return;
        let colors = THEME_COLORS.default;
        if (themeOrColor) {
            if (Array.isArray(themeOrColor)) colors = themeOrColor;
            else if (THEME_COLORS[themeOrColor]) colors = THEME_COLORS[themeOrColor];
            else colors = [themeOrColor.toString()];
        } else if (currentTheme && THEME_COLORS[currentTheme]) {
            colors = THEME_COLORS[currentTheme];
        }

        for (let i = 0; i < count; i++) {
            const angle = Math.random() * Math.PI * 2;
            const speed = Math.random() * 6 + 2;
            const p = borrowParticle();
            p.x = x; p.y = y;
            p.vx = Math.cos(angle) * speed;
            p.vy = Math.sin(angle) * speed - (Math.random() * 2);
            p.life = life; p.age = 0;
            p.color = colors[Math.floor(Math.random() * colors.length)];
            p.size = Math.random() * 8 + 4;
            active.push(p);
        }
    }

    function loop(ts) {
        if (!running) return;
        ctx.clearRect(0, 0, canvas.width, canvas.height);

        for (let i = active.length - 1; i >= 0; i--) {
            const p = active[i];
            p.age += 16;
            p.vy += 0.12; // gravity
            p.x += p.vx; p.y += p.vy;
            const alpha = 1 - (p.age / p.life);
            if (alpha <= 0) { active.splice(i, 1); returnParticle(p); continue; }
            ctx.globalAlpha = alpha;
            ctx.fillStyle = p.color;
            ctx.beginPath();
            ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
            ctx.fill();
        }

        ctx.globalAlpha = 1;
        tickSprites(ts);
        requestAnimationFrame(loop);
    }

    function screenShake(intensity = 6, duration = 300) {
        const el = document.getElementById('battleStage') || document.body;
        el.classList.add('shake');
        el.style.setProperty('--shake-intensity', intensity + 'px');
        setTimeout(() => el.classList.remove('shake'), duration);
    }

    function slowMotionPulse(duration = 400) {
        const el = document.getElementById('battleStage') || document.body;
        el.classList.add('slow-motion');
        setTimeout(() => el.classList.remove('slow-motion'), duration);
    }

    // Public convenience: themed hit
    function hitEffectAt(rect, isCrit = false, theme = null) {
        if (!enableParticles && !enableSfx) return;
        const docX = rect.left + rect.width / 2;
        const docY = rect.top + rect.height / 2;
        if (enableParticles) spawnParticles(docX, docY, theme || (isCrit ? 'fire' : currentTheme), isCrit ? 28 : 12, isCrit ? 1200 : 800);
        if (enableSfx) { if (isCrit) playSfx('crit', 0.9); else playSfx('hit', 0.6); }
        screenShake(isCrit ? 10 : 4, isCrit ? 450 : 220);
        if (isCrit) slowMotionPulse(400);
    }

    function setEnabled(options) {
        if (options === undefined) return;
        if (typeof options.particles === 'boolean') enableParticles = options.particles;
        if (typeof options.sfx === 'boolean') enableSfx = options.sfx;
    }

    function isEnabled() {
        return { particles: enableParticles, sfx: enableSfx };
    }

    // Expose new helper for backward-compat
    window.ArenaEffects.hitEffectAt = hitEffectAt;
})();
