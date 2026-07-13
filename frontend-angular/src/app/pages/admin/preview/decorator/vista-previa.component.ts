import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-vista-previa',
  standalone: true,
  templateUrl: '../ui/vista-previa.component.html',
  styleUrls: ['../css/vista-previa.component.css'],
  imports: [CommonModule, RouterLink]
})
export class VistaPreviaComponent {
  @Input() profile: any = null;
  @Input() apiImagesUrl: string = '';
}
