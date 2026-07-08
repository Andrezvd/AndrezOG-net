import { Component, ElementRef, ViewChild, afterNextRender, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../api/auth.service';
import { LoginRequest } from '../types/auth.types';
import { GoogleIdentityService } from '../../../services/google-identity.service';

@Component({
  selector: 'app-login',
  templateUrl: '../ui/login.component.html',
  styleUrls: ['../css/login.component.css'],
  imports: [FormsModule, RouterLink]
})
export class LoginComponent {
  credentials: LoginRequest = { email: '', password: '' };
  errorMessage = signal<string | null>(null);
  isLoading = signal<boolean>(false);
  @ViewChild('googleHost') googleHost?: ElementRef<HTMLElement>;

  constructor(
    private authService: AuthService,
    private router: Router,
    private googleIdentity: GoogleIdentityService
  ) {
    afterNextRender(() => this.initGoogleButton());
  }

  onSubmit() {
    this.errorMessage.set(null);
    this.isLoading.set(true);
    this.authService.login(this.credentials).subscribe({
      next: (res) => { this.isLoading.set(false); this.navigateByRole(res.role); },
      error: (err) => { this.isLoading.set(false); this.errorMessage.set(extractErrorMessage(err)); }
    });
  }

  initGoogleButton(): void {
    const ready = this.googleIdentity.init((response: any) => this.handleGoogleResponse(response));
    if (!ready) return;

    this.googleIdentity.renderButton(this.googleHost?.nativeElement ?? null);
  }

  handleGoogleResponse(response: any): void {
    this.errorMessage.set(null);
    this.isLoading.set(true);
    const idToken = response.credential;
    if (!idToken) { this.isLoading.set(false); this.errorMessage.set('No se recibió token de Google.'); return; }
    this.authService.externalLogin('Google', idToken).subscribe({
      next: (res) => { this.isLoading.set(false); this.navigateByRole(res.role); },
      error: (err) => { this.isLoading.set(false); this.errorMessage.set(extractErrorMessage(err)); }
    });
  }

  private navigateByRole(role: string): void {
    if (role === 'Admin') { this.router.navigate(['/admin/dashboard']); }
    else { this.router.navigate(['/client/dashboard']); }
  }
}

function extractErrorMessage(err: any): string {
  if (err?.error?.message) return err.error.message;
  if (err?.status === 0 || err?.status === 504) return 'No se pudo conectar con el servidor.';
  if (err?.status) return `Error del servidor (${err.status}).`;
  return 'Error inesperado.';
}