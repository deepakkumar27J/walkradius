import { useGenerateWalk } from "./hooks/useGenerateWalk";
import WalkForm from "./components/WalkForm";
import MapView from "./components/MapView";
import RouteInfo from "./components/RouteInfo";
import "./App.css";

export default function App() {
  const { route, loading, error, generateWalk } = useGenerateWalk();

  return (
    <div className="app">
      <div className="sidebar">
        <WalkForm onSubmit={generateWalk} loading={loading} />
        {error && <p className="error-msg">⚠️ {error}</p>}
        <RouteInfo route={route} />
      </div>
      <div className="map-area">
        <MapView route={route} />
      </div>
    </div>
  );
}