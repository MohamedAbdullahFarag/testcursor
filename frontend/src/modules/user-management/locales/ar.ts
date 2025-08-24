export default {
  users: 'المستخدمين',
  noUsers: 'لم يتم العثور على مستخدمين',
  loading: 'جاري تحميل المستخدمين...',
  create: 'إنشاء مستخدم',
  edit: 'تعديل المستخدم',
  delete: 'حذف المستخدم',
  save: 'حفظ',
  cancel: 'إلغاء',
  confirm: 'تأكيد',
  actions: 'الإجراءات',
  status: 'الحالة',
  active: 'نشط',
  inactive: 'غير نشط',
  roles: 'الأدوار',
  noRoles: 'لا توجد أدوار مخصصة',
  email: 'البريد الإلكتروني',
  fullName: 'الاسم الكامل',
  password: 'كلمة المرور',
  createdAt: 'تاريخ الإنشاء',
  lastLogin: 'آخر تسجيل دخول',
  search: 'البحث عن المستخدمين...',
  filter: 'تصفية',
  clearFilters: 'مسح التصفية',
  refresh: 'تحديث',
  export: 'تصدير',
  bulkDelete: 'حذف المحدد',
  selectedCount: '{{count}} محدد',
  showingResults: 'عرض {{start}}-{{end}} من {{total}} مستخدم',
  totalCount: '{{count}} مستخدم إجمالي',
  
  // Form fields
  form: {
    createTitle: 'إنشاء مستخدم جديد',
    editTitle: 'تعديل المستخدم',
    sections: {
      basicInfo: 'المعلومات الأساسية',
      roles: 'تعيين الأدوار',
      status: 'الحالة'
    },
    fields: {
      username: 'اسم المستخدم',
      email: 'عنوان البريد الإلكتروني',
      firstName: 'الاسم الأول',
      lastName: 'اسم العائلة',
      password: 'كلمة المرور',
      phoneNumber: 'رقم الهاتف',
      preferredLanguage: 'اللغة المفضلة',
      roles: 'الأدوار',
      isActive: 'حالة النشاط'
    },
    placeholders: {
      username: 'أدخل اسم المستخدم',
      email: 'أدخل عنوان البريد الإلكتروني',
      firstName: 'أدخل الاسم الأول',
      lastName: 'أدخل اسم العائلة',
      password: 'أدخل كلمة المرور',
      phoneNumber: 'أدخل رقم الهاتف'
    },
    descriptions: {
      isActive: 'تفعيل أو إلغاء تفعيل حساب هذا المستخدم'
    },
    errors: {
      usernameRequired: 'اسم المستخدم مطلوب',
      usernameTooShort: 'يجب أن يكون اسم المستخدم 3 أحرف على الأقل',
      emailRequired: 'البريد الإلكتروني مطلوب',
      emailInvalid: 'يرجى إدخال عنوان بريد إلكتروني صحيح',
      firstNameRequired: 'الاسم الأول مطلوب',
      firstNameTooShort: 'يجب أن يكون الاسم الأول حرفين على الأقل',
      lastNameRequired: 'اسم العائلة مطلوب',
      lastNameTooShort: 'يجب أن يكون اسم العائلة حرفين على الأقل',
      passwordRequired: 'كلمة المرور مطلوبة',
      passwordTooShort: 'يجب أن تكون كلمة المرور 8 أحرف على الأقل',
      phoneInvalid: 'يرجى إدخال رقم هاتف صحيح',
      roleRequired: 'مطلوب دور واحد على الأقل',
      submitFailed: 'فشل في حفظ المستخدم'
    },
    actions: {
      create: 'إنشاء المستخدم',
      save: 'حفظ التغييرات',
      cancel: 'إلغاء'
    },
    noRolesAvailable: 'لا توجد أدوار متاحة'
  },
  
  // Messages
  messages: {
    userCreated: 'تم إنشاء المستخدم بنجاح',
    userUpdated: 'تم تحديث المستخدم بنجاح',
    userDeleted: 'تم حذف المستخدم بنجاح',
    usersDeleted: 'تم حذف {{count}} مستخدم بنجاح',
    confirmDelete: 'هل أنت متأكد من أنك تريد حذف هذا المستخدم؟',
    confirmBulkDelete: 'هل أنت متأكد من أنك تريد حذف {{count}} مستخدم محدد؟',
    deleteWarning: 'لا يمكن التراجع عن هذا الإجراء.',
    operationFailed: 'فشلت العملية. يرجى المحاولة مرة أخرى.'
  },
  
  // Validation
  validation: {
    required: 'هذا الحقل مطلوب',
    emailInvalid: 'يرجى إدخال عنوان بريد إلكتروني صحيح',
    passwordMinLength: 'يجب أن تكون كلمة المرور {{min}} أحرف على الأقل',
    usernameMinLength: 'يجب أن يكون اسم المستخدم {{min}} أحرف على الأقل',
    nameMinLength: 'يجب أن يكون الاسم {{min}} أحرف على الأقل'
  }
};
