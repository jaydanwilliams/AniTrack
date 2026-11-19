import { useState, type FormEvent } from 'react';
import { Link, Navigate, useLocation } from 'react-router';
import FormField from '../components/FormField';
import { useAuth } from '../hooks/useAuth';
import { getErrorMessage, getFieldError } from '../utils/problem';

export default function LoginPage() {
    const { user, login } = useAuth();
    const location = useLocation();
    const from = (location.state as { from?: string } | null)?.from ?? '/browse';

    const [userNameOrEmail, setUserNameOrEmail] = useState('');
    const [password, setPassword] = useState('');
    const [rememberMe, setRememberMe] = useState(false);
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState<unknown>(null);

    // Already logged in (or just logged in) → go where they were headed
    if (user) return <Navigate to={from} replace />;

    async function handleSubmit(e: FormEvent<HTMLFormElement>) {
        e.preventDefault();
        setSubmitting(true);
        setError(null);
        try {
            await login({ userNameOrEmail, password, rememberMe });
        } catch (err) {
            setError(err);
        } finally {
            setSubmitting(false);
        }
    }

    const formError = getErrorMessage(error);

    return (
        <div className="auth-card">
            <h1>Log in</h1>
            <form onSubmit={handleSubmit} noValidate>
                <FormField
                    label="Username or email"
                    name="userNameOrEmail"
                    autoComplete="username"
                    value={userNameOrEmail}
                    onChange={(e) => setUserNameOrEmail(e.target.value)}
                    error={getFieldError(error, 'userNameOrEmail')}
                />
                <FormField
                    label="Password"
                    name="password"
                    type="password"
                    autoComplete="current-password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    error={getFieldError(error, 'password')}
                />
                <label className="checkbox">
                    <input type="checkbox" checked={rememberMe} onChange={(e) => setRememberMe(e.target.checked)} />
                    Remember me
                </label>

                {formError && <p className="form-error">{formError}</p>}

                <button className="btn btn--primary" type="submit" disabled={submitting}>
                    {submitting ? 'Logging in…' : 'Log in'}
                </button>
            </form>
            <p className="auth-card__footer">
                No account? <Link to="/register">Sign up</Link>
            </p>
        </div>
    );
}