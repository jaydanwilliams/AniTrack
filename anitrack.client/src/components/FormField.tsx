import type { InputHTMLAttributes } from 'react';

interface FormFieldProps extends InputHTMLAttributes<HTMLInputElement> {
    label: string;
    error?: string;
}

export default function FormField({ label, error, id, ...inputProps }: FormFieldProps) {
    const inputId = id ?? inputProps.name;
    const errorId = `${inputId}-error`;

    return (
        <div className="form-field">
            <label htmlFor={inputId}>{label}</label>
            <input
                id={inputId}
                aria-invalid={error ? true : undefined}
                aria-describedby={error ? errorId : undefined}
                {...inputProps}
            />
            {error && <p id={errorId} className="form-field__error">{error}</p>}
        </div>
    );
}