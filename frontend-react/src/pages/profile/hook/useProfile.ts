import { useState, useEffect } from 'react';
import type { MyProfileDto } from '../types/profile.types';
import { getPublicProfile } from '../api/profileApi';

export function useProfile() {
    const [profile, setProfile] = useState<MyProfileDto | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        const controller = new AbortController();

        getPublicProfile()
            .then(data => {
                setProfile(data);
                setLoading(false);
            })
            .catch(err => {
                if (!controller.signal.aborted) {
                    setError(err?.response?.data?.message ?? 'No se pudo cargar el perfil.');
                    setLoading(false);
                }
            });

        return () => controller.abort();
    }, []);

    return { profile, loading, error };
}