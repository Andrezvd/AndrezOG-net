import { Component, OnInit, OnDestroy, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';
import { Router } from '@angular/router';
import { AsyncPipe } from '@angular/common';
import { Observable, of, Subscription } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { GameCanvas } from '../../../game-canvas/game-canvas';
import { ProfileService } from '../../profile/api/profile.service';
import { MyProfileDto } from '../../profile/types/profile.types';
import { SkillService } from '../api/skill.service';
import { SkillCardDto } from '../types/skill.types';
import { AuthStateService } from '../../../services/auth-state.service';
import { AuthService } from '../../auth/api/auth.service';
import { LandingNavbarComponent } from './landing-navbar.component';
import { LandingFooterComponent } from './landing-footer.component';

@Component({
    selector: 'app-landing',
    imports: [GameCanvas, AsyncPipe, LandingNavbarComponent, LandingFooterComponent],
    templateUrl: '../ui/landing.component.html',
    styleUrl: '../../../app.css'
})
export class LandingComponent implements OnInit, OnDestroy {
    showPortfolio = false;
    showOverlay = true;
    hoverSkip = false;
    menuOpen = false;
    userMenuOpen = false;
    profileLoaded = false;
    profile$: Observable<MyProfileDto | null> = of(null);
    skills: SkillCardDto[] = [];
    private skillsSub?: Subscription;

    constructor(
        @Inject(PLATFORM_ID) private platformId: object,
        private profileService: ProfileService,
        private skillService: SkillService,
        public authState: AuthStateService,
        private authService: AuthService,
        private router: Router
    ) { }

    ngOnInit(): void {
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
        setTimeout(() => this.profileLoaded = true, 100);
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

    toggleUserMenu() {
        this.userMenuOpen = !this.userMenuOpen;
    }

    getUserInitials(): string {
        const name = this.authState.state.name;
        if (!name) return '?';
        const parts = name.split(' ');
        if (parts.length >= 2) {
            return (parts[0].charAt(0) + parts[1].charAt(0)).toUpperCase();
        }
        return name.charAt(0).toUpperCase();
    }

    logout(): void {
        this.authService.logout().subscribe({
            next: () => {
                this.userMenuOpen = false;
                this.router.navigate(['/']);
            },
            error: () => {
                this.userMenuOpen = false;
                this.router.navigate(['/']);
            }
        });
    }
}