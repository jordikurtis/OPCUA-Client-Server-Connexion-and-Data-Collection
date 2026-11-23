import { Paper, Stack, Typography } from "@mui/material";
import FiberManualRecordIcon from "@mui/icons-material/FiberManualRecord";

// status : { plcName, lastSeen, isOnline } | null (pas de groupe, ou pas
// encore chargé — le composant ne rend rien dans ce cas)
function ConnectionStatusBar({ status }) {
    if (!status) return null;

    return (
        <Paper
            variant="outlined"
            sx={{
                p: 1.5,
                mb: 2,
                display: "flex",
                alignItems: "center",
                gap: 1,
            }}
        >
            <FiberManualRecordIcon
                sx={{ fontSize: 12, color: status.isOnline ? "success.main" : "error.main" }}
            />
            <Typography variant="body2" sx={{ fontWeight: 600 }}>
                {status.isOnline ? "Online" : "Offline"}
            </Typography>
            {status.lastSeen && (
                <Typography variant="body2" color="text.secondary">
                    - Zuletzt gesehen: {new Date(status.lastSeen).toLocaleTimeString("de-DE")}
                </Typography>
            )}
        </Paper>
    );
}

export default ConnectionStatusBar;
