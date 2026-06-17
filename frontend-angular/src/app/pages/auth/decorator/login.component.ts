import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../api/auth.service';
import { Router } from '@angular/router';
import { LoginRequest, ErrorResponse } from '../types/auth.types';

@Component({
    selector: 'app-login',
    templateUrl: '../ui/login.component.html',
    styleUrls: ['../css/login.component.css'],
    imports: [FormsModule, RouterLink]
})
export class LoginComponent {
    credentials: LoginRequest = {
        email: '',
        password: ''
    };
    errorMessage: string | null = null;

    constructor(
        private authService: AuthService,
        private router: Router
    ) { }

    onSubmit() {
        this.errorMessage = null;

        this.authService.login(this.credentials).subscribe({
            next: (res) => {
                localStorage.setItem('token', res.token);
                this.router.navigate(['/']);
            },
            error: (err) => {
                const body: ErrorResponse = err.error;
                this.errorMessage = body?.message ?? 'Email o contraseña incorrectos.';
            }
        });
    }
}