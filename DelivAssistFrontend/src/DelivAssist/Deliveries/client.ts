import axios from "axios";

export const REMOTE_SERVER = import.meta.env.VITE_REMOTE_SERVER;
export const DELIVERIES_API = `${REMOTE_SERVER}/api/userdelivery`;

export const findUserDeliveries = async () => {
    const response = await axios.get(`${DELIVERIES_API}/my-deliveries`);
    return response.data;
}

export const addUserDelivery = async (userDelivery: any) => {
    const response = await axios.post(`${DELIVERIES_API}`, userDelivery);
    return response.data;
}

interface DeliveryFilters {
    app?: string;
    basePay?: number;
    tipPay?: number;
    totalPay?: number;
    restaurant?: string;
    customerNeighborhood?: string;
}

export const getFilteredDeliveries = async (filters: DeliveryFilters) => {
    const response = await axios.get(`${DELIVERIES_API}/filtered-deliveries`, {
        params: filters
    });
    return response.data;
}