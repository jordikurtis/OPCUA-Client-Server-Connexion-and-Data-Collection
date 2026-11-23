import { useEffect, useState } from "react";
import { Box, Typography } from "@mui/material";
import { getPlcs } from "../api/plcApi";
import { getGroups } from "../api/groupApi";
import { getAllActiveFaults } from "../api/faultEventApi";
import PlcCard from "../components/PlcCard";

function PlcPage() {
  const [plcs, setPlcs] = useState([]);
  const [groupsById, setGroupsById] = useState({});
  const [faultCountByPlcId, setFaultCountByPlcId] = useState({});

  useEffect(() => {
    const loadData = async () => {
      try {
        const [plcsData, groupsData, faultsData] = await Promise.all([
          getPlcs(),
          getGroups(),
          getAllActiveFaults(),
        ]);

        setPlcs(plcsData);

        const groupMap = {};
        groupsData.forEach((g) => {
          groupMap[g.id] = g.name;
        });
        setGroupsById(groupMap);

        // Un seul appel pour tous les PLCs — évite N requêtes (une par carte)
        const faultCounts = {};
        faultsData.forEach((f) => {
          faultCounts[f.plcId] = (faultCounts[f.plcId] ?? 0) + 1;
        });
        setFaultCountByPlcId(faultCounts);
      } catch (error) {
        console.error(error);
      }
    };

    loadData();
  }, []);

  if (plcs.length === 0) {
    return <Typography color="text.secondary">Keine PLCs konfiguriert.</Typography>;
  }

  return (
    <Box
      sx={{
        display: "grid",
        gap: 2.5,
        gridTemplateColumns: {
          xs: "1fr",
          sm: "repeat(2, 1fr)",
          md: "repeat(3, 1fr)",
          lg: "repeat(4, 1fr)",
        },
      }}
    >
      {plcs.map((plc) => (
        <PlcCard
          key={plc.id}
          plc={plc}
          groupName={groupsById[plc.groupId]}
          activeFaultCount={faultCountByPlcId[plc.id]}
        />
      ))}
    </Box>
  );
}

export default PlcPage;
