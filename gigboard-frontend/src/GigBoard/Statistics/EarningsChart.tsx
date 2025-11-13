import Plot from "react-plotly.js";

export type EarningsChartProps = {
    data: {
        dates: string[],
        earnings: number[]
    };
};

export default function EarningsChart({ data }: EarningsChartProps)
{
    return (
        <Plot 
            data={[
                {
                    x: data.dates,
                    y: data.earnings,
                    type: "scatter",
                    mode: "lines+markers",
                    marker: {color: "royalBlue", size: 8},
                    line: {width: 2},
                    name: "Earnings",
                    hovertemplate: '$%{y:.2f}<br>%{x}', 
                },
            ]}
            layout={{
                title: {text: "Earnings Over Time", font: { size: 20 }},
                xaxis: {
                    title: {text: "Date", font: { size: 16 }, standoff: 30},
                    tickangle: -30,
                    showgrid: true,
                    zeroline: false,
                    tickformat: "%b %d, %Y"
                },
                yaxis: {
                    title: {text: "Earnings ($)", font: { size: 16 }},
                    showgrid: true,
                    zeroline: false,
                    tickformat: ".1f",
                },
                plot_bgcolor: "white",
                paper_bgcolor: "white",
                margin: { t: 50, r: 40, b: 70, l: 60 },
                dragmode: false
            }}
            config={{
                responsive: true,
                displaylogo: false,
                scrollZoom: false,
                displayModeBar: false,
            }}
            style={{ width: "100%", height: "100%" }}
        />
    );
}