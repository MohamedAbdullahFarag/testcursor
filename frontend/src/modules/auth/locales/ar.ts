export const authAr = {
    auth: {
        // Login form
        loginTitle: 'مرحبًا بكم في نظام إدارة اختبار',
        loginDescription: 'إدارة الخدمات بسهولة وكفاءة. سجّل الدخول للمتابعة',
        login: 'تسجيل الدخول',
        logout: 'تسجيل الخروج',
        
        // Form fields
        email: 'البريد الإلكتروني',
        password: 'كلمة المرور',
        emailPlaceholder: 'أدخل بريدك الإلكتروني',
        passwordPlaceholder: 'أدخل كلمة المرور',
        
        // Loading states
        loggingIn: 'جارٍ تسجيل الدخول...',
        loggingOut: 'جارٍ تسجيل الخروج...',
        
        // Error messages
        loginError: 'فشل تسجيل الدخول',
        invalidCredentials: 'البريد الإلكتروني أو كلمة المرور غير صحيحة',
        networkError: 'حدث خطأ في الشبكة. يرجى المحاولة مرة أخرى.',
        sessionExpired: 'انتهت صلاحية جلستك. يرجى تسجيل الدخول مرة أخرى.',
        tokenRefreshFailed: 'فشل تجديد الجلسة. يرجى تسجيل الدخول مرة أخرى.',
        
        // Validation messages
        emailRequired: 'البريد الإلكتروني مطلوب',
        emailInvalid: 'يرجى إدخال بريد إلكتروني صحيح',
        passwordRequired: 'كلمة المرور مطلوبة',
        passwordMinLength: 'يجب أن تتكون كلمة المرور من 6 أحرف على الأقل',
        
        // Success messages
        loginSuccess: 'مرحبًا بعودتك!',
        logoutSuccess: 'تم تسجيل خروجك بنجاح',
        
        // Role-based access
        accessDenied: 'تم رفض الوصول',
        insufficientPermissions: 'ليس لديك صلاحيات كافية للوصول إلى هذا المورد',
        
        // Misc
        rememberMe: 'تذكرني',
        forgotPassword: 'هل نسيت كلمة المرور؟',
        noAccount: 'ليس لديك حساب؟',
        signUp: 'إنشاء حساب',
        backToLogin: 'العودة إلى تسجيل الدخول',
    },
} as const
