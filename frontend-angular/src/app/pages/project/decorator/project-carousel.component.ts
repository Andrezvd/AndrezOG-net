import { Component, Input } from '@angular/core';
import { ProjectCardDto } from '../types/project.types';

@Component({
    selector: 'app-project-carousel',
    standalone: true,
    templateUrl: '../ui/project-carousel.component.html',
    styleUrls: ['../css/project-carousel.css']
})
export class ProjectCarouselComponent {
    @Input() projects: ProjectCardDto[] = [];
}