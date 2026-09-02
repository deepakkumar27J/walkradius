import { useState } from 'react';

export default function WalkForm({ onSubmit, loading }) {
    const [constraintType, setConstraintType] = useState('Duration');
    const [value, setValue] = useState(30);
    const [location, setLocation] = useState(null);
    const [locationLabel, setLocationLabel] = useState(null);

    function useMyLocation() {
        if (!navigator.geolocation) return;
        navigator.geolocation.getCurrentPosition(
            (position) => {
                setLocation({
                    latitude: position.coords.latitude,
                    longitude: position.coords.longitude,
                });
                setLocationLabel("My Location");
            });

    }
    function handleSubmit() {
        const coords = location ?? { latitude: 54.5973, longitude: -5.9301 }; // Default to Belfast if location is not available
        onSubmit({ ...coords, constraintType, value: parseFloat(value) });
    }

    return (
        <div className="walk-form">
            <h1 className="app-title">Walk In Radius</h1>
            <p className="app-subtitle"> Generate a circular walk from your location</p>

            { /* Location */}
            <div className="form-group">
                <label className="form-label">Starting point</label>
                <button className="btn-location" onClick={useMyLocation}>📍 {locationLabel ?? "Use my location (default: Belfast)"}
                </button>
            </div>

            { /* Constraint Type toggle */}
            <div className="form-group">
                <label className="form-label">Walk by</label>
                <div className="toggle-group">
                    <button
                        className={`toggle-button ${constraintType === 'Duration' ? 'active' : ''}`}
                        onClick={() => setConstraintType('Duration')}
                    >
                        Time
                    </button>
                    <button
                        className={`toggle-button ${constraintType === 'Distance' ? 'active' : ''}`}
                        onClick={() => setConstraintType('Distance')}
                    >
                        Distance
                    </button>
                </div>
            </div>

            {/* Value slider */}
            <div className="form-group">
                <label className="form-label">{constraintType === 'Duration' ? `Duration ${value} mins` : `Distance ${value} km`}</label>
                <input
                    type="range"
                    min={constraintType === 'Duration' ? 10 : 0.5}
                    max={constraintType === 'Duration' ? 120 : 10}
                    step={constraintType === 'Duration' ? 5 : 0.5}
                    value={value}
                    onChange={(e) => setValue(e.target.value)}
                    className="slider"
                />
            <div className="slider-labels">
                <span>{constraintType === 'Duration' ? '10 mins' : '0.5 km'}</span>
                <span>{constraintType === 'Duration' ? '120 mins' : '10 km'}</span>
            </div>
            </div>
            {/* Submit button */}
            <button className="btn-generate" onClick={handleSubmit} disabled={loading}
            > 
                {loading ? 'Generating...' : 'Generate Walk'}
            </button>
        </div>
    );
}
