import { BrowserRouter, Routes, Route } from "react-router-dom";
import { AuthProvider } from "../context/AuthContext";
import ProtectedRoute from "../components/ProtectedRoute";

import MainLayout from "../layouts/MainLayout";
import LoginPage from "../pages/LoginPage";
import Dashboard from "../pages/Dashboard";
import PlcPage from "../pages/PlcPage";
import PlcDetailPage from "../pages/PlcDetailPage";
import VariablesPage from "../pages/VariablesPage";
import MeasurementsPage from "../pages/MeasurementsPage";
import AdminPage from "../pages/AdminPage";
import PlcVariablesPage from "../pages/PlcVariablesPage";
import StoerungenPage from "../pages/StoerungenPage";

function AppRoutes() {
    return (
        <AuthProvider>
            <BrowserRouter>
                <Routes>
                    {/* Seule route publique — pas de ProtectedRoute ici */}
                    <Route path="/login" element={<LoginPage />} />

                    {/* Tout le reste nécessite d'être connecté */}
                    <Route element={<ProtectedRoute />}>
                        <Route element={<MainLayout />}>
                            <Route path="/" element={<Dashboard />} />
                            <Route path="/plcs" element={<PlcPage />} />
                            <Route path="/plcs/:id" element={<PlcDetailPage />} />
                            <Route path="/variables" element={<VariablesPage />} />
                            <Route path="/measurements" element={<MeasurementsPage />} />

                            {/* Admin en plus réservé au rôle Admin */}
                            <Route element={<ProtectedRoute allowedRoles={["Admin"]} />}>
                                <Route path="/admin" element={<AdminPage />} />
                                <Route path="/admin/plcs/:id" element={<PlcVariablesPage />} />
                            </Route>

                            {/* Fehlerbehandlung réservée Admin + Instandhaltung */}
                            <Route element={<ProtectedRoute allowedRoles={["Admin", "Instandhaltung"]} />}>
                                <Route path="/stoerungen" element={<StoerungenPage />} />
                            </Route>
                        </Route>
                    </Route>
                </Routes>
            </BrowserRouter>
        </AuthProvider>
    );
}

export default AppRoutes;





/*import { BrowserRouter, Routes, Route } from "react-router-dom";
import { AuthProvider } from "../context/AuthContext";
import ProtectedRoute from "../components/ProtectedRoute";

import MainLayout from "../layouts/MainLayout";
import LoginPage from "../pages/LoginPage";
import Dashboard from "../pages/Dashboard";
import PlcPage from "../pages/PlcPage";
import PlcDetailPage from "../pages/PlcDetailPage";
import VariablesPage from "../pages/VariablesPage";
import MeasurementsPage from "../pages/MeasurementsPage";
import AdminPage from "../pages/AdminPage";
import PlcVariablesPage from "../pages/PlcVariablesPage";

function AppRoutes() {
    return (
        <AuthProvider>
            <BrowserRouter>
                <Routes>
                    {/* Seule route publique — pas de ProtectedRoute ici }
                    <Route path="/login" element={<LoginPage />} />

                    {/* Tout le reste nécessite d'être connecté }
                    <Route element={<ProtectedRoute />}>
                        <Route element={<MainLayout />}>
                            <Route path="/" element={<Dashboard />} />
                            <Route path="/plcs" element={<PlcPage />} />
                            <Route path="/plcs/:id" element={<PlcDetailPage />} />
                            <Route path="/variables" element={<VariablesPage />} />
                            <Route path="/measurements" element={<MeasurementsPage />} />

                            {/* Admin en plus réservé au rôle Admin }
                            <Route element={<ProtectedRoute allowedRoles={["Admin"]} />}>
                                <Route path="/admin" element={<AdminPage />} />
                                <Route path="/admin/plcs/:id" element={<PlcVariablesPage />} />
                            </Route>
                        </Route>
                    </Route>
                </Routes>
            </BrowserRouter>
        </AuthProvider>
    );
}

export default AppRoutes;
*/