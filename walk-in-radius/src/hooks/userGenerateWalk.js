import {useState} from 'react';
const API_URL = import.meta.env.VITE_API_URL;

export function useUserGenerateWalk () {
  const [route, setRoute] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  async function generateWalk ({latitude, longitude, constraintType, value}) {
    setLoading(true);
    setError(null);
    setRoute(null);
    try {
      const res = await fetch(`${API_URL}/api/walk`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          latitude,
          longitude,
          constraintType,
            value}),
        });
        if(!res.ok) {
            const errorData = await res.json();
            throw new Error(errorData.message || 'Failed to generate walk');
        }
        const data = await res.json();
        setRoute(data);
    } catch (err) {
        setError(err.message);
    } finally {
        setLoading(false);
    }
  }
  return {route, loading, error, generateWalk};
}
