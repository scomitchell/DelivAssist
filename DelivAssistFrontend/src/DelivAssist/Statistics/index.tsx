import * as client from "./client";
import { useEffect, useState } from "react";
export default function Statistics() {
    const [averagePay, setAveragePay] = useState(0);
    const [averageBase, setAverageBase] = useState(0);
    const [averageTip, setAverageTip] = useState(0);
    const [neighborhood, setNeighborhood] = useState({neighborhood: "", averageTipPay: 0});

    const fetchStatistics = async () => {
        const avgPay = await client.findAvgDeliveryPay();
        const avgBase = await client.findAvgBasePay();
        const avgTip = await client.findAvgTipPay();
        const bestNeighborhood = await client.findHighestPayingNeighborhood();
        setAveragePay(avgPay);
        setAverageBase(avgBase);
        setAverageTip(avgTip);
        setNeighborhood(bestNeighborhood);
    }

    useEffect(() => {
        fetchStatistics();
    }, [])

    return (
        <div id="da-statistics">
            <h1 className="mb-3">Your Statistiscs</h1>
            <div id="stats">
                <p><strong>Average total pay:</strong> ${averagePay.toFixed(2)}</p>
                <p><strong>Average base pay:</strong> ${averageBase.toFixed(2)}</p>
                <p><strong>Average tip pay:</strong> ${averageTip.toFixed(2)}</p>
                <p><strong>Best neighborhood:</strong> {neighborhood.neighborhood} {" "}
                    with an average tip of ${neighborhood.averageTipPay.toFixed(2)}
                </p>
            </div>
        </div>
    );
}