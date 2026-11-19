import { useCallback, useEffect, useMemo, useState, type ReactNode } from 'react';
import { authApi } from '../api/auth';
import type { LoginRequest, RegisterRequest, User } from '../types/auth';
import { AuthContext, type AuthContextValue } from './AuthContext';

export default function AuthProvider({ children }: { children: ReactNode }) {
    const [user, setUser] = useState<User | null>(null);
    const [initializing, setInitializing] = useState(true);

    // On startup, ask the server whether our cookie is still valid
    useEffect(() => {
        const controller = new AbortController();

        authApi
            .me(controller.signal)
            .then(setUser)
            .catch(() => {
                if (!controller.signal.aborted) setUser(null); // 401 means not logged in
            })
            .finally(() => {
                if (!controller.signal.aborted) setInitializing(false);
            });

        return () => controller.abort();
    }, []);

    const login = useCallback(async (request: LoginRequest) => {
        setUser(await authApi.login(request));
    }, []);

    const register = useCallback(async (request: RegisterRequest) => {
        setUser(await authApi.register(request));
    }, []);

    const logout = useCallback(async () => {
        await authApi.logout();
        setUser(null);
    }, []);

    const value = useMemo<AuthContextValue>(
        () => ({
            user,
            initializing,
            isAdmin: user?.roles.includes('Admin') ?? false,
            login,
            register,
            logout,
        }),
        [user, initializing, login, register, logout],
    );

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}