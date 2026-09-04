import StatsCard from "../components/StatsCard";

function Dashboard() {
    return (
        <div>
            <h1>Dashboard</h1>

            <div
                style={{
                    display: "flex",
                    gap: "20px",
                    flexWrap: "wrap",
                    marginTop: "20px",
                }}
            >
                <StatsCard title="PLCs" value="0" />

                <StatsCard title="Variables" value="0" />

                <StatsCard title="Groups" value="0" />

                <StatsCard title="Measurements" value="0" />
            </div>
        </div>
    );
}

export default Dashboard;
