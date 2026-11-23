
import os
import pyodbc

from dotenv import load_dotenv

load_dotenv()


def get_db_connection():
    server = os.getenv("DB_SERVER")
    port = os.getenv("DB_PORT")
    database = os.getenv("DB_DATABASE")
    username = os.getenv("DB_USER")
    password = os.getenv("DB_PASSWORD")
    driver = os.getenv(
        "DB_DRIVER",
        "ODBC Driver 18 for SQL Server"
    )

    required_settings = {
        "DB_SERVER": server,
        "DB_DATABASE": database,
        "DB_USER": username,
        "DB_PASSWORD": password
    }

    missing = [
        key for key, value in required_settings.items()
        if not value
    ]

    if missing:
        raise RuntimeError(
            f"Missing database settings: {', '.join(missing)}"
        )

    server_address = f"{server},{port}" if port else server

    connection_string = (
        f"DRIVER={{{driver}}};"
        f"SERVER={server_address};"
        f"DATABASE={database};"
        f"UID={username};"
        f"PWD={password};"
        "Encrypt=yes;"
        "TrustServerCertificate=yes;"
    )

    return pyodbc.connect(
        connection_string,
        timeout=5
    )


def execute_query(query: str, params=()):
    connection = get_db_connection()

    try:
        cursor = connection.cursor()

        try:
            cursor.execute(query, params)

            columns = [column[0] for column in cursor.description]

            return [
                dict(zip(columns, row))
                for row in cursor.fetchall()
            ]

        finally:
            cursor.close()

    finally:
        connection.close()
