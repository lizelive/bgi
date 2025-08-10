export type Bars = {
  faith: number
  fear: number
  ecstasy: number
  security: number
  status: number
  truth: number
  identity: number
  gain: number
}

export type LeaderTraits = {
  charisma: number // 0-5
  terror: number   // 0-5
  rigidity: number // 0-100
  temptation: number // 0-5
  mysticism: number // 0-5
  technocracy: number // 0-5
}

export type Resources = {
  influence: number
  materials: number
  followers: number
}

export type BuildingType = {
  id: string
  name: string
  description: string
  cost: number
  effects: Partial<Bars> // per-tick percentage deltas
  color?: string
  pros?: string[]
  cons?: string[]
  tags?: string[]
  upkeep?: { materialsPerSec?: number; influencePerSec?: number }
}

export type GameState = {
  tick: number
  running: boolean
  speed: 1 | 2 | 4
  bars: Bars
  traits: LeaderTraits
  resources: Resources
  buildings: Record<string, number>
  ops?: {
    scavengeCD: number
    tradeCD: number
  }
  doctrine: Doctrine
  log: string[]
}

export type Archetype = 'charisma' | 'terror' | 'technocracy' | 'mystic'

export type Doctrine = {
  archetype: Archetype | null
  points: number
  cooldown: number // seconds until you can earn/spend again
}
