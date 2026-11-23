import { useEffect, useState } from "react";
import { Box, Tabs, Tab, Button, Stack, Alert } from "@mui/material";
import AddIcon from "@mui/icons-material/Add";
import { getGroups, createGroup, updateGroup, deleteGroup } from "../api/groupApi";
import { getPlcs, createPlc, updatePlc, deletePlc } from "../api/plcApi";
import { getUsers, createUser, updateUser, deleteUser } from "../api/userApi";
import AdminGroupsTable from "../components/AdminGroupsTable";
import AdminPlcsTable from "../components/AdminPlcsTable";
import AdminUsersTable from "../components/AdminUsersTable";
import GroupFormDialog from "../components/GroupFormDialog";
import PlcFormDialog from "../components/PlcFormDialog";
import UserFormDialog from "../components/UserFormDialog";
import ConfirmDialog from "../components/ConfirmDialog";

// Message d'erreur lisible : le texte renvoyé par l'API (400/409) s'il existe,
// sinon un message générique.
function apiErrorMessage(err, fallback) {
  const data = err.response?.data;
  return typeof data === "string" && data.length > 0 ? data : fallback;
}

function AdminPage() {
  const [tab, setTab] = useState(0); // 0 = Gruppen, 1 = PLCs, 2 = Benutzer

  const [groups, setGroups] = useState([]);
  const [plcs, setPlcs] = useState([]);
  const [users, setUsers] = useState([]);
  const [error, setError] = useState(null);

  // État des dialogs
  const [groupDialogOpen, setGroupDialogOpen] = useState(false);
  const [editingGroup, setEditingGroup] = useState(null); // null = création
  const [plcDialogOpen, setPlcDialogOpen] = useState(false);
  const [editingPlc, setEditingPlc] = useState(null);
  const [userDialogOpen, setUserDialogOpen] = useState(false);
  const [editingUser, setEditingUser] = useState(null);
  const [confirmDelete, setConfirmDelete] = useState(null); // { type: "group"|"plc"|"user", item }

  const groupsById = Object.fromEntries(groups.map((g) => [g.id, g.name]));

  const loadData = async () => {
    try {
      const [groupsData, plcsData, usersData] = await Promise.all([
        getGroups(),
        getPlcs(),
        getUsers(),
      ]);
      setGroups(groupsData);
      setPlcs(plcsData);
      setUsers(usersData);
    } catch (err) {
      console.error(err);
      setError("Fehler beim Laden der Daten.");
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  // ── Groupes ──────────────────────────────────────────────
  const handleGroupSubmit = async (data) => {
    try {
      if (editingGroup) {
        await updateGroup(editingGroup.id, data);
      } else {
        await createGroup(data);
      }
      setGroupDialogOpen(false);
      setEditingGroup(null);
      await loadData();
    } catch (err) {
      console.error(err);
      setError("Fehler beim Speichern der Gruppe.");
    }
  };

  // ── PLCs ─────────────────────────────────────────────────
  const handlePlcSubmit = async (data) => {
    try {
      if (editingPlc) {
        await updatePlc(editingPlc.id, data);
      } else {
        await createPlc(data);
      }
      setPlcDialogOpen(false);
      setEditingPlc(null);
      await loadData();
    } catch (err) {
      console.error(err);
      setError("Fehler beim Speichern des PLCs.");
    }
  };

  // ── Benutzer ─────────────────────────────────────────────
  const handleUserSubmit = async (data) => {
    try {
      if (editingUser) {
        await updateUser(editingUser.id, data);
      } else {
        await createUser(data);
      }
      setUserDialogOpen(false);
      setEditingUser(null);
      await loadData();
    } catch (err) {
      console.error(err);
      setError(apiErrorMessage(err, "Fehler beim Speichern des Benutzers."));
    }
  };

  // ── Suppression (commune) ───────────────────────────────
  const handleConfirmDelete = async () => {
    try {
      if (confirmDelete.type === "group") {
        await deleteGroup(confirmDelete.item.id);
      } else if (confirmDelete.type === "plc") {
        await deletePlc(confirmDelete.item.id);
      } else {
        await deleteUser(confirmDelete.item.id);
      }
      setConfirmDelete(null);
      await loadData();
    } catch (err) {
      console.error(err);
      const message =
        confirmDelete?.type === "user"
          ? apiErrorMessage(err, "Fehler beim Löschen.")
          : err.response?.status === 409
          ? "Löschen nicht möglich: Es verweisen noch Daten darauf."
          : "Fehler beim Löschen.";
      setError(message);
      setConfirmDelete(null);
    }
  };

  const deleteTitle = {
    group: "Gruppe löschen?",
    plc: "PLC löschen?",
    user: "Benutzer löschen?",
  };

  const deleteMessage = () => {
    if (!confirmDelete) return "";
    const name = confirmDelete.type === "user" ? confirmDelete.item.username : confirmDelete.item.name;
    if (confirmDelete.type === "group") {
      return `"${name}" wird gelöscht. Zugewiesene PLCs verlieren nur ihre Gruppe (werden nicht gelöscht).`;
    }
    return `"${name}" wird endgültig gelöscht.`;
  };

  return (
    <Box>
      {error && (
        <Alert severity="error" onClose={() => setError(null)} sx={{ mb: 2 }}>
          {error}
        </Alert>
      )}

      <Tabs value={tab} onChange={(e, val) => setTab(val)} sx={{ mb: 3 }}>
        <Tab label="Maschinengruppen" />
        <Tab label="PLCs" />
        <Tab label="Benutzer" />
      </Tabs>

      {tab === 0 && (
        <Box>
          <Stack direction="row" justifyContent="flex-end" sx={{ mb: 2 }}>
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={() => {
                setEditingGroup(null);
                setGroupDialogOpen(true);
              }}
            >
              Neue Gruppe
            </Button>
          </Stack>

          <AdminGroupsTable
            groups={groups}
            onEdit={(g) => {
              setEditingGroup(g);
              setGroupDialogOpen(true);
            }}
            onDelete={(g) => setConfirmDelete({ type: "group", item: g })}
          />
        </Box>
      )}

      {tab === 1 && (
        <Box>
          <Stack direction="row" justifyContent="flex-end" sx={{ mb: 2 }}>
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={() => {
                setEditingPlc(null);
                setPlcDialogOpen(true);
              }}
            >
              Neuer PLC
            </Button>
          </Stack>

          <AdminPlcsTable
            plcs={plcs}
            groupsById={groupsById}
            onEdit={(p) => {
              setEditingPlc(p);
              setPlcDialogOpen(true);
            }}
            onDelete={(p) => setConfirmDelete({ type: "plc", item: p })}
          />
        </Box>
      )}

      {tab === 2 && (
        <Box>
          <Stack direction="row" justifyContent="flex-end" sx={{ mb: 2 }}>
            <Button
              variant="contained"
              startIcon={<AddIcon />}
              onClick={() => {
                setEditingUser(null);
                setUserDialogOpen(true);
              }}
            >
              Neuer Benutzer
            </Button>
          </Stack>

          <AdminUsersTable
            users={users}
            onEdit={(u) => {
              setEditingUser(u);
              setUserDialogOpen(true);
            }}
            onDelete={(u) => setConfirmDelete({ type: "user", item: u })}
          />
        </Box>
      )}

      <GroupFormDialog
        open={groupDialogOpen}
        group={editingGroup}
        onClose={() => setGroupDialogOpen(false)}
        onSubmit={handleGroupSubmit}
      />

      <PlcFormDialog
        open={plcDialogOpen}
        plc={editingPlc}
        groups={groups}
        onClose={() => setPlcDialogOpen(false)}
        onSubmit={handlePlcSubmit}
      />

      <UserFormDialog
        open={userDialogOpen}
        user={editingUser}
        onClose={() => setUserDialogOpen(false)}
        onSubmit={handleUserSubmit}
      />

      <ConfirmDialog
        open={confirmDelete != null}
        title={confirmDelete ? deleteTitle[confirmDelete.type] : ""}
        message={deleteMessage()}
        onConfirm={handleConfirmDelete}
        onCancel={() => setConfirmDelete(null)}
      />
    </Box>
  );
}

export default AdminPage;
