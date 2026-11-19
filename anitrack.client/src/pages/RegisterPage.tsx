import { useState, type FormEvent } from 'react';
import { Link, Navigate } from 'react-router';
import FormField from '../components/FormField';
import { useAuth } from '../hooks/useAuth';
import { getErrorMessage, getFieldError } from '../utils/problem';

export default function RegisterPage() {
    const { user, register } = useAuth();

    const [userName, setUserName] = useState('');
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState<unknown>(null);

    if (user) return <Navigate to="/browse" replace />;

    async function handleSubmit(e: FormEvent<HTMLFormElement>) {
        e.preventDefault();
        setSubmitting(true);
        setError(null);
        try {
            await register({ userName, email, password });
        } catch (err) {
            setError(err);
        } finally {
            setSubmitting(false);
        }
    }

    const formError = getErrorMessage(error);

    return (
        <div className="auth-card">
            <h1>Create an account</h1>
            <form onSubmit={handleSubmit} noValidate>
                <FormField
                    label="Username"
                    name="userName"
                    autoComplete="username"
                    value={userName}
                    onChange={(e) => setUserName(e.target.value)}
                    error={getFieldError(error, 'userName')}
                />
                <FormField
                    label="Email"
                    name="email"
                    type="email"
                    autoComplete="email"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    error={getFieldError(error, 'email')}
                />
                <FormField
                    label="Password"
                    name="password"
                    type="password"
                    autoComplete="new-password"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    error={getFieldError(error, 'password')}
                />

                {formError && <p className="form-error">{formError}</p>}

                <button className="btn btn--primary" type="submit" disabled={submitting}>
                    {submitting ? 'Creating account…' : 'Sign up'}
                </button>
            </form>
            <p className="auth-card__footer">
                Already have an account? <Link to="/login">Log in</Link>
            </p>
        </div>
    );
}