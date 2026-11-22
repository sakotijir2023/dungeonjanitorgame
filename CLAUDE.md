# Dungeon Janitor: project conventions for Claude Code

Read this file at the start of every session. The owner directs game feel and design and reviews by playing. Explain changes in plain language, and never ask him to hand-edit code.

## The game

A first-person 3D game (Slime Rancher / Mouse P.I. style). You are a nobody janitor resetting a dungeon for the next hero party: clean, fix, then stock and arm it. A timer runs; when it ends the Inspector arrives, grades the room, and pays points or cash that buy upgrades.

Tone: cute, spooky, darkly humorous. The Inspector is a dry, sarcastic jerk who grades fairly but hard; even a great run gets a nasty remark, and a perfect score is met with disbelief.

### v1 scope
- One plain "janitor training room" for testing every mess type. Runs last 3-5 minutes.
- Slice one: vacuum and mop on goo and stains, tool wheel with slow-motion, one room, timer, score, end screen, one music loop. Other mess families are added one at a time afterward.

### Mess families and tools
| Family | Tool |
| --- | --- |
| Goo and small debris | Vacuum |
| Stains, scorch marks, blood | Mop |
| Bones and gore bits | Trash bag |
| Magical and cursed residue, ectoplasm | Neutralizer ray |
| Cobwebs and dust | Duster or brush |
| Tarnished objects | Polish cloth |
| Creatures (rats, spiders, snakes) | Creature catcher |
| Broken lights, torches, chests, traps | Setup folder (slot 8) |

### Tool wheel
Eight slots; slot 8 is a "Setup" folder (hold to reveal trap kits, chest stocker, torch relighter, light repair kit; the chosen one becomes the active tool; remembers the last pick). Opening the wheel slows time (not a full pause). Placing traps and chests is its own system (placement preview, snapping, arming).

### Rules of the design (do not break)
- **Tools never have capacity or resource limits.** Upgrades only make cleaning faster and let you handle harder messes.
- Messes spawn from preset spawn points. Testing uses a fixed layout; the real game randomizes types and amounts (seeded). Build for both from the start.
- No hard fail. A bad grade pays little. Finishing early earns a time bonus. Cash and upgrades persist between runs and are saved.
- Cleaning must feel good: suction with visible give and take on the object, clear feedback, strong sounds. Mopping gets the same treatment.

## Stack
- Unity 6.3 LTS, PC, C#. Universal Render Pipeline (default for the 3D template).
- Unity's **Input System** package only. Do not use the legacy Input Manager. Support keyboard and mouse plus gamepad; tune with keyboard and mouse first, then do a gamepad pass (including a stick version of the wheel).
- Git on GitHub. Use the standard Unity `.gitignore` and Git LFS for large binaries (fbx, glb, png, psd, wav, mp3, ogg).

## Architecture: feel is data
- Every feel value (speeds, gravity, suction strength, mop radius, slow-motion amount, hitstop, shake, camera lag, timings) lives in ScriptableObjects under `Assets/_Game/Config/`. No magic numbers in logic.
- Build a live tuning panel early: a toggleable debug UI with a slider for every config value and save/load of named presets.
- Small, decoupled systems. One responsibility per script, around 300 lines or fewer. Systems talk through events, not direct references to each other.
- Tools implement a shared tool interface. The wheel is built from a data-driven list, so adding a tool or a folder option is a data change.
- Messes are components with a hardness value; upgrades are just numbers that change cleaning speed and which hardness a tool can handle.
- Slow-motion uses `Time.timeScale` with unscaled time for UI and the wheel. The amount comes from config (start near 0.2).

### Folder layout
```
Assets/_Game/
  Scripts/      (namespace DungeonJanitor.*)
  Config/       (ScriptableObjects, Presets/)
  Prefabs/
  Art/          (Models, Textures, Materials, Shaders)
  Audio/        (Sfx, Music)
  UI/
  Scenes/
docs/           (FEEL_JOURNAL.md, ASSET_LOG.md, AUDIO_MANIFEST.md)
Tools/blender/  (Blender Python scripts)
```

## How to work
- Small vertical slices: one behavior at a time, playable after each. Never build the whole game in one go.
- Commit after every working slice with a clear message. Commit before any large change so it can be rolled back.
- After code changes, check for compile errors before saying you are done. On Windows the Unity log is at `%LOCALAPPDATA%\Unity\Editor\Editor.log`; a batch-mode compile also works.
- Write Edit Mode tests for non-visual logic (scoring, upgrade math, mess spawning).
- Do not add packages, rename or delete assets outside `Assets/_Game/`, or do large refactors without asking first.
- After each slice, tell the owner what to playtest and which sliders to try.
- Append a dated line to `docs/FEEL_JOURNAL.md` whenever a feel value changes meaningfully and why.

## Art and audio pipeline
- **3D:** simple chunky shapes are built with Blender Python scripts in `Tools/blender/`. Organic shapes come from image-to-3D tools and get light cleanup. Export FBX or glTF with transforms applied, 1 unit = 1 meter, pivot at the floor center, sensible names. Slime Rancher-style: chunky, rounded, simple colors, little texture detail, darker palette; mood comes from lighting.
- **2D UI:** icons and frames come from the owner's image generator; build layout and wheel logic in Unity.
- **SFX:** generated with ElevenLabs text-to-SFX while a subscription is active (1-3 months). Generate several variants per sound and randomize variant and pitch at runtime. Keep every generated file.
- **Music:** made in Mureka by the owner.
- Record every AI-generated asset (tool, prompt, date, license notes) in `docs/ASSET_LOG.md`, and every sound in `docs/AUDIO_MANIFEST.md`. This is needed later for license checks and storefront disclosure.

## Parked (do not build yet)
Trap kit supply, captured-creature use, trap behavior, generated hallways, a roaming Inspector, the upgrade shop's final contents (color swaps undecided).
