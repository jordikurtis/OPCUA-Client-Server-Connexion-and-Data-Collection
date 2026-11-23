import apiClient from "./apiClient";

export const getUsers = async () => {
    const response = await apiClient.get("/Users");
    return response.data;
};

export const createUser = async (data) => {
    const response = await apiClient.post("/Users", data);
    return response.data;
};

export const updateUser = async (id, data) => {
    await apiClient.put(`/Users/${id}`, data);
};

export const deleteUser = async (id) => {
    await apiClient.delete(`/Users/${id}`);
};

export const changeOwnPassword = async (currentPassword, newPassword) => {
    await apiClient.post("/Users/me/password", { currentPassword, newPassword });
};
