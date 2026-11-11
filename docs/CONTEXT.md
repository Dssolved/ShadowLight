# Shadow & Light: project context

Read this at the start of every session, together with `CLAUDE.md` (rules).
Last updated: 2026-11-11 (art day: HUD, scene textures, companion, shadow bridges, animated enemies).

## The game
2D light-and-shadow puzzle for Android, landscape. Unity 6000.3.23f1, Universal 2D (URP), new Input System.

- The player moves a **lantern** along a rail at the top with a UI **slider**. The player never controls the hero.
- **Floating blocks** hang at a depth `z` in front of the wall. Their shadow on the wall is a projection from the lantern:
  `k = Lz / (Lz - z)`, `S(V) = L + (V - L) * k` for every block corner V (L = lantern, Lz = lantern depth).
  The shadow is a `PolygonCollider2D` on layer **Shadow**: the hero walks on it like ground ("shadow bridge").
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
| BlockShadow | projects the 4 block corners each LateUpdate into PolygonCollider2D + mesh + edge LineRenderer; mesh UVs come from world position x `textureScale` (0.25) so the smoke texture does not stretch |
| Hero | FSM Walk / Wait / Fall / Respawn / ReachedDoor; ground and edge rays; `Die()` is the only death path |
| HeroAnimator | feeds Animator parameters `Speed` and `Falling` from the Rigidbody2D velocity |
| Companion | the glowing orb next to the hero: SmoothDamp to hero + offset, sine bob; fields: target, offset, followSmoothTime, bobHeight, bobSpeed |
| Checkpoint | trigger: sets the hero respawn point |
| Door | trigger: hero stops, loads `nextSceneName` (MainMenu for now) after a delay |
| Lives | life counter, `Changed` event, `RestartLevel()` |
| FlamesHud | shows lit/spent flames from `Lives.Changed`; swaps two sprites (`litSprite`, `spentSprite`) |
| LightSensor | `IsLit()` for enemies: lantern radius + not in shadow |
| SpriteStateAnimator | frame animation without Animator: `clips[]` (frames, framesPerSecond, loop) indexed by the enemy state enum; `Play(int)`; `autoPlayClip` for the projectile |
| Patroller | dark spirit, waypoints; Patrol / Chase / WindUp 0.5 s / Recover 1.5 s / Flee; sight = distance, angle, raycast; calls `spriteAnimator.Play((int)state)` |
| Shooter | eye in the wall; Idle / Aim 0.6 s (red laser) / Cooldown 1.5 s / Blinded; main raycast sight, green/red debug line; calls `spriteAnimator.Play` |
| Projectile | slow shot (5 u/s, Dynamic body, gravity 0); rotated along its direction in `Launch`; kills the hero, breaks on Walls |
| Charger | stone beast; Dormant / WindUp 0.7 s (red lane) / Dash / Stunned 1 s / Return home; flips toward the dash target and toward home |
| CameraFit | zooms the camera out on screens narrower than 16:9 so the 17.8-unit level fits |
| StateLabelSwitch | one checkbox shows or hides the enemy state labels (off for playtest, on for the video) |
| SafeArea, MainMenu, PauseMenu | UI from Assignment 8-9 |
| Spinner | old pause test helper, not used in the scene any more (can be deleted) |

Enemy state enum order = clip order in `SpriteStateAnimator` (Charger: Dormant, WindUp, Dash, Stunned, Return; Patroller: Patrol, Chase, WindUp, Recover, Flee; Shooter: Idle, Aim, Cooldown, Blinded). Never reorder one without the other.

## Game scene (Assets/_Project/Scenes/Game.unity)
- Main Camera (ortho size 5, CameraFit), Global Light 2D (intensity 0.25).
- **BackWall:** Tiled sprite `WallTile` (tile 8 x 8 units), size 24 x 16, scale 1, tint (0.85, 0.80, 0.78).
- **Ground** (GroundLeft x -9.5..-4.5, GroundMiddle -1.5..2.5, GroundRight 5.5..9.5): Sliced sprite `GroundBlock` (size w x 3.6), **scale 1**, BoxCollider2D sized w x 3 (top at y = -2). Door, HangingStone, BackWall are also scale 1 with real collider sizes.
- Canvas > SafeArea > Flames (FlamesHud, flames 70 x 100 so the world lantern at the left end of the rail does not overlap them), LevelText (top left under the flames, size 48, left aligned), PauseButton (still default Unity look), Slider; PausePanel (still default).
- **Slider:** Background = `SliderTrack` (Sliced, 96 high), Fill = `SliderFill` (Sliced, 30 high), Handle = `SliderLantern` (84 wide, 150 high), Transition None, anchored at y = 100.
- RailStart (-6,4), RailEnd (6,4), **RailVisual** (picture only, y = 4.62, sprite `Rail`), Lantern (sprite `WorldLantern`, scale 0.6 kept so the light is unchanged; child LanternLight: Point Light 2D, radius from code).
- Block1 (-3.5, 0.6) + BlockShadow1, Block2 (4, 0.6) + BlockShadow2: StoneSlab sprite, 2 x 0.8 world units. BlockShadow vertex color is white; the picture comes from `Shadow.mat` + `ShadowSmoke.png`.
- Level: Checkpoint2 (5.8) with child **CheckpointBrazier**, Door (8.6, `DoorArch`), HangingStone (`HangingRock`, Walls layer, hides the hero on bridge 2 from the Shooter), PatrolPointA/B.
- Hero (-8,-1.4): Rigidbody2D, CapsuleCollider2D, Hero, Animator (Hero.controller), HeroAnimator; child Body (SpriteRenderer, unlit, **local scale 1.3**; colliders and ray lengths were NOT changed).
- **CompanionOrb:** SpriteRenderer (`Orb`, unlit, order 5, scale 1.1), Companion (target = Hero, offset (-0.4, 0.95)), child OrbLight (Point Light 2D, pale blue, radius 2.2, intensity 0.7, no shadows). It does not affect `LightSensor` (that uses code, not Light2D).
- Patroller, Shooter, Charger, Projectile: prefab instances (Assets/_Project/Prefabs). Root scales stay 0.6 / 0.6 / 0.8 / 0.25 because the child StateLabel inherits the scale; the sprite PPU is multiplied by that scale to compensate. Each has LightSensor (not the projectile), SpriteStateAnimator and a TMP StateLabel child. The state color fields of the enemy scripts are all white now (the art carries the look).
- Lives, StateLabels, EventSystem, PauseMenuController.

Layers: 6 Walls, 7 Shadow, 8 Hero, 9 Enemy. Slider starts at 0 (lantern x = -6).

## Level solution (enemy level for Assignment 10)
1. Hero waits at gap 1 -> lantern x ~ -4 (slider ~0.17): bridge 1.
2. Hero lands on the island -> lantern x ~ 4 (slider ~0.83): Charger and Patroller lit, Shooter blinded, bridge 2.
3. Hero on the last ground -> lantern x 6: everything near the door lit.
Skilled run: 12.4 s, no deaths. Simulated new player: 3 tries (fell once, Charger once). Bridge windows are 1 unit of lantern x each.
Not yet re-run with the new art (hero x1.3 visual, bigger orb light, textures): do one full run on the first playtest build.

## Visuals (art pipeline and sources)
- URP 2D lights: dark hall, warm lantern light with a visible edge. Hero, enemies, door, lantern, rail, brazier, orb use `SpriteUnlit.mat` so they stay readable in the dark; wall, ground and slabs are lit.
- Style: hand-painted storybook, palette: shadow #1C1626, warm amber #F2B84B for light, rose-pink scarf.
- `Sprites/UI`: SliderLantern, SliderTrack, SliderFill, FlameLit, FlameSpent (Unity AI, gemini-3.1-flash, Photoroom background removal, cropped by script).
- `Sprites/Scene`: WallTile and ShadowSmoke (Unity AI `gemini-3.1-flash-texture`, tileable), GroundBlock, DoorArch, HangingRock, Brazier (gemini-3.1-flash + Photoroom), Rail and WorldLantern (copies of the slider art with other PPU), Orb (generated by code: radial gradient, no AI).
- `Sprites/Enemies/<Charger|Patroller|Shooter|Projectile>/<state>_<n>.png`: the author's own enemy sheets (`_notes/art/enemy_*_sheet.png`, converted from webp), keyed from the white background by script and cut into frames. Pivot: Charger ground-contact, Patroller and Projectile 85% to the head, Shooter center.
- Stone blocks and hero as before: StoneSlab, HeroIdle (4), HeroWalk (8), HeroFall (2), clips in Assets/_Project/Animations, Hero.controller with Idle / Walk / Fall.
- Pause button, pause panel, main menu and the LEVEL text are still default Unity UI (planned after the Assignment 10 deadline).
- Art sources are listed in README.md.

## Course status
- Assignment 8-9 (UI): done.
- Assignment 10 (Three Enemy Types), Canvas closes **Thu 12 Nov 12:30** (group OR1-3). Code, level and art done.
  Still to do: APK, playtest with 2 new people on a phone, change 2+ numbers (old -> new, tries before -> after),
  2-3 min English video with a script map, DECISIONS.md rewritten by the student.
- Assignment 11 (Data and Saves), Canvas closes Thu 19 Nov 12:30 (OR1-3): move balance numbers into ScriptableObjects, 3 levels from `LevelData` assets,
  JSON save in persistentDataPath with safe loading, save on level end / settings change / OnApplicationPause, reset progress with a confirm panel, level select with locked levels.
  The full brief is in the private notes (`_notes/course`, summary in `_notes/ASSIGNMENTS.md`).

## Working notes for the agent
- The student commits through GitHub Desktop. Never run git.
- After each step: line-by-line explanation, list of scene changes, risks, and a suggested commit message.
- Testing through Unity MCP (Play mode): in one `RunCommand` set `EditorApplication.isPaused = true; isPlaying = true`, wait ~20 s (domain reload), then in a second `RunCommand` loop `EditorApplication.Step()` (about 0.03 s per step). Reaching the door loads MainMenu and losing all lives reloads Game, so look objects up fresh inside the loop and disable the `Hero` component to hold him still. Exit with `isPlaying = false` and wait before the next edit. Scene edits made in Play mode are discarded.
- Screenshots of the game (the Canvas is Screen Space Overlay and is not captured by cameras): temporarily set the Canvas to Screen Space - Camera with the main camera, sortingOrder 500, render the camera into a RenderTexture, write a PNG to the scratchpad, restore the Canvas. BlockShadow meshes only exist in Play mode.
- `RunCommand` does not allow `System.Reflection`; read state through sprite names or `SerializedObject`.
- Setting `SpriteRenderer.drawMode` to Sliced or Tiled through code can multiply the transform scale; reset `localScale` to 1 and check `lossyScale` afterwards. Sliced and Tiled sprites must use Mesh Type Full Rect.
- `Mathf.SmoothStep(from, to, t)` in Unity is not the shader smoothstep; write t*t*(3-2t) by hand.
- Saving scripts makes Unity recompile and the MCP connection drops for ~10-60 s; wait for it to come back. Use a `for` loop with `sleep` in Bash to wait.
- Sprite sheets on white: key by `min(r,g,b)` (alpha = (0.97 - m) / 0.22, color un-premultiplied against white), cut rows and columns by gaps (rows >= 12 px, frames >= 36 px) and drop islands smaller than 50 px. Windows can decode webp through PowerShell and WPF (`BitmapImage`); Unity cannot.
- Unity AI generation: the content filter blocks prompts or reference images that resemble known characters
  (it flagged the old hero sheet and the hooded concept sheets). Describe original designs in text instead. Pass `forceGeneration: true` after a blocked or interrupted generation. The user confirmed generation once in the 11 Nov session; ask again in a new session.
- `AssetDatabase.DeleteAsset/RenameAsset` can open a confirm dialog that MCP cannot answer; ask the student to delete or rename in the Project window.
