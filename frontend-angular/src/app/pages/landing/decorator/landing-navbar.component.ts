import { Component, Input, Output, EventEmitter } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-landing-navbar',
  standalone: true,
  imports: [RouterLink],
  templateUrl: '../ui/landing-navbar.component.html',
  styleUrls: ['../css/landing-navbar.css']
})
export class LandingNavbarComponent {
  @Input() isAuthenticated = false;
  @Input() userName = '';
  @Input() userRole: string | null = '';
  @Input() userInitials = '?';
  @Input() menuOpen = false;
  @Input() userMenuOpen = false;
  @Output() toggleMenu = new EventEmitter<void>();
  @Output() toggleUserMenu = new EventEmitter<void>();
  @Output() logout = new EventEmitter<void>();
}