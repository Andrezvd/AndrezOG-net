import { Component, AfterViewInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../api/auth.service';
import { RegisterRequest } from '../types/auth.types';
import { environment } from '../../../../environments/environment.development';

@Component({
  selector: 'app-register',
  templateUrl: '../ui/register.component.html',
  styleUrls: ['../css/register.component.css'],
  imports: [FormsModule, RouterLink]
})
export class RegisterComponent implements AfterViewInit {
  RegisterRequest: RegisterRequest = {
    email: '',
    password: '',
    confirmPassword: '',
    name: '',
    lastName: '',
    phoneNumber: '',
    country: ''
  };
  errorMessage = signal<string | null>(null);
  isLoading = signal<boolean>(false);

  constructor(
    private authService: AuthService,
    private router: Router
  ) { }

  ngAfterViewInit(): void {
    this.initGoogleButton();
  }

  onSubmit() {
    this.errorMessage.set(null);
    this.isLoading.set(true);

    if (this.RegisterRequest.password !== this.RegisterRequest.confirmPassword) {
      this.errorMessage.set('Las contraseñas no coinciden');
      this.isLoading.set(false);
      return;
    }

    this.authService.register(this.RegisterRequest).subscribe({
      next: (res) => {
        this.isLoading.set(false);
        this.navigateByRole(res.role);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(extractErrorMessage(err));
      }
    });
  }

  initGoogleButton(): void {
    // @ts-ignore
    if (typeof google === 'undefined' || !google.accounts) {
      return;
    }

    // @ts-ignore
    google.accounts.id.initialize({
      client_id: environment.googleClientId,
      callback: (response: any) => this.handleGoogleResponse(response),
      ux_mode: 'popup'
    });

    // @ts-ignore
    google.accounts.id.renderButton(
      document.getElementById('googleSignInBtn'),
      { theme: 'outline', size: 'large', width: '100%' }
    );
  }

  handleGoogleResponse(response: any): void {
    this.errorMessage.set(null);
    this.isLoading.set(true);

    const idToken = response.credential;

    if (!idToken) {
      this.isLoading.set(false);
      this.errorMessage.set('No se recibió el token de Google. Inténtalo de nuevo.');
      return;
    }

    this.authService.externalLogin('Google', idToken).subscribe({
      next: (res) => {
        this.isLoading.set(false);
        this.navigateByRole(res.role);
      },
      error: (err) => {
        this.isLoading.set(false);
        this.errorMessage.set(extractErrorMessage(err));
      }
    });
  }

  private navigateByRole(role: string): void {
    if (role === 'Admin') {
      this.router.navigate(['/admin/dashboard']);
    } else {
      this.router.navigate(['/']);
    }
  }
}

function extractErrorMessage(err: any): string {
  if (err?.error?.message) {
    return err.error.message;
  }
  if (err?.status === 0 || err?.status === 504) {
    return 'No se pudo conectar con el servidor. Verifica tu conexión o inténtalo más tarde.';
  }
  if (err?.status) {
    return `Error del servidor (${err.status}). Inténtalo de nuevo.`;
  }
  return 'Error inesperado. Inténtalo de nuevo.';
}