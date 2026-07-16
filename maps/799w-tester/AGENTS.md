# Agent Instructions

This is a Warcraft III custom map project for the `799W-tester` map.

Rules for future Codex work:

- Treat `map/799W-tester.w3x` as the packed source backup unless the user asks to replace it.
- Treat `extracted/799W-tester/files/` as readable reference data from the current map.
- Do not edit packed `.w3x` files directly unless the user explicitly asks for map repacking.
- Keep Lua systems modular and readable.
- Store unit, hero, item, tavern, and ability rawcodes in `src/data/`.
- Store reusable gameplay logic in `src/systems/`.
- Store helper functions in `src/util/`.
- Prefer clear data tables over hardcoded rawcodes inside systems.
- Document trigger migrations in `docs/trigger_notes.md`.
- When changing gameplay logic, update `docs/changelog.md` and `docs/known_bugs.md` when relevant.
- World Editor and in-game testing are still required before calling a gameplay change finished.
