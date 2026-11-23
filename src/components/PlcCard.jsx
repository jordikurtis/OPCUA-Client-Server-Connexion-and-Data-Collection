import { useNavigate } from "react-router-dom";
import { Card, CardActionArea, Box, Typography, Chip, Stack } from "@mui/material";
import MemoryOutlinedIcon from "@mui/icons-material/MemoryOutlined";
import ChevronRightIcon from "@mui/icons-material/ChevronRight";
import FiberManualRecordIcon from "@mui/icons-material/FiberManualRecord";
import ErrorOutlineIcon from "@mui/icons-material/ErrorOutlined";

// liveStatus : { plcName, lastSeen, isOnline } | undefined
// activeFaultCount : nombre de défauts actifs pour ce PLC (0 ou undefined = aucun)
function PlcCard({ plc, groupName, liveStatus, activeFaultCount }) {
  const navigate = useNavigate();
  const hasStatus = liveStatus != null;
  const isOnline = liveStatus?.isOnline ?? false;
  const hasFaults = (activeFaultCount ?? 0) > 0;

  return (
    <Card sx={{ height: "100%", position: "relative" }}>
      {hasFaults && (
        <Chip
          size="small"
          icon={<ErrorOutlineIcon sx={{ fontSize: 14 }} />}
          label={activeFaultCount}
          sx={{
            position: "absolute",
            top: 8,
            right: 8,
            height: 22,
            backgroundColor: "error.main",
            color: "#fff",
            fontWeight: 700,
            "& .MuiChip-icon": { color: "#fff" },
          }}
        />
      )}

      <CardActionArea
        onClick={() => navigate(`/plcs/${plc.id}`)}
        sx={{ height: "100%", p: 2.5, display: "flex", flexDirection: "column", alignItems: "stretch" }}
      >
        <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", mb: 1.5 }}>
          <Box
            sx={{
              width: 40,
              height: 40,
              borderRadius: 2,
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              backgroundColor: "rgba(8,145,178,0.1)",
              color: "primary.main",
            }}
          >
            <MemoryOutlinedIcon />
          </Box>
          <ChevronRightIcon sx={{ color: "text.secondary" }} />
        </Box>

        <Box sx={{ display: "flex", alignItems: "center", gap: 0.75, mb: 0.5 }}>
          {hasStatus && (
            <FiberManualRecordIcon
              sx={{ fontSize: 10, color: isOnline ? "success.main" : "error.main" }}
            />
          )}
          <Typography variant="subtitle1" sx={{ fontWeight: 600 }}>
            {plc.name}
          </Typography>
        </Box>

        <Stack direction="row" spacing={1} sx={{ mt: 1, flexWrap: "wrap", rowGap: 1 }}>
          {hasStatus && (
            <Chip
              size="small"
              label={isOnline ? "Online" : "Offline"}
              sx={{
                backgroundColor: isOnline ? "rgba(22,163,74,0.12)" : "rgba(220,38,38,0.12)",
                color: isOnline ? "success.main" : "error.main",
              }}
            />
          )}
          <Chip
            size="small"
            variant="outlined"
            label={plc.enabled ? "Konfiguriert" : "Deaktiviert"}
          />
          {groupName && <Chip size="small" variant="outlined" label={groupName} />}
        </Stack>
      </CardActionArea>
    </Card>
  );
}

export default PlcCard;
