import Plot from "react-plotly.js";

export type TipNeighborhoodsProps = {
    data: {
        neighborhoods: string[],
        tipPays: number[]
    };
};

export default function TipsByNeighborhoodChart({data}: TipNeighborhoodsProps) {
    return (
        <Plot 
            data={[
                {
                    x: data.neighborhoods,
                    y: data.tipPays,
                    type: "bar",
                    marker: {color: "royalblue"},
                    name: "Earnings",
                    hoverTemplate: `$%{y:.2f}<br>%{x}`,
                },
            ]}
            layout={{
                height: 450,
                width: 1050,
                title: {text: "Average Tip By Neighborhood", font: { size: 20, weight: "bold"}},
                xaxis: {
                    title: {text: "Neighborhood", font: { size: 16 }, standoff: 10},
                    tickangle: -45,
                    showgrid: true,
                    zeroline: false,
                },
                yaxis: {
                    title: {text: "Average Tip", font: { size: 16 }, standoff: 10},
                    showgrid: true,
                    zeroline: false,
                    tickprefix: "$",
                    tickformat: ".2f",
                },
                plot_bgcolor: "white",
                paper_bgcolor: "white",
                margin: { t: 50, r: 50, b: 80, l: 85},
                dragmode: false
            }}
            config={{
                responsive: true,
                displayModeBar: false,
                displaylogo: false,
                scrollZoom: false,
            }}
            style={{width: "100%", height: "100%"}}
        />
    );
}