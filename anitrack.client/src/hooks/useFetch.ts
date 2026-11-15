import { useEffect, useState } from 'react';

interface FetchState<T> {
    data: T | null;
    loading: boolean;
    error: Error | null;
}

interface FetchResult<T> {
    key: string;        // which request this result belongs to
    data: T | null;
    error: Error | null;
}

export function useFetch<T>(
    fetcher: (signal: AbortSignal) => Promise<T>,
    deps: unknown[],
): FetchState<T> {
    // A string that identifies the current request, e.g. '["genre=Romance&page=2"]'
    const key = JSON.stringify(deps);

    const [result, setResult] = useState<FetchResult<T> | null>(null);

    useEffect(() => {
        const controller = new AbortController();

        // setState is only called inside async callbacks, never directly in the effect body
        fetcher(controller.signal)
            .then((data) => setResult({ key, data, error: null }))
            .catch((error) => {
                if (controller.signal.aborted) return; // cancelled on purpose; ignore
                setResult({ key, data: null, error });
            });

        return () => controller.abort();
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [key]);

    return {
        data: result?.data ?? null,                              // previous data stays visible while loading
        error: result?.key === key ? result.error : null,        // only show errors for the current request
        loading: result?.key !== key,                            // calculated, not stored
    };
}