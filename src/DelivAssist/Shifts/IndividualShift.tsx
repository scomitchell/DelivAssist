import { useEffect, useState} from "react";
import {Col, Card} from "react-bootstrap";
import { useParams } from "react-router-dom";
import * as client from "./client";

export default function IndividualShift() {
    const { shiftId } = useParams();

    const [shiftDeliveries, setShiftDeliveries] = useState<any[]>([]);
    const [shift, setShift] = useState<any>({});

    const fetchShift = async () => {
        const userShift = await client.findShiftById(Number(shiftId));
        setShift(userShift);
    }

    const fetchDeliveriesForShift = async () => {
        const userShiftDeliveries = await client.findDeliveriesForShift(Number(shiftId));
        setShiftDeliveries(userShiftDeliveries);
    }

    // Display time as date, time
    const formatTime = (date: string) => {
        const newDate = new Date(date);
        const readable = newDate.toLocaleString();
        return readable;
    }

    useEffect(() => {
        fetchShift();
        fetchDeliveriesForShift();
    }, [])

    return (
        <div id="individiual-shifts">
            <h1>Details for shift: {formatTime(shift.startTime)} - {formatTime(shift.endTime)}</h1>
            <h2 className="mb-3">App: {shift.app}</h2>
            <h3>Deliveries:</h3>
            {shiftDeliveries.map((delivery: any) => 
                <Col sm={6}>
                    <Card className="mb-3 text-start user-delivery-card">
                        <Card.Body>
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
                        </Card.Body>
                    </Card> 
                </Col>
            )}
        </div>
    );
}