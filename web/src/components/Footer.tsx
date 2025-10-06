export default function Footer() {
  return (
    <footer className="footer">
      <div className="container" role="contentinfo">
        <div>© {new Date().getFullYear()} RallyBooming</div>
        <div className="muted">Made with unity.</div>
        <div className="muted">
          &copy; <a href="https://github.com/MartinHoly00">Martin Holý</a>
        </div>
      </div>
    </footer>
  );
}
