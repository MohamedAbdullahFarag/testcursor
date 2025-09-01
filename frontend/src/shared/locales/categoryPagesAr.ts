/**
 * Arabic translations for category pages
 * Shared translations for System, Analytics, Customer Experience, and Help category pages
 */

export const categoryPagesAr = {
  // Common elements
  common: {
    dashboard: 'لوحة التحكم',
    implemented: 'مُطبق',
    comingSoon: 'قريباً',
    inDevelopment: 'قيد التطوير'
  },

  // System category
  system: {
    title: 'إدارة النظام',
    description: 'إدارة إعدادات النظام والتكوينات والمراقبة',
    breadcrumbTitle: 'إدارة النظام',
    features: {
      settings: {
        title: 'إعدادات النظام',
        description: 'تكوين معاملات النظام والتفضيلات'
      },
      apiDocs: {
        title: 'توثيق واجهة البرمجة',
        description: 'عرض توثيق واجهة البرمجة ونقاط النهاية'
      },
      health: {
        title: 'صحة النظام',
        description: 'مراقبة أداء النظام ومقاييس الصحة'
      }
    }
  },

  // Analytics category  
  analytics: {
    title: 'التحليلات',
    description: 'عرض التحليلات الشاملة والتقارير',
    breadcrumbTitle: 'التحليلات',
    features: {
      dashboard: {
        title: 'لوحة التحليلات',
        description: 'نظرة عامة على تحليلات النظام والمقاييس الرئيسية'
      },
      users: {
        title: 'تحليلات المستخدمين',
        description: 'تحليل سلوك المستخدمين والتفاعل'
      },
      content: {
        title: 'تحليلات المحتوى',
        description: 'تتبع أداء المحتوى وإحصائيات الاستخدام'
      }
    }
  },

  // Customer Experience category
  customerExperience: {
    title: 'تجربة العملاء',
    description: 'إدارة ملاحظات العملاء والرضا',
    breadcrumbTitle: 'تجربة العملاء',
    features: {
      surveys: {
        title: 'استبيانات الخدمة',
        description: 'إنشاء وإدارة استبيانات جودة الخدمة'
      },
      feedback: {
        title: 'ملاحظات المحتوى',
        description: 'جمع وتحليل ملاحظات المحتوى'
      }
    }
  },

  // Help category
  help: {
    title: 'المساعدة والدعم',
    description: 'الوصول إلى موارد المساعدة ومعلومات الدعم',
    breadcrumbTitle: 'المساعدة والدعم',
    features: {
      faq: {
        title: 'الأسئلة الشائعة',
        description: 'الأسئلة المتكررة وإجاباتها'
      },
      terms: {
        title: 'الشروط والأحكام',
        description: 'عرض شروط وأحكام الخدمة'
      },
      privacy: {
        title: 'سياسة الخصوصية',
        description: 'مراجعة سياسة الخصوصية ومعالجة البيانات'
      }
    }
  }
};

export default categoryPagesAr;
