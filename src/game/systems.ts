import { Bars, BuildingType, GameState } from './types'

export const BUILDINGS: BuildingType[] = [
  {
    id: 'altar',
    name: 'Blood Altar',
    description: 'Ritual fears sharpen obedience. +Fear, -Security',
    cost: 20,
    effects: { fear: +0.25, security: -0.1 },
    color: '#ef4444',
    pros: ['Boosts Fear quickly to prevent drift', 'Synergizes with Terror trait'],
    cons: ['Hurts Security leading to mishaps', 'High Fear can cause revolt spikes'],
  },
  {
    id: 'studio',
    name: 'Propaganda Studio',
    description: 'Broadcast the Vision. +Faith, +Truth, +Influence',
    cost: 25,
    effects: { faith: +0.2, truth: +0.15 },
    color: '#9b87f5',
    pros: ['Raises Faith and Truth steadily', 'Improves Influence income via loyalty'],
    cons: ['Medium cost', 'Less effective at high Rigidity'],
  },
  {
    id: 'meme',
    name: 'Meme Lab',
    description: 'Ecstasy engines hum. +Ecstasy, -Truth',
    cost: 18,
    effects: { ecstasy: +0.3, truth: -0.1 },
    color: '#22c55e',
    pros: ['Fast Ecstasy gain boosts recruitment', 'Synergizes with Temptation trait'],
    cons: ['Erodes Truth risking cognitive dissonance', 'Can reduce long-term stability'],
  },
  {
    id: 'security',
    name: 'Security Post',
    description: 'Visible safety raises order. +Security, -Status',
    cost: 22,
    effects: { security: +0.25, status: -0.05 },
    color: '#60a5fa',
    pros: ['Reduces mishaps and stabilizes ops', 'Improves materials trickle'],
    cons: ['Hurts Status (followers feel less special)', 'Costs more materials'],
  },
  {
    id: 'meditation',
    name: 'Meditation Chamber',
    description: 'Quiet conviction. +Faith, -Ecstasy',
    cost: 16,
    effects: { faith: +0.18, ecstasy: -0.08 },
    color: '#eab308',
    pros: ['Reliable Faith without Fear', 'Pairs well with Rigidity builds'],
    cons: ['Dampens Ecstasy reducing fast growth', 'Low raw output'],
  },
  {
    id: 'forge',
    name: 'Fabrication Forge',
    description: 'Automates production. +Materials, -Ecstasy, -Truth',
    cost: 40,
    effects: { ecstasy: -0.15, truth: -0.05 },
    color: '#94a3b8',
    pros: ['Unlocks periodic material surges', 'Strong with Technocracy'],
    cons: ['Morale hit and narrative erosion', 'High upfront cost'],
  },
  {
    id: 'hall',
    name: 'Grand Hall',
    description: 'Ceremony and spectacle. +Status, +Faith, -Materials upkeep',
    cost: 35,
    effects: { status: +0.3, faith: +0.12 },
    color: '#f472b6',
    pros: ['Elevates ranks to reduce churn', 'Boosts Influence via Status'],
    cons: ['Expensive build; slow ROI', 'Ties resources to prestige'],
  },
  {
    id: 'labs',
    name: 'Augment Labs',
    description: 'Implants and data rites. +Truth, +Security, -Identity',
    cost: 38,
    effects: { truth: +0.25, security: +0.18, identity: -0.12 },
    color: '#10b981',
    pros: ['Solidifies order and accuracy', 'Boosts trickle and operations safety'],
    cons: ['Identity loss can cause schisms', 'High tech maintenance (narrative cost)'],
  },
]

export function clampBars(b: Bars): Bars {
  return {
    faith: clamp(b.faith),
    fear: clamp(b.fear),
    ecstasy: clamp(b.ecstasy),
    security: clamp(b.security),
    status: clamp(b.status),
    truth: clamp(b.truth),
    identity: clamp(b.identity),
    gain: clamp(b.gain),
  }
}

function clamp(n: number) {
  return Math.max(0, Math.min(100, n))
}

export function computeTick(state: GameState, dtSeconds: number): GameState {
  const s: GameState = JSON.parse(JSON.stringify(state))
  s.tick += 1

  // base passive decay towards 50 (homeostasis)
  const homeo = 0.02 * dtSeconds
  for (const k of Object.keys(s.bars) as (keyof Bars)[]) {
    const v = s.bars[k]
    s.bars[k] = v + (50 - v) * homeo
  }

  // buildings apply per-tick modifiers
  for (const b of BUILDINGS) {
    const count = s.buildings[b.id] || 0
    if (count <= 0) continue
    for (const key of Object.keys(b.effects) as (keyof Bars)[]) {
      s.bars[key] += (b.effects[key] || 0) * count * dtSeconds
    }
  }

  // traits modifiers
  const t = s.traits
  s.bars.faith += (t.charisma * 0.06 + t.mysticism * 0.04) * dtSeconds
  s.bars.fear += t.terror * 0.08 * dtSeconds
  s.bars.truth += (t.technocracy * 0.08 - t.mysticism * 0.05) * dtSeconds
  s.bars.ecstasy += (t.temptation * 0.08 - t.rigidity * 0.008) * dtSeconds
  s.bars.identity += (t.rigidity * 0.02 + t.mysticism * 0.05) * dtSeconds
  s.bars.status += (t.charisma * 0.05 - t.terror * 0.03) * dtSeconds
  s.bars.security += (t.technocracy * 0.06 - t.terror * 0.02) * dtSeconds

  // influence income driven by loyalty composite
  const loyalty = weightedAverage({
    faith: 1.2,
    fear: 0.6,
    ecstasy: 1.0,
    security: 0.8,
    status: 0.6,
    truth: 0.9,
    identity: 1.1,
    gain: 1.0,
  }, s.bars)
  const followers = s.resources.followers
  const influenceGain = (followers * (loyalty / 100)) * (1 + t.charisma * 0.05) * dtSeconds
  s.resources.influence += influenceGain

  // materials trickle from technocracy and security
  s.resources.materials += (0.05 + t.technocracy * 0.1 + s.bars.security * 0.002) * dtSeconds

  // operations cooldowns tick down
  if (s.ops) {
    s.ops.scavengeCD = Math.max(0, (s.ops.scavengeCD || 0) - dtSeconds)
    s.ops.tradeCD = Math.max(0, (s.ops.tradeCD || 0) - dtSeconds)
  }

  // follower growth from PR pressure and faith
  const growth = Math.max(0, (s.bars.faith + s.bars.ecstasy + s.resources.influence * 0.02 - s.bars.fear * 0.5) / 500)
  s.resources.followers += growth * dtSeconds

  s.bars = clampBars(s.bars)
  return s
}

function weightedAverage(weights: Record<keyof Bars, number>, bars: Bars): number {
  let sumW = 0, sum = 0
  for (const k of Object.keys(weights) as (keyof Bars)[]) {
    const w = weights[k]
    sumW += w
    sum += bars[k] * w
  }
  return sum / sumW
}
