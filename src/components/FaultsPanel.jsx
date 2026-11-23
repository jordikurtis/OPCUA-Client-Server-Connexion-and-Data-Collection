import { Box, Typography, Paper, Stack, Chip, Button } from "@mui/material";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutlined";
import CheckCircleOutlineIcon from "@mui/icons-material/CheckCircleOutlined";
import DoneIcon from "@mui/icons-material/Done";

// faults : liste de FaultEventDto actifs (voir PlcDetailPage, poll 5s)
// onAcknowledge : (faultId) => void — appelé au clic sur "Quittieren"
function FaultsPanel({ faults, onAcknowledge }) {
  if (faults.length === 0) {
    return (
      <Paper
        variant="outlined"
        sx={{
          p: 2,
          mb: 3,
          display: "flex",
          alignItems: "center",
          gap: 1,
          borderColor: "success.main",
          backgroundColor: "rgba(22,163,74,0.06)",
        }}
      >
        <CheckCircleOutlineIcon color="success" fontSize="small" />
        <Typography variant="body2" color="success.main" sx={{ fontWeight: 500 }}>
          Keine Aktiven Stoerungen
        </Typography>
      </Paper>
    );
  }

  return (
    <Paper
      variant="outlined"
      sx={{
        p: 2,
        mb: 3,
        borderColor: "error.main",
        backgroundColor: "rgba(220,38,38,0.05)",
      }}
    >
      <Stack direction="row" alignItems="center" spacing={1} sx={{ mb: 1.5 }}>
        <ErrorOutlineIcon color="error" fontSize="small" />
        <Typography variant="subtitle2" color="error.main">
          {faults.length} Aktive Stoerung{faults.length > 1 ? "en" : ""}
        </Typography>
      </Stack>

      <Stack spacing={1}>
        {faults.map((f) => (
          <Box
            key={f.id}
            sx={{
              display: "flex",
              alignItems: "center",
              justifyContent: "space-between",
              gap: 1,
              py: 0.5,
            }}
          >
            <Box>
              <Typography variant="body2" sx={{ fontWeight: 500 }}>
                {f.variableName}
              </Typography>
              <Typography variant="caption" color="text.secondary">
                Seit {new Date(f.occurredAt).toLocaleTimeString("de-DE")}
                {f.acknowledgedAt && " · Quittiert"}
              </Typography>
            </Box>

            <Stack direction="row" spacing={1} alignItems="center">
              <Chip
                size="small"
                label={`Code ${f.faultCode}`}
                sx={{
                  fontFamily: '"Roboto Mono", monospace',
                  backgroundColor: "rgba(220,38,38,0.12)",
                  color: "error.main",
                }}
              />
              {f.acknowledgedAt ? (
                <Chip
                  size="small"
                  icon={<DoneIcon sx={{ fontSize: 14 }} />}
                  label="Quittiert"
                  variant="outlined"
                  color="success"
                />
              ) : onAcknowledge ? (
                <Button
                  size="small"
                  variant="outlined"
                  onClick={() => onAcknowledge(f.id)}
                >
                  Quittieren
                </Button>
              ) : null}
            </Stack>
          </Box>
        ))}
      </Stack>
    </Paper>
  );
}

export default FaultsPanel;
