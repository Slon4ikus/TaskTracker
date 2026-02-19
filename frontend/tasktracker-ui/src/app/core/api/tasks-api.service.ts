import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment.development';
import { Observable } from 'rxjs';
import { CreateTaskRequest, TaskItem, UpdateTaskRequest } from '../models/task.models';

@Injectable({ providedIn: 'root' })
export class TasksApiService {
    private readonly baseUrl = environment.tasksApiBaseUrl;

    constructor(private readonly http: HttpClient) { }

    getMine(): Observable<TaskItem[]> {
        return this.http.get<TaskItem[]>(this.baseUrl);
    }

    create(req: CreateTaskRequest): Observable<{ id: string }> {
        return this.http.post<{ id: string }>(this.baseUrl, req);
    }

    update(id: string, req: UpdateTaskRequest): Observable<void> {
        return this.http.put<void>(`${this.baseUrl}${id}`, req);
    }

    delete(id: string): Observable<void> {
        return this.http.delete<void>(`${this.baseUrl}${id}`);
    }

}
