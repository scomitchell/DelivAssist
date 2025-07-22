import { Button, Modal, FormGroup, FormControl, FormLabel, Row, Col, Card } from "react-bootstrap";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import * as client from "./client";

export default function MyShifts() {
    // List of user shifts
    const [myShifts, setMyShifts] = useState<any>([]);

    // Modal control state
    const [showForm, setShowForm] = useState(false);

    // User entered Filters
    const [startTime, setStartTime] = useState<string | null>(null);
    const [endTime, setEndTime] = useState<string | null>(null);
    const [app, setApp] = useState<string | null>(null);

    const [userApps, setUserApps] = useState<any>([]);

    const fetchShifts = async () => {
        const shifts = await client.findUserShifts();
        shifts.sort((a: any, b: any) => new Date(b.endTime).getTime() - new Date(a.endTime).getTime());
        setMyShifts(shifts);
        setShowForm(false);
        return;
    }

    const fetchApps = async () => {
        const apps = await client.getUserApps();
        setUserApps(apps);
    }

    const formatTime = (date: string) => {
        const newDate = new Date(date);
        const readable = newDate.toLocaleString();
        return readable;
    }

    useEffect(() => {
        fetchShifts();
        fetchApps();
    }, [])

    return (
        <div id="da-my-shifts" className="mt-3 col-sm-8">
            <Button onClick={() => setShowForm(true)} className="btn btn-warning mb-3">
                Filter Shifts
            </Button>
            
            <Modal show={showForm} onHide={() => setShowForm(false)} centered size="lg">
                <Modal.Header closeButton>
                    <Modal.Title>Filter Shifts</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <div className="filter-shifts">
                        <FormGroup as={Row} className="mb-3">
                            <FormLabel column sm={4}>Starts after</FormLabel>
                                <Col sm={7}>
                                    <FormControl
                                        type="datetime-local"
                                        value={startTime === null ? "" : startTime}
                                        onChange={(e) => setStartTime(e.target.value === "" ? null : e.target.value)}
                                     />
                                </Col>
                        </FormGroup>
                        <FormGroup as={Row} className="mb-3">
                            <FormLabel column sm={4}>Ends after</FormLabel>
                            <Col sm={7}>
                                <FormControl
                                    type="datetime-local"
                                    value={endTime === null ? "" : endTime}
                                    onChange={(e) => setEndTime(e.target.value === "" ? null : e.target.value)}
                                />
                            </Col>
                        </FormGroup>
                        <FormGroup as={Row} className="d-flex align-items-center mb-2">
                            <FormLabel column sm={4}>App</FormLabel>
                            <Col sm={7}>
                                <select onChange={(e) => setUserApps(e.target.value)}
                                    className="form-control mb-2" id="da-app">
                                    <option value=""></option>
                                    {userApps.map((app: any) =>
                                        <option value={app}>{app}</option>
                                    )}
                                </select>
                            </Col>
                        </FormGroup>
                    </div>
                </Modal.Body>
            </Modal>

            <Row>
                {myShifts.map((shift: any) =>
                    <Col sm={6}>
                        <Card className="mb-3 text-start user-delivery-card">
                            <Card.Body style={{ padding: '0.5rem' }}>
                                <Card.Title className="fw-bold">
                                    {formatTime(shift.startTime)} {" - "} {formatTime(shift.endTime)}
                                </Card.Title>
                                <Card.Text>
                                    <strong>App:</strong> {shift.app} {" "} <br />
                                </Card.Text>
                            </Card.Body>
                        </Card>
                    </Col>
                )}
            </Row>
        </div>
    );
}