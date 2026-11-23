import apiClient from "./apiClient";

export const getGroups = async () => {
    const response = await apiClient.get("/MachineGroups");
    return response.data;
};

export const createGroup = async (data) => {
    const response = await apiClient.post("/MachineGroups", data);
    return response.data;
};

export const updateGroup = async (id, data) => {
    await apiClient.put(`/MachineGroups/${id}`, data);
};

export const deleteGroup = async (id) => {
    await apiClient.delete(`/MachineGroups/${id}`);
};
