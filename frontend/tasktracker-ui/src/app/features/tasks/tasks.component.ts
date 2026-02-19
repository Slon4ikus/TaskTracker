import { Component, OnInit, ChangeDetectorRef, NgZone } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TasksApiService } from '../../core/api/tasks-api.service';
import { TaskItem, CreateTaskRequest, TaskPriority, UpdateTaskRequest } from '../../core/models/task.models';


@Component({
    selector: 'app-tasks',
    standalone: true,
    imports: [CommonModule, FormsModule],
    templateUrl: './tasks.component.html'
})
export class TasksComponent implements OnInit {

    tasks: TaskItem[] = [];

    title = '';
    description = '';
    priority: TaskPriority = TaskPriority.Medium;
    dueDate: string | null = null;

    isSubmitting = false;
    error: string | null = null;

    TaskPriority = TaskPriority;

    editingId: string | null = null;

    edit: {
        title: string;
        description: string | null;
        priority: TaskPriority;
        dueDate: string | null;
        isCompleted: boolean;
    } = {
            title: '',
            description: null,
            priority: TaskPriority.Medium,
            dueDate: null,
            isCompleted: false
        };

    constructor(
        private readonly api: TasksApiService,
        private readonly zone: NgZone,
        private readonly cdr: ChangeDetectorRef
    ) { }

    private refreshUi(): void {
        this.cdr.markForCheck();
        this.cdr.detectChanges();
    }

    ngOnInit(): void {
        this.load();
    }

    load(): void {
        this.api.getMine().subscribe({
            next: tasks => this.zone.run(() => {
                this.tasks = tasks;
                this.refreshUi();
            }),
            error: () => this.zone.run(() => {
                this.error = 'Failed to load tasks';
                this.refreshUi();
            })
        });
    }

    create(): void {
        const title = this.title.trim();
        if (!title || this.isSubmitting) return;

        const request: CreateTaskRequest = {
            title,
            description: this.description || null,
            priority: this.priority,
            dueDate: this.dueDate || null
        };

        this.isSubmitting = true;
        this.error = null;
        this.refreshUi();

        this.api.create(request).subscribe({
            next: () => this.zone.run(() => {
                this.title = '';
                this.description = '';
                this.priority = TaskPriority.Medium;
                this.dueDate = null;
                this.isSubmitting = false;
                this.refreshUi();
                this.load();
            }),
            error: (e) => this.zone.run(() => {
                this.error = e?.status === 400 ? 'Bad request' : 'Failed to create task';
                this.isSubmitting = false;
                this.refreshUi();
            })
        });
    }

    delete(id: string): void {
        if (!confirm('Delete this task?')) return;

        this.api.delete(id).subscribe({
            next: () => this.load(),
            error: () => this.error = 'Failed to delete task'
        });
    }

    getPriorityLabel(priority: number): string {
        switch (priority) {
            case TaskPriority.Low: return 'Low';
            case TaskPriority.Medium: return 'Medium';
            case TaskPriority.High: return 'High';
            default: return '';
        }
    }

    startEdit(t: TaskItem): void {
        this.editingId = t.id;

        this.edit = {
            title: t.title ?? '',
            description: t.description ?? null,
            priority: t.priority,
            dueDate: t.dueDate ?? null,
            isCompleted: !!t.isCompleted
        };

        this.error = null;
    }

    cancelEdit(): void {
        this.editingId = null;
        this.error = null;
    }

    saveEdit(t: TaskItem): void {
        if (!this.editingId) return;

        const title = this.edit.title.trim();
        if (!title) {
            this.error = 'Title is required';
            return;
        }

        const req: UpdateTaskRequest = {
            title,
            description: this.edit.description,
            priority: this.edit.priority,
            dueDate: this.edit.dueDate,
            isCompleted: this.edit.isCompleted
        };

        this.api.update(t.id, req).subscribe({
            next: () => {
                this.editingId = null;
                this.load();
            },
            error: () => {
                this.error = 'Failed to update task';
            }
        });
    }
}
