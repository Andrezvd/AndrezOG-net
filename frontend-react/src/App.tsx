import { useState } from 'react'
import { Link } from 'react-router-dom'
import './App.css'
import { useProfile } from './pages/profile/hook/useProfile'

function App() {
  const [menuOpen, setMenuOpen] = useState(false)
  const { profile } = useProfile()

  return (
    <>
      {/* Video de fondo — fixed para que ocupe todo el viewport sin estirarse con el scroll */}
      <div className="container-video">
        <video autoPlay muted loop className="video-fondo">
          <source src="/videoFondo.mp4" type="video/mp4" />
        </video>
      </div>

      <main className="landing-shell">
        <div className="landing-backdrop" />

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

        <section className="hero-section">
          <p className="eyebrow">{profile?.title ?? 'System Engineer / Game Dev / FullStack Developer'}</p>
          <h1>Hi, I'm {profile?.name ?? 'Andrés Olivar'}</h1>
          <p className="hero-copy">{profile?.summary ?? 'Welcome to my world'}</p>

          <div className="hero-actions">
            <Link to="/register" className="primary-action">
              Get in Touch
            </Link>
            <a className="secondary-action" href="#about">
              Explore more
            </a>
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