import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { ThemeProvider, CssBaseline } from "@mui/material";
import theme from "./theme/theme";
import "./index.css";
import App from "./App.jsx";

    createRoot(document.getElementById("root")).render(
    < ThemeProvider theme = { theme } >
      <CssBaseline />
      <App />
    </ThemeProvider >
  </StrictMode >
);