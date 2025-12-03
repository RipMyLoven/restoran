import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import { AuthLayout } from './layouts/AuthLayout';
import { MainLayout } from './layouts/MainLayout';
import { LoginPage } from './pages/LoginPage';
import { RegisterPage } from './pages/RegisterPage';
import { DashboardPage } from './pages/DashboardPage';
import { RestaurantsPage } from './pages/RestaurantsPage';
import { TablesPage } from './pages/TablesPage';
import { MenuPage } from './pages/MenuPage';
import { OrdersPage } from './pages/OrdersPage';
import { BillsPage } from './pages/BillsPage';
import { NotificationsPage } from './pages/NotificationsPage';
import { ArchivePage } from './pages/ArchivePage';
import { UsersPage } from './pages/UsersPage';
import './index.css';

function ProtectedRoute({ children }: { children: React.ReactNode }) {
  const { isAuthenticated, isLoading } = useAuth();

  if (isLoading) {
    return <div className="flex items-center justify-center min-h-screen">Loading...</div>;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" />;
  }

  return <>{children}</>;
}

export function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          {/* Public Routes */}
          <Route element={<AuthLayout />}>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
          </Route>

          {/* Protected Routes */}
          <Route
            element={
              <ProtectedRoute>
                <MainLayout />
              </ProtectedRoute>
            }
          >
            <Route path="/" element={<DashboardPage />} />
            <Route path="/restaurants" element={<RestaurantsPage />} />
            <Route path="/tables" element={<TablesPage />} />
            <Route path="/menu" element={<MenuPage />} />
            <Route path="/orders" element={<OrdersPage />} />
            <Route path="/bills" element={<BillsPage />} />
            <Route path="/notifications" element={<NotificationsPage />} />
            <Route path="/archive" element={<ArchivePage />} />
            <Route path="/users" element={<UsersPage />} />
          </Route>
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}

export default App;
