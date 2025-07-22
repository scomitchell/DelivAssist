import axios from "axios";

export const REMOTE_SERVER = import.meta.env.VITE_REMOTE_SERVER;
export const SHIFTS_API = `${REMOTE_SERVER}/api/usershift`;

export const findUserShifts = async () => {
    const token = localStorage.getItem("token");

    const response = await axios.get(`${SHIFTS_API}/my-shifts`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const findShiftById = async (id: number) => {
    const token = localStorage.getItem("token");

    const response = await axios.get(`${SHIFTS_API}/${id}`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const addShift = async (userShift: any) => {
    const token = localStorage.getItem("token");

    const response = await axios.post(`${SHIFTS_API}`, userShift, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    });

    return response.data;
}

export const getUserApps = async () => {
    const token = localStorage.getItem("token");

    const response = await axios.get(`${SHIFTS_API}/apps`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export type ShiftFilters = {
    startTime?: string | null;
    endTime?: string | null;
    app?: string | null;
}

export const getFilteredShifts = async (filters: ShiftFilters) => {
    const token = localStorage.getItem("token");

    const response = await axios.get(`${SHIFTS_API}/filtered-shifts`, {
        params: filters,
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const deleteUserShift = async (shiftId: number) => {
    const token = localStorage.getItem("token");

    const response = await axios.delete(`${SHIFTS_API}/${shiftId}`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}