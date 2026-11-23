import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  IconButton,
  Typography,
  Stack,
  Chip,
} from "@mui/material";
import EditOutlinedIcon from "@mui/icons-material/EditOutlined";
import DeleteOutlineIcon from "@mui/icons-material/DeleteOutlined";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutlined";

function AdminVariablesTable({ variables, onEdit, onDelete }) {
  if (variables.length === 0) {
    return (
      <Paper variant="outlined" sx={{ p: 4, textAlign: "center" }}>
        <Typography color="text.secondary">Keine Variablen konfiguriert.</Typography>
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
            <TableCell>NodeId</TableCell>
            <TableCell>Datentyp</TableCell>
            <TableCell>Status</TableCell>
            <TableCell align="right">Aktionen</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {variables.map((v) => (
            <TableRow key={v.id} hover>
              <TableCell sx={{ color: "text.secondary" }}>
                <Stack direction="row" spacing={0.5} alignItems="center">
                  {v.isFaultIndicator && (
                    <ErrorOutlineIcon fontSize="inherit" color="error" titleAccess="Fehlerindikator" />
                  )}
                  <span>{v.nodeName}</span>
                </Stack>
              </TableCell>
              <TableCell sx={{ fontWeight: 500 }}>{v.variableName}</TableCell>
              <TableCell sx={{ fontFamily: '"Roboto Mono", monospace', fontSize: 13 }}>
                {v.nodeId}
              </TableCell>
              <TableCell sx={{ color: "text.secondary" }}>{v.dataType}</TableCell>
              <TableCell>
                <Chip
                  size="small"
                  label={v.enabled ? "Aktiviert" : "Deaktiviert"}
                  sx={{
                    backgroundColor: v.enabled ? "rgba(22,163,74,0.12)" : "rgba(100,116,139,0.12)",
                    color: v.enabled ? "success.main" : "text.secondary",
                  }}
                />
              </TableCell>
              <TableCell align="right">
                <Stack direction="row" spacing={0.5} justifyContent="flex-end">
                  <IconButton size="small" onClick={() => onEdit(v)}>
                    <EditOutlinedIcon fontSize="small" />
                  </IconButton>
                  <IconButton size="small" onClick={() => onDelete(v)}>
                    <DeleteOutlineIcon fontSize="small" color="error" />
                  </IconButton>
                </Stack>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}

export default AdminVariablesTable;
