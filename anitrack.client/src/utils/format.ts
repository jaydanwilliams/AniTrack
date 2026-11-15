import type { AnimeFormat, AnimeStatus } from '../types/anime';

export const formatLabels: Record<AnimeFormat, string> = {
    TV: 'TV',
    TVShort: 'TV Short',
    Movie: 'Movie',
    Special: 'Special',
    OVA: 'OVA',
    ONA: 'ONA',
    Music: 'Music',
};

export const statusLabels: Record<AnimeStatus, string> = {
    Finished: 'Finished',
    Releasing: 'Releasing',
    NotYetReleased: 'Not Yet Released',
    Cancelled: 'Cancelled',
    Hiatus: 'Hiatus',
};

// "2013-04-07" → "Apr 7, 2013"
export function formatDate(value: string | null): string | null {
    if (!value) return null;
    const [year, month, day] = value.split('-').map(Number);
    return new Date(year, month - 1, day).toLocaleDateString(undefined, {
        year: 'numeric',
        month: 'short',
        day: 'numeric',
    });
}

// AniList descriptions contain HTML tags like <br> and <i>. Turn them into plain text.
export function cleanDescription(html: string | null): string {
    if (!html) return 'No description available.';
    return html
        .replace(/<br\s*\/?>/gi, '\n')
        .replace(/<[^>]+>/g, '')
        .replace(/&amp;/g, '&')
        .replace(/&quot;/g, '"')
        .replace(/&#039;/g, "'")
        .replace(/&lt;/g, '<')
        .replace(/&gt;/g, '>')
        .replace(/\n{3,}/g, '\n\n')
        .trim();
}