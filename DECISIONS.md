# Decisions log

What I asked the agent, what it got wrong, what I changed and why. 3-5 lines per week.

## Week 10 (9-15 Nov)

2026-11-10  Planned the enemies with Claude Code. Decided "lit" means "not inside a shadow collider" (OverlapPoint on the Shadow layer), not a ray from the lantern: one cheap check, and it makes the puzzle: a shadow is a bridge for the hero but a hiding place for enemies.
2026-11-10  Asked for the shadow projection. The shadow mesh looked paler than #8D7FB0: the project uses Linear color space and mesh vertex colors are not converted. Fixed with shadowColor.linear.
2026-11-10  Hero FSM: when the shadow bridge is removed, the hero now falls straight down instead of keeping his walking speed, so the player can predict where he lands.
2026-11-10  The lecture code puts units in comments, but our rule is no comments. Units go into field names instead (respawnDelaySeconds, loadDelaySeconds).
2026-11-10  Patroller: the first rule "lit = not in a shadow" made every enemy lit almost everywhere, because the whole wall is lit. Added a light radius around the lantern (5.5 units, shown as a soft circle): an enemy is lit only near the lantern and outside shadows.
2026-11-10  Patroller wind-up 0.5 s with a red color; lighting it during the wind-up cancels the hit. This is the player's counter, because the hero is not controlled.
2026-11-10  Shooter shot is a slow projectile (5 units/s), not an instant raycast hit, so the laser and the shot are both readable. The projectile uses a Dynamic Rigidbody2D with gravity 0: a Kinematic body would not get trigger events from the static walls.
2026-11-10  Charger test showed a level bug: the lantern hangs at y = 4 and the ground is at y = -1.6, so with a light radius of 5.5 nothing on the ground could ever be lit. Light radius 5.5 -> 7 (the light now reaches about 4 units to each side at ground level).
2026-11-10  Charger dash stops at the edge of the ground (same down-ray check as the hero), so it never flies over a pit.
2026-11-10  Built the enemy level with two gaps: each bridge needs a different lantern position, and the lantern also decides which enemy is lit. A simulated new player needed 3 tries (fell once, Charger hit once).
2026-11-10  The simulation showed the hero respawning at the island checkpoint right next to an active Patroller. Removed that checkpoint; the level is only about 12 seconds long, so restarting from the start is fair. Charger also got a Return state, so after a dash it walks back home instead of staying on the path.
2026-11-10  The level had no wall between the Shooter and the hero, so "walls block the sight ray" could not be shown. Added a small hanging stone on the Walls layer: it hides the hero on bridge 2 but not at the gap edge, so the Shooter still threatens a player who leaves the lantern on the left. Added a green/red Debug line for the sight ray.
2026-11-10  On a 4:3 tablet the camera cut off the start, the door and the Shooter (and an off-screen Shooter never attacks). Added CameraFit: the camera zooms out until the whole level width fits.
2026-11-10  Visuals: replaced the flat light circle with a real URP Point Light 2D on the lantern. The lantern script sets the light radius from lightRadius, so the visible light and the "lit" rule can never disagree. Hero and enemies use an unlit material so they stay readable in the dark.
2026-11-10  First generated art: the stone slab came out 2:1, our blocks were 7:1, so stretching looked bad. Made the blocks thicker (2 x 0.8) with the same top edge, so the shadow bridges stay where they were. FloatingBlock now takes its corners from the sprite bounds, so the shadow always matches the picture.
