import * as client from "./client";
import { useEffect, useState } from "react";
import { Row, Col, Card } from "react-bootstrap";
export default function Statistics() {
    // Pay statistics
    const [averagePay, setAveragePay] = useState(0);
    const [averageBase, setAverageBase] = useState(0);
    const [averageTip, setAverageTip] = useState(0);
    const [avgDollarPerMile, setAvgDollarPerMile] = useState(0);

    // Location Statistics
    const [neighborhood, setNeighborhood] = useState({ neighborhood: "", averageTipPay: 0 });
    const [restaurant, setRestaurant] = useState({ restaurant: "", avgTotalPay: 0 });

    // App statistics
    const [baseApp, setBaseApp] = useState({ app: "", avgBase: 0 });
    const [tipApp, setTipApp] = useState({ app: "", avgTip: 0 });

    const fetchStatistics = async () => {
        // Fetch statistics
        const avgPay = await client.findAvgDeliveryPay();
        const avgBase = await client.findAvgBasePay();
        const avgTip = await client.findAvgTipPay();
        const dollarPerMile = await client.findDollarPerMile();

        const bestNeighborhood = await client.findHighestPayingNeighborhood();
        const bestRestaurant = await client.findHighestPayingRestaurant();

        const bestBaseApp = await client.findHighestPayingBaseApp();
        const bestTipApp = await client.findHighestPayingTipApp();

        // Set statistics
        setAveragePay(avgPay);
        setAverageBase(avgBase);
        setAverageTip(avgTip);
        setAvgDollarPerMile(dollarPerMile);

        setNeighborhood(bestNeighborhood);
        setRestaurant(bestRestaurant);

        setBaseApp(bestBaseApp);
        setTipApp(bestTipApp);
    }

    useEffect(() => {
        fetchStatistics();
    }, [])

    return (
        <div id="da-statistics">
            <h1 className="mb-3">Your Statistics</h1>
            <div id="stats" className="d-flex">
                <Row>
                    {/*Pay Statistics*/}
                    <Col sm={5}>
                        <Card className="me-1 mb-3 user-delivery-card">
                            <Card.Body style={{ padding: '0.25rem' }}>
                                <Card.Title className="fw-bold">Pay Statistics</Card.Title>
                                <Card.Text>
                                    <strong>Average total pay:</strong> ${averagePay.toFixed(2)} <br />
                                    <strong>Average base pay:</strong> ${averageBase.toFixed(2)} <br/>
                                    <strong>Average tip pay:</strong> ${averageTip.toFixed(2)} <br />
                                    <strong>Average dollar/mile:</strong> ${avgDollarPerMile.toFixed(2)} <br />
                                </Card.Text>
                            </Card.Body>
                        </Card>
                    </Col>

                    {/*Location Statistics*/}
                    <Col sm={5}>
                        <Card className="me-1 mb-3 user-delivery-card">
                            <Card.Body style={{ padding: '0.25rem' }}>
                                <Card.Title className="fw-bold">Location Statistics</Card.Title>
                                <Card.Text>
                                    <strong>Best neighborhood:</strong> {neighborhood.neighborhood} <br />
                                    <span style={{ marginLeft: "1rem" }}>
                                        <strong>- Average Tip:</strong> ${neighborhood.averageTipPay.toFixed(2)}
                                    </span> <br />
                                    <strong>Best restaurant:</strong> {restaurant.restaurant} <br />
                                    <span style={{ marginLeft: "1rem" }}>
                                        <strong>- Average Total:</strong> ${restaurant.avgTotalPay.toFixed(2)}
                                    </span>
                                </Card.Text>
                            </Card.Body>
                        </Card>
                    </Col>

                    {/*App Statistics*/}
                    <Col sm={5}>
                        <Card className="me-1 mb-3 user-delivery-card">
                            <Card.Body style={{ padding: '0.25rem' }}>
                                <Card.Title className="fw-bold">App Statistics</Card.Title>
                                <Card.Text>
                                    <strong>App with highest base pay:</strong> {baseApp.app} <br />
                                    <span style={{ marginLeft: "1rem" }}>
                                        <strong>- Average:</strong> ${baseApp.avgBase.toFixed(2)}
                                    </span> <br />
                                    <strong>App with highest tip pay:</strong> {tipApp.app} <br />
                                    <span style={{ marginLeft: "1rem" }}>
                                        <strong>- Average:</strong> ${tipApp.avgTip.toFixed(2)}
                                    </span>
                                </Card.Text>
                            </Card.Body>
                        </Card>
                    </Col>
                </Row>
            </div>
        </div>
    );
}