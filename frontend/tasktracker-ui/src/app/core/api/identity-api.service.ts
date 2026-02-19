import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';
import { LoginRequest, LoginResponse } from '../models/login.models';
import { Observable } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class IdentityApiService {
    private readonly baseUrl = environment.identityApiBaseUrl;

    constructor(private readonly http: HttpClient) { }

    login(req: LoginRequest): Observable<LoginResponse> {
        return this.http.post<LoginResponse>(`${this.baseUrl}login`, req);
    }
}
