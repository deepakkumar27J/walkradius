export default function RouteInfo({ route }) {
    if (!route) return null;

    return (
        <div className="route-info">
            <div className="route-stat">
                <span className="stat-value">{route.totalDistanceKm} km</span>
                <span className="stat-label">Distance</span>
            </div>
            <div className="route-divider" />
            <div className="route-stat">
                <span className="stat-value">{Math.round(route.totalDurationMinutes)} mins</span>
                <span className="stat-label">Duration</span>
            </div>
            <div className="route-divider" />
            <div className="route-stat">
                <span className="stat-value">{route.waypoints.length}</span>
                <span className="stat-label">Waypoints</span>
            </div>
        </div>
    );
}