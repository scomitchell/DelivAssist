import Plot from "react-plotly.js";

export type BaseByAppProps = {
    data: {
        apps: string[],
        basePays: number[]
    };
};

export default function BaseByAppsChart({data}: BaseByAppProps) {
    return (
        <Plot 
            data={[{
                x: data.apps,
                y: data.basePays,
                type: "bar",
                marker: {color: "royalblue"},
                name: "Base Pays by App",
                hoverTemplate: `$%{y:.2f}<br>%{x}`,
            }]}
            layout={{
                title: {text: "Average Base Pay by App", font: {size: 20}},
                xaxis: {
                    title: {text: "App", font: {size: 16}},
                    tickangle: -45,
                    showgrid: true,
                    zeroline: false
                },
                yaxis: {
                    title: {text: "Average Base Pay", font: {size: 16}},
                    showgrid: true,
                    zeroline: false,
                    tickprefix: "$",
                    tickformat: ".2f",
                },
                plot_bgcolor: "white",
                paper_bgcolor: "white",
                margin: { t: 50, r: 40, b: 80, l: 60 },
                dragmode: false
            }}
            config={{
                responsive: true,
                displayModeBar: false,
                displaylogo: false,
                scrollZoom: false,
            }}
            style={{ width: "100%", height: "100%" }}
        />
    );
}