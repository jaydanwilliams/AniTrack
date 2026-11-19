import { createContext } from 'react';
import type { LoginRequest, RegisterRequest, User } from '../types/auth';

export interface AuthContextValue {
    user: User | null;
    initializing: boolean;   // true until we've asked the server "am I logged in?"
    isAdmin: boolean;
    login: (request: LoginRequest) => Promise<void>;
    register: (request: RegisterRequest) => Promise<void>;
    logout: () => Promise<void>;
}

export const AuthContext = createContext<AuthContextValue | null>(null);