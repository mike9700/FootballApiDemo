require("dotenv").config();

const express = require("express");
const app = express();

const conferenceRoutes = require("./routes/conferenceRoutes");
const playerRoutes = require("./routes/playerRoutes");

app.use(express.json());

app.use("/conferences", conferenceRoutes);
app.use("/players", playerRoutes);

const PORT = 3000;

app.listen(PORT, () => {
    console.log(`Node API running on port ${PORT}`);
});