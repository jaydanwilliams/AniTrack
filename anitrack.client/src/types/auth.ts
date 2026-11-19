export interface User {
    id: string;
    userName: string;
    email: string;
    roles: string[];
}

export interface LoginRequest {
    userNameOrEmail: string;
    password: string;
    rememberMe: boolean;
}

export interface RegisterRequest {
    userName: string;
    email: string;
    password: string;
}