/**
 * PARTICLE SYSTEM FOR COMBAT EFFECTS
 * Handles hit sparks, damage numbers, special ability effects, etc.
 */

class ParticleSystem {
    constructor(canvas) {
        this.canvas = canvas;
        this.ctx = canvas.getContext('2d');
        this.particles = [];
    }

    update(deltaTime) {
        // Update all particles
        for (let i = this.particles.length - 1; i >= 0; i--) {
            const p = this.particles[i];
            
            p.life -= deltaTime;
            if (p.life <= 0) {
                this.particles.splice(i, 1);
                continue;
            }

            // Update position
            p.x += p.vx * (deltaTime / 16);
            p.y += p.vy * (deltaTime / 16);
            
            // Apply gravity if enabled
            if (p.gravity) {
                p.vy += p.gravity * (deltaTime / 16);
            }

            // Apply friction
            if (p.friction) {
                p.vx *= (1 - p.friction);
                p.vy *= (1 - p.friction);
            }

            // Update scale
            if (p.scaleDecay) {
                p.scale = Math.max(0, p.scale - p.scaleDecay * (deltaTime / 16));
            }

            // Update rotation
            if (p.rotationSpeed) {
                p.rotation += p.rotationSpeed * (deltaTime / 16);
            }

            // Update alpha
            if (p.fadeSpeed) {
                p.alpha = Math.max(0, p.alpha - p.fadeSpeed * (deltaTime / 16));
            }
        }
    }

    draw() {
        this.particles.forEach(p => {
            this.ctx.save();
            this.ctx.globalAlpha = p.alpha || 1.0;

            switch(p.type) {
                case 'spark':
                    this.drawSpark(p);
                    break;
                case 'damage-number':
                    this.drawDamageNumber(p);
                    break;
                case 'heal-number':
                    this.drawHealNumber(p);
                    break;
                case 'impact-ring':
                    this.drawImpactRing(p);
                    break;
                case 'slash-trail':
                    this.drawSlashTrail(p);
                    break;
                case 'blood-splatter':
                    this.drawBloodSplatter(p);
                    break;
                case 'magic-circle':
                    this.drawMagicCircle(p);
                    break;
                case 'holy-light':
                    this.drawHolyLight(p);
                    break;
                case 'poison-bubble':
                    this.drawPoisonBubble(p);
                    break;
                case 'fire-burst':
                    this.drawFireBurst(p);
                    break;
                case 'ice-shard':
                    this.drawIceShard(p);
                    break;
                case 'lightning-bolt':
                    this.drawLightningBolt(p);
                    break;
                case 'smoke-puff':
                    this.drawSmokePuff(p);
                    break;
                case 'shield-shimmer':
                    this.drawShieldShimmer(p);
                    break;
            }

            this.ctx.restore();
        });
    }

    // ========================================
    // PARTICLE SPAWNERS
    // ========================================

    emitHitSparks(x, y, count = 10, color = '#FFD700') {
        for (let i = 0; i < count; i++) {
            const angle = Math.random() * Math.PI * 2;
            const speed = 2 + Math.random() * 4;
            
            this.particles.push({
                type: 'spark',
                x: x,
                y: y,
                vx: Math.cos(angle) * speed,
                vy: Math.sin(angle) * speed,
                size: 2 + Math.random() * 3,
                life: 300 + Math.random() * 200,
                color: color,
                gravity: 0.2,
                alpha: 1.0,
                fadeSpeed: 0.02
            });
        }
    }

    emitDamageNumber(x, y, damage, isCritical = false) {
        this.particles.push({
            type: 'damage-number',
            x: x,
            y: y,
            vx: (Math.random() - 0.5) * 2,
            vy: -3 - Math.random() * 2,
            text: `-${damage}`,
            life: 1200,
            scale: isCritical ? 1.5 : 1.0,
            color: isCritical ? '#FF0000' : '#FFFFFF',
            isCritical: isCritical,
            alpha: 1.0,
            fadeSpeed: 0.008,
            gravity: 0.05
        });
    }

    emitHealNumber(x, y, amount) {
        this.particles.push({
            type: 'heal-number',
            x: x,
            y: y,
            vx: 0,
            vy: -2,
            text: `+${amount}`,
            life: 1000,
            scale: 1.0,
            color: '#00FF00',
            alpha: 1.0,
            fadeSpeed: 0.01
        });
    }

    emitImpactRing(x, y, size = 50, color = '#FFFFFF') {
        this.particles.push({
            type: 'impact-ring',
            x: x,
            y: y,
            vx: 0,
            vy: 0,
            size: 10,
            maxSize: size,
            color: color,
            life: 400,
            alpha: 1.0,
            fadeSpeed: 0.025,
            scaleSpeed: 3
        });
    }

    emitSlashTrail(x, y, angle, length = 100, color = '#FFFFFF') {
        this.particles.push({
            type: 'slash-trail',
            x: x,
            y: y,
            angle: angle,
            length: length,
            width: 20,
            color: color,
            life: 200,
            alpha: 1.0,
            fadeSpeed: 0.05
        });
    }

    emitBloodSplatter(x, y, count = 8) {
        for (let i = 0; i < count; i++) {
            const angle = Math.random() * Math.PI * 2;
            const speed = 1 + Math.random() * 3;
            
            this.particles.push({
                type: 'blood-splatter',
                x: x,
                y: y,
                vx: Math.cos(angle) * speed,
                vy: Math.sin(angle) * speed - 2,
                size: 2 + Math.random() * 4,
                life: 600 + Math.random() * 400,
                color: '#8B0000',
                gravity: 0.3,
                alpha: 1.0,
                fadeSpeed: 0.01
            });
        }
    }

    emitMagicCircle(x, y, radius = 80, color = '#9370DB') {
        this.particles.push({
            type: 'magic-circle',
            x: x,
            y: y,
            radius: radius,
            color: color,
            life: 800,
            alpha: 1.0,
            fadeSpeed: 0.0125,
            rotation: 0,
            rotationSpeed: 0.05
        });
    }

    emitHolyLight(x, y) {
        this.particles.push({
            type: 'holy-light',
            x: x,
            y: y,
            radius: 100,
            color: '#FFD700',
            life: 600,
            alpha: 0.8,
            fadeSpeed: 0.013,
            pulseSpeed: 0.1
        });
    }

    emitPoisonBubbles(x, y, count = 12) {
        for (let i = 0; i < count; i++) {
            const angle = Math.random() * Math.PI * 2;
            const speed = 0.5 + Math.random() * 1.5;
            
            this.particles.push({
                type: 'poison-bubble',
                x: x,
                y: y,
                vx: Math.cos(angle) * speed,
                vy: Math.sin(angle) * speed - 1,
                size: 3 + Math.random() * 5,
                life: 1000 + Math.random() * 500,
                color: '#32CD32',
                alpha: 0.7,
                fadeSpeed: 0.007,
                gravity: -0.05 // Float upward
            });
        }
    }

    emitFireBurst(x, y, count = 15) {
        for (let i = 0; i < count; i++) {
            const angle = Math.random() * Math.PI * 2;
            const speed = 2 + Math.random() * 4;
            
            this.particles.push({
                type: 'fire-burst',
                x: x,
                y: y,
                vx: Math.cos(angle) * speed,
                vy: Math.sin(angle) * speed,
                size: 4 + Math.random() * 6,
                life: 500 + Math.random() * 300,
                color: i % 2 === 0 ? '#FF4500' : '#FFD700',
                alpha: 1.0,
                fadeSpeed: 0.02,
                scaleDecay: 0.05
            });
        }
    }

    emitIceShards(x, y, count = 8) {
        for (let i = 0; i < count; i++) {
            const angle = Math.random() * Math.PI * 2;
            const speed = 3 + Math.random() * 3;
            
            this.particles.push({
                type: 'ice-shard',
                x: x,
                y: y,
                vx: Math.cos(angle) * speed,
                vy: Math.sin(angle) * speed,
                size: 4 + Math.random() * 8,
                life: 700 + Math.random() * 300,
                rotation: Math.random() * Math.PI * 2,
                rotationSpeed: (Math.random() - 0.5) * 0.2,
                alpha: 0.9,
                fadeSpeed: 0.012,
                gravity: 0.15
            });
        }
    }

    emitLightningBolt(startX, startY, endX, endY, segments = 6) {
        this.particles.push({
            type: 'lightning-bolt',
            startX: startX,
            startY: startY,
            endX: endX,
            endY: endY,
            segments: segments,
            life: 150,
            alpha: 1.0,
            fadeSpeed: 0.067,
            jitter: []
        });
        
        // Generate random jitter points
        const lastParticle = this.particles[this.particles.length - 1];
        for (let i = 0; i < segments; i++) {
            lastParticle.jitter.push({
                x: (Math.random() - 0.5) * 30,
                y: (Math.random() - 0.5) * 30
            });
        }
    }

    emitSmokePuff(x, y, count = 5) {
        for (let i = 0; i < count; i++) {
            const angle = Math.random() * Math.PI * 2;
            const speed = 0.3 + Math.random() * 0.7;
            
            this.particles.push({
                type: 'smoke-puff',
                x: x,
                y: y,
                vx: Math.cos(angle) * speed,
                vy: Math.sin(angle) * speed - 1,
                size: 15 + Math.random() * 20,
                life: 1000 + Math.random() * 500,
                alpha: 0.5,
                fadeSpeed: 0.005,
                scale: 1.0,
                scaleSpeed: 0.02
            });
        }
    }

    emitShieldShimmer(x, y, radius = 60) {
        for (let i = 0; i < 8; i++) {
            const angle = (i / 8) * Math.PI * 2;
            
            this.particles.push({
                type: 'shield-shimmer',
                x: x + Math.cos(angle) * radius,
                y: y + Math.sin(angle) * radius,
                angle: angle,
                centerX: x,
                centerY: y,
                radius: radius,
                life: 600,
                alpha: 1.0,
                fadeSpeed: 0.017,
                size: 6
            });
        }
    }

    // ========================================
    // PARTICLE RENDERERS
    // ========================================

    drawSpark(p) {
        const gradient = this.ctx.createRadialGradient(p.x, p.y, 0, p.x, p.y, p.size);
        gradient.addColorStop(0, p.color);
        gradient.addColorStop(1, 'transparent');
        this.ctx.fillStyle = gradient;
        this.ctx.beginPath();
        this.ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
        this.ctx.fill();
    }

    drawDamageNumber(p) {
        this.ctx.font = `bold ${24 * p.scale}px Arial`;
        this.ctx.fillStyle = p.color;
        this.ctx.strokeStyle = '#000000';
        this.ctx.lineWidth = 3;
        this.ctx.textAlign = 'center';
        this.ctx.strokeText(p.text, p.x, p.y);
        this.ctx.fillText(p.text, p.x, p.y);
        
        if (p.isCritical) {
            this.ctx.shadowBlur = 10;
            this.ctx.shadowColor = p.color;
            this.ctx.fillText(p.text, p.x, p.y);
        }
    }

    drawHealNumber(p) {
        this.ctx.font = `bold ${20 * p.scale}px Arial`;
        this.ctx.fillStyle = p.color;
        this.ctx.strokeStyle = '#004400';
        this.ctx.lineWidth = 2;
        this.ctx.textAlign = 'center';
        this.ctx.strokeText(p.text, p.x, p.y);
        this.ctx.fillText(p.text, p.x, p.y);
        this.ctx.shadowBlur = 8;
        this.ctx.shadowColor = p.color;
        this.ctx.fillText(p.text, p.x, p.y);
    }

    drawImpactRing(p) {
        const progress = 1 - (p.life / 400);
        p.size = 10 + (p.maxSize - 10) * progress;
        
        this.ctx.strokeStyle = p.color;
        this.ctx.lineWidth = 3;
        this.ctx.beginPath();
        this.ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
        this.ctx.stroke();
    }

    drawSlashTrail(p) {
        const endX = p.x + Math.cos(p.angle) * p.length;
        const endY = p.y + Math.sin(p.angle) * p.length;
        
        const gradient = this.ctx.createLinearGradient(p.x, p.y, endX, endY);
        gradient.addColorStop(0, 'transparent');
        gradient.addColorStop(0.5, p.color);
        gradient.addColorStop(1, 'transparent');
        
        this.ctx.strokeStyle = gradient;
        this.ctx.lineWidth = p.width;
        this.ctx.lineCap = 'round';
        this.ctx.beginPath();
        this.ctx.moveTo(p.x, p.y);
        this.ctx.lineTo(endX, endY);
        this.ctx.stroke();
    }

    drawBloodSplatter(p) {
        this.ctx.fillStyle = p.color;
        this.ctx.beginPath();
        this.ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
        this.ctx.fill();
    }

    drawMagicCircle(p) {
        this.ctx.save();
        this.ctx.translate(p.x, p.y);
        this.ctx.rotate(p.rotation);
        
        // Outer circle
        this.ctx.strokeStyle = p.color;
        this.ctx.lineWidth = 2;
        this.ctx.beginPath();
        this.ctx.arc(0, 0, p.radius, 0, Math.PI * 2);
        this.ctx.stroke();
        
        // Inner circle
        this.ctx.beginPath();
        this.ctx.arc(0, 0, p.radius * 0.7, 0, Math.PI * 2);
        this.ctx.stroke();
        
        // Runes/symbols
        for (let i = 0; i < 6; i++) {
            const angle = (i / 6) * Math.PI * 2;
            const x = Math.cos(angle) * p.radius * 0.85;
            const y = Math.sin(angle) * p.radius * 0.85;
            
            this.ctx.beginPath();
            this.ctx.arc(x, y, 5, 0, Math.PI * 2);
            this.ctx.fill();
        }
        
        this.ctx.restore();
    }

    drawHolyLight(p) {
        const gradient = this.ctx.createRadialGradient(p.x, p.y, 0, p.x, p.y, p.radius);
        gradient.addColorStop(0, p.color);
        gradient.addColorStop(0.5, 'rgba(255, 215, 0, 0.3)');
        gradient.addColorStop(1, 'transparent');
        
        this.ctx.fillStyle = gradient;
        this.ctx.beginPath();
        this.ctx.arc(p.x, p.y, p.radius, 0, Math.PI * 2);
        this.ctx.fill();
        
        // Light rays
        for (let i = 0; i < 8; i++) {
            const angle = (i / 8) * Math.PI * 2 + (Date.now() / 1000);
            const x = p.x + Math.cos(angle) * p.radius;
            const y = p.y + Math.sin(angle) * p.radius;
            
            this.ctx.strokeStyle = `rgba(255, 215, 0, ${p.alpha * 0.5})`;
            this.ctx.lineWidth = 2;
            this.ctx.beginPath();
            this.ctx.moveTo(p.x, p.y);
            this.ctx.lineTo(x, y);
            this.ctx.stroke();
        }
    }

    drawPoisonBubble(p) {
        const gradient = this.ctx.createRadialGradient(
            p.x - p.size * 0.3, p.y - p.size * 0.3, 0,
            p.x, p.y, p.size
        );
        gradient.addColorStop(0, '#90EE90');
        gradient.addColorStop(0.7, p.color);
        gradient.addColorStop(1, 'rgba(50, 205, 50, 0)');
        
        this.ctx.fillStyle = gradient;
        this.ctx.beginPath();
        this.ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
        this.ctx.fill();
        
        // Highlight
        this.ctx.fillStyle = 'rgba(255, 255, 255, 0.3)';
        this.ctx.beginPath();
        this.ctx.arc(p.x - p.size * 0.3, p.y - p.size * 0.3, p.size * 0.3, 0, Math.PI * 2);
        this.ctx.fill();
    }

    drawFireBurst(p) {
        const gradient = this.ctx.createRadialGradient(p.x, p.y, 0, p.x, p.y, p.size);
        gradient.addColorStop(0, '#FFFF00');
        gradient.addColorStop(0.5, p.color);
        gradient.addColorStop(1, 'transparent');
        
        this.ctx.fillStyle = gradient;
        this.ctx.beginPath();
        this.ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
        this.ctx.fill();
    }

    drawIceShard(p) {
        this.ctx.save();
        this.ctx.translate(p.x, p.y);
        this.ctx.rotate(p.rotation);
        
        const gradient = this.ctx.createLinearGradient(-p.size / 2, 0, p.size / 2, 0);
        gradient.addColorStop(0, 'transparent');
        gradient.addColorStop(0.5, '#B0E0E6');
        gradient.addColorStop(1, 'transparent');
        
        this.ctx.fillStyle = gradient;
        this.ctx.beginPath();
        this.ctx.moveTo(0, -p.size);
        this.ctx.lineTo(p.size * 0.3, 0);
        this.ctx.lineTo(0, p.size);
        this.ctx.lineTo(-p.size * 0.3, 0);
        this.ctx.closePath();
        this.ctx.fill();
        
        // Highlight
        this.ctx.fillStyle = 'rgba(255, 255, 255, 0.6)';
        this.ctx.fillRect(-p.size * 0.1, -p.size * 0.6, p.size * 0.2, p.size * 0.4);
        
        this.ctx.restore();
    }

    drawLightningBolt(p) {
        const points = [];
        points.push({ x: p.startX, y: p.startY });
        
        for (let i = 0; i < p.segments; i++) {
            const t = (i + 1) / (p.segments + 1);
            const x = p.startX + (p.endX - p.startX) * t + p.jitter[i].x;
            const y = p.startY + (p.endY - p.startY) * t + p.jitter[i].y;
            points.push({ x, y });
        }
        
        points.push({ x: p.endX, y: p.endY });
        
        // Draw glow
        this.ctx.shadowBlur = 15;
        this.ctx.shadowColor = '#00BFFF';
        this.ctx.strokeStyle = '#FFFFFF';
        this.ctx.lineWidth = 3;
        this.ctx.lineCap = 'round';
        
        this.ctx.beginPath();
        this.ctx.moveTo(points[0].x, points[0].y);
        for (let i = 1; i < points.length; i++) {
            this.ctx.lineTo(points[i].x, points[i].y);
        }
        this.ctx.stroke();
        
        // Draw core
        this.ctx.shadowBlur = 0;
        this.ctx.strokeStyle = '#00BFFF';
        this.ctx.lineWidth = 1;
        this.ctx.stroke();
    }

    drawSmokePuff(p) {
        p.size += p.scaleSpeed;
        
        const gradient = this.ctx.createRadialGradient(p.x, p.y, 0, p.x, p.y, p.size);
        gradient.addColorStop(0, `rgba(100, 100, 100, ${p.alpha})`);
        gradient.addColorStop(1, 'transparent');
        
        this.ctx.fillStyle = gradient;
        this.ctx.beginPath();
        this.ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
        this.ctx.fill();
    }

    drawShieldShimmer(p) {
        // Rotate around center
        p.angle += 0.05;
        p.x = p.centerX + Math.cos(p.angle) * p.radius;
        p.y = p.centerY + Math.sin(p.angle) * p.radius;
        
        const gradient = this.ctx.createRadialGradient(p.x, p.y, 0, p.x, p.y, p.size);
        gradient.addColorStop(0, '#4169E1');
        gradient.addColorStop(1, 'transparent');
        
        this.ctx.fillStyle = gradient;
        this.ctx.shadowBlur = 10;
        this.ctx.shadowColor = '#4169E1';
        this.ctx.beginPath();
        this.ctx.arc(p.x, p.y, p.size, 0, Math.PI * 2);
        this.ctx.fill();
    }

    // Clear all particles
    clear() {
        this.particles = [];
    }
}

// Export for use
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { ParticleSystem };
}
