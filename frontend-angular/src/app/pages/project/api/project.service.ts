import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../../../services-conf/api-config';
import { ProjectCardDto } from '../types/project.types';

@Injectable({ providedIn: 'root' })
export class ProjectService {
    constructor(private http: HttpClient) { }

    getPublicProjects(): Observable<ProjectCardDto[]> {
        return this.http.get<ProjectCardDto[]>(`${API_URL}/project/public`);
    }
}