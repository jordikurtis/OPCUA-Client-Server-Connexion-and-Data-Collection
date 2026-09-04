function PlcTable({ plcs }) {
    return (
        <table
            style={{
                width: "100%",
                borderCollapse: "collapse",
                backgroundColor: "white",
            }}
        >
            <thead>
                <tr>
                    <th style={{ padding: "12px", textAlign: "left" }}>ID</th>
                    <th style={{ padding: "12px", textAlign: "left" }}>Name</th>
                    <th style={{ padding: "12px", textAlign: "left" }}>Enabled</th>
                    <th style={{ padding: "12px", textAlign: "left" }}>Group</th>
                </tr>
            </thead>

            <tbody>
                {plcs.map((plc) => (
                    <tr key={plc.id}>
                        <td style={{ padding: "12px" }}>{plc.id}</td>
                        <td style={{ padding: "12px" }}>{plc.name}</td>

                        <td style={{ padding: "12px" }}>
                            {plc.enabled ? "✅ Yes" : "❌ No"}
                        </td>

                        <td style={{ padding: "12px" }}>
                            {plc.groupId ?? "-"}
                        </td>
                    </tr>
                ))}
            </tbody>
        </table>
    );
}

export default PlcTable;