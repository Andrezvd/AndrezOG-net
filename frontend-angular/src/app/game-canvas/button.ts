import { Button } from './types';

const BUTTON_X = 1120;
const BUTTON_Y = 460;
const BUTTON_WIDTH = 60;
const BUTTON_HEIGHT = 40;

export function createButton(): Button {
  return {
    x: BUTTON_X,
    y: BUTTON_Y,
    width: BUTTON_WIDTH,
    height: BUTTON_HEIGHT
  };
}

export function drawButton(ctx: CanvasRenderingContext2D, button: Button) {
  // Cuerpo del botón (verde brillante)
  ctx.fillStyle = '#00FF88';
  ctx.fillRect(button.x, button.y, button.width, button.height);

  // Borde
  ctx.strokeStyle = '#00FFAA';
  ctx.lineWidth = 2;
  ctx.strokeRect(button.x, button.y, button.width, button.height);

  // Texto "META"
  ctx.fillStyle = '#000';
  ctx.font = 'bold 14px monospace';
  ctx.textAlign = 'center';
  ctx.textBaseline = 'middle';
  ctx.fillText('META', button.x + button.width / 2, button.y + button.height / 2);
}

export function checkButtonReached(
  player: { x: number; y: number; radius: number },
  button: Button
): boolean {
  // Colisión simple AABB (caja contra caja)
  return (
    player.x + player.radius > button.x &&
    player.x - player.radius < button.x + button.width &&
    player.y + player.radius > button.y &&
    player.y - player.radius < button.y + button.height
  );
}
