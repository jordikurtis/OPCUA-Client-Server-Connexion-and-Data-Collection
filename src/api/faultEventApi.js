import apiClient from "./apiClient";
// Section Fehlerbehandlung — paginé, filtrable. Retourne
// { items, totalCount, page, pageSize, hasMore }
export const getAllFaults = async (plcId, status, page = 1, pageSize = 25) => {
    const response = await apiClient.get("/FaultEvents", {
        params: { plcId: plcId || undefined, status: status || undefined, page, pageSize },
    });
    return response.data;
};
export const getAllActiveFaults = async () => {
    const response = await apiClient.get("/FaultEvents/active");
    return response.data;
};
export const getActiveFaults = async (plcId) => {
    const response = await apiClient.get(`/FaultEvents/plc/${plcId}/active`);
    return response.data;
};
export const getFaultHistory = async (plcId, from, to) => {
    const response = await apiClient.get(`/FaultEvents/plc/${plcId}/history`, {
        params: { from, to },
    });
    return response.data;
};
export const acknowledgeFault = async (id) => {
    await apiClient.put(`/FaultEvents/${id}/acknowledge`);
};
export const startFaultProgress = async (id) => {
    await apiClient.put(`/FaultEvents/${id}/start-progress`);
};
export const resolveFault = async (id) => {
    await apiClient.put(`/FaultEvents/${id}/resolve`);
};



/*

import apiClient from "./apiClient";
// Section Fehlerbehandlung — tous PLCs, filtrable
export const getAllFaults = async (plcId, status) => {
    const response = await apiClient.get("/FaultEvents", {
        params: { plcId: plcId || undefined, status: status || undefined },
    });
    return response.data;
};
export const getAllActiveFaults = async () => {
    const response = await apiClient.get("/FaultEvents/active");
    return response.data;
};
export const getActiveFaults = async (plcId) => {
    const response = await apiClient.get(`/FaultEvents/plc/${plcId}/active`);
    return response.data;
};
export const getFaultHistory = async (plcId, from, to) => {
    const response = await apiClient.get(`/FaultEvents/plc/${plcId}/history`, {
        params: { from, to },
    });
    return response.data;
};
export const acknowledgeFault = async (id) => {
    await apiClient.put(`/FaultEvents/${id}/acknowledge`);
};
export const startFaultProgress = async (id) => {
    await apiClient.put(`/FaultEvents/${id}/start-progress`);
};
export const resolveFault = async (id) => {
    await apiClient.put(`/FaultEvents/${id}/resolve`);
}

*/