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

function VariableTable({ variables }) {
  if (variables.length === 0) {
    return (
      <Paper variant="outlined" sx={{ p: 4, textAlign: "center" }}>
        <Typography color="text.secondary">Keine überwachten Variablen.</Typography>
      </Paper>
    );
  }

  return (
    <TableContainer component={Paper} variant="outlined">
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell>ID</TableCell>
            <TableCell>Node</TableCell>
            <TableCell>Variable</TableCell>
            <TableCell>Status</TableCell>
          </TableRow>
        </TableHead>

        <TableBody>
          {variables.map((v) => (
            <TableRow key={v.id} hover>
              <TableCell sx={{ fontFamily: '"Roboto Mono", monospace', color: "text.secondary" }}>
                {v.id}
              </TableCell>

              <TableCell sx={{ color: "text.secondary" }}>{v.nodeName}</TableCell>

              <TableCell sx={{ fontWeight: 500 }}>{v.variableName}</TableCell>

              <TableCell>
                <Chip
                  size="small"
                  label={v.enabled ? "Aktiv" : "Inaktiv"}
                  sx={{
                    backgroundColor: v.enabled ? "rgba(22,163,74,0.12)" : "rgba(100,116,139,0.12)",
                    color: v.enabled ? "success.main" : "text.secondary",
                  }}
                />
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}

export default VariableTable;
