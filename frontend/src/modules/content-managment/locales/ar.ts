export const contentManagmentAr = {
    // Page header and navigation
    page: {
        title: 'إدارة المحتوى',
        description: 'إدارة جميع المحتويات بما في ذلك الملفات الوسائطية والأسئلة والإشعارات',
        breadcrumbTitle: 'إدارة المحتوى'
    },

    // Feature cards
    features: {
        mediaManagement: {
            title: 'إدارة الوسائط',
            description: 'إدارة المحتوى الوسائطي بما في ذلك الصور والفيديوهات والملفات الصوتية والمستندات',
            stats: '{{count}} ملف • {{size}} جيجابايت'
        },
        questionBank: {
            title: 'بنك الأسئلة',
            description: 'إنشاء وتنظيم الأسئلة وبنوك الأسئلة والمحتوى التعليمي',
            stats: 'إدارة الشجرة • فئات'
        },
        notifications: {
            title: 'الإشعارات',
            description: 'إدارة إشعارات النظام والتواصل مع المستخدمين',
            stats: 'مركز الإشعارات • التفضيلات'
        }
    },

    // Content overview section
    overview: {
        title: 'نظرة عامة على المحتوى',
        totalFiles: 'إجمالي الملفات',
        totalSize: 'إجمالي التخزين',
        activeNotifications: 'الإشعارات النشطة',
        questionBanks: 'بنوك الأسئلة',
        recentActivity: 'النشاط الأخير',
        mediaTypes: {
            images: 'الصور',
            videos: 'الفيديوهات',
            documents: 'المستندات',
            audio: 'الملفات الصوتية'
        }
    },

    // Status indicators
    status: {
        implemented: 'مُطبق',
        inProgress: 'قيد التطوير',
        planned: 'مخطط',
        active: 'نشط',
        inactive: 'غير نشط'
    },

    // Actions
    actions: {
        manage: 'إدارة',
        view: 'عرض',
        configure: 'تكوين',
        upload: 'رفع',
        create: 'إنشاء',
        edit: 'تعديل',
        delete: 'حذف'
    },

    // Legacy content management fields
    contentManagment: {
        title: 'إدارة المحتوى',
        nationalId: 'الهوية الوطنية',
    },
    
    // Shared validation messages (keeping existing)
    sharedValidation: {
        required: 'الرجاء إدخال {0}',
        requiredAn: 'الرجاء إدخال {0}',
        requiredThe: 'الرجاء إدخال {0}',
        requiredSelect: 'الرجاء اختيار {0}',
        requiredSelectAn: 'الرجاء اختيار {0}',
        requiredSelectThe: 'الرجاء اختيار {0}',
        valid: 'الرجاء إدخال {0} بشكل صحيح',
        requiredAttachment: 'الرجاء إرفاق {0}',
        requiredAccount: 'الرجاء إدخال حساب {0}',
        exceedsTheAllowedLimit: 'عدد خانات الحقل تجاوز الحد المسموح به',
        acceptLettersAndNumbers: 'حقل {0} يقبل حروف وأرقام فقط',
        acceptNumbers: 'الحقل يقبل أرقام فقط',
        lessThan: 'قيمة الحقل يجب ألا تزيد عن {0}',
        greaterThan: 'قيمة الحقل يجب ألا تقل عن {0}',
        canNotAcceptSpecialCharacter: "لايسمح بادخال الرموز الخاصة مثل: !@#$%^&*()_+-'~<>/\\?|÷×",
        acceptLetters: 'حقل {0} يقبل حروف فقط',
        noAccount: 'في حال عدم وجود حساب {0} الرجاء ادخال قيمة "لا يوجد"',
        noArabicLetters: 'حقل {0} لا يقبل حروف عربية',
        usedID: 'رقم الهوية المدخل مسجل مسبقا في اختبار.',
        maximunStudiPeriod: 'الحد الأقصى للدراسة خمس سنوات و11 شهر',
        maximunFiles: 'الحد الأقصى 4 ملفات',
        acceptedPassport: 'الرجاء إدخال جواز سفر ساري الصلاحية',
        endDateMustBeGreaterThan: '{0} يجب أن يكون أكبر من {1}',
        dateMustBeGreaterThanToday: '{0} يجب أن يكون أصغر من تاريخ اليوم',
    },
} as const
