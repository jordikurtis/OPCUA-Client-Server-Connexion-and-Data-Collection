import { useEffect, useRef, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { Box, Typography, IconButton, Chip, Breadcrumbs, Link as MuiLink, Button, Stack } from "@mui/material";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import HistoryIcon from "@mui/icons-material/History";
import { getPlcById, getPlcVariables } from "../api/plcApi";
import { getLatestMeasurements } from "../api/measurementApi";
import { getActiveFaults, acknowledgeFault } from "../api/faultEventApi";
import { getMachineStatusByGroup } from "../api/machineApi";
import { useAuth } from "../context/AuthContext";
import ModuleCard from "../components/ModuleCard";
import FaultsPanel from "../components/FaultsPanel";
import HistoryChartDialog from "../components/HistoryChartDialog";
import ConnectionStatusBar from "../components/ConnectionStatusBar";

const POLLING_INTERVAL_MS = 5000;

function PlcDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { role } = useAuth();
  const canAcknowledge = role === "Admin" || role === "Instandhaltung";

  const [plc, setPlc] = useState(null);
  const [variables, setVariables] = useState([]);
  const [measurementsByVariableId, setMeasurementsByVariableId] = useState({});
  const [activeFaults, setActiveFaults] = useState([]);
  const [historyOpen, setHistoryOpen] = useState(false);
  const [connectionStatus, setConnectionStatus] = useState(null);
  const statusPollingRef = useRef(null);
  const pollingRef = useRef(null);
  const faultsPollingRef = useRef(null);

  useEffect(() => {
    const loadStaticData = async () => {
      try {
        const [plcData, variablesData] = await Promise.all([
          getPlcById(id),
          getPlcVariables(id),
        ]);
        setPlc(plcData);
        setVariables(variablesData);
      } catch (error) {
        console.error(error);
      }
    };

    loadStaticData();
  }, [id]);

  useEffect(() => {
    const loadMeasurements = async () => {
      try {
        const data = await getLatestMeasurements();
        const map = {};
        data.forEach((m) => {
          map[m.variableId] = { value: m.value, status: m.status, timestamp: m.timestamp };
        });
        setMeasurementsByVariableId(map);
      } catch (error) {
        console.error(error);
      }
    };

    loadMeasurements();
    pollingRef.current = setInterval(loadMeasurements, POLLING_INTERVAL_MS);

    return () => clearInterval(pollingRef.current);
  }, []);

  const loadFaults = async () => {
    try {
      const data = await getActiveFaults(id);
      setActiveFaults(data);
    } catch (error) {
      console.error(error);
    }
  };

  useEffect(() => {
    loadFaults();
    faultsPollingRef.current = setInterval(loadFaults, POLLING_INTERVAL_MS);

    return () => clearInterval(faultsPollingRef.current);
  }, [id]);

  // Statut Online/Offline — dépend du groupe du PLC (l'API calcule ça par
  // groupe). Ne démarre qu'une fois plc chargé (besoin de plc.groupId).
  useEffect(() => {
    if (!plc?.groupId) {
      setConnectionStatus(null);
      return;
    }

    const loadStatus = async () => {
      try {
        const statuses = await getMachineStatusByGroup(plc.groupId);
        const match = statuses.find((s) => s.plcName === plc.name);
        setConnectionStatus(match ?? null);
      } catch (error) {
        console.error(error);
      }
    };

    loadStatus();
    statusPollingRef.current = setInterval(loadStatus, POLLING_INTERVAL_MS);

    return () => clearInterval(statusPollingRef.current);
  }, [plc]);

  const handleAcknowledge = async (faultId) => {
    try {
      await acknowledgeFault(faultId);
      // Rafraîchit immédiatement plutôt que d'attendre le prochain poll —
      // retour visuel instantané au clic.
      await loadFaults();
    } catch (error) {
      console.error(error);
    }
  };

  const variablesByNode = variables.reduce((acc, v) => {
    if (!acc[v.nodeName]) acc[v.nodeName] = [];
    acc[v.nodeName].push(v);
    return acc;
  }, {});

  if (!plc) return null;

  return (
    <Box>
      <Box sx={{ display: "flex", alignItems: "center", gap: 1, mb: 1 }}>
        <IconButton onClick={() => navigate("/plcs")} size="small">
          <ArrowBackIcon fontSize="small" />
        </IconButton>
        <Breadcrumbs>
          <MuiLink component="button" underline="hover" onClick={() => navigate("/plcs")}>
            PLCs
          </MuiLink>
          <Typography color="text.primary">{plc.name}</Typography>
        </Breadcrumbs>
      </Box>

      <Box sx={{ display: "flex", alignItems: "center", gap: 1.5, mb: 3 }}>
        <Typography variant="h5">{plc.name}</Typography>
        <Chip
          size="small"
          label={plc.enabled ? "Aktiv" : "Inaktiv"}
          sx={{
            backgroundColor: plc.enabled ? "rgba(22,163,74,0.12)" : "rgba(100,116,139,0.12)",
            color: plc.enabled ? "success.main" : "text.secondary",
          }}
        />
      </Box>

      <ConnectionStatusBar status={connectionStatus} />

      <Stack direction="row" justifyContent="flex-end" sx={{ mb: 1 }}>
        <Button
          size="small"
          startIcon={<HistoryIcon fontSize="small" />}
          onClick={() => setHistoryOpen(true)}
        >
          Störungsverlauf
        </Button>
      </Stack>

      <FaultsPanel faults={activeFaults} onAcknowledge={canAcknowledge ? handleAcknowledge : undefined} />

          <HistoryChartDialog
        open={historyOpen}
        plcId={id}
        onClose={() => setHistoryOpen(false)}
      />

      {Object.keys(variablesByNode).length === 0 ? (
        <Typography color="text.secondary">Keine Variablen für diese Maschine konfiguriert.</Typography>
      ) : (
        <Box
          sx={{
            display: "grid",
            gap: 2.5,
            gridTemplateColumns: {
              xs: "1fr",
              sm: "repeat(2, 1fr)",
              md: "repeat(3, 1fr)",
            },
          }}
        >
          {Object.entries(variablesByNode).map(([nodeName, nodeVariables]) => (
            <ModuleCard
              key={nodeName}
              nodeName={nodeName}
              variables={nodeVariables}
              measurementsByVariableId={measurementsByVariableId}
            />
          ))}
        </Box>
      )}
    </Box>
  );
}

export default PlcDetailPage;
