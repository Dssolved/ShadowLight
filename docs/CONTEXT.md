# Shadow & Light: project context

Read this at the start of every session, together with `CLAUDE.md` (rules).
Last updated: 2026-11-17 (Assignment 11 code done: data assets, three levels from LevelData, JSON save, settings and reset, level select; harder levels after the student's playtest. Left: APK, phone tests, hand-in material).

## The game
2D light-and-shadow puzzle for Android, landscape. Unity 6000.3.23f1, Universal 2D (URP), new Input System.

- The player moves a **lantern** along a rail at the top with a UI **slider**. The slider sets a target; the lantern **travels** to it at `moveSpeed` (4 units/s, the whole rail takes 3 s). The player never controls the hero.
- **Floating blocks** hang at a depth `z` in front of the wall. Their shadow on the wall is a projection from the lantern:
  `k = Lz / (Lz - z)`, `S(V) = L + (V - L) * k` for every block corner V (L = lantern, Lz = lantern depth 10, block z 5, so k = 2).
  The shadow is a `PolygonCollider2D` on layer **Shadow**: the hero walks on it like ground ("shadow bridge").
  Handy formula for a block 2 units wide at y 0.6: shadow centre x = 2 * blockX - lanternX, shadow width 4, shadow top exactly at ground level y = -2.
- The **hero** walks right by himself, waits at an edge with no ground ahead, falls if the ground disappears,
  respawns at the last checkpoint after 1 s. Goal: the door.
- **Lives:** one shared pool of flames for the whole game (PlayerStats: maxLives 10, startLives 6, minLivesAtLevelStart 3, healPerBrazier 1), saved in save.json (`flames`, -1 means not set yet). A fall or an enemy hit costs one flame. At the start of a level the pool is raised to at least 3. At 0 the level quietly restarts and the pool is set to 3. A brazier gives +1 flame only the first time that level is played (level has no record yet) and only once per brazier per game run (GameSession.healedBraziers). The HUD (FlamesHud) shows a flame icon, "xN" and a row of 10 embers (full/empty). At 2 flames or fewer the icon turns red and trembles; losing a flame flashes the lost ember red, healing makes the icon glow. No Game Over screen, no timer.
- **Light rule:** something is *lit* if it is within the lantern `lightRadius` (6) AND not inside a shadow collider.
  Light weakens every enemy; lighting an enemy during its wind-up cancels the attack. This is the player's only defence.
  The lantern is 5 units above the floor enemies, so the horizontal reach of the light is small:
  Charger (y -1.6) about ±2.1, Patroller (y -0.8) about ±3.6, Shooter (y 1.2-1.5) about ±5.3.
- **Shelter slabs:** a high floating block whose shadow covers a Shooter while the lantern stands at a bridge position. A Shooter inside a shadow is not lit, so it cannot be blinded from there. The shelter shadow hangs above the hero's head and never touches him.
- **Level design rule (from the 17 Nov playtest):** in every level the lantern position for the bridge and the position that handles an enemy must be different, so the player has to think about order and timing.

## Scripts (Assets/_Project/Scripts)
| Script | Responsibility |
|---|---|
| LanternController | each Update moves the lantern with `MoveTowards` toward the rail point of the slider value (`RailPoint` = Lerp RailStart-RailEnd); starts at the slider point in Awake; sets the Light2D radius from LanternStats |
| FloatingBlock | block depth (z=5); world corners from the sprite bounds |
| BlockShadow | projects the 4 block corners each LateUpdate into PolygonCollider2D + mesh + edge LineRenderer; `Init(lantern)` from LevelLoader |
| Hero | FSM Walk / Wait / Fall / Respawn / ReachedDoor; ground and edge rays; `Die()` is the only death path; numbers from PlayerStats |
| HeroAnimator | feeds Animator parameters `Speed` and `Falling` from the Rigidbody2D velocity |
| Companion | the glowing orb next to the hero: SmoothDamp to hero + offset, sine bob |
| Checkpoint | trigger: sets the hero respawn point |
| Door | trigger: hero stops, `SaveSystem.Data.CompleteLevel(levelIndex, time, flames)` + `Save()`, then after 1 s loads the next level (sets `GameSession.levelIndex + 1`, reloads Game) or MainMenu after the last level; `Init(lives, index, hasNextLevel)` from LevelLoader |
| Lives | life counter, `Changed` event, `RestartLevel()` (reloads the scene, GameSession keeps the level) |
| FlamesHud | shows lit/spent flames from `Lives.Changed` |
| LightSensor | `IsLit()` for enemies: lantern radius + not in shadow; `Init(lantern)` |
| SpriteStateAnimator | frame animation without Animator: `clips[]` indexed by the enemy state enum; `Play(int)` |
| Patroller | dark spirit; waypoints are `Vector2[]` (from LevelData); Patrol / Chase / WindUp 0.5 s / Recover 1.5 s / Flee; `Init(hero, lantern, waypoints)` |
| Shooter | eye in the wall; Idle / Aim 0.8 s (red laser) / Cooldown 1.5 s / Blinded; looks left (`lookDirection` in the prefab); `Init(hero, lantern)` |
| Projectile | shot from the Shooter, speed from ShooterStats (12 u/s since 17 Nov: at 5 it always flew over a walking hero); kills the hero, breaks on Walls |
| Charger | stone beast; Dormant / WindUp 0.8 s (red lane) / Dash / Stunned 1 s / Return home (home = spawn position); `Init(hero, lantern)` |
| PatrollerStats, ShooterStats, ChargerStats, PlayerStats, LanternStats | ScriptableObjects with all balance numbers (public fields, `[CreateAssetMenu]`, no runtime state). Assets in `Assets/_Project/Data` |
| LevelData | ScriptableObject per level: `levelName`, `layoutPrefab`, `heroStart`, `parTimeSeconds`, `List<EnemySpawn> enemies` (EnemySpawn = prefab, position, waypoints) |
| LevelList | ScriptableObject with `LevelData[] levels` in play order; read by LevelLoader and LevelSelect |
| GameSession | static `levelIndex`, set by the menu or the door, survives scene loads |
| LevelLoader | in Game: clamps `GameSession.levelIndex`, sets the HUD text (`LEVEL 2: THE WATCHER   GOAL 18 S`), puts the hero at `heroStart`, instantiates `layoutPrefab` (Init on every BlockShadow and the Door) and every enemy (TryGetComponent -> Init, then `StateLabelSwitch.Apply`) |
| StateLabelSwitch | `Apply(enemy)` shows or hides the TMP state labels of a spawned enemy (`showStateLabels` OFF for builds, ON only for videos) |
| SaveData | `[Serializable]` save file content: `version` (1), `unlockedLevels` (List<int>, level 0 always), `records` (List<LevelRecord>: levelIndex, bestTimeSeconds, bestFlamesLeft), `musicVolume`, `sfxVolume` (0.0001-1, default 0.8). `IsUnlocked`, `FindRecord`, `CompleteLevel` (unlocks index + 1, keeps lower time and higher flames), `Sanitize` |
| SaveSystem | static; `Data` loads lazily; `Load` returns defaults for a missing file, broken JSON, null or another version (warning in the console, never a crash) and calls `Sanitize`; `Save` writes `save.json.tmp`, then `File.Replace` (or `File.Move` the first time); `Reset` = new data + delete the file. JsonUtility only, no BinaryFormatter, no PlayerPrefs |
| SaveOnPause | `OnApplicationPause(true)` -> `SaveSystem.Save()` (one object in each scene) |
| SliderReleaseSave | `IPointerUpHandler` on a settings slider: saves when the finger is released |
| SettingsPanel | MainMenu settings: music and sound sliders write into `SaveSystem.Data`; Reset Progress opens a confirm panel; Yes calls `SaveSystem.Reset()` |
| LevelSelect | MainMenu level select: in `OnEnable` every button gets `interactable = IsUnlocked(i)` and the text `1. FIRST LIGHT   BEST 11.3 S` or `LOCKED`; `PlayLevel(i)` sets GameSession and loads Game |
| MainMenu | only `Quit()` now (Play opens the level select panel) |
| CameraFit | zooms the camera out on screens narrower than 16:9 so the 17.8-unit level fits |
| SafeArea, PauseMenu | UI from Assignment 8-9 |
| Spinner | old helper, not used (the student may delete it) |

Enemy state enum order = clip order in `SpriteStateAnimator` (Charger: Dormant, WindUp, Dash, Stunned, Return; Patroller: Patrol, Chase, WindUp, Recover, Flee; Shooter: Idle, Aim, Cooldown, Blinded). Never reorder one without the other.

## Data assets (Assets/_Project/Data)
- Stats: PatrollerStats, ShooterStats (aim 0.8, projectileSpeed 12), ChargerStats (windUp 0.8, triggerRange 3), PlayerStats (walkSpeed 1.5, maxLives 3), LanternStats (depthFromWall 10, lightRadius 6, moveSpeed 4).
  Play-mode edits of these assets stay after Play mode ends: check the values before every build.
- Levels: `Data/Levels/Level_01.asset`, `Level_02.asset`, `Level_03.asset`, `LevelList.asset` (order 1, 2, 3).
- Level geometry prefabs: `Prefabs/Levels/Level_0X_Layout.prefab` (ground, blocks with shadows, shelter slabs, checkpoint with brazier, door).

## The three levels (all reach-the-door, hero starts at (-8, -1.4), lantern starts at x -6)
Slider value s -> lantern x = -6 + 12 * s.
1. **First Light** (goal 15 s). One gap x -1.3..1.3, block at x 0: bridge when lantern x is in -0.7..0.7. Patroller waypoints 4.5 and 8 (y -0.8), out of reach of the bridge light.
   Solution: lantern to the middle; as soon as the hero is off the bridge, move it right (x ~5) to keep the Patroller lit.
   Punished: moving the light while the hero is on the bridge (fall), never lighting the Patroller (hit).
2. **The Watcher** (goal 18 s). Gaps x -5..-2.4 (block -3.7, lantern -4.4..-3.0) and 3.4..6.0 (block 4.7, lantern 4.0..5.4). Shooter at (8, 1.5) looking left; ShelterBlock at (5.75, 2.75) hides it while the lantern is at bridge 2 (x 1.5..5.5). It can only be blinded from x > 5.5 (the right end of the rail).
   Solution: bridge 1; once the hero is on the middle ground, lantern to the far right; when the hero stands at the second edge, slide a little left to bridge 2 (about x 5.2). The Shooter needs 1 s to recover and 0.8 s to aim; by then the hero is out of its cone.
   Punished: parking the lantern at bridge 2 early (shot from under the shelter).
3. **The Guarded Hall** (goal 20 s). Gap 1 -4.5..-1.5 (block -3.0, lantern -3.5..-2.5), island -1.5..2.5 with Charger at 2.4 and Patroller waypoints -0.5..2.3, gap 2 2.5..5.5 (block 4, lantern 3.5..4.5), ground C 5.5..9.5 with checkpoint 5.8 and door 8.6. Shooter at (8.3, 1.2); ShelterBlock at (5.15, 2.6) hides it for lantern x 0.6..4.0. No HangingStone any more.
   Solution: bridge 1; as soon as the hero lands on the island, rush the lantern right (about 0.8 s to reach the Charger, light it at x ~1); when the hero nears gap 2, lantern to x ~4.3 (bridge 2 + Charger lit + Shooter out of the shelter, a narrow window 4.0-4.5); after landing on C, lantern to the right end.
   Punished: leaving bridge 1 late (Charger), standing at bridge 2 inside the shelter range (Shooter).
Checked 17 Nov with an autopilot in Play mode: the right play reaches the door on all three levels with no deaths (about 12.5-13 s each); every listed mistake costs a life. Not yet played by hand on a phone.

## Scenes
**Game** (`Assets/_Project/Scenes/Game.unity`): only the shared parts live here, the level itself is built by LevelLoader at Start.
- Main Camera (ortho size 5, CameraFit), Global Light 2D (0.25), BackWall (tiled `WallTile`), RailStart (-6,4), RailEnd (6,4), RailVisual, Lantern (+ LanternLight).
- Hero (-8,-1.4) with child Body, CompanionOrb (+ OrbLight), Lives, StateLabels (StateLabelSwitch), SaveOnPause, LevelLoader (levelList, hero, lantern, lives, stateLabels, levelText).
- Canvas > SafeArea > Flames, LevelText (1100 wide, left aligned, set by LevelLoader), PauseButton, Slider; PausePanel. EventSystem, PauseMenuController.
- In the editor the Game scene shows no ground or enemies until Play mode. Edit geometry in the layout prefabs and enemies in the LevelData assets.

**MainMenu**: Canvas > Background, SafeArea (Title, PlayButton -> `LevelSelectPanel.SetActive(true)`, SettingsButton, QuitButton);
SettingsPanel (off; music and sound sliders 0.0001-1 with SliderReleaseSave, Reset Progress, Back, ConfirmPanel with Yes/Cancel);
LevelSelectPanel (off; title, Level1-3 buttons -> `LevelSelect.PlayLevel(0/1/2)`, Back). MainMenuController (Quit), SaveOnPause, EventSystem.

Layers: 6 Walls, 7 Shadow, 8 Hero, 9 Enemy.

## Save file
- Path: `Application.persistentDataPath/save.json`. Editor: `%USERPROFILE%\AppData\LocalLow\DefaultCompany\ShadowLight\save.json`. On Android: the app's persistent data folder (use `adb` or print the path to show it).
- Saved: when the hero enters a door, when a settings slider is released, in `OnApplicationPause(true)`, and after Reset (file deleted).
- Example:
  ```json
  { "version": 1, "unlockedLevels": [0, 1], "records": [ { "levelIndex": 0, "bestTimeSeconds": 12.3, "bestFlamesLeft": 3 } ], "musicVolume": 0.8, "sfxVolume": 0.8 }
  ```
- Volumes are saved but do nothing yet: they are applied to the AudioMixer in Assignment 12.
- `CompleteLevel` on the last level adds index 3 to `unlockedLevels`; LevelSelect only looks at its 3 buttons, so this is harmless.

## Visuals (art pipeline and sources)
- URP 2D lights: dark hall, warm lantern light with a visible edge. Hero, enemies, door, lantern, rail, brazier, orb use `SpriteUnlit.mat`; wall, ground and slabs are lit.
- Style: hand-painted storybook, palette: shadow #1C1626, warm amber #F2B84B for light, rose-pink scarf.
- `Sprites/UI`, `Sprites/Scene`, `Sprites/Enemies/...`: see README.md for sources (Unity AI generation and the author's own enemy sheets).
- Pause button and panel, main menu, settings and level select panels are still plain Unity UI (art pass planned after Assignment 11).
- Known visual nit: in level 2 the shelter shadow overlaps part of the bridge-2 slab sprite.

## Course status
- Assignment 8-9 (UI): done. Assignment 10 (Three Enemy Types): handed in 12 Nov.
- **Assignment 11 (Data and Saves), Canvas closes Thu 19 Nov 12:30 (OR1-3).** Code and levels are done (see above). Left:
  APK build and phone tests (close and reopen: progress kept; damaged save.json: starts with defaults), a screenshot of one data asset in the Inspector, the save.json text,
  a 2-3 min English video with the student's voice (live balance change in an asset, phone restart test, broken save test, script map), DECISIONS.md lines for week 11 written by the student, commits spread over the week.
- Assignment 12 (Sound Pass), Thu 26 Nov 12:30: brief received, not started. Volume fields already exist in SaveData.
- Build notes: StateLabels `showStateLabels` OFF in builds. Package name is still com.DefaultCompany.urp_2d: change it before the Google Play closed test (about 24 Nov).

## Working notes for the agent
- The student commits through GitHub Desktop. Never run git. Work in small steps; after each step give a suggested commit message, a line-by-line explanation, the scene changes and the risks.
- Unity MCP `Unity_RunCommand`: parameters `Code` and `Title`; class `internal class CommandScript : IRunCommand`. No `System.Reflection`. Use `UnityEngine.UI.Image` (Image clashes with a namespace). UnityEventTools: `AddPersistentListener`, `AddBoolPersistentListener`, `AddIntPersistentListener`, `RemovePersistentListener`.
- After saving a script Unity recompiles and MCP answers "Unity not detected" for 20-60 s. Wait with the Monitor tool (`sleep 40; echo done`), not Bash sleep.
- A warning or error logged during a command makes the tool report "executed partially" even when the work is done; check the result. `Unsupported type Vector2f` errors come from the Unity AI Assistant package when a Vector2 field is changed through SerializedObject; harmless.
- `AssetDatabase.DeleteAsset` (and anything with a dialog) fails with "User interactions are not supported" and cancels the whole command: ask the student to delete in the Project window.
- Edit prefabs with `PrefabUtility.LoadPrefabContents` -> change -> `SaveAsPrefabAsset` -> `UnloadPrefabContents`.
- **Play-mode tests:** define a small test MonoBehaviour inside the RunCommand code, put it on a `DontDestroyOnLoad` object, set `GameSession.levelIndex` and `SceneManager.LoadScene("Game")`. In its Update it moves the slider by hero x thresholds and logs results with `Debug.Log`; read them later with `Unity_GetConsoleLogs`. Do not use `EditorApplication.Step()` from a command in Play mode (it causes "PlayerLoop called recursively").
- When Unity is not the focused window it does not tick in Play mode. `Application.runInBackground = true` fixes this but it also writes `PlayerSettings.runInBackground`: set it back to false after the test and check `ProjectSettings/ProjectSettings.asset` still says `runInBackground: 0`.
- Door tests write `save.json`. After tests call `SaveSystem.Reset()` (or restore the old file) so the student starts clean.
- The Shooter shot flies to where the hero was; a hero walking away is never hit, a hero standing still or walking toward the Shooter is.
- Camera captures do not show the Screen Space Overlay Canvas. For UI screenshots temporarily switch the Canvas to Screen Space - Camera.
- Setting `SpriteRenderer.drawMode` to Sliced or Tiled through code can multiply the transform scale; reset `localScale` to 1. Sliced and Tiled sprites must use Mesh Type Full Rect.
- Sprite sheets on white: key by `min(r,g,b)` (alpha = (0.97 - m) / 0.22, color un-premultiplied against white), cut rows and columns by gaps (rows >= 12 px, frames >= 36 px) and drop islands smaller than 50 px. Windows can decode webp through PowerShell and WPF (`BitmapImage`); Unity cannot.
- `Mathf.SmoothStep(from, to, t)` in Unity is not the shader smoothstep; write t*t*(3-2t) by hand.
- Unity AI generation: describe original designs in text (the filter blocks references that resemble known characters); pass `forceGeneration: true` after a blocked generation; ask the student for consent in every new session.
