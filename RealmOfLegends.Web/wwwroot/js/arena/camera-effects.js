/**
 * CAMERA EFFECTS SYSTEM
 * Screen shake, zoom, slow-motion, and other cinematic effects
 */

class CameraEffects {
    constructor(canvas) {
        this.canvas = canvas;
        this.ctx = canvas.getContext('2d');
        
        // Screen shake
        this.shakeAmount = 0;
        this.shakeDuration = 0;
        this.shakeX = 0;
        this.shakeY = 0;
        
        // Zoom/pulse
        this.zoomTarget = 1.0;
        this.zoomCurrent = 1.0;
        this.zoomSpeed = 0.05;
        
        // Flash
        this.flashAlpha = 0;
        this.flashColor = '#FFFFFF';
        
        // Freeze frame
        this.freezeFrameDuration = 0;
        this.isFrozen = false;
        
        // Chromatic aberration
        this.chromaticAmount = 0;
        
        // Slow motion
        this.timeScale = 1.0;
        this.targetTimeScale = 1.0;
    }

    update(deltaTime) {
        // Update screen shake
        if (this.shakeDuration > 0) {
            this.shakeDuration -= deltaTime;
            const decay = this.shakeDuration / 300; // Normalize
            this.shakeX = (Math.random() - 0.5) * this.shakeAmount * decay;
            this.shakeY = (Math.random() - 0.5) * this.shakeAmount * decay;
        } else {
            this.shakeX = 0;
            this.shakeY = 0;
        }

        // Update zoom
        if (Math.abs(this.zoomCurrent - this.zoomTarget) > 0.001) {
            this.zoomCurrent += (this.zoomTarget - this.zoomCurrent) * this.zoomSpeed;
        }

        // Update flash
        if (this.flashAlpha > 0) {
            this.flashAlpha -= deltaTime / 200; // Fade in 200ms
            if (this.flashAlpha < 0) this.flashAlpha = 0;
        }

        // Update freeze frame
        if (this.freezeFrameDuration > 0) {
            this.freezeFrameDuration -= deltaTime;
            this.isFrozen = this.freezeFrameDuration > 0;
        }

        // Update chromatic aberration
        if (this.chromaticAmount > 0) {
            this.chromaticAmount -= deltaTime / 300;
            if (this.chromaticAmount < 0) this.chromaticAmount = 0;
        }

        // Update time scale
        if (Math.abs(this.timeScale - this.targetTimeScale) > 0.01) {
            this.timeScale += (this.targetTimeScale - this.timeScale) * 0.05;
        }
    }

    // ========================================
    // EFFECT TRIGGERS
    // ========================================

    shake(amount = 10, duration = 300) {
        this.shakeAmount = amount;
        this.shakeDuration = duration;
    }

    flash(color = '#FFFFFF', intensity = 1.0) {
        this.flashColor = color;
        this.flashAlpha = intensity;
    }

    zoom(targetZoom, duration = 500) {
        this.zoomTarget = targetZoom;
        
        // Auto reset after duration
        setTimeout(() => {
            this.zoomTarget = 1.0;
        }, duration);
    }

    freezeFrame(duration = 100) {
        this.freezeFrameDuration = duration;
        this.isFrozen = true;
    }

    chromaticAberration(amount = 5) {
        this.chromaticAmount = amount;
    }

    slowMotion(scale = 0.3, duration = 1000) {
        this.targetTimeScale = scale;
        
        setTimeout(() => {
            this.targetTimeScale = 1.0;
        }, duration);
    }

    // ========================================
    // RENDERING EFFECTS
    // ========================================

    applyTransform() {
        this.ctx.save();
        
        // Apply shake
        this.ctx.translate(this.shakeX, this.shakeY);
        
        // Apply zoom (from center)
        if (this.zoomCurrent !== 1.0) {
            const centerX = this.canvas.width / 2;
            const centerY = this.canvas.height / 2;
            this.ctx.translate(centerX, centerY);
            this.ctx.scale(this.zoomCurrent, this.zoomCurrent);
            this.ctx.translate(-centerX, -centerY);
        }
    }

    removeTransform() {
        this.ctx.restore();
    }

    renderPostEffects() {
        // Flash effect
        if (this.flashAlpha > 0) {
            this.ctx.save();
            this.ctx.globalAlpha = this.flashAlpha;
            this.ctx.fillStyle = this.flashColor;
            this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
            this.ctx.restore();
        }

        // Chromatic aberration (simplified version)
        if (this.chromaticAmount > 0) {
            this.renderChromaticAberration();
        }
    }

    renderChromaticAberration() {
        // Get current canvas content
        const imageData = this.ctx.getImageData(0, 0, this.canvas.width, this.canvas.height);
        const data = imageData.data;
        
        const offset = Math.floor(this.chromaticAmount);
        
        // Create a copy for red channel shift
        const shifted = new Uint8ClampedArray(data);
        
        // Shift red channel right
        for (let y = 0; y < this.canvas.height; y++) {
            for (let x = this.canvas.width - 1; x >= offset; x--) {
                const targetIdx = (y * this.canvas.width + x) * 4;
                const sourceIdx = (y * this.canvas.width + (x - offset)) * 4;
                shifted[targetIdx] = data[sourceIdx]; // Red
            }
        }
        
        // Apply
        for (let i = 0; i < data.length; i += 4) {
            data[i] = shifted[i]; // Use shifted red
            // Green and blue stay the same
        }
        
        this.ctx.putImageData(imageData, 0, 0);
    }

    // ========================================
    // CINEMATIC EFFECTS
    // ========================================

    impactEffect(intensity = 1.0) {
        this.shake(15 * intensity, 200);
        this.flash('#FFFFFF', 0.6 * intensity);
        this.freezeFrame(60 * intensity);
        this.zoom(1.1, 200);
    }

    criticalHitEffect() {
        this.shake(20, 300);
        this.flash('#FF0000', 0.8);
        this.freezeFrame(100);
        this.chromaticAberration(8);
        this.slowMotion(0.2, 500);
    }

    heavyImpact() {
        this.shake(25, 400);
        this.flash('#FFFF00', 0.7);
        this.freezeFrame(120);
        this.zoom(1.15, 300);
    }

    deathEffect() {
        this.shake(30, 500);
        this.slowMotion(0.1, 2000);
        this.flash('#000000', 0.5);
    }

    victoryEffect() {
        this.flash('#FFD700', 0.8);
        this.zoom(1.2, 1000);
    }

    specialAbilityEffect(abilityType) {
        switch(abilityType) {
            case 'warrior':
                this.shake(18, 250);
                this.flash('#FF4500', 0.5);
                break;
            case 'mage':
                this.flash('#9370DB', 0.6);
                this.chromaticAberration(6);
                break;
            case 'rogue':
                this.slowMotion(0.4, 800);
                this.flash('#696969', 0.3);
                break;
            case 'paladin':
                this.flash('#FFD700', 0.8);
                this.zoom(1.1, 500);
                break;
        }
    }

    // ========================================
    // GETTERS
    // ========================================

    getTimeScale() {
        return this.timeScale;
    }

    isCurrentlyFrozen() {
        return this.isFrozen;
    }

    getEffectiveShake() {
        return { x: this.shakeX, y: this.shakeY };
    }
}

// Export for use
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { CameraEffects };
}
