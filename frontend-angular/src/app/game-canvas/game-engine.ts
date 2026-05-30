import { GameState, Button } from './types';
import { createPlayer, updatePlayer, drawPlayer } from './player';
import { createObstacles, createProjectiles, updateProjectiles, drawProjectiles, checkProjectileCollision } from './obstacles';
import { createButton, drawButton, checkButtonReached } from './button';

export class GameEngine {
  private state: GameState;
  private button: Button;
  private ctx: CanvasRenderingContext2D;
  private canvasWidth: number;
  private canvasHeight: number;
  private groundY: number;

  constructor(ctx: CanvasRenderingContext2D, canvasWidth: number, canvasHeight: number) {
    this.ctx = ctx;
    this.canvasWidth = canvasWidth;
    this.canvasHeight = canvasHeight;
    this.groundY = canvasHeight - 100;  // Suelo a 100px del borde inferior
    this.state = this.createInitialState();
    this.button = this.createButton();
  }

  private createButton() {
    return {
      x: this.canvasWidth - 100,
      y: this.groundY - 40,
      width: 60,
      height: 40
    };
  }

  private createInitialState(): GameState {
    return {
      player: createPlayer(),
      obstacles: createObstacles(this.canvasWidth, this.groundY),
      projectiles: createProjectiles(this.canvasWidth, this.groundY),
      keys: {},
      gameOver: false,
      won: false
    };
  }

  setKey(key: string, pressed: boolean) {
    this.state.keys[key] = pressed;
  }

  update() {
    if (this.state.gameOver || this.state.won) return;

    updatePlayer(this.state.player, this.state.keys, this.groundY, this.canvasWidth);
    updateProjectiles(this.state.projectiles);

    for (const p of this.state.projectiles) {
      if (checkProjectileCollision(this.state.player, p)) {
        this.state.gameOver = true;
        return;
      }
    }

    for (const obstacle of this.state.obstacles) {
      if (this.checkCollision(this.state.player, obstacle)) {
        this.state.gameOver = true;
        return;
      }
    }

    if (checkButtonReached(this.state.player, this.button)) {
      this.state.won = true;
    }
  }

  private checkCollision(
    player: { x: number; y: number; radius: number },
    obstacle: { x: number; y: number; width: number; height: number }
  ): boolean {
    const closestX = Math.max(obstacle.x, Math.min(player.x, obstacle.x + obstacle.width));
    const closestY = Math.max(obstacle.y, Math.min(player.y, obstacle.y + obstacle.height));
    const dx = player.x - closestX;
    const dy = player.y - closestY;
    return Math.sqrt(dx * dx + dy * dy) < player.radius;
  }

  draw() {
    this.ctx.clearRect(0, 0, this.canvasWidth, this.canvasHeight);

    // Suelo
    this.ctx.fillStyle = '#333';
    this.ctx.fillRect(0, this.groundY, this.canvasWidth, 5);

    // Proyectiles
    drawProjectiles(this.ctx, this.state.projectiles);

    // Botón
    drawButton(this.ctx, this.button);

    // Personaje
    drawPlayer(this.ctx, this.state.player);

    // Game over
    if (this.state.gameOver) {
      this.ctx.fillStyle = '#FF4444';
      this.ctx.font = 'bold 32px monospace';
      this.ctx.textAlign = 'center';
      this.ctx.fillText('¡GAME OVER!', this.canvasWidth / 2, this.canvasHeight / 2 - 20);
      this.ctx.font = '16px monospace';
      this.ctx.fillText('Presiona R para reiniciar', this.canvasWidth / 2, this.canvasHeight / 2 + 20);
    }

    // Victoria
    if (this.state.won) {
      this.ctx.fillStyle = '#00FF88';
      this.ctx.font = 'bold 32px monospace';
      this.ctx.textAlign = 'center';
      this.ctx.fillText('¡ANDRÉS OLIVAR!', this.canvasWidth / 2, this.canvasHeight / 2 - 20);
      this.ctx.font = '16px monospace';
      this.ctx.fillText('Bienvenido a mi portafolio', this.canvasWidth / 2, this.canvasHeight / 2 + 20);
    }
  }

  reset() {
    this.state = this.createInitialState();
    this.button = this.createButton();
  }

  hasWon(): boolean {
    return this.state.won;
  }
}
