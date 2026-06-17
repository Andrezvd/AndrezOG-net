import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../api/auth.service';
import { Router } from '@angular/router';
import { RegisterRequest, ErrorResponse } from '../types/auth.types';

@Component({
    selector: 'app-register',
    templateUrl: '../ui/register.component.html',
    styleUrls: ['../css/register.component.css'],
    imports: [FormsModule, RouterLink]
})
export class RegisterComponent {
    RegisterRequest: RegisterRequest = {
        email: '',
        password: '',
        confirmPassword: '',
        name: '',
        lastName: '',
        phoneNumber: '',
        country: ''
    };
    errorMessage: string | null = null;

    constructor(
        private authService: AuthService,
        private router: Router
    ) {}

    onSubmit() {
        this.errorMessage = null;

        if (this.RegisterRequest.password !== this.RegisterRequest.confirmPassword) {
            this.errorMessage = 'Las contraseñas no coinciden';
            return;
        }

        this.authService.register(this.RegisterRequest).subscribe({
            next: () => {
                this.router.navigate(['/login']);
            },
            error: (err) => {
                const body: ErrorResponse = err.error;
                this.errorMessage = body?.message ?? 'Error al registrar. Intenta de nuevo.';
            }
        });
    }
}
