import apiClient from "./apiClient";

export const getPlcs = async () => {
    const response = await apiClient.get("/PlcConfigs");
    return response.data;
};