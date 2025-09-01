/**
 * System Status Banner Component
 * Shows when APIs are experiencing issues
 */

import React from 'react';
import { API_FEATURE_FLAGS } from '../config/apiFeatureFlags';

interface SystemStatusBannerProps {
  className?: string;
}

export const SystemStatusBanner: React.FC<SystemStatusBannerProps> = ({ className = '' }) => {
  // Show banner only if APIs are disabled and we're in dev mode
  const showBanner = (!API_FEATURE_FLAGS.AUDIT_LOGS_ENABLED || !API_FEATURE_FLAGS.MEDIA_SEARCH_ENABLED) && API_FEATURE_FLAGS.DEV_MODE_ENABLED;

  if (!showBanner) return null;

  const disabledServices: string[] = [];
  if (!API_FEATURE_FLAGS.AUDIT_LOGS_ENABLED) disabledServices.push('Audit Logs');
  if (!API_FEATURE_FLAGS.MEDIA_SEARCH_ENABLED) disabledServices.push('Media Search');

  return (
    <div className={`bg-yellow-50 border-l-4 border-yellow-400 p-4 mb-4 ${className}`}>
      <div className="flex">
        <div className="flex-shrink-0">
          <svg className="h-5 w-5 text-yellow-400" viewBox="0 0 20 20" fill="currentColor">
            <path fillRule="evenodd" d="M8.257 3.099c.765-1.36 2.722-1.36 3.486 0l5.58 9.92c.75 1.334-.213 2.98-1.742 2.98H4.42c-1.53 0-2.493-1.646-1.743-2.98l5.58-9.92zM11 13a1 1 0 11-2 0 1 1 0 012 0zm-1-8a1 1 0 00-1 1v3a1 1 0 002 0V6a1 1 0 00-1-1z" clipRule="evenodd" />
          </svg>
        </div>
        <div className="ml-3">
          <p className="text-sm text-yellow-700">
            <span className="font-medium">Development Notice:</span>{' '}
            Some services are temporarily unavailable: {disabledServices.join(', ')}. 
            Using mock data for testing.
          </p>
        </div>
      </div>
    </div>
  );
};
