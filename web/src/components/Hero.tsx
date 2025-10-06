export default function Hero() {
  return (
    <header className="hero" id="home">
      <div className="container hero-grid">
        <div className="hero-card">
          <div
            className="hero-media"
            role="img"
            aria-label="Low poly car in neon garage"
          ></div>
          <div className="hero-content">
            <div className="eyebrow">Low Poly 3D • Arcade RPG</div>
            <h1 className="title-xl">Drive. Level Up. Dominate the Rally.</h1>
            <p className="lead">
              Collect XP, upgrade your ride, and unlock better cars as you crush
              the roads. Welcome to RallyBooming.
            </p>
            <div className="hero-cta">
              <a href="#download" className="rb-btn">
                Download / Play
              </a>
              <a href="#features" className="rb-btn secondary">
                Learn more
              </a>
            </div>
          </div>
        </div>
        <div className="card" aria-live="polite">
          <h3 style={{ marginTop: 0 }}>Core Loop</h3>
          <ul style={{ margin: 0, paddingLeft: "18px" }}>
            <li>Drive and collect XP or coins</li>
            <li>Level up to unlock upgrades</li>
            <li>Earn money to buy better cars</li>
          </ul>
          <div style={{ height: 12 }} />
          <h4 style={{ marginTop: 0 }}>Quick Stats</h4>
          <div className="muted">
            Procedural tracks • Multiple car classes • Skill-based handling
          </div>
        </div>
      </div>
    </header>
  );
}
