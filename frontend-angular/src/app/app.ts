import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { GameCanvas } from  './game-canvas/game-canvas';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule, GameCanvas],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  showPortfolio = false;

  onGameWon() {
    this.showPortfolio = true;
  }
}
