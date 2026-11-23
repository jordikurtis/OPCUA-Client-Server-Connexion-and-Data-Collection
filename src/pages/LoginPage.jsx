import { useState } from "react";
import { useNavigate } from "react-router-dom";
import {
    Box,
    Paper,
    TextField,
    Button,
    Typography,
    Alert,
    Stack,
} from "@mui/material";

import TimelineOutlinedIcon from "@mui/icons-material/TimelineOutlined";
import MemoryOutlinedIcon from "@mui/icons-material/MemoryOutlined";
import WarningAmberOutlinedIcon from "@mui/icons-material/WarningAmberOutlined";

import { login as loginApi } from "../api/authApi";
import { useAuth } from "../context/AuthContext";

const SUPPORT_EMAIL = "WILFRIEDJORDAN.TONFACKFOKOUA@TENNECO.com";
const AppVersion = "v1.0.1";

function FeatureCard({ icon, title }) {
    return (
        <Box
            sx={{
                display: "flex",
                alignItems: "center",
                gap: 2,
                p: 2,
                borderRadius: 2,
                backgroundColor: "rgba(255,255,255,0.05)",
                border: "1px solid rgba(255,255,255,0.08)",
                backdropFilter: "blur(6px)",
                width: "fit-content",
                minWidth: 260,
            }}
        >
            {icon}

            <Typography
                sx={{
                    fontWeight: 500,
                }}
            >
                {title}
            </Typography>
        </Box>
    );
}

function LoginPage() {
    const [username, setUsername] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(false);

    const { login } = useAuth();
    const navigate = useNavigate();

    const handleSubmit = async (e) => {
        e.preventDefault();

        setError(null);
        setLoading(true);

        try {
            const data = await loginApi(username, password);

            login(data);
            navigate("/");
        } catch {
            setError("Benutzername oder Passwort falsch.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <Box
            sx={{
                minHeight: "100vh",
                display: "flex",
                overflow: "hidden",
                background:
                    "linear-gradient(180deg, #04172d 0%, #071f3b 100%)",
            }}
        >
            {/* Section gauche */}
            <Box
                sx={{
                    flex: 1,
                    display: { xs: "none", md: "flex" },
                    flexDirection: "column",
                    justifyContent: "center",
                    px: 8,
                    color: "white",
                }}
            >
                <Typography
                    sx={{
                        color: "#67E8F9",
                        fontSize: 13,
                        fontWeight: 700,
                        letterSpacing: 2,
                        textTransform: "uppercase",
                        mb: 1,
                    }}
                >
                    TENNECO
                </Typography>

                <Box
                    sx={{
                        width: 75,
                        height: 3,
                        borderRadius: 1,
                        bgcolor: "#0891B2",
                        mb: 3,
                    }}
                />

                <Typography
                    variant="h4"
                    sx={{
                        fontWeight: 700,
                        mb: 2,
                        maxWidth: 650,
                        letterSpacing: "-0.02em",
                    }}
                >
                    OPC UA Monitoring Platform
                </Typography>

                <Typography
                    variant="h6"
                    sx={{
                        maxWidth: 600,
                        opacity: 0.85,
                        lineHeight: 1.6,
                        fontWeight: 400,
                        mb: 5,
                    }}
                >
                    Echtzeit-Ueberwachung von SPS-Systemen,
                    Historisierung von Messwerten sowie
                    Stoerungs- und Alarmmanagement fuer industrielle Anlagen.
                </Typography>

                <Stack spacing={2.5}>
                    <FeatureCard
                        icon={<MemoryOutlinedIcon sx={{ color: "#67E8F9" }} />}
                        title="SPS Monitoring"
                    />

                    <FeatureCard
                        icon={<TimelineOutlinedIcon sx={{ color: "#67E8F9" }} />}
                        title="Historian & Analytics"
                    />

                    <FeatureCard
                        icon={
                            <WarningAmberOutlinedIcon
                                sx={{ color: "#67E8F9" }}
                            />
                        }
                        title="Alarm & Fault Management"
                    />
                </Stack>
            </Box>

            {/* Section droite */}
            <Box
                sx={{
                    flex: { xs: 1, md: 0.45 },
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    p: 3,
                }}
            >
                <Paper
                    variant="outlined"
                    sx={{
                        width: "100%",
                        maxWidth: 440,
                        p: 5,
                        borderRadius: 3,
                        backgroundColor: "background.paper",
                        boxShadow:
                            "0px 12px 32px rgba(0,0,0,0.15)",
                    }}
                >
                    <Typography
                        sx={{
                            color: "primary.main",
                            fontSize: 12,
                            fontWeight: 700,
                            letterSpacing: 1.5,
                            textTransform: "uppercase",
                            mb: 1,
                        }}
                    >
                        TENNECO
                    </Typography>

                    <Typography
                        variant="h5"
                       
                        sx={{
                            fontWeight: 700,
                            mb: 4,
                        }}
                    >
                        Willkommen
                    </Typography>

                    {error && (
                        <Alert severity="error" sx={{ mb: 3 }}>
                            {error}
                        </Alert>
                    )}

                    <Box component="form" onSubmit={handleSubmit}>
                        <TextField
                            label="Benutzername"
                            value={username}
                            onChange={(e) =>
                                setUsername(e.target.value)
                            }
                            fullWidth
                            autoFocus
                            sx={{ mb: 2.5 }}
                        />

                        <TextField
                            label="Passwort"
                            type="password"
                            value={password}
                            onChange={(e) =>
                                setPassword(e.target.value)
                            }
                            fullWidth
                            sx={{ mb: 3 }}
                        />

                        <Button
                            type="submit"
                            variant="contained"
                            fullWidth
                            size="large"
                            disabled={loading}
                            sx={{
                                py: 1.3,
                                fontWeight: 600,
                            }}
                        >
                            {loading
                                ? "Anmeldung..."
                                : "Anmelden"}
                        </Button>
                    </Box>

                    <Box
                        sx={{
                            mt: 4,
                            pt: 3,
                            borderTop: "1px solid",
                            borderColor: "divider",
                        }}
                    >
                        <Typography
                            variant="caption"
                            color="text.secondary"
                            sx={{
                                display: "block",
                                textAlign: "center",
                                lineHeight: 1.8,
                            }}
                        >
                            Noch kein Zugang ?
                          
                            Konten werden vom Administrator angelegt.
                            <br />
                            Bei Problemen :
                        </Typography>

                        <Typography
                            variant="caption"
                            sx={{
                                color: "primary.main",
                                fontSize: "0.75rem",
                                display: "block",
                                mt: 1.0,
                                textAlign: "center",
                                opacity: 0.8,
                            }}
                        >
                          {SUPPORT_EMAIL}
                        </Typography>

                        <Typography
                            variant="caption"
                            sx={{
                                color: "text.secondary",
                                fontSize: "0.70rem",
                                display: "block",
                                mt: 1.0,
                                textAlign: "center",
                                opacity: 0.8,
                            }}
                        >
                          Version {AppVersion}
                        </Typography>
                    </Box>
                </Paper>
                </Box>
            </Box> 
    );
}

export default LoginPage;

/*
import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Box, Paper, TextField, Button, Typography, Alert } from "@mui/material";
import { login as loginApi } from "../api/authApi";
import { useAuth } from "../context/AuthContext";

function LoginPage() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(false);
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError(null);
    setLoading(true);
    try {
      const data = await loginApi(username, password);
      login(data);
      navigate("/");
    } catch (err) {
      setError("Benutzername oder Passwort falsch.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box
      sx={{
        minHeight: "100vh",
        display: "flex",
        alignItems: "center",
        justifyContent: "center",
        backgroundColor: "#F4F6F8",
      }}
    >
      <Paper sx={{ p: 4, width: 360 }} variant="outlined">
        <Typography variant="h6" sx={{ mb: 3, fontWeight: 700 }}>
          OPCUA Platform
        </Typography>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        <Box component="form" onSubmit={handleSubmit}>
          <TextField
            label="Benutzername"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            fullWidth
            autoFocus
            sx={{ mb: 2 }}
          />
          <TextField
            label="Passwort"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            fullWidth
            sx={{ mb: 3 }}
          />
          <Button type="submit" variant="contained" fullWidth disabled={loading}>
            Anmelden
          </Button>
        </Box>
      </Paper>
    </Box>
  );
}

export default LoginPage;

*/