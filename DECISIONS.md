# Decisions log

What I asked the agent, what it got wrong, what I changed and why. 3-5 lines per week.

## Week 10 (9-15 Nov)

2026-11-10  Planned the enemies with Claude Code. Decided "lit" means "not inside a shadow collider" (OverlapPoint on the Shadow layer), not a ray from the lantern: one cheap check, and it makes the puzzle: a shadow is a bridge for the hero but a hiding place for enemies.
2026-11-10  Asked for the shadow projection. The shadow mesh looked paler than #8D7FB0: the project uses Linear color space and mesh vertex colors are not converted. Fixed with shadowColor.linear.
2026-11-10  Hero FSM: when the shadow bridge is removed, the hero now falls straight down instead of keeping his walking speed, so the player can predict where he lands.
2026-11-10  The lecture code puts units in comments, but our rule is no comments. Units go into field names instead (respawnDelaySeconds, loadDelaySeconds).
2026-11-11  Patroller: the first rule "lit = not in a shadow" made every enemy lit almost everywhere, because the whole wall is lit. Added a light radius around the lantern (5.5 units, shown as a soft circle): an enemy is lit only near the lantern and outside shadows.
2026-11-11  Patroller wind-up 0.5 s with a red color; lighting it during the wind-up cancels the hit. This is the player's counter, because the hero is not controlled.
2026-11-11  Shooter shot is a slow projectile (5 units/s), not an instant raycast hit, so the laser and the shot are both readable. The projectile uses a Dynamic Rigidbody2D with gravity 0: a Kinematic body would not get trigger events from the static walls.
2026-11-11  Charger test showed a level bug: the lantern hangs at y = 4 and the ground is at y = -1.6, so with a light radius of 5.5 nothing on the ground could ever be lit. Light radius 5.5 -> 7 (the light now reaches about 4 units to each side at ground level).
2026-11-11  Charger dash stops at the edge of the ground (same down-ray check as the hero), so it never flies over a pit.
