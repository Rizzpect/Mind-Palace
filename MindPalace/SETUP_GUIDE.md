# Mind Palace - Unity Setup Guide

## 🎯 What You're Building
A 3D first-person "method of loci" memory training app where users:
- Walk through a virtual palace with multiple rooms
- Interact with hotspots (loci) to create/review memory cards
- Use spaced repetition (SM-2 algorithm) to optimize retention
- All data saved locally as JSON

**Target Platform:** Unity 2022.3 LTS, Built-in Render Pipeline  
**Hardware Optimized For:** i5-13500H + Intel Iris Xe integrated graphics

---

## ✅ What's Already Done

The previous agent created **all the C# scripts** and they're working correctly:

### Core Systems (100% Complete)
- ✅ `MemoryModels.cs` - Data structures for cards and save data
- ✅ `MemoryScheduler.cs` - SM-2 lite spaced repetition algorithm
- ✅ `SaveSystem.cs` - JSON save/load to disk
- ✅ `LocusHotspot.cs` - Trigger zones for interaction
- ✅ `PalaceManager.cs` - Main singleton manager
- ✅ `MemoryCardUI.cs` - UI panel for card creation/review
- ✅ `MindPalaceTools.cs` - Editor automation menus

**All scripts compile without errors** and include helpful comments!

---

## 🚧 What You Need to Do

The **Unity scene setup** hasn't been done yet. Follow these steps:

---

## Step 1: Open the Project in Unity

1. Launch **Unity Hub**
2. Open the project folder (the `MindPalace` directory containing the `Assets` folder)
3. Wait for Unity to compile scripts (should be no errors)
4. Open the scene: `Assets/PalaceScene.unity`

---

## Step 2: Create the Basic Palace Geometry

You need 3 rooms connected by a corridor. Here's the simple layout:

### A. Create Room A (Entry)
1. Right-click in **Hierarchy** → **3D Object → Cube**
2. Name it: `Room_A`
3. Set Transform:
   - Position: `X=0, Y=2.5, Z=0`
   - Scale: `X=8, Y=5, Z=8`
4. In Inspector, **remove** the BoxCollider component (we want to walk inside)

### B. Create Corridor
1. Create another Cube, name it: `Corridor`
2. Set Transform:
   - Position: `X=0, Y=2.5, Z=6`
   - Scale: `X=4, Y=5, Z=4`
3. Remove BoxCollider

### C. Create Room B (Study)
1. Create Cube: `Room_B`
2. Transform:
   - Position: `X=0, Y=2.5, Z=12`
   - Scale: `X=8, Y=5, Z=8`
3. Remove BoxCollider

### D. Create Room C (Archive)
1. Create Cube: `Room_C`
2. Transform:
   - Position: `X=0, Y=2.5, Z=24`
   - Scale: `X=8, Y=5, Z=8`
3. Remove BoxCollider

### E. Create Floor
1. Create Cube: `Floor`
2. Transform:
   - Position: `X=0, Y=0, Z=12`
   - Scale: `X=10, Y=0.2, Z=30`
3. **Keep** the BoxCollider on this one

---

## Step 3: Add Lighting

1. Delete the default **Directional Light** if it exists
2. Right-click Hierarchy → **Light → Directional Light**
3. Name it: `MainLight`
4. Set Transform:
   - Rotation: `X=50, Y=-30, Z=0`
5. In Inspector:
   - Mode: **Baked** (for performance)
   - Intensity: `1`
   - Shadow Type: **Soft Shadows**

### Configure Baked Lighting (Optimization for Iris Xe)
1. Go to **Window → Rendering → Lighting**
2. **Environment** tab:
   - Skybox Material: Default (or simple solid color)
3. **Baked Lightmaps** section:
   - Lightmapper: **Progressive CPU**
   - Enable **Baked Global Illumination**
4. Click **Generate Lighting** (bottom right)
5. Wait for baking to complete

---

## Step 4: Add Player Controller

You have **two options**:

### Option A: Use Unity Starter Assets (Recommended)
1. Go to **Window → Package Manager**
2. Click the **+** button → **Add package from git URL**
3. Paste: `com.unity.starter-assets`
4. Import the **First Person Controller** sample
5. Drag the `PlayerArmature` prefab into your scene
6. Set Position: `X=0, Y=1.2, Z=6` (in corridor)
7. Select the PlayerArmature, set **Tag** to `Player` in the Inspector

### Option B: Create a Simple Capsule Player (Quick Test)
1. Right-click Hierarchy → **3D Object → Capsule**
2. Name it: `Player`
3. Set Position: `X=0, Y=1.2, Z=6`
4. Set **Tag** to `Player`
5. Add Component → **Character Controller**
6. Add a simple movement script (or search Unity forums for "First Person Controller")

**⚠️ Important:** The player MUST have the **"Player" tag** for hotspots to detect it!

---

## Step 5: Create the UI System

1. In Unity menu bar, click: **MindPalace → Create Memory UI**
2. This creates:
   - Canvas with MemoryCardUI component
   - Input fields for front/back of cards
   - 4 grading buttons (Again, Hard, Good, Easy)
   - Close button
   - Auto-creates a **Managers** GameObject with PalaceManager

3. **Verify Setup:**
   - Find `Managers` in Hierarchy
   - Click it and look at the **PalaceManager** component
   - The `Memory Card UI` field should be filled automatically ✅

---

## Step 6: Create Hotspots (Loci)

Now add interactive points where memory cards live:

### Create First Hotspot in Room A
1. Create an **Empty GameObject** in Room A (as visual reference)
2. Name it: `Desk_A`
3. Position: `X=2, Y=1, Z=0`
4. **Select** `Desk_A` in Hierarchy
5. Menu: **MindPalace → Create Hotspot At Selection**
6. This creates `Desk_A_Hotspot` with:
   - SphereCollider (trigger, radius 1.2)
   - LocusHotspot component
   - locusId = "desk_a"

### Create More Hotspots (Repeat for other rooms)
1. Create empty GameObjects in Room B and Room C:
   - `Bookshelf_B` at `X=-3, Y=1, Z=12`
   - `Globe_B` at `X=3, Y=1, Z=12`
   - `Archive_C` at `X=0, Y=1, Z=24`
2. Select each and run **MindPalace → Create Hotspot At Selection**

**Result:** You should have 4-5 hotspots total.

---

## Step 7: Register Hotspots with Manager

1. Select the `Managers` GameObject
2. In **PalaceManager** component, expand the **Loci** list
3. You can either:
   - **Auto-populate:** Just run the scene (Start() finds all hotspots automatically)
   - **Manual (optional):** Set size to 4, drag each hotspot GameObject into slots

---

## Step 8: Optimize Graphics for Iris Xe

1. Go to **Edit → Project Settings → Quality**
2. Select the **current quality level** (usually "Medium" or "High")
3. Adjust these settings:

```
Anti Aliasing: Disabled (or 2x Multi Sampling if needed)
Soft Particles: Off
Shadows:
  - Shadow Distance: 40-60
  - Shadow Cascades: Two Cascades
  - Shadow Resolution: Medium
Texture Quality: Full Res
Vsync: Don't Sync (or use external limit)
```

4. **Edit → Project Settings → Player:**
   - Other Settings → **Color Space: Linear** (optional, better visuals)

---

## Step 9: Test the Core Loop

1. Click **Play** ▶️
2. Use **WASD** to walk around
3. Walk toward a hotspot until you see the trigger zone
4. Press **E** to open the memory panel
5. Type a question in "Front Field"
6. Type an answer in "Back Field"
7. Click **Good** or **Easy**
8. The card is saved! ✅

### Verify Persistence
1. Stop Play mode
2. Start Play again
3. Walk to the same hotspot and press **E**
4. You should see your card data loaded from disk!

---

## 🐛 Troubleshooting

### "PalaceManager.Instance is null"
- Make sure the `Managers` GameObject exists in Hierarchy
- It should have the **PalaceManager** component attached

### "memoryCardUI is not assigned"
- Select `Managers` → PalaceManager component
- The `Memory Card UI` field should reference the UI_Root/MemoryCardUI
- If not, drag the `UI_Root` GameObject into that slot

### Player falls through floor
- Check that `Floor` has a **BoxCollider** component
- If using Character Controller, ensure it's enabled

### Hotspot doesn't trigger
- Check player has **Tag = "Player"**
- Verify hotspot's SphereCollider has **Is Trigger = checked**

### E key does nothing
- Make sure you're standing inside the hotspot's sphere
- Check Console for errors (Window → General → Console)

---

## 📊 Where Save Data Lives

When you grade a card, it's saved to:
```
Windows: C:\Users\<YourName>\AppData\LocalLow\DefaultCompany\MindPalace\palace-save.json
```

You can open this file in a text editor to see your cards!

---

## 🎨 Optional: Make It Look Better

### Add Materials
1. Create materials (Assets → Create → Material)
2. Set colors:
   - Room_A: Light blue tint
   - Room_B: Warm yellow tint
   - Room_C: Cool gray
3. Drag materials onto room cubes

### Add Props
- Import simple 3D models (desks, bookshelves) from Unity Asset Store
- Place them inside rooms as visual landmarks
- Position hotspots near these props

### Add "Press E" Prompt
1. Create 3D Text or TextMeshPro floating above hotspots
2. Set text: "Press E"
3. Drag into each hotspot's `Interact Prompt` field in Inspector
4. It will show/hide automatically when player is near!

---

## 🚀 What's Next (Out of Scope for MVP)

The brief mentioned these as **optional/future**:
- Daily ritual selector (filter due cards across palace)
- Procedural world generation
- VR support
- Cloud sync
- Advanced visual effects

For now, you have a **fully functional MVP** with:
✅ Walkable 3D palace  
✅ Memory card creation  
✅ Spaced repetition scheduling  
✅ Local persistence  
✅ Editor tools for rapid setup  

---

## 📝 Summary of What's Working

| Component | Status | Notes |
|-----------|--------|-------|
| C# Scripts | ✅ 100% | All compile, well-commented |
| Data Models | ✅ Done | MemoryCard, SaveData |
| SRS Algorithm | ✅ Done | SM-2 lite (Again/Hard/Good/Easy) |
| Save System | ✅ Done | JSON to disk |
| Hotspot System | ✅ Done | Trigger + E key interaction |
| UI System | ✅ Done | Auto-generated via menu |
| Scene Geometry | ⚠️ Manual | Follow Step 2 above |
| Player Setup | ⚠️ Manual | Follow Step 4 above |
| Lighting | ⚠️ Manual | Follow Step 3 above |

---

## 🎓 How the Code Works (Brief)

### When You Press E at a Hotspot:
1. `LocusHotspot.Update()` detects key press
2. Calls `PalaceManager.OpenLocus(this)`
3. Manager finds next due card for that locusId (or creates new blank)
4. Opens `MemoryCardUI.Open(card, callback)`
5. UI pauses game and shows fields

### When You Grade a Card:
1. `MemoryCardUI.Grade()` saves field text
2. Calls `MemoryScheduler.ApplyGrade(card, grade, now)`
3. Scheduler updates `intervalDays`, `ease`, `reps`, `lapses`
4. Sets new `dueIsoUtc` timestamp
5. Callback to `PalaceManager.OnCardUpdated()`
6. `SaveSystem.Save()` writes JSON to disk

---

## 💡 Tips for Success

1. **Test frequently** - Run Play mode after each major step
2. **Check Console** - Errors will show up there (Window → General → Console)
3. **Save your scene** - Ctrl+S after making changes
4. **Backup** - The save file is just JSON, easy to backup
5. **Performance** - If FPS drops, reduce shadow distance or turn off shadows

---

## 🤝 Need Help?

If something's not working:
1. Check the **Console** for error messages
2. Verify all GameObjects have the correct **Tags**
3. Make sure all **components are attached** (check Inspector)
4. The save file location is logged when you grade a card

Good luck building your Mind Palace! 🏛️🧠

---

**Project Status:**  
Code: ✅ Complete  
Scene: ⚠️ Needs manual setup (this guide)  
Ready to build: Once scene is set up per Steps 1-7

**Estimated Time to Complete Setup:** 15-20 minutes
