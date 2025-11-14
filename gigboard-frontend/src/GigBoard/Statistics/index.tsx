import * as client from "./client";
import { useEffect, useState } from "react";
import { Row, Col } from "react-bootstrap";
import { Card, CardContent, Typography } from "@mui/material";
import PredictEarnings from "./PredictEarnings";
import EarningsChart from "./EarningsChart";
import TipsByNeighborhoodChart from "./TipsByNeighborhoodChart";
import BaseByAppsChart from "./BaseByAppsChart";
import HourlyEarningsChart from "./HourlyEarningsChart";
import EarningsDonutChart from "./EarningsDonutChart";
import type { HourlyEarningsProps } from "./HourlyEarningsChart";
import type { EarningsChartProps } from "./EarningsChart";
import type { TipNeighborhoodsProps } from "./TipsByNeighborhoodChart";
import type { BaseByAppProps } from  "./BaseByAppsChart";
import type { EarningsDonutProps } from "./EarningsDonutChart";
import "../../index.css";

type MonthlySpendingType = {
  type: string;
  avgExpense: number;
};

export default function Statistics() {
    // Pay statistics
    const [averagePay, setAveragePay] = useState(0);
    const [averageBase, setAverageBase] = useState(0);
    const [averageTip, setAverageTip] = useState(0);
    const [avgDollarPerMile, setAvgDollarPerMile] = useState(0);
    const [avgTipPerMile, setAvgTipPerMile] = useState(0);

    // Location Statistics
    const [neighborhood, setNeighborhood] = useState({ neighborhood: "", averageTipPay: 0 });
    const [restaurant, setRestaurant] = useState({ restaurant: "", avgTotalPay: 0 });
    const [restaurantWithMost, setRestaurantWithMost] = useState({restaurant: "", orderCount: 0});

    // App statistics
    const [baseApp, setBaseApp] = useState({ app: "", avgBase: 0 });
    const [tipApp, setTipApp] = useState({ app: "", avgTip: 0 });

    // Expense statistics
    const [monthlySpending, setMonthlySpending] = useState(0);
    const [monthlySpendingByType, setMonthlySpendingByType] = useState<MonthlySpendingType[]>([]);

    // Shift statistics
    const [averageShiftLength, setAverageShiftLength] = useState<number | null>(null);
    const [appWithMostShifts, setAppWithMostShifts] = useState<string | null>(null);
    const [avgDeliveriesPerShift, setAvgDeliveriesPerShift] = useState(0);

    // Charts
    const [plotlyEarningsData, setPlotlyEarningsData] = useState<EarningsChartProps["data"] | null>(null);
    const [plotlyTipNeighborhoodsData, setPlotlyTipNeighborhoodsData] = useState<TipNeighborhoodsProps["data"] | null>(null);
    const [plotlyBaseByAppData, setPlotlyBaseByAppData] = useState<BaseByAppProps["data"] | null>(null);
    const [hourlyEarningsData, setHourlyEarningsData] = useState<HourlyEarningsProps["data"] | null>(null);
    const [donutChartData, setDonutChartData] = useState<EarningsDonutProps["data"] | null>(null);

    // Loading
    const [loading, setLoading] = useState(true);

    // Page Control
    const [page, setPage] = useState("stats");


    const fetchStatistics = async () => {
        // Fetch statistics
        try {
            const avgPay = await client.findAvgDeliveryPay();
            const avgBase = await client.findAvgBasePay();
            const avgTip = await client.findAvgTipPay();
            const dollarPerMile = await client.findDollarPerMile();
            const tipPerMile = await client.findAverageTipPerMile();

            setAveragePay(avgPay ?? 0);
            setAverageBase(avgBase ?? 0);
            setAverageTip(avgTip ?? 0);
            setAvgDollarPerMile(dollarPerMile ?? 0);
            setAvgTipPerMile(tipPerMile ?? 0);
        } catch {
            setAveragePay(0)
            setAverageBase(0)
            setAverageTip(0);
            setAvgDollarPerMile(0);
            setAvgTipPerMile(0);
        }

        try {
            const bestNeighborhood = await client.findHighestPayingNeighborhood();
            const bestRestaurant = await client.findHighestPayingRestaurant();
            const restaurantWithMostOrders = await client.findRestaurantWithMostDeliveries();

            setNeighborhood(bestNeighborhood ?? {neighborhood: "N/A", averageTipPay: 0});
            setRestaurant(bestRestaurant ?? {restaurant: "N/A", avgTotalPay: 0});
            setRestaurantWithMost(restaurantWithMostOrders ?? {restaurant: "N/A", orderCount: 0});
        } catch {
            setNeighborhood({neighborhood: "N/A", averageTipPay: 0});
            setRestaurant({restaurant: "N/A", avgTotalPay: 0});
            setRestaurantWithMost({restaurant: "N/A", orderCount: 0});
        }

        try {
            const bestBaseApp = await client.findHighestPayingBaseApp();
            const bestTipApp = await client.findHighestPayingTipApp();

            setBaseApp(bestBaseApp ?? {app: "N/A", avgBase: 0});
            setTipApp(bestTipApp ?? {app: "", avgTip: 0});
        } catch {
            setBaseApp({app: "N/A", avgBase: 0});
            setTipApp({app: "N/A", avgTip: 0});
        }

        try {
            const averageMonthlyExpenses = await client.findAverageMonthlySpending();
            const avgMonthlySpendingByType = await client.findMonthlySpendingByType();

            setMonthlySpending(averageMonthlyExpenses ?? 0);
            setMonthlySpendingByType(avgMonthlySpendingByType ?? []);
        } catch {
            setMonthlySpending(0);
            setMonthlySpendingByType([]);
        }

        try {
            const averageUserShiftLength = await client.findAverageShiftLength();
            const appWithMostUserShifts = await client.findAppWithMostShifts();
            const avgOrdersPerShift = await client.findAverageDeliveriesPerShift();

            setAverageShiftLength(averageUserShiftLength);
            setAppWithMostShifts(appWithMostUserShifts);
            setAvgDeliveriesPerShift(avgOrdersPerShift);
        } catch {
            setAverageShiftLength(0);
            setAppWithMostShifts("N/A");
            setAvgDeliveriesPerShift(0);
        }

        try {
            const userHourlyEarningsData = await client.findHourlyPayData();

            setHourlyEarningsData(userHourlyEarningsData);
        } catch {
            setHourlyEarningsData(null);
        }

        try {
            const userPlotlyEarningsData = await client.findPlotlyEarningsData();
            const userTipNeighborhoodsData = await client.findPlotlyTipNeighborhoodData();
            const userBaseByAppData = await client.findPlotlyBaseByApp();
            const userEarningsDonutData = await client.findDonutChartData();

            setPlotlyEarningsData(userPlotlyEarningsData);
            setPlotlyTipNeighborhoodsData(userTipNeighborhoodsData);
            setPlotlyBaseByAppData(userBaseByAppData);
            setDonutChartData(userEarningsDonutData);
        } catch {
            setPlotlyEarningsData(null);
            setPlotlyTipNeighborhoodsData(null);
            setPlotlyBaseByAppData(null);
            setDonutChartData(null);
        }

        setLoading(false);
    }

    const getContent = () => {
        if (page === "stats") {
            return (
                <div id="stats" className="d-flex">
                <Row>
                    {/*Pay Statistics*/}
                    <Col sm={5}>
                        <Card sx={{
                            mb: 3,
                            textAlign: "start",
                            borderRadius: 3,
                            boxShadow: 3,
                            position: "relative",
                            transition: "0.3s",
                        }}>
                            <CardContent sx={{ p: 2}}>
                                <Typography variant="h6" fontWeight="bold">Pay Statistics</Typography>
                                <Typography variant="body1" sx={{ mt: 1}}>
                                    {loading ? 
                                        <div>
                                            <strong>Average total pay:</strong> Loading... <br />
                                            <strong>Average base pay:</strong> Loading... <br/>
                                            <strong>Average tip pay:</strong> Loading...<br /> <br />
                                            <strong>Average dollar/mile:</strong> Loading...<br />
                                            <strong>Average tip/mile:</strong> Loading...<br />
                                        </div>
                                        :
                                        <div>
                                            <strong>Average total pay:</strong> ${averagePay.toFixed(2)} <br />
                                            <strong>Average base pay:</strong> ${averageBase.toFixed(2)} <br/>
                                            <strong>Average tip pay:</strong> ${averageTip.toFixed(2)} <br /> <br />
                                            <strong>Average dollar/mile:</strong> ${avgDollarPerMile.toFixed(2)} <br />
                                            <strong>Average tip/mile</strong> ${avgTipPerMile.toFixed(2)} <br />
                                        </div>
                                    }
                                </Typography>
                            </CardContent>
                        </Card>
                    </Col>

                    {/*Location Statistics*/}
                    <Col sm={5}>
                            <Card sx={{
                                mb: 3,
                                textAlign: "start",
                                borderRadius: 3,
                                boxShadow: 3,
                                position: "relative",
                                transition: "0.3s",
                            }}>
                            <CardContent sx={{ p: 2}}>
                                <Typography variant="h6" fontWeight="bold">Location Statistics</Typography>
                                <Typography sx={{ mt: 1 }}>
                                    {loading ?
                                    <div>
                                        <strong>Best tip neighborhood:</strong> Loading... <br />
                                        <span style={{ marginLeft: "1rem" }}>
                                            <strong>- Average Tip:</strong> Loading...
                                        </span> <br />
                                        <strong>Best paying restaurant:</strong> Loading... <br />
                                        <span style={{ marginLeft: "1rem" }}>
                                            <strong>- Average Total:</strong> Loading... <br />
                                        </span>
                                        <strong>Restaurant with most orders:</strong> Loading... <br />
                                        <span style={{marginLeft:"1rem"}}>
                                            <strong>- Number of Orders:</strong> Loading...
                                        </span>
                                    </div>
                                    :
                                    <div>
                                        <strong>Best tip neighborhood:</strong> {neighborhood.neighborhood} <br />
                                        <span style={{ marginLeft: "1rem" }}>
                                            <strong>- Average Tip:</strong> ${neighborhood.averageTipPay.toFixed(2)}
                                        </span> <br />
                                        <strong>Best paying restaurant:</strong> {restaurant.restaurant} <br />
                                        <span style={{ marginLeft: "1rem" }}>
                                            <strong>- Average Total:</strong> ${restaurant.avgTotalPay.toFixed(2)} <br />
                                        </span>
                                        <strong>Restaurant with most orders:</strong> {restaurantWithMost.restaurant} <br />
                                        <span style={{marginLeft:"1rem"}}>
                                            <strong>- Number of Orders:</strong> {restaurantWithMost.orderCount}
                                        </span>
                                    </div>
                                    }       
                                </Typography>
                            </CardContent>
                        </Card>
                    </Col>

                    {/*App Statistics*/}
                    <Col sm={5}>
                        <Card sx={{
                                mb: 3,
                                textAlign: "start",
                                borderRadius: 3,
                                boxShadow: 3,
                                position: "relative",
                                transition: "0.3s",
                            }}>
                            <CardContent sx={{ p: 2}}>
                                <Typography variant="h6" fontWeight="bold">App Statistics</Typography>
                                <Typography sx={{ mt: 1 }}>
                                    <strong>App with highest base pay:</strong> {baseApp.app} <br />
                                    <span style={{ marginLeft: "1rem" }}>
                                        <strong>- Average:</strong> ${baseApp.avgBase.toFixed(2)}
                                    </span> <br />
                                    <strong>App with highest tip pay:</strong> {tipApp.app} <br />
                                    <span style={{ marginLeft: "1rem" }}>
                                        <strong>- Average:</strong> ${tipApp.avgTip.toFixed(2)}
                                    </span>
                                </Typography>
                            </CardContent>
                        </Card>
                    </Col>

                    {/*Expense Statistics*/}
                    <Col sm={5}>
                        <Card sx={{
                                mb: 3,
                                textAlign: "start",
                                borderRadius: 3,
                                boxShadow: 3,
                                position: "relative",
                                transition: "0.3s",
                            }}>
                            <CardContent sx={{ p: 2 }}>
                                <Typography variant="h6" fontWeight="bold">Expense Statistics</Typography>
                                <Typography sx={{mt: 1}}>
                                    <strong>Average monthly spending:</strong> ${monthlySpending.toFixed(2)} <br />
                                    <strong>Monthly spending by type:</strong>
                                    <div style={{ marginLeft: "1rem" }}>
                                        {monthlySpendingByType.map((average, idx) => (
                                            <div key={idx}>
                                                <strong>- {average.type}:</strong> ${average.avgExpense.toFixed(2)}
                                            </div>
                                        ))}
                                    </div>
                                </Typography>
                            </CardContent>
                        </Card>
                    </Col>

                    {/*Shift Statistics*/}
                    <Col sm={5}>
                        <Card sx={{
                                mb: 3,
                                textAlign: "start",
                                borderRadius: 3,
                                boxShadow: 3,
                                position: "relative",
                                transition: "0.3s",
                            }}>
                            <CardContent sx={{ p: 2 }}>
                                <Typography variant="h6" fontWeight="bold">Shift Statistics</Typography>
                                <Typography sx={{ mt: 1}}>
                                    <strong>Average shift length:</strong> {averageShiftLength?.toFixed(0)} minutes <br />
                                    <strong>Average number of deliveries per shift:</strong> {Math.floor(avgDeliveriesPerShift)} <br />
                                    <strong>App with most shifts:</strong> {appWithMostShifts} <br />
                                </Typography>
                            </CardContent>
                        </Card>
                    </Col>
                </Row>
            </div>
            );
        } else if (page === "earnings-over-time") {
            return (
                <div id="charts">
                    {plotlyEarningsData && (
                        <EarningsChart data={plotlyEarningsData} />
                    )}
                </div>
            );
        } else if (page === "tips-by-neighborhood") {
            return (
                <div id="charts">
                    {plotlyTipNeighborhoodsData &&
                        <TipsByNeighborhoodChart data={plotlyTipNeighborhoodsData} />
                    }
                </div>
            );
        } else if (page === "base-by-app") {
            return (
                <div id="charts">
                    {plotlyBaseByAppData &&
                        <BaseByAppsChart data={plotlyBaseByAppData} />
                    }
                </div>
            );
        } else if (page === "hourly-pay-chart") {
            return (
                <div id="charts">
                    {hourlyEarningsData && (
                        <HourlyEarningsChart data={hourlyEarningsData} />
                    )}
                </div>
            )
        } else if (page === "earnings-donut") {
            return (
                <div id="charts">
                    {donutChartData && (
                        <EarningsDonutChart data={donutChartData} />
                    )}
                </div>
            );
        } else if (page === "predict-earnings") {
            return (
                <div id="predict-earnings">
                    <PredictEarnings />
                </div>
            );
        }
    }

    useEffect(() => {
        fetchStatistics();
    }, [])

    return (
        <div id="da-statistics">
            <h1 className="mb-3">Your Statistics</h1>
            <Col sm={6}>
                <select onChange={(e) => setPage(e.target.value)} className="form-control mb-3">
                    <option value="stats">Overall Statistics</option>
                    <option value="earnings-donut">Overall Earnings Chart</option>
                    <option value="earnings-over-time">Earnings Over Time Chart</option>
                    <option value="tips-by-neighborhood">Average Tip by Neighborhood Chart</option>
                    <option value="base-by-app">Average Base Pay by App Chart</option>
                    <option value="hourly-pay-chart">Average Hourly Pay Chart</option>
                    <option value="predict-earnings">Predict Earnings</option>
                </select>
            </Col>
            
            {getContent()}
        </div>
    );
}