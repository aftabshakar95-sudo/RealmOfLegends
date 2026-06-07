/**
 * REALM OF LEGENDS - CANVAS ARENA ENGINE
 * Core classes for sprite animation, combat entities, and rendering
 */

// ========================================
// SPRITE SHEET CLASS
// ========================================
class SpriteSheet {
    constructor(imagePath, frameWidth, frameHeight) {
        this.image = new Image();
        this.imagePath = imagePath;
        this.frameWidth = frameWidth;
        this.frameHeight = frameHeight;
        this.loaded = false;
        this.framesPerRow = 0;
    }

    load() {
        return new Promise((resolve) => {
            this.image.onload = () => {
                this.loaded = true;
                this.framesPerRow = Math.floor(this.image.width / this.frameWidth);
                resolve(this);
            };
            this.image.onerror = () => {
                console.warn(`Sprite not found: ${this.imagePath}, using placeholder`);
                resolve(this); // Resolve anyway to allow fallback to run
            };
            
            this.image.src = this.imagePath;
            
            // Timeout fallback just in case
            setTimeout(() => {
                if (!this.loaded) {
                    resolve(this);
                }
            }, 1000);
        });
    }

    drawFrame(ctx, frameIndex, x, y, scale = 1, flipH = false) {
        if (!this.loaded || !this.image.complete) {
            // Draw animated 2D character as fallback
            this.drawAnimatedCharacter(ctx, x, y, scale, flipH, frameIndex);
            return;
        }

        const row = Math.floor(frameIndex / this.framesPerRow);
        const col = frameIndex % this.framesPerRow;
        const sx = col * this.frameWidth;
        const sy = row * this.frameHeight;

        ctx.save();
        ctx.translate(x, y);
        if (flipH) {
            ctx.scale(-1, 1);
        }
        ctx.scale(scale, scale);
        ctx.drawImage(
            this.image,
            sx, sy, this.frameWidth, this.frameHeight,
            -this.frameWidth / 2, -this.frameHeight / 2, this.frameWidth, this.frameHeight
        );
        ctx.restore();
    }

    drawAnimatedCharacter(ctx, x, y, scale, flipH, frameIndex) {
        // Determine character type from sprite path or use default
        const isEnemy = this.imagePath.includes('enemy');
        
        if (isEnemy) {
            this.drawEnemyCharacter(ctx, x, y, scale, flipH, frameIndex);
        } else {
            // Draw player character (warrior style by default)
            this.drawPlayerCharacter(ctx, x, y, scale, flipH, frameIndex);
        }
    }

    drawPlayerCharacter(ctx, x, y, scale, flipH, frameIndex) {
        const size = 60 * scale;
        const bounce = Math.sin(frameIndex * 0.2) * 3;
        
        ctx.save();
        ctx.translate(x, y - bounce);
        if (flipH) {
            ctx.scale(-1, 1);
        }
        
        // Shadow
        ctx.fillStyle = 'rgba(0, 0, 0, 0.3)';
        ctx.beginPath();
        ctx.ellipse(0, size * 0.7, size * 0.4, size * 0.1, 0, 0, Math.PI * 2);
        ctx.fill();
        
        // Body (armored torso)
        const gradient = ctx.createLinearGradient(-size * 0.25, -size * 0.3, size * 0.25, size * 0.3);
        gradient.addColorStop(0, '#4A4A4A');
        gradient.addColorStop(0.5, '#6A6A6A');
        gradient.addColorStop(1, '#3A3A3A');
        ctx.fillStyle = gradient;
        ctx.fillRect(-size * 0.25, -size * 0.3, size * 0.5, size * 0.6);
        
        // Armor details
        ctx.strokeStyle = '#8A8A8A';
        ctx.lineWidth = 2;
        ctx.strokeRect(-size * 0.25, -size * 0.3, size * 0.5, size * 0.6);
        
        // Head with helmet
        ctx.fillStyle = '#FFE4C4';
        ctx.beginPath();
        ctx.arc(0, -size * 0.5, size * 0.22, 0, Math.PI * 2);
        ctx.fill();
        
        // Helmet
        ctx.fillStyle = '#4A4A4A';
        ctx.beginPath();
        ctx.ellipse(0, -size * 0.55, size * 0.28, size * 0.15, 0, Math.PI, 0, true);
        ctx.fill();
        
        // Eyes
        ctx.fillStyle = '#000';
        ctx.beginPath();
        ctx.arc(-size * 0.08, -size * 0.48, size * 0.05, 0, Math.PI * 2);
        ctx.arc(size * 0.08, -size * 0.48, size * 0.05, 0, Math.PI * 2);
        ctx.fill();
        
        // Arms with animation
        const armSwing = Math.sin(frameIndex * 0.3) * 0.3;
        ctx.strokeStyle = '#4A4A4A';
        ctx.lineWidth = size * 0.18;
        ctx.lineCap = 'round';
        
        // Left arm
        ctx.beginPath();
        ctx.moveTo(-size * 0.25, -size * 0.15);
        ctx.lineTo(-size * 0.45, size * 0.15 + armSwing);
        ctx.stroke();
        
        // Right arm (sword arm)
        ctx.beginPath();
        ctx.moveTo(size * 0.25, -size * 0.15);
        ctx.lineTo(size * 0.45, size * 0.1 - armSwing);
        ctx.stroke();
        
        // Legs with animation
        ctx.strokeStyle = '#3A3A3A';
        ctx.lineWidth = size * 0.16;
        const legSwing = Math.sin(frameIndex * 0.3) * 0.15;
        
        // Left leg
        ctx.beginPath();
        ctx.moveTo(-size * 0.1, size * 0.3);
        ctx.lineTo(-size * 0.15, size * 0.65 + legSwing);
        ctx.stroke();
        
        // Right leg
        ctx.beginPath();
        ctx.moveTo(size * 0.1, size * 0.3);
        ctx.lineTo(size * 0.15, size * 0.65 - legSwing);
        ctx.stroke();
        
        // Sword
        ctx.strokeStyle = '#C0C0C0';
        ctx.lineWidth = size * 0.06;
        ctx.beginPath();
        ctx.moveTo(size * 0.45, size * 0.1 - armSwing);
        ctx.lineTo(size * 0.6, size * 0.5 - armSwing);
        ctx.stroke();
        
        // Sword glow
        ctx.shadowBlur = 10;
        ctx.shadowColor = '#FFD700';
        ctx.strokeStyle = '#FFD700';
        ctx.lineWidth = size * 0.03;
        ctx.beginPath();
        ctx.moveTo(size * 0.45, size * 0.1 - armSwing);
        ctx.lineTo(size * 0.6, size * 0.5 - armSwing);
        ctx.stroke();
        ctx.shadowBlur = 0;
        
        // Sword handle
        ctx.strokeStyle = '#8B4513';
        ctx.lineWidth = size * 0.1;
        ctx.beginPath();
        ctx.moveTo(size * 0.43, size * 0.08 - armSwing);
        ctx.lineTo(size * 0.43, size * 0.18 - armSwing);
        ctx.stroke();
        
        // Shield on back
        ctx.fillStyle = '#4169E1';
        ctx.beginPath();
        ctx.arc(-size * 0.3, 0, size * 0.2, 0, Math.PI * 2);
        ctx.fill();
        ctx.strokeStyle = '#FFD700';
        ctx.lineWidth = 3;
        ctx.stroke();
        
        ctx.restore();
    }

    drawEnemyCharacter(ctx, x, y, scale, flipH, frameIndex) {
        const size = 65 * scale;
        const bounce = Math.sin(frameIndex * 0.25) * 4;
        
        ctx.save();
        ctx.translate(x, y - bounce);
        if (flipH) {
            ctx.scale(-1, 1);
        }
        
        // Shadow
        ctx.fillStyle = 'rgba(0, 0, 0, 0.4)';
        ctx.beginPath();
        ctx.ellipse(0, size * 0.7, size * 0.45, size * 0.12, 0, 0, Math.PI * 2);
        ctx.fill();
        
        // Body (darker, more menacing)
        const gradient = ctx.createLinearGradient(-size * 0.3, -size * 0.35, size * 0.3, size * 0.35);
        gradient.addColorStop(0, '#2A2A2A');
        gradient.addColorStop(0.5, '#3A3A3A');
        gradient.addColorStop(1, '#1A1A1A');
        ctx.fillStyle = gradient;
        ctx.fillRect(-size * 0.3, -size * 0.35, size * 0.6, size * 0.7);
        
        // Spikes on shoulders
        ctx.fillStyle = '#666';
        ctx.beginPath();
        ctx.moveTo(-size * 0.35, -size * 0.3);
        ctx.lineTo(-size * 0.45, -size * 0.45);
        ctx.lineTo(-size * 0.25, -size * 0.35);
        ctx.fill();
        
        ctx.beginPath();
        ctx.moveTo(size * 0.35, -size * 0.3);
        ctx.lineTo(size * 0.45, -size * 0.45);
        ctx.lineTo(size * 0.25, -size * 0.35);
        ctx.fill();
        
        // Head (monstrous)
        ctx.fillStyle = '#2F4F2F';
        ctx.beginPath();
        ctx.arc(0, -size * 0.5, size * 0.28, 0, Math.PI * 2);
        ctx.fill();
        
        // Horns
        ctx.fillStyle = '#8B0000';
        ctx.beginPath();
        ctx.moveTo(-size * 0.2, -size * 0.65);
        ctx.lineTo(-size * 0.15, -size * 0.8);
        ctx.lineTo(-size * 0.1, -size * 0.65);
        ctx.fill();
        
        ctx.beginPath();
        ctx.moveTo(size * 0.2, -size * 0.65);
        ctx.lineTo(size * 0.15, -size * 0.8);
        ctx.lineTo(size * 0.1, -size * 0.65);
        ctx.fill();
        
        // Glowing red eyes
        ctx.fillStyle = '#FF0000';
        ctx.shadowBlur = 15;
        ctx.shadowColor = '#FF0000';
        ctx.beginPath();
        ctx.arc(-size * 0.1, -size * 0.5, size * 0.06, 0, Math.PI * 2);
        ctx.arc(size * 0.1, -size * 0.5, size * 0.06, 0, Math.PI * 2);
        ctx.fill();
        ctx.shadowBlur = 0;
        
        // Fangs
        ctx.fillStyle = '#FFF';
        ctx.beginPath();
        ctx.moveTo(-size * 0.08, -size * 0.42);
        ctx.lineTo(-size * 0.05, -size * 0.35);
        ctx.lineTo(-size * 0.02, -size * 0.42);
        ctx.fill();
        
        ctx.beginPath();
        ctx.moveTo(size * 0.08, -size * 0.42);
        ctx.lineTo(size * 0.05, -size * 0.35);
        ctx.lineTo(size * 0.02, -size * 0.42);
        ctx.fill();
        
        // Arms with claws
        const armSwing = Math.sin(frameIndex * 0.35) * 0.35;
        ctx.strokeStyle = '#2F4F2F';
        ctx.lineWidth = size * 0.2;
        ctx.lineCap = 'round';
        
        // Left arm
        ctx.beginPath();
        ctx.moveTo(-size * 0.3, -size * 0.2);
        ctx.lineTo(-size * 0.5, size * 0.2 + armSwing);
        ctx.stroke();
        
        // Claws on left hand
        ctx.strokeStyle = '#666';
        ctx.lineWidth = size * 0.04;
        for (let i = 0; i < 3; i++) {
            ctx.beginPath();
            ctx.moveTo(-size * 0.5 + i * size * 0.06, size * 0.2 + armSwing);
            ctx.lineTo(-size * 0.52 + i * size * 0.06, size * 0.3 + armSwing);
            ctx.stroke();
        }
        
        // Right arm
        ctx.strokeStyle = '#2F4F2F';
        ctx.lineWidth = size * 0.2;
        ctx.beginPath();
        ctx.moveTo(size * 0.3, -size * 0.2);
        ctx.lineTo(size * 0.5, size * 0.2 - armSwing);
        ctx.stroke();
        
        // Claws on right hand
        ctx.strokeStyle = '#666';
        ctx.lineWidth = size * 0.04;
        for (let i = 0; i < 3; i++) {
            ctx.beginPath();
            ctx.moveTo(size * 0.5 - i * size * 0.06, size * 0.2 - armSwing);
            ctx.lineTo(size * 0.52 - i * size * 0.06, size * 0.3 - armSwing);
            ctx.stroke();
        }
        
        // Legs
        ctx.strokeStyle = '#1A1A1A';
        ctx.lineWidth = size * 0.18;
        const legSwing = Math.sin(frameIndex * 0.35) * 0.2;
        
        ctx.beginPath();
        ctx.moveTo(-size * 0.12, size * 0.35);
        ctx.lineTo(-size * 0.18, size * 0.7 + legSwing);
        ctx.stroke();
        
        ctx.beginPath();
        ctx.moveTo(size * 0.12, size * 0.35);
        ctx.lineTo(size * 0.18, size * 0.7 - legSwing);
        ctx.stroke();
        
        // Red aura
        ctx.strokeStyle = 'rgba(255, 0, 0, 0.3)';
        ctx.lineWidth = 3;
        ctx.shadowBlur = 20;
        ctx.shadowColor = '#FF0000';
        ctx.strokeRect(-size * 0.4, -size * 0.6, size * 0.8, size * 1.3);
        ctx.shadowBlur = 0;
        
        ctx.restore();
    }
}

// ========================================
// ANIMATION CONTROLLER CLASS
// ========================================
class AnimationController {
    constructor() {
        this.animations = {};
        this.currentAnimation = null;
        this.currentFrame = 0;
        this.frameTimer = 0;
        this.isPlaying = false;
        this.loop = true;
        this.onComplete = null;
        this.priority = 0;
    }

    addAnimation(name, startFrame, frameCount, fps, loop = true, priority = 0) {
        this.animations[name] = {
            startFrame,
            frameCount,
            fps,
            loop,
            priority,
            frameDuration: 1000 / fps
        };
    }

    play(animationName, onComplete = null) {
        const anim = this.animations[animationName];
        if (!anim) return false;

        // Check priority
        if (this.currentAnimation && this.isPlaying) {
            const currentPriority = this.animations[this.currentAnimation]?.priority || 0;
            if (currentPriority > anim.priority) {
                return false; // Current animation has higher priority
            }
        }

        this.currentAnimation = animationName;
        this.currentFrame = 0;
        this.frameTimer = 0;
        this.isPlaying = true;
        this.loop = anim.loop;
        this.onComplete = onComplete;
        return true;
    }

    update(deltaTime) {
        if (!this.isPlaying || !this.currentAnimation) return this.getCurrentFrameIndex();

        const anim = this.animations[this.currentAnimation];
        if (!anim) return 0;

        this.frameTimer += deltaTime;

        if (this.frameTimer >= anim.frameDuration) {
            this.frameTimer = 0;
            this.currentFrame++;

            if (this.currentFrame >= anim.frameCount) {
                if (this.loop) {
                    this.currentFrame = 0;
                } else {
                    this.currentFrame = anim.frameCount - 1;
                    this.isPlaying = false;
                    if (this.onComplete) {
                        this.onComplete();
                        this.onComplete = null;
                    }
                }
            }
        }

        return this.getCurrentFrameIndex();
    }

    getCurrentFrameIndex() {
        if (!this.currentAnimation) return 0;
        const anim = this.animations[this.currentAnimation];
        return anim ? anim.startFrame + this.currentFrame : 0;
    }

    isFinished() {
        return !this.isPlaying;
    }
}

// ========================================
// COMBAT ENTITY CLASS
// ========================================
class CombatEntity {
    constructor(x, y, spriteSheet, animController, config = {}) {
        this.baseX = x;
        this.baseY = y;
        this.x = x;
        this.y = y;
        this.velocityX = 0;
        this.velocityY = 0;
        this.spriteSheet = spriteSheet;
        this.animController = animController;
        this.scale = config.scale || 1;
        this.flipH = config.flipH || false;
        this.characterType = config.characterType || 'warrior'; // warrior, mage, rogue, paladin, enemy
        
        // Visual effects
        this.hitFlashAlpha = 0;
        this.hitFlashColor = '#FFFFFF';
        this.shadowWidth = config.shadowWidth || 80;
        
        // Idle breathing
        this.breatheTimer = 0;
        this.breatheAmplitude = 3;
        this.breathePeriod = 1.8;
        
        // Combat state
        this.isAttacking = false;
        this.isHit = false;
        this.isDead = false;
    }

    playAnimation(name, onComplete) {
        return this.animController.play(name, onComplete);
    }

    attack(target, onPeak) {
        if (this.isAttacking) return;
        
        this.isAttacking = true;
        const duration = 600; // milliseconds
        const startX = this.x;
        const targetX = target.x + (this.flipH ? 50 : -50);
        const startTime = Date.now();

        const attackLoop = () => {
            const elapsed = Date.now() - startTime;
            const progress = Math.min(elapsed / duration, 1);

            if (progress < 0.7) {
                // Lunge forward
                const easeProgress = this.easeOutCubic(progress / 0.7);
                this.x = startX + (targetX - startX) * easeProgress;
                
                if (progress >= 0.6 && onPeak) {
                    onPeak();
                    onPeak = null; // Call only once
                }
            } else {
                // Return
                const returnProgress = (progress - 0.7) / 0.3;
                const easeReturn = this.easeInCubic(returnProgress);
                this.x = targetX + (startX - targetX) * easeReturn;
            }

            if (progress < 1) {
                requestAnimationFrame(attackLoop);
            } else {
                this.x = startX;
                this.isAttacking = false;
            }
        };

        attackLoop();
    }

    knockback(force, duration = 400) {
        const startX = this.x;
        const startY = this.y;
        const direction = this.flipH ? 1 : -1;
        const distance = force * 30;
        const jumpHeight = force * 20;
        const startTime = Date.now();

        const knockbackLoop = () => {
            const elapsed = Date.now() - startTime;
            const progress = Math.min(elapsed / duration, 1);

            // Horizontal knockback with deceleration
            this.x = startX + distance * direction * (1 - this.easeOutQuad(progress));
            
            // Vertical arc (jump and land)
            const verticalProgress = progress * 2;
            if (verticalProgress < 1) {
                this.y = startY - jumpHeight * Math.sin(verticalProgress * Math.PI);
            } else {
                this.y = startY;
            }

            if (progress < 1) {
                requestAnimationFrame(knockbackLoop);
            } else {
                this.x = startX;
                this.y = startY;
            }
        };

        knockbackLoop();
    }

    hitFlash(color = '#FFFFFF', duration = 120) {
        this.hitFlashColor = color;
        this.hitFlashAlpha = 1.0;
        
        const startTime = Date.now();
        const flashLoop = () => {
            const elapsed = Date.now() - startTime;
            this.hitFlashAlpha = Math.max(0, 1 - (elapsed / duration));
            
            if (this.hitFlashAlpha > 0) {
                requestAnimationFrame(flashLoop);
            }
        };
        
        flashLoop();
    }

    update(deltaTime) {
        // Update animation
        const currentFrame = this.animController.update(deltaTime);
        
        // Idle breathing when not in special state
        if (!this.isAttacking && !this.isHit && !this.isDead) {
            this.breatheTimer += deltaTime / 1000;
            const breatheOffset = Math.sin(this.breatheTimer * Math.PI / this.breathePeriod) * this.breatheAmplitude;
            this.y = this.baseY + breatheOffset;
        }
        
        return currentFrame;
    }

    draw(ctx) {
        // Draw drop shadow
        this.drawDropShadow(ctx);
        
        // Draw sprite
        const frameIndex = this.animController.getCurrentFrameIndex();
        this.spriteSheet.drawFrame(ctx, frameIndex, this.x, this.y, this.scale, this.flipH);
        
        // Draw hit flash
        if (this.hitFlashAlpha > 0) {
            ctx.save();
            ctx.globalAlpha = this.hitFlashAlpha;
            ctx.globalCompositeOperation = 'lighter';
            ctx.fillStyle = this.hitFlashColor;
            const flashSize = this.spriteSheet.frameWidth * this.scale;
            ctx.fillRect(this.x - flashSize / 2, this.y - flashSize / 2, flashSize, flashSize);
            ctx.restore();
        }
        
        // Draw ground shadow
        this.drawGroundShadow(ctx);
    }

    drawGroundShadow(ctx) {
        const groundY = this.baseY + 40;
        const shadowHeight = 10;
        const distanceFromGround = Math.abs(this.y - this.baseY);
        const shadowScale = 1 - (distanceFromGround / 100);
        const shadowWidth = this.shadowWidth * shadowScale;
        
        ctx.save();
        ctx.globalAlpha = 0.3 * shadowScale;
        ctx.fillStyle = '#000000';
        ctx.beginPath();
        ctx.ellipse(this.x, groundY, shadowWidth / 2, shadowHeight / 2, 0, 0, Math.PI * 2);
        ctx.fill();
        ctx.restore();
    }

    drawDropShadow(ctx) {
        ctx.save();
        ctx.globalAlpha = 0.5;
        ctx.filter = 'blur(8px)';
        ctx.fillStyle = '#000000';
        const frameIndex = this.animController.getCurrentFrameIndex();
        const offsetX = 5;
        const offsetY = 5;
        this.spriteSheet.drawFrame(ctx, frameIndex, this.x + offsetX, this.y + offsetY, this.scale, this.flipH);
        ctx.restore();
    }

    // Easing functions
    easeOutCubic(t) {
        return 1 - Math.pow(1 - t, 3);
    }

    easeInCubic(t) {
        return t * t * t;
    }

    easeOutQuad(t) {
        return t * (2 - t);
    }
}

// Export for use in other modules
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { SpriteSheet, AnimationController, CombatEntity };
}
