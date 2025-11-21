import GigBoard from "./GigBoard";
import Navigation from "./GigBoard/Navigation";
import Deliveries from "./GigBoard/Deliveries";
import Shifts from "./GigBoard/Shifts";
import Expenses from "./GigBoard/Expenses";
import Account from "./GigBoard/Account";
import Statistics from "./GigBoard/Statistics";
import IndividualShift from "./GigBoard/Shifts/IndividualShift";
import store from "./GigBoard/store";
import { Provider, useDispatch } from "react-redux";
import { useEffect, useState } from "react";
import { HashRouter, Route, Routes, Navigate, useNavigate } from "react-router-dom";
import { setCurrentUser } from "./GigBoard/Account/reducer";
import { SignalRProvider } from "./GigBoard/SignalRContext";
import {jwtDecode } from "jwt-decode";
import { useSignalR } from "./GigBoard/SignalRContext";
import type { JwtPayload } from "jwt-decode";
import './App.css'

type GigBoardJwt = {
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier": string;
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name": string;
} & JwtPayload;

function AuthTokenListener() {
    const navigate = useNavigate();
    const dispatch = useDispatch();
    const { clearStats } = useSignalR();

    useEffect(() => {
        const onStorageChange = (event: StorageEvent) => {
            if (event.key === "token" && event.newValue === null) {
                dispatch(setCurrentUser(null));
                clearStats();
                navigate("/");
            }

            if (event.key === "token" && event.newValue) {
                const token: any = localStorage.getItem("token");
                try {
                    const decodedUser = jwtDecode<GigBoardJwt>(token);
                    const user = {
                        id: decodedUser["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"],
                        username: decodedUser["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"]
                    };

                    const exp = decodedUser.exp;

                    if (!exp) {
                        console.log("Token has no expiration");
                    } else {
                        const now = Math.floor(Date.now() / 1000);

                        if (exp < now) {
                            clearStats();
                            localStorage.removeItem("token");

                            window.dispatchEvent(new Event("logout"));
                            dispatch(setCurrentUser(null));
                            navigate("/");
                        } else {
                            dispatch(setCurrentUser(user));
                        }
                    }
                } catch (e) {
                    localStorage.removeItem("token");
                }
            }
        };

        window.addEventListener("storage", onStorageChange);

        return () => {
            window.removeEventListener("storage", onStorageChange);
        };
    }, [dispatch, navigate, clearStats]);

    useEffect(() => {
        const token: any = localStorage.getItem("token");
        if (token) {
            try {
                const decodedUser = jwtDecode<GigBoardJwt>(token);
                const user = {
                    id: decodedUser["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"],
                    username: decodedUser["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"]
                };

                const exp = decodedUser.exp;

                if (!exp) {
                    console.log("Token has no expiration");
                } else {
                    const now = Math.floor(Date.now() / 1000);

                    if (exp < now) {
                        clearStats();
                        localStorage.removeItem("token");

                        window.dispatchEvent(new Event("logout"));
                        dispatch(setCurrentUser(null));
                        navigate("/");
                    } else {
                        dispatch(setCurrentUser(user));
                    }
                }
            } catch (e) {
                localStorage.removeItem("token");
            }
        }
    }, [dispatch])

    return null;
}

export default function App() {
    const [token, setToken] = useState(localStorage.getItem("token"));

    useEffect(() => {
        const handleLogout = () => {
            setToken(null);
        };

        window.addEventListener("logout", handleLogout);

        return () => {
            window.removeEventListener("logout", handleLogout);
        };
    }, []);

    useEffect(() => {
        const handleLogin = () => {
            setToken(localStorage.getItem("token"));
        };

        window.addEventListener("login", handleLogin);

        return () => {
            window.removeEventListener("login", handleLogin);
        };
    }, []);
    
    return (
        <HashRouter>
            <SignalRProvider token={token}>
                <Provider store={store}>
                    <AuthTokenListener />
                    <div id="da-main-app">
                        <Navigation />
                        <Routes>
                            <Route path="/" element={<Navigate to="GigBoard" />} />
                            <Route path="/GigBoard/*" element={<GigBoard />} />
                            <Route path="/GigBoard/MyDeliveries/*" element={<Deliveries />} />
                            <Route path="/GigBoard/Shifts/*" element={<Shifts />} />
                            <Route path="/GigBoard/Shifts/:shiftId" element={<IndividualShift />} />
                            <Route path="/GigBoard/Expenses/*" element={<Expenses />} />
                            <Route path="/GigBoard/Account/*" element={<Account />} />
                            <Route path="/GigBoard/Statistics/*" element={<Statistics />} />
                        </Routes>
                    </div>
                </Provider>
            </SignalRProvider>
        </HashRouter>
    );
}
