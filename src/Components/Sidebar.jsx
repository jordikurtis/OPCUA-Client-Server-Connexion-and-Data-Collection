import { NavLink } from "react-router-dom";
import {
  Drawer,
  Box,
  Typography,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Divider,
} from "@mui/material";
import DashboardOutlinedIcon from "@mui/icons-material/DashboardOutlined";
import MemoryOutlinedIcon from "@mui/icons-material/MemoryOutlined";
import TuneOutlinedIcon from "@mui/icons-material/TuneOutlined";
import TimelineOutlinedIcon from "@mui/icons-material/TimelineOutlined";

export const DRAWER_WIDTH = 240;

const navItems = [
  { label: "Dashboard", to: "/", icon: <DashboardOutlinedIcon /> },
  { label: "PLCs", to: "/plcs", icon: <MemoryOutlinedIcon /> },
  { label: "Variablen", to: "/variables", icon: <TuneOutlinedIcon /> },
  { label: "Messwerte", to: "/measurements", icon: <TimelineOutlinedIcon /> },
];

function SidebarContent() {
  return (
    <Box sx={{ height: "100%", display: "flex", flexDirection: "column" }}>
      <Box sx={{ px: 3, py: 3 }}>
        <Typography variant="h6" sx={{ color: "#FFFFFF", fontWeight: 700 }}>
          OPCUA Platform
        </Typography>
        <Typography variant="caption" sx={{ color: "#94A3B8" }}>
          Tenneco Kolben Ring
        </Typography>
      </Box>

      <Divider sx={{ borderColor: "#1E293B" }} />

      <List sx={{ px: 1.5, py: 2, flex: 1 }}>
        {navItems.map((item) => (
          <ListItemButton
            key={item.to}
            component={NavLink}
            to={item.to}
            end={item.to === "/"}
            sx={{
              borderRadius: 2,
              mb: 0.5,
              color: "#CBD5E1",
              "& .MuiListItemIcon-root": { color: "#94A3B8", minWidth: 40 },
              "&.active": {
                backgroundColor: "rgba(8, 145, 178, 0.16)",
                color: "#67E8F9",
                "& .MuiListItemIcon-root": { color: "#67E8F9" },
              },
              "&:hover": {
                backgroundColor: "rgba(255,255,255,0.05)",
              },
            }}
          >
            <ListItemIcon>{item.icon}</ListItemIcon>
            <ListItemText
              primary={item.label}
              primaryTypographyProps={{ fontSize: 14, fontWeight: 500 }}
            />
          </ListItemButton>
        ))}
      </List>
    </Box>
  );
}

function Sidebar({ mobileOpen, onClose }) {
  return (
    <>
      {/* Desktop : sidebar permanente */}
      <Drawer
        variant="permanent"
        sx={{
          display: { xs: "none", md: "block" },
          width: DRAWER_WIDTH,
          flexShrink: 0,
          "& .MuiDrawer-paper": {
            width: DRAWER_WIDTH,
            boxSizing: "border-box",
            backgroundColor: "#0F172A",
            borderRight: "none",
          },
        }}
        open
      >
        <SidebarContent />
      </Drawer>

      {/* Mobile : sidebar temporaire (tiroir) */}
      <Drawer
        variant="temporary"
        open={mobileOpen}
        onClose={onClose}
        ModalProps={{ keepMounted: true }}
        sx={{
          display: { xs: "block", md: "none" },
          "& .MuiDrawer-paper": {
            width: DRAWER_WIDTH,
            boxSizing: "border-box",
            backgroundColor: "#0F172A",
          },
        }}
      >
        <SidebarContent />
      </Drawer>
    </>
  );
}

export default Sidebar;
