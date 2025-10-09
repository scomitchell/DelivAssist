// import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import "bootstrap/dist/css/bootstrap.min.css";
import { ThemeProvider, CssBaseline, createTheme } from '@mui/material';

const theme = createTheme({
  palette: {
    primary: { main: '#1976d2' },   // blue
    secondary: { main: '#FFC107' }, // orange
  },
})

createRoot(document.getElementById('root')!).render(
    <ThemeProvider theme={theme}>
        <CssBaseline />
        <App />
    </ThemeProvider>
)
