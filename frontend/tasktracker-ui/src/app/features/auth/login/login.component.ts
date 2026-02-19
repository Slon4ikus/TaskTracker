import { Component, ChangeDetectorRef, NgZone } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { firstValueFrom } from 'rxjs';
import { AuthService } from '../../../core/auth/auth.service';

@Component({
    selector: 'app-login',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './login.component.html'
})
export class LoginComponent {
    userName = '';
    password = '';

    isSubmitting = false;
    error: string | null = null;

    constructor(
        private readonly auth: AuthService,
        private readonly router: Router,
        private readonly zone: NgZone,
        private readonly cdr: ChangeDetectorRef
    ) { }

    clearError(): void {
        this.error = null;
        this.cdr.markForCheck();
    }

    async login(): Promise<void> {
        if (this.isSubmitting) return;

        const userName = this.userName.trim();
        if (!userName || !this.password) {
            this.error = 'Enter username and password';
            this.cdr.markForCheck();
            return;
        }

        this.isSubmitting = true;
        this.error = null;
        this.cdr.markForCheck();

        try {
            await firstValueFrom(this.auth.login({ userName, password: this.password }));
            await this.zone.run(() => this.router.navigate(['/tasks']));
        } catch (e: any) {
            this.zone.run(() => {
                this.error = e?.status === 401 ? 'Invalid username or password' : 'Login failed';
                this.isSubmitting = false;
                this.cdr.markForCheck();
            });
            return;
        }

        this.zone.run(() => {
            this.isSubmitting = false;
            this.cdr.markForCheck();
        });
    }
}
