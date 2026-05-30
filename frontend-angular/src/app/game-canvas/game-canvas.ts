import { Component, ElementRef, ViewChild, AfterViewInit, HostListener, Output, EventEmitter } from '@angular/core';
import { GameEngine } from './game-engine';

@Component({
  selector: 'app-game-canvas',
  standalone: true,
  template: `
    <canvas
      #gameCanvas
      class="game-canvas"
    ></canvas>
  `,
  styles: [`
    .game-canvas {
      display: block;
      background: #0a0a0a;
      border: 1px solid #333;
      margin: 0 auto;
    }
  `]
})
export class GameCanvas implements AfterViewInit {
  @ViewChild('gameCanvas') canvasRef!: ElementRef<HTMLCanvasElement>;
  @Output() gameWon = new EventEmitter<void>();
  private engine!: GameEngine;
  private hasEmittedWin = false;

  ngAfterViewInit() {
    const canvas = this.canvasRef.nativeElement;

    // Ajustar tamaño: 80% del ancho, 50% del alto de la ventana
    canvas.width = Math.floor(window.innerWidth * 0.8);
    canvas.height = Math.floor(window.innerHeight * 0.5);

    const ctx = canvas.getContext('2d')!;
    this.engine = new GameEngine(ctx, canvas.width, canvas.height);
    this.gameLoop();
  }

  @HostListener('window:keydown', ['$event'])
  onKeyDown(event: KeyboardEvent) {
    this.engine.setKey(event.key, true);
    if (event.key === 'r' || event.key === 'R') {
      this.engine.reset();
    }
    if (['ArrowUp', 'ArrowDown', 'ArrowLeft', 'ArrowRight', ' '].includes(event.key)) {
      event.preventDefault();
    }
  }

  @HostListener('window:keyup', ['$event'])
  onKeyUp(event: KeyboardEvent) {
    this.engine.setKey(event.key, false);
  }

  private gameLoop() {
    this.engine.update();
    this.engine.draw();

    // Si ganó y aún no hemos emitido el evento, lo emitimos
    if (this.engine.hasWon() && !this.hasEmittedWin) {
      this.hasEmittedWin = true;
      this.gameWon.emit();
    }

    requestAnimationFrame(() => this.gameLoop());
  }
}
