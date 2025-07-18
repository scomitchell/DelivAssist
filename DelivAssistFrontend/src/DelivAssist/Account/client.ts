import axios from "axios";

export const REMOTE_SERVER = import.meta.env.VITE_REMOTE_SERVER;
export const USERS_API = `${REMOTE_SERVER}/api/user`;

export const registerUser = async (user: any) => {
    const response = await axios.post(`${USERS_API}/register`, user);
    return response.data;
}

export const loginUser = async (user: any) => {
    const response = await axios.post(`${USERS_API}/login`, user);
    return response.data;
}

export const updateUser = async (user: any) => {
    const token = localStorage.getItem("token");

    const response = await axios.put(`${USERS_API}`, user, {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });
    return response.data;
}

export const getUserByUsername = async (username: string) => {
    const token = localStorage.getItem("token");

    const response = await axios.get(`${USERS_API}/${username}`, {
        headers: {
            Authorization: `Bearer ${token}`,
        },
    });
    return response.data;
}