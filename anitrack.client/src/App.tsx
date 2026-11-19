import { Navigate, Route, Routes } from 'react-router';
import Layout from './components/Layout';
import AnimeDetailPage from './pages/AnimeDetailPage';
import BrowsePage from './pages/BrowsePage';
import NotFoundPage from './pages/NotFoundPage';
import LoginPage from './pages/LoginPage';
import RegisterPage from './pages/RegisterPage';

export default function App() {
    return (
        <Routes>
            <Route element={<Layout />}>
                <Route index element={<Navigate to="/browse" replace />} />
                <Route path="browse" element={<BrowsePage />} />
                <Route path="anime/:id" element={<AnimeDetailPage />} />
                <Route path="login" element={<LoginPage />} />
                <Route path="register" element={<RegisterPage />} />
                <Route path="*" element={<NotFoundPage />} />
            </Route>
        </Routes>
    );
}