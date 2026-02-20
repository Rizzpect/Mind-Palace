# 🏛️ Mind Palace — Memory Training Game

A 3D first-person **memory training game** built in Unity, inspired by the **Method of Loci** (Memory Palace) technique. Walk through a virtual palace, interact with hotspots to create flashcards, and let the built-in **spaced repetition algorithm (SM-2)** schedule your reviews for optimal retention.

## 🏆 Hackathon Origin

> This project was created during the **Vibeathon** hackathon conducted by the **AI&DS Department** at **KLH Aziznagar University**.

## ✨ Features

### Gameplay
- **3D First-Person Exploration** — WASD + mouse, sprint with Shift, head bob
- **Memory Hotspots (Loci)** — Interactive glowing locations tied to flashcards
- **HUD Overlay** — Crosshair, interaction prompts, live card/locus stats
- **Card Navigation** — Browse multiple cards per locus with ◀ ▶ buttons
- **Create & Delete Cards** — Add new cards or remove old ones at any locus
- **Spaced Repetition (SM-2)** — Cards scheduled with Again/Hard/Good/Easy grading

### Technical
- **Local Persistence** — All data saved as JSON to disk automatically
- **Editor Tools** — One-click scene building, hotspot creation, diagnostics, and decoration
- **Zero Dependencies** — No extra packages needed (uses built-in render pipeline)
- **Editor Gizmos** — Hotspot radii visible in Scene view for easy level design

## 🛠️ Tech Stack

| Component | Detail |
|-----------|--------|
| Engine | Unity 2022.3 LTS |
| Render Pipeline | Built-in (Standard) |
| Language | C# |
| UI | Unity UI + TextMeshPro + OnGUI HUD |
| Data Format | JSON (via `JsonUtility`) |
| Algorithm | SM-2 Lite Spaced Repetition |

## 🚀 Quick Start

### Prerequisites
- [Unity Hub](https://unity.com/download) + **Unity 2022.3 LTS**
- TextMeshPro (auto-imported by Unity)

### Setup
1. Clone this repository
2. Open `MindPalace/` folder in Unity Hub
3. Wait for scripts to compile (should be zero errors)
4. In Unity menu bar: **MindPalace → ⭐ Build Complete Palace Scene (Auto)**
5. Press **Play** ▶️
6. Walk to a yellow cube (hotspot) and press **E** to create memory cards!

> For detailed step-by-step setup, see [SETUP_GUIDE.md](MindPalace/SETUP_GUIDE.md)

## 🎮 Controls

| Key | Action |
|-----|--------|
| WASD | Move |
| Shift | Sprint |
| Mouse | Look around |
| E | Interact with hotspot |
| Click | Re-lock cursor |
| Escape | Unlock cursor |

## 📁 Project Structure

```
MindPalace/
├── Assets/
│   ├── Scripts/
│   │   ├── Gameplay/
│   │   │   ├── LocusHotspot.cs              # Hotspot interaction + HUD integration
│   │   │   └── SimpleFirstPersonController.cs  # FPS movement + sprint + head bob
│   │   ├── Managers/
│   │   │   └── PalaceManager.cs             # Singleton manager + stats API
│   │   ├── Models/
│   │   │   └── MemoryModels.cs              # Data structures
│   │   ├── Services/
│   │   │   ├── MemoryScheduler.cs           # SM-2 spaced repetition
│   │   │   └── SaveSystem.cs                # JSON persistence
│   │   └── UI/
│   │       ├── MemoryCardUI.cs              # Card panel + navigation
│   │       └── PalaceHUD.cs                 # Crosshair + stats overlay
│   ├── Editor/
│   │   ├── MindPalaceTools.cs               # Core editor menus
│   │   ├── SceneAutoBuilder.cs              # One-click scene builder
│   │   ├── MindPalaceDiagnostics.cs         # Scene validation tool
│   │   ├── AutoDecorate.cs                  # Auto-decoration tool
│   │   └── PalaceDecorator.cs               # Manual decoration menus
│   └── Scenes/
│       └── SampleScene.unity
├── SETUP_GUIDE.md
└── ProjectSettings/
```

## 📊 How It Works

### Memory Flow
1. Walk to a hotspot → Press **E**
2. Create a flashcard (front = question, back = answer)
3. Grade your recall: **Again** / **Hard** / **Good** / **Easy**
4. The SM-2 algorithm calculates when to show the card next
5. Browse multiple cards at each locus with ◀ ▶, or add new ones with **+ New Card**
6. Cards are saved to `%AppData%/../LocalLow/DefaultCompany/MindPalace/palace-save.json`

### Editor Tools (Unity Menu Bar)
| Menu Item | Description |
|-----------|-------------|
| MindPalace → ⭐ Build Complete Palace Scene | One-click full scene setup |
| MindPalace → Create Memory UI | Creates the full flashcard UI + HUD |
| MindPalace → Create Simple Player | Creates FPS player with sprint + camera |
| MindPalace → Create Hotspot At Selection | Adds hotspot at selected object |
| MindPalace → Validate Scene Setup | Checks scene for common issues |
| MindPalace → Delete Save File (Reset) | Clears all saved cards |
| GameObject → 🎨 Make Palace Beautiful | Auto-add colors + furniture |

## 🔮 Future Ideas

- 🔊 Sound effects (footsteps, card flip, grade chimes)
- 🌍 Procedural world generation
- 🎮 VR support
- ☁️ Cloud sync
- 📊 Statistics dashboard (review accuracy, streak tracking)
- 🧪 Daily review mode (filter due cards across the whole palace)

## 🤝 Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## 📄 License

This project is licensed under the MIT License — see [LICENSE](LICENSE) for details.

---

*Built during the Vibeathon hackathon at KLH Aziznagar University*
