import { Link, NavLink, Outlet, useNavigate } from 'react-router';
import { useAuth } from '../hooks/useAuth';

export default function Layout() {
    const { user, initializing, logout } = useAuth();
    const navigate = useNavigate();

    async function handleLogout() {
        await logout();
        navigate('/browse');
    }

    return (
        <>
            <header className="navbar">
                <div className="container navbar__inner">
                    <Link to="/" className="navbar__brand">AniTrack</Link>
                    <nav className="navbar__links">
                        <NavLink to="/browse">Browse</NavLink>
                    </nav>

                    <div className="navbar__auth">
                        {initializing ? null : user ? (
                            <>
                                <span className="navbar__user">{user.userName}</span>
                                <button className="btn btn--ghost" onClick={handleLogout}>Log out</button>
                            </>
                        ) : (
                            <>
                                <Link to="/login" className="btn btn--ghost">Log in</Link>
                                <Link to="/register" className="btn btn--primary">Sign up</Link>
                            </>
                        )}
                    </div>
                </div>
            </header>
            <main className="container">
                <Outlet />
            </main>
        </>
    );
}