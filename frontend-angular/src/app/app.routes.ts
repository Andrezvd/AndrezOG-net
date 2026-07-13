import { Routes } from '@angular/router';
import { LoginComponent } from './pages/auth/decorator/login.component';
import { RegisterComponent } from './pages/auth/decorator/register.component';
import { LandingComponent } from './pages/landing/decorator/landing.component';
import { AdminDashboardComponent } from './pages/admin/decorator/dashboard.component';
import { ClientDashboardComponent } from './pages/tenants/dashboard/decorator/client-dashboard.component';
import { AdminGuard } from './services/auth.guard';
import { AuthGuard } from './services/auth.guard';

export const routes: Routes = [
    { path: '', component: LandingComponent, data: { title: 'OG Home' } },
    { path: 'login', component: LoginComponent, data: { title: 'OG Login' } },
    { path: 'register', component: RegisterComponent, data: { title: 'OG Register' } },
    {
        path: 'admin/dashboard',
        component: AdminDashboardComponent,
        canActivate: [AdminGuard],
        data: { title: 'OG Admin' }
    },
    {
        path: 'client/dashboard',
        component: ClientDashboardComponent,
        canActivate: [AuthGuard],
        data: { title: 'OG Client' }
    },
    { path: '**', redirectTo: '' },
];
