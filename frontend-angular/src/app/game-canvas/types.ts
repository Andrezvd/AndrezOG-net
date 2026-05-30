export interface Player {
    x: number;
    y: number;
    speed: number;
    radius: number;
    onGround: boolean;
    vy: number;
    isBlocking: boolean;
};

export interface Obstacle {
    x: number;
    y: number;
    width: number;
    height: number;
    type: 'jump' | 'duck' | 'block';
    label: string;
}

export interface Button {
  x: number;
  y: number;
  width: number;
  height: number;
}

export interface Projectile {
  speed: number;
  x: number;
  y: number;
  width: number;
  height: number;
  active: boolean;
  label: string;
}

export interface GameState {
    player: Player;
    obstacles: Obstacle[];
    keys: { [key: string]: boolean };
    gameOver: boolean;
    won: boolean;
    projectiles: Projectile[];
}

