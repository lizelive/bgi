import { describe, it, expect } from 'vitest'
import { makeInitialState, saveState, loadState } from '../src/game/state'
import { runAISimulation } from '../src/game/ai'

function approx(n: number, min: number, max: number) {
  expect(n).toBeGreaterThanOrEqual(min)
  expect(n).toBeLessThanOrEqual(max)
}

describe('AI e2e simulation', () => {
  it('runs 300s, buys buildings, and keeps bars sane', () => {
    let s = makeInitialState()

    // run 5 minutes of AI
    s = runAISimulation(s, 300)

    // sanity checks
    expect(Object.values(s.bars).every(v => v >= 0 && v <= 100)).toBe(true)
    // should have purchased at least one building
    const totalBuildings = Object.values(s.buildings).reduce((a, b) => a + (b || 0), 0)
    expect(totalBuildings).toBeGreaterThan(0)

    // followers and influence should grow
    approx(s.resources.followers, 5, 1000)
    approx(s.resources.influence, 0, 10000)

    // saving and loading works
    saveState(s)
    const loaded = loadState()
    expect(loaded).not.toBeNull()
    expect(loaded?.tick).toBeGreaterThan(0)
    expect(loaded?.resources.materials).toBeCloseTo(s.resources.materials)
  })

  it('keeps core bars from crashing after a longer run', () => {
    let s = makeInitialState()
    s = runAISimulation(s, 1200) // 20 minutes
    // key stability bars should remain within range
    approx(s.bars.security, 20, 100)
    approx(s.bars.truth, 10, 100)
    approx(s.bars.faith, 10, 100)
  })
})
