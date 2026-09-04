import axios from "axios";

const apiClient = axios.create({
    baseURL: "https://localhost:7126/api",
});

export default apiClient;