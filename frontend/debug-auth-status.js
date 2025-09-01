/**
 * Debug script to check authentication status
 * Run this in the browser console on the frontend to check auth state
 */

// Check Zustand auth store state
function checkAuthState() {
    const authStorage = localStorage.getItem('auth-storage');
    if (!authStorage) {
        console.log('❌ No auth storage found');
        return null;
    }
    
    try {
        const parsed = JSON.parse(authStorage);
        const authState = parsed.state;
        
        console.log('📊 Auth State:', {
            isAuthenticated: authState?.isAuthenticated,
            hasUser: !!authState?.user,
            hasAccessToken: !!authState?.accessToken,
            hasRefreshToken: !!authState?.refreshToken,
            userRoles: authState?.user?.roles,
            userName: authState?.user?.name,
            userEmail: authState?.user?.email
        });
        
        // Check token expiration
        if (authState?.accessToken) {
            try {
                const payload = JSON.parse(atob(authState.accessToken.split('.')[1]));
                const isExpired = payload.exp * 1000 < Date.now();
                console.log('🔑 Access Token:', {
                    isExpired,
                    expiresAt: new Date(payload.exp * 1000).toLocaleString(),
                    roles: payload.role || payload.roles
                });
            } catch (e) {
                console.log('❌ Invalid access token format');
            }
        }
        
        return authState;
    } catch (e) {
        console.log('❌ Error parsing auth storage:', e);
        return null;
    }
}

// Test API endpoints
async function testApiEndpoints() {
    const authState = checkAuthState();
    
    if (!authState?.isAuthenticated) {
        console.log('⚠️ Not authenticated - API tests skipped');
        return;
    }
    
    const baseUrl = 'https://localhost:7001';
    const headers = {
        'Authorization': `Bearer ${authState.accessToken}`,
        'Content-Type': 'application/json'
    };
    
    // Test audit logs endpoint
    console.log('\n🔍 Testing Audit Logs API...');
    try {
        const auditResponse = await fetch(`${baseUrl}/api/audit-logs`, { headers });
        console.log('Audit Logs Response:', auditResponse.status, auditResponse.statusText);
        
        if (auditResponse.status === 403) {
            console.log('❌ Audit logs: Insufficient permissions (needs Admin/Supervisor role)');
        } else if (auditResponse.status === 401) {
            console.log('❌ Audit logs: Authentication failed');
        } else if (auditResponse.ok) {
            console.log('✅ Audit logs: Access granted');
        }
    } catch (error) {
        console.log('❌ Audit logs error:', error.message);
    }
    
    // Test media search endpoint
    console.log('\n🔍 Testing Media Search API...');
    try {
        const mediaResponse = await fetch(`${baseUrl}/api/media/search`, {
            method: 'POST',
            headers,
            body: JSON.stringify({
                searchTerm: '',
                page: 1,
                pageSize: 10
            })
        });
        console.log('Media Search Response:', mediaResponse.status, mediaResponse.statusText);
        
        if (mediaResponse.status === 500) {
            console.log('❌ Media search: Server error (check backend logs)');
        } else if (mediaResponse.status === 401) {
            console.log('❌ Media search: Authentication failed');
        } else if (mediaResponse.ok) {
            console.log('✅ Media search: Working correctly');
        }
    } catch (error) {
        console.log('❌ Media search error:', error.message);
    }
}

// Run the debug checks
console.log('🚀 Starting authentication debug...\n');
checkAuthState();
testApiEndpoints();

console.log('\n📋 To copy this script, run:');
console.log('copy("' + 
    checkAuthState.toString() + ';' +
    testApiEndpoints.toString() + ';' +
    'checkAuthState(); testApiEndpoints();' + 
'")');
