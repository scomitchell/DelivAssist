import { Col } from "react-bootstrap";
import { Card, CardContent, Typography } from "@mui/material";
export default function GigBoard() {
    return (
        <div id="home-header" className="ms-1">
            <h1>GigBoard</h1>
            <h2>Your insight dashboard</h2>
            <Col sm={6}>
                <Card sx={{
                    mb: 3,
                    textAlign: "start",
                    borderRadius: 3,
                    boxShadow: 3,
                    position: "relative",
                    transition: "0.3s",
                }}>
                    <CardContent sx={{ p: 2}}>
                        <Typography variant="h6" fontWeight="bold">Instructions</Typography>
                        <ol>
                            <li>Create an account or sign in to get started</li>
                            <li>Backend is currently deployed on free tier of Render, please allow 50s spinup time.</li>
                            <li>
                                Shifts, deliveries, and expenses can be added in any order. 
                                All input fields are required except notes.
                            </li>
                            <li>
                                To see details and deliveries for each shift, click on the card for that shift.
                            </li>
                            <li>
                                When a delivery is added from the delivery page, if a shift exists matching its time and app, it will 
                                automatically be added to the shift. If no matching shift exists, the delivery will be added when
                                the shift is created.
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
                    </CardContent>
                </Card>
            </Col>
        </div>
    );
}