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
} from "@mui/material";

import EditOutlinedIcon from "@mui/icons-material/EditOutlined";
import DeleteOutlineIcon from "@mui/icons-material/DeleteOutlined";

function AdminGroupsTable({ groups, onEdit, onDelete }) {
    if (groups.length === 0) {
        return (
            <Paper variant="outlined" sx={{ p: 4, textAlign: "center" }}>
                <Typography color="text.secondary">Keine Gruppen vorhanden.</Typography>
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
                        <TableCell>Beschreibung</TableCell>
                        <TableCell align="right">Aktionen</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {groups.map((g) => (
                        <TableRow key={g.id} hover>
                            <TableCell sx={{ fontFamily: '"Roboto Mono", monospace', color: "text.secondary" }}>
                                {g.id}
                            </TableCell>
                            <TableCell sx={{ fontWeight: 500 }}>{g.name}</TableCell>
                            <TableCell sx={{ color: "text.secondary" }}>{g.description || "—"}</TableCell>
                            <TableCell align="right">
                                <Stack direction="row" spacing={0.5} justifyContent="flex-end">
                                    <IconButton size="small" onClick={() => onEdit(g)}>
                                        <EditOutlinedIcon fontSize="small" />
                                    </IconButton>
                                    <IconButton size="small" onClick={() => onDelete(g)}>
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

export default AdminGroupsTable;
