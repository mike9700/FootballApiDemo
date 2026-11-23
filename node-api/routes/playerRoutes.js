
const express = require("express");
const router = express.Router();

const { sql, getPool } = require("../database");

const playerQuery = `
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
`;

// GET /players
// Returns all players
router.get("/", async (req, res) => {
    try {
        const pool = await getPool();

        const result = await pool.request().query(playerQuery);

        return res.json(result.recordset);

    } catch (error) {
        console.error("Error retrieving players:", error);

        return res.status(500).json({
            message: "Error retrieving players"
        });
    }
});

// GET /players/:playerId
// Returns a specific player
router.get("/:playerId", async (req, res) => {
    try {
        const playerId = Number(req.params.playerId);

        if (!Number.isInteger(playerId) || playerId <= 0) {
            return res.status(400).json({
                message: "Invalid PlayerId"
            });
        }

        const pool = await getPool();

        const result = await pool.request()
            .input("playerId", sql.Int, playerId)
            .query(`${playerQuery} WHERE PlayerId = @playerId`);

        if (result.recordset.length === 0) {
            return res.status(404).json({
                message: `Player ${playerId} not found`
            });
        }

        return res.json(result.recordset[0]);

    } catch (error) {
        console.error("Error retrieving player:", error);

        return res.status(500).json({
            message: "Error retrieving player"
        });
    }
});

module.exports = router;
