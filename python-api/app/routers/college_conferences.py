from fastapi import APIRouter, FastAPI, HTTPException
from typing import List
import httpx

from ..config import BASE_URL
from ..models import Conference, ConferenceStandings

router = APIRouter(
    prefix="/api",
    tags=["college-conferences"],
)


@router.get("/college-conferences", response_model=List[Conference])
async def get_conferences():

    url = f"{BASE_URL}/apis/v2/sports/football/college-football/standings"

    try:
        async with httpx.AsyncClient(timeout=30.0) as client:
            response = await client.get(url)
            response.raise_for_status()

            data = response.json()

        # returns conferences inside the children array
        conferences = data.get("children", [])

        # Flatten the response to only the fields we need
        results = [
            Conference(
                uid=conference["uid"],
                id=conference["id"],
                name=conference["name"],
                abbreviation=conference["abbreviation"],
                shortName=conference["shortName"]
            )
            for conference in conferences
            if conference.get("isConference") is True
        ]

        return results

    except httpx.HTTPError as ex:
        raise HTTPException(
            status_code=502,
            detail=f"Error retrieving conferences: {str(ex)}"
        )


@router.get("/college-conferences/{conference_id}/standings", response_model=List[ConferenceStandings])
async def get_conferencestandings(conference_id: str):

    url = f"{BASE_URL}/apis/v2/sports/football/college-football/standings"
    try:
        async with httpx.AsyncClient(timeout=30.0) as client:
            response = await client.get(url)
            response.raise_for_status()

            data = response.json()

        # returns conferences inside the children array
        conferences = data.get("children", [])

        # Find the requested conference
        conference = next(
            (
                item for item in data.get("children", [])
                if item.get("id") == conference_id
                and item.get("isConference") is True
            ),
            None
        )
        if not conference:
            raise HTTPException(
                status_code=404,
                detail=f"Conference with ID {conference_id} not found"
            )

        # Get the teams in the conference standings
        entries = conference.get("standings", {}).get("entries", [])

        results = []

        for entry in entries:

            team = entry.get("team", {})
            stats = entry.get("stats", [])

            # Overall record is stored as a summary, e.g. "8-2"
            record_stat = next(
                (
                    stat for stat in stats
                    if stat.get("type") == "total"
                ),
                {}
            )

            record = record_stat.get("summary", "0-0")

            # Parse wins, losses, and ties from the overall record
            record_parts = record.split("-")

            wins = int(record_parts[0]) if len(record_parts) > 0 else 0
            losses = int(record_parts[1]) if len(record_parts) > 1 else 0
            ties = int(record_parts[2]) if len(record_parts) > 2 else 0

            results.append(
                ConferenceStandings(
                    uid=team.get("uid", ""),
                    id=team.get("id", ""),
                    name=team.get("displayName", ""),

                    gamesBehind=float(
                        get_stat_value(stats, "gamesBehind")
                    ),

                    leagueWinPercentage=float(
                        get_stat_value(stats, "leagueWinPercent")
                    ),

                    playoffSeed=int(
                        get_stat_value(stats, "playoffSeed")
                    ),

                    pointDifferential=get_stat_display_value(
                        stats, "pointDifferential"
                    ),

                    pointsAgainst=int(
                        get_stat_value(stats, "pointsAgainst")
                    ),

                    pointsFor=int(
                        get_stat_value(stats, "pointsFor")
                    ),

                    streak=get_stat_display_value(stats, "streak"),

                    wins=wins,
                    losses=losses,
                    ties=ties,

                    divisionWins=int(
                        get_stat_value(stats, "divisionWins")
                    ),

                    divisionLosses=int(
                        get_stat_value(stats, "divisionLosses")
                    ),

                    divisionTies=int(
                        get_stat_value(stats, "divisionTies")
                    ),

                    record=record,

                    gamesBack=get_stat_display_value(
                        stats, "gamesBehind"
                    ),

                    lockedDivRank=int(
                        get_stat_value(stats, "lockedDivRank")
                    )
                )
            )

        return results
    except httpx.HTTPError as ex:
        raise HTTPException(
            status_code=502,
            detail=f"Error retrieving conferences: {str(ex)}"
        )


def get_stat(stats: list, stat_name: str):
    """
    Find a statistic by its name in the stats array.
    """
    for stat in stats:
        if stat.get("name") == stat_name:
            return stat

    return {}


def get_stat_value(stats: list, stat_name: str, default=0):
    """
    Return the numeric value of a statistic.
    """
    stat = get_stat(stats, stat_name)

    return stat.get("value", default)


def get_stat_display_value(stats: list, stat_name: str, default=""):
    """
    Return the formatted display value of a statistic.
    """
    stat = get_stat(stats, stat_name)

    return stat.get("displayValue", default)
