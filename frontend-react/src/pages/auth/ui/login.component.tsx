import '../css/login.component.css';
import { Link } from 'react-router-dom';
import { useLogin } from '../hook/useLogin';

export default function LoginPage() {
    const { form, error, handleChange, handleSubmit } = useLogin();

    return (
        <div className="login-container">
            <Link to="/" className="auth-logo">AndrezOG</Link>
            <div className="login-form">
                <h2>Iniciar Sesión</h2>
                <form onSubmit={handleSubmit}>
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

                    {error && <p className="error-message">{error}</p>}

                    <button type="submit">Entrar</button>
                    <p className="register-link">
                        ¿No tienes una cuenta? <Link to="/register">Regístrate</Link>
                    </p>
                </form>
            </div>
        </div>
    );
}