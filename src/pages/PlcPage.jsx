import { useEffect, useState } from "react";
import { Box } from "@mui/material";
import { getPlcs } from "../api/plcApi";
import { getGroups } from "../api/groupApi";
import PlcTable from "../components/PlcTable";

function PlcPage() {
  const [plcs, setPlcs] = useState([]);
  const [groupsById, setGroupsById] = useState({});

  useEffect(() => {
    const loadData = async () => {
      try {
        const [plcsData, groupsData] = await Promise.all([getPlcs(), getGroups()]);

        setPlcs(plcsData);

        const map = {};
        groupsData.forEach((g) => {
          map[g.id] = g.name;
        });
        setGroupsById(map);
      } catch (error) {
        console.error(error);
      }
    };

    loadData();
  }, []);

  return (
    <Box>
      <PlcTable plcs={plcs} groupsById={groupsById} />
    </Box>
  );
}

export default PlcPage;
