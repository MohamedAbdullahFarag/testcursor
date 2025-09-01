import React from 'react';
import { useNavigate } from 'react-router-dom';

interface ComingSoonPageProps {
  featureName: string;
  description?: string;
}

const ComingSoonPage: React.FC<ComingSoonPageProps> = ({ 
  featureName, 
  description = "This feature is currently under development and will be available soon." 
}) => {
  const navigate = useNavigate();

  return (
    <div className="flex flex-col items-center justify-center min-h-[60vh] text-center px-4">
      <div className="max-w-md mx-auto">
        {/* Coming Soon Icon */}
        <div className="mb-6">
          <div className="w-24 h-24 mx-auto bg-blue-100 rounded-full flex items-center justify-center">
            <svg 
              className="w-12 h-12 text-blue-600" 
              fill="none" 
              stroke="currentColor" 
              viewBox="0 0 24 24"
            >
              <path 
                strokeLinecap="round" 
                strokeLinejoin="round" 
                strokeWidth={2} 
                d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" 
              />
            </svg>
          </div>
        </div>

        {/* Title */}
        <h1 className="text-3xl font-bold text-gray-900 mb-4">
          {featureName}
        </h1>
        
        <div className="mb-6">
          <span className="inline-block px-3 py-1 text-sm font-medium text-blue-800 bg-blue-100 rounded-full">
            Coming Soon
          </span>
        </div>

        {/* Description */}
        <p className="text-gray-600 mb-8 leading-relaxed">
          {description}
        </p>

        {/* Action Buttons */}
        <div className="space-y-3 sm:space-y-0 sm:space-x-3 sm:flex sm:justify-center">
          <button
            onClick={() => navigate('/dashboard')}
            className="w-full sm:w-auto px-6 py-3 bg-blue-600 text-white font-medium rounded-lg hover:bg-blue-700 transition-colors"
          >
            Back to Dashboard
          </button>
          
          <button
            onClick={() => navigate(-1)}
            className="w-full sm:w-auto px-6 py-3 bg-gray-100 text-gray-700 font-medium rounded-lg hover:bg-gray-200 transition-colors"
          >
            Go Back
          </button>
        </div>

        {/* Additional Info */}
        <div className="mt-8 p-4 bg-gray-50 rounded-lg">
          <p className="text-sm text-gray-500">
            Want to be notified when this feature is ready?{' '}
            <a href="#" className="text-blue-600 hover:text-blue-700 font-medium">
              Contact support
            </a>
          </p>
        </div>
      </div>
    </div>
  );
};

export default ComingSoonPage;
