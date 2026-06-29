import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../auth/api/auth.service';
import { AuthStateService } from '../../../services/auth-state.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  templateUrl: '../ui/dashboard.component.html',
  styleUrls: ['../css/dashboard.component.css']
})
export class AdminDashboardComponent {
  userName: string;
  userEmail: string;

  constructor(
    private authState: AuthStateService,
    private authService: AuthService,
    private router: Router
  ) {
    this.userName = this.authState.state.name ?? 'Admin';
    this.userEmail = this.authState.state.email ?? '';
  }

  logout(): void {
    this.authService.logout().subscribe({
      next: () => this.router.navigate(['/login']),
      error: () => this.router.navigate(['/login'])
    });
  }
}