import React, { useState, useEffect } from 'react';
import { useTranslation } from 'react-i18next';
import {
  Button,
  Input,
  Label,
  Switch,
  Checkbox,
  Alert,
  AlertDescription,
} from 'mada-design-system';
import { Role, CreateRoleRequest, UpdateRoleRequest, Permission } from '../models/role.types';

interface RoleFormProps {
  /** Role to edit (null for create mode) */
  role?: Role | null;
  /** Available permissions for assignment */
  availablePermissions?: Permission[];
  /** Loading state */
  isLoading?: boolean;
  /** Form mode */
  mode: 'create' | 'edit';
  /** Form submission callback */
  onSubmit: (data: CreateRoleRequest | UpdateRoleRequest) => Promise<void>;
  /** Cancel callback */
  onCancel: () => void;
}

interface FormData {
  code: string;
  name: string;
  description: string;
  isActive: boolean;
  permissions: string[];
}

interface FormErrors {
  code?: string;
  name?: string;
  description?: string;
  form?: string;
}

/**
 * RoleForm component - Form for creating and editing roles
 * 
 * Features:
 * - Create and edit modes
 * - Validation with error messages
 * - Permission selection
 * - System role protection
 * - Loading state handling
 * 
 * @param props - RoleFormProps
 */
export const RoleForm: React.FC<RoleFormProps> = ({
  role,
  availablePermissions = [],
  isLoading = false,
  mode,
  onSubmit,
  onCancel,
}) => {
  const { t } = useTranslation('roleManagement');
  const isEditMode = mode === 'edit';
  const isSystemRole = role?.isSystemRole || false;

  // Form state
  const [formData, setFormData] = useState<FormData>({
    code: '',
    name: '',
    description: '',
    isActive: true,
    permissions: [],
  });

  const [errors, setErrors] = useState<FormErrors>({});
  const [touched, setTouched] = useState<Record<string, boolean>>({});

  // Initialize form data from role (for edit mode)
  useEffect(() => {
    if (isEditMode && role) {
      setFormData({
        code: role.code,
        name: role.name,
        description: role.description || '',
        isActive: role.isActive,
        permissions: role.permissions || [],
      });
    }
  }, [role, isEditMode]);

  // Handle form input changes
  const handleChange = (
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>
  ) => {
    const { name, value } = e.target;
    
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
    
    // Mark field as touched
    if (!touched[name]) {
      setTouched((prev) => ({
        ...prev,
        [name]: true,
      }));
    }
    
    // Clear field error when user types
    if (errors[name as keyof FormErrors]) {
      setErrors((prev) => ({
        ...prev,
        [name]: undefined,
      }));
    }
  };

  // Handle checkbox/switch changes
  const handleBooleanChange = (name: string, checked: boolean) => {
    setFormData((prev) => ({
      ...prev,
      [name]: checked,
    }));
  };

  // Handle permission selection
  const handlePermissionChange = (code: string, checked: boolean) => {
    setFormData((prev) => ({
      ...prev,
      permissions: checked
        ? [...prev.permissions, code]
        : prev.permissions.filter((p) => p !== code),
    }));
  };

  // Validate the form
  const validateForm = (): boolean => {
    const newErrors: FormErrors = {};
    
    // Required field validation
    if (!formData.name.trim()) {
      newErrors.name = t('nameRequired');
    }
    
    // Only validate code in create mode
    if (!isEditMode) {
      if (!formData.code.trim()) {
        newErrors.code = t('codeRequired');
      } else if (!/^[a-z0-9-_]+$/.test(formData.code)) {
        newErrors.code = t('codeInvalid');
      }
    }
    
    setErrors(newErrors);
    
    // Mark all fields as touched
    setTouched({
      code: true,
      name: true,
      description: true,
    });
    
    return Object.keys(newErrors).length === 0;
  };

  // Handle form submission
  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    if (!validateForm()) {
      return;
    }
    
    try {
      if (isEditMode) {
        // For edit mode, only include fields that can be updated
        await onSubmit({
          name: formData.name,
          description: formData.description,
          isActive: formData.isActive,
          permissions: formData.permissions,
        });
      } else {
        // For create mode, include all fields
        await onSubmit(formData);
      }
    } catch (err) {
      setErrors({
        form: err instanceof Error ? err.message : t('submitError'),
      });
    }
  };

  // Group permissions by category for better organization
  const groupedPermissions = availablePermissions.reduce<Record<string, Permission[]>>(
    (groups, permission) => {
      const category = permission.category || 'Other';
      if (!groups[category]) {
        groups[category] = [];
      }
      groups[category].push(permission);
      return groups;
    },
    {}
  );

  return (
    <form onSubmit={handleSubmit}>
      {/* Form error */}
      {errors.form && (
        <Alert className="mb-4">
          <AlertDescription>{errors.form}</AlertDescription>
        </Alert>
      )}

      <div className="grid gap-4 py-4">
        {/* Code field (only in create mode) */}
        {!isEditMode && (
          <div className="grid gap-2">
            <Label htmlFor="code" className="text-left">
              {t('code')} *
            </Label>
            <Input
              id="code"
              name="code"
              value={formData.code}
              onChange={handleChange}
              placeholder={t('codeExample')}
              disabled={isLoading}
              aria-invalid={!!errors.code}
              aria-describedby={errors.code ? 'code-error' : undefined}
            />
            {errors.code && touched.code && (
              <p id="code-error" className="text-sm text-red-500">
                {errors.code}
              </p>
            )}
            <p className="text-sm text-gray-500">{t('codeHelper')}</p>
          </div>
        )}

        {/* Name field */}
        <div className="grid gap-2">
          <Label htmlFor="name" className="text-left">
            {t('name')} *
          </Label>
          <Input
            id="name"
            name="name"
            value={formData.name}
            onChange={handleChange}
            placeholder={t('namePlaceholder')}
            disabled={isLoading || isSystemRole}
            aria-invalid={!!errors.name}
            aria-describedby={errors.name ? 'name-error' : undefined}
          />
          {errors.name && touched.name && (
            <p id="name-error" className="text-sm text-red-500">
              {errors.name}
            </p>
          )}
        </div>

        {/* Description field */}
        <div className="grid gap-2">
          <Label htmlFor="description" className="text-left">
            {t('description')}
          </Label>
          <Input
            id="description"
            name="description"
            value={formData.description}
            onChange={handleChange}
            placeholder={t('descriptionPlaceholder')}
            disabled={isLoading || isSystemRole}
          />
        </div>

        {/* Active status */}
        <div className="flex items-center justify-between">
          <Label htmlFor="isActive" className="text-left">
            {t('isActive')}
          </Label>
          <Switch
            id="isActive"
            checked={formData.isActive}
            onCheckedChange={(checked) => handleBooleanChange('isActive', checked)}
            disabled={isLoading || isSystemRole}
            aria-label={t('isActive')}
          />
        </div>

        {/* Permissions section */}
        {availablePermissions.length > 0 && (
          <div className="grid gap-2 mt-4">
            <h3 className="font-medium">{t('permissions')}</h3>
            
            {Object.entries(groupedPermissions).map(([category, permissions]) => (
              <div key={category} className="bg-gray-50 p-3 rounded-md mb-2">
                <h4 className="font-medium mb-2">{category}</h4>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-2">
                  {permissions.map((permission) => (
                    <div key={permission.code} className="flex items-center space-x-2">
                      <Checkbox
                        id={`permission-${permission.code}`}
                        checked={formData.permissions.includes(permission.code)}
                        onCheckedChange={(checked) => 
                          handlePermissionChange(permission.code, checked === true)
                        }
                        disabled={isLoading || isSystemRole}
                        aria-label={permission.name}
                      />
                      <label
                        htmlFor={`permission-${permission.code}`}
                        className="text-sm leading-none peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
                      >
                        {permission.name}
                        {permission.description && (
                          <span className="block text-xs text-gray-500">
                            {permission.description}
                          </span>
                        )}
                      </label>
                    </div>
                  ))}
                </div>
              </div>
            ))}
            
            {Object.keys(groupedPermissions).length === 0 && (
              <p className="text-sm text-gray-500">{t('noPermissionsAvailable')}</p>
            )}
          </div>
        )}
      </div>

      {/* System role warning */}
      {isSystemRole && (
        <Alert className="mb-4">
          <AlertDescription>{t('systemRoleWarning')}</AlertDescription>
        </Alert>
      )}

      {/* Form actions */}
      <div className="flex justify-end gap-2">
        <Button
          type="button"
          variant="outline"
          onClick={onCancel}
          disabled={isLoading}
          aria-label={t('cancel')}
        >
          {t('cancel')}
        </Button>
        
        <Button
          type="submit"
          disabled={isLoading || isSystemRole}
          aria-label={isEditMode ? t('saveChanges') : t('createRole')}
        >
          {isLoading ? t('saving') : isEditMode ? t('saveChanges') : t('createRole')}
        </Button>
      </div>
    </form>
  );
};
