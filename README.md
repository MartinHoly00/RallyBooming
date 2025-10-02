## RallyBooming

RallyBooming is a low‑poly 3D arcade driving game made with Unity (URP). Drive, collect XP, level up to upgrade your car, and earn money to unlock better cars.

**🎮 [Play the game online](https://martinholy00.github.io/RallyBooming)**

### Requirements

- **Unity**: 6000.0.51f1 (Unity 6) or newer in the same major stream
- **Platforms**: macOS, Windows, Linux
- Optional (for `web/` website): **Node.js** 18+ and npm

### Download & Play (Releases)

Prebuilt game binaries are published on GitHub Releases.

1. Go to the Releases page of this repository.
2. Download the latest build for your OS (macOS/Windows/Linux).
3. Unzip and run the executable inside the extracted folder.

Note: On macOS, you may need to allow the app in System Settings → Privacy & Security if macOS flags it as from an unidentified developer.

### Quick Start (Play in Editor)

1. Open the project in Unity Hub using version 6000.0.51f1.
2. Load the scene at `Assets/Scenes/SampleScene.unity`.
3. Press Play.

### Gameplay Overview

- Drive around stylized low‑poly environments.
- Collect XP to level up and pick upgrades that improve handling, speed, or other car attributes.
- Earn in‑game currency by driving well and completing objectives.
- Spend money to unlock better, faster cars.

### Controls (default)

- WASD / Arrow Keys: Drive
- Space: Brake/Handbrake
- Esc: Pause/Menu

If the project uses the Input System, controls can be remapped via `Edit → Project Settings → Input System`.

### Project Structure

- `Assets/` — Game assets, scripts, scenes (URP template base)
- `Packages/` — Unity packages (URP, Input System, Post‑processing, etc.)
- `ProjectSettings/` — Project configuration
- `web/` — Vite + React website (introduction/landing page for the game)

### Building From Source (Desktop)

1. Open `File → Build Settings...` and select your target platform (Windows/macOS/Linux).
2. Add your game scene(s) to the build list.
3. Use IL2CPP and Release configuration for final builds.
4. Click Build and choose an output directory.

### Website (`web/`)

The `web/` folder contains a Vite + React website introducing the game (not the gameplay build).

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

### Troubleshooting

- Unity version mismatch: open with exactly 6000.0.51f1 to avoid upgrade prompts.
- macOS cannot open the app: allow the app in Privacy & Security.
- Performance: ensure URP quality and resolution settings match your machine.

### Contributing

Issues and PRs are welcome. Please keep edits scoped and describe changes clearly.

### License

No license specified yet. If you plan to share or publish, add a license file.
