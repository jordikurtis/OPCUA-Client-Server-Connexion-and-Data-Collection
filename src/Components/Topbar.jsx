import { useLocation } from "react-router-dom";
import { AppBar, Toolbar, IconButton, Typography, Box } from "@mui/material";
import MenuIcon from "@mui/icons-material/Menu";

const PAGE_TITLES = {
  "/": "Dashboard",
  "/plcs": "PLCs",
  "/variables": "Variablen",
  "/measurements": "Messwerte",
};

function Topbar({ onMenuClick }) {
  const location = useLocation();
  const title = PAGE_TITLES[location.pathname] ?? "OPCUA Platform";

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
      </Toolbar>
    </AppBar>
  );
}

export default Topbar;
