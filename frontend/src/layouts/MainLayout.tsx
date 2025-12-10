import { Outlet, Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { Button } from '../components/Button';

export function MainLayout() {
  const { user, logout } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const handleLogout = async () => {
    await logout();
    navigate('/login');
  };

  const isActive = (path: string) => location.pathname === path;

  const getNavLinks = () => {
    const links = [
      { to: '/', label: 'Dashboard', roles: ['Admin', 'Waiter', 'Cook'] },
      { to: '/restaurants', label: 'Restaurants', roles: ['Admin'] },
      { to: '/tables', label: 'Tables', roles: ['Admin', 'Waiter'] },
      { to: '/menu', label: 'Menu', roles: ['Admin', 'Cook'] },
      { to: '/orders', label: 'Orders', roles: ['Admin', 'Waiter', 'Cook'] },
      { to: '/bills', label: 'Bills', roles: ['Admin', 'Waiter'] },
      //{ to: '/notifications', label: 'Notifications', roles: ['Admin', 'Waiter', 'Cook'] },
      { to: '/archive', label: 'Archive', roles: ['Admin'] },
      { to: '/users', label: 'Users', roles: ['Admin'] },
    ];

    return links.filter(link => link.roles.includes(user?.role || ''));
  };

  const navLinks = getNavLinks();

  return (
    <div className="min-h-screen bg-gray-100">
      <nav className="bg-white shadow-sm">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="flex justify-between h-16">
            <div className="flex">
              <div className="flex-shrink-0 flex items-center">
                <span className="text-xl font-bold text-blue-600">Restoran</span>
              </div>
              <div className="hidden md:ml-6 md:flex md:space-x-4">
                {navLinks.map((link) => (
                  <Link
                    key={link.to}
                    to={link.to}
                    className={`inline-flex items-center px-2 pt-1 border-b-2 text-sm font-medium ${
                      isActive(link.to)
                        ? 'border-blue-500 text-gray-900'
                        : 'border-transparent text-gray-500 hover:border-gray-300 hover:text-gray-700'
                    }`}
                  >
                    {link.label}
                  </Link>
                ))}
              </div>
            </div>
            <div className="flex items-center">
              <span className="text-sm text-gray-700 mr-4">
                {user?.username} ({user?.role})
              </span>
              <Button variant="outline" size="sm" onClick={handleLogout}>
                Logout
              </Button>
            </div>
          </div>
        </div>
        {/* Mobile nav */}
        <div className="md:hidden px-4 pb-3">
          <div className="flex flex-wrap gap-2">
            {navLinks.map((link) => (
              <Link
                key={link.to}
                to={link.to}
                className={`px-3 py-1 rounded-full text-sm ${
                  isActive(link.to)
                    ? 'bg-blue-100 text-blue-700'
                    : 'bg-gray-100 text-gray-600'
                }`}
              >
                {link.label}
              </Link>
            ))}
          </div>
        </div>
      </nav>

      <main className="py-10">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <Outlet />
        </div>
      </main>
    </div>
  );
}
