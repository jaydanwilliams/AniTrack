import { ApiError } from '../api/client';

// The first error for one field, e.g. "Password" → "Passwords must be at least 8 characters."
export function getFieldError(error: unknown, field: string): string | undefined {
    if (!(error instanceof ApiError) || !error.problem?.errors) return undefined;
    const key = Object.keys(error.problem.errors).find((k) => k.toLowerCase() === field.toLowerCase());
    return key ? error.problem.errors[key][0] : undefined;
}

// An error that isn't about a specific field, to show at the top of the form
export function getErrorMessage(error: unknown): string | undefined {
    if (!error) return undefined;
    if (error instanceof ApiError) {
        if (error.problem?.errors) return error.problem.errors['']?.[0]; // field errors are shown next to fields
        return error.problem?.title ?? 'Something went wrong. Please try again.';
    }
    return 'Could not reach the server. Please try again.';
}