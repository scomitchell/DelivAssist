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
                title: {text: "Average Tip By Neighborhood", font: { size: 20 }},
                xaxis: {
                    title: {text: "Neighborhood", font: { size: 16 }},
                    tickangle: -45,
                    showgrid: true,
                    zeroline: false,
                },
                yaxis: {
                    title: {text: "Average Tip", font: { size: 16 }},
                    showgrid: true,
                    zeroline: false,
                    tickprefix: "$",
                    tickformat: ".2f",
                },
                plot_bgcolor: "white",
                paper_bgcolor: "white",
                margin: { t: 50, r: 40, b: 80, l: 60},
            }}
            config={{
                responsive: true,
                displayModeBar: false,
                displaylogo: false,
                scrollZoom: false,
                staticPlot: true
            }}
            style={{width: "100%", height: "100%"}}
        />
    );
}