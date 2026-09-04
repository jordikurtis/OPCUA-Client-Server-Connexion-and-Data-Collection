import { BrowserRouter, Routes, Route } from "react-router-dom";

import MainLayout from "../layouts/MainLayout";

import Dashboard from "../pages/Dashboard";
import PlcPage from "../pages/PlcPage";
import VariablesPage from "../pages/VariablesPage";
import MeasurementsPage from "../pages/MeasurementsPage";

function AppRoutes() {
    return (
        <BrowserRouter>
            <Routes>
                <Route element={<MainLayout />}>
                    <Route path="/" element={<Dashboard />} />
                    <Route path="/plcs" element={<PlcPage />} />
                    <Route path="/variables" element={<VariablesPage />} />
                    <Route path="/measurements" element={<MeasurementsPage />} />
                </Route>
            </Routes>
        </BrowserRouter>
    );
}

export default AppRoutes;