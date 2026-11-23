import { Dialog, DialogTitle, DialogContent, DialogContentText, DialogActions, Button } from "@mui/material";

function ConfirmDialog({ open, title, message, onConfirm, onCancel }) {
    return (
        <Dialog open={open} onClose={onCancel}>
            <DialogTitle>{title}</DialogTitle>
            <DialogContent>
                <DialogContentText>{message}</DialogContentText>
            </DialogContent>
            <DialogActions>
                <Button onClick={onCancel}>Abbrechen</Button>
                <Button onClick={onConfirm} color="error" variant="contained">
                    Löschen
                </Button>
            </DialogActions>
        </Dialog>
    );
}

export default ConfirmDialog;
