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
            <div className="px-4 py-5 sm:p-6">
              <div className="flex items-center justify-between">
                <div>
                  <h1 className="text-2xl font-bold text-gray-900">
                    Welcome, {user?.fullName || user?.email}!
                  </h1>
                  <p className="mt-1 text-sm text-gray-500">
                    You are logged in as: {user?.roles?.join(', ') || 'User'}
                  </p>
                </div>
                <button
                  onClick={handleLogout}
                  className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-red-600 hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-red-500"
                >
                  Logout
                </button>
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
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
