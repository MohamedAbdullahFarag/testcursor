import React from 'react';
import { useAuth } from '../../auth/hooks/useAuth';
import { useTranslation } from 'react-i18next';

export const Dashboard: React.FC = () => {
  const { user, logout } = useAuth();
  const { t } = useTranslation();

  const handleLogout = async () => {
    try {
      await logout();
    } catch (error) {
      console.error('Logout failed:', error);
    }
  };

  return (
    <div className="min-h-screen bg-gray-50">
      <div className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
        <div className="px-4 py-6 sm:px-0">
          <div className="bg-white overflow-hidden shadow rounded-lg">
            {/* Sidebar */}
            <div data-testid="sidebar" className="hidden md:block w-64 bg-gray-100 p-4">
              <nav className="space-y-2">
                <div className="text-sm font-medium text-gray-900">Navigation</div>
                <a href="#" className="block px-3 py-2 text-sm text-gray-700 hover:bg-gray-200 rounded">Dashboard</a>
                <a href="#" className="block px-3 py-2 text-sm text-gray-700 hover:bg-gray-200 rounded">Profile</a>
                <a href="#" className="block px-3 py-2 text-sm text-gray-700 hover:bg-gray-200 rounded">Settings</a>
              </nav>
            </div>
            
            <div data-testid="main-content" className="px-4 py-5 sm:p-6">
              <div className="flex items-center justify-between">
                <div>
                  <h1 data-testid="dashboard-title" className="text-2xl font-bold text-gray-900">
                    Welcome, {user?.fullName || user?.email}!
                  </h1>
                  <p data-testid="welcome-message" className="mt-1 text-sm text-gray-500">
                    You are logged in as: {user?.roles?.join(', ') || 'User'}
                  </p>
                </div>
                <div data-testid="user-profile" className="text-right">
                  <div data-testid="user-menu" className="relative inline-block text-left">
                    <button
                      className="inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                    >
                      {user?.email || 'User'}
                    </button>
                  </div>
                  <button
                    data-testid="logout-button"
                    onClick={handleLogout}
                    className="ml-2 inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-red-600 hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500"
                  >
                    Logout
                  </button>
                </div>
              </div>
              
              <div className="mt-8">
                <h2 className="text-lg font-medium text-gray-900 mb-4">
                  Dashboard Content
                </h2>
                <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
                  <div className="bg-blue-50 p-4 rounded-lg">
                    <h3 className="text-sm font-medium text-blue-800">Quick Actions</h3>
                    <p className="mt-1 text-sm text-blue-600">
                      Access your most used features
                    </p>
                  </div>
                  <div className="bg-green-50 p-4 rounded-lg">
                    <h3 className="text-sm font-medium text-green-800">Recent Activity</h3>
                    <p className="mt-1 text-sm text-green-600">
                      View your recent actions
                    </p>
                  </div>
                  <div className="bg-purple-50 p-4 rounded-lg">
                    <h3 className="text-sm font-medium text-purple-800">Notifications</h3>
                    <p className="mt-1 text-sm text-purple-600">
                      Check for new updates
                    </p>
                  </div>
                </div>
              </div>
              
              {/* Notifications Section */}
              <div data-testid="notifications" className="mt-6 p-4 bg-yellow-50 rounded-lg">
                <h3 className="text-sm font-medium text-yellow-800 mb-2">System Notifications</h3>
                <p className="text-sm text-yellow-600">No new notifications at this time.</p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
