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
import { useAuth } from "../context/AuthContext";

const ROLE_COLORS = {
  Admin: { bg: "rgba(124,58,237,0.12)", fg: "#7C3AED" },
  Operator: { bg: "rgba(8,145,178,0.12)", fg: "#0891B2" },
  Instandhaltung: { bg: "rgba(217,119,6,0.12)", fg: "#D97706" },
};

function AdminUsersTable({ users, onEdit, onDelete }) {
  const { username: currentUsername } = useAuth();

  if (users.length === 0) {
    return (
      <Paper variant="outlined" sx={{ p: 4, textAlign: "center" }}>
        <Typography color="text.secondary">Keine Benutzer vorhanden.</Typography>
      </Paper>
    );
  }

  return (
    <TableContainer component={Paper} variant="outlined">
      <Table size="small">
        <TableHead>
          <TableRow>
            <TableCell>ID</TableCell>
            <TableCell>Benutzername</TableCell>
            <TableCell>Rolle</TableCell>
            <TableCell align="right">Aktionen</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {users.map((u) => {
            const colors = ROLE_COLORS[u.role] ?? ROLE_COLORS.Operator;
            const isSelf = u.username === currentUsername;

            return (
              <TableRow key={u.id} hover>
                <TableCell sx={{ fontFamily: '"Roboto Mono", monospace', color: "text.secondary" }}>
                  {u.id}
                </TableCell>
                <TableCell sx={{ fontWeight: 500 }}>
                  {u.username}
                  {isSelf && (
                    <Typography component="span" variant="caption" color="text.secondary" sx={{ ml: 1 }}>
                      (Sie)
                    </Typography>
                  )}
                </TableCell>
                <TableCell>
                  <Chip
                    size="small"
                    label={u.role}
                    sx={{ backgroundColor: colors.bg, color: colors.fg }}
                  />
                </TableCell>
                <TableCell align="right">
                  <Stack direction="row" spacing={0.5} justifyContent="flex-end">
                    <IconButton size="small" onClick={() => onEdit(u)}>
                      <EditOutlinedIcon fontSize="small" />
                    </IconButton>
                    {/* Pas de suppression de son propre compte (aussi bloqué côté API) */}
                    {!isSelf && (
                      <IconButton size="small" onClick={() => onDelete(u)}>
                        <DeleteOutlineIcon fontSize="small" color="error" />
                      </IconButton>
                    )}
                  </Stack>
                </TableCell>
              </TableRow>
            );
          })}
        </TableBody>
      </Table>
    </TableContainer>
  );
}

export default AdminUsersTable;
