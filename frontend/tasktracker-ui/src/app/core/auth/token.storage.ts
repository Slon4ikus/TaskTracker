import { Injectable } from '@angular/core';

const TOKEN_KEY = 'tasktracker.token';

@Injectable({ providedIn: 'root' })
export class TokenStorage {
    getToken(): string | null {
        return localStorage.getItem(TOKEN_KEY);
    }

    setToken(token: string): void {
        localStorage.setItem(TOKEN_KEY, token);
    }

    clear(): void {
        localStorage.removeItem(TOKEN_KEY);
    }
}
