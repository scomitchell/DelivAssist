import { useState, useEffect } from "react";
import { Card, FormGroup, FormLabel, FormControl, Modal, Button, Row, Col } from "react-bootstrap";
import * as client from "./client";
import type { DeliveryFilters } from "./client";
import '../../index.css';

export default function MyDeliveries() {
    const [myDeliveries, setMyDeliveries] = useState<any>([]);
    const [showForm, setShowForm] = useState(false);
    const [totalPay, setTotalPay] = useState<number | null>(null);
    const [basePay, setBasePay] = useState<number | null>(null);
    const [tipPay, setTipPay] = useState<number | null>(null);
    const [neighborhood, setNeighborhood] = useState<string | null>(null);
    const [neighborhoods, setNeighborhoods] = useState<any>([]);
    const [app, setApp] = useState<string | null>(null);
    const [apps, setApps] = useState<any>([]);
    const [reset, setReset] = useState(false);
    const [error, setError] = useState("");

    const fetchDeliveries = async () => {
        if (totalPay || basePay || tipPay || neighborhood || app) {
            const filters: DeliveryFilters = {
                totalPay: totalPay,
                basePay: basePay,
                tipPay: tipPay,
                customerNeighborhood: neighborhood,
                app: app
            }

            const deliveries = await client.getFilteredDeliveries(filters);
            deliveries.sort((a: any, b: any) => new Date(b.deliveryTime).getTime() - new Date(a.deliveryTime).getTime());
            setMyDeliveries(deliveries);
            setShowForm(false);
            return;
        }

        const deliveries = await client.findUserDeliveries();
        deliveries.sort((a: any, b: any) => new Date(b.deliveryTime).getTime() - new Date(a.deliveryTime).getTime());
        setMyDeliveries(deliveries);
    }

    const fetchNeighborhoods = async () => {
        const userNeighborhoods = await client.findUserNeighborhoods();
        setNeighborhoods(userNeighborhoods);
    }

    const fetchApps = async () => {
        const userApps = await client.findUserApps();
        setApps(userApps);
    }

    const formatTime = (date: string) => {
        const newDate = new Date(date);
        const readable = newDate.toLocaleString();
        return readable;
    }

    const resetFilters = () => {
        setTotalPay(null);
        setBasePay(null);
        setTipPay(null);
        setNeighborhood(null);
        setApp(null);
        setReset(true);
    }

    useEffect(() => {
        fetchDeliveries();
        fetchNeighborhoods();
        fetchApps();
    }, [])

    useEffect(() => {
        const allCleared =
            totalPay === null &&
            basePay === null &&
            tipPay === null &&
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
        <div id="da-my-deliveries" className="mt-3 col-sm-4">
            <Button onClick={() => setShowForm(true)} className="btn btn-warning mb-3 me-2">
                Filter Deliveries
            </Button>
            <Button onClick={resetFilters} className="btn btn-danger mb-3">
                Reset Filters
            </Button>

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


            {myDeliveries.map((delivery: any) => 
                <Card className="mb-2 text-start user-delivery-card">
                    <Card.Body style={{ padding: '0.5rem' }}>
                        <Card.Title className="fw-bold">Total Pay: ${delivery.totalPay.toFixed(2)}</Card.Title>
                        <Card.Text>
                            <strong>Date Completed:</strong> {formatTime(delivery.deliveryTime)} {" "} <br />
                            <strong>Base Pay:</strong> ${delivery.basePay.toFixed(2)} {" "} <br />
                            <strong>Tip Pay:</strong> ${delivery.tipPay.toFixed(2)} {" "} <br />
                            <strong>App:</strong> {delivery.app} {" "} <br />
                            <strong>Restaurant:</strong> {delivery.restaurant} {" "} <br />
                            <strong>Customer Neighborhood:</strong> {delivery.customerNeighborhood} {" "} <br />
                            <strong>Notes:</strong> {delivery.notes} {" "}
                        </Card.Text>
                    </Card.Body>
                </Card>
            )}
        </div>
    );
}