type NavBarProps = {
  logo: string;
};

export default function NavBar(props: NavBarProps) {
  return (
    <nav className="navbar" aria-label="Primary">
      <div className="container navbar-inner">
        <div className="brand">
          <img src={props.logo} alt="logo" className="logo" />
        </div>
        <div className="nav-links">
          <a href="#features">Features</a>
          <a href="#cars">Cars</a>
          <a href="#roadmap">Roadmap</a>
          <a
            href="#download"
            className="rb-btn"
            style={{ padding: "10px 14px" }}
          >
            Play
          </a>
        </div>
      </div>
    </nav>
  );
}
