import '../css/register.component.css';
import { Link } from 'react-router-dom';
import { useRegister } from '../hook/useRegister';

export default function RegisterPage() {
    const { form, error, handleChange, handleSubmit } = useRegister();

    return (
        <div className="register-container">
            <Link to="/" className="auth-logo">AndrezOG</Link>
            <div className="register-form">
                <h2>Crear Cuenta</h2>
                <form onSubmit={handleSubmit}>
                    <div className="form-group">
                        <label htmlFor="name">Nombre</label>
                        <input
                            type="text"
                            id="name"
                            name="name"
                            value={form.name}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="lastName">Apellido</label>
                        <input
                            type="text"
                            id="lastName"
                            name="lastName"
                            value={form.lastName}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="email">Correo Electrónico</label>
                        <input
                            type="email"
                            id="email"
                            name="email"
                            value={form.email}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="phoneNumber">Número de Teléfono</label>
                        <input
                            type="text"
                            id="phoneNumber"
                            name="phoneNumber"
                            value={form.phoneNumber}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="country">País</label>
                        <input
                            type="text"
                            id="country"
                            name="country"
                            value={form.country}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="password">Contraseña</label>
                        <input
                            type="password"
                            id="password"
                            name="password"
                            value={form.password}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="form-group">
                        <label htmlFor="confirmPassword">Confirmar Contraseña</label>
                        <input
                            type="password"
                            id="confirmPassword"
                            name="confirmPassword"
                            value={form.confirmPassword}
                            onChange={handleChange}
                            required
                        />
                    </div>

                    {error && <p className="error-message">{error}</p>}

                    <button type="submit">Registrarse</button>
                    <p className="login-link">
                        ¿Ya tienes una cuenta? <Link to="/login">Inicia sesión</Link>
                    </p>
                </form>
            </div>
        </div>
    );
}