import React from 'react';
import { RoleManagementPage } from './components/RoleManagementPage';

/**
 * RoleManagement component - Main entry point for role management
 * Uses the comprehensive RoleManagementPage component
 */
const RoleManagement: React.FC = () => {
  return <RoleManagementPage defaultTab="assignments" />;
};

export default RoleManagement;
