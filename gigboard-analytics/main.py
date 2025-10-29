from fastapi import FastAPI
from pydantic import BaseModel
import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt
import plotly.express as px
import pandas as pd
import os
import joblib
from sklearn.ensemble import RandomForestRegressor
from sklearn.preprocessing import OneHotEncoder
from datetime import datetime
import io
import base64
import os
import uvicorn

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

class HourlyPayData(BaseModel):
    hours: list[str]
    earnings: list[float]

class TrainingSample(BaseModel):
    start_time: str
    end_time: str
    app: str
    neighborhoods: list[str]
    earnings: float

class TrainingData(BaseModel):
    samples: list[TrainingSample]

class PredictionSample(BaseModel):
    start_time: str
    end_time: str
    app: str
    neighborhood: str

@app.post("/charts/earnings")
def generate_chart(data: EarningsData):
    plt.figure(figsize=(8, 5))
    plt.plot(data.dates, data.earnings, marker="o")
    plt.title("Earnings over time")
    plt.xlabel("Date")
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
    plt.figure(figsize=(8, 6))
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

@app.post("/charts/hourly-earnings")
def generate_hourly_chart(data: HourlyPayData):
    plt.figure(figsize=(8, 6))
    plt.plot(data.hours, data.earnings, marker="o")
    plt.title("Average earnings by hour last week")
    plt.xlabel("Hour")
    plt.ylabel("Earnings ($)")
    plt.grid(True)

    buf = io.BytesIO()
    plt.savefig(buf, format="png")
    plt.close()
    buf.seek(0)

    img_base64 = base64.b64encode(buf.read()).decode("utf-8")
    return {"image": img_base64}

@app.post("/train/shift-model")
def train_shift_model(data: TrainingData):
    df = pd.DataFrame([sample.dict() for sample in data.samples])

    df = df.explode("neighborhoods")
    df.rename(columns={"neighborhoods": "neighborhood"}, inplace=True)

    # Convert start time to minutes
    df["start_minutes"] = df["start_time"].apply(lambda t: time_str_to_minutes(t))
    df["end_minutes"] = df["end_time"].apply(lambda t: time_str_to_minutes(t))
    df["duration_hours"] = (df["end_minutes"] - df["start_minutes"]) / 60.0

    df["earnings_per_hour"] = df["earnings"] / df["duration_hours"]

    X = df[["start_minutes", "duration_hours", "app", "neighborhood"]]
    y = df["earnings_per_hour"]

    # Encode Categorical Data
    encoder = OneHotEncoder()
    X_encoded = encoder.fit_transform(X[["app", "neighborhood"]]).toarray()

    encoded_feature_names = encoder.get_feature_names_out(["app", "neighborhood"])

    # Join numeric and encoded categorical data tables
    X_final = pd.concat([X[["start_minutes", "duration_hours"]].reset_index(drop=True),
                         pd.DataFrame(X_encoded, columns=encoded_feature_names)], axis=1)
    
    model = RandomForestRegressor(n_estimators=100, random_state=42)
    model.fit(X_final, y)

    joblib.dump(model, "shift_model.pkl")
    joblib.dump(encoder, "encoder.pkl")

    return {"message": "Model trained successfully"}

@app.post("/predict/shift-earnings")
def predict_shift_earnings(data: PredictionSample):
    if not os.path.exists("shift_model.pkl") or not os.path.exists("encoder.pkl"):
        return {"error": "Model or encoder not found. Please train the model first."}

    # Load model and encodere
    model = joblib.load("shift_model.pkl")
    encoder = joblib.load("encoder.pkl")

    # Convert data for prediction to accepted format
    start_minutes = time_str_to_minutes(data.start_time)
    end_minutes = time_str_to_minutes(data.end_time)
    duration_hours = (end_minutes - start_minutes) / 60.0

    # Create a df for prediction data
    input_df = pd.DataFrame([{
        "start_minutes": start_minutes,
        "duration_hours": duration_hours,
        "app": data.app,
        "neighborhood": data.neighborhood
    }])

    # Encode categorical data and join encoded data to numerical
    encoded = encoder.transform(input_df[["app", "neighborhood"]]).toarray()
    encoded_df = pd.DataFrame(encoded, columns=encoder.get_feature_names_out(["app", "neighborhood"]))

    X_final = pd.concat([
        input_df[["start_minutes", "duration_hours"]].reset_index(drop=True),
        encoded_df.reset_index(drop=True)
    ], axis=1)

    # Predict earnings
    prediction = model.predict(X_final)[0]

    predicted_total_earnings = prediction * duration_hours

    return {"predicted_earnings": round(float(predicted_total_earnings), 2)}

def time_str_to_minutes(t: str) -> int:
    dt = datetime.strptime(t, "%H:%M")
    return dt.hour * 60 + dt.minute

if __name__ == "__main__":
    port = int(os.environ.get("PORT", 10000))
    uvicorn.run(app, host="0.0.0.0", port=port)