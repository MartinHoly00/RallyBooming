function NavBar() {
  return (
    <nav className="navbar" aria-label="Primary">
      <div className="container navbar-inner">
        <div className="brand">
          <span className="brand-badge" aria-hidden="true"></span>
          <span>RallyBooming</span>
        </div>
        <div className="nav-links">
          <a href="#features">Features</a>
          <a href="#cars">Cars</a>
          <a href="#roadmap">Roadmap</a>
          <a href="#download" className="rb-btn" style={{padding: '10px 14px'}}>Play</a>
        </div>
      </div>
    </nav>
  )
}

function Hero() {
  return (
    <header className="hero" id="home">
      <div className="container hero-grid">
        <div className="hero-card">
          <div className="hero-media" role="img" aria-label="Low poly car in neon garage">
            <span>Gameplay preview</span>
          </div>
          <div className="hero-content">
            <div className="eyebrow">Low Poly 3D • Arcade RPG</div>
            <h1 className="title-xl">Drive. Level Up. Dominate the Rally.</h1>
            <p className="lead">Collect XP, upgrade your ride, and unlock better cars as you crush the roads. Welcome to RallyBooming.</p>
            <div className="hero-cta">
              <a href="#download" className="rb-btn">Download / Play</a>
              <a href="#features" className="rb-btn secondary">Learn more</a>
            </div>
          </div>
        </div>
        <div className="card" aria-live="polite">
          <h3 style={{marginTop: 0}}>Core Loop</h3>
          <ul style={{margin: 0, paddingLeft: '18px'}}>
            <li>Drive and collect XP or coins</li>
            <li>Level up to unlock upgrades</li>
            <li>Earn money to buy better cars</li>
          </ul>
          <div style={{height: 12}} />
          <h4 style={{marginTop: 0}}>Quick Stats</h4>
          <div className="muted">Procedural tracks • Multiple car classes • Skill-based handling</div>
        </div>
      </div>
    </header>
  )
}

function Features() {
  const items = [
    { title: 'Progression', text: 'Collect XP and invest in speed, handling, nitro, armor.' },
    { title: 'Economy', text: 'Earn coins, unlock new cars and epic cosmetics.' },
    { title: 'Handling', text: 'Snappy low poly feel with drift-friendly physics.' },
  ]
  return (
    <section className="section" id="features" aria-label="Features">
      <div className="container">
        <h2>Features</h2>
        <div className="grid-3">
          {items.map((x) => (
            <div key={x.title} className="card">
              <h3 style={{marginTop: 0}}>{x.title}</h3>
              <p className="muted">{x.text}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  )
}

function Cars() {
  const cars = [
    { name: 'Starter Hatch', stat: 'Balanced' },
    { name: 'Rally Beast', stat: 'Acceleration' },
    { name: 'Street Phantom', stat: 'Top Speed' },
  ]
  return (
    <section className="section" id="cars" aria-label="Cars">
      <div className="container">
        <h2>Cars</h2>
        <div className="grid-3">
          {cars.map((c) => (
            <div key={c.name} className="card">
              <div className="muted">Class</div>
              <h3 style={{marginTop: 6}}>{c.name}</h3>
              <div className="muted">Strength: {c.stat}</div>
            </div>
          ))}
        </div>
      </div>
    </section>
  )
}

function Roadmap() {
  const items = [
    { q: 'Now', text: 'Core loop, upgrades, coin economy' },
    { q: 'Next', text: 'More tracks, challenges, leaderboards' },
    { q: 'Later', text: 'Multiplayer duels and clubs' },
  ]
  return (
    <section className="section" id="roadmap" aria-label="Roadmap">
      <div className="container">
        <h2>Roadmap</h2>
        <div className="grid-3">
          {items.map((i) => (
            <div key={i.q} className="card">
              <div className="muted">{i.q}</div>
              <h3 style={{marginTop: 6}}>{i.text}</h3>
            </div>
          ))}
        </div>
      </div>
    </section>
  )
}

function CTA() {
  return (
    <section className="section" id="download" aria-label="Download">
      <div className="container">
        <div className="hero-card">
          <div className="hero-content">
            <h2 style={{marginTop: 0}}>Ready to hit the road?</h2>
            <p className="muted">Download RallyBooming and start your climb today.</p>
            <div className="hero-cta">
              <a className="rb-btn" href="#">Download for Windows</a>
              <a className="rb-btn secondary" href="#">Wishlist / Follow</a>
            </div>
          </div>
        </div>
      </div>
    </section>
  )
}

function Footer() {
  return (
    <footer className="footer">
      <div className="container" role="contentinfo">
        <div>© {new Date().getFullYear()} RallyBooming</div>
        <div className="muted">Made with unity and a lot of drift.</div>
      </div>
    </footer>
  )
}

function App() {
  return (
    <>
      <NavBar />
      <Hero />
      <Features />
      <Cars />
      <Roadmap />
      <CTA />
      <Footer />
    </>
  )
}

export default App
