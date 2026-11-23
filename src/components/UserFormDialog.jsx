import { useEffect, useState } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Stack,
  MenuItem,
} from "@mui/material";

const ROLES = ["Admin", "Operator", "Instandhaltung"];
const MIN_PASSWORD_LENGTH = 8;

// user : null (création) ou { id, username, role } (édition — le nom
// d'utilisateur n'est pas modifiable, le mot de passe est optionnel)
function UserFormDialog({ open, user, onClose, onSubmit }) {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [role, setRole] = useState("Operator");

  const isEdit = user != null;

  useEffect(() => {
    setUsername(user?.username ?? "");
    setPassword("");
    setRole(user?.role ?? "Operator");
  }, [user, open]);

  const handleSubmit = () => {
    if (isEdit) {
      onSubmit({ role, password: password === "" ? null : password });
    } else {
      onSubmit({ username, password, role });
    }
  };

  const passwordTooShort = password !== "" && password.length < MIN_PASSWORD_LENGTH;

  const canSubmit = isEdit
    ? !passwordTooShort
    : username.trim() && password.length >= MIN_PASSWORD_LENGTH;

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
      <DialogTitle>{isEdit ? "Benutzer bearbeiten" : "Neuer Benutzer"}</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          <TextField
            label="Benutzername"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            fullWidth
            autoFocus={!isEdit}
            disabled={isEdit}
          />
          <TextField
            label={isEdit ? "Neues Passwort" : "Passwort"}
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            fullWidth
            error={passwordTooShort}
            helperText={
              isEdit
                ? "Leer lassen, um das bestehende Passwort beizubehalten."
                : `Mindestens ${MIN_PASSWORD_LENGTH} Zeichen.`
            }
          />
          <TextField
            select
            label="Rolle"
            value={role}
            onChange={(e) => setRole(e.target.value)}
            fullWidth
          >
            {ROLES.map((r) => (
              <MenuItem key={r} value={r}>
                {r}
              </MenuItem>
            ))}
          </TextField>
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

export default UserFormDialog;
