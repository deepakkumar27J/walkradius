import { useEffect } from "react";
import { MapContainer, TileLayer, Polyline, Marker, Popup, useMap } from "react-leaflet";
import "leaflet/dist/leaflet.css";
import L from "leaflet";

// Fix Leaflet default marker icon broken in Vite
delete L.Icon.Default.prototype._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png",
  iconUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png",
  shadowUrl: "https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png",
});

// Moves the map to fit the route when it changes
function FitBounds({ positions }) {
  const map = useMap();
  useEffect(() => {
    if (positions.length > 0) {
      map.fitBounds(positions, { padding: [40, 40] });
    }
  }, [positions, map]);
  return null;
}

export default function MapView({ route }) {
  const defaultCenter = [54.5973, -5.9301]; // Belfast

  const positions = route
    ? route.waypoints.map((w) => [w.latitude, w.longitude])
    : [];

  const startPoint = positions[0];

  return (
  <div style={{ height: "100vh", width: "100%" }}>
    <MapContainer
      key="map"
      center={defaultCenter}
      zoom={14}
      style={{ height: "100%", width: "100%" }}
      zoomControl={true}
    >
        <TileLayer
          attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
          url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
        />

        {route && (
          <>
            <FitBounds positions={positions} />
            <Polyline
              positions={positions}
              pathOptions={{ color: "#f59e0b", weight: 4, opacity: 0.9 }}
            />
            <Marker position={startPoint}>
              <Popup>Start / End</Popup>
            </Marker>
          </>
        )}
      </MapContainer>
    </div>
  );
}