# Shadow & Light (Тень и свет)

2D Unity 6000.3.23f1 puzzle for Android, landscape. Universal 2D (URP), new Input System.
University project (Narxoz, Game Development Fundamentals). The student must be able to explain every line of code at a defense, so keep code simple and readable.
Full context: docs/CONTEXT.md. Read it at the start of a session.

## Rules

- Unity 6 API: Rigidbody2D.linearVelocity, FindFirstObjectByType. No obsolete API.
- No Find\* and no GetComponent in Update. Cache references in Awake/Start.
- All tuning numbers (speeds, times, distances, damage, cooldowns) are [SerializeField] fields, no magic numbers.
- Enemy AI: enum + switch FSM with a SetState() method that resets the state timer. At least 3 states, every state has an exit.
- Sight checks: distance, then angle, then Physics2D.Raycast with a LayerMask.
- Every attack has a wind-up (0.3-0.8 s) and a cooldown. Enemies never attack while off screen.
- Layers: Walls, Shadow, Hero, Enemy. Use LayerMask fields, never hard-coded layer numbers.
- Do not write comments inside code.
- All UI text in English.
- Folders: Assets/\_Project/Scripts, Scenes, Prefabs, Sprites.
- Never edit .unity, .prefab or .meta files as text. Change scenes and prefabs only through Unity MCP tools, and list every scene change you made.
- Do not run git commands. The student commits himself.
- Keep scripts small, one responsibility each.
- After every task: explain each script line by line in simple words, list scene changes, and say what could go wrong.
- Change only what the task asks for.
