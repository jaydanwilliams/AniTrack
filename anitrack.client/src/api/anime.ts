import { getJson } from './client';
import type { AnimeDetail, AnimeQuery, AnimeSummary, PagedResult } from '../types/anime';

function toQueryString(query: AnimeQuery): string {
    const params = new URLSearchParams();
    for (const [key, value] of Object.entries(query)) {
        if (value !== undefined && value !== null && value !== '') {
            params.set(key, String(value));
        }
    }
    const qs = params.toString();
    return qs ? `?${qs}` : '';
}

export const animeApi = {
    search: (query: AnimeQuery, signal?: AbortSignal) =>
        getJson<PagedResult<AnimeSummary>>(`/api/anime${toQueryString(query)}`, signal),

    getById: (id: number, signal?: AbortSignal) =>
        getJson<AnimeDetail>(`/api/anime/${id}`, signal),

    getGenres: (signal?: AbortSignal) =>
        getJson<string[]>('/api/genres', signal),
};