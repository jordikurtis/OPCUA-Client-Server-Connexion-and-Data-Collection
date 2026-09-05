import { createTheme } from "@mui/material/styles";

// ============================================================
// Thème OPCUA Platform
// Vocabulaire visuel "salle de contrôle industrielle" :
// - accent technique unique (cyan-bleu), pas le bleu MUI par défaut
// - vert/rouge/ambre réservés au SENS (Online/Offline/Warning),
//   jamais utilisés comme couleur décorative ailleurs
// - police monospace pour les valeurs numériques/techniques
//   (NodeId, valeurs de mesure, timestamps) — lisibilité type
//   afficheur d'instrument, pas un choix esthétique gratuit
// ============================================================

const theme = createTheme({
  palette: {
    mode: "light",
    primary: {
      main: "#0891B2", // cyan-700 — accent technique
      dark: "#0E7490",
      light: "#67E8F9",
      contrastText: "#FFFFFF",
    },
    success: { main: "#16A34A" }, // Online
    error: { main: "#DC2626" },   // Offline / Fehler
    warning: { main: "#D97706" }, // Warning
    background: {
      default: "#F4F6F8",
      paper: "#FFFFFF",
    },
    text: {
      primary: "#0F172A",
      secondary: "#64748B",
    },
    divider: "#E2E8F0",
    grey: {
      900: "#0F172A", // fond sidebar
      800: "#1E293B",
      700: "#334155",
      100: "#F1F5F9",
    },
  },
  typography: {
    fontFamily: '"Inter", "Segoe UI", Roboto, sans-serif',
    h4: { fontWeight: 600, letterSpacing: "-0.02em" },
    h5: { fontWeight: 600, letterSpacing: "-0.01em" },
    h6: { fontWeight: 600 },
    subtitle2: { color: "#64748B", fontWeight: 500 },
    button: { textTransform: "none", fontWeight: 600 },
  },
  shape: { borderRadius: 10 },
  components: {
    MuiCard: {
      styleOverrides: {
        root: {
          boxShadow: "none",
          border: "1px solid #E2E8F0",
        },
      },
    },
    MuiButton: {
      styleOverrides: {
        root: { borderRadius: 8 },
      },
    },
    MuiChip: {
      styleOverrides: {
        root: { fontWeight: 600 },
      },
    },
    MuiTableCell: {
      styleOverrides: {
        head: {
          fontWeight: 600,
          fontSize: "0.75rem",
          color: "#64748B",
          borderBottom: "2px solid #E2E8F0",
        },
      },
    },
  },
});

// Classe utilitaire pour les valeurs techniques (import via sx={{ fontFamily: theme.custom.mono }})
theme.custom = {
  mono: '"Roboto Mono", ui-monospace, Consolas, monospace',
};

export default theme;
