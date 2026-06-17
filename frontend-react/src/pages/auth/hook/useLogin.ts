import { useState, type ChangeEvent, type FormEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import type { LoginRequest } from '../types/auth.types';
import { loginUser } from '../api/authApi';
import { setAuthToken } from '../../../axios/axiosConf';

export function useLogin() {
    const [form, setForm] = useState<LoginRequest>({
        email: '',
        password: ''
    });
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();

    const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setForm(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e: FormEvent) => {
        e.preventDefault();
        setError(null);

        try {
            const res = await loginUser(form);
            setAuthToken(res.token);
            navigate('/');
        } catch (err: unknown) {
            const axiosError = err as { response?: { data?: { message?: string } } };
            setError(axiosError?.response?.data?.message ?? 'Email o contraseña incorrectos.');
        }
    };

    return { form, error, handleChange, handleSubmit };
}