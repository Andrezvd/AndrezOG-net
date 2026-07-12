import { Component, Input, Output, EventEmitter } from '@angular/core';
import { ProjectCardDto } from '../types/project.types';

@Component({
    selector: 'app-project-carousel',
    standalone: true,
    templateUrl: '../ui/project-carousel.component.html',
    styleUrls: ['../css/project-carousel.css']
})
export class ProjectCarouselComponent {
    @Input() projects: ProjectCardDto[] = [];
    @Input() currentIndex = 0;
    @Output() prev = new EventEmitter<void>();
    @Output() next = new EventEmitter<void>();
    @Output() goTo = new EventEmitter<number>();

    getStacksSummary(stacks: { summary: string }[]): string {
        return stacks.map(s => s.summary).join(' • ');
    }
}