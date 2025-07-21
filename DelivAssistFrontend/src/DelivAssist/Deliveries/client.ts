import axios from "axios";

const axiosWithCredentials = axios.create({ withCredentials: true });
export const REMOTE_SERVER = import.meta.env.VITE_REMOTE_SERVER;
export const DELIVERIES_API = `${REMOTE_SERVER}/api/userdelivery`;

export const findUserDeliveries = async () => {
    const token = localStorage.getItem("token");

    const response = await axios.get(`${DELIVERIES_API}/my-deliveries`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const findUserNeighborhoods = async () => {
    const token = localStorage.getItem("token");

    const response = await axios.get(`${DELIVERIES_API}/delivery-neighborhoods`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const findUserApps = async () => {
    const token = localStorage.getItem("token");

    const response = await axios.get(`${DELIVERIES_API}/delivery-apps`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const addUserDelivery = async (userDelivery: any) => {
    const token = localStorage.getItem("token");

    const response = await axios.post(`${DELIVERIES_API}`, userDelivery, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export type DeliveryFilters = {
    app?: string | null;
    basePay?: number | null;
    tipPay?: number | null;
    totalPay?: number | null;
    mileage?: number | null;
    restaurant?: string | null;
    customerNeighborhood?: string | null;
}

export const getFilteredDeliveries = async (filters: DeliveryFilters) => {
    const token = localStorage.getItem("token");

    const response = await axios.get(`${DELIVERIES_API}/filtered-deliveries`, {
        params: filters,
        headers: {
            Authorization: `Bearer ${token}`
        },
    });
    return response.data;
}