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
import { ProjectService } from '../../project/api/project.service';
import { ProjectCardDto } from '../../project/types/project.types';
import { AuthStateService } from '../../../services/auth-state.service';
import { AuthService } from '../../auth/api/auth.service';
import { LandingNavbarComponent } from './landing-navbar.component';
import { LandingFooterComponent } from './landing-footer.component';
import { ProjectCarouselComponent } from '../../project/decorator/project-carousel.component';

@Component({
    selector: 'app-landing',
    imports: [GameCanvas, AsyncPipe, LandingNavbarComponent, LandingFooterComponent, ProjectCarouselComponent],
    templateUrl: '../ui/landing.component.html',
    styleUrls: ['../../../app.css']
})
export class LandingComponent implements OnInit, OnDestroy {
    showPortfolio = false;
    showOverlay = true;
    hoverSkip = false;
    menuOpen = false;
    userMenuOpen = false;
    profile$: Observable<MyProfileDto | null> = of(null);
    myProfile$: Observable<MyProfileDto | null> = of(null);
    projects: ProjectCardDto[] = [];
    skills: SkillCardDto[] = [];
    currentProjectIndex = 0;
    private skillsSub?: Subscription;
    private projectsSub?: Subscription;
    private myProfileSub?: Subscription;

    constructor(
        @Inject(PLATFORM_ID) private platformId: object,
        private profileService: ProfileService,
        private skillService: SkillService,
        private projectService: ProjectService,
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

        // Cargar skills
        this.skillsSub?.unsubscribe();
        this.skillsSub = this.skillService.getPublicSkills().pipe(
            catchError(() => of([]))
        ).subscribe(list => this.skills = list);

        // Cargar proyectos
        this.projectsSub?.unsubscribe();
        this.projectsSub = this.projectService.getPublicProjects().pipe(
            catchError(() => of([]))
        ).subscribe(list => this.projects = list);

        // Si el usuario está logueado, cargar SU perfil (para la foto en el navbar)
        if (this.authState.isAuthenticated) {
            this.myProfileSub?.unsubscribe();
            this.myProfileSub = this.profileService.getMyProfile().pipe(
                catchError(() => of(null))
            ).subscribe(profile => {
                // Actualizamos myProfile$ con el perfil del usuario logueado
                this.myProfile$ = of(profile);
            });
        }
    }

    ngOnDestroy() {
        this.skillsSub?.unsubscribe();
        this.projectsSub?.unsubscribe();
        this.myProfileSub?.unsubscribe();
    }

    prevProject(): void {
        if (this.projects.length === 0) return;
        this.currentProjectIndex = this.currentProjectIndex === 0
            ? this.projects.length - 1
            : this.currentProjectIndex - 1;
    }

    nextProject(): void {
        if (this.projects.length === 0) return;
        this.currentProjectIndex = this.currentProjectIndex === this.projects.length - 1
            ? 0
            : this.currentProjectIndex + 1;
    }

    getStacksSummary(stacks: { summary: string }[]): string {
        return stacks.map(s => s.summary).join(' • ');
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