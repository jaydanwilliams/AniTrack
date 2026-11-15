import { Navigate, Route, Routes } from 'react-router';
import Layout from './components/Layout';
import AnimeDetailPage from './pages/AnimeDetailPage';
import BrowsePage from './pages/BrowsePage';
import NotFoundPage from './pages/NotFoundPage';

export default function App() {
    return (
        <Routes>
            <Route element={<Layout />}>
                <Route index element={<Navigate to="/browse" replace />} />
                <Route path="browse" element={<BrowsePage />} />
                <Route path="anime/:id" element={<AnimeDetailPage />} />
                <Route path="*" element={<NotFoundPage />} />
            </Route>
        </Routes>
    );
}