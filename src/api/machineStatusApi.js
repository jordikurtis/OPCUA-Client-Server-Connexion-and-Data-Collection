import apiClient from "./apiClient";

export const getMachineStatusByGroup = async(groupId) => {
    const response = await apiClient.get(`/Machines/group/${groupId}`);
    return response.data;
};