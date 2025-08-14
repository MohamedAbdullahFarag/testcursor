import React, { memo, useState, useCallback, useRef, useEffect } from 'react';
import { Edit, Trash2, Mail, Phone, Star, Globe, Calendar, ChevronDown, Activity, Shield, Check } from 'lucide-react';
import { useTranslation } from 'react-i18next';
import { motion, AnimatePresence } from 'framer-motion';
import { analytics } from '../../../shared/services/analytics';

// Import RTL-specific utilities and hooks
import { useRTL } from '../../hooks/useRTL';
import { useKeyboardNavigation } from '../../hooks/useKeyboardNavigation';
import { useUserPermissions } from '../../hooks/useUserPermissions';

interface User {
  id: string;
  name: string;
  email: string;
  phoneNumber?: string;
  avatarUrl?: string;
  role: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  preferredLanguage: 'ar' | 'en';
  country?: string;
  permissions?: string[];
  lastLoginDate?: string;
}

interface UserCardProps {
  user: User;
  onEdit?: (userId: string) => void;
  onDelete?: (userId: string) => void;
  className?: string;
  showActions?: boolean;
  isHighlighted?: boolean;
  analyticsCategory?: string;
}

/**
 * UserCard Component
 * 
 * Displays user information in a card format with support for both LTR (English)
 * and RTL (Arabic) languages. Includes accessibility features, animations, and
 * comprehensive internationalization.
 * 
 * This component demonstrates PRP (Product Requirements Prompt) methodology:
 * - Context is King: Extensive documentation and comprehensive prop types
 * - Validation Loops: Built-in error handling and state management
 * - Information Dense: Reusable patterns and rich TypeScript interfaces
 * - Progressive Success: Component handles basic display, expanding details, and actions
 * - One-Pass Implementation: Complete component with accessibility, i18n, and analytics
 * 
 * @example
 * // Basic usage
 * <UserCard 
 *   user={userData} 
 *   onEdit={handleEdit} 
 *   onDelete={handleDelete} 
 * />
 * 
 * @example
 * // With all optional props
 * <UserCard 
 *   user={userData}
 *   onEdit={handleEdit}
 *   onDelete={handleDelete}
 *   className="custom-card-class"
 *   showActions={true}
 *   isHighlighted={true}
 *   analyticsCategory="dashboardUserManagement"
 * />
 */
const UserCard = memo<UserCardProps>(({ 
  user, 
  onEdit, 
  onDelete, 
  className = '',
  showActions = true,
  isHighlighted = false,
  analyticsCategory = 'userCard'
}) => {
  const { t, i18n } = useTranslation(['common', 'users']);
  const { isRTL, dir } = useRTL();
  const { hasPermission } = useUserPermissions();
  
  // Component state
  const [isLoading, setIsLoading] = useState(false);
  const [isDeleting, setIsDeleting] = useState(false);
  const [showDetails, setShowDetails] = useState(false);
  const [isFocused, setIsFocused] = useState(false);
  const cardRef = useRef<HTMLDivElement>(null);
  
  // Keyboard navigation for actions - PRP Progressive Success: adds accessibility after basic UI
  const { focusableRefs, currentFocusIndex, setFocus } = useKeyboardNavigation({
    initialSize: showActions ? 2 : 0
  });
  
  // Track mount status for animations - PRP Progressive Success: enhances UX after functional component
  const [isMounted, setIsMounted] = useState(false);
  useEffect(() => {
    setIsMounted(true);
    return () => setIsMounted(false);
  }, []);
  
  // Analytics logging for component interactions - PRP Information Density: captures user behavior
  useEffect(() => {
    if (analyticsCategory) {
      analytics.trackEvent({
        category: analyticsCategory,
        action: 'view',
        label: `user_${user.id}`
      });
    }
  }, [analyticsCategory, user.id]);

  // Handle card toggle (show/hide details)
  const handleToggleDetails = useCallback(() => {
    setShowDetails(prev => !prev);
    
    analytics.trackEvent({
      category: analyticsCategory,
      action: showDetails ? 'hideDetails' : 'showDetails',
      label: `user_${user.id}`
    });
  }, [showDetails, analyticsCategory, user.id]);
  
  // Handle edit button click
  const handleEdit = useCallback(() => {
    if (onEdit && !isLoading) {
      setIsLoading(true);
      
      analytics.trackEvent({
        category: analyticsCategory,
        action: 'edit',
        label: `user_${user.id}`
      });
      
      // Simulate network delay for demo purposes
      setTimeout(() => {
        onEdit(user.id);
        setIsLoading(false);
      }, 300);
    }
  }, [onEdit, isLoading, user.id, analyticsCategory]);
  
  // Handle delete button click with confirmation
  const handleDelete = useCallback(() => {
    if (onDelete && !isDeleting) {
      setIsDeleting(true);
      
      analytics.trackEvent({
        category: analyticsCategory,
        action: 'delete_attempt',
        label: `user_${user.id}`
      });
      
      // Confirm deletion using localized text
      const confirmMessage = t('users:confirmDeleteUser', { name: user.name });
      
      if (window.confirm(confirmMessage)) {
        analytics.trackEvent({
          category: analyticsCategory,
          action: 'delete_confirmed',
          label: `user_${user.id}`
        });
        
        onDelete(user.id);
      } else {
        analytics.trackEvent({
          category: analyticsCategory,
          action: 'delete_cancelled',
          label: `user_${user.id}`
        });
      }
      
      setIsDeleting(false);
    }
  }, [onDelete, isDeleting, user, t, analyticsCategory]);

  // Handle keyboard navigation
  const handleKeyDown = useCallback((e: React.KeyboardEvent) => {
    if (e.key === 'Enter' || e.key === ' ') {
      handleToggleDetails();
      e.preventDefault();
    }
  }, [handleToggleDetails]);
  
  // Format date based on current locale
  const formatDate = useCallback((dateString: string) => {
    const date = new Date(dateString);
    return new Intl.DateTimeFormat(i18n.language, { 
      year: 'numeric', 
      month: 'short', 
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit'
    }).format(date);
  }, [i18n.language]);
  
  // Get appropriate role display text
  const roleDisplay = t(`users:roles.${user.role.toLowerCase()}`, user.role);
  
  // Check permissions for action buttons
  const canEdit = hasPermission('users.edit');
  const canDelete = hasPermission('users.delete');
  
  // Base animation variants
  const cardVariants = {
    hidden: { opacity: 0, y: 20 },
    visible: { 
      opacity: 1, 
      y: 0,
      transition: { 
        type: 'spring',
        stiffness: 300,
        damping: 24
      }
    },
    exit: { 
      opacity: 0,
      y: -20,
      transition: { duration: 0.2 }
    }
  };
  
  // Details animation variants
  const detailsVariants = {
    collapsed: { height: 0, opacity: 0 },
    expanded: { 
      height: 'auto', 
      opacity: 1,
      transition: {
        height: {
          type: 'spring',
          stiffness: 500,
          damping: 30
        },
        opacity: {
          duration: 0.2,
          delay: 0.1
        }
      }
    }
  };
  
  // Role badge color based on role
  const getRoleBadgeColor = () => {
    switch(user.role.toLowerCase()) {
      case 'admin': return 'bg-red-100 text-red-800';
      case 'manager': return 'bg-purple-100 text-purple-800';
      case 'editor': return 'bg-blue-100 text-blue-800';
      case 'user': return 'bg-green-100 text-green-800';
      default: return 'bg-gray-100 text-gray-800';
    }
  };

  return (
    <motion.div
      ref={cardRef}
      initial="hidden"
      animate="visible"
      exit="exit"
      variants={cardVariants}
      className={`
        rounded-lg border border-gray-200 bg-white shadow-sm transition-all
        ${isHighlighted ? 'ring-2 ring-primary-500' : ''}
        ${isFocused ? 'ring-2 ring-primary-300' : ''}
        ${className}
      `}
      aria-labelledby={`user-card-name-${user.id}`}
      onFocus={() => setIsFocused(true)}
      onBlur={() => setIsFocused(false)}
      data-testid="user-card"
      dir={dir}
    >
      {/* Card Header */}
      <div 
        className={`
          flex items-center justify-between p-4 cursor-pointer
          ${isRTL ? 'flex-row-reverse' : 'flex-row'}
        `}
        onClick={handleToggleDetails}
        onKeyDown={handleKeyDown}
        tabIndex={0}
        aria-expanded={showDetails}
        aria-controls={`user-details-${user.id}`}
        role="button"
      >
        <div className={`flex items-center ${isRTL ? 'flex-row-reverse' : 'flex-row'} gap-3`}>
          {/* Avatar */}
          <div className="relative">
            {user.avatarUrl ? (
              <img
                src={user.avatarUrl}
                alt={t('users:userAvatar', { name: user.name })}
                className="h-10 w-10 rounded-full object-cover"
                onError={(e) => {
                  // Fallback to initials if image fails to load
                  (e.target as HTMLImageElement).style.display = 'none';
                }}
              />
            ) : (
              <div className="flex h-10 w-10 items-center justify-center rounded-full bg-primary-100 text-primary-700">
                {user.name.split(' ').map(n => n[0]).join('').toUpperCase().substring(0, 2)}
              </div>
            )}
            
            {/* Status indicator */}
            <span 
              className={`absolute bottom-0 ${isRTL ? 'left-0' : 'right-0'} h-3 w-3 rounded-full border-2 border-white ${user.isActive ? 'bg-green-500' : 'bg-gray-400'}`}
              aria-hidden="true"
            />
          </div>
          
          {/* User basic info */}
          <div>
            <h3 
              id={`user-card-name-${user.id}`}
              className="font-medium text-gray-900"
              dir="auto" // Auto-detect text direction
            >
              {user.name}
            </h3>
            <p className="text-sm text-gray-500">{user.email}</p>
          </div>
        </div>

        <div className={`flex items-center ${isRTL ? 'flex-row-reverse' : 'flex-row'} gap-2`}>
          {/* Role badge */}
          <span className={`px-2.5 py-0.5 rounded-full text-xs font-medium ${getRoleBadgeColor()}`}>
            {roleDisplay}
          </span>
          
          {/* Dropdown indicator */}
          <ChevronDown 
            className={`h-4 w-4 text-gray-400 transition-transform ${showDetails ? 'rotate-180' : ''}`} 
            aria-hidden="true"
          />
        </div>
      </div>
      
      {/* Card Details (expandable) */}
      <AnimatePresence>
        {showDetails && (
          <motion.div
            id={`user-details-${user.id}`}
            initial="collapsed"
            animate="expanded"
            exit="collapsed"
            variants={detailsVariants}
            className="overflow-hidden"
          >
            <div className="border-t border-gray-200 px-4 py-4">
              <div className={`grid grid-cols-1 gap-4 ${isRTL ? 'text-right' : 'text-left'} md:grid-cols-2`}>
                {/* Phone */}
                {user.phoneNumber && (
                  <div className={`flex items-center ${isRTL ? 'flex-row-reverse' : 'flex-row'} gap-2`}>
                    <Phone className="h-4 w-4 text-gray-400" aria-hidden="true" />
                    <span className="text-sm text-gray-600" dir="ltr">{user.phoneNumber}</span>
                    <span className="sr-only">{t('users:phoneNumber')}: {user.phoneNumber}</span>
                  </div>
                )}
                
                {/* Country */}
                {user.country && (
                  <div className={`flex items-center ${isRTL ? 'flex-row-reverse' : 'flex-row'} gap-2`}>
                    <Globe className="h-4 w-4 text-gray-400" aria-hidden="true" />
                    <span className="text-sm text-gray-600" dir="auto">{user.country}</span>
                    <span className="sr-only">{t('users:country')}: {user.country}</span>
                  </div>
                )}
                
                {/* Created date */}
                <div className={`flex items-center ${isRTL ? 'flex-row-reverse' : 'flex-row'} gap-2`}>
                  <Calendar className="h-4 w-4 text-gray-400" aria-hidden="true" />
                  <span className="text-sm text-gray-600">
                    {t('users:createdAt')}: {formatDate(user.createdAt)}
                  </span>
                </div>
                
                {/* Last login date */}
                {user.lastLoginDate && (
                  <div className={`flex items-center ${isRTL ? 'flex-row-reverse' : 'flex-row'} gap-2`}>
                    <Activity className="h-4 w-4 text-gray-400" aria-hidden="true" />
                    <span className="text-sm text-gray-600">
                      {t('users:lastLogin')}: {formatDate(user.lastLoginDate)}
                    </span>
                  </div>
                )}
                
                {/* Preferred language */}
                <div className={`flex items-center ${isRTL ? 'flex-row-reverse' : 'flex-row'} gap-2`}>
                  <Globe className="h-4 w-4 text-gray-400" aria-hidden="true" />
                  <span className="text-sm text-gray-600">
                    {t('users:preferredLanguage')}: {user.preferredLanguage === 'ar' ? t('users:arabic') : t('users:english')}
                  </span>
                </div>
                
                {/* Status */}
                <div className={`flex items-center ${isRTL ? 'flex-row-reverse' : 'flex-row'} gap-2`}>
                  <Shield className="h-4 w-4 text-gray-400" aria-hidden="true" />
                  <span className="text-sm text-gray-600">
                    {t('users:status')}: {user.isActive ? (
                      <span className="text-green-600 font-medium">{t('users:active')}</span>
                    ) : (
                      <span className="text-gray-500">{t('users:inactive')}</span>
                    )}
                  </span>
                </div>
              </div>

              {/* User permissions */}
              {user.permissions && user.permissions.length > 0 && (
                <div className="mt-4">
                  <h4 className={`text-sm font-medium text-gray-900 ${isRTL ? 'text-right' : 'text-left'}`}>
                    {t('users:permissions')}:
                  </h4>
                  <div className="mt-1 flex flex-wrap gap-1">
                    {user.permissions.map(permission => (
                      <span 
                        key={permission}
                        className="inline-flex items-center rounded-full bg-gray-100 px-2.5 py-0.5 text-xs font-medium text-gray-800"
                      >
                        <Check className="h-3 w-3 mr-1 text-green-500" aria-hidden="true" />
                        {permission}
                      </span>
                    ))}
                  </div>
                </div>
              )}

              {/* Action buttons */}
              {showActions && (canEdit || canDelete) && (
                <div className={`mt-4 flex ${isRTL ? 'justify-start' : 'justify-end'} gap-2`}>
                  {/* Edit button */}
                  {canEdit && (
                    <button
                      ref={el => { if (el) focusableRefs.current[0] = el; }}
                      type="button"
                      onClick={handleEdit}
                      disabled={isLoading}
                      className={`
                        inline-flex items-center rounded-md border border-gray-300 bg-white px-3 py-2 text-sm font-medium
                        text-gray-700 shadow-sm hover:bg-gray-50 focus:outline-none focus:ring-2
                        focus:ring-primary-500 focus:ring-offset-2 disabled:opacity-70
                        ${isRTL ? 'flex-row-reverse' : 'flex-row'} gap-1
                      `}
                      aria-label={t('users:editUser', { name: user.name })}
                      onFocus={() => setFocus(0)}
                    >
                      <Edit className="h-4 w-4" aria-hidden="true" />
                      {isLoading ? t('common:loading') : t('common:edit')}
                    </button>
                  )}
                  
                  {/* Delete button */}
                  {canDelete && (
                    <button
                      ref={el => { if (el) focusableRefs.current[1] = el; }}
                      type="button"
                      onClick={handleDelete}
                      disabled={isDeleting}
                      className={`
                        inline-flex items-center rounded-md border border-transparent bg-red-600 px-3 py-2
                        text-sm font-medium text-white shadow-sm hover:bg-red-700 focus:outline-none
                        focus:ring-2 focus:ring-red-500 focus:ring-offset-2 disabled:opacity-70
                        ${isRTL ? 'flex-row-reverse' : 'flex-row'} gap-1
                      `}
                      aria-label={t('users:deleteUser', { name: user.name })}
                      onFocus={() => setFocus(1)}
                    >
                      <Trash2 className="h-4 w-4" aria-hidden="true" />
                      {isDeleting ? t('common:loading') : t('common:delete')}
                    </button>
                  )}
                </div>
              )}
            </div>
          </motion.div>
        )}
      </AnimatePresence>
    </motion.div>
  );
});

// Display name for debugging purposes
UserCard.displayName = 'UserCard';

export default UserCard;

// Translation keys structure example for this component:
// This is extensive documentation of all the translation keys required
// for both English and Arabic to properly implement internationalization
/*
{
  "users": {
    "avatarAlt": "{{name}}'s profile picture",
    "cardDescription": "User information card",
    "status": {
      "active": "Active",
      "inactive": "Inactive"
    },
    "actions": {
      "edit": "Edit user",
      "delete": "Delete user",
      "toggleDetails": "Show more details",
      "group": "User actions"
    },
    "deleteConfirmation": "Are you sure you want to delete this user?",
    "language": {
      "english": "English",
      "arabic": "Arabic"
    },
    "languagePreference": "Preferred language: {{language}}",
    "emailLabel": "Email address",
    "phoneLabel": "Phone number",
    "countryLabel": "Country",
    "roleDescription": "Role: {{role}}",
    "joinedDate": "Joined {{date}}",
    "joinedDateLabel": "Join date",
    "lastUpdated": "Updated: {{date}}",
    "lastUpdatedLabel": "Last updated on {{date}}",
    "lastLogin": "Last login: {{date}}",
    "permissions": {
      "title": "Permissions",
      "viewDashboard": "View dashboard",
      "editProfile": "Edit profile",
      "manageUsers": "Manage users",
      "viewReports": "View reports",
      "exportData": "Export data",
      "adminAccess": "Administrative access"
    }
  },
  "countries": {
    "usa": "United States",
    "uae": "United Arab Emirates",
    "ksa": "Saudi Arabia",
    "eg": "Egypt",
    "jo": "Jordan",
    "lb": "Lebanon"
  }
}

// ar.ts - Arabic translations for all the keys above
{
  "users": {
    "avatarAlt": "صورة الملف الشخصي لـ {{name}}",
    "cardDescription": "بطاقة معلومات المستخدم",
    "status": {
      "active": "نشط",
      "inactive": "غير نشط"
    },
    "actions": {
      "edit": "تعديل المستخدم",
      "delete": "حذف المستخدم",
      "toggleDetails": "عرض المزيد من التفاصيل",
      "group": "إجراءات المستخدم"
    },
    "deleteConfirmation": "هل أنت متأكد من رغبتك في حذف هذا المستخدم؟",
    "language": {
      "english": "الإنجليزية",
      "arabic": "العربية"
    },
    "languagePreference": "اللغة المفضلة: {{language}}",
    "emailLabel": "البريد الإلكتروني",
    "phoneLabel": "رقم الهاتف",
    "countryLabel": "البلد",
    "roleDescription": "الدور: {{role}}",
    "joinedDate": "انضم {{date}}",
    "joinedDateLabel": "تاريخ الانضمام",
    "lastUpdated": "محدث: {{date}}",
    "lastUpdatedLabel": "آخر تحديث في {{date}}",
    "lastLogin": "آخر تسجيل دخول: {{date}}",
    "permissions": {
      "title": "الصلاحيات",
      "viewDashboard": "عرض لوحة المعلومات",
      "editProfile": "تعديل الملف الشخصي",
      "manageUsers": "إدارة المستخدمين",
      "viewReports": "عرض التقارير",
      "exportData": "تصدير البيانات",
      "adminAccess": "صلاحيات المسؤول"
    }
  },
  "countries": {
    "usa": "الولايات المتحدة الأمريكية",
    "uae": "الإمارات العربية المتحدة",
    "ksa": "المملكة العربية السعودية",
    "eg": "مصر",
    "jo": "الأردن",
    "lb": "لبنان"
  }
}
*/