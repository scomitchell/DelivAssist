import { FormControl, Button } from "react-bootstrap";
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";
import { setCurrentUser } from "./reducer";
import * as client from "./client";

export default function Login() {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

    const navigate = useNavigate();
    const dispatch = useDispatch();

    function scheduleAutoLogout() {
        setTimeout(() => {
            // Clear token and log out user
            localStorage.removeItem("token");
            dispatch(setCurrentUser(null));
            navigate("/GigBoard/Account/Login");
        }, 60 * 60 * 1000);
    }

    const handleLogin = async () => {
        try {
            const response = await client.loginUser({ username, password });
            localStorage.setItem("token", response.token);
            scheduleAutoLogout();

            dispatch(setCurrentUser(response.user));
            console.log(response.user);
            navigate("/");
        } catch (err: any) {
            setError("Login failed");
        }
    };

    if (error.length > 0) {
        return (
            <p> {error} </p>
        );
    }

    return (
        <div id="da-signin-screen">
            <h1>Sign in</h1>
            <FormControl defaultValue={username}
                style={{maxWidth: "300px"}}
                onChange={(e) => setUsername(e.target.value)}
                className="mb-2 mt-4" placeholder="Username" id="wd-username" />
            <FormControl defaultValue={password} type="password"
                style={{ maxWidth: "300px" }}
                onChange={(e) => setPassword(e.target.value)}
                className="mb-2" placeholder="Password" id="wd-password" />
            <Button onClick={handleLogin} id="da-signin-button"
                className="btn btn-primary w-100" style={{ maxWidth: "300px" }}>
                Sign in
            </Button>
        </div>
    );
}