import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Chip,
  Typography,
} from "@mui/material";

// variablesById : { [id]: { variableName, nodeName } } — construit dans MeasurementsPage
function MeasurementTable({ measurements, variablesById = {} }) {
  if (measurements.length === 0) {
    return (
      <Paper variant="outlined" sx={{ p: 4, textAlign: "center" }}>
        <Typography color="text.secondary">Noch keine Messwerte empfangen.</Typography>
      </Paper>
    );
  }

  return (
    <TableContainer component={Paper} variant="outlined">
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell>Node</TableCell>
            <TableCell>Variable</TableCell>
            <TableCell>Wert</TableCell>
            <TableCell>Status</TableCell>
            <TableCell>Zeitstempel</TableCell>
          </TableRow>
        </TableHead>

        <TableBody>
          {measurements.map((m) => {
            const variable = variablesById[m.variableId];
            const isGood = m.status?.toLowerCase() === "good";

            return (
              <TableRow key={m.variableId} hover>
                <TableCell sx={{ color: "text.secondary" }}>
                  {variable?.nodeName ?? "—"}
                </TableCell>

                <TableCell sx={{ fontWeight: 500 }}>
                  {variable?.variableName ?? `#${m.variableId}`}
                </TableCell>

                <TableCell sx={{ fontFamily: '"Roboto Mono", monospace' }}>
                  {m.value}
                </TableCell>

                <TableCell>
                  <Chip
                    size="small"
                    label={m.status}
                    sx={{
                      backgroundColor: isGood ? "rgba(22,163,74,0.12)" : "rgba(217,119,6,0.12)",
                      color: isGood ? "success.main" : "warning.main",
                    }}
                  />
                </TableCell>

                <TableCell sx={{ fontFamily: '"Roboto Mono", monospace', color: "text.secondary" }}>
                  {new Date(m.timestamp).toLocaleTimeString("de-DE")}
                </TableCell>
              </TableRow>
            );
          })}
        </TableBody>
      </Table>
    </TableContainer>
  );
}

export default MeasurementTable;
