import { createContext, useContext, useMemo, useState } from "react";
import { ThemeProvider, CssBaseline } from "@mui/material";
import { createAppTheme } from "../theme/theme";

const ThemeModeContext = createContext(null);
const STORAGE_KEY = "opcua_theme_mode";

function loadStoredMode() {
    try {
        return localStorage.getItem(STORAGE_KEY) || "light";
    } catch {
        return "light";
    }
}

// Englobe ThemeProvider/CssBaseline ici (pas dans main.jsx) pour pouvoir
// recalculer le thème MUI à chaque changement de mode via useState.
export function ThemeModeProvider({ children }) {
    const [mode, setMode] = useState(loadStoredMode);

    const toggleMode = () => {
        setMode((prev) => {
            const next = prev === "light" ? "dark" : "light";
            try {
                localStorage.setItem(STORAGE_KEY, next);
            } catch {
                // ignore — pas de persistance possible, le mode reste en mémoire
            }
            return next;
        });
    };

    const theme = useMemo(() => createAppTheme(mode), [mode]);

    return (
        <ThemeModeContext.Provider value={{ mode, toggleMode }}>
            <ThemeProvider theme={theme}>
                <CssBaseline />
                {children}
            </ThemeProvider>
        </ThemeModeContext.Provider>
    );
}

export function useThemeMode() {
    const ctx = useContext(ThemeModeContext);
    if (!ctx) throw new Error("useThemeMode must be used within ThemeModeProvider");
    return ctx;
}

