// Formate un timestamp ISO avec millisecondes — ex: "14:53:41.327"
// Utile en contexte industriel où la précision compte (distinguer
// deux mesures Echtzeit rapprochées).
export function formatTimeWithMs(isoString) {
	if (!isoString) return "—";
	const date = new Date(isoString);
	const time = date.toLocaleTimeString("de-DE", { hour12: false });
	const ms = date.getMilliseconds().toString().padStart(3, "0");
	return `${time}.${ms}`;
}

// Formate une durée écoulée depuis un timestamp — ex: "vor 12s", "vor 3min"
export function formatRelativeSince(isoString) {
	if (!isoString) return "—";
	const diffMs = Date.now() - new Date(isoString).getTime();
	const diffSec = Math.floor(diffMs / 1000);

	if (diffSec < 5) return "gerade eben";
	if (diffSec < 60) return `vor ${diffSec}s`;

	const diffMin = Math.floor(diffSec / 60);
	if (diffMin < 60) return `vor ${diffMin}min`;

	const diffH = Math.floor(diffMin / 60);
	return `vor ${diffH}h`;
}
