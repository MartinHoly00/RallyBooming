import { createBrowserRouter, Outlet, RouterProvider } from "react-router-dom";
import Home from "./pages/Home";
import New from "./pages/New";
import { NavBar } from "./components";

import logo from "./assets/rallyBoomin-icon.png";

function Layout() {
  return (
    <>
      <NavBar logo={logo} />
      <Outlet />
    </>
  );
}

function App() {
  const baseUrl = import.meta.env.BASE_URL;

  const router = createBrowserRouter([
    {
      path: `${baseUrl}`,
      element: <Layout />,
      children: [
        {
          index: true,
          element: <Home />,
        },
        {
          path: "new",
          element: <New />,
        },
      ],
    },
  ]);

  return <RouterProvider router={router} />;
}

export default App;
