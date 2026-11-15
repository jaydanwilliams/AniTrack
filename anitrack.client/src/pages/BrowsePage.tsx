import { useEffect, useState } from 'react';
import { useSearchParams } from 'react-router';
import { animeApi } from '../api/anime';
import AnimeCard from '../components/AnimeCard';
import Pagination from '../components/Pagination';
import { useDebounce } from '../hooks/useDebounce';
import { useFetch } from '../hooks/useFetch';
import type { AnimeFormat, AnimeQuery, AnimeSeason, AnimeSort } from '../types/anime';
import { formatLabels } from '../utils/format';

const SEASONS: AnimeSeason[] = ['Winter', 'Spring', 'Summer', 'Fall'];

const SORTS: { value: AnimeSort; label: string }[] = [
    { value: 'Popularity', label: 'Popularity' },
    { value: 'Score', label: 'Average score' },
    { value: 'Newest', label: 'Newest' },
    { value: 'Title', label: 'Title' },
];

const currentYear = new Date().getFullYear();
const YEARS = Array.from({ length: currentYear + 1 - 1970 + 1 }, (_, i) => currentYear + 1 - i);

export default function BrowsePage() {
    const [params, setParams] = useSearchParams();

    // The URL is the single source of truth for every filter
    const query: AnimeQuery = {
        search: params.get('search') ?? undefined,
        genre: params.get('genre') ?? undefined,
        year: params.has('year') ? Number(params.get('year')) : undefined,
        season: (params.get('season') ?? undefined) as AnimeSeason | undefined,
        format: (params.get('format') ?? undefined) as AnimeFormat | undefined,
        sort: (params.get('sort') ?? 'Popularity') as AnimeSort,
        page: Number(params.get('page') ?? '1'),
        pageSize: 24,
    };

    const { data, loading, error } = useFetch(
        (signal) => animeApi.search(query, signal),
        [params.toString()],
    );
    const { data: genres } = useFetch((signal) => animeApi.getGenres(signal), []);

    function updateParam(key: string, value: string) {
        setParams((prev) => {
            const next = new URLSearchParams(prev);
            if (value) next.set(key, value);
            else next.delete(key);
            if (key !== 'page') next.delete('page'); // changing a filter goes back to page 1
            return next;
        });
    }

    // The search box updates as you type; the URL (and so the request) only updates after a pause
    const urlSearch = params.get('search') ?? '';
    const [searchText, setSearchText] = useState(urlSearch);
    const [prevUrlSearch, setPrevUrlSearch] = useState(urlSearch);

    // When the URL's search value changes from outside the box (e.g. the back button), update the box
    if (urlSearch !== prevUrlSearch) {
        setPrevUrlSearch(urlSearch);
        if (urlSearch !== searchText.trim()) {
            setSearchText(urlSearch);
        }
    }
    const debouncedSearch = useDebounce(searchText.trim(), 400);

    useEffect(() => {
        if (debouncedSearch !== (params.get('search') ?? '')) {
            updateParam('search', debouncedSearch);
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [debouncedSearch]);

    return (
        <>
            <div className="filters">
                <input
                    type="search"
                    placeholder="Search anime…"
                    value={searchText}
                    onChange={(e) => setSearchText(e.target.value)}
                />

                <select value={query.genre ?? ''} onChange={(e) => updateParam('genre', e.target.value)}>
                    <option value="">Any genre</option>
                    {genres?.map((g) => (
                        <option key={g} value={g}>{g}</option>
                    ))}
                </select>

                <select value={query.year ?? ''} onChange={(e) => updateParam('year', e.target.value)}>
                    <option value="">Any year</option>
                    {YEARS.map((y) => (
                        <option key={y} value={y}>{y}</option>
                    ))}
                </select>

                <select value={query.season ?? ''} onChange={(e) => updateParam('season', e.target.value)}>
                    <option value="">Any season</option>
                    {SEASONS.map((s) => (
                        <option key={s} value={s}>{s}</option>
                    ))}
                </select>

                <select value={query.format ?? ''} onChange={(e) => updateParam('format', e.target.value)}>
                    <option value="">Any format</option>
                    {(Object.keys(formatLabels) as AnimeFormat[]).map((f) => (
                        <option key={f} value={f}>{formatLabels[f]}</option>
                    ))}
                </select>

                <select
                    value={query.sort}
                    onChange={(e) => updateParam('sort', e.target.value === 'Popularity' ? '' : e.target.value)}
                >
                    {SORTS.map((s) => (
                        <option key={s.value} value={s.value}>{s.label}</option>
                    ))}
                </select>
            </div>

            {error && <p className="status status--error">Couldn't load anime. Is the API running?</p>}
            {!data && loading && <p className="status">Loading…</p>}
            {data && data.items.length === 0 && <p className="status">No anime match those filters.</p>}

            {data && data.items.length > 0 && (
                <>
                    <div className="anime-grid" style={{ opacity: loading ? 0.5 : 1 }}>
                        {data.items.map((anime) => (
                            <AnimeCard key={anime.id} anime={anime} />
                        ))}
                    </div>
                    <Pagination
                        page={data.page}
                        totalPages={data.totalPages}
                        onPageChange={(p) => {
                            updateParam('page', String(p));
                            window.scrollTo({ top: 0, behavior: 'smooth' });
                        }}
                    />
                </>
            )}
        </>
    );
}