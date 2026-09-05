import { useEffect, useState } from "react";
import { Grid } from "@mui/material";
import MemoryOutlinedIcon from "@mui/icons-material/MemoryOutlined";
import TuneOutlinedIcon from "@mui/icons-material/TuneOutlined";
import GroupWorkOutlinedIcon from "@mui/icons-material/GroupWorkOutlined";
import TimelineOutlinedIcon from "@mui/icons-material/TimelineOutlined";
import StatsCard from "../components/StatsCard";
import { getGroups } from "../api/groupApi";
import { getPlcs } from "../api/plcApi";
import { getVariables } from "../api/variableApi";
import { getLatestMeasurements } from "../api/measurementApi";

function Dashboard() {
  const [stats, setStats] = useState({
    plcs: null,
    variables: null,
    groups: null,
    measurements: null,
  });

  useEffect(() => {
    const loadStats = async () => {
      // Chaque appel est indépendant : si l'un échoue (ex: DB pas
      // encore peuplée pour une ressource), les autres stats
      // s'affichent quand même au lieu de tout bloquer.
      const [plcs, variables, groups, measurements] = await Promise.allSettled([
        getPlcs(),
        getVariables(),
        getGroups(),
        getLatestMeasurements(),
      ]);

      setStats({
        plcs: plcs.status === "fulfilled" ? plcs.value.length : "—",
        variables: variables.status === "fulfilled" ? variables.value.length : "—",
        groups: groups.status === "fulfilled" ? groups.value.length : "—",
        measurements: measurements.status === "fulfilled" ? measurements.value.length : "—",
      });
    };

    loadStats();
  }, []);

  const cards = [
    { title: "PLCs", value: stats.plcs, icon: <MemoryOutlinedIcon />, color: "#0891B2" },
    { title: "Überwachte Variablen", value: stats.variables, icon: <TuneOutlinedIcon />, color: "#7C3AED" },
    { title: "Maschinengruppen", value: stats.groups, icon: <GroupWorkOutlinedIcon />, color: "#D97706" },
    { title: "Aktive Messwerte", value: stats.measurements, icon: <TimelineOutlinedIcon />, color: "#16A34A" },
  ];

  return (
    <Grid container spacing={2.5}>
      {cards.map((card) => (
        <Grid item xs={12} sm={6} md={3} key={card.title}>
          <StatsCard
            title={card.title}
            value={card.value ?? "…"}
            icon={card.icon}
            accentColor={card.color}
          />
        </Grid>
      ))}
    </Grid>
  );
}

export default Dashboard;
