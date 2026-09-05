import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  Chip,
  Box,
  Typography,
} from "@mui/material";

// groupsById : { [id]: name } — construit dans PlcPage à partir de getGroups()
function PlcTable({ plcs, groupsById = {} }) {
  if (plcs.length === 0) {
    return (
      <Paper variant="outlined" sx={{ p: 4, textAlign: "center" }}>
        <Typography color="text.secondary">Keine PLCs konfiguriert.</Typography>
      </Paper>
    );
  }

  return (
    <TableContainer component={Paper} variant="outlined">
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell>ID</TableCell>
            <TableCell>Name</TableCell>
            <TableCell>Status</TableCell>
            <TableCell>Gruppe</TableCell>
          </TableRow>
        </TableHead>

        <TableBody>
          {plcs.map((plc) => (
            <TableRow key={plc.id} hover>
              <TableCell sx={{ fontFamily: '"Roboto Mono", monospace', color: "text.secondary" }}>
                {plc.id}
              </TableCell>

              <TableCell sx={{ fontWeight: 500 }}>{plc.name}</TableCell>

              <TableCell>
                <Chip
                  size="small"
                  label={plc.enabled ? "Aktiv" : "Inaktiv"}
                  sx={{
                    backgroundColor: plc.enabled ? "rgba(22,163,74,0.12)" : "rgba(100,116,139,0.12)",
                    color: plc.enabled ? "success.main" : "text.secondary",
                  }}
                />
              </TableCell>

              <TableCell>
                {plc.groupId != null ? (
                  groupsById[plc.groupId] ?? (
                    <Box component="span" sx={{ fontFamily: "monospace" }}>
                      #{plc.groupId}
                    </Box>
                  )
                ) : (
                  <Typography component="span" color="text.secondary">
                    —
                  </Typography>
                )}
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}

export default PlcTable;
