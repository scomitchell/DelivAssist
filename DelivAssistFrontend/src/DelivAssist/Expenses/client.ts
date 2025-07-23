import axios from "axios";

const REMOTE_SERVER = import.meta.env.VITE_REMOTE_SERVER;
const EXPENSES_API = `${REMOTE_SERVER}/api/userexpense`;

export const addExpense = async (userExpense: any) => {
    const token = localStorage.getItem("token");

    const response = await axios.post(`${EXPENSES_API}`, userExpense, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const findMyExpenses = async () => {
    const token = localStorage.getItem("token");

    const response = await axios.get(`${EXPENSES_API}/my-expenses`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const findExpenseById = async (expenseId: number) => {
    const token = localStorage.getItem("token");

    const response = await axios.get(`${EXPENSES_API}/${expenseId}`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const deleteExpense = async (expenseId: number) => {
    const token = localStorage.getItem("token");

    const response = await axios.delete(`${EXPENSES_API}/${expenseId}`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export type ExpenseFilters = {
    amount?: number | null;
    date?: string | null;
    type?: string | null;
}

export const findFilteredExpenses = async (filters: ExpenseFilters) => {
    const token = localStorage.getItem("token");

    const response = await axios.get(`${EXPENSES_API}/filtered-expenses`, {
        params: filters,
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}

export const findExpenseTypes = async () => {
    const token = localStorage.getItem("token");

    const response = await axios.get(`${EXPENSES_API}/types`, {
        headers: {
            Authorization: `Bearer ${token}`,
        }
    });

    return response.data;
}