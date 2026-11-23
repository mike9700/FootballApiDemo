from fastapi import APIRouter, FastAPI
from fastapi.middleware.cors import CORSMiddleware
from typing import List
from .config import CORS_ORIGINS
from .routers.college_conferences import router as college_conferences_router
from .routers.pro_passing import router as pro_passing_router
from .routers.pro_player import router as pro_player_router

app = FastAPI(title="FootballAPI")

app.add_middleware(
    CORSMiddleware,
    allow_origins=CORS_ORIGINS,
    allow_credentials=False,
    allow_methods=["*"],
    allow_headers=["*"],
)


@app.get("/")
async def root():
    return {"message": "API is running"}


app.include_router(college_conferences_router)
app.include_router(pro_passing_router)
app.inlude_router(pro_player_router)
