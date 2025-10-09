import { useEffect, useState } from "react";
import { FormGroup, FormControl, FormLabel, Row, Col, Button } from "react-bootstrap";
import * as deliveryClient from "../Deliveries/client";
import * as client from "./client";

export default function PredictEarnings()
{
    const [startTime, setStartTime] = useState("");
    const [endTime, setEndTime] = useState("");
    const [app, setApp] = useState("");
    const [neighborhood, setNeighborhood] = useState("");

    const [neighborhoods, setNeighborhoods] = useState<any>([]);
    const [apps, setApps] = useState<any>([]);

    const [predictedEarnings, setPredictedEarnings] = useState<number | null>(null);

    const trainModel = async () => {
        await client.trainShiftModel();
    }

    const predictEarnings = async () => {
        if (startTime === "" || endTime === "" || app === "" || neighborhood === "") {
            alert("Please complete all fields before submitting");
            return;
        }

        const data = {
            startTime,
            endTime,
            app,
            neighborhood
        };

        const earningsPrediction = await client.predictShift(data);

        setPredictedEarnings(earningsPrediction.predicted_earnings);
    }

    const fetchNeighborhoods = async () => {
        const userNeighborhoods = await deliveryClient.findUserNeighborhoods();
        setNeighborhoods(userNeighborhoods);
    }

    const fetchApps = async () => {
        const userApps = await deliveryClient.findUserApps();
        setApps(userApps);
    }

    useEffect(() => {
        fetchNeighborhoods();
        fetchApps();
        trainModel();
    }, [])

    return (
        <div id="da-predict-form" style={{padding: "1rem"}}>
            <FormGroup as={Row} className="d-flex align-items-center mb-2">
                <FormLabel column sm={2}>Start Time</FormLabel>
                <Col sm={4}>
                    <FormControl 
                        type="time"
                        onChange={(e) => setStartTime(e.target.value)}
                    />
                </Col>
            </FormGroup>
            <FormGroup as={Row} className="d-flex align-items-center mb-2">
                <FormLabel column sm={2}>End Time</FormLabel>
                <Col sm={4}>
                    <FormControl 
                        type="time"
                        onChange={(e) => setEndTime(e.target.value)}
                    />
                </Col>
            </FormGroup>
            <FormGroup as={Row} className="d-flex align-items-center mb-2">
                <FormLabel column sm={2}>App</FormLabel>
                <Col sm={4}>
                    <select onChange={(e) => setApp(e.target.value)}
                        className="form-control mb-2" id="da-app">
                        <option value=""></option>
                        {apps.map((app: any) => 
                            <option value={app}>{app}</option>
                        )}
                    </select>
                </Col>
            </FormGroup>
            <FormGroup as={Row} className="d-flex align-items-center mb-2">
                <FormLabel column sm={2}>Neighborhood</FormLabel>
                <Col sm={4}>
                    <select onChange={(e) => setNeighborhood(e.target.value)}
                        className="form-control mb-2" id="da-neighborhood">
                        <option value=""></option>
                        {neighborhoods.map((neighborhood: any) => 
                            <option value={neighborhood}>{neighborhood}</option>
                        )}
                    </select>
                </Col>
            </FormGroup>
            <Button onClick={predictEarnings} className="btn btn-primary">
                Predict Earnings
            </Button>
            
            <br />
            <br />
            <strong>Predicted Earnings: ${predictedEarnings?.toFixed(2)}</strong>
        </div>
    )
}