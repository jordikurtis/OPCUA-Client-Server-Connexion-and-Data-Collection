import apiClient from "./apiClient";
// Statut Online/Offline (calculé côté API sur MAX(source_timestamp), seuil 30s)
// pour tous les PLCs d'un groupe donné.
export const getMachineStatusByGroup = async (groupId) => {
    const response = await apiClient.get(`/Machines/group/${groupId}`);
    return response.data;
};