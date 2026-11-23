import os

BASE_URL = os.getenv(
    "BASE_URL",
    "https://site.api.espn.com"
)

CORS_ORIGINS = [
    "http://localhost:3000",
    "http://localhost:5173",
    "http://127.0.0.1:3000",
    "http://127.0.0.1:5173",
]


DB_HOST = os.environ.get("DB_HOST")
DB_PORT = os.environ.get("DB_PORT", "1433")
DB_DRIVER = os.environ.get("DB_DRIVER", "ODBC Driver 18 for SQL Server")
