import apiClient from "./apiClient";
export const getLatestMeasurements = async () => {
    const response = await apiClient.get("/Measurements/latest");
    return response.data;
};
// Historian — from/to en ISO string. Retourne { items, totalCount, page, pageSize, hasMore }
export const getMeasurementHistory = async (variableId, from, to, page = 1, pageSize = 2000) => {
    const response = await apiClient.get("/Measurements/history", {
        params: { variableId, from, to, page, pageSize },
    });
    return response.data;
};
// Récupère TOUTES les pages d'une plage — pour l'export CSV, qui ne doit
// jamais être tronqué même si l'affichage graphique l'est (voir
// HistoryChartDialog : le graphique se limite à la 1ère page pour rester
// lisible/performant, mais le CSV doit contenir la plage complète).
export const getAllMeasurementHistory = async (variableId, from, to) => {
    const pageSize = 2000;
    let page = 1;
    let all = [];
    let hasMore = true;
    while (hasMore) {
        const result = await getMeasurementHistory(variableId, from, to, page, pageSize);
        all = all.concat(result.items);
        hasMore = result.hasMore;
        page += 1;
    }
    return all;
};


/*import apiClient from "./apiClient";

export const getLatestMeasurements = async () => {
    const response = await apiClient.get("/Measurements/latest");
    return response.data;
};

// Historian — from/to en ISO string (ex: new Date().toISOString())
export const getMeasurementHistory = async (variableId, from, to) => {
    const response = await apiClient.get("/Measurements/history", {
        params: { variableId, from, to },
    });
    return response.data;
};
*/