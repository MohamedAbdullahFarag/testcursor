import js from '@eslint/js'
import reactPlugin from 'eslint-plugin-react'
import reactHooks from 'eslint-plugin-react-hooks'
import reactRefresh from 'eslint-plugin-react-refresh'
import globals from 'globals'
import tseslint from 'typescript-eslint'

export default tseslint.config(
    { ignores: ['dist'] },
    {
        extends: [js.configs.recommended, ...tseslint.configs.recommended],
        files: ['**/*.{ts,tsx}'],
        languageOptions: {
            ecmaVersion: 2020,
            globals: globals.browser,
        },
        plugins: {
            'react-hooks': reactHooks,
            'react-refresh': reactRefresh,
            react: reactPlugin,
        },
        rules: {
            ...reactHooks.configs.recommended.rules,
            ...reactPlugin.configs.recommended.rules,
            'react/react-in-jsx-scope': 0,
            'react/prop-types': 1, // Disable PropTypes validation in favor of TypeScript
            'react/no-unescaped-entities': 'warn', // Change to warning instead of error
            'react-refresh/only-export-components': ['warn', { allowConstantExport: true }],
            '@typescript-eslint/no-explicit-any': 'warn', // Change to warning for gradual migration
            '@typescript-eslint/no-unused-vars': ['warn', { argsIgnorePattern: '^_' }], // Allow unused vars with underscore prefix
            'no-useless-escape': 'warn', // Change to warning
            'no-useless-catch': 'warn', // Change to warning
            '@typescript-eslint/no-unused-expressions': 'warn', // Change to warning
        },
        settings: {
            react: {
                version: 'detect',
            },
        },
    },
)
