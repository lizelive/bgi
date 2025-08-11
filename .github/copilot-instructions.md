# Copilot instructions for this repo

Purpose: Help AI agents contribute productively to this Vite + React + TypeScript game by following the project’s architecture, conventions, and workflows.

## Big picture
- The UI is React; the game simulation is headless, pure TypeScript in `src/game/**`.
- Core loop: `computeTick(state, dtSeconds)` in `src/game/systems.ts` updates bars/resources deterministically. No timers/randomness inside.
- State shape and invariants are in `src/game/types.ts` and `src/game/state.ts` (includes `makeInitialState`, `saveState`, `loadState`).
- Buildings and doctrine rules live in `src/game/systems.ts` (`BUILDINGS`, per-tag multipliers, clamping, weighted averages).
- Headless harness for QC/balancing exists in:
  - `src/game/actions.ts` (buy, tick, doctrine/speed changes) — pure functions only
  - `src/game/ai.ts` (simple heuristic AI + `runAISimulation`)
- Tests use Vitest + jsdom in `tests/**` (e.g. `tests/e2e.sim.test.ts`).

## Invariants and patterns
- Keep simulation deterministic and pure:
  - Do not read system time or use randomness in `computeTick`; if needed, inject PRNG or add params.
  - Always clamp bars via `clampBars` before returning a new `GameState`.
  - Treat `GameState` as immutable; return new objects (no in-place mutation outside local variables).
- Buildings:
  - Define in `BUILDINGS`: include `id`, `cost`, `effects: Partial<Bars>`, optional `tags`, `upkeep`.
  - Tags power doctrine multipliers (`applyTagMultiplier`); use existing tags like `ritual`, `media`, `order`, `terror`, `technocracy`, `industry`, `mystic`.
  - Balance via small per-second deltas; remember upkeep subtracts from `resources`.
- Doctrine:
  - Global modifiers applied each tick based on `doctrine.archetype` (charisma/terror/technocracy/mystic).
  - `doctrine.points` accrues slowly; setting archetype sets a short cooldown.
- Save/load:
  - Uses `localStorage` with key `bgi-save-v1` in `src/game/state.ts`.
  - Tests run under jsdom so `localStorage` is available.

## Developer workflows
- Run the app:
  - `npm run dev` (Vite dev server)
  - `npm run build` then `npm run preview` (prod build + preview)
- Tests (e2e simulation and future unit tests):
  - `npm test` (Vitest run)
 - Documentation hygiene:
   - When adding or changing systems, UI, commands, or gameplay loops, update `README.md` with a concise note of what changed and how to try it.
   - Keep “What’s included” and core loops accurate; add or trim bullets as features evolve.
   - If you introduce new mechanics (e.g., Heat, hero attacks, follower needs), add a short explanation and where to see them in the UI.

## How to extend
- Add a building:
  - Edit `src/game/systems.ts` `BUILDINGS` with `id`, `name`, `cost`, `effects`, `tags`, `upkeep`.
  - Consider doctrine tag multipliers; avoid large per-second effects; keep within clamped bars (0..100).
  - Add/adjust tests that cover new balance expectations.
  - Update `README.md` if the building introduces a new concept (e.g., defense, economy) or impacts core loops.
- Add a new system effect:
  - Update `computeTick` carefully; keep pure; respect `dtSeconds` and bar clamping.
  - Prefer composing small helpers (like `applyTagMultiplier`, `weightedAverage`).
  - Document the mechanic briefly in `README.md` under “What’s included” or a relevant section.
- Add AI behavior:
  - Extend `src/game/ai.ts` heuristics; operate through `applyAction` from `src/game/actions.ts`.
  - Keep one-second tick steps for determinism in tests.
  - If AI visibly changes behavior, add a one-liner in `README.md` so players know what to expect.

## Project-specific tips
- Keep UI thin; put game rules in `src/game/**`. Example components: `src/components/ProgressBar.tsx`, `src/components/Panel.tsx`.
- Resource economics: materials get a trickle from security/technocracy; influence scales with a loyalty weighted average.
- If introducing randomness/events, make it injectable (seeded) so tests remain stable.
- Avoid re-formatting large files; keep diffs focused. Add small, targeted tests with clear bounds (e.g., bars remain in [0..100], resources non-explosive growth).
 - After feature changes, do a quick docs pass: keep `README.md` aligned with current features and entry points.

## Reference files
- The docs are king. Check `docs/**` and `README.md` for relevent docs before making changes.
- Game types/state: `src/game/types.ts`, `src/game/state.ts`
- Core sim and content: `src/game/systems.ts`
- Headless harness & AI: `src/game/actions.ts`, `src/game/ai.ts`
- App entry: `src/App.tsx`, `src/main.tsx`
- Tests: `tests/e2e.sim.test.ts`
