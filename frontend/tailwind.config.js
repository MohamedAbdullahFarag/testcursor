import tailwinds from 'mada-design-system/tailwind'

//export const presets = [tailwinds]
export const darkMode = ['class']
export const content = ['./src/**/*.{ts,tsx}', './node_modules/mada-design-system/*.{html,js,ts,jsx,tsx}']

export const theme = {
  ...tailwinds.theme,
  extend: {
    ...tailwinds.theme?.extend,
    zIndex: {
      ...tailwinds.theme?.extend?.zIndex,
      'modal': '9999',
      'tooltip': '10000',
      'popup': '9999',
    },
    maxHeight: {
      ...tailwinds.theme?.extend?.maxHeight,
      'modal': '90vh',
      'popup': '90vh',
    },
    backdropBlur: {
      ...tailwinds.theme?.extend?.backdropBlur,
      'modal': '4px',
      'popup': '4px',
    },
    transitionDuration: {
      ...tailwinds.theme?.extend?.transitionDuration,
      'popup': '200ms',
    },
    transitionTimingFunction: {
      ...tailwinds.theme?.extend?.transitionTimingFunction,
      'popup': 'cubic-bezier(0.4, 0, 0.2, 1)',
    },
    animation: {
      ...tailwinds.theme?.extend?.animation,
      'popup-in': 'popup-fade-in 200ms cubic-bezier(0.4, 0, 0.2, 1)',
      'popup-out': 'popup-fade-out 200ms cubic-bezier(0.4, 0, 0.2, 1)',
    },
    keyframes: {
      ...tailwinds.theme?.extend?.keyframes,
      'popup-fade-in': {
        '0%': { opacity: '0', transform: 'scale(0.95)' },
        '100%': { opacity: '1', transform: 'scale(1)' },
      },
      'popup-fade-out': {
        '0%': { opacity: '1', transform: 'scale(1)' },
        '100%': { opacity: '0', transform: 'scale(0.95)' },
      },
    },
  }
}
export const plugins = tailwinds.plugins
