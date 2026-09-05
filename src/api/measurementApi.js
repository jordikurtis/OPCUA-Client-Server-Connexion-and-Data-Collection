import apiClient from "./apiClient";

export const getLatestMeasurements = async () => {
    const response = await apiClient.get("/Measurements/latest");
    return response.data;
};
