import { useEffect, useState } from "react";
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Stack,
  Alert,
} from "@mui/material";
import { changeOwnPassword } from "../api/userApi";

const MIN_PASSWORD_LENGTH = 8;

function ChangePasswordDialog({ open, onClose }) {
  const [currentPassword, setCurrentPassword] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [error, setError] = useState(null);
  const [success, setSuccess] = useState(false);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    if (open) {
      setCurrentPassword("");
      setNewPassword("");
      setConfirmPassword("");
      setError(null);
      setSuccess(false);
    }
  }, [open]);

  const mismatch = confirmPassword !== "" && newPassword !== confirmPassword;
  const tooShort = newPassword !== "" && newPassword.length < MIN_PASSWORD_LENGTH;
  const canSubmit =
    currentPassword && newPassword.length >= MIN_PASSWORD_LENGTH && newPassword === confirmPassword;

  const handleSubmit = async () => {
    setError(null);
    setLoading(true);
    try {
      await changeOwnPassword(currentPassword, newPassword);
      setSuccess(true);
    } catch (err) {
      const data = err.response?.data;
      setError(typeof data === "string" ? data : "Passwort konnte nicht geändert werden.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="xs">
      <DialogTitle>Passwort ändern</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          {error && <Alert severity="error">{error}</Alert>}
          {success && <Alert severity="success">Passwort wurde geändert.</Alert>}

          <TextField
            label="Aktuelles Passwort"
            type="password"
            value={currentPassword}
            onChange={(e) => setCurrentPassword(e.target.value)}
            fullWidth
            autoFocus
            disabled={success}
          />
          <TextField
            label="Neues Passwort"
            type="password"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            fullWidth
            error={tooShort}
            helperText={`Mindestens ${MIN_PASSWORD_LENGTH} Zeichen.`}
            disabled={success}
          />
          <TextField
            label="Neues Passwort bestätigen"
            type="password"
            value={confirmPassword}
            onChange={(e) => setConfirmPassword(e.target.value)}
            fullWidth
            error={mismatch}
            helperText={mismatch ? "Passwörter stimmen nicht überein." : undefined}
            disabled={success}
          />
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose}>{success ? "Schließen" : "Abbrechen"}</Button>
        {!success && (
          <Button onClick={handleSubmit} variant="contained" disabled={!canSubmit || loading}>
            Speichern
          </Button>
        )}
      </DialogActions>
    </Dialog>
  );
}

export default ChangePasswordDialog;
