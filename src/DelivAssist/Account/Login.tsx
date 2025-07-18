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

    const handleLogin = async () => {
        try {
            const response = await client.loginUser({ username, password });
            localStorage.setItem("token", response.token);

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
                onChange={(e) => setUsername(e.target.value)}
                className="mb-2" placeholder="username" id="wd-username" />
            <FormControl defaultValue={password} type="password"
                onChange={(e) => setPassword(e.target.value)}
                className="mb-2" placeholder="password" id="wd-password" />
            <Button onClick={handleLogin} id="da-signin-button" className="btn btn-danger w-100"> Sign in </Button>
        </div>
    );
}