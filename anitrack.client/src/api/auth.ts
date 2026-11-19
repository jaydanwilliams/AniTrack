import { getJson, postJson } from './client';
import type { LoginRequest, RegisterRequest, User } from '../types/auth';

export const authApi = {
    me: (signal?: AbortSignal) => getJson<User>('/api/auth/me', signal),
    login: (body: LoginRequest) => postJson<User>('/api/auth/login', body),
    register: (body: RegisterRequest) => postJson<User>('/api/auth/register', body),
    logout: () => postJson<void>('/api/auth/logout'),
};