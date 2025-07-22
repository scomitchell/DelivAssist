import { useState, useEffect } from "react";
import { Card, FormGroup, FormLabel, FormControl, Modal, Button, Row, Col, Dropdown } from "react-bootstrap";
import * as client from "./client";
import type { DeliveryFilters } from "./client";
import '../../index.css';

export default function MyDeliveries({ myDeliveries, setMyDeliveries }: {
    myDeliveries: any[],
    setMyDeliveries: React.Dispatch<React.SetStateAction<any[]>>}) {
    // Control Modal
    const [showForm, setShowForm] = useState(false);
    const [showModal, setShowModal] = useState(false);

    // User entered filters
    const [totalPay, setTotalPay] = useState<number | null>(null);
    const [basePay, setBasePay] = useState<number | null>(null);
    const [tipPay, setTipPay] = useState<number | null>(null);
    const [mileage, setMileage] = useState<number | null>(null);
    const [neighborhood, setNeighborhood] = useState<string | null>(null);
    const [app, setApp] = useState<string | null>(null);


    // Items for dropdown filters
    const [neighborhoods, setNeighborhoods] = useState<any>([]);
    const [apps, setApps] = useState<any>([]);

    // Reset and error handling
    const [reset, setReset] = useState(false);
    const [error, setError] = useState("");


    // Initial fetch deliveries
    const fetchDeliveries = async () => {
        // If filters applied, call getFilteredDeliveries, sort by date
        if (totalPay || basePay || tipPay || neighborhood || app || mileage) {
            const filters: DeliveryFilters = {
                totalPay: totalPay,
                basePay: basePay,
                tipPay: tipPay,
                mileage: mileage,
                customerNeighborhood: neighborhood,
                app: app
            }

            const deliveries = await client.getFilteredDeliveries(filters);
            deliveries.sort((a: any, b: any) => new Date(b.deliveryTime).getTime() - new Date(a.deliveryTime).getTime());
            setMyDeliveries(deliveries);
            setShowForm(false);
            return;
        }

        // If no filters retrieve all deliveries, sort by date
        const deliveries = await client.findUserDeliveries();
        deliveries.sort((a: any, b: any) => new Date(b.deliveryTime).getTime() - new Date(a.deliveryTime).getTime());
        setMyDeliveries(deliveries);
    }

    // Deletes delivery from the database
    const deleteDelivery = async (deliveryId: number) => {
        await client.deleteUserDelivery(deliveryId);
        fetchDeliveries();
    }


    // Retrieves list of user neighborhoods for dropdown
    const fetchNeighborhoods = async () => {
        const userNeighborhoods = await client.findUserNeighborhoods();
        setNeighborhoods(userNeighborhoods);
    }


    // Retreieves list of user apps for dropdown
    const fetchApps = async () => {
        const userApps = await client.findUserApps();
        setApps(userApps);
    }

    // Converts datetime to readable format
    const formatTime = (date: string) => {
        const newDate = new Date(date);
        const readable = newDate.toLocaleString();
        return readable;
    }

    // Set all filters to null
    const resetFilters = () => {
        setTotalPay(null);
        setBasePay(null);
        setTipPay(null);
        setMileage(null);
        setNeighborhood(null);
        setApp(null);
        setReset(true);
    }

    // Initial fetch
    useEffect(() => {
        fetchDeliveries();
        fetchNeighborhoods();
        fetchApps();
    }, [])


    // If reset intitiated and all cleared, re-fetch deliveries
    useEffect(() => {
        const allCleared =
            totalPay === null &&
            basePay === null &&
            tipPay === null &&
            mileage === null &&
            neighborhood === null &&
            app === null;

        if (reset && allCleared) {
            fetchDeliveries();
            setReset(false); // Reset the flag
        }
    }, [totalPay, basePay, tipPay, neighborhood, app, reset]);

    if (error.length > 0) {
        return (
            <p>{error}</p>
        );
    }

    return (
        <div id="da-my-deliveries" className="mt-3 col-sm-8">
            <Button onClick={() => setShowForm(true)} className="btn btn-warning mb-3 me-2">
                Filter Deliveries
            </Button>
            <Button onClick={resetFilters} className="btn btn-danger mb-3">
                Reset Filters
            </Button>

            {/*Modal to apply delivery filters*/}
            <Modal show={showForm} onHide={() => setShowForm(false)} centered size="lg">
                <Modal.Header closeButton>
                    <Modal.Title>Filter Deliveries</Modal.Title>
                </Modal.Header>
                <Modal.Body>
                    <div className="filter-deliveries">
                        <FormGroup as={Row} className="mb-3">
                            <FormLabel column sm={4}>Minimum Total Pay</FormLabel>
                            <Col sm={7}>
                                <FormControl
                                    type="number"
                                    value={totalPay === null ? "" : totalPay}
                                    min="1.00"
                                    step="0.01"
                                    onChange={e => setTotalPay(e.target.value === "" ? null : Number(e.target.value))}
                                />
                            </Col>
                        </FormGroup>
                        <FormGroup as={Row} className="mb-3">
                            <FormLabel column sm={4}>Minimum Base Pay</FormLabel>
                            <Col sm={7}>
                                <FormControl
                                    type="number"
                                    value={basePay === null ? "" : basePay}
                                    min="1.00"
                                    step="0.01"
                                    onChange={e => setBasePay(e.target.value === "" ? null : Number(e.target.value))}
                                />
                            </Col>
                        </FormGroup>
                        <FormGroup as={Row} className="mb-3">
                            <FormLabel column sm={4}>Minimum Tip Pay</FormLabel>
                            <Col sm={7}>
                                <FormControl
                                    type="number"
                                    value={tipPay === null ? "" : tipPay}
                                    min="1.00"
                                    step="0.01"
                                    onChange={e => setTipPay(e.target.value === "" ? null : Number(e.target.value))}
                                />
                            </Col>
                        </FormGroup>
                        <FormGroup as={Row} className="mb-3">
                            <FormLabel column sm={4}>Minimum Mileage</FormLabel>
                            <Col sm={7}>
                                <FormControl
                                    type="number"
                                    value={mileage === null ? "" : mileage}
                                    min="1.00"
                                    step="0.01"
                                    onChange={e => setMileage(e.target.value === "" ? null : Number(e.target.value))}
                                />
                            </Col>
                        </FormGroup>
                        <FormGroup as={Row} className="d-flex align-items-center mb-2">
                            <FormLabel column sm={4}>Neighborhood</FormLabel>
                            <Col sm={7}>
                                <select onChange={(e) => setNeighborhood(e.target.value)}
                                    className="form-control mb-2" id="da-app">
                                    <option value=""></option>
                                    {neighborhoods.map((neighborhood: any) => 
                                        <option value={neighborhood}>{neighborhood}</option>
                                    )}
                                </select>
                            </Col>
                        </FormGroup>
                        <FormGroup as={Row} className="d-flex align-items-center mb-2">
                            <FormLabel column sm={4}>Delivery App</FormLabel>
                            <Col sm={7}>
                                <select onChange={(e) => setApp(e.target.value)}
                                    className="form-control mb-2" id="da-app">
                                    <option value=""></option>
                                    {apps.map((app: any) =>
                                        <option value={app}>{app}</option>
                                    )}
                                </select>
                            </Col>
                        </FormGroup>
                        <Button onClick={fetchDeliveries} className="btn btn-primary">
                            Apply Filters
                        </Button>
                    </div>
                </Modal.Body>
            </Modal>

            {/*Render individual delivery details on cards*/}
            <Row>
                {myDeliveries.map((delivery: any) => 
                    <Col sm={6}>
                        <Card className="mb-3 text-start user-delivery-card">
                            <Card.Body style={{ padding: '0.5rem' }}>
                                <div style={{ position: 'absolute', top: '0.5rem', right: '0.5rem' }}>
                                    <Dropdown>
                                        <Dropdown.Toggle variant="secondary" size="sm">
                                            &#x22EE;
                                        </Dropdown.Toggle>

                                        <Dropdown.Menu>
                                            <Dropdown.Item onClick={() => setShowModal(true)} className="text-danger">
                                                Delete Shift
                                            </Dropdown.Item>
                                        </Dropdown.Menu>
                                    </Dropdown>
                                </div>

                                <Card.Title className="fw-bold">Total Pay: ${delivery.totalPay.toFixed(2)}</Card.Title>
                                <Card.Text>
                                    <strong>Date Completed:</strong> {formatTime(delivery.deliveryTime)} {" "} <br />
                                    <strong>Base Pay:</strong> ${delivery.basePay.toFixed(2)} {" "} <br />
                                    <strong>Tip Pay:</strong> ${delivery.tipPay.toFixed(2)} {" "} <br />
                                    <strong>Mileage:</strong> {delivery.mileage.toFixed(2)} {" miles"} <br />
                                    <strong>App:</strong> {delivery.app} {" "} <br />
                                    <strong>Restaurant:</strong> {delivery.restaurant} {" "} <br />
                                    <strong>Customer Neighborhood:</strong> {delivery.customerNeighborhood} {" "} <br />
                                    <strong>Notes:</strong> {delivery.notes} {" "}
                                </Card.Text>

                                <>
                                    <Modal show={showModal} onHide={() => setShowModal(false)} centered>
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
                                                    deleteDelivery(delivery.id);
                                                    setShowModal(false);
                                                }}
                                            >
                                                Delete
                                            </Button>
                                        </Modal.Footer>
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