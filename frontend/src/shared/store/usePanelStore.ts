// stores/panelStore.ts
import { create } from 'zustand'
import DevtoolsMiddlewares from './middleware'

interface PanelState {
    [routePath: string]: {
        isVisible: boolean
    }
}

interface PanelStore {
    // Stores the visibility state for each route's panel
    panels: PanelState
    // Tracks the current active route
    currentRoute: string | null
    // Controls panel visibility in mobile view (true = showing side panel, false = showing main panel)
    isShowingMobile: boolean
    // Toggles panel visibility for the current route (show/hide)
    toggleSidePanel: () => void
    // Hides the side panel for the current route
    hideSidePanel: () => void
    // Shows the side panel for the current route
    showSidePanel: () => void
    // Toggles between main and side panel views on mobile screens
    toggleMobileView: () => void
    // Updates the current route and resets mobile view
    setCurrentRoute: (route: string) => void
    // Returns whether the panel should be visible for a given route
    isSidePanelVisible: (route: string) => boolean
}

export const usePanelStore = create<PanelStore>()(
    DevtoolsMiddlewares(
        (set, get) => ({
            panels: {},
            currentRoute: null,
            isShowingMobile: false,

            toggleSidePanel: () =>
                set(
                    state => {
                        const route = state.currentRoute
                        if (!route) return state

                        const currentValue = state.panels[route]?.isVisible ?? true

                        return {
                            panels: {
                                ...state.panels,
                                [route]: {
                                    isVisible: !currentValue,
                                },
                            },
                        }
                    },
                    false,
                    'toggleSidePanel',
                ),
            hideSidePanel: () =>
                set(
                    state => {
                        const route = state.currentRoute
                        if (!route) return state

                        return {
                            panels: {
                                ...state.panels,
                                [route]: {
                                    isVisible: false,
                                },
                            },
                        }
                    },
                    false,
                    'hideSidePanel',
                ),

            showSidePanel: () =>
                set(
                    state => {
                        const route = state.currentRoute
                        if (!route) return state

                        return {
                            panels: {
                                ...state.panels,
                                [route]: {
                                    isVisible: true,
                                },
                            },
                        }
                    },
                    false,
                    'showSidePanel',
                ),

            toggleMobileView: () => set(state => ({ isShowingMobile: !state.isShowingMobile }), false, 'toggleMobileView'),

            setCurrentRoute: route => {
                const { currentRoute, panels } = get()
                if (currentRoute !== route) {
                    // If this is the first time visiting this route, set default to visible
                    if (!panels[route]) {
                        set({
                            currentRoute: route,
                            panels: {
                                ...panels,
                                [route]: {
                                    isVisible: true,
                                },
                            },
                            isShowingMobile: false,
                        })
                    } else {
                        set({
                            currentRoute: route,
                            isShowingMobile: false,
                        })
                    }
                }
            },

            isSidePanelVisible: route => {
                const state = get()
                return state.panels[route]?.isVisible ?? true // Default to true if not set
            },
        }),
        {
            name: 'panel-store',
        },
    ),
)
