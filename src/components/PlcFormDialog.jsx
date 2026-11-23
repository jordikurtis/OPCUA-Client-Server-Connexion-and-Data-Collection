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
    MenuItem,
} from "@mui/material";

// plc : null (création) ou { id, name, enabled, groupId } (édition — jamais
// d'endpointUrl ici, GET ne le renvoie pas, cf. décision sécurité)
// groups : liste des groupes pour le select
function PlcFormDialog({ open, plc, groups, onClose, onSubmit }) {
    const [name, setName] = useState("");
    const [endpointUrl, setEndpointUrl] = useState("");
    const [enabled, setEnabled] = useState(false);
    const [groupId, setGroupId] = useState("");

    const isEdit = plc != null;

    useEffect(() => {
        setName(plc?.name ?? "");
        setEndpointUrl(""); //jamais pré-rempli — voir helperText plus bas
        setEnabled(plc?.enabled ?? false);
        setGroupId(plc?.groupId ?? "");
    }, [plc, open]);

    const handleSubmit = () => {
        onSubmit({
            name,
            endpointUrl,
            enabled,
            groupId: groupId === "" ? null : groupId,
        });
    };

    //Création : nom + endpoint obligatoires. Édition : nom seul obligatoire
    //(endpoint vide = on garde l'ancienne valeur côté API).
    const canSubmit = isEdit ? name.trim().length > 0 : name.trim() && endpointUrl.trim();

    return (
        <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
            <DialogTitle>{isEdit ? "PLC bearbeiten" : "Neuer PLC"}</DialogTitle>
            <DialogContent>
                <Stack spacing={2} sx={{ mt: 1 }}>
                    <TextField
                        label="Name"
                        value={name}
                        onChange={(e) => setName(e.target.value)}
                        fullWidth
                        autoFocus
                    />
                    <TextField
                        label="Endpoint URL"
                        value={endpointUrl}
                        onChange={(e) => setEndpointUrl(e.target.value)}
                        placeholder="opc.tcp://..."
                        fullWidth
                        helperText={
                            isEdit
                                ? "Aus Sicherheitsgruenden nicht angezeigt. Leer lassen, um die bestehende URL beizubehalten."
                                : undefined
                        }
                    />
                    <TextField
                        select
                        label="Gruppe"
                        value={groupId}
                        onChange={(e) => setGroupId(e.target.value)}
                        fullWidth
                    >
                        <MenuItem value=""> Keine Gruppe </MenuItem>
                        {groups.map((g) => (
                            <MenuItem key={g.id} value={g.id}>
                                {g.name}
                            </MenuItem>
                        ))}
                    </TextField>
                    <FormControlLabel
                        control={<Switch checked={enabled} onChange={(e) => setEnabled(e.target.checked)} />}
                        label="Aktiviert"
                    />
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

export default PlcFormDialog;
