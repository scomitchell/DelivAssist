import { Button, Modal, FormGroup, FormControl, FormLabel, Row, Col, Card, Dropdown } from "react-bootstrap";
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import * as client from "./client";
import * as deliveryClient from "../Deliveries/client";
import type { ShiftFilters } from "./client";

export default function MyShifts({ myShifts, setMyShifts }: {
    myShifts: any[],
    setMyShifts: React.Dispatch<React.SetStateAction<any[]>>}) {
    // Modal control state
    const [showForm, setShowForm] = useState(false);
    const [showModal, setShowModal] = useState(false)
    const [showSDModal, setShowSDModal] = useState(false);

    // User entered Filters
    const [startTime, setStartTime] = useState<string | null>(null);
    const [endTime, setEndTime] = useState<string | null>(null);
    const [app, setApp] = useState<string | null>(null);
    
    // Select menu options
    const [userApps, setUserApps] = useState<any>([]);

    // Control reset
    const [reset, setReset] = useState(false);
    const [deliveries, setDeliveries] = useState<any[]>([]);

    // Shift deliveries
    const [deliveryToAdd, setDeliveryToAdd] = useState<number>(-1);
    const [shiftForDelivery, setShiftForDelivery] = useState<number>(-1);
    
    // Fetch all or fitered shifts
    const fetchShifts = async () => {
        // If any filters applied, call filteredShifts
        if (startTime || endTime || app) {
            const filters: ShiftFilters = {
                startTime: startTime,
                endTime: endTime,
                app: app
            }

            const shifts = await client.getFilteredShifts(filters);

            // Sort shifts by date
            shifts.sort((a: any, b: any) => new Date(b.endTime).getTime() - new Date(a.endTime).getTime());

            setMyShifts(shifts);
            setShowForm(false);
            return;
        }

        const shifts = await client.findUserShifts();
        shifts.sort((a: any, b: any) => new Date(b.endTime).getTime() - new Date(a.endTime).getTime());
        setMyShifts(shifts);
        setShowForm(false);
        return;
    }

    const fetchDeliveries = async () => {
        const userDeliveries = await deliveryClient.findUnassignedUserDeliveries();
        setDeliveries(userDeliveries);
    }

      // Single handler to open the modal for a given shift:
    const openAddDelivery = (shiftId: number) => {
        console.log(shiftId);
        setShiftForDelivery(shiftId);
        setDeliveryToAdd(-1);        // reset previous
        setShowSDModal(true);
    };

    // Unified “add” handler, always logs the actual payload:
    const handleAddDelivery = async () => {
        console.log("ADDING DELIVERY:", {
        shiftForDelivery,
        deliveryToAdd
        });

        if (shiftForDelivery > 0 && deliveryToAdd > 0) {
        await client.AddShiftDelivery(shiftForDelivery, deliveryToAdd);
        setShowSDModal(false);
        fetchShifts();
        } else {
        alert("Please select a valid shift & delivery.");
        }
    };

    // Delete shift from db
    const deleteShift = async (shiftId: number) => {
        await client.deleteUserShift(shiftId);
        fetchShifts();
    }

    // Fetch list of user used apps
    const fetchApps = async () => {
        const apps = await client.getUserApps();
        setUserApps(apps);
    }

    // Display time as date, time
    const formatTime = (date: string) => {
        const newDate = new Date(date);
        const readable = newDate.toLocaleString();
        return readable;
    }

    // Clear all filters
    const resetFilters = () => {
        setStartTime(null);
        setEndTime(null);
        setApp(null);
        setReset(true);
    }

    useEffect(() => {
        fetchShifts();
        fetchApps();
        fetchDeliveries();
    }, [])

    // useEffect for reset filters
    useEffect(() => {
        const allCleared =
            startTime === null &&
            endTime === null &&
            app === null;

        if (reset && allCleared) {
            fetchShifts();
            setReset(false);
        }
    }, [startTime, endTime, app, reset])

    return (
        <div id="da-my-shifts" className="mt-3 col-sm-8">
            <Button onClick={() => setShowForm(true)} className="btn btn-warning mb-3 me-2">
                Filter Shifts
            </Button>
            <Button onClick={resetFilters} className="btn btn-danger mb-3">
                Reset filters
            </Button>
            
            {/*Modal to filter shifts*/}
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
                            <FormLabel column sm={4}>Ends before</FormLabel>
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
                                <select onChange={(e) => setApp(e.target.value)}
                                    className="form-control mb-2" id="da-app">
                                    <option value=""></option>
                                    {userApps.map((app: any) =>
                                        <option value={app}>{app}</option>
                                    )}
                                </select>
                            </Col>
                        </FormGroup>
                        <Button onClick={fetchShifts} className="btn btn-primary">
                            Apply Filters
                        </Button>
                    </div>
                </Modal.Body>
            </Modal>
            
            {/*Display Shift details on cards*/}
            <Row>
                {myShifts.map((shift: any) =>
                    <Col sm={6}>
                        <Card className="mb-3 text-start user-shift-card">
                            <Card.Body style={{ padding: '0.5rem' }}>
                                {/*Fix dropdown menu to top right corner of card*/}
                                <div style={{ position: 'absolute', top: '0.5rem', right: '0.5rem'}}>
                                    {/*Dropdown menu for delete Shift*/}
                                    <Dropdown>
                                        <Dropdown.Toggle variant="secondary" size="sm">
                                            &#x22EE;
                                        </Dropdown.Toggle>

                                        <Dropdown.Menu>
                                            <Dropdown.Item onClick={(e) => {
                                                e.preventDefault();
                                                setShowModal(true);
                                                }} 
                                                className="text-danger">
                                                Delete Shift
                                            </Dropdown.Item>
                                            <Dropdown.Item onClick={(e) => {
                                                e.preventDefault();
                                                openAddDelivery(shift.id);
                                            }} 
                                                className="text-warning">
                                                Add Deliveries to Shift
                                            </Dropdown.Item>
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </div>

                                {/* Only this part is clickable */}
                                <Link to={`/DelivAssist/Shifts/${shift.id}`} className="text-decoration-none text-dark">
                                    <Card.Title className="fw-bold">
                                        {formatTime(shift.startTime)} {" - "} {formatTime(shift.endTime)}
                                    </Card.Title>
                                    <Card.Text>
                                        <strong>App:</strong> {shift.app} <br />
                                    </Card.Text>
                                </Link>

                                {/*Modal to confirm delete shift*/}
                                <>
                                    <Modal show={showModal} onHide={() => setShowModal(false)} centered size="lg">
                                        <Modal.Header closeButton>
                                            <Modal.Title>Confirm Deletion</Modal.Title>
                                        </Modal.Header>
                                        <Modal.Body>Are you sure you want to delete this shift?</Modal.Body>
                                        <Modal.Footer>
                                            <Button variant="secondary" onClick={() => setShowModal(false)}>
                                                Cancel
                                            </Button>
                                            <Button
                                                variant="danger"
                                                onClick={() => {
                                                    deleteShift(shift.id);
                                                    setShowModal(false);
                                                }}
                                            >
                                                Delete
                                            </Button>
                                        </Modal.Footer>
                                    </Modal>

                                    <Modal show={showSDModal} onHide={() => setShowSDModal(false)} centered size="lg">
                                        <Modal.Header closeButton>
                                            <Modal.Title>Select deliveries to add to shift</Modal.Title>
                                        </Modal.Header>
                                        <Modal.Body>
                                            <FormGroup as={Row} className="d-flex align-items-center mb-2">
                                                <FormLabel column sm={4}>Delivery</FormLabel>
                                                <Col sm={7}>
                                                    <select onChange={(e) => setDeliveryToAdd(Number(e.target.value))}
                                                        className="form-control mb-2" 
                                                        id="da-delivery">
                                                        <option value={-1}></option>
                                                        {deliveries.map((delivery: any) =>
                                                            <option value={delivery.id}>
                                                                {formatTime(delivery.deliveryTime)}
                                                            </option>
                                                        )}
                                                    </select>
                                                </Col>
                                                </FormGroup>
                                            <Button onClick={(e) => {
                                                    e.preventDefault();
                                                    handleAddDelivery();
                                                }} 
                                                className="btn btn-primary">
                                                Add Delivery to Shift
                                            </Button>
                                        </Modal.Body>
                                    </Modal>
                                </>
                            </Card.Body>
                        </Card>
                    </Col>
                )}
            </Row>
        </div>
    );
}