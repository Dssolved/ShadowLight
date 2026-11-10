# Shadow & Light

A 2D light-and-shadow puzzle for Android (landscape). You move a lantern, the shadows of floating blocks change, and a small hero walks across the shadows to the door.

Course project: Game Development Fundamentals, Narxoz University (RK2).

## Status

- Assignment 8-9: Main Menu, HUD and Pause screens.
- Assignment 10: lantern on a rail, shadow bridges, auto-walking hero with lives and checkpoints, three enemy types (Patroller, Shooter, Charger) that react to the lantern light, one level with all three.

## Requirements

- Unity 6000.3.23f1, Universal 2D template (URP)
- Input System package
- Target platform: Android, landscape

## How to open

1. Clone the repository.
2. Add the project folder in Unity Hub and open it with Unity 6000.3.23f1.
3. Open `Assets/_Project/Scenes/MainMenu.unity` and press Play.
4. To check the layout on different phones, switch the Game view to Simulator.

## UI screens

- **Main Menu:** title, Play, Quit.
- **HUD:** three flames (instead of hearts), level number, pause button (top right), light slider (bottom, handle 120x120 px).
- **Pause:** Resume, Restart, Menu. Time stops via `Time.timeScale`.

All screens use Canvas Scaler (Scale With Screen Size, 1920x1080, Match 1), anchors and a SafeArea panel. Tested in Device Simulator on 16:9, a notch phone (19.5:9) and a 4:3 tablet.

## Project structure

```
Assets/_Project/
  Scenes/     MainMenu, Game
  Scripts/    game scripts (one class per file)
  Prefabs/    Patroller, Shooter, Charger, Projectile
  Sprites/    hero frames, StoneSlab, Concept/
  Animations/ Hero.controller, HeroIdle, HeroWalk, HeroFall
  Materials/  Shadow, Laser, SpriteUnlit
```

## Art sources

| Asset | Source |
|---|---|
| HeroSide 1.png, HeroWalk.png, HeroIdle.png, HeroFall.png | Hooded hero, generated with Unity AI (Gemini 3.1 Flash) from a text description of the author's design; background removed with Unity AI (Photoroom Background Removal) |
| Concept/HeroConceptA.png, HeroConceptB.png | Author's character concepts (style reference only) |
| HeroSheet.png | First hero concept (made with Gemini), no longer used in the game |
| StoneSlab.png | Generated with Unity AI (Gemini 3.1 Flash), background removed with Unity AI (Photoroom Background Removal) |
| Other shapes | Unity built-in 2D sprites (Square, Circle, Capsule) |

Licenses of AI-generated assets must be checked against the Unity AI terms before the Google Play release.

## AI usage

- Assignment 8-9: an AI chat was used for learning and explanations, see `AI_LOG.txt`. No AI agents wrote code.
- From Assignment 10 (agents allowed by the course): Claude Code was used as a coding agent under my lead. Decisions and corrections are logged in `DECISIONS.md`.
