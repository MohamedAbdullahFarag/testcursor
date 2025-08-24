import { AuthService, OIDCConfig } from 'mada-design-system'

const config: OIDCConfig = {
    clientId: import.meta.env.VITE_OIDC_CLIENT_ID,
    clientSecret: import.meta.env.VITE_OIDC_CLIENT_SECRET,
    baseUrl: process.env.NODE_ENV === 'production' ? import.meta.env.VITE_APP_BASE_URL : 'https://localhost:7001',
    authUrl: `${import.meta.env.VITE_APP_AUTH_URL}mga/sps/oauth/oauth20`,
    oauth20LogoutUrl: `${import.meta.env.VITE_APP_AUTH_URL}pkmslogout?redirect=${process.env.NODE_ENV === 'production' ? import.meta.env.VITE_APP_BASE_URL : 'https://localhost:7001'}`,
    disablePKCE: JSON.parse(import.meta.env.VITE_DISABLE_PKCE),
}

export const authService = AuthService.getInstance(config)
