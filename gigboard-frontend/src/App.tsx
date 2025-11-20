import GigBoard from "./GigBoard";
import Navigation from "./GigBoard/Navigation";
import Deliveries from "./GigBoard/Deliveries";
import Shifts from "./GigBoard/Shifts";
import Expenses from "./GigBoard/Expenses";
import Account from "./GigBoard/Account";
import Statistics from "./GigBoard/Statistics";
import IndividualShift from "./GigBoard/Shifts/IndividualShift";
import store from "./GigBoard/store";
import { Provider } from "react-redux";
import { useEffect } from "react";
import { HashRouter, Route, Routes, Navigate, useNavigate } from "react-router-dom";
import { SignalRProvider } from "./GigBoard/SignalRContext";
import './App.css'

function AuthTokenListener() {
    const navigate = useNavigate();

    useEffect(() => {
        const onStorageChange = (event: StorageEvent) => {
            if (event.key === "token") {
                window.location.reload();
                navigate("/");
            }
        };

        window.addEventListener("storage", onStorageChange);

        return () => {
            window.removeEventListener("storage", onStorageChange);
        };
    }, [navigate]);

    return null;
}

export default function App() {
    return (
        <HashRouter>
            <SignalRProvider>
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
