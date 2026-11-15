import { Link, NavLink, Outlet } from 'react-router';

export default function Layout() {
    return (
        <>
            <header className="navbar">
                <div className="container navbar__inner">
                    <Link to="/" className="navbar__brand">AniTrack</Link>
                    <nav className="navbar__links">
                        <NavLink to="/browse">Browse</NavLink>
                    </nav>
                </div>
            </header>
            <main className="container">
                <Outlet />
            </main>
        </>
    );
}