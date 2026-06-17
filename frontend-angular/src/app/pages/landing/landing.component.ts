import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { GameCanvas } from '../../game-canvas/game-canvas';
import { ProfileService } from '../profile/api/profile.service';
import { MyProfileDto } from '../profile/types/profile.types';

@Component({
    selector: 'app-landing',
    imports: [GameCanvas, RouterLink, AsyncPipe],
    templateUrl: './landing.component.html',
    styleUrl: '../../app.css'
})
export class LandingComponent {
    showPortfolio = false;
    menuOpen = false;
    profile$: Observable<MyProfileDto | null> = of(null);

    constructor(private profileService: ProfileService) { }

    onGameWon() {
        this.showPortfolio = true;
        this.profile$ = this.profileService.getPublicProfile().pipe(
            catchError(() => of(null))
        );
    }

    toggleMenu() {
        this.menuOpen = !this.menuOpen;
    }
}