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

// Individual follower modeling using Maslow-like needs
export type FollowerNeeds = {
  physiological: number
  safety: number
  love: number
  esteem: number
  self: number // self-actualization
}

export type Follower = {
  id: number
  name: string
  needs: FollowerNeeds
  loyalty: number // 0..100 average of needs
  focus: keyof FollowerNeeds // current most unmet need
  intent: 'idle' | 'seek' | 'resist'
}

export type Hero = {
  id: string
  name: string
  description: string
  tier: 1 | 2 | 3
  // Deterministic attack effects applied on hit
  effects: {
    materials?: number // negative numbers reduce
    influence?: number
    security?: number
    fear?: number
    status?: number
  truth?: number
  }
}

export type ThreatState = {
  nextAttackIn: number // seconds until next hero attack
  attackCount: number
  lastHeroId?: string | null
  lastHeroName?: string | null
  lastEffects?: {
    materials?: number
    influence?: number
    security?: number
    fear?: number
    status?: number
    truth?: number
  } | null
  flashSec?: number // UI cue duration
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
  followersList?: Follower[]
  nextFollowerId?: number
  heat?: number // 0..100
  threat?: ThreatState
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
