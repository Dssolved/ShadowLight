# Shadow & Light: project context

Read this at the start of every session, together with `CLAUDE.md` (rules).
Last updated: 2026-11-10 (end of the Assignment 10 build day).

## The game
2D light-and-shadow puzzle for Android, landscape. Unity 6000.3.23f1, Universal 2D (URP), new Input System.

- The player moves a **lantern** along a rail at the top with a UI **slider**. The player never controls the hero.
- **Floating blocks** hang at a depth `z` in front of the wall. Their shadow on the wall is a projection from the lantern:
  `k = Lz / (Lz - z)`, `S(V) = L + (V - L) * k` for every block corner V (L = lantern, Lz = lantern depth).
  The shadow is a `PolygonCollider2D` on layer **Shadow**: the hero walks on it like ground.
- The **hero** walks right by himself, waits at an edge with no ground ahead, falls if the ground disappears,
  respawns at the last checkpoint after 1 s. Goal: the door.
- **Lives:** 3 flames in the HUD. A fall or an enemy hit costs one flame. At 0 the level quietly restarts. No Game Over screen, no timer.
- **Light rule:** something is *lit* if it is within the lantern `lightRadius` (7) AND not inside a shadow collider.
  Light weakens every enemy; lighting an enemy during its wind-up cancels the attack. This is the player's only defense.

## Scripts (Assets/_Project/Scripts)
| Script | Responsibility |
|---|---|
| LanternController | moves the lantern along RailStart-RailEnd from the slider; holds depthFromWall (Lz=10) and lightRadius (7); sets the Light2D radius from lightRadius |
| FloatingBlock | block depth (z=5); world corners from the sprite bounds |
| BlockShadow | projects the 4 block corners each LateUpdate into PolygonCollider2D + mesh + edge LineRenderer |
| Hero | FSM Walk / Wait / Fall / Respawn / ReachedDoor; ground and edge rays; `Die()` is the only death path |
| HeroAnimator | feeds Animator parameters `Speed` and `Falling` from the Rigidbody2D velocity |
| Checkpoint | trigger: sets the hero respawn point |
| Door | trigger: hero stops, loads `nextSceneName` (MainMenu for now) after a delay |
| Lives | life counter, `Changed` event, `RestartLevel()` |
| FlamesHud | shows lit/spent flames from `Lives.Changed` |
| LightSensor | `IsLit()` for enemies: lantern radius + not in shadow |
| Patroller | dark spirit, waypoints; Patrol / Chase / WindUp 0.5 s / Recover 1.5 s / Flee; sight = distance, angle, raycast |
| Shooter | eye in the wall; Idle / Aim 0.6 s (red laser) / Cooldown 1.5 s / Blinded; main raycast sight, green/red debug line |
| Projectile | slow shot (5 u/s, Dynamic body, gravity 0); kills the hero, breaks on Walls |
| Charger | stone beast; Dormant / WindUp 0.7 s (red lane) / Dash / Stunned 1 s / Return home |
| CameraFit | zooms the camera out on screens narrower than 16:9 so the 17.8-unit level fits |
| StateLabelSwitch | one checkbox shows or hides the enemy state labels (off for playtest, on for the video) |
| SafeArea, MainMenu, PauseMenu | UI from Assignment 8-9 |
| Spinner | old pause test helper, not used in the scene any more (can be deleted) |

## Game scene (Assets/_Project/Scenes/Game.unity)
- Main Camera (ortho size 5, CameraFit), Global Light 2D (intensity 0.25), BackWall (warm #6E5C50 lit sprite).
- Canvas > SafeArea > Flames (FlamesHud), LevelText, PauseButton, Slider; PausePanel.
- RailStart (-6,4), RailEnd (6,4), Lantern (+ child LanternLight: Point Light 2D, radius from code).
- Block1 (-3.5, 0.6) + BlockShadow1, Block2 (4, 0.6) + BlockShadow2: StoneSlab sprite, 2 x 0.8 world units.
- Level: GroundLeft (x -9.5..-4.5), GroundMiddle (-1.5..2.5), GroundRight (5.5..9.5), Checkpoint2 (5.8), Door (8.6), HangingStone (Walls, hides the hero on bridge 2 from the Shooter), PatrolPointA/B.
- Hero (-8,-1.4): Rigidbody2D, CapsuleCollider2D, Hero, Animator (Hero.controller), HeroAnimator; child Body (SpriteRenderer, unlit).
- Patroller, Shooter, Charger: prefab instances (Assets/_Project/Prefabs), each with LightSensor and a TMP StateLabel child.
- Lives, StateLabels, EventSystem, PauseMenuController.

Layers: 6 Walls, 7 Shadow, 8 Hero, 9 Enemy. Slider starts at 0 (lantern x = -6).

## Level solution (enemy level for Assignment 10)
1. Hero waits at gap 1 -> lantern x ~ -4 (slider ~0.17): bridge 1.
2. Hero lands on the island -> lantern x ~ 4 (slider ~0.83): Charger and Patroller lit, Shooter blinded, bridge 2.
3. Hero on the last ground -> lantern x 6: everything near the door lit.
Skilled run: 12.4 s, no deaths. Simulated new player: 3 tries (fell once, Charger once). Bridge windows are 1 unit of lantern x each.

## Visuals
- URP 2D lights: dark hall, warm lantern light with a visible edge. Hero, enemies, door, lantern use `SpriteUnlit.mat` so they stay readable in the dark.
- Shadows: dark #1C1626 with a light violet edge line.
- Hero: hooded figure with a rose-pink scarf and a glowing yellow eye. Sprites HeroIdle (4), HeroWalk (8), HeroFall (2),
  clips in Assets/_Project/Animations, Hero.controller with states Idle / Walk / Fall.
- Stone blocks: generated StoneSlab sprite. Enemies are still simple shapes; an artist brief exists (private notes).
- Art sources are listed in README.md.

## Course status
- Assignment 8-9 (UI): done.
- Assignment 10 (Three Enemy Types), Canvas closes **Thu 12 Nov 12:30** (group OR1-3). Code and level done.
  Still to do: APK, playtest with 2 new people on a phone, change 2+ numbers (old -> new, tries before -> after),
  2-3 min English video with a script map, DECISIONS.md rewritten by the student.
- Assignment 11 (Data and Saves), expected Thu 19 Nov: move balance numbers into ScriptableObjects, 3 levels from data,
  JSON save in persistentDataPath with safe loading, reset progress with a confirm panel.

## Working notes for the agent
- The student commits through GitHub Desktop. Never run git.
- After each step: line-by-line explanation, list of scene changes, risks, and a suggested commit message.
- Testing through Unity MCP: the editor does not tick frames while it is in the background. Enter Play mode paused and drive
  frames with `EditorApplication.Step()` from an `EditorApplication.update` callback; pass results back through `SessionState`.
- Saving scripts makes Unity recompile and the MCP connection drops for ~10-60 s; wait for it to come back.
- Unity AI generation: the content filter blocks prompts or reference images that resemble known characters
  (it flagged the old hero sheet and the hooded concept sheets). Describe original designs in text instead.
- `AssetDatabase.DeleteAsset/RenameAsset` can open a confirm dialog that MCP cannot answer; ask the student to delete or rename in the Project window.
