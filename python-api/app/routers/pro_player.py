
import logging
import pyodbc
from fastapi import APIRouter, HTTPException, Path

from app.database import execute_query
from ..models.player import Player
logger = logging.getLogger(__name__)

router = APIRouter(
    prefix="/players",
    tags=["Players"]
)

PLAYER_QUERY = """
    SELECT
        PlayerId,
        FirstName,
        LastName,
        Jersey,
        Position,
        College,
        DraftYear,
        DraftRound,
        DraftPick,
        AccruedSeasons,
        YearsExperience,
        Height,
        Weight,
        Speed,
        ArmLength,
        HandSpan
    FROM Player
"""


# GET /players
# Returns all players
@router.get("", response_model=list[Player])
def get_players():
    try:
        return execute_query(PLAYER_QUERY)

    except (pyodbc.Error, RuntimeError):
        logger.exception("Error retrieving players")

        raise HTTPException(
            status_code=500,
            detail="Error retrieving players"
        )


# GET /players/{player_id}
# Returns a specific player
@router.get("/{player_id}", response_model=Player)
def get_player(
    player_id: int = Path(gt=0)
):
    try:
        query = PLAYER_QUERY + """
            WHERE PlayerId = ?
        """

        players = execute_query(query, (player_id,))

        if not players:
            raise HTTPException(
                status_code=404,
                detail=f"Player {player_id} not found"
            )

        return players[0]

    except HTTPException:
        raise

    except (pyodbc.Error, RuntimeError):
        logger.exception(
            "Error retrieving player %s",
            player_id
        )

        raise HTTPException(
            status_code=500,
            detail="Error retrieving player"
        )
