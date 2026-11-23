function getStat(stats, name) {
    return stats.find(stat => stat.name === name);
}

function getStatValue(stats, name, defaultValue = 0) {
    return getStat(stats, name)?.value ?? defaultValue;
}

function getStatDisplayValue(stats, name, defaultValue = "") {
    return getStat(stats, name)?.displayValue ?? defaultValue;
}

function getRecord(stats) {
    return stats.find(stat => stat.type === "total")?.summary ?? "0-0";
}

function toConferenceStandings(entry) {
    const team = entry.team;
    const stats = entry.stats;

    return {
        uid: team.uid,
        id: team.id,
        name: team.displayName,

        gamesBehind: Number(getStatValue(stats, "gamesBehind")),
        leagueWinPercentage: Number(getStatValue(stats, "winPercent")),
        playoffSeed: Number(getStatValue(stats, "playoffSeed")),

        pointDifferential: getStatDisplayValue(stats, "pointDifferential"),
        pointsAgainst: Number(getStatValue(stats, "pointsAgainst")),
        pointsFor: Number(getStatValue(stats, "pointsFor")),

        streak: getStatDisplayValue(stats, "streak"),

        wins: Number(getStatValue(stats, "wins")),
        losses: Number(getStatValue(stats, "losses")),
        ties: Number(getStatValue(stats, "ties")),

        divisionWins: Number(getStatValue(stats, "divisionWins")),
        divisionLosses: Number(getStatValue(stats, "divisionLosses")),
        divisionTies: Number(getStatValue(stats, "divisionTies")),

        record: getRecord(stats),

        gamesBack: getStatDisplayValue(stats, "gamesBehind"),

        lockedDivRank: Number(getStatValue(stats, "lockedDivRank"))
    };
}

module.exports = {
    toConferenceStandings
};