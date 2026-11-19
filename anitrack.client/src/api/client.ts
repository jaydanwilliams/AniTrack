export interface ProblemDetails {
    title?: string;
    status?: number;
    detail?: string;
    errors?: Record<string, string[]>;
}

export class ApiError extends Error {
    status: number;
    problem: ProblemDetails | null;

    constructor(status: number, problem: ProblemDetails | null) {
        super(problem?.title ?? `Request failed with status ${status}`);
        this.status = status;
        this.problem = problem;
        this.name = 'ApiError';
    }
}

async function request<T>(method: string, url: string, body?: unknown, signal?: AbortSignal): Promise<T> {
    const headers: Record<string, string> = { Accept: 'application/json' };
    if (body !== undefined) headers['Content-Type'] = 'application/json';

    const response = await fetch(url, {
        method,
        headers,
        body: body !== undefined ? JSON.stringify(body) : undefined,
        signal,
    });

    if (!response.ok) {
        let problem: ProblemDetails | null = null;
        try {
            problem = (await response.json()) as ProblemDetails;
        } catch {
            // the error response had no JSON body (e.g. a bare 401)
        }
        throw new ApiError(response.status, problem);
    }

    if (response.status === 204) return undefined as T; // 204 No Content has no body to read
    return (await response.json()) as T;
}

export function getJson<T>(url: string, signal?: AbortSignal): Promise<T> {
    return request<T>('GET', url, undefined, signal);
}

export function postJson<T>(url: string, body?: unknown): Promise<T> {
    return request<T>('POST', url, body);
}