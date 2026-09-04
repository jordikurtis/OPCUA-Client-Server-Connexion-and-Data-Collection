import apiClient from "./apiClient";

export const getGroups = async () => {
    const response = await apiClient.get("/MachineGroups");
    return response.data;
};