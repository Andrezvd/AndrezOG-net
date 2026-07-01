import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../../../services-conf/api-config';
import { SkillCardDto } from '../types/skill.types';

@Injectable({ providedIn: 'root' })
export class SkillService {
    constructor(private http: HttpClient) { }

    getPublicSkills(): Observable<SkillCardDto[]> {
        return this.http.get<SkillCardDto[]>(`${API_URL}/skill/public`);
    }
}