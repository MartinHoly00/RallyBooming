export default function Footer() {
  return (
    <footer className="footer">
      <div className="container" role="contentinfo">
        <div>© {new Date().getFullYear()} RallyBooming</div>
        <div className="muted">Made with unity and a lot of drift.</div>
      </div>
    </footer>
  );
}
