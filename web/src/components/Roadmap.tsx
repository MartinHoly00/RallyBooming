export default function Roadmap() {
  const items = [
    { q: "Now", text: "Core loop, upgrades, coin economy" },
    { q: "Next", text: "More maps, challenges, upgrades" },
    { q: "Later", text: "Multiplayer and more game modes" },
  ];
  return (
    <section className="section" id="roadmap" aria-label="Roadmap">
      <div className="container">
        <h2>Roadmap</h2>
        <div className="grid-3">
          {items.map((i) => (
            <div key={i.q} className="card">
              <div className="muted">{i.q}</div>
              <h3 style={{ marginTop: 6 }}>{i.text}</h3>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
