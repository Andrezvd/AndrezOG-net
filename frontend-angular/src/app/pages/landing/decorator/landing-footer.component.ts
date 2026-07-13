import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-landing-footer',
  standalone: true,
  imports: [RouterLink],
  templateUrl: '../ui/landing-footer.component.html',
  styleUrls: ['../css/landing-footer.css']
})
export class LandingFooterComponent {}
