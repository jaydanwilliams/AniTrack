import type { ReactNode } from 'react';
import { Link, useParams } from 'react-router';
import { animeApi } from '../api/anime';
import { ApiError } from '../api/client';
import { useFetch } from '../hooks/useFetch';
import { cleanDescription, formatDate, formatLabels, statusLabels } from '../utils/format';
import NotFoundPage from './NotFoundPage';

function InfoRow({ label, value }: { label: string; value: ReactNode }) {
    if (value === null || value === undefined || value === '') return null;
    return (
        <>
            <dt>{label}</dt>
            <dd>{value}</dd>
        </>
    );
}

export default function AnimeDetailPage() {
    const { id } = useParams();
    const animeId = Number(id);

    const { data: anime, loading, error } = useFetch(
        (signal) => animeApi.getById(animeId, signal),
        [animeId],
    );

    if (error instanceof ApiError && error.status === 404) return <NotFoundPage />;
    if (error) return <p className="status status--error">Something went wrong loading this anime.</p>;
    if (loading && !anime) return <p className="status">Loading…</p>;
    if (!anime) return null;

    const title = anime.titleEnglish ?? anime.titleRomaji;

    return (
        <article className="detail">
            {anime.bannerImageUrl && (
                <div className="detail__banner" style={{ backgroundImage: `url(${anime.bannerImageUrl})` }} />
            )}

            <div className="detail__header">
                {anime.coverImageUrl && <img className="detail__cover" src={anime.coverImageUrl} alt={title} />}
                <div>
                    <h1>{title}</h1>
                    {anime.titleEnglish && <p className="detail__subtitle">{anime.titleRomaji}</p>}
                    <div className="detail__genres">
                        {anime.genres.map((g) => (
                            <Link key={g} to={`/browse?genre=${encodeURIComponent(g)}`} className="tag">
                                {g}
                            </Link>
                        ))}
                    </div>
                </div>
            </div>

            <div className="detail__body">
                <aside className="detail__info">
                    <dl>
                        <InfoRow label="Format" value={anime.format && formatLabels[anime.format]} />
                        <InfoRow label="Episodes" value={anime.episodes} />
                        <InfoRow label="Episode Duration" value={anime.durationMinutes && `${anime.durationMinutes} mins`} />
                        <InfoRow label="Status" value={anime.status && statusLabels[anime.status]} />
                        <InfoRow label="Season" value={anime.season && `${anime.season} ${anime.seasonYear ?? ''}`} />
                        <InfoRow label="Start Date" value={formatDate(anime.startDate)} />
                        <InfoRow label="End Date" value={formatDate(anime.endDate)} />
                        <InfoRow label="Average Score" value={anime.averageScore !== null ? `${anime.averageScore}%` : null} />
                        <InfoRow label="Popularity" value={anime.popularity?.toLocaleString()} />
                    </dl>
                </aside>

                <section className="detail__description">{cleanDescription(anime.description)}</section>
            </div>
        </article>
    );
}