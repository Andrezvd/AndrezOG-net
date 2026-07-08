import { Component, OnInit, OnDestroy, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
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
        @Inject(PLATFORM_ID) private platformId: object,
        private profileService: ProfileService,
        private skillService: SkillService
    ) { }

    ngOnInit(): void {
        // Solo acceder a sessionStorage en el navegador (no durante SSR/prerendering)
        if (isPlatformBrowser(this.platformId)) {
            if (sessionStorage.getItem('andrezog_game_completed') === 'true') {
                this.showPortfolio = true;
                this.showOverlay = false;
                this.loadPortfolioData();
            }
        }
    }

    onGameWon() {
        if (isPlatformBrowser(this.platformId)) {
            sessionStorage.setItem('andrezog_game_completed', 'true');
        }
        this.showPortfolio = true;
        this.showOverlay = false;
        this.loadPortfolioData();
    }

    skipGame() {
        if (isPlatformBrowser(this.platformId)) {
            sessionStorage.setItem('andrezog_game_completed', 'true');
        }
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