/**
 * PROCEDURAL BACKGROUND RENDERER
 * Generates all 10 arena level backgrounds using Canvas primitives
 */

class ProceduralBackground {
    constructor(canvas, level) {
        this.canvas = canvas;
        this.ctx = canvas.getContext('2d');
        this.level = level;
        this.animationTime = 0;
        this.particles = [];
        this.initLevel(level);
    }

    initLevel(level) {
        // Initialize particles and level-specific data
        switch(level) {
            case 1: this.initForest(); break;
            case 2: this.initCrypt(); break;
            case 3: this.initTwilightForest(); break;
            case 4: this.initElvenRuins(); break;
            case 5: this.initVolcanicForge(); break;
            case 6: this.initFrozenTundra(); break;
            case 7: this.initMountainPass(); break;
            case 8: this.initShadowRealm(); break;
            case 9: this.initDragonLair(); break;
            case 10: this.initThroneDarkness(); break;
        }
    }

    update(deltaTime) {
        this.animationTime += deltaTime;
        
        // Update particles
        this.particles.forEach(p => {
            p.x += p.vx * (deltaTime / 16);
            p.y += p.vy * (deltaTime / 16);
            p.life -= deltaTime;
            
            // Wrap or respawn particles
            if (p.life <= 0 || p.y > this.canvas.height + 50 || p.y < -50) {
                this.respawnParticle(p);
            }
        });
    }

    draw() {
        switch(this.level) {
            case 1: this.drawForest(); break;
            case 2: this.drawCrypt(); break;
            case 3: this.drawTwilightForest(); break;
            case 4: this.drawElvenRuins(); break;
            case 5: this.drawVolcanicForge(); break;
            case 6: this.drawFrozenTundra(); break;
            case 7: this.drawMountainPass(); break;
            case 8: this.drawShadowRealm(); break;
            case 9: this.drawDragonLair(); break;
            case 10: this.drawThroneDarkness(); break;
        }
        
        // Draw vignette
        this.drawVignette();
    }

    // ========================================
    // LEVEL 1: DEEP FOREST
    // ========================================
    initForest() {
        // Fireflies
        for (let i = 0; i < 20; i++) {
            this.particles.push({
                x: Math.random() * this.canvas.width,
                y: Math.random() * this.canvas.height,
                vx: (Math.random() - 0.5) * 0.5,
                vy: (Math.random() - 0.5) * 0.5,
                size: 2 + Math.random() * 2,
                life: Infinity,
                type: 'firefly',
                alpha: Math.random()
            });
        }
    }

    drawForest() {
        const ctx = this.ctx;
        const w = this.canvas.width;
        const h = this.canvas.height;
        
        // Sky gradient - deep forest green
        const skyGrad = ctx.createLinearGradient(0, 0, 0, h);
        skyGrad.addColorStop(0, '#1a3d1a');
        skyGrad.addColorStop(1, '#2d5a1b');
        ctx.fillStyle = skyGrad;
        ctx.fillRect(0, 0, w, h);
        
        // Tree layers (parallax effect)
        this.drawTreeLayer(ctx, w, h, '#0d2d0d', 4, 0.3);
        this.drawTreeLayer(ctx, w, h, '#1a3d1a', 3, 0.5);
        this.drawTreeLayer(ctx, w, h, '#2d5a1b', 2, 0.7);
        
        // Ground
        const groundGrad = ctx.createLinearGradient(0, h * 0.7, 0, h);
        groundGrad.addColorStop(0, '#1a4d1a');
        groundGrad.addColorStop(1, '#0d2d0d');
        ctx.fillStyle = groundGrad;
        ctx.fillRect(0, h * 0.7, w, h * 0.3);
        
        // Light rays from upper left
        ctx.save();
        ctx.globalAlpha = 0.1;
        ctx.fillStyle = '#ffeb3b';
        for (let i = 0; i < 5; i++) {
            ctx.beginPath();
            ctx.moveTo(w * 0.2, 0);
            ctx.lineTo(w * 0.3 + i * 50, h);
            ctx.lineTo(w * 0.3 + i * 50 + 30, h);
            ctx.lineTo(w * 0.2 + 20, 0);
            ctx.fill();
        }
        ctx.restore();
        
        // Fireflies
        this.particles.forEach(p => {
            if (p.type === 'firefly') {
                const pulse = Math.sin(this.animationTime / 500 + p.x) * 0.5 + 0.5;
                ctx.save();
                ctx.globalAlpha = pulse * 0.8;
                ctx.fillStyle = '#ffeb3b';
                ctx.shadowBlur = 10;
                ctx.shadowColor = '#ffeb3b';
                ctx.beginPath();
                ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
                ctx.fill();
                ctx.restore();
            }
        });
    }

    drawTreeLayer(ctx, w, h, color, count, yPos) {
        ctx.fillStyle = color;
        for (let i = 0; i < count; i++) {
            const x = (w / count) * i + (w / count) * 0.5;
            const treeH = h * (0.4 + yPos * 0.3);
            ctx.beginPath();
            ctx.moveTo(x, h * yPos);
            ctx.lineTo(x - 40, h * yPos - treeH);
            ctx.lineTo(x - 20, h * yPos - treeH * 0.7);
            ctx.lineTo(x, h * yPos - treeH);
            ctx.lineTo(x + 20, h * yPos - treeH * 0.7);
            ctx.lineTo(x + 40, h * yPos - treeH);
            ctx.closePath();
            ctx.fill();
        }
    }

    // ========================================
    // LEVEL 2: STONE CRYPT
    // ========================================
    initCrypt() {
        // Torch particles
        for (let i = 0; i < 30; i++) {
            this.particles.push({
                x: this.canvas.width * 0.2,
                y: this.canvas.height * 0.3,
                vx: (Math.random() - 0.5) * 0.3,
                vy: -Math.random() * 2,
                size: 2 + Math.random() * 3,
                life: 1000 + Math.random() * 500,
                maxLife: 1000,
                type: 'torch-left'
            });
            this.particles.push({
                x: this.canvas.width * 0.8,
                y: this.canvas.height * 0.3,
                vx: (Math.random() - 0.5) * 0.3,
                vy: -Math.random() * 2,
                size: 2 + Math.random() * 3,
                life: 1000 + Math.random() * 500,
                maxLife: 1000,
                type: 'torch-right'
            });
        }
    }

    drawCrypt() {
        const ctx = this.ctx;
        const w = this.canvas.width;
        const h = this.canvas.height;
        
        // Dark stone background
        ctx.fillStyle = '#1a1a3d';
        ctx.fillRect(0, 0, w, h);
        
        // Vaulted ceiling arch
        ctx.strokeStyle = '#2a2a4d';
        ctx.lineWidth = 3;
        ctx.beginPath();
        ctx.arc(w / 2, h, w * 0.6, Math.PI, 0);
        ctx.stroke();
        
        // Side walls with skulls
        ctx.fillStyle = '#0d0d2a';
        ctx.fillRect(0, 0, w * 0.15, h);
        ctx.fillRect(w * 0.85, 0, w * 0.15, h);
        
        // Floor with perspective
        const floorGrad = ctx.createLinearGradient(0, h * 0.6, 0, h);
        floorGrad.addColorStop(0, '#2a2a4d');
        floorGrad.addColorStop(1, '#1a1a3d');
        ctx.fillStyle = floorGrad;
        ctx.fillRect(0, h * 0.6, w, h * 0.4);
        
        // Floor grid lines
        ctx.strokeStyle = '#3a3a5d';
        ctx.lineWidth = 1;
        for (let i = 0; i < 5; i++) {
            const y = h * 0.6 + (i * h * 0.08);
            ctx.beginPath();
            ctx.moveTo(w * 0.2, y);
            ctx.lineTo(w * 0.8, y);
            ctx.stroke();
        }
        
        // Torches and fire particles
        const flicker = Math.sin(this.animationTime / 100) * 0.2 + 0.8;
        
        // Left torch
        ctx.save();
        ctx.globalAlpha = flicker;
        ctx.fillStyle = '#ff6b00';
        ctx.shadowBlur = 30;
        ctx.shadowColor = '#ff6b00';
        ctx.beginPath();
        ctx.ellipse(w * 0.2, h * 0.3, 20, 30, 0, 0, Math.PI * 2);
        ctx.fill();
        ctx.restore();
        
        // Right torch
        ctx.save();
        ctx.globalAlpha = flicker;
        ctx.fillStyle = '#ff6b00';
        ctx.shadowBlur = 30;
        ctx.shadowColor = '#ff6b00';
        ctx.beginPath();
        ctx.ellipse(w * 0.8, h * 0.3, 20, 30, 0, 0, Math.PI * 2);
        ctx.fill();
        ctx.restore();
        
        // Fire particles
        this.particles.forEach(p => {
            if (p.type.includes('torch')) {
                const alpha = p.life / p.maxLife;
                ctx.save();
                ctx.globalAlpha = alpha;
                const gradient = ctx.createRadialGradient(p.x, p.y, 0, p.x, p.y, p.size);
                gradient.addColorStop(0, '#ffeb3b');
                gradient.addColorStop(0.5, '#ff6b00');
                gradient.addColorStop(1, 'transparent');
                ctx.fillStyle = gradient;
                ctx.beginPath();
                ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
                ctx.fill();
                ctx.restore();
            }
        });
    }

    // ========================================
    // LEVEL 3: TWILIGHT FOREST
    // ========================================
    initTwilightForest() {
        // Falling leaves
        for (let i = 0; i < 25; i++) {
            this.particles.push({
                x: Math.random() * this.canvas.width,
                y: Math.random() * this.canvas.height,
                vx: -0.5 - Math.random() * 0.5,
                vy: 0.5 + Math.random() * 0.5,
                size: 3 + Math.random() * 4,
                life: Infinity,
                rotation: Math.random() * Math.PI * 2,
                rotSpeed: (Math.random() - 0.5) * 0.1,
                type: 'leaf'
            });
        }
    }

    drawTwilightForest() {
        const ctx = this.ctx;
        const w = this.canvas.width;
        const h = this.canvas.height;
        
        // Twilight sky
        const skyGrad = ctx.createLinearGradient(0, 0, 0, h);
        skyGrad.addColorStop(0, '#2a0d3d');
        skyGrad.addColorStop(0.5, '#4a1d6d');
        skyGrad.addColorStop(1, '#6a2d8d');
        ctx.fillStyle = skyGrad;
        ctx.fillRect(0, 0, w, h);
        
        // Tree silhouettes - 4 layers
        this.drawTreeSilhouette(ctx, w, h, '#1a0d2d', 0.3);
        this.drawTreeSilhouette(ctx, w, h, '#2a0d3d', 0.45);
        this.drawTreeSilhouette(ctx, w, h, '#3a1d4d', 0.6);
        this.drawTreeSilhouette(ctx, w, h, '#4a2d5d', 0.75);
        
        // Golden light from behind trees
        ctx.save();
        ctx.globalAlpha = 0.3;
        const lightGrad = ctx.createRadialGradient(w * 0.7, h * 0.3, 0, w * 0.7, h * 0.3, w * 0.4);
        lightGrad.addColorStop(0, '#ffeb3b');
        lightGrad.addColorStop(1, 'transparent');
        ctx.fillStyle = lightGrad;
        ctx.fillRect(0, 0, w, h);
        ctx.restore();
        
        // Falling leaves
        this.particles.forEach(p => {
            if (p.type === 'leaf') {
                p.rotation += p.rotSpeed;
                ctx.save();
                ctx.translate(p.x, p.y);
                ctx.rotate(p.rotation);
                ctx.fillStyle = '#8d6d3d';
                ctx.fillRect(-p.size / 2, -p.size / 2, p.size, p.size);
                ctx.restore();
            }
        });
    }

    drawTreeSilhouette(ctx, w, h, color, yStart) {
        ctx.fillStyle = color;
        const treeCount = 6;
        for (let i = 0; i < treeCount; i++) {
            const x = (w / treeCount) * i;
            const treeWidth = 40 + Math.random() * 40;
            const treeHeight = h * (0.5 - yStart * 0.3);
            
            // Trunk
            ctx.fillRect(x - 10, h * yStart, 20, treeHeight);
            
            // Foliage (triangle)
            ctx.beginPath();
            ctx.moveTo(x, h * yStart - treeHeight);
            ctx.lineTo(x - treeWidth, h * yStart);
            ctx.lineTo(x + treeWidth, h * yStart);
            ctx.closePath();
            ctx.fill();
        }
    }

    // ========================================
    // LEVEL 5: VOLCANIC FORGE (BOSS)
    // ========================================
    initVolcanicForge() {
        // Rising embers
        for (let i = 0; i < 50; i++) {
            this.particles.push({
                x: Math.random() * this.canvas.width,
                y: this.canvas.height + Math.random() * 100,
                vx: (Math.random() - 0.5) * 0.5,
                vy: -1 - Math.random() * 2,
                size: 2 + Math.random() * 4,
                life: Infinity,
                type: 'ember'
            });
        }
    }

    drawVolcanicForge() {
        const ctx = this.ctx;
        const w = this.canvas.width;
        const h = this.canvas.height;
        
        // Orange-red volcanic atmosphere
        const skyGrad = ctx.createLinearGradient(0, 0, 0, h);
        skyGrad.addColorStop(0, '#4a1000');
        skyGrad.addColorStop(1, '#2a0800');
        ctx.fillStyle = skyGrad;
        ctx.fillRect(0, 0, w, h);
        
        // Lava river at bottom with glow
        const lavaGrad = ctx.createLinearGradient(0, h * 0.8, 0, h);
        lavaGrad.addColorStop(0, '#ff6b00');
        lavaGrad.addColorStop(0.5, '#ff4500');
        lavaGrad.addColorStop(1, '#8b0000');
        ctx.fillStyle = lavaGrad;
        ctx.fillRect(0, h * 0.8, w, h * 0.2);
        
        // Lava glow
        ctx.save();
        ctx.globalAlpha = 0.5 + Math.sin(this.animationTime / 200) * 0.2;
        ctx.shadowBlur = 50;
        ctx.shadowColor = '#ff6b00';
        ctx.fillStyle = '#ff6b00';
        ctx.fillRect(0, h * 0.8, w, 5);
        ctx.restore();
        
        // Ceiling glow from lava reflection
        const ceilingGrad = ctx.createLinearGradient(0, 0, 0, h * 0.3);
        ceilingGrad.addColorStop(0, 'rgba(255, 107, 0, 0.3)');
        ceilingGrad.addColorStop(1, 'transparent');
        ctx.fillStyle = ceilingGrad;
        ctx.fillRect(0, 0, w, h * 0.3);
        
        // Rising embers
        this.particles.forEach(p => {
            if (p.type === 'ember') {
                const alpha = 0.5 + Math.random() * 0.5;
                ctx.save();
                ctx.globalAlpha = alpha;
                const gradient = ctx.createRadialGradient(p.x, p.y, 0, p.x, p.y, p.size);
                gradient.addColorStop(0, '#ffeb3b');
                gradient.addColorStop(0.5, '#ff6b00');
                gradient.addColorStop(1, 'transparent');
                ctx.fillStyle = gradient;
                ctx.beginPath();
                ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
                ctx.fill();
                ctx.restore();
            }
        });
        
        // Heat haze effect (subtle wave distortion)
        ctx.save();
        ctx.globalAlpha = 0.1;
        for (let i = 0; i < h; i += 20) {
            const offset = Math.sin(this.animationTime / 100 + i / 10) * 3;
            ctx.fillStyle = i % 40 === 0 ? '#ff6b0020' : '#00000020';
            ctx.fillRect(0, i, w, 10);
        }
        ctx.restore();
    }

    // ========================================
    // LEVEL 6: FROZEN TUNDRA
    // ========================================
    initFrozenTundra() {
        // Falling snow
        for (let i = 0; i < 100; i++) {
            this.particles.push({
                x: Math.random() * this.canvas.width,
                y: Math.random() * this.canvas.height,
                vx: (Math.random() - 0.5) * 0.5,
                vy: 0.5 + Math.random() * 1.5,
                size: 2 + Math.random() * 3,
                life: Infinity,
                type: 'snow',
                depth: Math.random() // For size variation
            });
        }
    }

    drawFrozenTundra() {
        const ctx = this.ctx;
        const w = this.canvas.width;
        const h = this.canvas.height;
        
        // Pale blue-white sky
        const skyGrad = ctx.createLinearGradient(0, 0, 0, h);
        skyGrad.addColorStop(0, '#d0e8f0');
        skyGrad.addColorStop(1, '#a0c8d8');
        ctx.fillStyle = skyGrad;
        ctx.fillRect(0, 0, w, h);
        
        // Blizzard fog in midground
        ctx.save();
        ctx.globalAlpha = 0.6;
        const fogGrad = ctx.createLinearGradient(0, h * 0.3, 0, h * 0.6);
        fogGrad.addColorStop(0, 'transparent');
        fogGrad.addColorStop(0.5, '#e0f0f8');
        fogGrad.addColorStop(1, 'transparent');
        ctx.fillStyle = fogGrad;
        ctx.fillRect(0, h * 0.3, w, h * 0.3);
        ctx.restore();
        
        // Ice formations
        ctx.fillStyle = '#b0d8e8';
        ctx.fillRect(w * 0.1, h * 0.4, 80, h * 0.4);
        ctx.fillRect(w * 0.85, h * 0.5, 60, h * 0.3);
        
        // Frozen ground
        const groundGrad = ctx.createLinearGradient(0, h * 0.7, 0, h);
        groundGrad.addColorStop(0, '#c0d8e8');
        groundGrad.addColorStop(1, '#a0b8c8');
        ctx.fillStyle = groundGrad;
        ctx.fillRect(0, h * 0.7, w, h * 0.3);
        
        // Snow particles with depth
        this.particles.forEach(p => {
            if (p.type === 'snow') {
                const size = p.size * (0.5 + p.depth * 0.5);
                ctx.save();
                ctx.globalAlpha = 0.6 + p.depth * 0.4;
                ctx.fillStyle = '#ffffff';
                ctx.shadowBlur = 3;
                ctx.shadowColor = '#ffffff';
                ctx.beginPath();
                ctx.arc(p.x, p.y, size, 0, Math.PI * 2);
                ctx.fill();
                ctx.restore();
            }
        });
    }

    // ========================================
    // LEVEL 10: THRONE OF DARKNESS (FINAL BOSS)
    // ========================================
    initThroneDarkness() {
        // Shadow wisps
        for (let i = 0; i < 30; i++) {
            this.particles.push({
                x: Math.random() * this.canvas.width,
                y: Math.random() * this.canvas.height,
                vx: (Math.random() - 0.5) * 0.3,
                vy: (Math.random() - 0.5) * 0.3,
                size: 5 + Math.random() * 10,
                life: Infinity,
                type: 'shadow-wisp',
                alpha: Math.random() * 0.5
            });
        }
    }

    drawThroneDarkness() {
        const ctx = this.ctx;
        const w = this.canvas.width;
        const h = this.canvas.height;
        
        // Pure darkness
        ctx.fillStyle = '#000000';
        ctx.fillRect(0, 0, w, h);
        
        // Very subtle floor indication
        ctx.save();
        ctx.globalAlpha = 0.1;
        const floorGrad = ctx.createLinearGradient(0, h * 0.7, 0, h);
        floorGrad.addColorStop(0, 'transparent');
        floorGrad.addColorStop(1, '#1a0d2a');
        ctx.fillStyle = floorGrad;
        ctx.fillRect(0, h * 0.7, w, h * 0.3);
        ctx.restore();
        
        // Purple throne glow in center
        ctx.save();
        ctx.globalAlpha = 0.3;
        const throneGrad = ctx.createRadialGradient(w / 2, h * 0.4, 0, w / 2, h * 0.4, w * 0.3);
        throneGrad.addColorStop(0, '#4a0d6d');
        throneGrad.addColorStop(1, 'transparent');
        ctx.fillStyle = throneGrad;
        ctx.fillRect(0, 0, w, h);
        ctx.restore();
        
        // Shadow wisps
        this.particles.forEach(p => {
            if (p.type === 'shadow-wisp') {
                const pulse = Math.sin(this.animationTime / 500 + p.x * 0.01) * 0.3 + 0.5;
                ctx.save();
                ctx.globalAlpha = p.alpha * pulse;
                const gradient = ctx.createRadialGradient(p.x, p.y, 0, p.x, p.y, p.size);
                gradient.addColorStop(0, '#8a3dda');
                gradient.addColorStop(1, 'transparent');
                ctx.fillStyle = gradient;
                ctx.beginPath();
                ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
                ctx.fill();
                ctx.restore();
            }
        });
    }

    // ========================================
    // LEVEL 4: ELVEN RUINS  
    // ========================================
    initElvenRuins() {
        // Magic shimmer dust
        for (let i = 0; i < 40; i++) {
            this.particles.push({
                x: Math.random() * this.canvas.width,
                y: this.canvas.height + Math.random() * 100,
                vx: (Math.random() - 0.5) * 0.2,
                vy: -0.5 - Math.random() * 0.5,
                size: 1 + Math.random() * 2,
                life: Infinity,
                type: 'shimmer',
                hue: Math.random() * 60 + 40
            });
        }
    }

    drawElvenRuins() {
        const ctx = this.ctx;
        const w = this.canvas.width;
        const h = this.canvas.height;
        
        // Teal-accented atmosphere
        const skyGrad = ctx.createLinearGradient(0, 0, 0, h);
        skyGrad.addColorStop(0, '#0d2a2a');
        skyGrad.addColorStop(1, '#1a3d3d');
        ctx.fillStyle = skyGrad;
        ctx.fillRect(0, 0, w, h);
        
        // Stone columns with arches
        ctx.fillStyle = '#2a4d4d';
        ctx.fillRect(w * 0.15, h * 0.2, 40, h * 0.6);
        ctx.fillRect(w * 0.75, h * 0.2, 40, h * 0.6);
        
        // Carved arch
        ctx.strokeStyle = '#3a5d5d';
        ctx.lineWidth = 5;
        ctx.beginPath();
        ctx.arc(w / 2, h * 0.2, w * 0.3, Math.PI, 0);
        ctx.stroke();
        
        // Reflective wet floor
        const floorGrad = ctx.createLinearGradient(0, h * 0.7, 0, h);
        floorGrad.addColorStop(0, '#1a3d3d');
        floorGrad.addColorStop(1, '#0d2a2a');
        ctx.fillStyle = floorGrad;
        ctx.fillRect(0, h * 0.7, w, h * 0.3);
        
        // Magic shimmer particles
        this.particles.forEach(p => {
            if (p.type === 'shimmer') {
                ctx.save();
                ctx.globalAlpha = 0.5 + Math.random() * 0.5;
                ctx.fillStyle = `hsl(${p.hue}, 70%, 60%)`;
                ctx.shadowBlur = 5;
                ctx.shadowColor = ctx.fillStyle;
                ctx.beginPath();
                ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
                ctx.fill();
                ctx.restore();
            }
        });
    }

    // ========================================
    // LEVEL 7: MOUNTAIN PASS
    // ========================================
    initMountainPass() {
        // Wind-blown dust
        for (let i = 0; i < 30; i++) {
            this.particles.push({
                x: -50,
                y: Math.random() * this.canvas.height * 0.7,
                vx: 2 + Math.random() * 2,
                vy: (Math.random() - 0.5) * 0.3,
                size: 1 + Math.random() * 2,
                life: Infinity,
                type: 'dust'
            });
        }
    }

    drawMountainPass() {
        const ctx = this.ctx;
        const w = this.canvas.width;
        const h = this.canvas.height;
        
        // Stormy grey sky
        const skyGrad = ctx.createLinearGradient(0, 0, 0, h);
        skyGrad.addColorStop(0, '#3a3a4a');
        skyGrad.addColorStop(1, '#5a5a6a');
        ctx.fillStyle = skyGrad;
        ctx.fillRect(0, 0, w, h);
        
        // Rocky cliff walls
        ctx.fillStyle = '#2a2a3a';
        ctx.fillRect(0, 0, w * 0.2, h);
        ctx.fillRect(w * 0.8, 0, w * 0.2, h);
        
        // Mountain peaks in background
        ctx.fillStyle = '#1a1a2a';
        ctx.beginPath();
        ctx.moveTo(0, h * 0.5);
        ctx.lineTo(w * 0.3, h * 0.2);
        ctx.lineTo(w * 0.5, h * 0.4);
        ctx.lineTo(w * 0.7, h * 0.15);
        ctx.lineTo(w, h * 0.5);
        ctx.lineTo(w, h);
        ctx.lineTo(0, h);
        ctx.closePath();
        ctx.fill();
        
        // Dirt and stone ground
        const groundGrad = ctx.createLinearGradient(0, h * 0.7, 0, h);
        groundGrad.addColorStop(0, '#4a4a3a');
        groundGrad.addColorStop(1, '#3a3a2a');
        ctx.fillStyle = groundGrad;
        ctx.fillRect(0, h * 0.7, w, h * 0.3);
        
        // Dust particles
        this.particles.forEach(p => {
            if (p.type === 'dust') {
                ctx.save();
                ctx.globalAlpha = 0.4;
                ctx.fillStyle = '#8a8a7a';
                ctx.beginPath();
                ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
                ctx.fill();
                ctx.restore();
            }
        });
    }

    // ========================================
    // LEVEL 8: SHADOW REALM
    // ========================================
    initShadowRealm() {
        // Purple energy orbs
        for (let i = 0; i < 25; i++) {
            this.particles.push({
                x: Math.random() * this.canvas.width,
                y: Math.random() * this.canvas.height,
                vx: (Math.random() - 0.5) * 0.4,
                vy: (Math.random() - 0.5) * 0.4,
                size: 3 + Math.random() * 6,
                life: Infinity,
                type: 'shadow-orb',
                alpha: 0.3 + Math.random() * 0.4
            });
        }
    }

    drawShadowRealm() {
        const ctx = this.ctx;
        const w = this.canvas.width;
        const h = this.canvas.height;
        
        // Deep purple-black void
        const skyGrad = ctx.createRadialGradient(w / 2, h / 2, 0, w / 2, h / 2, w * 0.7);
        skyGrad.addColorStop(0, '#1a0d2a');
        skyGrad.addColorStop(1, '#0d002a');
        ctx.fillStyle = skyGrad;
        ctx.fillRect(0, 0, w, h);
        
        // Floating platforms
        ctx.fillStyle = '#2a1a3a';
        ctx.fillRect(w * 0.1, h * 0.6, w * 0.3, 20);
        ctx.fillRect(w * 0.6, h * 0.5, w * 0.3, 20);
        
        // Shadow energy orbs
        this.particles.forEach(p => {
            if (p.type === 'shadow-orb') {
                const pulse = Math.sin(this.animationTime / 300 + p.x * 0.01) * 0.3 + 0.7;
                ctx.save();
                ctx.globalAlpha = p.alpha * pulse;
                const gradient = ctx.createRadialGradient(p.x, p.y, 0, p.x, p.y, p.size);
                gradient.addColorStop(0, '#8a3dda');
                gradient.addColorStop(0.5, '#5a1daa');
                gradient.addColorStop(1, 'transparent');
                ctx.fillStyle = gradient;
                ctx.beginPath();
                ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
                ctx.fill();
                ctx.restore();
            }
        });
    }

    // ========================================
    // LEVEL 9: DRAGON LAIR
    // ========================================
    initDragonLair() {
        // Floating embers and sparks
        for (let i = 0; i < 60; i++) {
            this.particles.push({
                x: Math.random() * this.canvas.width,
                y: this.canvas.height + Math.random() * 100,
                vx: (Math.random() - 0.5) * 0.8,
                vy: -1 - Math.random() * 3,
                size: 2 + Math.random() * 5,
                life: Infinity,
                type: 'dragon-ember'
            });
        }
    }

    drawDragonLair() {
        const ctx = this.ctx;
        const w = this.canvas.width;
        const h = this.canvas.height;
        
        // Dark cave with fire glow
        const skyGrad = ctx.createLinearGradient(0, 0, 0, h);
        skyGrad.addColorStop(0, '#1a0800');
        skyGrad.addColorStop(1, '#3a1800');
        ctx.fillStyle = skyGrad;
        ctx.fillRect(0, 0, w, h);
        
        // Cave walls
        ctx.fillStyle = '#0d0400';
        ctx.fillRect(0, 0, w * 0.15, h);
        ctx.fillRect(w * 0.85, 0, w * 0.15, h);
        
        // Treasure pile glow
        ctx.save();
        ctx.globalAlpha = 0.3 + Math.sin(this.animationTime / 200) * 0.2;
        const treasureGrad = ctx.createRadialGradient(w / 2, h * 0.8, 0, w / 2, h * 0.8, w * 0.3);
        treasureGrad.addColorStop(0, '#ffeb3b');
        treasureGrad.addColorStop(1, 'transparent');
        ctx.fillStyle = treasureGrad;
        ctx.fillRect(0, 0, w, h);
        ctx.restore();
        
        // Cave floor
        const floorGrad = ctx.createLinearGradient(0, h * 0.7, 0, h);
        floorGrad.addColorStop(0, '#2a1800');
        floorGrad.addColorStop(1, '#1a0800');
        ctx.fillStyle = floorGrad;
        ctx.fillRect(0, h * 0.7, w, h * 0.3);
        
        // Dragon embers
        this.particles.forEach(p => {
            if (p.type === 'dragon-ember') {
                const alpha = 0.5 + Math.random() * 0.5;
                ctx.save();
                ctx.globalAlpha = alpha;
                const gradient = ctx.createRadialGradient(p.x, p.y, 0, p.x, p.y, p.size);
                gradient.addColorStop(0, '#ffeb3b');
                gradient.addColorStop(0.4, '#ff8b00');
                gradient.addColorStop(1, 'transparent');
                ctx.fillStyle = gradient;
                ctx.beginPath();
                ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
                ctx.fill();
                ctx.restore();
            }
        });
    }

    // ========================================
    // HELPER METHODS
    // ========================================
    respawnParticle(p) {
        const w = this.canvas.width;
        const h = this.canvas.height;
        
        switch(p.type) {
            case 'firefly':
                p.x = Math.random() * w;
                p.y = Math.random() * h;
                break;
            case 'torch-left':
            case 'torch-right':
                p.x = p.type === 'torch-left' ? w * 0.2 : w * 0.8;
                p.y = h * 0.3;
                p.life = 1000 + Math.random() * 500;
                break;
            case 'leaf':
                p.x = w + 50;
                p.y = Math.random() * h;
                break;
            case 'shimmer':
                p.x = Math.random() * w;
                p.y = h + 50;
                break;
            case 'ember':
            case 'dragon-ember':
                p.x = Math.random() * w;
                p.y = h + 50;
                break;
            case 'snow':
                p.x = Math.random() * w;
                p.y = -50;
                break;
            case 'dust':
                p.x = -50;
                p.y = Math.random() * h * 0.7;
                break;
            case 'shadow-orb':
            case 'shadow-wisp':
                p.x = Math.random() * w;
                p.y = Math.random() * h;
                break;
        }
    }

    drawVignette() {
        const ctx = this.ctx;
        const w = this.canvas.width;
        const h = this.canvas.height;
        
        // Radial gradient from center (transparent) to edges (dark)
        const vignette = ctx.createRadialGradient(w / 2, h / 2, w * 0.3, w / 2, h / 2, w * 0.8);
        vignette.addColorStop(0, 'transparent');
        vignette.addColorStop(1, 'rgba(0, 0, 0, 0.6)');
        
        ctx.fillStyle = vignette;
        ctx.fillRect(0, 0, w, h);
    }
}

// Export for use
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { ProceduralBackground };
}
