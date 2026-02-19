export interface LoginRequest {
    userName: string;
    password: string;
}

export interface LoginResponse {
    accessToken: string;
}
