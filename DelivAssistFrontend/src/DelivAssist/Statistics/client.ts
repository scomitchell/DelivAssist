import axios from "axios";

const axiosWithCredentials = axios.create({withCredentials: true});
export const REMOTE_SERVER = import.meta.env.VITE_REMOTE_SERVER;
export const STATISTICS_API = `${REMOTE_SERVER}/api/statistics`;

export const findAvgDeliveryPay = async () => {
    const token = localStorage.getItem("token");

    const response = await axiosWithCredentials.get(`${STATISTICS_API}/deliveries/avg-delivery-pay`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const findAvgBasePay = async () => {
    const token = localStorage.getItem("token");

    const response = await axiosWithCredentials.get(`${STATISTICS_API}/deliveries/average-base-pay`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const findAvgTipPay = async () => {
    const token = localStorage.getItem("token");

    const response = await axiosWithCredentials.get(`${STATISTICS_API}/deliveries/average-tip`, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    });

    return response.data;
}

export const findHighestPayingNeighborhood = async () => {
    const token = localStorage.getItem("token");

    const response = await axiosWithCredentials.get(`${STATISTICS_API}/deliveries/highest-paying-neighborhood`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const findHighestPayingRestaurant = async () => {
    const token = localStorage.getItem("token");

    const response = await axiosWithCredentials.get(`${STATISTICS_API}/deliveries/highest-paying-restaurant`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const findDollarPerMile = async () => {
    const token = localStorage.getItem("token");

    const response = await axiosWithCredentials.get(`${STATISTICS_API}/deliveries/dollar-per-mile`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const findHighestPayingBaseApp = async () => {
    const token = localStorage.getItem("token");

    const response = await axiosWithCredentials.get(`${STATISTICS_API}/deliveries/highest-paying-base-app`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const findHighestPayingTipApp = async () => {
    const token = localStorage.getItem("token");

    const response = await axiosWithCredentials.get(`${STATISTICS_API}/deliveries/highest-paying-tip-app`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const findAverageMonthlySpending = async () => {
    const token = localStorage.getItem("token");

    const response = await axiosWithCredentials.get(`${STATISTICS_API}/expenses/average-monthly-spending`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const findMonthlySpendingByType = async () => {
    const token = localStorage.getItem("token");

    const response = await axiosWithCredentials.get(`${STATISTICS_API}/expenses/average-spending-by-type`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const findAverageShiftLength = async () => {
    const token = localStorage.getItem("token");

    const response = await axiosWithCredentials.get(`${STATISTICS_API}/shifts/average-shift-length`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const findAppWithMostShifts = async () => {
    const token = localStorage.getItem("token");

    const response = await axiosWithCredentials.get(`${STATISTICS_API}/shifts/app-with-most-shifts`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const findRestaurantWithMostDeliveries = async () => {
    const token = localStorage.getItem("token");

    const response = await axiosWithCredentials.get(`${STATISTICS_API}/deliveries/restaurant-with-most-deliveries`, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    });

    return response.data;
}

export const findAverageTipPerMile = async () => {
    const token = localStorage.getItem("token");

    const response = await axiosWithCredentials.get(`${STATISTICS_API}/deliveries/tip-per-mile`, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    });

    return response.data;
}

export const findAverageDeliveriesPerShift = async () => {
    const token = localStorage.getItem("token");

    const response = await axiosWithCredentials.get(`${STATISTICS_API}/shifts/average-num-deliveries`, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    });

    return response.data;
}

export const findEarningsChart = async () => {
    const token = localStorage.getItem("token");

    const response = await axiosWithCredentials.get(`${STATISTICS_API}/charts/earnings-over-time`, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    });

    return response.data;
}

export const findTipNeighborhoodHist = async () => {
    const token = localStorage.getItem("token");

    const response = await axiosWithCredentials.get(`${STATISTICS_API}/charts/tip-neighborhoods`, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    });

    return response.data;
}

export const findBaseByAppHist = async () => {
    const token = localStorage.getItem("token");

    const response = await axiosWithCredentials.get(`${STATISTICS_API}/charts/apps-by-base`, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    });

    return response.data;
}

export const findHourlyPayChart = async () => {
    const token = localStorage.getItem("token");

    const response = await axiosWithCredentials.get(`${STATISTICS_API}/charts/hourly-earnings`, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    });

    return response.data;
}