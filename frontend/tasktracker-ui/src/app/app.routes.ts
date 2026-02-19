import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { LoginComponent } from './features/auth/login/login.component';
import { TasksComponent } from './features/tasks/tasks.component';

export const routes: Routes = [
    { path: 'login', component: LoginComponent },

    {
        path: 'tasks',
        component: TasksComponent,
        canActivate: [authGuard]
    },

    { path: '', pathMatch: 'full', redirectTo: 'login' },

    { path: '**', redirectTo: 'login' }
];
