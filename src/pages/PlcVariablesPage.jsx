import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import {
  Box,
  Typography,
  Button,
  Stack,
  Alert,
  IconButton,
  Breadcrumbs,
  Link as MuiLink,
} from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import { getPlcById, getPlcVariables } from "../api/plcApi";
import { createVariable, updateVariable, deleteVariable } from "../api/variableApi";
import AdminVariablesTable from "../components/AdminVariablesTable";
import VariableFormDialog from "../components/VariableFormDialog";
import ConfirmDialog from "../components/ConfirmDialog";

function PlcVariablesPage() {
  const { id } = useParams();
  const navigate = useNavigate();

  const [plc, setPlc] = useState(null);
  const [variables, setVariables] = useState([]);
  const [error, setError] = useState(null);

  const [dialogOpen, setDialogOpen] = useState(false);
  const [editingVariable, setEditingVariable] = useState(null);
  const [confirmDelete, setConfirmDelete] = useState(null);

  const loadData = async () => {
    try {
      const [plcData, variablesData] = await Promise.all([
        getPlcById(id),
        getPlcVariables(id),
      ]);
      setPlc(plcData);
      setVariables(variablesData);
    } catch (err) {
      console.error(err);
      setError("Fehler beim Laden der Daten.");
    }
  };

  useEffect(() => {
    loadData();
  }, [id]);

  const handleSubmit = async (data) => {
    try {
      if (editingVariable) {
        await updateVariable(editingVariable.id, data);
      } else {
        // PlcName/GroupId viennent du PLC courant, pas du formulaire —
        // une variable ne peut être créée que sous un PLC déjà connu.
          await createVariable({ ...data, plcId: plc.id, groupId: plc.groupId ?? null });
      }
      setDialogOpen(false);
      setEditingVariable(null);
      await loadData();
    } catch (err) {
      console.error(err);
      setError("Fehler beim Speichern der Variable.");
    }
  };

  const handleConfirmDelete = async () => {
    try {
      await deleteVariable(confirmDelete.id);
      setConfirmDelete(null);
      await loadData();
    } catch (err) {
      console.error(err);
      const message =
        err.response?.status === 409
          ? "Loeschen nicht moeglich: Es verweisen noch Messwerte darauf."
          : "Fehler beim Loeschen.";
      setError(message);
      setConfirmDelete(null);
    }
  };

  if (!plc) return null;

  return (
    <Box>
      <Box sx={{ display: "flex", alignItems: "center", gap: 1, mb: 1 }}>
        <IconButton onClick={() => navigate("/admin")} size="small">
          <ArrowBackIcon fontSize="small" />
        </IconButton>
        <Breadcrumbs>
          <MuiLink component="button" underline="hover" onClick={() => navigate("/admin")}>
            Admin
          </MuiLink>
          <Typography color="text.primary">{plc.name} — Variablen</Typography>
        </Breadcrumbs>
      </Box>

      {error && (
        <Alert severity="error" onClose={() => setError(null)} sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <Stack direction="row" justifyContent="space-between" alignItems="center" sx={{ mb: 2 }}>
        <Typography variant="h5">{plc.name}</Typography>
        <Button
          variant="contained"
          startIcon={<AddIcon />}
          onClick={() => {
            setEditingVariable(null);
            setDialogOpen(true);
          }}
        >
          Neue Variable
        </Button>
      </Stack>

      <AdminVariablesTable
        variables={variables}
        onEdit={(v) => {
          setEditingVariable(v);
          setDialogOpen(true);
        }}
        onDelete={(v) => setConfirmDelete(v)}
      />

      <VariableFormDialog
        open={dialogOpen}
        variable={editingVariable}
        onClose={() => setDialogOpen(false)}
        onSubmit={handleSubmit}
      />

      <ConfirmDialog
        open={confirmDelete != null}
        title="Variable loeschen?"
        message={`"${confirmDelete?.variableName}" wird endgueltig geloescht.`}
        onConfirm={handleConfirmDelete}
        onCancel={() => setConfirmDelete(null)}
      />
    </Box>
  );
}

export default PlcVariablesPage;
