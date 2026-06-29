import { Routes } from '@angular/router';
import { LoginComponent } from './pages/auth/decorator/login.component';
import { RegisterComponent } from './pages/auth/decorator/register.component';
import { LandingComponent } from './pages/landing/landing.component';
import { AdminDashboardComponent } from './pages/admin/decorator/dashboard.component';
import { AdminGuard } from './services/auth.guard';

export const routes: Routes = [
    { path: '', component: LandingComponent },
    { path: 'login', component: LoginComponent },
    { path: 'register', component: RegisterComponent },
    {
        path: 'admin/dashboard',
        component: AdminDashboardComponent,
        canActivate: [AdminGuard]
    },
    { path: '**', redirectTo: '' },
];
