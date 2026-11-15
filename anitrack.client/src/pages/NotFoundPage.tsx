import { Link } from 'react-router';

export default function NotFoundPage() {
    return (
        <div className="status">
            <h1>Page not found</h1>
            <p>
                <Link to="/browse" className="tag">Back to browse</Link>
            </p>
        </div>
    );
}