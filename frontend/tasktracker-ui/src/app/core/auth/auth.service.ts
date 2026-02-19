import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { tap } from 'rxjs';
import { IdentityApiService } from '../api/identity-api.service';
import { TokenStorage } from './token.storage';
import { LoginRequest } from '../models/login.models';

@Injectable({ providedIn: 'root' })
export class AuthService {
    constructor(
        private readonly identityApi: IdentityApiService,
        private readonly tokenStorage: TokenStorage,
        private readonly router: Router
    ) { }

    login(req: LoginRequest) {
        return this.identityApi.login(req).pipe(
            tap(res => this.tokenStorage.setToken(res.accessToken))
        );
    }

    logout(): void {
        this.tokenStorage.clear();
        this.router.navigate(['/login']);
    }

    isAuthenticated(): boolean {
        return !!this.tokenStorage.getToken();
    }

    getToken(): string | null {
        return this.tokenStorage.getToken();
    }
}
