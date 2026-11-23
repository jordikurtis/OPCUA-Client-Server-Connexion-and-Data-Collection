import apiClient from "./apiClient";

export const login = async (username, password) => {
    const response = await apiClient.post("/Auth/login", { username, password });
    return response.data; // { token, username, role, expiresAt }
};
