import { useEffect, useRef, useState } from "react";
import { Box, Typography } from "@mui/material";
import { getLatestMeasurements } from "../api/measurementApi";
import { getVariables } from "../api/variableApi";
import MeasurementsTable from "../components/MeasurementsTable";

const POLLING_INTERVAL_MS = 5000; // données live : on rafraîchit automatiquement

function MeasurementsPage() {
    const [measurements, setMeasurements] = useState([]);
    const [variablesById, setVariablesById] = useState({});
    const [lastUpdate, setLastUpdate] = useState(null);
    const pollingRef = useRef(null);

    // Les variables (config quasi-statique) : chargées une seule fois
    useEffect(() => {
        const loadVariables = async () => {
            try {
                const data = await getVariables();
                const map = {};
                data.forEach((v) => {
                    map[v.id] = { variableName: v.variableName, nodeName: v.nodeName };
                });
                setVariablesById(map);
            } catch (error) {
                console.error(error);
            }
        };

        loadVariables();
    }, []);

    // Les mesures (données live) : chargées immédiatement puis en polling
    useEffect(() => {
        const loadMeasurements = async () => {
            try {
                const data = await getLatestMeasurements();
                setMeasurements(data);
                setLastUpdate(new Date());
            } catch (error) {
                console.error(error);
            }
        };

        loadMeasurements();
        pollingRef.current = setInterval(loadMeasurements, POLLING_INTERVAL_MS);

        return () => clearInterval(pollingRef.current);
    }, []);

    return (
        <Box>
            <MeasurementsTable measurements={measurements} variablesById={variablesById} />

            {lastUpdate && (
                <Typography variant="caption" color="text.secondary" sx={{ mt: 1.5, display: "block" }}>
                    Letzte Aktualisierung: {lastUpdate.toLocaleTimeString("de-DE",
                        { hour: "2-digit", minute: "2-digit", second: "2-digit", fractionalSecondDigits: 3, hour12: false })}
                </Typography>
            )}
        </Box>
    );
}

export default MeasurementsPage