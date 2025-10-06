export default function Cars() {
  const cars = [
    { name: "Starter Hatch", stat: "Balanced" },
    { name: "Rally Beast", stat: "Acceleration" },
    { name: "Street Phantom", stat: "Top Speed" },
  ];
  return (
    <section className="section" id="cars" aria-label="Cars">
      <div className="container">
        <h2>Cars</h2>
        <div className="grid-3">
          {cars.map((c) => (
            <div key={c.name} className="card">
              <div className="muted">Class</div>
              <h3 style={{ marginTop: 6 }}>{c.name}</h3>
              <div className="muted">Strength: {c.stat}</div>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
