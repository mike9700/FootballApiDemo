from pydantic import BaseModel


class Conference(BaseModel):
    uid: str
    id: str
    name: str
    abbreviation: str
    shortName: str


class ConferenceStandings(BaseModel):
    uid: str
    id: str
    name: str
    gamesBehind: float
    leagueWinPercentage: float
    playoffSeed: int
    pointDifferential: str
    pointsAgainst: int
    pointsFor: int
    streak: str
    wins: int
    losses: int
    ties: int
    divisionWins: int
    divisionLosses: int
    divisionTies: int
    record: str
    gamesBack: str
    lockedDivRank: int


class PasserYpa(BaseModel):
    passer_id: str
    passer: str
    ypa: float


class Player(BaseModel):
    player_id: int
    first_name: str
    last_name: str
    jersey: str
    position: str
    college: str
    draft_year: int
    draft_round: int
    draft_pick: int
    accrued_seasons: int
    years_experience: int
    height: str
    weight: int
    speed: float
    arm_length: float
    hand_span: float
