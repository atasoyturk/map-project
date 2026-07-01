import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import "ol/ol.css";          // ol css is a global css
import './index.css'
import App from './App.tsx'

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
