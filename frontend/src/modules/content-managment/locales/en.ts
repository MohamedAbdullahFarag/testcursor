export const contentManagmentEn = {
    // Page header and navigation
    page: {
        title: 'Content Management',
        description: 'Manage all your content including media files, questions, and notifications',
        breadcrumbTitle: 'Content Management'
    },

    // Feature cards
    features: {
        mediaManagement: {
            title: 'Media Management',
            description: 'Manage multimedia content including images, videos, audio files, and documents',
            stats: '{{count}} files • {{size}} GB'
        },
        questionBank: {
            title: 'Question Bank',
            description: 'Create and organize questions, question banks, and educational content',
            stats: 'Tree management • Categories'
        },
        notifications: {
            title: 'Notifications',
            description: 'Manage system notifications and user communication',
            stats: 'Notification center • Preferences'
        }
    },

    // Content overview section
    overview: {
        title: 'Content Overview',
        totalFiles: 'Total Files',
        totalSize: 'Total Storage',
        activeNotifications: 'Active Notifications',
        questionBanks: 'Question Banks',
        recentActivity: 'Recent Activity',
        mediaTypes: {
            images: 'Images',
            videos: 'Videos',
            documents: 'Documents',
            audio: 'Audio'
        }
    },

    // Status indicators
    status: {
        implemented: 'Implemented',
        inProgress: 'In Progress',
        planned: 'Planned',
        active: 'Active',
        inactive: 'Inactive'
    },

    // Actions
    actions: {
        manage: 'Manage',
        view: 'View',
        configure: 'Configure',
        upload: 'Upload',
        create: 'Create',
        edit: 'Edit',
        delete: 'Delete'
    },

    // Legacy content management fields
    contentManagment: {
        title: 'Content Management',
        nationalId: 'National ID',
    },
    
    // Shared validation messages (keeping existing)
    sharedValidation: {
        required: 'This field is mandatory. Please Enter a {0}.',
        requiredAn: 'This field is mandatory. Please Enter an {0}.',
        requiredThe: 'This field is mandatory. Please Enter the {0}.',
        requiredSelect: 'This field is mandatory. Please select a {0}.',
        requiredSelectAn: 'This field is mandatory. Please select an {0}.',
        requiredSelectThe: 'This field is mandatory. Please select the {0}.',
        valid: 'The {0} entered is incorrect',
        requiredAttachment: 'Please attach {1} {0}',
        requiredAccount: 'Please enter {0} account',
        exceedsTheAllowedLimit: 'You have reached the maximum limit of allowed characters',
        acceptLettersAndNumbers: 'The {0} field accepts letters and numbers only',
        acceptNumbers: 'The field accepts numbers only',
        lessThan: 'The value of field must be less than or  equal to {0}',
        greaterThan: 'The value of the field must be greater than or equal to {0}',
        canNotAcceptSpecialCharacter: "It is not allowed to enter special characters like: !@#$%^&*()_+-'~<>/\\?|÷×",
        acceptLetters: 'The {0} field accepts letters only',
        noAccount: 'In case you do not have a {0} account please enter "nothing"',
        noArabicLetters: 'The {0} field does not accept Arabic letters',
        usedID: 'National ID already registered',
        maximunStudiPeriod: 'Maximum study is five years and 11 months',
        maximunFiles: 'Maximum 4 files',
        acceptedPassport: 'Please enter a valid passport',
        endDateMustBeGreaterThan: '{0} must be greater than {1}',
        dateMustBeGreaterThanToday: "{0} must be greater than today's date",
    },
} as const
