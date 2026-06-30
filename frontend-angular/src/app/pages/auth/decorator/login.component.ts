import { Component, AfterViewInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../api/auth.service';
import { LoginRequest } from '../types/auth.types';
import { environment } from '../../../../environments/environment.development';

@Component({
  selector: 'app-login',
  templateUrl: '../ui/login.component.html',
  styleUrls: ['../css/login.component.css'],
  imports: [FormsModule, RouterLink]
})
export class LoginComponent implements AfterViewInit {
  credentials: LoginRequest = { email: '', password: '' };
  errorMessage = signal<string | null>(null);
  isLoading = signal<boolean>(false);

  constructor(private authService: AuthService, private router: Router) { }

  ngAfterViewInit(): void {
    this.initGoogleButton();
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
    // @ts-ignore
    if (typeof google === 'undefined' || !google?.accounts) return;
    // @ts-ignore
    google.accounts.id.initialize({
      client_id: environment.googleClientId,
      callback: (response: any) => this.handleGoogleResponse(response),
      ux_mode: 'popup'
    });
    // @ts-ignore
    google.accounts.id.renderButton(
      document.getElementById('googleHiddenBtn'),
      { theme: 'outline', size: 'large', width: 300 }
    );
  }

  googleLogin(): void {
    this.errorMessage.set(null);
    // Simular clic en el botón renderizado de Google (oculto con display:none)
    const btn = document.querySelector<HTMLElement>('#googleHiddenBtn div[role=button]');
    if (btn) {
      btn.click();
    }
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
    else { this.router.navigate(['/']); }
  }
}

function extractErrorMessage(err: any): string {
  if (err?.error?.message) return err.error.message;
  if (err?.status === 0 || err?.status === 504) return 'No se pudo conectar con el servidor.';
  if (err?.status) return `Error del servidor (${err.status}).`;
  return 'Error inesperado.';
}