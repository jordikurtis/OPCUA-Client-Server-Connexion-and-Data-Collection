import apiClient from "./apiClient";

export const getVariables = async () => {
    const response = await apiClient.get("/MonitoredVariables");
    return response.data;
};