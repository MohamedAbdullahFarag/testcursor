/**
 * Temporary API Feature Flags
 * Use these to disable problematic APIs while backend issues are being resolved
 */

export const API_FEATURE_FLAGS = {
    // Enable audit logs API calls (working properly)
    AUDIT_LOGS_ENABLED: true,
    
    // Enable media search API calls (AutoMapper issue fixed)
    MEDIA_SEARCH_ENABLED: true,
    
    // Enable development mode features
    DEV_MODE_ENABLED: import.meta.env.DEV,
    
    // Mock data for disabled APIs
    USE_MOCK_DATA: true,
} as const;

// Mock data for audit logs
export const MOCK_AUDIT_LOGS = {
    data: [
        {
            id: "1",
            createdAt: new Date(),
            updatedAt: new Date(),
            timestamp: new Date().toISOString(),
            userId: 1,
            username: "Admin User",
            action: "LOGIN",
            description: "User logged in successfully",
            entityType: "User",
            entityId: "1",
            ipAddress: "127.0.0.1",
            userAgent: "Mozilla/5.0",
            data: null,
            severity: "Low" as const,
            category: "Authentication" as const,
            success: true
        },
        {
            id: "2",
            createdAt: new Date(Date.now() - 300000),
            updatedAt: new Date(Date.now() - 300000),
            timestamp: new Date(Date.now() - 300000).toISOString(),
            userId: 1,
            username: "Admin User",
            action: "VIEW_DASHBOARD",
            description: "User accessed dashboard",
            entityType: "Page",
            entityId: "dashboard",
            ipAddress: "127.0.0.1",
            userAgent: "Mozilla/5.0",
            data: null,
            severity: "Low" as const,
            category: "System" as const,
            success: true
        }
    ],
    total: 2,
    page: 0,
    pageSize: 25,
    totalPages: 1,
    hasNext: false,
    hasPrevious: false
};

// Mock data for media search
export const MOCK_MEDIA_FILES = {
    items: [
        {
            id: 1,
            fileName: "sample-image.jpg",
            originalFileName: "sample-image.jpg",
            mimeType: "image/jpeg",
            fileSize: 245760,
            uploadedAt: new Date().toISOString(),
            uploadedBy: 1,
            status: "ACTIVE" as const,
            metadata: []
        },
        {
            id: 2,
            fileName: "document.pdf",
            originalFileName: "document.pdf",
            mimeType: "application/pdf",
            fileSize: 1048576,
            uploadedAt: new Date(Date.now() - 600000).toISOString(),
            uploadedBy: 1,
            status: "ACTIVE" as const,
            metadata: []
        }
    ],
    total: 2,
    page: 1,
    pageSize: 10,
    totalPages: 1,
    hasNext: false,
    hasPrevious: false
};
