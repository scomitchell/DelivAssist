import axios from "axios";

export const REMOTE_SERVER = import.meta.env.VITE_REMOTE_SERVER;
export const STATISTICS_API = `${REMOTE_SERVER}/api/statistics`;

export const findAvgDeliveryPay = async () => {
    const token = localStorage.getItem("token");

    const response = await axios.get(`${STATISTICS_API}/deliveries/avg-delivery-pay`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const findAvgBasePay = async () => {
    const token = localStorage.getItem("token");

    const response = await axios.get(`${STATISTICS_API}/deliveries/average-base-pay`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const findAvgTipPay = async () => {
    const token = localStorage.getItem("token");

    const response = await axios.get(`${STATISTICS_API}/deliveries/average-tip`, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    });

    return response.data;
}

export const findHighestPayingNeighborhood = async () => {
    const token = localStorage.getItem("token");

    const response = await axios.get(`${STATISTICS_API}/deliveries/highest-paying-neighborhood`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}