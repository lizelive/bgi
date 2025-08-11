import { BUILDINGS, computeTick } from './systems'
import { GameState } from './types'

export type Action =
  | { type: 'tick'; seconds: number }
  | { type: 'buy'; buildingId: string }
  | { type: 'setSpeed'; speed: 1 | 2 | 4 }
  | { type: 'setDoctrine'; archetype: GameState['doctrine']['archetype'] }

export function canAfford(state: GameState, buildingId: string): boolean {
  const b = BUILDINGS.find(x => x.id === buildingId)
  if (!b) return false
  return state.resources.materials >= b.cost
}

export function applyAction(state: GameState, action: Action): GameState {
  switch (action.type) {
    case 'tick': {
      const steps = Math.max(1, Math.floor(action.seconds))
      let s = state
      for (let i = 0; i < steps; i++) s = computeTick(s, 1)
      return s
    }
    case 'buy': {
      const b = BUILDINGS.find(x => x.id === action.buildingId)
      if (!b) return state
      if (state.resources.materials < b.cost) return state
      return {
        ...state,
        resources: { ...state.resources, materials: state.resources.materials - b.cost },
        buildings: { ...state.buildings, [b.id]: (state.buildings[b.id] || 0) + 1 },
        log: [...state.log, `Built ${b.name}`],
      }
    }
    case 'setSpeed':
      return { ...state, speed: action.speed }
    case 'setDoctrine':
      return { ...state, doctrine: { ...state.doctrine, archetype: action.archetype, cooldown: 30 } }
    default:
      return state
  }
}

export function clone<T>(x: T): T {
  return JSON.parse(JSON.stringify(x))
}
