from fastapi import APIRouter, FastAPI, HTTPException
from typing import List
import httpx
import pandas as pd
import numpy as np
import nfl_data_py as nfl
import os
from dataclasses import dataclass

from ..models import PasserYpa
LONG_PASS_AIR_YARDS = 20
MIN_ATTEMPTS = {"short": 100, "long": 30}

router = APIRouter(
    prefix="/api",
    tags=["pro-passing"],
)


@router.get("/pass-ypa", response_model=List[PasserYpa])
async def get_passerYpa(start_year: int = 2020, end_year: int = 2023):
    """
    Get passing statistics for NFL players between the specified years.
    """
    try:
        seasons = range(start_year, end_year + 1)

        return get_ypa_leaderboard(pass_length="long", top_n=50, seasons=seasons)

    except Exception as ex:
        raise HTTPException(
            status_code=502,
            detail=f"Error retrieving passing statistics: {str(ex)}"
        )


def load_passing_plays(seasons: range) -> pd.DataFrame:
    """Load play-by-play data and keep only passing plays with air yards."""
    pbp = nfl.import_pbp_data(list(seasons))
    passes = pbp.query("play_type == 'pass' & air_yards.notnull()").copy()

    passes["pass_length_air_yards"] = np.where(
        passes["air_yards"] >= LONG_PASS_AIR_YARDS, "long", "short"
    )
    passes["passing_yards"] = passes["passing_yards"].fillna(0)
    return passes


def ypa_leaderboard(passes: pd.DataFrame, pass_length: str = "long", top_n: int = 10,) -> list[PasserYpa]:
    """Return the top passers by yards per attempt for a season and pass length."""
    min_attempts = MIN_ATTEMPTS[pass_length]

    stats = (
        passes[
            (passes["pass_length_air_yards"] == pass_length)
        ]
        .groupby(["passer_id", "passer"], as_index=False)
        .agg(ypa=("passing_yards", "mean"), n=("passing_yards", "count"))
    )

    top = (
        stats[stats["n"] >= min_attempts]
        .sort_values("ypa", ascending=False)
        .head(top_n)
    )

    return [
        PasserYpa(passer_id=row.passer_id, passer=row.passer, ypa=row.ypa)
        for row in top.itertuples(index=False)
    ]


def get_ypa_leaderboard(
    pass_length: str = "long",
    top_n: int = 10,
    seasons: range = range(2016, 2022 + 1),
) -> list[PasserYpa]:
    passes = load_passing_plays(seasons)
    return ypa_leaderboard(passes, pass_length, top_n)
