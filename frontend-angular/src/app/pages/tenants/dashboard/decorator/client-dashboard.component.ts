import { Component } from '@angular/core';
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../../../auth/api/auth.service';

@Component({
    selector: 'app-client-dashboard',
    standalone: true,
    imports: [RouterLink],
    templateUrl: '../ui/client-dashboard.component.html',
    styleUrls: ['../css/client-dashboard.css']
})
export class ClientDashboardComponent {
    constructor(
        private authService: AuthService,
        private router: Router
    ) {}

    logout(): void {
        this.authService.logout().subscribe({
            next: () => this.router.navigate(['/']),
            error: () => this.router.navigate(['/'])
        });
    }
}