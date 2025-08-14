import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import {
  Card,
  CardContent,
  CardHeader,
  CardTitle,
  Button,
  Input,
  Label,
  Badge,
  Alert,
  AlertDescription,
  Switch,
} from 'mada-design-system';
import { User, CreateUserRequest, UpdateUserRequest } from '../models/user.types';

// ❌ DON'T: Mix form state with business logic
// ❌ DON'T: Create components without proper typing
// ❌ DON'T: Forget to handle loading and error states
// ❌ DON'T: Skip input validation
// ❌ DON'T: Ignore accessibility requirements

export interface Role {
  id: number;
  name: string;
  description?: string;
}

interface UserFormProps {
  /** User to edit (null for create mode) */
  user?: User | null;
  /** Available roles for assignment */
  availableRoles: Role[];
  /** Loading state */
  isLoading?: boolean;
  /** Form submission callback */
  onSubmit: (data: CreateUserRequest | UpdateUserRequest) => Promise<void>;
  /** Cancel callback */
  onCancel: () => void;
  /** Form mode */
  mode: 'create' | 'edit';
}

interface FormData {
  username: string;
  email: string;
  firstName: string;
  lastName: string;
  password: string;
  phoneNumber: string;
  preferredLanguage: string;
  isActive: boolean;
  roles: string[];
}

interface FormErrors {
  username?: string;
  email?: string;
  firstName?: string;
  lastName?: string;
  password?: string;
  phoneNumber?: string;
  roles?: string;
  general?: string;
}

/**
 * UserForm component for creating and editing users
 * 
 * Features:
 * - Create and edit modes
 * - Form validation with real-time feedback
 * - Role selection with multi-select support
 * - Internationalization support
 * - Loading and error states
 * - Accessibility features
 * 
 * @param props - UserFormProps
 */
export const UserForm: React.FC<UserFormProps> = ({
  user,
  availableRoles,
  isLoading = false,
  onSubmit,
  onCancel,
  mode,
}) => {
  const { t, i18n } = useTranslation('userManagement');
  const isRtl = i18n.language === 'ar';

  // Form state
  const [formData, setFormData] = useState<FormData>({
    username: user?.username || '',
    email: user?.email || '',
    firstName: user?.firstName || '',
    lastName: user?.lastName || '',
    password: '', // Always empty for security
    phoneNumber: user?.phoneNumber || '',
    preferredLanguage: user?.preferredLanguage || 'en',
    isActive: user?.isActive ?? true,
    roles: user?.roles || [],
  });

  const [errors, setErrors] = useState<FormErrors>({});
  const [isSubmitting, setIsSubmitting] = useState(false);

  // Update form when user prop changes
  useEffect(() => {
    if (user) {
      setFormData({
        username: user.username || '',
        email: user.email || '',
        firstName: user.firstName || '',
        lastName: user.lastName || '',
        password: '', // Always empty for security
        phoneNumber: user.phoneNumber || '',
        preferredLanguage: user.preferredLanguage || 'en',
        isActive: user.isActive ?? true,
        roles: user.roles || [],
      });
    }
  }, [user]);

  /**
   * Validates form data
   * @param data - Form data to validate
   * @returns Validation errors
   */
  const validateForm = (data: FormData): FormErrors => {
    const newErrors: FormErrors = {};

    // Username validation
    if (!data.username.trim()) {
      newErrors.username = t('form.errors.usernameRequired');
    } else if (data.username.trim().length < 3) {
      newErrors.username = t('form.errors.usernameTooShort');
    }

    // Email validation
    if (!data.email.trim()) {
      newErrors.email = t('form.errors.emailRequired');
    } else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(data.email)) {
      newErrors.email = t('form.errors.emailInvalid');
    }

    // First name validation
    if (!data.firstName.trim()) {
      newErrors.firstName = t('form.errors.firstNameRequired');
    } else if (data.firstName.trim().length < 2) {
      newErrors.firstName = t('form.errors.firstNameTooShort');
    }

    // Last name validation
    if (!data.lastName.trim()) {
      newErrors.lastName = t('form.errors.lastNameRequired');
    } else if (data.lastName.trim().length < 2) {
      newErrors.lastName = t('form.errors.lastNameTooShort');
    }

    // Password validation (only for create mode)
    if (mode === 'create') {
      if (!data.password) {
        newErrors.password = t('form.errors.passwordRequired');
      } else if (data.password.length < 8) {
        newErrors.password = t('form.errors.passwordTooShort');
      }
    }

    // Phone validation (optional but must be valid if provided)
    if (data.phoneNumber.trim() && !/^\+?[\d\s\-\(\)]+$/.test(data.phoneNumber)) {
      newErrors.phoneNumber = t('form.errors.phoneInvalid');
    }

    // Role validation
    if (data.roles.length === 0) {
      newErrors.roles = t('form.errors.roleRequired');
    }

    return newErrors;
  };

  /**
   * Handles input field changes
   */
  const handleInputChange = (field: keyof FormData, value: string | boolean | string[]) => {
    setFormData(prev => ({
      ...prev,
      [field]: value,
    }));

    // Clear field error when user starts typing
    if (errors[field as keyof FormErrors]) {
      setErrors(prev => ({
        ...prev,
        [field]: undefined,
      }));
    }
  };

  /**
   * Handles role selection
   */
  const handleRoleToggle = (roleName: string) => {
    const newRoles = formData.roles.includes(roleName)
      ? formData.roles.filter(name => name !== roleName)
      : [...formData.roles, roleName];
    
    handleInputChange('roles', newRoles);
  };

  /**
   * Handles form submission
   */
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Validate form
    const validationErrors = validateForm(formData);
    if (Object.keys(validationErrors).length > 0) {
      setErrors(validationErrors);
      return;
    }

    setIsSubmitting(true);
    setErrors({});

    try {
      // Prepare data for submission
      const submitData = mode === 'create' 
        ? {
            username: formData.username.trim(),
            email: formData.email.trim(),
            firstName: formData.firstName.trim(),
            lastName: formData.lastName.trim(),
            password: formData.password,
            phoneNumber: formData.phoneNumber.trim() || undefined,
            preferredLanguage: formData.preferredLanguage,
            roles: formData.roles,
            isActive: formData.isActive,
          } as CreateUserRequest
        : {
            id: user!.id,
            username: formData.username.trim(),
            email: formData.email.trim(),
            firstName: formData.firstName.trim(),
            lastName: formData.lastName.trim(),
            phoneNumber: formData.phoneNumber.trim() || undefined,
            preferredLanguage: formData.preferredLanguage,
            roles: formData.roles,
            isActive: formData.isActive,
          } as UpdateUserRequest;

      await onSubmit(submitData);
    } catch (error) {
      setErrors({
        general: error instanceof Error ? error.message : t('form.errors.submitFailed'),
      });
    } finally {
      setIsSubmitting(false);
    }
  };

  /**
   * Gets the selected roles for display
   */
  const getSelectedRoles = () => {
    return availableRoles.filter(role => formData.roles.includes(role.name));
  };

  return (
    <Card className="w-full max-w-2xl mx-auto">
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          <span>👤</span>
          {mode === 'create' ? t('form.createTitle') : t('form.editTitle')}
        </CardTitle>
      </CardHeader>
      
      <CardContent>
        <form onSubmit={handleSubmit} className="space-y-6">
          {/* General Error */}
          {errors.general && (
            <Alert variant="default">
              <span>⚠️</span>
              <AlertDescription>{errors.general}</AlertDescription>
            </Alert>
          )}

          {/* Basic Information */}
          <div className="space-y-4">
            <h3 className="text-lg font-semibold">{t('form.sections.basicInfo')}</h3>
            
            {/* Username */}
            <div className="space-y-2">
              <Label htmlFor="username" className="required">
                {t('form.fields.username')}
              </Label>
              <Input
                id="username"
                value={formData.username}
                onChange={(e) => handleInputChange('username', e.target.value)}
                placeholder={t('form.placeholders.username')}
                disabled={isLoading || isSubmitting}
                className={errors.username ? 'border-red-500' : ''}
                dir={isRtl ? 'rtl' : 'ltr'}
              />
              {errors.username && (
                <p className="text-sm text-red-600">{errors.username}</p>
              )}
            </div>

            {/* Email */}
            <div className="space-y-2">
              <Label htmlFor="email" className="required">
                {t('form.fields.email')}
              </Label>
              <Input
                id="email"
                type="email"
                value={formData.email}
                onChange={(e) => handleInputChange('email', e.target.value)}
                placeholder={t('form.placeholders.email')}
                disabled={isLoading || isSubmitting}
                className={errors.email ? 'border-red-500' : ''}
                dir={isRtl ? 'rtl' : 'ltr'}
              />
              {errors.email && (
                <p className="text-sm text-red-600">{errors.email}</p>
              )}
            </div>

            {/* Password (only for create mode) */}
            {mode === 'create' && (
              <div className="space-y-2">
                <Label htmlFor="password" className="required">
                  {t('form.fields.password')}
                </Label>
                <Input
                  id="password"
                  type="password"
                  value={formData.password}
                  onChange={(e) => handleInputChange('password', e.target.value)}
                  placeholder={t('form.placeholders.password')}
                  disabled={isLoading || isSubmitting}
                  className={errors.password ? 'border-red-500' : ''}
                />
                {errors.password && (
                  <p className="text-sm text-red-600">{errors.password}</p>
                )}
              </div>
            )}

            {/* Name Fields */}
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {/* First Name */}
              <div className="space-y-2">
                <Label htmlFor="firstName" className="required">
                  {t('form.fields.firstName')}
                </Label>
                <Input
                  id="firstName"
                  value={formData.firstName}
                  onChange={(e) => handleInputChange('firstName', e.target.value)}
                  placeholder={t('form.placeholders.firstName')}
                  disabled={isLoading || isSubmitting}
                  className={errors.firstName ? 'border-red-500' : ''}
                  dir={isRtl ? 'rtl' : 'ltr'}
                />
                {errors.firstName && (
                  <p className="text-sm text-red-600">{errors.firstName}</p>
                )}
              </div>

              {/* Last Name */}
              <div className="space-y-2">
                <Label htmlFor="lastName" className="required">
                  {t('form.fields.lastName')}
                </Label>
                <Input
                  id="lastName"
                  value={formData.lastName}
                  onChange={(e) => handleInputChange('lastName', e.target.value)}
                  placeholder={t('form.placeholders.lastName')}
                  disabled={isLoading || isSubmitting}
                  className={errors.lastName ? 'border-red-500' : ''}
                  dir={isRtl ? 'rtl' : 'ltr'}
                />
                {errors.lastName && (
                  <p className="text-sm text-red-600">{errors.lastName}</p>
                )}
              </div>
            </div>

            {/* Phone Number */}
            <div className="space-y-2">
              <Label htmlFor="phoneNumber">
                {t('form.fields.phoneNumber')}
              </Label>
              <Input
                id="phoneNumber"
                type="tel"
                value={formData.phoneNumber}
                onChange={(e) => handleInputChange('phoneNumber', e.target.value)}
                placeholder={t('form.placeholders.phoneNumber')}
                disabled={isLoading || isSubmitting}
                className={errors.phoneNumber ? 'border-red-500' : ''}
                dir="ltr" // Phone numbers are always LTR
              />
              {errors.phoneNumber && (
                <p className="text-sm text-red-600">{errors.phoneNumber}</p>
              )}
            </div>

            {/* Preferred Language */}
            <div className="space-y-2">
              <Label htmlFor="preferredLanguage">
                {t('form.fields.preferredLanguage')}
              </Label>
              <select
                id="preferredLanguage"
                value={formData.preferredLanguage}
                onChange={(e) => handleInputChange('preferredLanguage', e.target.value)}
                disabled={isLoading || isSubmitting}
                className="w-full px-3 py-2 border rounded-md"
              >
                <option value="en">English</option>
                <option value="ar">العربية</option>
              </select>
            </div>
          </div>

          {/* Role Assignment */}
          <div className="space-y-4">
            <h3 className="text-lg font-semibold">{t('form.sections.roles')}</h3>
            
            <div className="space-y-2">
              <Label className="required">{t('form.fields.roles')}</Label>
              <div className="border rounded-md p-3 min-h-[60px] bg-background">
                {availableRoles.length === 0 ? (
                  <p className="text-sm text-gray-500">{t('form.noRolesAvailable')}</p>
                ) : (
                  <div className="space-y-2">
                    {availableRoles.map((role) => (
                      <div
                        key={role.id}
                        className="flex items-center justify-between p-2 border rounded hover:bg-gray-50"
                      >
                        <div className="flex items-center gap-3">
                          <input
                            type="checkbox"
                            id={`role-${role.id}`}
                            checked={formData.roles.includes(role.name)}
                            onChange={() => handleRoleToggle(role.name)}
                            disabled={isLoading || isSubmitting}
                            className="h-4 w-4"
                          />
                          <Label htmlFor={`role-${role.id}`} className="cursor-pointer">
                            {role.name}
                          </Label>
                        </div>
                        {role.description && (
                          <span className="text-sm text-gray-500">{role.description}</span>
                        )}
                      </div>
                    ))}
                  </div>
                )}
              </div>
              {errors.roles && (
                <p className="text-sm text-red-600">{errors.roles}</p>
              )}
              
              {/* Selected Roles Summary */}
              {getSelectedRoles().length > 0 && (
                <div className="flex flex-wrap gap-2 mt-2">
                  {getSelectedRoles().map((role) => (
                    <Badge key={role.id} variant="default">
                      {role.name}
                    </Badge>
                  ))}
                </div>
              )}
            </div>
          </div>

          {/* Status */}
          <div className="space-y-4">
            <h3 className="text-lg font-semibold">{t('form.sections.status')}</h3>
            
            <div className="flex items-center justify-between">
              <div className="space-y-1">
                <Label htmlFor="isActive">{t('form.fields.isActive')}</Label>
                <p className="text-sm text-gray-500">{t('form.descriptions.isActive')}</p>
              </div>
              <Switch
                id="isActive"
                checked={formData.isActive}
                onCheckedChange={(checked) => handleInputChange('isActive', checked)}
                disabled={isLoading || isSubmitting}
              />
            </div>
          </div>

          {/* Form Actions */}
          <div className="flex gap-3 pt-4 border-t">
            <Button
              type="button"
              variant="outline"
              onClick={onCancel}
              disabled={isLoading || isSubmitting}
              className="flex-1 sm:flex-none"
            >
              {t('form.actions.cancel')}
            </Button>
            <Button
              type="submit"
              disabled={isLoading || isSubmitting}
              className="flex-1 sm:flex-none"
            >
              {isSubmitting && <span className="mr-2">⏳</span>}
              {mode === 'create' ? t('form.actions.create') : t('form.actions.save')}
            </Button>
          </div>
        </form>
      </CardContent>
    </Card>
  );
};

export default UserForm;
