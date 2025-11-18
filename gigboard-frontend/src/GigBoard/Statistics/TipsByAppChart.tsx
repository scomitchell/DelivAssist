import Plot from "react-plotly.js";

export type TipsByAppProps = {
    data: {
        apps: string[],
        tipPays: number[]
    }
};

export default function TipsByAppChart({data}: TipsByAppProps) {
    const chartData = [
        {
            x: data.apps,
            y: data.tipPays,
            type: "bar",
            marker: { color: "royalblue" },
            name: "Tips by app",
            hoverTemplate: `$%{y:.2f}<br>%{x}`,
        }
    ];

    const layout = {
        autosize: true,
        automargin: true,
        title: { text: "Average Tip by App", font: { size: 20, weight: "bold" }},
        xaxis: {
            title: {text: "App", font: { size: 16 }, standoff: 10},
            tickangle: -30,
            zeroline: false,
            showgrid: true
        },
        yaxis: {
            title: {text: "Average Tip ($)", font: {size: 20}, weight: "bold"},
            showgrid: true,
            zeroline: false,
            tickprefix: "$",
            tickformat: ".2f"
        },
        plot_bgcolor: "white",
        paper_bgcolor: "white",
        dragmode: false
    };

    return (
        <div style={{
            minHeight: 450,
            minWidth: 0,
            width: "100%",
            position: "relative",
            overflowX: "auto"
        }}>
            <Plot 
                data={chartData}
                layout={layout}
                config={{displayModeBar: false}}
                style={{height: "100%", width: "100%"}}
            />
        </div>
    )
}