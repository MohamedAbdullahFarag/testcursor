import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import App from './App.tsx'
import './assets/css/shared.css'
import './shared/i18n' // Initialize i18n

// Suppress known warnings from external libraries
import './utils/suppressWarnings'

createRoot(document.getElementById('root')!).render(
    <StrictMode>
        <App />
    </StrictMode>,
)
