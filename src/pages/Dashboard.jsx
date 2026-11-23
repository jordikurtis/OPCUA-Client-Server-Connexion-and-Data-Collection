import { useEffect, useState } from "react";
import { Box } from "@mui/material";
import MemoryOutlinedIcon from "@mui/icons-material/MemoryOutlined";
import TuneOutlinedIcon from "@mui/icons-material/TuneOutlined";
import GroupWorkOutlinedIcon from "@mui/icons-material/GroupWorkOutlined";
import TimelineOutlinedIcon from "@mui/icons-material/TimelineOutlined";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutlined";
import StatsCard from "../components/StatsCard";
import { getGroups } from "../api/groupApi";
import { getPlcs } from "../api/plcApi";
import { getVariables } from "../api/variableApi";
import { getLatestMeasurements } from "../api/measurementApi";
import { getAllActiveFaults } from "../api/faultEventApi";

function Dashboard() {
  const [stats, setStats] = useState({
    plcs: null,
    variables: null,
    groups: null,
    measurements: null,
    activeFaults: null,
  });

  useEffect(() => {
    const loadStats = async () => {
      const [plcs, variables, groups, measurements, faults] = await Promise.allSettled([
        getPlcs(),
        getVariables(),
        getGroups(),
        getLatestMeasurements(),
        getAllActiveFaults(),
      ]);

      setStats({
        plcs: plcs.status === "fulfilled" ? plcs.value.length : "—",
        variables: variables.status === "fulfilled" ? variables.value.length : "—",
        groups: groups.status === "fulfilled" ? groups.value.length : "—",
        measurements: measurements.status === "fulfilled" ? measurements.value.length : "—",
        activeFaults: faults.status === "fulfilled" ? faults.value.length : "—",
      });
    };

    loadStats();
    // Défauts actifs : pertinent de rafraîchir régulièrement (pas juste au
    // chargement), contrairement aux autres compteurs plus statiques.
    const interval = setInterval(loadStats, 10000);
    return () => clearInterval(interval);
  }, []);

  const cards = [
    { title: "PLCs", value: stats.plcs, icon: <MemoryOutlinedIcon />, color: "#0891B2" },
    { title: "Überwachte Variablen", value: stats.variables, icon: <TuneOutlinedIcon />, color: "#7C3AED" },
    { title: "Maschinengruppen", value: stats.groups, icon: <GroupWorkOutlinedIcon />, color: "#D97706" },
    { title: "Aktive Messwerte", value: stats.measurements, icon: <TimelineOutlinedIcon />, color: "#16A34A" },
    { title: "Aktive Störungen", value: stats.activeFaults, icon: <ErrorOutlineIcon />, color: "#DC2626" },
  ];

  return (
    <Box
      sx={{
        display: "grid",
        gap: 2.5,
        gridTemplateColumns: {
          xs: "1fr",
          sm: "repeat(2, 1fr)",
          md: "repeat(3, 1fr)",
          lg: "repeat(5, 1fr)",
        },
      }}
    >
      {cards.map((card) => (
        <StatsCard
          key={card.title}
          title={card.title}
          value={card.value ?? "…"}
          icon={card.icon}
          accentColor={card.color}
        />
      ))}
    </Box>
  );
}

export default Dashboard;
