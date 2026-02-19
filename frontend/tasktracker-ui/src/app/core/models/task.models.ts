export enum TaskPriority {
    Low = 0,
    Medium = 1,
    High = 2
}

export interface TaskItem {
    id: string;
    ownerUserId: string;
    title: string;
    description?: string | null;
    priority: TaskPriority;
    dueDate?: string | null;
    isCompleted: boolean;
}

export interface CreateTaskRequest {
    title: string;
    description?: string | null;
    priority: TaskPriority;
    dueDate?: string | null;
}

export interface UpdateTaskRequest {
    title: string;
    description?: string | null;
    priority: TaskPriority;
    dueDate?: string | null;
    isCompleted: boolean;
}
