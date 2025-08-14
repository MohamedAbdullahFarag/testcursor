# Project Structure

This document describes the structure of the project to help you navigate and understand the purpose of each folder and file.

## Root Level

- **`App.tsx`**: The main application component where the root component tree is defined.
- **`main.tsx`**: The entry point of the application, where the React app is bootstrapped.

## Directory Structure

### Tree View of the Directory Structure

```
┣ assets
┃ ┣ css
┃ ┃ ┗ shared.css
┃ ┗ images
┃   ┣ Logo ME.svg
┃   ┣ MEFullLogo.svg
┃   ┣ MOESpinner.svg
┃   ┗ Vision.svg
┃
┣ layout
┃ ┣ DashboardLayout
┃ ┃ ┣ DashboardSideBar.tsx
┃ ┃ ┣ DashboardTopHeader.tsx
┃ ┃ ┗ index.tsx
┃ ┣ PortalLayout
┃ ┃ ┣ index.tsx
┃ ┃ ┣ PortalFooter.tsx
┃ ┃ ┗ PortalTopHeader.ts
┃ ┗ Accessibility.tsx
┣ modules
┃ ┣ auth
┃ ┃ ┣ components
┃ ┃ ┃ ┣ AuthProvider.tsx
┃ ┃ ┃ ┣ OidcAuthCallback.tsx
┃ ┃ ┃ ┗ PortalProvider.tsx
┃ ┃ ┣ hooks
┃ ┃ ┃ ┗ useLogout.ts
┃ ┃ ┣ locales
┃ ┃ ┃ ┣ ar.ts
┃ ┃ ┃ ┗ en.ts
┃ ┃ ┣ store
┃ ┃ ┃ ┗ authStore.ts
┃ ┃ ┣ services
┃ ┃ ┃ ┗ useAuthentication.ts
┃ ┃ ┗ views
┃ ┃   ┗ Login.tsx
┃ ┣ content-managment
┃ ┃ ┣ locales
┃ ┃ ┃ ┣ ar.ts
┃ ┃ ┃ ┗ en.ts
┃ ┃ ┗ views
┃ ┃   ┗ index.tsx
┃ ┣ dashboard
┃ ┃ ┣ locales
┃ ┃ ┃ ┣ ar.ts
┃ ┃ ┃ ┗ en.ts
┃ ┃ ┗ views
┃ ┃   ┗ index.tsx
┃ ┣ platform-managment
┃ ┃ ┣ locales
┃ ┃ ┃ ┣ ar.ts
┃ ┃ ┃ ┗ en.ts
┃ ┃ ┗ views
┃ ┃   ┗ index.tsx
┃ ┗ requests-managment
┃   ┣ locales
┃   ┃ ┣ ar.ts
┃   ┃ ┗ en.ts
┃   ┗ views
┃     ┗ index.tsx
┣ pages
┃ ┣ ErrorPage
┃ ┃ ┗ index.tsx
┃ ┗ NotFoundPage
┃   ┗ index.tsx
┣ routes
┃ ┣ DashboardRoutes
┃ ┃ ┣ dashboardRoutes.tsx
┃ ┃ ┗ index.tsx
┃ ┣ PortalRoutes
┃ ┃ ┣ index.tsx
┃ ┃ ┗ portalRoutes.tsx
┃ ┗ index.tsx
┣ shared
┃ ┣ components
┃ ┃ ┣ MainPanel.tsx
┃ ┃ ┣ PageLoader.tsx
┃ ┃ ┗ SidePanel.tsx
┃ ┣ constants
┃ ┃ ┣ oidcConfig.ts
┃ ┃ ┗ pathNames.ts
┃ ┣ hooks
┃ ┃ ┗ index.tsx
┃ ┣ Lib
┃ ┃ ┗ sonar.ts
┃ ┣ locales
┃ ┃ ┣ ar.ts
┃ ┃ ┣ en.ts
┃ ┃ ┗ index.ts
┃ ┣ models
┃ ┃ ┣ enums
┃ ┃ ┃ ┗ index.ts
┃ ┃ ┗ interfaces
┃ ┃   ┗ index.ts
┃ ┣ services
┃ ┃ ┗ http.ts
┃ ┣ store
┃ ┃ ┣ middleware.ts
┃ ┃ ┗ usePanelStore.ts
┃ ┗ utils
┃   ┗ lazyRetry.ts
┣ App.tsx
┗ main.tsx
```

### Naming Conventions

#### Folders

- Lower case for structural/categorical folders: `modules/`, `services/`, `components/`
- PascalCase for component folders: `Button/`, `InventoryList/`

#### Files

- **React Components**: PascalCase (e.g., `InventoryList.tsx`, `Button.tsx`)
- **Entry points**: lowercase (e.g., `index.tsx`, `main.tsx`)
- **Services/Utils/Hooks/Types**: camelCase (e.g., `apiClient.ts`, `useAuth.ts`, `inventoryTypes.ts`)

### Detailed Description

### **`assets`**

Contains static assets like CSS and images.

- **`css/`**: Stylesheets shared across the application.
    - `shared.css`: Global shared CSS styles.
- **`images/`**: Image assets used throughout the application.
    - `Logo ME.svg`, `MEFullLogo.svg`, `MOESpinner.svg`, `Vision.svg`: Various SVG assets.

### **`layout`**

Defines the layout components for the application.

- **`DashboardLayout/`**: Components specific to the dashboard layout.
    - `DashboardSideBar.tsx`: Sidebar component for the dashboard.
    - `DashboardTopHeader.tsx`: Top header component for the dashboard.
    - `index.tsx`: Entry point for exporting dashboard layout components.
- **`PortalLayout/`**: Components specific to the portal layout.
    - `PortalFooter.tsx`: Footer component for the portal.
    - `PortalTopHeader.tsx`: Top header component for the portal.
    - `index.tsx`: Entry point for exporting portal layout components.
- **`Accessibility.tsx`**: Component for managing accessibility features.

### **`modules`**

Contains feature-specific modules, each with its own set of components, hooks, locales, stores, and views.

#### Example: `auth`

- **`components/`**: Contains reusable authentication-related components.

    - `AuthProvider.tsx`, `OidcAuthCallback.tsx`, `PortalProvider.tsx`

- **`hooks/`**: Hooks specific to authentication.

    - `useLogout.ts`

- `locales/`: Localization files for authentication.

    - `ar.ts`: Arabic translations.
    - `en.ts`: English translations.

- **`store/`**: State management for authentication.

    - `authStore.ts`

- **`services/`**: HTTP client data fetching definition (TanStack Query).

    - `authAuthentication.ts`

- **`views/`**: Authentication-related views.

    - `Login.tsx`

Other modules follow a similar structure (e.g., `dashboard`, `content-managment`).

### **`pages`**

Contains standalone page components.

- **`ErrorPage/`**: Generic error page.
    - `index.tsx`
- **`NotFoundPage/`**: 404 Not Found page.
    - `index.tsx`

### **`routes`**

Defines route configurations for the application. (react Router 7)

- **`DashboardRoutes/`**: Routes for the dashboard.
    - `dashboardRoutes.tsx`: Specific routes for the dashboard.
    - `index.tsx`: Export router point.
- **`PortalRoutes/`**: Routes for the portal.
    - `portalRoutes.tsx`: Specific routes for the portal.
    - `index.tsx`: Export router point.
- **`index.tsx`**: Centralized routing configuration.

### **`shared`**

Contains shared resources used across the application.

- **`components/`**: Common reusable components.
    - `MainPanel.tsx`, `PageLoader.tsx`, `SidePanel.tsx`
- **`constants/`**: Application-wide constants.
    - `oidcConfig.ts`: Configuration for OIDC authentication.
    - `pathNames.ts`: Predefined path names for routes.
- **`hooks/`**: Shared hooks.
    - `index.tsx`: Entry point for exporting shared hooks.
- **`Lib/`**: Shared libraries.
    - `sonar.ts`: Utility library for Sonar integration.
- **`locales/`**: Application-wide localization files.
    - `ar.ts`, `en.ts`, `index.ts`: Localization configuration and resources.
- **`models/`**: Shared TypeScript types and enums.
    - **`enums/`**: Shared enums.
    - **`interfaces/`**: Shared interfaces.
- **`services/`**: Shared service files.
    - `http.ts`: HTTP client setup.
- **`store/`**: Shared state management. (Zustand)
    - `middleware.ts`, `usePanelStore.ts`
- **`utils/`**: Shared utility functions.
    - `lazyRetry.ts`: Retry logic for lazy-loaded components.

---

This structure is designed to keep the project modular, scalable, and maintainable, with clear separation of concerns.
