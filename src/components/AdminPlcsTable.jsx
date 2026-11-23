import { useNavigate } from "react-router-dom";
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
  Tooltip,
} from "@mui/material";
import EditOutlinedIcon from "@mui/icons-material/EditOutlined";
import DeleteOutlineIcon from "@mui/icons-material/DeleteOutlined";
import TuneOutlinedIcon from "@mui/icons-material/TuneOutlined";

// groupsById : { [id]: name } pour afficher le nom du groupe plutôt que l'ID
function AdminPlcsTable({ plcs, groupsById, onEdit, onDelete }) {
  const navigate = useNavigate();

  if (plcs.length === 0) {
    return (
      <Paper variant="outlined" sx={{ p: 4, textAlign: "center" }}>
        <Typography color="text.secondary">Keine PLCs vorhanden.</Typography>
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
            <TableCell align="right">Aktionen</TableCell>
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
                  label={plc.enabled ? "Aktiviert" : "Deaktiviert"}
                  sx={{
                    backgroundColor: plc.enabled ? "rgba(22,163,74,0.12)" : "rgba(100,116,139,0.12)",
                    color: plc.enabled ? "success.main" : "text.secondary",
                  }}
                />
              </TableCell>
              <TableCell>
                {plc.groupId != null ? groupsById[plc.groupId] ?? `#${plc.groupId}` : "—"}
              </TableCell>
              <TableCell align="right">
                <Stack direction="row" spacing={0.5} justifyContent="flex-end">
                  <Tooltip title="Variablen verwalten">
                    <IconButton size="small" onClick={() => navigate(`/admin/plcs/${plc.id}`)}>
                      <TuneOutlinedIcon fontSize="small" />
                    </IconButton>
                  </Tooltip>
                  <IconButton size="small" onClick={() => onEdit(plc)}>
                    <EditOutlinedIcon fontSize="small" />
                  </IconButton>
                  <IconButton size="small" onClick={() => onDelete(plc)}>
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

export default AdminPlcsTable;
