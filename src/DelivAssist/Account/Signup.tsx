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

    const signup = async () => {
        try {
            const response = await client.registerUser({ firstName, lastName, email, username, password });
            localStorage.setItem("token", response.token);

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
                onChange={(e: any) => setFirstName(e.target.value)}
                className="mb-2" placeholder="first name" id="da-firstname" />
            <FormControl defaultValue={lastName}
                onChange={(e: any) => setLastName(e.target.value)}
                className="mb-2" placeholder="last name" id="da-lastname" />
            <FormControl defaultValue={email}
                onChange={(e: any) => setEmail(e.target.value)}
                className="mb-2" placeholder="email" id="da-email" />
            <FormControl defaultValue={username}
                onChange={(e: any) => setUsername(e.target.value)}
                className="mb-2" placeholder="username" id="da-username" />
            <FormControl defaultValue={password}
                onChange={(e: any) => setPassword(e.target.value)}
                className="mb-2" placeholder="password" id="da-password" />
            <Button onClick={signup} id="da-signin-button" className="btn btn-danger w-100"> Sign Up </Button>
        </div>
    );
}
