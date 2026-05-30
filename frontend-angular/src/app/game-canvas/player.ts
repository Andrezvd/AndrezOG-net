import { Player } from './types';

const GRAVITY = 0.3;      // Qué tan rápido cae el personaje (px por frame²)
const JUMP_FORCE = -8;    // Fuerza del salto (negativo = hacia arriba)
const PLAYER_SPEED = 3;    // Qué tan rápido se mueve horizontalmente
const PLAYER_RADIUS = 10;  // Tamaño del personaje (círculo)


export function createPlayer(): Player {
  return {
    x: 50,              // Empieza cerca de la izquierda
    y: 0,               // La gravedad lo pondrá en el suelo
    radius: PLAYER_RADIUS,
    speed: PLAYER_SPEED,
    vy: 0,              // Empieza sin velocidad vertical
    onGround: false,  // Empieza en el aire (caerá por gravedad)
    isBlocking: false   // No está bloqueando al inicio
  };
}


export function updatePlayer(
  player: Player,
  keys: { [key: string]: boolean },
  groundY: number,
  canvasWidth: number
) {


  if (keys['g'] || keys['G']) {
    player.isBlocking = true;
    player.vy = 0; // Detiene cualquier movimiento vertical
    }else {
      player.isBlocking = false;
  }

  if (!player.isBlocking) {
    player.vy  += GRAVITY;  // Aplica gravedad solo si no está bloqueando
    player.y += player.vy;  // Actualiza la posición vertical
  }

  // colision con el suelo
  if (player.y + player.radius >= groundY) {
    player.y = groundY - player.radius;  // Lo ponemos justo en el suelo
    player.vy = 0;                        // Detenemos la caída
    player.onGround = true;               // Marcamos que está en el suelo
  } else {
    player.onGround = false;              // Está en el aire
  }

  // mov horizontal
  if (keys['a'] || keys['ArrowLeft']) player.x -= player.speed;
  if (keys['d'] || keys['ArrowRight']) player.x += player.speed;

  if ((keys['w'] || keys['ArrowUp']) && player.onGround) {
    player.vy = JUMP_FORCE;  // Empuja al personaje hacia arriba
  }


  // Limitar dentro del canvas
  player.x = Math.max(player.radius, Math.min(canvasWidth - player.radius, player.x));
}



export function drawPlayer(ctx: CanvasRenderingContext2D, player: Player) {
  const x = player.x;
  const y = player.y;
  const r = player.radius;

  // --- ESCUDO (si está bloqueando) ---
  if (player.isBlocking) {
    ctx.fillStyle = 'rgba(0, 255, 255, 0.2)';
    ctx.beginPath();
    ctx.arc(x, y, r * 2, 0, Math.PI * 2);
    ctx.fill();
    ctx.strokeStyle = 'rgba(0, 255, 255, 0.5)';
    ctx.lineWidth = 2;
    ctx.stroke();
  }

  // --- CUERPO ---
  ctx.fillStyle = '#00F5FF';
  
  // Cabeza (círculo arriba)
  ctx.beginPath();
  ctx.arc(x, y - r * 0.5, r * 0.4, 0, Math.PI * 2);
  ctx.fill();

  // Cuerpo (rectángulo)
  ctx.fillRect(x - r * 0.3, y - r * 0.2, r * 0.6, r * 0.6);

  // --- PIERNAS ---
  ctx.strokeStyle = '#00F5FF';
  ctx.lineWidth = 2;
  ctx.beginPath();
  ctx.moveTo(x - r * 0.2, y + r * 0.4);
  ctx.lineTo(x - r * 0.3, y + r * 0.8);
  ctx.moveTo(x + r * 0.2, y + r * 0.4);
  ctx.lineTo(x + r * 0.3, y + r * 0.8);
  ctx.stroke();

  // --- BRAZOS ---
  ctx.beginPath();
  ctx.moveTo(x - r * 0.3, y);
  ctx.lineTo(x - r * 0.6, y + r * 0.3);
  ctx.moveTo(x + r * 0.3, y);
  ctx.lineTo(x + r * 0.6, y + r * 0.3);
  ctx.stroke();
}
