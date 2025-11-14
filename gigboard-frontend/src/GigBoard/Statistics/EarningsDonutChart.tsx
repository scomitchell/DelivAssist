import Plot from "react-plotly.js";

export type EarningsDonutProps = {
    data: {
        totalPay: number,
        totalBasePay: number,
        totalTipPay: number
    };
};

export default function EarningsDonutChart({data}: EarningsDonutProps) {
    const { totalPay, totalBasePay, totalTipPay } = data;

    const chartData = [
        {
            values: [totalBasePay, totalTipPay],
            labels: ["Base Pay", "Tip Pay"],
            type: 'pie',
            hole: 0.8,
            texttemplate: `%{label}<br />%{percent}<br />$%{value:.2f}`,
            textposition: "outside",
            marker: {
                colors: ["#1484d4ff", "#22b900ff"]
            },
            hovertemplate: `%{label}<br />%{percent}<br />$%{value:.2f}<extra></extra>`,
        },
    ];

    const layout = {
        height: 400,
        width: 400,
        showlegend: false,
        margin: { t: 0, b: 0, l: 80, r: 100 },

        annotations: [
            {
                text: `$${totalPay.toFixed(2)}`,
                x: 0.5,
                y: 0.5,
                font: {
                    size: 24,
                    color: "black",
                    weight: "bold"
                },
                showarrow: false
            },
        ],
    };

    return (
        <Plot 
            data={chartData}
            layout={layout}
            config={{displayModeBar: false}}
        />
    );
}