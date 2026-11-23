import apiClient from "./apiClient";

export const getPlcs = async () => {
    const response = await apiClient.get("/PlcConfigs");
    return response.data;
};

export const getPlcById = async (id) => {
    const response = await apiClient.get(`/PlcConfigs/${id}`);
    return response.data;
};

export const getPlcVariables = async (id) => {
    const response = await apiClient.get(`/PlcConfigs/${id}/Variables`);
    return response.data;
};

export const createPlc = async (data) => {
    const response = await apiClient.post("/PlcConfigs", data);
    return response.data;
};

export const updatePlc = async (id, data) => {
    await apiClient.put(`/PlcConfigs/${id}`, data);
};

export const deletePlc = async (id) => {
    await apiClient.delete(`/PlcConfigs/${id}`);
};
