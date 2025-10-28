import DelivAssist from "./DelivAssist";
import Navigation from "./DelivAssist/Navigation";
import Deliveries from "./DelivAssist/Deliveries";
import Shifts from "./DelivAssist/Shifts";
import Expenses from "./DelivAssist/Expenses";
import Account from "./DelivAssist/Account";
import Statistics from "./DelivAssist/Statistics";
import IndividualShift from "./DelivAssist/Shifts/IndividualShift";
import store from "./DelivAssist/store";
import { Provider } from "react-redux";
import { HashRouter, Route, Routes, Navigate } from "react-router-dom";
import './App.css'

export default function App() {
    return (
        <HashRouter>
            <Provider store={store}>
                <div id="da-main-app">
                    <Navigation />
                    <Routes>
                        <Route path="/" element={<Navigate to="DelivAssist" />} />
                        <Route path="/DelivAssist/*" element={<DelivAssist />} />
                        <Route path="/DelivAssist/MyDeliveries/*" element={<Deliveries />} />
                        <Route path="/DelivAssist/Shifts/*" element={<Shifts /> } />
                        <Route path="/DelivAssist/Shifts/:shiftId" element={<IndividualShift />} />
                        <Route path="/DelivAssist/Expenses/*" element={<Expenses /> } />
                        <Route path="/DelivAssist/Account/*" element={<Account />} />
                        <Route path="/DelivAssist/Statistics/*" element={<Statistics /> } />
                    </Routes>
                </div>
            </Provider>
        </HashRouter>
    );
}
