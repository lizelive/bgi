import { GameState, Bars, LeaderTraits, Follower, FollowerNeeds, Hero } from './types'

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
  const followersList: Follower[] = []
  const needsFromBars = (bars: Bars): FollowerNeeds => ({
    physiological: clamp(0.6 * bars.security + 0.4 * bars.gain),
    safety: clamp(0.8 * bars.security + 0.2 * (100 - bars.fear)),
    love: clamp(0.5 * bars.faith + 0.3 * bars.ecstasy + 0.2 * bars.status),
    esteem: clamp(0.6 * bars.status + 0.2 * bars.truth + 0.2 * (100 - bars.identity)),
    self: clamp(0.4 * bars.truth + 0.2 * bars.faith + 0.4 * (100 - bars.identity)),
  })
  const bars = { ...initialBars }
  for (let i = 1; i <= initialResources.followers; i++) {
    const needs = needsFromBars(bars)
    const loyalty = Math.round((needs.physiological + needs.safety + needs.love + needs.esteem + needs.self) / 5)
    const focus: keyof FollowerNeeds = (Object.entries(needs).sort((a, b) => a[1] - b[1])[0][0] as keyof FollowerNeeds)
    followersList.push({ id: i, name: `Follower ${i}`, needs, loyalty, focus, intent: 'idle' })
  }
  return {
    tick: 0,
    running: true,
    speed: 1,
    bars,
    traits: { ...initialTraits },
    resources: { ...initialResources },
    buildings: {},
  followersList,
  nextFollowerId: initialResources.followers + 1,
  heat: 5,
  threat: { nextAttackIn: 90, attackCount: 0, lastHeroId: null },
  ops: { scavengeCD: 0, tradeCD: 0 },
  doctrine: { archetype: null, points: 2, cooldown: 0 },
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

// local helpers
function clamp(n: number) { return Math.max(0, Math.min(100, n)) }

const initialResources = { influence: 10, materials: 50, followers: 5 }

// Optional: exported roster if needed elsewhere
export const HEROES: Hero[] = [
  { id: 'bat', name: 'Batman', tier: 1, description: 'Brooding vigilante disrupts operations.', effects: { materials: -10, security: -2, fear: +3 } },
  { id: 'ram', name: 'Rambo', tier: 2, description: 'Explosive raid damages facilities.', effects: { materials: -20, security: -4, fear: +4, status: -2 } },
  { id: 'gla', name: 'GLaDOS', tier: 3, description: 'Cold logic unravels your narrative.', effects: { influence: -15, truth: -4, status: -4, fear: +2 } },
]
