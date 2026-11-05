# Shadow & Light

A 2D light-and-shadow puzzle for Android (landscape). You move a lantern, the shadows of floating blocks change, and a small hero walks across the shadows to the door.

Course project: Game Development Fundamentals, Narxoz University (RK2, Assignment 8-9).

## Status

Assignment 8-9: Main Menu, HUD and Pause screens are done. Game mechanics come in the next assignments.

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
  Scenes/    MainMenu, Game
  Scripts/   SafeArea, MainMenu, PauseMenu, Spinner
```

## AI usage

An AI chat was used for learning and explanations. See `AI_LOG.txt`. No AI agents that write code in the project were used.
