import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { ReactQueryDevtools } from '@tanstack/react-query-devtools'
import { Suspense, useEffect, useLayoutEffect } from 'react'
import { createBrowserRouter, RouterProvider } from 'react-router-dom'
import { useTranslation } from 'react-i18next'
import appRoutes from './routes'
import { PageLoader } from './shared/components/PageLoader'
import './shared/i18n' // Initialize i18n
import './assets/css/shared.css'
import './shared/styles/modal-fixes.css' // Modal positioning fixes
import './shared/styles/popup-base.css' // Consolidated popup styles

function App() {
    const { i18n } = useTranslation()
    const currentLang = i18n.language
    const dir = currentLang === 'ar' ? 'rtl' : 'ltr'

    const router = createBrowserRouter(appRoutes, {
        future: {
            v7_startTransition: true,
        },
    })
    const queryClient = new QueryClient({
        defaultOptions: {
            queries: {
                retry: false,
                refetchOnWindowFocus: false,
                refetchOnMount: false,
                retryOnMount: false,
            },
        },
    })

    useLayoutEffect(() => {
        document.dir = dir
        document.documentElement.lang = currentLang
        if (navigator.userAgent.indexOf('iPhone') > -1) {
            document?.querySelector('[name=viewport]')?.setAttribute('content', 'width=device-width, initial-scale=1, maximum-scale=1')
        }
    }, [dir, currentLang])

    useEffect(() => {
        // Set language in localStorage for persistence
        localStorage.setItem('i18nextLng', currentLang)
    }, [currentLang])

    // Global modal positioning fix
    useEffect(() => {
        const applyModalFixes = () => {
            // Find all dialog overlays and fix them
            const overlays = document.querySelectorAll('[data-radix-portal] > div[data-state="open"], [data-radix-dialog-overlay]');
            overlays.forEach((overlay) => {
                const element = overlay as HTMLElement;
                element.style.setProperty('position', 'fixed', 'important');
                element.style.setProperty('inset', '0', 'important');
                element.style.setProperty('display', 'flex', 'important');
                element.style.setProperty('align-items', 'center', 'important');
                element.style.setProperty('justify-content', 'center', 'important');
                element.style.setProperty('z-index', '9999', 'important');
            });

            // Find all dialog content and fix them
            const dialogs = document.querySelectorAll('[role="dialog"], [data-radix-dialog-content]');
            dialogs.forEach((dialog) => {
                const element = dialog as HTMLElement;
                element.style.setProperty('position', 'relative', 'important');
                element.style.setProperty('left', 'auto', 'important');
                element.style.setProperty('right', 'auto', 'important');
                element.style.setProperty('top', 'auto', 'important');
                element.style.setProperty('bottom', 'auto', 'important');
                element.style.setProperty('transform', 'none', 'important');
                element.style.setProperty('margin', 'auto', 'important');
            });
        };

        // Apply fixes on DOM mutations
        const observer = new MutationObserver(applyModalFixes);
        observer.observe(document.body, { childList: true, subtree: true });

        // Apply fixes on initial load
        applyModalFixes();

        return () => observer.disconnect();
    }, []);

    return (
        <QueryClientProvider client={queryClient}>
            <Suspense fallback={<PageLoader />}>
                <RouterProvider router={router} />
            </Suspense>
            <ReactQueryDevtools initialIsOpen={false} />
        </QueryClientProvider>
    )
}

export default App
