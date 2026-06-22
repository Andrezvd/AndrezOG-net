import { useState } from 'react'
import { Link } from 'react-router-dom'
import './App.css'
import { useProfile } from './pages/profile/hook/useProfile'

function App() {
  const [menuOpen, setMenuOpen] = useState(false)
  const { profile } = useProfile()

  return (
    <>
      <main className="landing-shell">
        <div className="landing-backdrop"/>
        <header className="landing-nav">
          <Link to="/" className="auth-logo">AndrezOG</Link>

          <div
            className="menu-wrapper"
            onMouseEnter={() => setMenuOpen(true)}
            onMouseLeave={() => setMenuOpen(false)}
          >
            <button className="menu-trigger">
              Get in Touch
              <span style={{ fontSize: '0.7rem' }}>{menuOpen ? '▲' : '▼'}</span>
            </button>

            {menuOpen && (
              <div className="menu-dropdown">
                <Link to="/register">Cliente</Link>
                <Link to="/login">Soy yo (Login)</Link>
              </div>
            )}
          </div>
        </header>

        <section className="hero-grid-section">
          <div className="hero-grid">

            {/* ===== COLUMNA IZQUIERDA ===== */}
            <div className="hero-left">

              {/* Foto circular */}
              {profile?.photoUrl ? (
                <img
                  src={profile.photoUrl}
                  alt={profile.name}
                  className="hero-avatar"
                />
              ) : (
                <div className="hero-avatar-fallback">
                  {profile?.name?.charAt(0)}{profile?.lastName?.charAt(0)}
                </div>
              )}

              {/* Datos de contacto */}
              <div className="hero-contact">
                {profile?.phoneNumber && <p>📞 {profile.phoneNumber}</p>}
                {profile?.email && <p>📧 {profile.email}</p>}
                {profile?.gitHubUrl && (
                  <a href={profile.gitHubUrl} target="_blank" rel="noopener noreferrer">
                    🔗 GitHub
                  </a>
                )}
                {profile?.linkedInUrl && (
                  <a href={profile.linkedInUrl} target="_blank" rel="noopener noreferrer">
                    🔗 LinkedIn
                  </a>
                )}
                {profile?.education && (
                  <p>🎓 {profile.education} ({profile.educationStartYear} - {profile.educationEndYear})</p>
                )}
                <p className="hero-available">
                  <span className={`dot-available ${profile?.available ? 'dot-green' : 'dot-red'}`} />
                  {profile?.availableText}
                </p>
              </div>

              {/* Skills (placeholder) */}
              <div className="hero-skills">
                <p className="hero-skills-label">Skills</p>
                <div className="hero-skills-placeholder">Skills coming soon...</div>
              </div>

            </div>

            {/* ===== COLUMNA DERECHA ===== */}
            <div className="hero-right">

              {/* Nombre + título */}
              <div className="hero-name-block">
                <h1 className="hero-name">
                  Hola, soy {profile?.name ?? 'Andrés Olivar'} {profile?.lastName ?? ''}
                </h1>
                <p className="hero-title">
                  {profile?.title ?? 'System Engineer / Game Dev / FullStack Developer'}
                </p>
              </div>

              {/* Summary */}
              {profile?.summary && (
                <div className="hero-summary-box">
                  <p>{profile.summary}</p>
                </div>
              )}

            </div>

          </div>
        </section>

        <section className="info-grid" id="about">
          <article>
            <span>01</span>
            <h2>Focus</h2>
            <p>Build clean interfaces, solid APIs and playful experiences with a modern stack.</p>
          </article>
          <article>
            <span>02</span>
            <h2>Style</h2>
            <p>Minimal layout, strong contrast and a cinematic hero that keeps the attention on the message.</p>
          </article>
          <article>
            <span>03</span>
            <h2>Goal</h2>
            <p>A landing page that introduces the portfolio before the rest of the app content.</p>
          </article>
        </section>

        <footer className="landing-footer" id="contact">
          <p>AndrezOG · Portfolio landing</p>
          <a href="mailto:contacto@andrezog.dev">contacto@andrezog.dev</a>
        </footer>
      </main>
    </>
  )
}

export default App