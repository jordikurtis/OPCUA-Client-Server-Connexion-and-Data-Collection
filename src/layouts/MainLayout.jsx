import { useState } from "react";
import { Outlet } from "react-router-dom";
import { Box } from "@mui/material";
import Sidebar, { DRAWER_WIDTH } from "../components/Sidebar";
import Topbar from "../components/Topbar";

function MainLayout() {
  const [mobileOpen, setMobileOpen] = useState(false);

  return (
    <Box sx={{ display: "flex", minHeight: "100vh" }}>
      <Sidebar mobileOpen={mobileOpen} onClose={() => setMobileOpen(false)} />

      <Box
        sx={{
          flexGrow: 1,
          minWidth: 0, // évite l'overflow du contenu (tableaux larges)
          width: { md: `calc(100% - ${DRAWER_WIDTH}px)` },
        }}
      >
        <Topbar onMenuClick={() => setMobileOpen(true)} />

        <Box sx={{ p: { xs: 2, md: 4 } }}>
          <Outlet />
        </Box>
      </Box>
    </Box>
  );
}

export default MainLayout;
