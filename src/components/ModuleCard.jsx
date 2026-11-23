import { useState } from "react";
import { Card, CardContent, Typography, Box, Stack, Chip, IconButton, Tooltip } from "@mui/material";
import ShowChartIcon from "@mui/icons-material/ShowChart";
import HistoryChartDialog from "./HistoryChartDialog";

// variables : liste des MonitoredVariableDto pour ce node
// measurementsByVariableId : { [variableId]: { value, status, timestamp } }
function ModuleCard({ nodeName, variables, measurementsByVariableId }) {
  const [historyVariable, setHistoryVariable] = useState(null);

  return (
    <Card variant="outlined">
      <CardContent>
        <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 1.5 }}>
          {nodeName}
        </Typography>

        <Stack spacing={1.25}>
          {variables.map((v) => {
            const measurement = measurementsByVariableId[v.id];
            const isGood = measurement?.status?.toLowerCase() === "good";

            return (
              <Box
                key={v.id}
                sx={{
                  display: "flex",
                  alignItems: "center",
                  justifyContent: "space-between",
                  gap: 1,
                }}
              >
                <Typography variant="body2" color="text.secondary" noWrap>
                  {v.variableName}
                </Typography>

                <Stack direction="row" spacing={0.5} alignItems="center">
                  <Typography
                    variant="body2"
                    sx={{ fontFamily: '"Roboto Mono", monospace', fontWeight: 500 }}
                  >
                    {measurement ? measurement.value : "—"}
                  </Typography>
                  {measurement && (
                    <Chip
                      size="small"
                      label={measurement.status}
                      sx={{
                        height: 18,
                        fontSize: 11,
                        backgroundColor: isGood ? "rgba(22,163,74,0.12)" : "rgba(217,119,6,0.12)",
                        color: isGood ? "success.main" : "warning.main",
                      }}
                    />
                  )}
                  <Tooltip title="Verlauf anzeigen">
                    <IconButton size="small" onClick={() => setHistoryVariable(v)}>
                      <ShowChartIcon fontSize="inherit" />
                    </IconButton>
                  </Tooltip>
                </Stack>
              </Box>
            );
          })}
        </Stack>
      </CardContent>

      <HistoryChartDialog
        open={historyVariable != null}
        variable={historyVariable}
        onClose={() => setHistoryVariable(null)}
      />
    </Card>
  );
}

export default ModuleCard;
