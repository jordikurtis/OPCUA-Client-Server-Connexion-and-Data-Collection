import { useEffect, useState } from "react";
import { Box } from "@mui/material";
import { getVariables } from "../api/variableApi";
import VariableTable from "../components/VariableTable";

function VariablesPage() {
  const [variables, setVariables] = useState([]);

  useEffect(() => {
    const loadData = async () => {
      try {
        const data = await getVariables();
        setVariables(data);
      } catch (error) {
        console.error(error);
      }
    };

    loadData();
  }, []);

  return (
    <Box>
      <VariableTable variables={variables} />
    </Box>
  );
}

export default VariablesPage;
