import React, { useEffect, useMemo, useRef, useState } from 'react'
import Panel from './components/Panel'
import ProgressBar from './components/ProgressBar'
import { BUILDINGS, computeTick } from './game/systems'
import { GameState } from './game/types'
import { loadState, makeInitialState, saveState } from './game/state'

function fmt(n: number, digits = 0) {
  const v = n < 1000 ? n : Math.round(n)
  return v.toFixed(digits)
}

export default function App() {
  const [state, setState] = useState<GameState>(() => loadState() || makeInitialState())
  const [last, setLast] = useState<number>(() => performance.now())
  const raf = useRef(0)

  // main loop
  useEffect(() => {
    function frame(now: number) {
      const dt = Math.min(0.25, (now - last) / 1000) // cap delta
      setLast(now)
      setState(s => s.running ? computeTick(s, dt * s.speed) : s)
      raf.current = requestAnimationFrame(frame)
    }
    raf.current = requestAnimationFrame(frame)
    return () => cancelAnimationFrame(raf.current)
  }, [last])

  // autosave
  useEffect(() => {
    const id = setInterval(() => saveState(state), 2000)
    return () => clearInterval(id)
  }, [state])

  const loyalty = useMemo(() => (state.bars.faith + state.bars.ecstasy + state.bars.security + state.bars.truth + state.bars.identity + state.bars.gain - state.bars.fear) / 5.0, [state.bars])

  function addBuilding(id: string) {
    const def = BUILDINGS.find(b => b.id === id)!
    if (state.resources.materials < def.cost) return pushLog(`Not enough materials for ${def.name}.`)
    setState(s => ({
      ...s,
      resources: { ...s.resources, materials: s.resources.materials - def.cost },
      buildings: { ...s.buildings, [id]: (s.buildings[id] || 0) + 1 },
      log: [`Built: ${def.name}.`, ...s.log].slice(0, 40),
    }))
  }

  function removeBuilding(id: string) {
    const count = state.buildings[id] || 0
    if (count <= 0) return
    setState(s => ({
      ...s,
      buildings: { ...s.buildings, [id]: count - 1 },
      log: [`Dismantled: ${BUILDINGS.find(b => b.id === id)?.name}.`, ...s.log].slice(0, 40),
    }))
  }

  function pushLog(msg: string) {
    setState(s => ({ ...s, log: [msg, ...s.log].slice(0, 40) }))
  }

  return (
    <div className="app">
      <header className="panel">
        <div className="title">Bad Guy Inc — Villain Colony (Prototype)</div>
        <div className="row" style={{ gap: 12 }}>
          <span className="badge">Speed</span>
          {([1,2,4] as const).map(sp => (
            <button key={sp} className={`button ${state.speed===sp ? 'primary' : ''}`} onClick={() => setState(s => ({...s, speed: sp}))}>{sp}x</button>
          ))}
          <button className="button" onClick={() => setState(s => ({...s, running: !s.running}))}>
            {state.running ? 'Pause' : 'Resume'}
          </button>
        </div>
      </header>

      <div className="left">
        <Panel title="Resources">
          <div className="col">
            <div className="row" style={{ justifyContent: 'space-between' }}>
              <span>Influence</span>
              <span className="badge">{fmt(state.resources.influence)}</span>
            </div>
            <div className="row" style={{ justifyContent: 'space-between' }}>
              <span>Materials</span>
              <span className="badge">{fmt(state.resources.materials)}</span>
            </div>
            <div className="row" style={{ justifyContent: 'space-between' }}>
              <span>Followers</span>
              <span className="badge">{fmt(state.resources.followers, 1)}</span>
            </div>
            <div className="row" style={{ justifyContent: 'space-between' }}>
              <span>Loyalty Index</span>
              <span className="badge">{fmt(loyalty, 0)}%</span>
            </div>
          </div>
        </Panel>

        <Panel title="Follower Bars">
          <div className="col" style={{ gap: 10 }}>
            <ProgressBar label="Faith" value={state.bars.faith} color="#a78bfa" />
            <ProgressBar label="Fear" value={state.bars.fear} color="#ef4444" />
            <ProgressBar label="Ecstasy" value={state.bars.ecstasy} color="#22c55e" />
            <ProgressBar label="Security" value={state.bars.security} color="#60a5fa" />
            <ProgressBar label="Status" value={state.bars.status} color="#f472b6" />
            <ProgressBar label="Truth" value={state.bars.truth} color="#f59e0b" />
            <ProgressBar label="Identity Dissolution" value={state.bars.identity} color="#94a3b8" />
            <ProgressBar label="Personal Gain" value={state.bars.gain} color="#34d399" />
          </div>
        </Panel>
      </div>

      <main className="main">
        <Panel title="Facilities">
          <div className="grid">
            {BUILDINGS.map(b => {
              const count = state.buildings[b.id] || 0
              return (
                <div className="listItem" key={b.id}>
                  <div className="col" style={{ gap: 4 }}>
                    <div className="row" style={{ gap: 8 }}>
                      <div className="badge" style={{ background: '#0f1324', borderColor: '#222a44', color: b.color }}>{b.name}</div>
                      <span className="muted small">x{count}</span>
                    </div>
                    <span className="muted small">{b.description}</span>
                    <span className="small">Cost: {b.cost} materials</span>
                  </div>
                  <div className="col" style={{ gap: 6, minWidth: 120 }}>
                    <button className="button primary" onClick={() => addBuilding(b.id)}>Build</button>
                    <button className="button" onClick={() => removeBuilding(b.id)} disabled={count===0}>Dismantle</button>
                  </div>
                </div>
              )
            })}
          </div>
        </Panel>
      </main>

      <aside className="right">
        <Panel title="Doctrine & Traits">
          <div className="col">
            <Slider label="Rigidity" value={state.traits.rigidity} min={0} max={100} step={1}
              onChange={v => setState(s => ({ ...s, traits: { ...s.traits, rigidity: v }}))} />
            <Slider label="Charisma" value={state.traits.charisma} min={0} max={5} step={1}
              onChange={v => setState(s => ({ ...s, traits: { ...s.traits, charisma: v }}))} />
            <Slider label="Terror" value={state.traits.terror} min={0} max={5} step={1}
              onChange={v => setState(s => ({ ...s, traits: { ...s.traits, terror: v }}))} />
            <Slider label="Temptation" value={state.traits.temptation} min={0} max={5} step={1}
              onChange={v => setState(s => ({ ...s, traits: { ...s.traits, temptation: v }}))} />
            <Slider label="Mysticism" value={state.traits.mysticism} min={0} max={5} step={1}
              onChange={v => setState(s => ({ ...s, traits: { ...s.traits, mysticism: v }}))} />
            <Slider label="Technocracy" value={state.traits.technocracy} min={0} max={5} step={1}
              onChange={v => setState(s => ({ ...s, traits: { ...s.traits, technocracy: v }}))} />
          </div>
        </Panel>

        <Panel title="Event Log">
          <div className="col" style={{ maxHeight: 280, overflow: 'auto' }}>
            {state.log.map((l, i) => (
              <span key={i} className="small">• {l}</span>
            ))}
          </div>
        </Panel>
      </aside>

      <footer>
        <span className="muted small">Tip: Build facilities to shape bars. Adjust traits to steer your doctrine.</span>
        <div className="row" style={{ gap: 8 }}>
          <button className="button" onClick={() => { saveState(state); }}>Save</button>
          <button className="button" onClick={() => { localStorage.clear(); location.reload(); }}>Reset</button>
        </div>
      </footer>
    </div>
  )
}

function Slider({ label, value, min, max, step, onChange }: { label: string; value: number; min: number; max: number; step?: number; onChange: (v: number) => void }) {
  return (
    <div className="sliderRow">
      <span className="small" style={{ width: 96 }}>{label}</span>
      <input type="range" min={min} max={max} step={step ?? 1} value={value} onChange={e => onChange(Number(e.target.value))} />
      <span className="badge" style={{ minWidth: 36, textAlign: 'center' }}>{value}</span>
    </div>
  )
}
