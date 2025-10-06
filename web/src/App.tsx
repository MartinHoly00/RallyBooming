import logo from "./assets/icon.png";
import {
  Cars,
  CTA,
  Features,
  Footer,
  Hero,
  NavBar,
  Roadmap,
} from "./components";

function App() {
  return (
    <>
      <NavBar logo={logo} />
      <Hero />
      <Features />
      <Cars />
      <Roadmap />
      <CTA />
      <Footer />
    </>
  );
}

export default App;
