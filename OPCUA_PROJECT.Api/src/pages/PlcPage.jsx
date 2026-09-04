import { useEffect, useState } from "react";
import { getPlcs } from "../api/plcApi";
import PlcTable from "../components/PlcTable";
function PlcPage() {

    const [plcs, setPlcs] = useState([]);

    useEffect(() => {

        const loadData = async () => {

            try {

                const data = await getPlcs();

                setPlcs(data);

            }
            catch (error) {

                console.error(error);

            }

        };

        loadData();

    }, []);

    return (
        <div>
            <h1>PLCs</h1>

            <PlcTable plcs={plcs} />
        </div>
    );
}

export default PlcPage;