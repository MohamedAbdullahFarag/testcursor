/**
 * RoleManagementPage - Main page component for role management
 * Provides comprehensive role management functionality with proper authorization
 */

import React, { useState } from 'react';
import { useTranslation } from 'react-i18next';
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  Alert,
  AlertDescription,
  Tabs,
  TabsContent,
  TabsList,
  TabsTrigger,
} from 'mada-design-system';
import { Shield, Users, Settings, UserCheck } from 'lucide-react';
import { RoleManagementView } from './RoleManagementView';
import { UserRoleAssignmentComponent } from './UserRoleAssignmentComponent';
import { useRole } from '@/modules/auth/hooks/useRole';

interface RoleManagementPageProps {
  /** Default active tab */
  defaultTab?: 'roles' | 'assignments' | 'permissions';
}

/**
 * RoleManagementPage component - Main orchestrator for all role management operations
 * 
 * Features:
 * - Tab-based navigation between different role management views
 * - Role-based access control
 * - Responsive design with proper error handling
 * - Internationalization support
 * 
 * @param props - RoleManagementPageProps
 */
export const RoleManagementPage: React.FC<RoleManagementPageProps> = ({
  defaultTab = 'roles',
}) => {
  const { t } = useTranslation('roleManagement');
  const { userRoles, hasRole, isSystemAdmin } = useRole();
  const [activeTab, setActiveTab] = useState(defaultTab);

  // Authorization check
  const canManageRoles = isSystemAdmin || hasRole(['exam-manager']);
  const canViewRoles = isSystemAdmin || hasRole(['exam-manager', 'reviewer']);
  const canAssignRoles = isSystemAdmin || hasRole(['exam-manager']);

  // If user doesn't have any role management permissions
  if (!canViewRoles && !canAssignRoles) {
    return (
      <div className="container mx-auto px-4 py-8">
        <Card>
          <CardContent className="flex items-center justify-center p-8">
            <div className="text-center space-y-4">
              <Shield className="mx-auto h-12 w-12 text-gray-400" />
              <h2 className="text-lg font-medium text-gray-900">
                {t('accessDenied')}
              </h2>
              <p className="text-gray-600">
                {t('accessDeniedMessage')}
              </p>
            </div>
          </CardContent>
        </Card>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8 space-y-6">
      {/* Page Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-3xl font-bold text-gray-900" data-testid="role-management-title">
            {t('roleManagement')}
          </h1>
          <p className="text-gray-600 mt-2">
            {t('roleManagementDescription')}
          </p>
        </div>
        
        <div className="flex items-center space-x-2">
          <Badge variant="secondary" className="flex items-center space-x-1">
            <Users className="h-4 w-4" />
            <span>{userRoles.join(', ') || 'No roles'}</span>
          </Badge>
        </div>
      </div>

      {/* Tab Navigation */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center space-x-2">
            <Settings className="h-5 w-5" />
            <span>{t('roleManagementTabs')}</span>
          </CardTitle>
        </CardHeader>
        <CardContent>
          <Tabs value={activeTab} onValueChange={(value) => setActiveTab(value as any)}>
            <TabsList className="grid w-full grid-cols-2">
              {canViewRoles && (
                <TabsTrigger value="roles" className="flex items-center space-x-2">
                  <Shield className="h-4 w-4" />
                  <span>{t('rolesTab')}</span>
                </TabsTrigger>
              )}
              
              {canAssignRoles && (
                <TabsTrigger value="assignments" className="flex items-center space-x-2">
                  <UserCheck className="h-4 w-4" />
                  <span>{t('assignmentsTab')}</span>
                </TabsTrigger>
              )}
            </TabsList>

            {/* Roles Management Tab */}
            {canViewRoles && (
              <TabsContent value="roles" className="space-y-4">
                <div className="space-y-4">
                  <div className="flex items-center justify-between">
                    <h3 className="text-lg font-medium">
                      {t('rolesManagement')}
                    </h3>
                    {!canManageRoles && (
                      <Alert>
                        <AlertDescription>
                          {t('readOnlyMode')}
                        </AlertDescription>
                      </Alert>
                    )}
                  </div>
                  
                  <RoleManagementView 
                    showHeader={false}
                    className="border-none shadow-none"
                  />
                </div>
              </TabsContent>
            )}

            {/* User Role Assignments Tab */}
            {canAssignRoles && (
              <TabsContent value="assignments" className="space-y-4">
                <div className="space-y-4">
                  <h3 className="text-lg font-medium">
                    {t('userRoleAssignments')}
                  </h3>
                  
                  <UserRoleAssignmentComponent 
                    readOnly={!canAssignRoles}
                  />
                </div>
              </TabsContent>
            )}
          </Tabs>
        </CardContent>
      </Card>

      {/* Help Section */}
      <Card>
        <CardHeader>
          <CardTitle>{t('helpAndGuidance')}</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="grid md:grid-cols-2 gap-4">
            <div className="space-y-2">
              <h4 className="font-medium">{t('systemRoles')}</h4>
              <p className="text-sm text-gray-600">
                {t('systemRolesDescription')}
              </p>
            </div>
            
            <div className="space-y-2">
              <h4 className="font-medium">{t('customRoles')}</h4>
              <p className="text-sm text-gray-600">
                {t('customRolesDescription')}
              </p>
            </div>
            
            <div className="space-y-2">
              <h4 className="font-medium">{t('roleAssignments')}</h4>
              <p className="text-sm text-gray-600">
                {t('roleAssignmentsDescription')}
              </p>
            </div>
            
            <div className="space-y-2">
              <h4 className="font-medium">{t('permissions')}</h4>
              <p className="text-sm text-gray-600">
                {t('permissionsDescription')}
              </p>
            </div>
          </div>
        </CardContent>
      </Card>
    </div>
  );
};

// Add Badge component if not imported
const Badge: React.FC<{ variant?: string; className?: string; children: React.ReactNode }> = ({ 
  className = '', 
  children 
}) => (
  <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-gray-100 text-gray-800 ${className}`}>
    {children}
  </span>
);

export default RoleManagementPage;
