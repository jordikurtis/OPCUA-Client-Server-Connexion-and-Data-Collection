import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../context/AuthContext";

// allowedRoles : optionnel — si fourni, restreint en plus par rôle
// (ex: ["Admin"] pour les routes /admin). Sans, juste "être connecté" suffit.
function ProtectedRoute({ allowedRoles }) {
  const { isAuthenticated, role } = useAuth();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  if (allowedRoles && !allowedRoles.includes(role)) {
    return <Navigate to="/" replace />;
  }

  return <Outlet />;
}

export default ProtectedRoute;
