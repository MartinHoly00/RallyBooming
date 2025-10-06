export default function Features() {
  const items = [
    {
      title: "Progression",
      text: "Collect XP and invest in speed, handling, nitro, armor.",
    },
    {
      title: "Economy",
      text: "Earn coins, unlock new cars and epic cosmetics.",
    },
    {
      title: "Handling",
      text: "Snappy low poly feel with drift-friendly physics.",
    },
  ];
  return (
    <section className="section" id="features" aria-label="Features">
      <div className="container">
        <h2>Features</h2>
        <div className="grid-3">
          {items.map((x) => (
            <div key={x.title} className="card">
              <h3 style={{ marginTop: 0 }}>{x.title}</h3>
              <p className="muted">{x.text}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
