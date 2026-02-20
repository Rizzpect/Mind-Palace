# Contributing to Mind Palace

Thank you for your interest in contributing! This project was born during a hackathon, and we welcome improvements.

## Getting Started

1. **Fork** this repository
2. **Clone** your fork locally
3. Open `MindPalace/` in **Unity Hub** (requires Unity 2022.3 LTS)
4. Wait for scripts to compile — there should be zero errors
5. Create a new branch: `git checkout -b feature/your-feature`

## Development Setup

### Editor Tools
Use the built-in editor menus to set up scenes quickly:
- **MindPalace → ⭐ Build Complete Palace Scene** — One-click full scene
- **MindPalace → Create Memory UI** — Creates the UI canvas
- **MindPalace → Validate Scene Setup** — Checks for common issues

### Project Structure
```
Assets/
├── Scripts/
│   ├── Gameplay/    → Player controller, hotspot interaction
│   ├── Managers/    → PalaceManager singleton
│   ├── Models/      → Data structures (MemoryCard, SaveData)
│   ├── Services/    → SM-2 scheduler, JSON save system
│   └── UI/          → Card UI, HUD overlay
└── Editor/          → Editor-only tools (guarded with #if UNITY_EDITOR)
```

## Guidelines

### Code Style
- Use **tabs** for indentation
- All editor scripts **must** be wrapped in `#if UNITY_EDITOR` / `#endif`
- Add `[Tooltip("...")]` attributes to public fields
- Add `/// <summary>` XML docs to public methods

### Pull Requests
1. One feature per PR
2. Test in Unity before submitting
3. Describe what was changed and why
4. Include screenshots for UI changes

## Ideas for Contributions

- 🎨 Better materials and lighting
- 🔊 Sound effects (footsteps, card flip, grade chimes)
- 🌍 Procedural room generation
- 📊 Statistics dashboard (review accuracy, streak tracking)
- 🎮 VR support
- ☁️ Cloud sync for save data
- 🧪 Unit tests for MemoryScheduler

## License

By contributing, you agree that your contributions will be licensed under the MIT License.
