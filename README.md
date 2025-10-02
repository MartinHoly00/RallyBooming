## RallyBooming

An arcade rally racing prototype built with Unity (URP). This repo contains the Unity project and a separate `web/` app (Vite + React) for a landing page or hosting a WebGL build.

### Requirements
- **Unity**: 6000.0.51f1 (Unity 6) or newer in the same major stream
- **Platforms**: macOS, Windows, Linux, WebGL
- Optional (for `web/`): **Node.js** 18+ and npm

### Quick Start (Play in Editor)
1. Open the project in Unity Hub using version 6000.0.51f1.
2. Load the scene at `Assets/Scenes/SampleScene.unity`.
3. Press Play.

### Controls (default)
- WASD / Arrow Keys: Drive
- Space: Brake/Handbrake
- Esc: Pause/Menu

Note: If your project uses the new Input System, controls can be remapped in `Edit → Project Settings → Input System`.

### Project Structure
- `Assets/` — Game assets, scripts, scenes (URP template base)
- `Packages/` — Unity packages (URP, Input System, Post-processing, etc.)
- `ProjectSettings/` — Project configuration
- `web/` — Vite + React app (optional landing site / host for WebGL build)

### Build Instructions (Unity)
1. Set the target platform in `File → Build Settings...`.
2. Add the main scene(s) to the build.
3. Click Build (or Build And Run) and choose an output directory.

Recommended per-platform notes:
- **Windows/macOS/Linux**: Use IL2CPP and Release configuration for final builds.
- **WebGL**:
  - Template: `Default` (configurable in Player Settings)
  - Compression: Gzip/Brotli as needed (serve with correct Content-Encoding)
  - Threads: Disabled by default; enable only if your hosting allows cross-origin isolation
  - Memory: Increase `webGLInitialMemorySize`/`webGLMaximumMemorySize` if you see out-of-memory errors

### Hosting a WebGL Build
After building for WebGL, you will have a folder with `index.html`, `*.data`, `*.wasm`, and `*.js` files. Host them on any static server with correct headers:
- `Content-Type` for `.wasm`: `application/wasm`
- `Content-Encoding` if compressed: `gzip` or `br`

Popular options: GitHub Pages, Netlify, Vercel, Cloudflare Pages, or serving locally with `npx serve`.

### `web/` App (Optional)
This is an independent Vite + React app you can use for a landing page or to embed/redirect to the Unity WebGL build.

Run locally:
```bash
cd web
npm install
npm run dev
```

Build for production:
```bash
cd web
npm run build
npm run preview
```

To integrate a Unity WebGL build:
1. Build the Unity project for WebGL to a folder (e.g., `Build/ WebGL`).
2. Copy the output into `web/public/game/` (create the folder if it does not exist).
3. In the `web/` app, link to `/game/index.html` or create an iframe/embed page.

### Common Issues
- Wrong Unity version: open with exactly 6000.0.51f1 to avoid upgrade prompts.
- WebGL black screen: check console for CORS or Content-Encoding header issues.
- Performance: ensure URP asset and quality settings match target hardware.

### Contributing
Pull requests and issues are welcome. Please describe changes clearly and keep edits scoped.

### License
No license specified yet. If you plan to share or publish, add a license file.


