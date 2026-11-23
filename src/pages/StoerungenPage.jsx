import { useEffect, useRef, useState } from "react";
import {
    Box,
    Typography,
    Table,
    TableHead,
    TableBody,
    TableRow,
    TableCell,
    TableContainer,
    Paper,
    Chip,
    Button,
    MenuItem,
    TextField,
    Stack,
    Pagination,
} from "@mui/material";
import { getPlcs } from "../api/plcApi";
import {
    getAllFaults,
    acknowledgeFault,
    startFaultProgress,
    resolveFault,
} from "../api/faultEventApi";

const POLLING_INTERVAL_MS = 5000;
const PAGE_SIZE = 25;

const STATUSES = ["Offen", "Quittiert", "In Bearbeitung", "Erledigt"];

const STATUS_COLORS = {
    Offen: { bg: "rgba(220,38,38,0.12)", fg: "#DC2626" },
    Quittiert: { bg: "rgba(217,119,6,0.12)", fg: "#D97706" },
    "In Bearbeitung": { bg: "rgba(8,145,178,0.12)", fg: "#0891B2" },
    Erledigt: { bg: "rgba(22,163,74,0.12)", fg: "#16A34A" },
};

// Prochaine action possible selon le statut courant — null = aucune
// (Erledigt est un état final)
function nextAction(status) {
    switch (status) {
        case "Offen":
            return { label: "Quittieren", handler: acknowledgeFault };
        case "Quittiert":
            return { label: "Bearbeitung starten", handler: startFaultProgress };
        case "In Bearbeitung":
            return { label: "Als erledigt markieren", handler: resolveFault };
        default:
            return null;
    }
}

function StoerungenPage() {
    const [faults, setFaults] = useState([]);
    const [totalCount, setTotalCount] = useState(0);
    const [page, setPage] = useState(1);
    const [plcs, setPlcs] = useState([]);
    const [plcFilter, setPlcFilter] = useState("");
    const [statusFilter, setStatusFilter] = useState("");
    const pollingRef = useRef(null);

    useEffect(() => {
        getPlcs().then(setPlcs).catch(console.error);
    }, []);

    const loadFaults = async () => {
        try {
            const result = await getAllFaults(plcFilter || null, statusFilter || null, page, PAGE_SIZE);
            setFaults(result.items);
            setTotalCount(result.totalCount);
        } catch (error) {
            console.error(error);
        }
    };

    // Revenir à la page 1 quand un filtre change — une page 3 peut ne plus
    // exister une fois le filtre appliqué.
    useEffect(() => {
        setPage(1);
    }, [plcFilter, statusFilter]);

    useEffect(() => {
        loadFaults();
        pollingRef.current = setInterval(loadFaults, POLLING_INTERVAL_MS);
        return () => clearInterval(pollingRef.current);
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [plcFilter, statusFilter, page]);

    const handleAction = async (fault) => {
        const action = nextAction(fault.status);
        if (!action) return;
        try {
            await action.handler(fault.id);
            await loadFaults();
        } catch (error) {
            console.error(error);
        }
    };

    const pageCount = Math.max(1, Math.ceil(totalCount / PAGE_SIZE));

    return (
        <Box>
            <Typography variant="h5" sx={{ mb: 3 }}>
                Fehlerbehandlung
            </Typography>

            <Stack direction="row" spacing={2} sx={{ mb: 2.5 }}>
                <TextField
                    select
                    label="PLC"
                    value={plcFilter}
                    onChange={(e) => setPlcFilter(e.target.value)}
                    size="small"
                    sx={{ minWidth: 200 }}
                >
                    <MenuItem value="">Alle PLCs</MenuItem>
                    {plcs.map((p) => (
                        <MenuItem key={p.id} value={p.id}>
                            {p.name}
                        </MenuItem>
                    ))}
                </TextField>

                <TextField
                    select
                    label="Status"
                    value={statusFilter}
                    onChange={(e) => setStatusFilter(e.target.value)}
                    size="small"
                    sx={{ minWidth: 180 }}
                >
                    <MenuItem value="">Alle Status</MenuItem>
                    {STATUSES.map((s) => (
                        <MenuItem key={s} value={s}>
                            {s}
                        </MenuItem>
                    ))}
                </TextField>
            </Stack>

            {faults.length === 0 ? (
                <Paper variant="outlined" sx={{ p: 4, textAlign: "center" }}>
                    <Typography color="text.secondary">Keine Stoerungen gefunden.</Typography>
                </Paper>
            ) : (
                <>
                    <TableContainer component={Paper} variant="outlined">
                        <Table size="small">
                            <TableHead>
                                <TableRow>
                                    <TableCell>PLC</TableCell>
                                    <TableCell>Variable</TableCell>
                                    <TableCell>Code</TableCell>
                                    <TableCell>Aufgetreten</TableCell>
                                    <TableCell>Status</TableCell>
                                    <TableCell align="right">Aktion</TableCell>
                                </TableRow>
                            </TableHead>
                            <TableBody>
                                {faults.map((f) => {
                                    const colors = STATUS_COLORS[f.status] ?? STATUS_COLORS.Offen;
                                    const action = nextAction(f.status);

                                    return (
                                        <TableRow key={f.id} hover>
                                            <TableCell sx={{ fontWeight: 500 }}>{f.plcName}</TableCell>
                                            <TableCell sx={{ color: "text.secondary" }}>{f.variableName}</TableCell>
                                            <TableCell sx={{ fontFamily: '"Roboto Mono", monospace' }}>
                                                {f.faultCode}
                                            </TableCell>
                                            <TableCell sx={{ color: "text.secondary" }}>
                                                {new Date(f.occurredAt).toLocaleString("de-DE")}
                                            </TableCell>
                                            <TableCell>
                                                <Chip
                                                    size="small"
                                                    label={f.status}
                                                    sx={{ backgroundColor: colors.bg, color: colors.fg, fontWeight: 600 }}
                                                />
                                                {f.status === "Erledigt" && f.clearedByUsername && (
                                                    <Typography variant="caption" color="text.secondary" sx={{ display: "block", mt: 0.25 }}>
                                                        von {f.clearedByUsername}
                                                    </Typography>
                                                )}
                                            </TableCell>
                                            <TableCell align="right">
                                                {action && (
                                                    <Button size="small" variant="outlined" onClick={() => handleAction(f)}>
                                                        {action.label}
                                                    </Button>
                                                )}
                                            </TableCell>
                                        </TableRow>
                                    );
                                })}
                            </TableBody>
                        </Table>
                    </TableContainer>

                    <Stack direction="row" justifyContent="space-between" alignItems="center" sx={{ mt: 2 }}>
                        <Typography variant="caption" color="text.secondary">
                            {totalCount} Stoerung {totalCount > 1 ? "en" : ""} Insgesamt
                        </Typography>
                        {pageCount > 1 && (
                            <Pagination
                                count={pageCount}
                                page={page}
                                onChange={(e, val) => setPage(val)}
                                size="small"
                            />
                        )}
                    </Stack>
                </>
            )}
        </Box>
    );
}

export default StoerungenPage;


/*
import { useEffect, useRef, useState } from "react";
import {
    Box,
    Typography,
    Table,
    TableHead,
    TableBody,
    TableRow,
    TableCell,
    TableContainer,
    Paper,
    Chip,
    Button,
    MenuItem,
    TextField,
    Stack,
} from "@mui/material";
import { getPlcs } from "../api/plcApi";
import {
    getAllFaults,
    acknowledgeFault,
    startFaultProgress,
    resolveFault,
} from "../api/faultEventApi";

const POLLING_INTERVAL_MS = 5000;

const STATUSES = ["Offen", "Quittiert", "In Bearbeitung", "Erledigt"];

const STATUS_COLORS = {
    Offen: { bg: "rgba(220,38,38,0.12)", fg: "#DC2626" },
    Quittiert: { bg: "rgba(217,119,6,0.12)", fg: "#D97706" },
    "In Bearbeitung": { bg: "rgba(8,145,178,0.12)", fg: "#0891B2" },
    Erledigt: { bg: "rgba(22,163,74,0.12)", fg: "#16A34A" },
};

// Prochaine action possible selon le statut courant — null = aucune
// (Erledigt est un état final)
function nextAction(status) {
    switch (status) {
        case "Offen":
            return { label: "Quittieren", handler: acknowledgeFault };
        case "Quittiert":
            return { label: "Bearbeitung starten", handler: startFaultProgress };
        case "In Bearbeitung":
            return { label: "Als erledigt markieren", handler: resolveFault };
        default:
            return null;
    }
}

function StoerungenPage() {
    const [faults, setFaults] = useState([]);
    const [plcs, setPlcs] = useState([]);
    const [plcFilter, setPlcFilter] = useState("");
    const [statusFilter, setStatusFilter] = useState("");
    const pollingRef = useRef(null);

    useEffect(() => {
        getPlcs().then(setPlcs).catch(console.error);
    }, []);

    const loadFaults = async () => {
        try {
            const data = await getAllFaults(plcFilter || null, statusFilter || null);
            setFaults(data);
        } catch (error) {
            console.error(error);
        }
    };

    useEffect(() => {
        loadFaults();
        pollingRef.current = setInterval(loadFaults, POLLING_INTERVAL_MS);
        return () => clearInterval(pollingRef.current);
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [plcFilter, statusFilter]);

    const handleAction = async (fault) => {
        const action = nextAction(fault.status);
        if (!action) return;
        try {
            await action.handler(fault.id);
            await loadFaults();
        } catch (error) {
            console.error(error);
        }
    };

    return (
        <Box>
            <Typography variant="h5" sx={{ mb: 3 }}>
                Fehlerbehandlung
            </Typography>

            <Stack direction="row" spacing={2} sx={{ mb: 2.5 }}>
                <TextField
                    select
                    label="PLC"
                    value={plcFilter}
                    onChange={(e) => setPlcFilter(e.target.value)}
                    size="small"
                    sx={{ minWidth: 200 }}
                >
                    <MenuItem value="">Alle PLCs</MenuItem>
                    {plcs.map((p) => (
                        <MenuItem key={p.id} value={p.id}>
                            {p.name}
                        </MenuItem>
                    ))}
                </TextField>

                <TextField
                    select
                    label="Status"
                    value={statusFilter}
                    onChange={(e) => setStatusFilter(e.target.value)}
                    size="small"
                    sx={{ minWidth: 180 }}
                >
                    <MenuItem value="">Alle Status</MenuItem>
                    {STATUSES.map((s) => (
                        <MenuItem key={s} value={s}>
                            {s}
                        </MenuItem>
                    ))}
                </TextField>
            </Stack>

            {faults.length === 0 ? (
                <Paper variant="outlined" sx={{ p: 4, textAlign: "center" }}>
                    <Typography color="text.secondary">Keine Stoerungen gefunden.</Typography>
                </Paper>
            ) : (
                <TableContainer component={Paper} variant="outlined">
                    <Table size="small">
                        <TableHead>
                            <TableRow>
                                <TableCell>PLC</TableCell>
                                <TableCell>Variable</TableCell>
                                <TableCell>Code</TableCell>
                                <TableCell>Aufgetreten</TableCell>
                                <TableCell>Status</TableCell>
                                <TableCell align="right">Aktion</TableCell>
                            </TableRow>
                        </TableHead>
                        <TableBody>
                            {faults.map((f) => {
                                const colors = STATUS_COLORS[f.status] ?? STATUS_COLORS.Offen;
                                const action = nextAction(f.status);

                                return (
                                    <TableRow key={f.id} hover>
                                        <TableCell sx={{ fontWeight: 500 }}>{f.plcName}</TableCell>
                                        <TableCell sx={{ color: "text.secondary" }}>{f.variableName}</TableCell>
                                        <TableCell sx={{ fontFamily: '"Roboto Mono", monospace' }}>
                                            {f.faultCode}
                                        </TableCell>
                                        <TableCell sx={{ color: "text.secondary" }}>
                                            {new Date(f.occurredAt).toLocaleString("de-DE")}
                                        </TableCell>
                                        <TableCell>
                                            <Chip
                                                size="small"
                                                label={f.status}
                                                sx={{ backgroundColor: colors.bg, color: colors.fg, fontWeight: 600 }}
                                            />
                                            {f.status === "Erledigt" && f.clearedByUsername && (
                                                <Typography variant="caption" color="text.secondary" sx={{ display: "block", mt: 0.25 }}>
                                                    von {f.clearedByUsername}
                                                </Typography>
                                            )}
                                        </TableCell>
                                        <TableCell align="right">
                                            {action && (
                                                <Button size="small" variant="outlined" onClick={() => handleAction(f)}>
                                                    {action.label}
                                                </Button>
                                            )}
                                        </TableCell>
                                    </TableRow>
                                );
                            })}
                        </TableBody>
                    </Table>
                </TableContainer>
            )}
        </Box>
    );
}

export default StoerungenPage;
*/