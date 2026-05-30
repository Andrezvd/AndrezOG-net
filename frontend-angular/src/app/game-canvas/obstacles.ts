import { Obstacle, Projectile } from "./types";

// Lista de eventos del juego (se ejecutan en orden)
const GAME_EVENTS = [
  { type: 'projectile', width: 30, height: 10, speed: -5, label: '🏹' },
  // Aquí irán más eventos después
];

export function createObstacles(canvasWidth: number, groundY: number): Obstacle[] {
  return [
    {
      x: canvasWidth * 0.4,  // 40% del ancho del canvas
      y: groundY - 30,
      width: 40,
      height: 30,
      type: 'jump',
      label: '⬆'
    },
  ];
}

export function createProjectiles(canvasWidth: number, groundY: number): Projectile[] {
  return GAME_EVENTS
    .filter(e => e.type === 'projectile')
    .map(e => ({
      x: canvasWidth,           // Empieza en el borde derecho
      y: groundY - 20,          // Justo encima del suelo
      width: e.width,
      height: e.height,
      speed: e.speed,
      active: true,
      label: e.label
    }));
}

export function updateProjectiles(projectiles: Projectile[]) {
  projectiles.forEach(p => {
    if (p.active) {
      p.x += p.speed;  // Mover de derecha a izquierda
      if (p.x + p.width < 0) p.active = false;  // Salió de pantalla
    }
  });
}

export function drawProjectiles(ctx: CanvasRenderingContext2D, projectiles: Projectile[]) {
  projectiles.forEach(p => {
    if (!p.active) return;

    // Cuerpo de la flecha
    ctx.fillStyle = '#FF4444';
    ctx.fillRect(p.x, p.y, p.width, p.height);

    // Punta de la flecha (triángulo)
    ctx.beginPath();
    ctx.moveTo(p.x, p.y);
    ctx.lineTo(p.x, p.y + p.height);
    ctx.lineTo(p.x - 10, p.y + p.height / 2);
    ctx.closePath();
    ctx.fill();

    // Etiqueta
    ctx.fillStyle = '#FFF';
    ctx.font = '12px monospace';
    ctx.textAlign = 'center';
    ctx.fillText(p.label, p.x + p.width / 2, p.y - 5);
  });
}

export function checkProjectileCollision(
  player: { x: number; y: number; radius: number },
  projectile: Projectile
): boolean {
  if (!projectile.active) return false;

  const closestX = Math.max(projectile.x, Math.min(player.x, projectile.x + projectile.width));
  const closestY = Math.max(projectile.y, Math.min(player.y, projectile.y + projectile.height));
  const dx = player.x - closestX;
  const dy = player.y - closestY;
  return Math.sqrt(dx * dx + dy * dy) < player.radius;
}

export function drawObstacles(ctx: CanvasRenderingContext2D, obstacles: Obstacle[]) {
  obstacles.forEach(obstacle => {
    switch (obstacle.type) {
      case 'jump':  ctx.fillStyle = '#FF4444'; break;
      case 'duck':  ctx.fillStyle = '#FFAA00'; break;
      case 'block': ctx.fillStyle = '#AA44FF'; break;
    }

    ctx.fillRect(obstacle.x, obstacle.y, obstacle.width, obstacle.height);

    ctx.fillStyle = '#FFF';
    ctx.font = '16px monospace';
    ctx.textAlign = 'center';
    ctx.fillText(obstacle.label, obstacle.x + obstacle.width / 2, obstacle.y - 5);
  });
}
