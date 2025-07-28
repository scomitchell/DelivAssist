import { Col, Card } from "react-bootstrap";
export default function DelivAssist() {
    return (
        <div id="home-header" className="ms-1">
            <h1>DelivAssist</h1>
            <h2>A dashboard for gig workers</h2>
            <Col sm={6}>
                <Card className="mt-3 user-delivery-card">
                    <Card.Title>Instructions</Card.Title>
                    <Card.Body>
                        <ol>
                            <li>Create an account or sign in to get started</li>
                            <li>
                                Shifts, deliveries, and expenses can be added in any order. 
                                All input fields are required except notes.
                            </li>
                            <li>
                                To see details and deliveries for each shift, click on the card for that shift.
                            </li>
                            <li>
                                When a delivery is added from the delivery page, if a shift exists matching its time and app, it will 
                                automatically be added to the shift. If no matching shift exists, deliveries can be added 
                                manually once one is created
                            </li>
                            <li>
                                When a delivery is added from the shift page, the app and time must match the shift of the
                                current page or it will not succeed.
                            </li>
                            <li>
                                Manually adding a pre-existing delivery to a shift will only succeed if the time and app of 
                                the shift match the delivery.
                            </li>
                            <li>
                                Statistics
                                <ul>
                                    <li>
                                        Best neighborhood is calculated based on average tip
                                    </li>
                                    <li>
                                        Best restaurant is calculated based on average total pay
                                    </li>
                                    <li>
                                        App statistics are based on averages over all deliveries for that app
                                    </li>
                                    <li>
                                        Expense statistics look at all months expenses were logged for.
                                        For example, if you logged 2 months of expenses but only logged type maintenance
                                        for one of those months, it would be averaged over the 2 months you have expenses for,
                                        not just the one month you logged maintenance.
                                    </li>
                                </ul>
                            </li>
                        </ol>
                    </Card.Body>
                </Card>
            </Col>
        </div>
    );
}