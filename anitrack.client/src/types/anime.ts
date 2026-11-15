export type AnimeFormat = 'TV' | 'TVShort' | 'Movie' | 'Special' | 'OVA' | 'ONA' | 'Music';
export type AnimeStatus = 'Finished' | 'Releasing' | 'NotYetReleased' | 'Cancelled' | 'Hiatus';
export type AnimeSeason = 'Winter' | 'Spring' | 'Summer' | 'Fall';
export type AnimeSort = 'Popularity' | 'Score' | 'Title' | 'Newest';

export interface AnimeSummary {
    id: number;
    title: string;
    coverImageUrl: string | null;
    format: AnimeFormat | null;
    season: AnimeSeason | null;
    seasonYear: number | null;
    episodes: number | null;
    averageScore: number | null;
    genres: string[];
}

export interface AnimeDetail {
    id: number;
    aniListId: number;
    titleRomaji: string;
    titleEnglish: string | null;
    description: string | null;
    coverImageUrl: string | null;
    bannerImageUrl: string | null;
    format: AnimeFormat | null;
    status: AnimeStatus | null;
    season: AnimeSeason | null;
    seasonYear: number | null;
    episodes: number | null;
    durationMinutes: number | null;
    averageScore: number | null;
    popularity: number | null;
    startDate: string | null; // "2013-04-07"
    endDate: string | null;
    genres: string[];
}

export interface PagedResult<T> {
    items: T[];
    page: number;
    pageSize: number;
    totalCount: number;
    totalPages: number;
    hasNextPage: boolean;
}

export interface AnimeQuery {
    search?: string;
    genre?: string;
    year?: number;
    season?: AnimeSeason;
    format?: AnimeFormat;
    status?: AnimeStatus;
    sort?: AnimeSort;
    page?: number;
    pageSize?: number;
}