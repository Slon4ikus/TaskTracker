import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { TokenStorage } from './token.storage';

export const authGuard: CanActivateFn = () => {
    const token = inject(TokenStorage).getToken();
    if (token) return true;

    inject(Router).navigate(['/login']);
    return false;
};
