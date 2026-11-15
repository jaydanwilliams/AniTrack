import { Link } from 'react-router';
import type { AnimeSummary } from '../types/anime';
import { formatLabels } from '../utils/format';

export default function AnimeCard({ anime }: { anime: AnimeSummary }) {
    const meta = [
        anime.format && formatLabels[anime.format],
        anime.seasonYear,
        anime.episodes && `${anime.episodes} eps`,
    ]
        .filter(Boolean)
        .join(' · ');

    return (
        <Link to={`/anime/${anime.id}`} className="anime-card">
            <div className="anime-card__cover">
                {anime.coverImageUrl ? (
                    <img src={anime.coverImageUrl} alt={anime.title} loading="lazy" />
                ) : (
                    <div className="anime-card__placeholder">No image</div>
                )}
                {anime.averageScore !== null && (
                    <span className="anime-card__score">{anime.averageScore}%</span>
                )}
            </div>
            <h3 className="anime-card__title">{anime.title}</h3>
            <p className="anime-card__meta">{meta}</p>
        </Link>
    );
}