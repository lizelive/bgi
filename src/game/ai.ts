import { BUILDINGS } from './systems'
import { GameState } from './types'
import { Action, applyAction, canAfford, clone } from './actions'

// Simple heuristic AI:
// - Keep security >= 45, truth >= 45, faith >= 55
// - Prefer buildings by current deficits and affordability
// - Occasionally pick doctrine based on strongest trait
export function aiStep(state: GameState): GameState {
  let s = clone(state)

  // set doctrine if none and have points
  if (!s.doctrine.archetype && s.doctrine.points >= 1 && s.doctrine.cooldown <= 0) {
    const t = s.traits
    const best = (
      t.technocracy >= t.charisma && t.technocracy >= t.mysticism && t.technocracy >= t.terror
        ? 'technocracy'
        : t.charisma >= t.mysticism && t.charisma >= t.terror
        ? 'charisma'
        : t.mysticism >= t.terror
        ? 'mystic'
        : 'terror'
    ) as GameState['doctrine']['archetype']
    s = applyAction(s, { type: 'setDoctrine', archetype: best })
  }

  // choose building to buy if materials high
  const targetOrder: { id: string; priority: number }[] = []
  for (const b of BUILDINGS) {
    let p = 0
    if (b.effects.security) p += (50 - s.bars.security) * 0.5 * Math.sign(b.effects.security)
    if (b.effects.truth) p += (50 - s.bars.truth) * 0.4 * Math.sign(b.effects.truth)
    if (b.effects.faith) p += (60 - s.bars.faith) * 0.3 * Math.sign(b.effects.faith)
    if ((s.resources.materials - (b.cost || 0)) < 5) p -= 5
    // prefer cheaper when similar
    p -= (b.cost || 0) * 0.02
    targetOrder.push({ id: b.id, priority: p })
  }
  targetOrder.sort((a, b) => b.priority - a.priority)

  for (const t of targetOrder) {
    if (canAfford(s, t.id)) {
      s = applyAction(s, { type: 'buy', buildingId: t.id })
      break
    }
  }

  // tick 1 second
  s = applyAction(s, { type: 'tick', seconds: 1 })
  return s
}

export function runAISimulation(s0: GameState, seconds: number): GameState {
  let s = clone(s0)
  for (let i = 0; i < seconds; i++) {
    s = aiStep(s)
  }
  return s
}
