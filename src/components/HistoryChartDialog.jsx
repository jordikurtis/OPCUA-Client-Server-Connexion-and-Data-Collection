import { useEffect, useState } from "react";
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Button,
    Box,
    Typography,
    ToggleButtonGroup,
    ToggleButton,
    CircularProgress,
    Table,
    TableBody,
    TableCell,
    TableRow,
} from "@mui/material";
import {
    LineChart,
    Line,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    ResponsiveContainer,
} from "recharts";
import DownloadOutlinedIcon from "@mui/icons-material/DownloadOutlined";
import { getMeasurementHistory, getAllMeasurementHistory } from "../api/measurementApi";

// Nécessite recharts : npm install recharts

// Pas de dépendance supplémentaire — un CSV se construit très bien à la
// main pour 3 colonnes, et ça marche pour n'importe quel type de valeur
// (numérique, booléen, texte), contrairement au graphique.
function buildCsv(points) {
    const header = "Zeitstempel;Wert;Status\n";
    const rows = points
        .map((p) => `${p.timestamp};${p.value};${p.status ?? ""}`)
        .join("\n");
    return "\uFEFF" + header + rows; // BOM UTF-8 pour Excel (trèmas)
}

function downloadCsv(content, variableName) {
    const blob = new Blob([content], { type: "text/csv;charset=utf-8;" });
    const url = URL.createObjectURL(blob);

    const a = document.createElement("a");
    a.href = url;
    a.download = `${variableName || "verlauf"}_${new Date().toISOString().slice(0, 19)}.csv`;
    a.click();
    URL.revokeObjectURL(url);
}

const RANGES = [
    { label: "1h", hours: 1 },
    { label: "6h", hours: 6 },
    { label: "24h", hours: 24 },
    { label: "7T", hours: 24 * 7 },
];

// variable : { id, variableName, nodeName } — la variable dont on veut
// voir l'historique
function HistoryChartDialog({ open, variable, onClose }) {
    const [rangeHours, setRangeHours] = useState(24);
    const [points, setPoints] = useState([]);
    const [totalCount, setTotalCount] = useState(0);
    const [hasMore, setHasMore] = useState(false);
    const [loading, setLoading] = useState(false);
    const [exporting, setExporting] = useState(false);
    const [isNumeric, setIsNumeric] = useState(true);

    useEffect(() => {
        if (!open || !variable) return;

        const load = async () => {
            setLoading(true);
            try {
                const to = new Date();
                const from = new Date(to.getTime() - rangeHours * 60 * 60 * 1000);

                // Page 1 seulement pour l'affichage — un SPS réel en continu peut
                // générer bien plus de points qu'un graphique n'en a besoin pour
                // rester lisible. Le CSV (export complet) boucle sur toutes les
                // pages séparément, voir handleExport.
                const result = await getMeasurementHistory(
                    variable.id,
                    from.toISOString(),
                    to.toISOString(),
                    1,
                    2000
                );

                // Certaines variables (Boolean, String) ne se prêtent pas à un
                // graphique de courbe — on détecte et on retombe sur une liste.
                const parsed = result.items.map((m) => ({
                    ...m,
                    // Normalise la virgule décimale (format allemand, ex: "0,378")
                    // avant de parser — sinon Number() échoue et bascule à tort en liste.
                    numericValue: Number(String(m.value).replace(",", ".")),
                    timeLabel: new Date(m.timestamp).toLocaleString("de-DE", {
                        day: "2-digit",
                        month: "2-digit",
                        hour: "2-digit",
                        minute: "2-digit",
                    }),
                }));

                const allNumeric = parsed.every((p) => !Number.isNaN(p.numericValue));
                setIsNumeric(allNumeric);
                setPoints(parsed);
                setTotalCount(result.totalCount);
                setHasMore(result.hasMore);
            } catch (error) {
                console.error(error);
                setPoints([]);
                setTotalCount(0);
                setHasMore(false);
            } finally {
                setLoading(false);
            }
        };

        load();
    }, [open, variable, rangeHours]);

    const handleExport = async () => {
        if (!variable) return;
        setExporting(true);
        try {
            const to = new Date();
            const from = new Date(to.getTime() - rangeHours * 60 * 60 * 1000);

            // Export complet — contrairement au graphique, le CSV ne doit pas
            // être tronqué : on récupère toutes les pages de la plage.
            const allPoints = hasMore
                ? await getAllMeasurementHistory(variable.id, from.toISOString(), to.toISOString())
                : points;

            downloadCsv(buildCsv(allPoints), variable.variableName);
        } catch (error) {
            console.error(error);
        } finally {
            setExporting(false);
        }
    };

    return (
        <Dialog open={open} onClose={onClose} fullWidth maxWidth="md">
            <DialogTitle>
                Verlauf — {variable?.variableName}
                {variable?.nodeName && (
                    <Typography variant="body2" color="text.secondary">
                        {variable.nodeName}
                    </Typography>
                )}
            </DialogTitle>
            <DialogContent>
                <ToggleButtonGroup
                    value={rangeHours}
                    exclusive
                    size="small"
                    onChange={(e, val) => val !== null && setRangeHours(val)}
                    sx={{ mb: 2 }}
                >
                    {RANGES.map((r) => (
                        <ToggleButton key={r.hours} value={r.hours}>
                            {r.label}
                        </ToggleButton>
                    ))}
                </ToggleButtonGroup>

                {hasMore && !loading && (
                    <Typography variant="caption" color="text.secondary" sx={{ display: "block", mb: 1.5 }}>
                        Anzeige der ersten {points.length} von {totalCount} Datenpunkten — für die
                        vollständige Ansicht Zeitraum verkleinern. Der CSV-Export enthält immer alle Daten.
                    </Typography>
                )}

                {loading ? (
                    <Box sx={{ display: "flex", justifyContent: "center", py: 6 }}>
                        <CircularProgress size={28} />
                    </Box>
                ) : points.length === 0 ? (
                    <Typography color="text.secondary" sx={{ py: 4, textAlign: "center" }}>
                        Keine Daten für diesen Zeitraum.
                    </Typography>
                ) : isNumeric ? (
                    <Box sx={{ height: 320 }}>
                        <ResponsiveContainer width="100%" height="100%">
                            <LineChart data={points}>
                                <CartesianGrid strokeDasharray="3 3" stroke="#E2E8F0" />
                                <XAxis dataKey="timeLabel" tick={{ fontSize: 11 }} minTickGap={30} />
                                <YAxis tick={{ fontSize: 11 }} />
                                <Tooltip
                                    formatter={(value) => [value, variable?.variableName]}
                                    labelFormatter={(label) => label}
                                />
                                <Line
                                    type="monotone"
                                    dataKey="numericValue"
                                    stroke="#0891B2"
                                    dot={false}
                                    strokeWidth={2}
                                />
                            </LineChart>
                        </ResponsiveContainer>
                    </Box>
                ) : (
                    // Repli liste — variable non numérique (Boolean/String)
                    <Table size="small">
                        <TableBody>
                            {points.slice(0, 100).map((p) => (
                                <TableRow key={p.timestamp}>
                                    <TableCell sx={{ color: "text.secondary" }}>{p.timeLabel}</TableCell>
                                    <TableCell sx={{ fontFamily: '"Roboto Mono", monospace' }}>{p.value}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                )}
            </DialogContent>
            <DialogActions>
                <Button
                    startIcon={<DownloadOutlinedIcon />}
                    onClick={handleExport}
                    disabled={points.length === 0 || exporting}
                >
                    {exporting ? "Exportiere…" : "CSV exportieren"}
                </Button>
                <Button onClick={onClose}>Schließen</Button>
            </DialogActions>
        </Dialog>
    );
}

export default HistoryChartDialog;


/*
import { useEffect, useState } from "react";
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Button,
    Box,
    Typography,
    ToggleButtonGroup,
    ToggleButton,
    CircularProgress,
    Table,
    TableBody,
    TableCell,
    TableRow,
} from "@mui/material";
import {
    LineChart,
    Line,
    XAxis,
    YAxis,
    CartesianGrid,
    Tooltip,
    ResponsiveContainer,
} from "recharts";
import DownloadOutlinedIcon from "@mui/icons-material/DownloadOutlined";
import { getMeasurementHistory } from "../api/measurementApi";

// Nécessite recharts : npm install recharts

// Pas de dépendance supplémentaire — un CSV se construit très bien à la
// main pour 3 colonnes, et ça marche pour n'importe quel type de valeur
// (numérique, booléen, texte), contrairement au graphique.
function exportToCsv(points, variableName) {
    const header = "Zeitstempel;Wert;Status\n";
    const rows = points
        .map((p) => `${p.timestamp};${p.value};${p.status ?? ""}`)
        .join("\n");

    // BOM UTF-8 pour qu'Excel affiche correctement les trèmas
    const blob = new Blob(["\uFEFF" + header + rows], { type: "text/csv;charset=utf-8;" });
    const url = URL.createObjectURL(blob);

    const a = document.createElement("a");
    a.href = url;
    a.download = `${variableName || "verlauf"}_${new Date().toISOString().slice(0, 19)}.csv`;
    a.click();
    URL.revokeObjectURL(url);
}

const RANGES = [
    { label: "1h", hours: 1 },
    { label: "6h", hours: 6 },
    { label: "24h", hours: 24 },
    { label: "7T", hours: 24 * 7 },
];

// variable : { id, variableName, nodeName } — la variable dont on veut
// voir l'historique
function HistoryChartDialog({ open, variable, onClose }) {
    const [rangeHours, setRangeHours] = useState(24);
    const [points, setPoints] = useState([]);
    const [loading, setLoading] = useState(false);
    const [isNumeric, setIsNumeric] = useState(true);

    useEffect(() => {
        if (!open || !variable) return;

        const load = async () => {
            setLoading(true);
            try {
                const to = new Date();
                const from = new Date(to.getTime() - rangeHours * 60 * 60 * 1000);

                const data = await getMeasurementHistory(
                    variable.id,
                    from.toISOString(),
                    to.toISOString()
                );

                // Certaines variables (Boolean, String) ne se prêtent pas à un
                // graphique de courbe — on détecte et on retombe sur une liste.
                const parsed = data.map((m) => ({
                    ...m,
                    // Normalise la virgule décimale (format allemand, ex: "0,378")
                    // avant de parser — sinon Number() échoue et bascule à tort en liste.
                    numericValue: Number(String(m.value).replace(",", ".")),
                    timeLabel: new Date(m.timestamp).toLocaleString("de-DE", {
                        day: "2-digit",
                        month: "2-digit",
                        hour: "2-digit",
                        minute: "2-digit",
                    }),
                }));

                const allNumeric = parsed.every((p) => !Number.isNaN(p.numericValue));
                setIsNumeric(allNumeric);
                setPoints(parsed);
            } catch (error) {
                console.error(error);
                setPoints([]);
            } finally {
                setLoading(false);
            }
        };

        load();
    }, [open, variable, rangeHours]);

    return (
        <Dialog open={open} onClose={onClose} fullWidth maxWidth="md">
            <DialogTitle>
                Verlauf — {variable?.variableName}
                {variable?.nodeName && (
                    <Typography variant="body2" color="text.secondary">
                        {variable.nodeName}
                    </Typography>
                )}
            </DialogTitle>
            <DialogContent>
                <ToggleButtonGroup
                    value={rangeHours}
                    exclusive
                    size="small"
                    onChange={(e, val) => val !== null && setRangeHours(val)}
                    sx={{ mb: 2 }}
                >
                    {RANGES.map((r) => (
                        <ToggleButton key={r.hours} value={r.hours}>
                            {r.label}
                        </ToggleButton>
                    ))}
                </ToggleButtonGroup>

                {loading ? (
                    <Box sx={{ display: "flex", justifyContent: "center", py: 6 }}>
                        <CircularProgress size={28} />
                    </Box>
                ) : points.length === 0 ? (
                    <Typography color="text.secondary" sx={{ py: 4, textAlign: "center" }}>
                        Keine Daten für diesen Zeitraum.
                    </Typography>
                ) : isNumeric ? (
                    <Box sx={{ height: 320 }}>
                        <ResponsiveContainer width="100%" height="100%">
                            <LineChart data={points}>
                                <CartesianGrid strokeDasharray="3 3" stroke="#E2E8F0" />
                                <XAxis dataKey="timeLabel" tick={{ fontSize: 11 }} minTickGap={30} />
                                <YAxis tick={{ fontSize: 11 }} />
                                <Tooltip
                                    formatter={(value) => [value, variable?.variableName]}
                                    labelFormatter={(label) => label}
                                />
                                <Line
                                    type="monotone"
                                    dataKey="numericValue"
                                    stroke="#0891B2"
                                    dot={false}
                                    strokeWidth={2}
                                />
                            </LineChart>
                        </ResponsiveContainer>
                    </Box>
                ) : (
                    // Repli liste — variable non numérique (Boolean/String)
                    <Table size="small">
                        <TableBody>
                            {points.slice(0, 100).map((p) => (
                                <TableRow key={p.timestamp}>
                                    <TableCell sx={{ color: "text.secondary" }}>{p.timeLabel}</TableCell>
                                    <TableCell sx={{ fontFamily: '"Roboto Mono", monospace' }}>{p.value}</TableCell>
                                </TableRow>
                            ))}
                        </TableBody>
                    </Table>
                )}
            </DialogContent>
            <DialogActions>
                <Button
                    startIcon={<DownloadOutlinedIcon />}
                    onClick={() => exportToCsv(points, variable?.variableName)}
                    disabled={points.length === 0}
                >
                    CSV exportieren
                </Button>
                <Button onClick={onClose}>Schließen</Button>
            </DialogActions>
        </Dialog>
    );
}

export default HistoryChartDialog;
*/



