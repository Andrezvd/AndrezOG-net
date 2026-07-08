import { Component, OnInit, OnDestroy } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { Observable, of, Subscription } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { GameCanvas } from '../../game-canvas/game-canvas';
import { ProfileService } from '../profile/api/profile.service';
import { MyProfileDto } from '../profile/types/profile.types';
import { SkillService } from './api/skill.service';
import { SkillCardDto } from './types/skill.types';
import { API_URL_IMAGES } from '../../services-conf/api-config';

@Component({
    selector: 'app-landing',
    imports: [GameCanvas, RouterLink, AsyncPipe],
    templateUrl: './landing.component.html',
    styleUrl: '../../app.css'
})
export class LandingComponent implements OnInit, OnDestroy {
    showPortfolio = false;
    showOverlay = true;
    hoverSkip = false;
    menuOpen = false;
    profile$: Observable<MyProfileDto | null> = of(null);
    skills: SkillCardDto[] = [];
    apiImagesUrl = API_URL_IMAGES;
    private skillsSub?: Subscription;

    constructor(
        private profileService: ProfileService,
        private skillService: SkillService
    ) { }

    ngOnInit(): void {
        // Si el usuario ya completo u omitio el juego antes, ir directo al portfolio
        if (sessionStorage.getItem('andrezog_game_completed') === 'true') {
            this.showPortfolio = true;
            this.showOverlay = false;
            this.loadPortfolioData();
        }
    }

    onGameWon() {
        sessionStorage.setItem('andrezog_game_completed', 'true');
        this.showPortfolio = true;
        this.showOverlay = false;
        this.loadPortfolioData();
    }

    skipGame() {
        sessionStorage.setItem('andrezog_game_completed', 'true');
        this.showPortfolio = true;
        this.showOverlay = false;
        this.loadPortfolioData();
    }

    private loadPortfolioData() {
        this.profile$ = this.profileService.getPublicProfile().pipe(
            catchError(() => of(null))
        );
        this.skillsSub?.unsubscribe();
        this.skillsSub = this.skillService.getPublicSkills().pipe(
            catchError(() => of([]))
        ).subscribe(list => this.skills = list);
    }

    ngOnDestroy() {
        this.skillsSub?.unsubscribe();
    }

    toggleMenu() {
        this.menuOpen = !this.menuOpen;
    }
}