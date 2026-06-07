/**
 * MAIN ARENA COMBAT ENGINE
 * Orchestrates turn-based combat with Canvas rendering
 */

class ArenaCombat {
    constructor(canvasId, config) {
        this.canvas = document.getElementById(canvasId);
        this.ctx = this.canvas.getContext('2d');
        
        // Set canvas size
        this.resizeCanvas();
        window.addEventListener('resize', () => this.resizeCanvas());
        
        // Combat configuration
        this.config = config; // { playerId, enemyId, levelId }
        
        // Game state
        this.state = 'loading'; // loading, intro, player-turn, enemy-turn, victory, defeat
        this.player = null;
        this.enemy = null;
        
        // Rendering components
        this.background = null;
        this.playerEntity = null;
        this.enemyEntity = null;
        this.particleSystem = null;
        this.cameraEffects = null;
        
        // Combat data
        this.playerData = null;
        this.enemyData = null;
        this.turnNumber = 1;
        this.battleLog = [];
        
        // Animation timing
        this.lastFrameTime = Date.now();
        this.animationFrame = null;
        
        // UI callbacks
        this.onBattleEnd = null;
        this.onTurnComplete = null;
        this.onLogUpdate = null;
    }

    async initialize() {
        try {
            this.state = 'loading';
            
            // Load combat data from server
            await this.loadCombatData();
            
            // Initialize rendering components
            this.particleSystem = new ParticleSystem(this.canvas);
            this.cameraEffects = new CameraEffects(this.canvas);
            this.background = new ProceduralBackground(this.canvas, this.config.levelId);
            
            // Load sprites (with fallback to colored rectangles)
            await this.loadSprites();
            
            // Setup combat entities
            this.setupEntities();
            
            // Start render loop
            this.state = 'intro';
            this.startIntro();
            
        } catch (error) {
            console.error('Failed to initialize arena:', error);
            this.state = 'error';
        }
    }

    async loadCombatData() {
        // Load player stats
        const playerResponse = await fetch(`/ArenaCanvas/api/player-stats`);
        if (!playerResponse.ok) throw new Error('Failed to load player data');
        this.playerData = await playerResponse.json();
        
        // Load enemy stats
        const enemyResponse = await fetch(`/ArenaCanvas/api/enemy/${this.config.enemyId}`);
        if (!enemyResponse.ok) throw new Error('Failed to load enemy data');
        this.enemyData = await enemyResponse.json();
    }

    async loadSprites() {
        // Player sprite based on class
        const playerSpriteMap = {
            'Warrior': '/images/sprites/warrior.png',
            'Mage': '/images/sprites/mage.png',
            'Rogue': '/images/sprites/rogue.png',
            'Paladin': '/images/sprites/paladin.png'
        };
        
        const playerSpritePath = playerSpriteMap[this.playerData.className] || '/images/sprites/default.png';
        const playerSpriteSheet = new SpriteSheet(playerSpritePath, 64, 64);
        await playerSpriteSheet.load();
        
        const playerAnimController = new AnimationController();
        playerAnimController.addAnimation('idle', 0, 4, 8, true, 0);
        playerAnimController.addAnimation('attack', 4, 4, 12, false, 5);
        playerAnimController.addAnimation('hurt', 8, 2, 10, false, 10);
        playerAnimController.addAnimation('victory', 12, 4, 6, true, 0);
        playerAnimController.addAnimation('defeat', 16, 4, 6, false, 0);
        playerAnimController.play('idle');
        
        this.playerEntity = new CombatEntity(
            this.canvas.width * 0.3,
            this.canvas.height * 0.5,
            playerSpriteSheet,
            playerAnimController,
            { scale: 2.5, flipH: false, characterType: 'player' }
        );
        
        // Enemy sprite
        const enemySpritePath = `/images/sprites/enemy_${this.config.enemyId}.png`;
        const enemySpriteSheet = new SpriteSheet(enemySpritePath, 64, 64);
        await enemySpriteSheet.load();
        
        const enemyAnimController = new AnimationController();
        enemyAnimController.addAnimation('idle', 0, 4, 6, true, 0);
        enemyAnimController.addAnimation('attack', 4, 4, 10, false, 5);
        enemyAnimController.addAnimation('hurt', 8, 2, 10, false, 10);
        enemyAnimController.addAnimation('defeat', 12, 4, 6, false, 0);
        enemyAnimController.play('idle');
        
        this.enemyEntity = new CombatEntity(
            this.canvas.width * 0.7,
            this.canvas.height * 0.5,
            enemySpriteSheet,
            enemyAnimController,
            { scale: 2.5, flipH: true, characterType: 'enemy' }
        );
    }

    setupEntities() {
        this.player = {
            entity: this.playerEntity,
            stats: {
                hp: this.playerData.health,
                maxHp: this.playerData.health,
                attack: this.playerData.attack,
                defense: this.playerData.defense,
                className: this.playerData.className
            },
            buffs: []
        };
        
        this.enemy = {
            entity: this.enemyEntity,
            stats: {
                hp: this.enemyData.health,
                maxHp: this.enemyData.health,
                attack: this.enemyData.attack,
                defense: this.enemyData.defense,
                name: this.enemyData.name
            },
            buffs: []
        };
    }

    resizeCanvas() {
        this.canvas.width = window.innerWidth;
        this.canvas.height = window.innerHeight;
    }

    startIntro() {
        // Play intro animation
        this.cameraEffects.zoom(1.3, 1500);
        
        setTimeout(() => {
            this.state = 'player-turn';
            this.startRenderLoop();
        }, 1500);
    }

    startRenderLoop() {
        const render = () => {
            const now = Date.now();
            const deltaTime = Math.min(now - this.lastFrameTime, 100); // Cap at 100ms
            this.lastFrameTime = now;
            
            // Apply time scale from camera effects
            const effectiveDelta = deltaTime * this.cameraEffects.getTimeScale();
            
            // Update
            this.update(effectiveDelta);
            
            // Render
            this.render();
            
            // Continue loop
            if (this.state !== 'error') {
                this.animationFrame = requestAnimationFrame(render);
            }
        };
        
        render();
    }

    update(deltaTime) {
        // Update background
        if (this.background) {
            this.background.update(deltaTime);
        }
        
        // Update entities
        if (this.playerEntity) {
            this.playerEntity.update(deltaTime);
        }
        if (this.enemyEntity) {
            this.enemyEntity.update(deltaTime);
        }
        
        // Update particle system
        if (this.particleSystem) {
            this.particleSystem.update(deltaTime);
        }
        
        // Update camera effects
        if (this.cameraEffects) {
            this.cameraEffects.update(deltaTime);
        }
    }

    render() {
        // Clear canvas
        this.ctx.fillStyle = '#000000';
        this.ctx.fillRect(0, 0, this.canvas.width, this.canvas.height);
        
        // Apply camera transform
        this.cameraEffects.applyTransform();
        
        // Draw background
        if (this.background) {
            this.background.draw();
        }
        
        // Draw entities
        if (this.enemyEntity && this.enemy.stats.hp > 0) {
            this.enemyEntity.draw(this.ctx);
        }
        if (this.playerEntity && this.player.stats.hp > 0) {
            this.playerEntity.draw(this.ctx);
        }
        
        // Draw particles
        if (this.particleSystem) {
            this.particleSystem.draw();
        }
        
        // Remove camera transform
        this.cameraEffects.removeTransform();
        
        // Draw UI (health bars, etc.) - these don't get camera effects
        this.drawUI();
        
        // Apply post-processing effects
        this.cameraEffects.renderPostEffects();
    }

    drawUI() {
        const barWidth = 250;
        const barHeight = 25;
        const padding = 20;
        
        // Player health bar
        this.drawHealthBar(
            padding,
            padding,
            barWidth,
            barHeight,
            this.player.stats.hp,
            this.player.stats.maxHp,
            this.playerData.name,
            '#00FF00'
        );
        
        // Enemy health bar
        this.drawHealthBar(
            this.canvas.width - barWidth - padding,
            padding,
            barWidth,
            barHeight,
            this.enemy.stats.hp,
            this.enemy.stats.maxHp,
            this.enemy.stats.name,
            '#FF0000'
        );
        
        // Turn indicator
        this.drawTurnIndicator();
    }

    drawHealthBar(x, y, width, height, currentHp, maxHp, name, color) {
        const hpPercent = Math.max(0, currentHp / maxHp);
        
        // Background
        this.ctx.fillStyle = '#333333';
        this.ctx.fillRect(x, y, width, height);
        
        // Health fill
        this.ctx.fillStyle = color;
        this.ctx.fillRect(x, y, width * hpPercent, height);
        
        // Border
        this.ctx.strokeStyle = '#FFFFFF';
        this.ctx.lineWidth = 2;
        this.ctx.strokeRect(x, y, width, height);
        
        // Text
        this.ctx.fillStyle = '#FFFFFF';
        this.ctx.font = 'bold 14px Arial';
        this.ctx.textAlign = 'center';
        this.ctx.fillText(`${name}`, x + width / 2, y - 5);
        this.ctx.fillText(`${Math.max(0, currentHp)} / ${maxHp}`, x + width / 2, y + height / 2 + 5);
    }

    drawTurnIndicator() {
        if (this.state !== 'player-turn' && this.state !== 'enemy-turn') return;
        
        const text = this.state === 'player-turn' ? 'YOUR TURN' : 'ENEMY TURN';
        const y = this.canvas.height - 40;
        
        this.ctx.save();
        this.ctx.fillStyle = 'rgba(0, 0, 0, 0.7)';
        this.ctx.fillRect(0, y - 20, this.canvas.width, 40);
        
        this.ctx.fillStyle = this.state === 'player-turn' ? '#00FF00' : '#FF0000';
        this.ctx.font = 'bold 24px Arial';
        this.ctx.textAlign = 'center';
        this.ctx.fillText(text, this.canvas.width / 2, y + 10);
        this.ctx.restore();
    }

    // ========================================
    // COMBAT ACTIONS
    // ========================================

    async playerAttack() {
        if (this.state !== 'player-turn') return;
        
        this.state = 'animating';
        
        // Calculate damage
        const baseDamage = this.player.stats.attack;
        const mitigatedDamage = Math.max(1, baseDamage - this.enemy.stats.defense);
        const isCritical = Math.random() < 0.15;
        const finalDamage = isCritical ? Math.floor(mitigatedDamage * 1.5) : mitigatedDamage;
        
        // Play attack animation
        this.playerEntity.playAnimation('attack');
        this.playerEntity.attack(this.enemyEntity, () => {
            // On attack peak
            this.enemyEntity.playAnimation('hurt');
            this.enemyEntity.hitFlash('#FFFFFF', 150);
            this.enemyEntity.knockback(1.5);
            
            // Camera effects
            if (isCritical) {
                this.cameraEffects.criticalHitEffect();
            } else {
                this.cameraEffects.impactEffect(0.7);
            }
            
            // Particles
            this.particleSystem.emitHitSparks(this.enemyEntity.x, this.enemyEntity.y, 12, '#FFD700');
            this.particleSystem.emitDamageNumber(this.enemyEntity.x, this.enemyEntity.y - 40, finalDamage, isCritical);
            this.particleSystem.emitImpactRing(this.enemyEntity.x, this.enemyEntity.y, 60, '#FFFFFF');
            
            // Apply damage
            this.enemy.stats.hp -= finalDamage;
            
            // Log
            this.addLog(`You dealt ${finalDamage} damage!` + (isCritical ? ' CRITICAL!' : ''));
        });
        
        // Wait for animation
        await this.wait(800);
        
        // Check for victory
        if (this.enemy.stats.hp <= 0) {
            await this.handleVictory();
        } else {
            // Switch to enemy turn
            this.state = 'enemy-turn';
            await this.wait(800);
            await this.enemyAttack();
        }
    }

    async playerSpecialAbility() {
        if (this.state !== 'player-turn') return;
        
        this.state = 'animating';
        
        const className = this.player.stats.className;
        let logMessage = '';
        
        switch(className) {
            case 'Warrior':
                // Taunt - reduces enemy attack
                this.cameraEffects.specialAbilityEffect('warrior');
                this.particleSystem.emitSmokePuff(this.playerEntity.x, this.playerEntity.y, 8);
                this.player.buffs.push({ type: 'taunt', duration: 2 });
                logMessage = 'You taunt the enemy! Enemy attack reduced!';
                break;
                
            case 'Mage':
                // Arcane Blast
                const damage = Math.floor(this.player.stats.attack * 1.8);
                this.cameraEffects.specialAbilityEffect('mage');
                this.particleSystem.emitMagicCircle(this.playerEntity.x, this.playerEntity.y, 100, '#9370DB');
                await this.wait(400);
                this.particleSystem.emitFireBurst(this.enemyEntity.x, this.enemyEntity.y, 20);
                this.enemyEntity.hitFlash('#9370DB', 200);
                this.enemy.stats.hp -= damage;
                this.particleSystem.emitDamageNumber(this.enemyEntity.x, this.enemyEntity.y - 40, damage, true);
                logMessage = `Arcane Blast deals ${damage} magic damage!`;
                break;
                
            case 'Rogue':
                // Bleed
                this.cameraEffects.specialAbilityEffect('rogue');
                this.particleSystem.emitSlashTrail(
                    this.playerEntity.x,
                    this.playerEntity.y,
                    0,
                    150,
                    '#8B0000'
                );
                this.particleSystem.emitBloodSplatter(this.enemyEntity.x, this.enemyEntity.y, 12);
                this.enemy.buffs.push({ type: 'bleed', duration: 3, damagePerTurn: 5 });
                logMessage = 'Bleeding applied! Enemy takes damage over time!';
                break;
                
            case 'Paladin':
                // Heal
                const healAmount = Math.floor(this.player.stats.maxHp * 0.3);
                this.player.stats.hp = Math.min(this.player.stats.maxHp, this.player.stats.hp + healAmount);
                this.cameraEffects.specialAbilityEffect('paladin');
                this.particleSystem.emitHolyLight(this.playerEntity.x, this.playerEntity.y);
                this.particleSystem.emitHealNumber(this.playerEntity.x, this.playerEntity.y - 40, healAmount);
                logMessage = `Healed for ${healAmount} HP!`;
                break;
        }
        
        this.addLog(logMessage);
        await this.wait(1000);
        
        // Check for victory
        if (this.enemy.stats.hp <= 0) {
            await this.handleVictory();
        } else {
            this.state = 'enemy-turn';
            await this.wait(800);
            await this.enemyAttack();
        }
    }

    async enemyAttack() {
        if (this.state !== 'enemy-turn') return;
        
        this.state = 'animating';
        
        // Calculate damage
        const baseDamage = this.enemy.stats.attack;
        const mitigatedDamage = Math.max(1, baseDamage - this.player.stats.defense);
        const finalDamage = Math.floor(mitigatedDamage);
        
        // Play attack animation
        this.enemyEntity.playAnimation('attack');
        this.enemyEntity.attack(this.playerEntity, () => {
            // On attack peak
            this.playerEntity.playAnimation('hurt');
            this.playerEntity.hitFlash('#FF0000', 150);
            this.playerEntity.knockback(1.5);
            
            // Camera effects
            this.cameraEffects.impactEffect(0.6);
            
            // Particles
            this.particleSystem.emitHitSparks(this.playerEntity.x, this.playerEntity.y, 10, '#FF6B6B');
            this.particleSystem.emitDamageNumber(this.playerEntity.x, this.playerEntity.y - 40, finalDamage, false);
            
            // Apply damage
            this.player.stats.hp -= finalDamage;
            
            // Log
            this.addLog(`${this.enemy.stats.name} dealt ${finalDamage} damage!`);
        });
        
        await this.wait(800);
        
        // Process buffs/debuffs
        await this.processBuffs();
        
        // Check for defeat
        if (this.player.stats.hp <= 0) {
            await this.handleDefeat();
        } else {
            // Increment turn
            this.turnNumber++;
            this.state = 'player-turn';
        }
    }

    async processBuffs() {
        // Process bleed on enemy
        for (let i = this.enemy.buffs.length - 1; i >= 0; i--) {
            const buff = this.enemy.buffs[i];
            if (buff.type === 'bleed') {
                this.enemy.stats.hp -= buff.damagePerTurn;
                this.particleSystem.emitDamageNumber(this.enemyEntity.x, this.enemyEntity.y - 40, buff.damagePerTurn, false);
                this.addLog(`Bleed deals ${buff.damagePerTurn} damage!`);
                
                buff.duration--;
                if (buff.duration <= 0) {
                    this.enemy.buffs.splice(i, 1);
                    this.addLog('Bleed effect ended.');
                }
            }
        }
    }

    async handleVictory() {
        this.state = 'victory';
        
        // Death animation
        this.enemyEntity.playAnimation('defeat');
        this.cameraEffects.deathEffect();
        this.particleSystem.emitSmokePuff(this.enemyEntity.x, this.enemyEntity.y, 15);
        
        await this.wait(1500);
        
        // Victory effect
        this.playerEntity.playAnimation('victory');
        this.cameraEffects.victoryEffect();
        
        this.addLog('VICTORY!');
        
        // Send result to server
        await this.sendBattleResult(true);
        
        if (this.onBattleEnd) {
            this.onBattleEnd('victory');
        }
    }

    async handleDefeat() {
        this.state = 'defeat';
        
        // Death animation
        this.playerEntity.playAnimation('defeat');
        this.cameraEffects.deathEffect();
        this.particleSystem.emitSmokePuff(this.playerEntity.x, this.playerEntity.y, 15);
        
        await this.wait(1500);
        
        this.addLog('DEFEAT...');
        
        // Send result to server
        await this.sendBattleResult(false);
        
        if (this.onBattleEnd) {
            this.onBattleEnd('defeat');
        }
    }

    async sendBattleResult(victory) {
        try {
            const response = await fetch('/ArenaCanvas/api/battle-result', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    enemyId: this.config.enemyId,
                    victory: victory,
                    turnCount: this.turnNumber,
                    damageDealt: this.enemyData.health - this.enemy.stats.hp,
                    damageTaken: this.playerData.health - this.player.stats.hp
                })
            });
            
            if (!response.ok) {
                console.error('Failed to send battle result');
            }
        } catch (error) {
            console.error('Error sending battle result:', error);
        }
    }

    addLog(message) {
        this.battleLog.push(`[Turn ${this.turnNumber}] ${message}`);
        if (this.onLogUpdate) {
            this.onLogUpdate(message);
        }
    }

    wait(ms) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    destroy() {
        if (this.animationFrame) {
            cancelAnimationFrame(this.animationFrame);
        }
    }
}

// Export for use
if (typeof module !== 'undefined' && module.exports) {
    module.exports = { ArenaCombat };
}
