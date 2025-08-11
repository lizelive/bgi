import { Bars, BuildingType, GameState, Follower, FollowerNeeds } from './types'
import { HEROES } from './state'

export const BUILDINGS: BuildingType[] = [
  {
    id: 'altar',
    name: 'Blood Altar',
    description: 'Ritual fears sharpen obedience. +Fear, -Security',
    cost: 20,
    effects: { fear: +0.25, security: -0.1 },
    color: '#ef4444',
  tags: ['ritual','terror'],
  upkeep: { influencePerSec: 0.1 },
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
  tags: ['charisma','media'],
  upkeep: { influencePerSec: 0.05 },
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
  tags: ['charisma','temptation','media'],
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
  tags: ['order'],
  upkeep: { materialsPerSec: 0.02 },
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
  tags: ['ritual','mystic'],
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
  tags: ['technocracy','industry'],
  upkeep: { materialsPerSec: 0.04, influencePerSec: 0.02 },
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
  tags: ['charisma','ritual'],
  upkeep: { influencePerSec: 0.08 },
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
  tags: ['technocracy','order'],
  upkeep: { materialsPerSec: 0.03 },
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
    // upkeep costs
    if (b.upkeep) {
      s.resources.materials -= (b.upkeep.materialsPerSec || 0) * count * dtSeconds
      s.resources.influence -= (b.upkeep.influencePerSec || 0) * count * dtSeconds
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

  // doctrine archetype global modifiers
  switch (s.doctrine.archetype) {
    case 'charisma':
      // media/ritual buildings +10% effect; upkeep -5%
      applyTagMultiplier(s, ['media','ritual'], 1.1, 0.95, dtSeconds)
      s.bars.status += 0.05 * dtSeconds
      break
    case 'terror':
      // order/terror buildings +12% effect; faith decays slightly; mishaps reduced by Security weighting elsewhere
      applyTagMultiplier(s, ['order','terror'], 1.12, 1.0, dtSeconds)
      s.bars.faith -= 0.04 * dtSeconds
      break
    case 'technocracy':
      // industry/technocracy +15% effect; identity decays
      applyTagMultiplier(s, ['industry','technocracy'], 1.15, 1.05, dtSeconds)
      s.bars.identity -= 0.03 * dtSeconds
      break
    case 'mystic':
      // ritual/mystic +14% effect; truth decays a hair
      applyTagMultiplier(s, ['ritual','mystic'], 1.14, 1.0, dtSeconds)
      s.bars.truth -= 0.02 * dtSeconds
      break
  }

  // operations cooldowns tick down
  if (s.ops) {
    s.ops.scavengeCD = Math.max(0, (s.ops.scavengeCD || 0) - dtSeconds)
    s.ops.tradeCD = Math.max(0, (s.ops.tradeCD || 0) - dtSeconds)
  s.ops.recruitCD = Math.max(0, (s.ops.recruitCD || 0) - dtSeconds)
  }

  // doctrine points accrual with cooldown to prevent willy-nilly changes
  s.doctrine.cooldown = Math.max(0, s.doctrine.cooldown - dtSeconds)
  if (s.doctrine.cooldown <= 0) {
    // earn a point slowly based on Truth and Identity cohesion
    const earnRate = Math.max(0, (s.bars.truth + s.bars.identity) / 2000) // ~0.1 pt every 10s at high cohesion
    s.doctrine.points += earnRate * dtSeconds
    // clamp fractional accumulation to one decimal and store
    s.doctrine.points = Math.min(10, Number(s.doctrine.points.toFixed(1)))
  }

  // follower growth from PR pressure and faith
  const growth = Math.max(0, (s.bars.faith + s.bars.ecstasy + s.resources.influence * 0.02 - s.bars.fear * 0.5) / 500)
  s.resources.followers += growth * dtSeconds

  // update individual followers (deterministic, no randomness)
  const needsTarget = (bars: Bars): FollowerNeeds => ({
    physiological: clamp(0.6 * bars.security + 0.4 * bars.gain),
    safety: clamp(0.8 * bars.security + 0.2 * (100 - bars.fear)),
    love: clamp(0.5 * bars.faith + 0.3 * bars.ecstasy + 0.2 * bars.status),
    esteem: clamp(0.6 * bars.status + 0.2 * bars.truth + 0.2 * (100 - bars.identity)),
    self: clamp(0.4 * bars.truth + 0.2 * bars.faith + 0.4 * (100 - bars.identity)),
  })

  const relax = 0.6 // per-second proportional relaxation toward target
  const targets = needsTarget(s.bars)
  if (!s.followersList) s.followersList = []
  for (const f of s.followersList) {
    // evolve needs toward targets
    for (const k of Object.keys(f.needs) as (keyof FollowerNeeds)[]) {
      const cur = f.needs[k]
      const tgt = (targets as any)[k] as number
      f.needs[k] = clamp(cur + (tgt - cur) * relax * dtSeconds)
    }
    // recompute loyalty and focus
    const avg = (f.needs.physiological + f.needs.safety + f.needs.love + f.needs.esteem + f.needs.self) / 5
    f.loyalty = clamp(avg)
    f.focus = (Object.entries(f.needs).sort((a, b) => a[1] - b[1])[0][0] as keyof FollowerNeeds)
    // simple intent
    f.intent = f.loyalty < 40 && s.bars.fear > 55 ? 'resist' : (f.loyalty < 65 ? 'seek' : 'idle')
  }

  // create new individual followers if population grew past integer thresholds
  const want = Math.max(0, Math.floor(s.resources.followers) - (s.followersList?.length || 0))
  if (want > 0) {
    const nextId = s.nextFollowerId || 1
    for (let i = 0; i < want; i++) {
      const id = nextId + i
      const needs: FollowerNeeds = { ...targets }
      const loyalty = clamp((needs.physiological + needs.safety + needs.love + needs.esteem + needs.self) / 5)
      const focus = (Object.entries(needs).sort((a, b) => a[1] - b[1])[0][0] as keyof FollowerNeeds)
      const f: Follower = { id, name: `Follower ${id}`, needs, loyalty, focus, intent: 'idle' }
      s.followersList!.push(f)
    }
    s.nextFollowerId = nextId + want
  }

  // Heat system: rises with Influence, Fear, and specific buildings; Security damps it
  if (s.heat === undefined) s.heat = 5
  if (!s.threat) s.threat = { nextAttackIn: 90, attackCount: 0, lastHeroId: null }
  // tag-weighted building heat
  let buildingHeat = 0
  for (const b of BUILDINGS) {
    const count = s.buildings[b.id] || 0
    if (count <= 0) continue
    const tags = b.tags || []
    let per = 0.01 // base per-building heat
    if (tags.includes('terror')) per += 0.03
    if (tags.includes('ritual')) per += 0.02
    if (tags.includes('media')) per += 0.01
    if (tags.includes('industry') || tags.includes('technocracy')) per += 0.01
    if (tags.includes('order')) per -= 0.01
    buildingHeat += per * count
  }
  const fearHeat = s.bars.fear * 0.002
  const influenceHeat = s.resources.influence * 0.001
  const securityDamp = s.bars.security * 0.001 // softer damping
  const heatDelta = (influenceHeat + buildingHeat + fearHeat - securityDamp) * dtSeconds
  s.heat = clamp(Math.max(0, Math.min(100, (s.heat || 0) + heatDelta)))

  // Threat timer ticks down faster with heat
  const speedMul = 1 + (s.heat / 100) * 1.5 // up to 2.5x faster
  s.threat.nextAttackIn = Math.max(0, s.threat.nextAttackIn - dtSeconds * speedMul)

  // On attack trigger, pick hero deterministically by tier from heat and apply effects
  if (s.threat.nextAttackIn <= 0) {
    const tier = s.heat > 66 ? 3 : s.heat > 33 ? 2 : 1
    const pool = HEROES.filter(h => h.tier === tier)
    const idx = s.threat.attackCount % pool.length
    const hero = pool[idx]
    const e = hero.effects
    if (e.materials) s.resources.materials = Math.max(0, s.resources.materials + e.materials)
    if (e.influence) s.resources.influence = Math.max(0, s.resources.influence + e.influence)
    if (e.security) s.bars.security = clamp(s.bars.security + e.security)
    if (e.fear) s.bars.fear = clamp(s.bars.fear + e.fear)
    if (e.status) s.bars.status = clamp(s.bars.status + e.status)
    if ((e as any).truth) s.bars.truth = clamp(s.bars.truth + (e as any).truth)
    s.threat.attackCount += 1
  s.threat.lastHeroId = hero.id
  s.threat.lastHeroName = hero.name
  s.threat.lastEffects = { ...e }
  s.threat.flashSec = 3
    // reset timer based on heat; higher heat shorter interval
    const base = 90
    const reduce = (s.heat / 100) * 50 // up to 50s shorter
    s.threat.nextAttackIn = base - reduce
    s.log = [`Hero attack: ${hero.name} hit your ops.`, ...s.log].slice(0, 40)
  }
  // tick down UI flash; clear lastEffects after it ends
  if (s.threat.flashSec && s.threat.flashSec > 0) {
    s.threat.flashSec = Math.max(0, s.threat.flashSec - dtSeconds)
    if (s.threat.flashSec === 0) {
      s.threat.lastEffects = null
      s.threat.lastHeroName = null
    }
  }

  s.bars = clampBars(s.bars)
  return s
}

function applyTagMultiplier(s: GameState, tags: string[], effectMul: number, upkeepMul: number, dt: number) {
  for (const b of BUILDINGS) {
    const count = s.buildings[b.id] || 0
    if (count <= 0) continue
    if (!b.tags || !b.tags.some(t => tags.includes(t))) continue
    for (const key of Object.keys(b.effects) as (keyof Bars)[]) {
      const base = b.effects[key] || 0
      if (base !== 0) s.bars[key] += (base * (effectMul - 1)) * count * dt
    }
    if (b.upkeep) {
      s.resources.materials -= ((b.upkeep.materialsPerSec || 0) * (upkeepMul - 1)) * count * dt
      s.resources.influence -= ((b.upkeep.influencePerSec || 0) * (upkeepMul - 1)) * count * dt
    }
  }
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
