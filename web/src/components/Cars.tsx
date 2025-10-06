import { cars } from "../data/cars";

export default function Cars() {
  return (
    <section className="section" id="cars" aria-label="Cars">
      <div className="container">
        <h2>Cars</h2>
        <div className="grid-3">
          {cars.map((c) => (
            <div key={c.name} className="card">
              <h3 style={{ marginTop: 6 }}>{c.name}</h3>
              <img src={c.icon} alt="car_icon" />
              <div className="muted">{c.desc}</div>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}
