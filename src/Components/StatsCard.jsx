import { Card, Box, Typography } from "@mui/material";

function StatsCard({ title, value, icon, accentColor = "#0891B2" }) {
  return (
    <Card sx={{ p: 2.5, height: "100%" }}>
      <Box sx={{ display: "flex", alignItems: "flex-start", justifyContent: "space-between" }}>
        <Box>
          <Typography variant="subtitle2" sx={{ mb: 0.5 }}>
            {title}
          </Typography>
          <Typography
            variant="h4"
            sx={{ fontFamily: '"Roboto Mono", ui-monospace, monospace' }}
          >
            {value}
          </Typography>
        </Box>

        {icon && (
          <Box
            sx={{
              width: 40,
              height: 40,
              borderRadius: 2,
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              backgroundColor: `${accentColor}1A`, // accent à 10% d'opacité
              color: accentColor,
            }}
          >
            {icon}
          </Box>
        )}
      </Box>
    </Card>
  );
}

export default StatsCard;
