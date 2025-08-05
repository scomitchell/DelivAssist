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

@app.post("/charts/earnings")
def generate_chart(data: EarningsData):
    plt.figure(figsize=(6, 4))
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