import { Link } from "react-router-dom";

function Sidebar() {
    return (
        <div
            style={{
                width: "250px",
                height: "100vh",
                backgroundColor: "#1e293b",
                color: "white",
                padding: "20px",
            }}
        >
            <h2>OPCUA Platform</h2>

            <ul
                style={{
                    listStyle: "none",
                    padding: 0,
                }}
            >
                <li>
                    <Link to="/" style={{ color: "white" }}>
                        Dashboard
                    </Link>
                </li>

                <li>
                    <Link to="/plcs" style={{ color: "white" }}>
                        PLCs
                    </Link>
                </li>

                <li>
                    <Link to="/variables" style={{ color: "white" }}>
                        Variables
                    </Link>
                </li>

                <li>
                    <Link to="/measurements" style={{ color: "white" }}>
                        Measurements
                    </Link>
                </li>
            </ul>
        </div>
    );
}

export default Sidebar;