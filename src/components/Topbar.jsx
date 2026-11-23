import { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { AppBar, Toolbar, IconButton, Typography, Box, Stack, Tooltip } from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";
import LogoutIcon from "@mui/icons-material/Logout";
import VpnKeyOutlinedIcon from "@mui/icons-material/VpnKeyOutlined";
import DarkModeOutlinedIcon from "@mui/icons-material/DarkModeOutlined";
import LightModeOutlinedIcon from "@mui/icons-material/LightModeOutlined";
import { useAuth } from "../context/AuthContext";
import { useThemeMode } from "../context/ThemeModeContext";
import ChangePasswordDialog from "./ChangePasswordDialog";

const PAGE_TITLES = {
    "/": "Dashboard",
    "/plcs": "PLCs",
    "/variables": "Variablen",
    "/measurements": "Messwerte",
    "/stoerungen": "Fehlerbehandlung",
    "/admin": "Admin",
};

function Topbar({ onMenuClick }) {
    const location = useLocation();
    const navigate = useNavigate();
    const { username, role, logout } = useAuth();
    const { mode, toggleMode } = useThemeMode();
    const [passwordDialogOpen, setPasswordDialogOpen] = useState(false);
    const title = PAGE_TITLES[location.pathname] ?? "OPCUA Platform";

    const handleLogout = () => {
        logout();
        navigate("/login");
    };

    return (
        <AppBar
            position="sticky"
            elevation={0}
            sx={{
                // "background.paper" du thème actif, plus "#FFFFFF" en dur —
                // sans ça la barre reste blanche même en dark mode.
                backgroundColor: "background.paper",
                color: "text.primary",
                borderBottom: "1px solid",
                borderColor: "divider",
            }}
        >
            <Toolbar>
                <IconButton
                    edge="start"
                    onClick={onMenuClick}
                    sx={{ mr: 2, display: { xs: "inline-flex", md: "none" } }}
                >
                    <MenuIcon />
                </IconButton>

                <Typography variant="h6" sx={{ fontWeight: 600 }}>
                    {title}
                </Typography>

                <Box sx={{ flexGrow: 1 }} />

                <Tooltip title={mode === "dark" ? "Light Mode" : "Dark Mode"}>
                    <IconButton size="small" onClick={toggleMode} sx={{ mr: username ? 1 : 0 }}>
                        {mode === "dark" ? (
                            <LightModeOutlinedIcon fontSize="small" />
                        ) : (
                            <DarkModeOutlinedIcon fontSize="small" />
                        )}
                    </IconButton>
                </Tooltip>

                {username && (
                    <Stack direction="row" spacing={1} alignItems="center">
                        <Typography variant="body2" color="text.secondary">
                            {username} ({role})
                        </Typography>
                        <Tooltip title="Passwort ändern">
                            <IconButton size="small" onClick={() => setPasswordDialogOpen(true)}>
                                <VpnKeyOutlinedIcon fontSize="small" />
                            </IconButton>
                        </Tooltip>
                        <Tooltip title="Abmelden">
                            <IconButton size="small" onClick={handleLogout}>
                                <LogoutIcon fontSize="small" />
                            </IconButton>
                        </Tooltip>
                    </Stack>
                )}
            </Toolbar>

            <ChangePasswordDialog
                open={passwordDialogOpen}
                onClose={() => setPasswordDialogOpen(false)}
            />
        </AppBar>
    );
}

export default Topbar;



/*import { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { AppBar, Toolbar, IconButton, Typography, Box, Stack, Tooltip } from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";
import LogoutIcon from "@mui/icons-material/Logout";
import VpnKeyOutlinedIcon from "@mui/icons-material/VpnKeyOutlined";
import { useAuth } from "../context/AuthContext";
import ChangePasswordDialog from "./ChangePasswordDialog";

const PAGE_TITLES = {
  "/": "Dashboard",
  "/plcs": "PLCs",
  "/variables": "Variablen",
  "/measurements": "Messwerte",
  "/admin": "Admin",
};

function Topbar({ onMenuClick }) {
  const location = useLocation();
  const navigate = useNavigate();
  const { username, role, logout } = useAuth();
  const [passwordDialogOpen, setPasswordDialogOpen] = useState(false);
  const title = PAGE_TITLES[location.pathname] ?? "OPCUA Platform";

  const handleLogout = () => {
    logout();
    navigate("/login");
  };

  return (
    <AppBar
      position="sticky"
      elevation={0}
      sx={{
        backgroundColor: "#FFFFFF",
        color: "text.primary",
        borderBottom: "1px solid",
        borderColor: "divider",
      }}
    >
      <Toolbar>
        <IconButton
          edge="start"
          onClick={onMenuClick}
          sx={{ mr: 2, display: { xs: "inline-flex", md: "none" } }}
        >
          <MenuIcon />
        </IconButton>

        <Typography variant="h6" sx={{ fontWeight: 600 }}>
          {title}
        </Typography>

        <Box sx={{ flexGrow: 1 }} />

        {username && (
          <Stack direction="row" spacing={1} alignItems="center">
            <Typography variant="body2" color="text.secondary">
              {username} ({role})
            </Typography>
            <Tooltip title="Passwort ändern">
              <IconButton size="small" onClick={() => setPasswordDialogOpen(true)}>
                <VpnKeyOutlinedIcon fontSize="small" />
              </IconButton>
            </Tooltip>
            <Tooltip title="Abmelden">
              <IconButton size="small" onClick={handleLogout}>
                <LogoutIcon fontSize="small" />
              </IconButton>
            </Tooltip>
          </Stack>
        )}
      </Toolbar>

      <ChangePasswordDialog
        open={passwordDialogOpen}
        onClose={() => setPasswordDialogOpen(false)}
      />
    </AppBar>
  );
}

export default Topbar;
*/
