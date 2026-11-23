
const sql = require("mssql");

const config = {
    server: process.env.DB_SERVER,
    port: Number(process.env.DB_PORT || 1433),
    database: process.env.DB_DATABASE,
    user: process.env.DB_USER,
    password: process.env.DB_PASSWORD,

    options: {
        encrypt: true,
        trustServerCertificate: true
    },

    pool: {
        max: 10,
        min: 0,
        idleTimeoutMillis: 30000
    }
};

let poolPromise;

async function getPool() {
    if (!poolPromise) {
        poolPromise = sql.connect(config).catch(error => {
            poolPromise = null;
            throw error;
        });
    }

    return poolPromise;
}

module.exports = {
    sql,
    getPool
};
