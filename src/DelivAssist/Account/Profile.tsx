import { useEffect, useState } from "react";
import { useDispatch, useSelector } from "react-redux";
import { setCurrentUser } from "./reducer";
import { Row, Col, FormLabel, FormGroup, FormControl, Button } from "react-bootstrap";
import * as client from "./client";

export default function Profile() {
    const [user, setUser] = useState<any>({});
    const { currentUser } = useSelector((state: any) => state.accountReducer);
    const [loading, setLoading] = useState(true);

    const dispatch = useDispatch();

    const fetchProfile = async () => {
        const user = await client.getUserByUsername(currentUser.username);
        setUser(user);
        setLoading(false);
    }

    const updateProfile = async () => {
        const updatedProfile = await client.updateUser(user);
        dispatch(setCurrentUser(updatedProfile));
    }

    useEffect(() => {
        setLoading(true);
        fetchProfile();
    }, [currentUser]);

    if (loading) {
        return (
            <p>Loading...</p>
        );
    }

    return (
        <div id="da-profile">
            <h1>Your Profile</h1>
            <div id="da-profile-details" className="col-sm-8 col-md-4">
                <FormGroup as={Row} className="mb-3 mt-4 align-items-center d-flex">
                    <FormLabel column sm={3} className="text-sm-end">First Name</FormLabel>
                    <Col sm={9}>
                        <FormControl
                            placeholder="First Name"
                            id="da-firstname"
                            defaultValue={user.firstName}
                            onChange={(e) => setUser({ ...user, firstName: e.target.value })}
                        />
                    </Col>
                </FormGroup>

                <FormGroup as={Row} className="mb-3 align-items-center d-flex">
                    <FormLabel column sm={3} className="text-sm-end">Last Name</FormLabel>
                    <Col sm={9}>
                        <FormControl
                            placeholder="Last Name"
                            id="da-lastname"
                            defaultValue={user.lastName}
                            onChange={(e) => setUser({ ...user, lastName: e.target.value })}
                        />
                    </Col>
                </FormGroup>

                <FormGroup as={Row} className="mb-3 align-items-center d-flex">
                    <FormLabel column sm={3} className="text-sm-end">Email</FormLabel>
                    <Col sm={9}>
                        <FormControl
                            placeholder="Email"
                            type="email"
                            id="da-lastname"
                            defaultValue={user.email}
                            onChange={(e) => setUser({ ...user, email: e.target.value })}
                        />
                    </Col>
                </FormGroup>

                <FormGroup as={Row} className="mb-3 align-items-center d-flex">
                    <FormLabel column sm={3} className="text-sm-end">Username</FormLabel>
                    <Col sm={9}>
                        <FormControl
                            placeholder="Username"
                            id="da-username"
                            defaultValue={user.username}
                            onChange={(e) => setUser({ ...user, username: e.target.value })}
                        />
                    </Col>
                </FormGroup>

                <FormGroup as={Row} className="mb-3 align-items-center d-flex">
                    <FormLabel column sm={3} className="text-sm-end">Password</FormLabel>
                    <Col sm={9}>
                        <FormControl
                            placeholder="Password"
                            id="da-password"
                            defaultValue=""
                            type="password"
                            onChange={(e) => setUser({ ...user, password: e.target.value })}
                        />
                    </Col>
                </FormGroup>

                <Button onClick={updateProfile} id="da-update-profile-btn" className="btn btn-primary w-100">
                    Update Profile
                </Button>
            </div>
        </div>
    );
}