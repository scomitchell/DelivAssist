import { Route, Routes, Navigate } from "react-router-dom";
import Login from "./Login";
import Signup from "./Signup";
import Profile from "./Profile";

export default function Account() {
    return (
        <div id="da-account">
            <Routes>
                <Route index element={<Navigate to="Login" />} />
                <Route path="Login/*" element={<Login />} />
                <Route path="Signup/*" element={<Signup />} />
                <Route path="Profile/*" element={<Profile /> }/>
            </Routes>
        </div>
    );
}