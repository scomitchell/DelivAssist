import { useState } from "react";
import { FormControl, Button } from "react-bootstrap";
import { useNavigate } from "react-router-dom";
import { useDispatch } from "react-redux";
import { setCurrentUser } from "./reducer";
import * as client from "./client";

export default function Signup() {
    const [firstName, setFirstName] = useState("");
    const [lastName, setLastName] = useState("");
    const [email, setEmail] = useState("");
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");

    const navigate = useNavigate();
    const dispatch = useDispatch();

    function scheduleAutoLogout() {
        setTimeout(() => {
            // Clear token and log out user
            localStorage.removeItem("token");
            navigate("/GigBoard/Account/Login");
        }, 60 * 60 * 1000);
    }

    const signup = async () => {
        try {
            const response = await client.registerUser({ firstName, lastName, email, username, password });
            localStorage.setItem("token", response.token);
            scheduleAutoLogout();

            dispatch(setCurrentUser(response.user));
            navigate("/");
        } catch (err: any) {
            setError("Signin Failed");
        }
    };

    if (error.length > 0) {
        return (
            <p> {error} </p>
        );
    }

    return (
        <div id="da-signup-form">
            <h1>Sign up</h1>
            <FormControl defaultValue={firstName}
                type="text" style={{ maxWidth: "300px" }}
                onChange={(e: any) => setFirstName(e.target.value)}
                className="mb-2 mt-4" placeholder="First Name" id="da-firstname" />
            <FormControl defaultValue={lastName}
                type="text" style={{ maxWidth: "300px" }}
                onChange={(e: any) => setLastName(e.target.value)}
                className="mb-2" placeholder="Last Name" id="da-lastname" />
            <FormControl defaultValue={email}
                type="email" style={{ maxWidth: "300px" }}
                onChange={(e: any) => setEmail(e.target.value)}
                className="mb-2" placeholder="Email" id="da-email" />
            <FormControl defaultValue={username}
                type="text" style={{ maxWidth: "300px" }}
                onChange={(e: any) => setUsername(e.target.value)}
                className="mb-2" placeholder="Username" id="da-username" />
            <FormControl defaultValue={password}
                type="password" style={{maxWidth: "300px"}}
                onChange={(e: any) => setPassword(e.target.value)}
                className="mb-2" placeholder="Password" id="da-password" />
            <Button onClick={signup} id="da-signin-button"
                className="btn btn-primary w-100" style={{ maxWidth: "300px" }}>
                Sign Up
            </Button>
        </div>
    );
}
