import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { API_URL } from '../../../services-conf/api-config';
import { MyProfileDto } from '../types/profile.types';

@Injectable({ providedIn: 'root' })
export class ProfileService {
    constructor(private http: HttpClient) { }

    getPublicProfile(): Observable<MyProfileDto> {
        return this.http.get<MyProfileDto>(`${API_URL}/profile`);
    }

    getMyProfile(): Observable<MyProfileDto> {
        return this.http.get<MyProfileDto>(`${API_URL}/profile/me`);
    }
}
