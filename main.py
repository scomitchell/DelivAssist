from fastapi import FastAPI
from pydantic import BaseModel
import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt
import io
import base64

app = FastAPI()

class EarningsData(BaseModel):
    dates: list[str]
    earnings: list[float]

class TipsByNeighborhoodData(BaseModel):
    neighborhoods: list[str]
    tipPays: list[float]

class BaseByAppData(BaseModel):
    apps: list[str]
    basePays: list[float]

@app.post("/charts/earnings")
def generate_chart(data: EarningsData):
    plt.figure(figsize=(8, 5))
    plt.plot(data.dates, data.earnings, marker="o")
    plt.title("Earnings over time")
    plt.xlabel("date")
    plt.ylabel("Earnings ($)")
    plt.grid(True)

    buf = io.BytesIO()
    plt.savefig(buf, format="png")
    plt.close()
    buf.seek(0)

    img_base64 = base64.b64encode(buf.read()).decode("utf-8")
    return {"image": img_base64}

@app.post("/charts/tips-neighborhood")
def generate_tips_by_neighborhood(data: TipsByNeighborhoodData):
    plt.figure(figsize=(10, 6))
    plt.bar(data.neighborhoods, data.tipPays, color="skyblue", edgecolor="black")
    plt.title("Average Tips By Neighborhood")
    plt.xlabel("Neighborhood")
    plt.ylabel("Average Tip ($)")
    plt.xticks(rotation=45, ha="right")

    for i, tip in enumerate(data.tipPays):
        plt.text(i, tip + 0.05, f"${tip:.2f}", ha='center')

    plt.tight_layout()
    
    buf = io.BytesIO()
    plt.savefig(buf, format="png")
    plt.close()

    buf.seek(0)
    data_bytes = buf.read()
    print(f"Image bytes length: {len(data_bytes)}")  # Add logging
    
    if len(data_bytes) == 0:
        raise ValueError("Generated image is empty!")

    img_base64 = base64.b64encode(data_bytes).decode("utf-8")
    return {"image": img_base64}

@app.post("/charts/apps-by-base")
def generate_apps_by_base(data: BaseByAppData):
    plt.figure(figsize=(8, 5))
    plt.bar(data.apps, data.basePays, color="skyblue", edgecolor="black")
    plt.title("Average base pays by app")
    plt.xlabel("App")
    plt.ylabel("Average Base Pay ($)")
    plt.xticks(rotation=45, ha="right")

    for i, base in enumerate(data.basePays):
        plt.text(i, base + 0.05, f"${base:.2f}", ha='center')

    buf = io.BytesIO()
    plt.tight_layout()
    plt.savefig(buf, format="png")
    plt.close()
    buf.seek(0)

    img_base64 = base64.b64encode(buf.read()).decode("utf-8")
    return {"image": img_base64}