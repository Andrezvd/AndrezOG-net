import './App.css'

function App() {
  return (
    <main className="landing-shell">
      <div className="landing-backdrop" />

      <header className="landing-nav">
        <span className="brand">AndrezOG</span>
        <nav className="nav-links" aria-label="Primary">
          <a href="#contact">Get in Touch</a>
        </nav>
      </header>

      <section className="hero-section">
        <p className="eyebrow">System Engineer / Game Dev / FullStack Developer</p>
        <h1>Hi, I'm Andrés Olivar</h1>
        <p className="hero-copy">Welcome to my world</p>

        <div className="hero-actions">
          <a className="primary-action" href="#contact">
            Get in Touch
          </a>
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
  )
}

export default App
