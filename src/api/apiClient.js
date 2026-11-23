import axios from "axios";

const apiClient = axios.create({
    baseURL: "https://localhost:7126/api",
});

const STORAGE_KEY = "opcua_auth";

// Attache le token JWT à chaque requête sortante, s'il y en a un.
apiClient.interceptors.request.use((config) => {
    try {
        const raw = localStorage.getItem(STORAGE_KEY);
        if (raw) {
            const { token } = JSON.parse(raw);
            if (token) config.headers.Authorization = `Bearer ${token}`;
        }
    } catch {
        // ignore — pas de token exploitable
    }
    return config;
});

// 401 = token invalide/expiré -> déconnexion propre + retour au login.
apiClient.interceptors.response.use(
    (response) => response,
    (error) => {
        if (error.response?.status === 401) {
            localStorage.removeItem(STORAGE_KEY);
            window.location.href = "/login";
        }
        return Promise.reject(error);
    }
);

export default apiClient;



/*
import axios from "axios";

const apiClient = axios.create({
    baseURL: "https://localhost:7126/api",
});

export default apiClient;
*/