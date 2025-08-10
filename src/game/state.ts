import { GameState, Bars, LeaderTraits } from './types'

export const initialBars: Bars = {
  faith: 50,
  fear: 20,
  ecstasy: 30,
  security: 40,
  status: 25,
  truth: 50,
  identity: 10,
  gain: 20,
}

export const initialTraits: LeaderTraits = {
  charisma: 2,
  terror: 1,
  rigidity: 40,
  temptation: 1,
  mysticism: 2,
  technocracy: 1,
}

export function makeInitialState(): GameState {
  return {
    tick: 0,
    running: true,
    speed: 1,
    bars: { ...initialBars },
    traits: { ...initialTraits },
    resources: { influence: 10, materials: 50, followers: 5 },
    buildings: {},
    log: [
      'A whisper becomes a rumor. A rumor becomes a movement.',
      'You are the Brand. The world is the market.',
    ],
  }
}

const KEY = 'bgi-save-v1'

export function loadState(): GameState | null {
  try {
    const raw = localStorage.getItem(KEY)
    if (!raw) return null
    const parsed = JSON.parse(raw) as GameState
    if (!parsed || typeof parsed !== 'object' || !parsed.bars) return null
    return parsed
  } catch {
    return null
  }
}

export function saveState(state: GameState) {
  try {
    localStorage.setItem(KEY, JSON.stringify(state))
  } catch {}
}
