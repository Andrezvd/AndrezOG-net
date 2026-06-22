import { useState, type ChangeEvent } from 'react';
import { useNavigate } from 'react-router-dom';
import type { RegisterRequest } from '../types/auth.types';
import { registerUser } from '../api/authApi';

export function useRegister() {
    const [form, setForm] = useState<RegisterRequest>({
        email: '',
        password: '',
        confirmPassword: '',
        name: '',
        lastName: '',
        phoneNumber: '',
        country: ''
    });
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();

    const handleChange = (e: ChangeEvent<HTMLInputElement>) => {
        const { name, value } = e.target;
        setForm(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (e: React.SubmitEvent<HTMLFormElement>) => {
        e.preventDefault();
        setError(null);

        if (form.password !== form.confirmPassword) {
            setError('Las contraseñas no coinciden');
            return;
        }

        try {
            await registerUser(form);
            navigate('/login');
        } catch (err: unknown) {
            const axiosError = err as { response?: { data?: { message?: string } } };
            setError(axiosError?.response?.data?.message ?? 'Error al registrar. Intenta de nuevo.');
        }
    };

    return { form, error, handleChange, handleSubmit };
}