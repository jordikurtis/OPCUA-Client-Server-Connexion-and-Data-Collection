function StatsCard({ title, value }) {
    return (
        <div
            style={{
                backgroundColor: "white",
                borderRadius: "12px",
                padding: "20px",
                boxShadow: "0 2px 8px rgba(0,0,0,0.1)",
                minWidth: "200px",
            }}
        >
            <h3>{title}</h3>

            <h1>{value}</h1>
        </div>
    );
}

export default StatsCard;