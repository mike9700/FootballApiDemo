const express = require("express");
const router = express.Router();

const { toConferenceStandings } =
    require("../models/conferenceStandings");

const BASE_URL = process.env.BASE_URL;

router.get("/:conferenceId/standings", async (req, res) => {

    const { conferenceId } = req.params;

    const url =
        `${BASE_URL}/apis/v2/sports/football/nfl/standings`;

    try {
        // Call the API
        const response = await fetch(url);

        if (!response.ok) {
            return res.status(502).json({
                message: "Error retrieving NFL standings from ESPN",
                status: response.status
            });
        }

        const data = await response.json();

        // Find the requested conference
        const conference = data.children?.find(
            item =>
                item.id === conferenceId &&
                item.isConference === true
        );

        if (!conference) {
            return res.status(404).json({
                message: `Conference ${conferenceId} not found`
            });
        }

        // Get conference standings
        const entries = conference.standings?.entries ?? [];

        // Flatten the response
        const standings = entries.map(entry =>
            toConferenceStandings(entry)
        );

        return res.json(standings);

    } catch (error) {
        console.error("Error retrieving NFL standings:", error);

        return res.status(500).json({
            message: "An unexpected error occurred retrieving NFL standings"
        });
    }
});

module.exports = router;