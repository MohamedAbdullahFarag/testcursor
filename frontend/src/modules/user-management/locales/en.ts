export default {
  users: 'Users',
  noUsers: 'No users found',
  loading: 'Loading users...',
  create: 'Create User',
  edit: 'Edit User',
  delete: 'Delete User',
  save: 'Save',
  cancel: 'Cancel',
  confirm: 'Confirm',
  actions: 'Actions',
  status: 'Status',
  active: 'Active',
  inactive: 'Inactive',
  roles: 'Roles',
  noRoles: 'No roles assigned',
  email: 'Email',
  fullName: 'Full Name',
  password: 'Password',
  createdAt: 'Created At',
  lastLogin: 'Last Login',
  search: 'Search users...',
  filter: 'Filter',
  clearFilters: 'Clear Filters',
  refresh: 'Refresh',
  export: 'Export',
  bulkDelete: 'Delete Selected',
  selectedCount: '{{count}} selected',
  showingResults: 'Showing {{start}}-{{end}} of {{total}} users',
  totalCount: '{{count}} total users',
  
  // Form fields
  form: {
    createTitle: 'Create New User',
    editTitle: 'Edit User',
    sections: {
      basicInfo: 'Basic Information',
      roles: 'Role Assignment',
      status: 'Status'
    },
    fields: {
      username: 'Username',
      email: 'Email Address',
      firstName: 'First Name',
      lastName: 'Last Name',
      password: 'Password',
      phoneNumber: 'Phone Number',
      preferredLanguage: 'Preferred Language',
      roles: 'Roles',
      isActive: 'Active Status'
    },
    placeholders: {
      username: 'Enter username',
      email: 'Enter email address',
      firstName: 'Enter first name',
      lastName: 'Enter last name',
      password: 'Enter password',
      phoneNumber: 'Enter phone number'
    },
    descriptions: {
      isActive: 'Enable or disable this user account'
    },
    errors: {
      usernameRequired: 'Username is required',
      usernameTooShort: 'Username must be at least 3 characters',
      emailRequired: 'Email is required',
      emailInvalid: 'Please enter a valid email address',
      firstNameRequired: 'First name is required',
      firstNameTooShort: 'First name must be at least 2 characters',
      lastNameRequired: 'Last name is required',
      lastNameTooShort: 'Last name must be at least 2 characters',
      passwordRequired: 'Password is required',
      passwordTooShort: 'Password must be at least 8 characters',
      phoneInvalid: 'Please enter a valid phone number',
      roleRequired: 'At least one role is required',
      submitFailed: 'Failed to save user'
    },
    actions: {
      create: 'Create User',
      save: 'Save Changes',
      cancel: 'Cancel'
    },
    noRolesAvailable: 'No roles available'
  },
  
  // Messages
  messages: {
    userCreated: 'User created successfully',
    userUpdated: 'User updated successfully',
    userDeleted: 'User deleted successfully',
    usersDeleted: '{{count}} users deleted successfully',
    confirmDelete: 'Are you sure you want to delete this user?',
    confirmBulkDelete: 'Are you sure you want to delete {{count}} selected users?',
    deleteWarning: 'This action cannot be undone.',
    operationFailed: 'Operation failed. Please try again.'
  },
  
  // Validation
  validation: {
    required: 'This field is required',
    emailInvalid: 'Please enter a valid email address',
    passwordMinLength: 'Password must be at least {{min}} characters',
    usernameMinLength: 'Username must be at least {{min}} characters',
    nameMinLength: 'Name must be at least {{min}} characters'
  }
};
