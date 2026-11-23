import apiClient from "./apiClient";

// NOTE : ce fichier remplace VariableApi.js (V majuscule). L'ancien fichier
// est à supprimer — l'import "../api/variableApi" (minuscule) utilisé dans
// MeasurementsPage/VariablesPage/Dashboard fonctionnait par coïncidence sous
// Windows/macOS (insensibles à la casse), mais casse sur Linux/CI.

export const getVariables = async () => {
    const response = await apiClient.get("/MonitoredVariables");
    return response.data;
};

export const getVariableById = async (id) => {
    const response = await apiClient.get(`/MonitoredVariables/${id}`);
    return response.data;
};

export const createVariable = async (data) => {
    const response = await apiClient.post("/MonitoredVariables", data);
    return response.data;
};

export const updateVariable = async (id, data) => {
    await apiClient.put(`/MonitoredVariables/${id}`, data);
};

export const deleteVariable = async (id) => {
    await apiClient.delete(`/MonitoredVariables/${id}`);
};
