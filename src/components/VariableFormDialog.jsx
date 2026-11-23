import { useEffect, useState } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Stack,
  FormControlLabel,
  Switch,
  Divider,
  Typography,
} from "@mui/material";

// variable : null (création) ou variable existante (édition — PlcId/GroupId
// non modifiables ici, une variable ne change pas de PLC une fois créée)
function VariableFormDialog({ open, variable, onClose, onSubmit }) {
  const [nodeName, setNodeName] = useState("");
  const [variableName, setVariableName] = useState("");
  const [nodeId, setNodeId] = useState("");
  const [dataType, setDataType] = useState("");
  const [enabled, setEnabled] = useState(true);
  const [isFaultIndicator, setIsFaultIndicator] = useState(false);

  const isEdit = variable != null;

  useEffect(() => {
    setNodeName(variable?.nodeName ?? "");
    setVariableName(variable?.variableName ?? "");
    setNodeId(variable?.nodeId ?? "");
    setDataType(variable?.dataType ?? "");
    setEnabled(variable?.enabled ?? true);
    setIsFaultIndicator(variable?.isFaultIndicator ?? false);
  }, [variable, open]);

  const handleSubmit = () => {
    onSubmit({ nodeName, variableName, nodeId, dataType, enabled, isFaultIndicator });
  };

  const canSubmit = nodeName.trim() && variableName.trim() && nodeId.trim();

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>{isEdit ? "Variable bearbeiten" : "Neue Variable"}</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          <TextField
            label="Node"
            value={nodeName}
            onChange={(e) => setNodeName(e.target.value)}
            fullWidth
            autoFocus
            helperText="Logische Gruppierung (z.B. Antrieb_Eindrückstempel)"
          />
          <TextField
            label="Variable"
            value={variableName}
            onChange={(e) => setVariableName(e.target.value)}
            fullWidth
          />
          <TextField
            label="NodeId (OPC-UA)"
            value={nodeId}
            onChange={(e) => setNodeId(e.target.value)}
            fullWidth
            placeholder="ns=3;i=1001"
          />
          <TextField
            label="Datentyp"
            value={dataType}
            onChange={(e) => setDataType(e.target.value)}
            fullWidth
            placeholder="Double, Int32, Boolean..."
          />
          <FormControlLabel
            control={<Switch checked={enabled} onChange={(e) => setEnabled(e.target.checked)} />}
            label="Aktiviert"
          />

          <Divider />

          <FormControlLabel
            control={
              <Switch
                checked={isFaultIndicator}
                onChange={(e) => setIsFaultIndicator(e.target.checked)}
                color="error"
              />
            }
            label="Fehlerindikator"
          />
          <Typography variant="caption" color="text.secondary" sx={{ mt: -1.5 }}>
            Wert "0" = kein Fehler, jeder andere Wert = aktiver Fehler (Fehlercode = Wert).
          </Typography>
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>Abbrechen</Button>
        <Button onClick={handleSubmit} variant="contained" disabled={!canSubmit}>
          Speichern
        </Button>
      </DialogActions>
    </Dialog>
  );
}

export default VariableFormDialog;
