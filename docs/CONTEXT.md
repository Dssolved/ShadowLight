# Shadow & Light: project context

Read this at the start of every session, together with `CLAUDE.md` (rules).
Last updated: 2026-11-19, night. Assignment 11 was handed in on 19 Nov. Assignment 12 (Sound Pass, deadline Thu 26 Nov 12:30) has all code, the mixer, the sounds, the spawn door and the new level 2 and 3 backgrounds done in the editor; left: tests on a phone, DECISIONS.md check, APK, mixer screenshot, video, upload (see `_notes/HANDOFF.md`). Section "Sound (Assignment 12)" below describes the audio system.

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
| BlockShadow | each LateUpdate projects the 4 block corners into the PolygonCollider2D (the walkable bridge), moves and scales the child `Visual` SpriteRenderer (soft silhouette of the block sprite, scale = block scale * k) and rebuilds a faint dark cone mesh (child `Cone`, 4 quads between block corners and shadow corners, alpha 0.3 -> 0.08); `Init(lantern)` from LevelLoader |
| BrazierGlow | on the brazier sprite object (SpriteLit material since 19 Nov): cold sprite until `Ignite()`, then lit sprite, warm Light2D (pop 3.2 -> 1.8, Perlin flicker), the fire sound and the `Lit` event (once) |
| BrazierHeal | on the Checkpoint: listens to `BrazierGlow.Lit`; heals `healPerBrazier` only if the level has no record yet and the brazier key (level * 10 + index) is new in `GameSession.healedBraziers`; `Init(lives, level, index)` from LevelLoader |
| Hero | FSM Walk / Wait / Fall / Respawn / ReachedDoor / Emerge (Emerge is last in the enum; the hero starts in it for `PlayerStats.emergeSeconds` = 1 s, standing still, then Fall -> Walk; a respawn at a checkpoint skips it); ground and edge rays; `Die()` is the only death path (plays the death sound); `DieFromHit()` is called by Patroller, Charger and Projectile: it plays the hit sound and calls `Die()`; `CanDie` blocks double deaths; numbers from PlayerStats |
| HeroFadeIn | on the Hero: sets the Body sprite alpha to 0 in Awake and fades it to 1 in `fadeSeconds` (0.4), then disables itself |
| HeroFootsteps | on the Hero: while `abs(velocity.x) >= minSpeed` (0.5) plays a footstep every `stepIntervalSeconds` (0.45); the timer is reset when standing; raises the event `Stepped` on every step |
| ShadowStepRipple | on the Hero (20 Nov): on `Stepped` raycasts down on the Shadow layer; if it hits a shadow bridge, one of 4 pooled ring sprites (`StepRipples/Ring0..3`, `RingRipple.png`) appears at the hit point, grows and fades in 0.6 s. Gives the bridge a magical surface so the hero does not look like he floats |
| ShadowStepPuff | on the Hero (20 Nov): same trigger; 2 tiny pale motes per step from a pool of 12 (`StepPuffs/Puff0..11`, `MistPuff.png`) rise straight up and fade in 0.8 s |
| HeroAnimator | feeds Animator parameters `Speed` and `Falling` from the Rigidbody2D velocity |
| Companion | the glowing orb next to the hero: SmoothDamp to hero + offset, sine bob |
| Checkpoint | trigger (2 units wide): sets the hero respawn point and calls `glow.Ignite()` |
| Door | trigger on the exit door: hero stops, win sound, `SaveSystem.Data.CompleteLevel(levelIndex, time, flames)` + `Save()`, then after `loadDelaySeconds` (2.5 since 19 Nov, set in the three layout prefabs; the script default is still 1) loads the next level (sets `GameSession.levelIndex + 1`, reloads Game) or MainMenu after the last level; `Init(lives, index, hasNextLevel)` from LevelLoader |
| Lives | shared pool: Awake takes `SaveSystem.Data.flames` (-1 -> startLives), raises it to `minLivesAtLevelStart`, clamps to `maxLives`; `LoseLife`, `Heal(amount)` write the pool back into the save data and fire `Changed`; `RestartLevel()` plays the lose sound, sets the pool to the floor and reloads the scene (GameSession keeps the level) |
| FlamesHud | on `Lives.Changed`: text "xN", 10 ember Images (full/empty), flame icon red + rotation tremble at <= 2, pop scale and colors on loss or heal (unscaled time) |
| LightSensor | `IsLit()` for enemies: lantern radius + not in shadow; `Init(lantern)` |
| SpriteStateAnimator | frame animation without Animator: `clips[]` indexed by the enemy state enum; `Play(int)` |
| Patroller | dark spirit; waypoints are `Vector2[]` (from LevelData); Patrol / Chase / WindUp 0.5 s / Recover 1.5 s / Flee; `Init(hero, lantern, waypoints)` |
| Shooter | eye in the wall; Idle / Aim 0.8 s (red laser) / Cooldown 1.5 s / Blinded; looks left (`lookDirection` in the prefab); `Init(hero, lantern)` |
| Projectile | shot from the Shooter, speed from ShooterStats (12 u/s since 17 Nov: at 5 it always flew over a walking hero); kills the hero, breaks on Walls |
| Charger | stone beast; Dormant / WindUp 0.8 s (red lane) / Dash / Stunned 1 s / Return home (home = spawn position); `Init(hero, lantern)` |
| PatrollerStats, ShooterStats, ChargerStats, PlayerStats, LanternStats | ScriptableObjects with all balance numbers (public fields, `[CreateAssetMenu]`, no runtime state). Assets in `Assets/_Project/Data` |
| LevelData | ScriptableObject per level: `levelName`, `layoutPrefab`, `backgroundPrefab`, `ambientColor`, `ambientIntensity`, `heroStart`, `parTimeSeconds`, `List<EnemySpawn> enemies` (EnemySpawn = prefab, position, waypoints) |
| LevelList | ScriptableObject with `LevelData[] levels` in play order; read by LevelLoader and LevelSelect |
| GameSession | static `levelIndex` (set by the menu or the door, survives scene loads) and `healedBraziers` (HashSet<int>, cleared only when the app restarts) |
| LevelLoader | in Game: clamps `GameSession.levelIndex`, sets the HUD text (two lines: `LEVEL 2: THE WATCHER` and a smaller `GOAL 18 S`), puts the hero at `heroStart`, `BuildBackground` (sets the Global Light color and intensity from LevelData, instantiates `backgroundPrefab`), instantiates `layoutPrefab` (Init on every BlockShadow, the Door and every BrazierHeal) and every enemy (TryGetComponent -> Init, then `StateLabelSwitch.Apply`) |
| StateLabelSwitch | `Apply(enemy)` shows or hides the TMP state labels of a spawned enemy (`showStateLabels` OFF for builds, ON only for videos) |
| SaveData | `[Serializable]` save file content: `version` (1), `unlockedLevels` (List<int>, level 0 always), `records` (List<LevelRecord>: levelIndex, bestTimeSeconds, bestFlamesLeft), `musicVolume`, `sfxVolume` (0.0001-1, default 0.8), `flames` (shared lives pool, -1 = not set; added on 17 Nov without a version bump because old files read it as the default). `IsUnlocked`, `IsCompleted`, `FindRecord`, `CompleteLevel` (unlocks index + 1, keeps lower time and higher flames), `Sanitize` |
| SaveSystem | static; `Data` loads lazily; `Load` returns defaults for a missing file, broken JSON, null or another version (warning in the console, never a crash) and calls `Sanitize`; `Save` writes `save.json.tmp`, then `File.Replace` (or `File.Move` the first time); `Reset` = new data + delete the file. JsonUtility only, no BinaryFormatter, no PlayerPrefs |
| SaveOnPause | `OnApplicationPause(true)` -> `SaveSystem.Save()` (one object in each scene) |
| SliderReleaseSave | `IPointerUpHandler` on a settings slider: saves when the finger is released |
| SettingsPanel | MainMenu settings: music and sound sliders write into `SaveSystem.Data` and call `MixerVolume.Apply` (has an `AudioMixer mixer` field); Reset Progress opens a confirm panel; Yes calls `SaveSystem.Reset()` and applies the default volumes |
| LevelSelect | MainMenu level select: in `OnEnable` every button gets `interactable = IsUnlocked(i)` and the text `1. FIRST LIGHT   BEST 11.3 S` or `LOCKED`; `PlayLevel(i)` sets GameSession and loads Game |
| MainMenu | only `Quit()` now (Play opens the level select panel) |
| CameraFit | zooms the camera out on screens narrower than 16:9 so the 17.8-unit level fits |
| SafeArea | UI from Assignment 8-9 |
| PauseMenu | UI from Assignment 8-9 + sound: `SetPaused` also does `TransitionTo(snapshotFadeSeconds = 0.3)` to the `pausedSnapshot` or `normalSnapshot` (serialized AudioMixerSnapshot fields); `ToMenu` returns to Normal |
| SoundBank, SfxPlayer, MusicManager, MixerVolume, SceneMusic, ButtonClickSounds, LanternSound | the audio system, see the section "Sound (Assignment 12)" |
| Spinner | old helper, not used (the student may delete it) |

Enemy state enum order = clip order in `SpriteStateAnimator` (Charger: Dormant, WindUp, Dash, Stunned, Return; Patroller: Patrol, Chase, WindUp, Recover, Flee; Shooter: Idle, Aim, Cooldown, Blinded). Never reorder one without the other.

## Data assets (Assets/_Project/Data)
- Stats: PatrollerStats, ShooterStats (aim 0.8, projectileSpeed 12), ChargerStats (windUp 0.8, triggerRange 3), PlayerStats (walkSpeed 1.5, maxLives 10, startLives 6, minLivesAtLevelStart 3, healPerBrazier 1), LanternStats (depthFromWall 10, lightRadius 6, moveSpeed 4). All checked on 17 Nov before the APK.
  Play-mode edits of these assets stay after Play mode ends: check the values before every build.
- Levels: `Data/Levels/Level_01.asset`, `Level_02.asset`, `Level_03.asset`, `LevelList.asset` (order 1, 2, 3). Ambient light per level: 1 = (0.62, 0.58, 0.88) x 0.20, 2 = (0.78, 0.72, 0.88) x 0.25, 3 = (0.92, 0.82, 0.82) x 0.32.
- Level geometry prefabs: `Prefabs/Levels/Level_0X_Layout.prefab` (ground, blocks with shadows, shelter slabs, checkpoint with brazier, door). Blocks: `Block1` = SlabA, `Block2` = SlabB, `ShelterBlock` = SlabShelter, each with its own `...Shadow.png` silhouette in the BlockShadow `Visual` child.
- Backgrounds (rebuilt on 19 Nov): the scene object `BackWall` (tiled `WallTile`, order -10) stays for every level. `Prefabs/Backgrounds/Bg_01` = hanging roots only (original look). `Bg_02` = `Wall` (tiled `WallTile_Damp`, 24 x 16 at (0, 0.5), order -9, lit) + `Roots` + props `WallRing` (-2.6, 2.3) and `TorchBracket` (2.3, 2.4, scale 1.25). `Bg_03` = `Wall` (tiled `WallTile_Warm`, sprite color (0.72, 0.68, 0.64) to darken it) + `Banner` (-0.6, 2.9) + `TorchBracketLeft` (-4.8, 2.4) and `TorchBracketRight` (1.8, 2.4). Props use `SpriteLit.mat`, order -8. The old ceiling-crack sticker, arched-window sticker, their lights and light shafts were removed because they did not blend with the wall (several attempts with full-wall pictures from Gemini failed on brick scale, so the final way is: per-level seamless tile variants + separate props). Files `ArchWindow.png`, `CeilingCrack.png` and `LightShaft.png` in `Sprites/Backgrounds` are no longer used.
- Known: the generated tiles are not perfectly seamless (a faint seam every 8 units, worse after darkening). Edge brightness was matched with a script; the originals are not kept in the project. Level 1 still uses the original `WallTile`.

## The three levels (all reach-the-door, hero starts at (-8.3, -1.4) in front of the spawn door since 19 Nov, lantern starts at x -6)
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
- Canvas > SafeArea > Flames (Flame1 icon, Count text; Flame2 and Flame3 are switched off leftovers), Embers (10 ember Images), LevelText (900x110, two lines, set by LevelLoader), PauseButton (round stone), Slider (lantern art); PausePanel > PauseSafeArea > Frame, PausedTitle, Resume/Restart/Menu. EventSystem, PauseMenuController. LevelLoader also holds `globalLight` (the Global Light 2D).
- In the editor the Game scene shows no ground or enemies until Play mode. Edit geometry in the layout prefabs and enemies in the LevelData assets.

**MainMenu**: camera clear color #120E18; Canvas > WallBackdrop (tiled WallTile, dark), Background (dark overlay 0.55), SafeArea (Title, PlayButton -> `LevelSelectPanel.SetActive(true)`, SettingsButton, QuitButton);
SettingsPanel (off; music and sound sliders 0.0001-1 with SliderReleaseSave, Reset Progress, Back, ConfirmPanel with Yes/Cancel);
LevelSelectPanel (off; title, Level1-3 buttons -> `LevelSelect.PlayLevel(0/1/2)`, Back). MainMenuController (Quit), SaveOnPause, EventSystem.

Audio objects in both scenes (added 19 Nov): an instance of the prefab `Prefabs/AudioManager` and an empty object `SceneAudio` with `SceneMusic` (MainMenu: `MenuMusic_GeminiLoop`, Game: `LevelMusic_GeminiLoop`) and `ButtonClickSounds`. In Game: `PauseMenu` has the Normal and Paused snapshots, the Hero has `HeroFootsteps` and `HeroFadeIn`, the Lantern has an `AudioSource` (loop, SFX group) and `LanternSound`. In MainMenu: `SettingsPanel.mixer` is set.
Level layout prefabs got an extra object `SpawnDoor` (the entrance door sprite `SpawnDoor.png`, 1.49 x 1.6, at (-8.4, -1.25), SpriteLit, no collider) and the exit `Door` got a child `ExitLight` (Light2D point, color (1, 0.78, 0.45), intensity 1, outer radius 3).

Layers: 6 Walls, 7 Shadow, 8 Hero, 9 Enemy.

## Save file
- Path: `Application.persistentDataPath/save.json`. Editor: `%USERPROFILE%\AppData\LocalLow\DefaultCompany\ShadowLight\save.json`. On Android: the app's persistent data folder (use `adb` or print the path to show it).
- Saved: when the hero enters a door, when a settings slider is released, in `OnApplicationPause(true)`, and after Reset (file deleted).
- Example:
  ```json
  { "version": 1, "unlockedLevels": [0, 1], "records": [ { "levelIndex": 0, "bestTimeSeconds": 12.3, "bestFlamesLeft": 3 } ], "musicVolume": 0.8, "sfxVolume": 0.8 }
  ```
- Volumes are applied to the AudioMixer in decibels: at launch (`MixerVolume.Start`) and on every slider change (`SettingsPanel`).
- `CompleteLevel` on the last level adds index 3 to `unlockedLevels`; LevelSelect only looks at its 3 buttons, so this is harmless.

## Visuals (art pipeline and sources)
- URP 2D lights: dark hall, warm lantern light with a visible edge. Hero, lantern, rail, orb and the Projectile use `SpriteUnlit.mat`; wall, ground, slabs, both doors (since 19 Nov), braziers (since 19 Nov), background tiles and props, enemies (`SpriteLit.mat`, each with a faint violet `Aura` Light2D so they stay visible in the dark) are lit. Observed once in Play captures: when a brazier is lit its Light2D seems to darken the scene to the right of the lantern radius (the same with an unlit brazier sprite, so it comes from the light, not the material); the student has not confirmed it in the Game view. Light2D components made by code have an empty sorting-layer list: set `m_ApplyToSortingLayers` through SerializedObject or they light nothing.
- Style: hand-painted storybook, palette: shadow #1C1626, warm amber #F2B84B for light, rose-pink scarf, cream UI text #F2E6C8.
- `Sprites/UI`, `Sprites/Scene`, `Sprites/Backgrounds`, `Sprites/Enemies/...`: see README.md for sources (Unity AI generation, editor scripts and the author's own enemy sheets). Fonts: `Fonts/Cinzel.ttf` (SIL OFL, license file next to it) and `Cinzel SDF.asset` (dynamic TMP font asset with the ASCII set).
- UI kit (`Sprites/UI`): ButtonFrame and PanelFrame (9-slice, `pixelsPerUnitMultiplier` 3 for buttons and 1.5 for panels), RoundButton (pause), slider art (SliderTrack, SliderFill, SliderLantern) used by the lantern slider and both settings sliders, EmberFull/EmberEmpty (made by script). All UI texts use Cinzel and cream color.
- Known visual nits: Charger asleep looks like a dark rock; slab sprites look pale inside the lantern light (lit material and light intensity 1.8); the entrance door at x -8.4 touches the left screen edge on 16:9; background tiles show a faint seam.
- New art on 19 Nov (all listed in README): `SpawnDoor.png`, `WallTile_Damp.png`, `WallTile_Warm.png`, `WallRingChain.png`, `WallTorchBracket.png`, `WallBanner.png` (Unity AI, Nano Banana 2.1 `gemini-nano-banana-2-1` for sprites, `gemini-3.1-flash-texture` for tiles, `photoroom-bg-removal`).

## Sound (Assignment 12)
**Mixer** `Assets/_Project/Audio/MainMixer.mixer`: groups Master -> Music -> Tracks (child) and Master -> SFX. Exposed parameters `MusicVol` (Music group volume) and `SFXVol` (SFX group volume); the volume sliders set them, so they override snapshots for these two parameters. Therefore the base level and the pause effect live on the non-exposed child group **Tracks**: snapshot `Normal` = Tracks -10 dB, Lowpass Simple 22000 Hz (start snapshot); `Paused` = Tracks -16 dB, 800 Hz. All music AudioSources (`MusicA`, `MusicB`) output to Tracks; effect AudioSources (`Voice0..5`, the lantern loop) output to SFX. No UI group: button clicks go through SFX.

**Scripts**
| Script | What it does |
|---|---|
| SoundBank | ScriptableObject `Audio/SoundBank.asset`: one `AudioClip[]` per event (footsteps, heroHit, heroDeath, brazierFire, patrollerWindUp, shooterAim, shooterShot, chargerWindUp, uiClick, win, lose) |
| SfxPlayer | singleton (`Instance`, duplicate destroys itself, `DontDestroyOnLoad`); `Bank` property; `Play(AudioClip[])` picks a random clip that differs from the last one, sets a random pitch 0.92-1.08 and plays it on the next of 6 voice AudioSources (round robin, so the pitch of a playing sound is never changed) |
| MusicManager | singleton; two AudioSources; `Play(clip)` returns early when that clip is already playing, else swaps the sources and crossfades 1 s with `unscaledDeltaTime`; loops |
| MixerVolume | on the AudioManager: `Start` sets `mixer.updateMode = UnscaledTime` and applies the saved volumes; static `Apply(mixer, music, sfx)` writes `Log10(max(v, 0.0001)) * 20` into `MusicVol` and `SFXVol` |
| SceneMusic | in `Start` asks `MusicManager` to play the clip of the scene |
| ButtonClickSounds | in `Start` finds every Button (inactive too) with `FindObjectsByType` and adds a click sound listener |
| LanternSound | on the Lantern next to a looping AudioSource: volume follows the lantern speed (`maxVolume` 0.35, `fullVolumeSpeed` 4, `volumeChangePerSecond` 1.5, pitch 0.95-1.1), fades to 0 when still or paused |
Calls elsewhere: `SfxPlayer.Instance.Play(SfxPlayer.Instance.Bank.xxx)` in Hero (death, hit), Lives.RestartLevel (lose), Door (win), BrazierGlow.Ignite (fire), Patroller.SetState(WindUp), Charger.SetState(WindUp), Shooter.SetState(Aim) and Shooter.Fire (aim, shot), HeroFootsteps, ButtonClickSounds.
Both scenes contain the AudioManager prefab; the one from the first scene survives (`DontDestroyOnLoad`) and later copies destroy themselves. `SetFloat` runs in `Start`, not in `Awake`.

**Files** (`Assets/_Project/Audio`, sources and licenses in README "Sound sources"): `Music/MenuMusic_GeminiLoop.ogg`, `LevelMusic_GeminiLoop.ogg` (Gemini music with the end crossfaded into the start; streaming, Vorbis); `Sfx/`: Footstep1-3, HeroHit, HeroDeath, BrazierFire, PatrollerGhost (patroller wind-up), ShooterAim, ShooterShot, ChargerWindUp, UiClickB, Win, Lose (cut from Gemini tracks, stereo), LanternSlide (Pixabay "Rocks sliding", seamless 5 s loop). SFX are mono, Decompress On Load, ADPCM. Unused leftovers the student may delete: `Music/MenuMusic.wav`, `LevelMusic.wav` (Lyria), `Sfx/` StepA/B, HitA/B, ShotA/B, PatrollerWindUp, BrazierIgnite*.
Lessons (also for DECISIONS.md): MusicGen and ffmpeg-synthesized effects were rejected by the student; real recordings (Pixabay, ZvukBox) and Gemini music worked; Gemini tracks end with a fade-out, so a loop needs a tail-into-head crossfade.

## Course status
- Assignment 8-9 (UI): done. Assignment 10 (Three Enemy Types): handed in 12 Nov. **Assignment 11 (Data and Saves): handed in 19 Nov.**
- **Assignment 12 (Sound Pass), Canvas closes Thu 26 Nov 2026 12:30 (OR1-3).** Done in the editor: mixer, all scripts and scene wiring above, 11 sound events plus the lantern loop, music crossfade, pause snapshots, volume sliders in dB applied at launch, README sound sources, DECISIONS.md Week 12 draft. Left, in this order: listening tests (editor and phone: footsteps, enemy wind-ups heard before they land, sliders, pause, win, lose, lantern loop, close and reopen keeps volume), screenshot of the Audio Mixer window (groups and the Paused snapshot), APK, one 2-3 min English MP4 with the student's voice (ears test, settings test, the events, crossfade, pause, script map), upload. Details and order in `_notes/HANDOFF.md`.
- Unity AI credits expire on 24 Nov.
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
- `Mathf.SmoothStep(from, to, t)` in Unity is not the shader smoothstep; write t*t*(3-2t) by hand. (It cost one sprite on 17 Nov: the output alpha was `(byte)` of a negative number.)
- The editor can be in Play mode (paused) because the student started it: `isPlaying` then makes scene edits vanish. Check `EditorApplication.isPlaying` first and stop it before editing scenes. Play mode does not advance frames between tool calls unless Unity is focused.
- UI preview: switch the Canvas to Screen Space - Camera (`worldCamera = Camera.main`, `planeDistance = 5`), activate the panel, `Unity_Camera_Capture` with the camera instance id, then set the Canvas back to Overlay, hide the panel and save the scene. `OpenScene(..., Additive)` of the other scene logs a harmless "More than one global light" error.
- Build errors: read `C:\Users\raket\AppData\Local\Unity\Editor\Editor.log` (grep for "Internal build system error" and read the lines before it). On 17 Nov one build failed with "Couldn't launch process clang++ ... Access is denied" (likely antivirus); building again fixed it.
- Unity AI generation: `GetModels` returns an empty list until the student is signed in and credits show in the Unity AI window; model ids that worked: `gemini-3.1-flash` (sprites, edits), `photoroom-bg-removal`, `game-ui-elements-flux`. Reference images by instance id work for style. Edit a copy of a sprite with `EditSpriteWithPrompt`, never the original.
- TMP font asset from a TTF through MCP: `TMP_FontAsset.CreateFontAsset` + `AssetDatabase.CreateAsset` in one command, add atlas texture and material as sub assets in a second one, `TryAddCharacters(string, out string)` in a third (one big command was refused with "User interactions are not supported").
- Enemy and prefab scripts must not use `using System;` together with `UnityEngine.Random` (ambiguous); write `System.Action` instead.
- Unity AI generation: describe original designs in text (the filter blocks references that resemble known characters); pass `forceGeneration: true` after a blocked generation; ask the student for consent in every new session. Model ids on 19 Nov: `gemini-nano-banana-2-1` (sprites and images; `width`/`height` are ignored, it returns 2048 x 2048), `gemini-3.1-flash-texture` (seamless tiles, use `GenerateImage` with a reference image; the result follows the reference layout), `photoroom-bg-removal` (`RemoveSpriteBackground`), `gemini-3.0-pro`, `flux-2-dev` (custom resolutions). No sound-effect model except `elevenlabs-sound-effects-v3`, which was not tried.
- **Play-mode stall after a texture import:** right after importing or rewriting a big texture the first Play often stays at `Time.frameCount` 1-2 for a minute. Stop Play, start it again, then it runs. Read the frame count with a command before taking a capture. `Camera.main` instance id changes every Play: fetch it again before `Unity_Camera_Capture`. Scene change inside Play: `GameSession.levelIndex = n; SceneManager.LoadScene("Game")` from a command.
- No Python on this PC. ffmpeg: `/c/Users/raket/AppData/Local/Microsoft/WinGet/Links/ffmpeg` (trim, convert, normalize, crossfade loops, `eq` for saturation, `geq` for masks). Image processing that needs pixels: write a C# `RunCommand` (`ImageConversion.LoadImage`, `GetPixels32`, `EncodeToPNG`, `File.WriteAllBytes`, then `AssetDatabase.ImportAsset`). `System.Reflection` is forbidden in commands (use SerializedObject).
- Generated wall pictures: Gemini (web, the student's Plus account) gives 2528 x 1696 or 2752 x 1536 images but cannot hold the brick scale; the "looks like the reference tile repeated 3 x 2 times" trick helped once. A tile variant from the texture model keeps the reference brick layout and scale (best result).
- A prop sprite from Unity AI needs: `GenerateSprite`, then `RemoveSpriteBackground`, then an editor script that crops to the alpha bounds and sets `spritePixelsPerUnit = pixelHeight / wantedWorldHeight`.
