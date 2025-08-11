import React, { useEffect, useMemo, useRef, useState } from 'react'
import Panel from './components/Panel'
import ProgressBar from './components/ProgressBar'
import { BUILDINGS, computeTick } from './game/systems'
import { GameState, Follower } from './game/types'
import { loadState, makeInitialState, saveState } from './game/state'
import { MusicEngine } from './audio/music'

function fmt(n: number, digits = 0) {
  const v = n < 1000 ? n : Math.round(n)
  return v.toFixed(digits)
}

export default function App() {
  const [state, setState] = useState<GameState>(() => loadState() || makeInitialState())
  const [last, setLast] = useState<number>(() => performance.now())
  const raf = useRef(0)
  const musicRef = useRef<MusicEngine | null>(null)
  const [audioOn, setAudioOn] = useState(false)
  const [muted, setMuted] = useState(false)
  const [volume, setVolume] = useState(0.6)
  const [selectedFollowerId, setSelectedFollowerId] = useState<number | null>(null)

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

  // music engine
  useEffect(() => {
    if (!audioOn) return
    if (!musicRef.current) musicRef.current = new MusicEngine()
    musicRef.current.start()
    musicRef.current.setMuted(muted)
    musicRef.current.setVolume(volume)
    musicRef.current.update(state)
  }, [audioOn, muted, volume, state])

  const loyalty = useMemo(() => (state.bars.faith + state.bars.ecstasy + state.bars.security + state.bars.truth + state.bars.identity + state.bars.gain - state.bars.fear) / 5.0, [state.bars])
  const followersList = state.followersList || []
  const selectedFollower: Follower | null = useMemo(() => followersList.find(f => f.id === selectedFollowerId) || null, [followersList, selectedFollowerId])

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

  function scavenge() {
    setState(s => {
      if (s.ops && s.ops.scavengeCD > 0) return s
      const followers = Math.max(1, Math.floor(s.resources.followers))
      const haul = Math.round(5 + followers * 0.8 + (s.bars.security * 0.1))
      const mishapChance = Math.max(0, 0.2 - s.bars.security * 0.001 - (s.traits.technocracy * 0.02))
      let newBars = { ...s.bars }
      let msg = `Scavenged +${haul} materials.`
      if (Math.random() < mishapChance) {
        newBars.security = Math.max(0, newBars.security - 2)
        newBars.fear = Math.min(100, newBars.fear + 1)
        msg += ' Minor mishap (−2 Security, +1 Fear).'
      }
      return {
        ...s,
        resources: { ...s.resources, materials: s.resources.materials + haul },
        bars: newBars,
        ops: { ...(s.ops || { scavengeCD: 0, tradeCD: 0 }), scavengeCD: 20 },
        log: [msg, ...s.log].slice(0, 40),
      }
    })
  }

  function trade() {
    setState(s => {
      if (s.ops && s.ops.tradeCD > 0) return s
      const spend = Math.min(s.resources.influence, 10 + s.traits.charisma * 5)
      const rate = 1.6 + s.traits.charisma * 0.2 + s.bars.status * 0.01
      const gained = Math.round(spend * rate)
      if (spend <= 0.5) return s
      return {
        ...s,
        resources: { ...s.resources, influence: s.resources.influence - spend, materials: s.resources.materials + gained },
        ops: { ...(s.ops || { scavengeCD: 0, tradeCD: 0 }), tradeCD: 12 },
        log: [`Traded ${spend.toFixed(0)} Influence for +${gained} Materials.`, ...s.log].slice(0, 40),
      }
    })
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
          <span className="badge">Audio</span>
          {!audioOn ? (
            <button className="button primary" onClick={() => setAudioOn(true)}>Enable</button>
          ) : (
            <>
              <button className="button" onClick={() => setMuted(m => !m)}>{muted ? 'Unmute' : 'Mute'}</button>
              <input type="range" min={0} max={1} step={0.01} value={volume} onChange={e => setVolume(Number(e.target.value))} />
            </>
          )}
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
              <span>Heat</span>
              <span className="badge">{fmt(state.heat ?? 0, 0)}%</span>
            </div>
            {state.threat && (
              <div className="row" style={{ justifyContent: 'space-between' }}>
                <span>Next Attack</span>
                <span className="badge">{Math.max(0, state.threat.nextAttackIn).toFixed(0)}s</span>
              </div>
            )}
            <div className="row" style={{ justifyContent: 'space-between' }}>
              <span>Loyalty Index</span>
              <span className="badge">{fmt(loyalty, 0)}%</span>
            </div>
            <div className="row" style={{ gap: 8 }}>
              <button className="button primary" onClick={scavenge} disabled={!!state.ops && state.ops.scavengeCD > 0}>
                Scavenge{state.ops && state.ops.scavengeCD > 0 ? ` (${state.ops.scavengeCD.toFixed(0)}s)` : ''}
              </button>
              <button className="button" onClick={trade} disabled={!!state.ops && state.ops.tradeCD > 0}>
                Trade{state.ops && state.ops.tradeCD > 0 ? ` (${state.ops.tradeCD.toFixed(0)}s)` : ''}
              </button>
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
                    {(b.pros && b.pros.length > 0) || (b.cons && b.cons.length > 0) ? (
                      <div className="col small" style={{ gap: 2, marginTop: 2 }}>
                        {b.pros?.map((p, i) => (
                          <span key={`pro-${i}`} style={{ color: '#22c55e' }}>+ {p}</span>
                        ))}
                        {b.cons?.map((c, i) => (
                          <span key={`con-${i}`} style={{ color: '#ef4444' }}>- {c}</span>
                        ))}
                      </div>
                    ) : null}
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
        <Panel title="Followers">
          <div className="col" style={{ gap: 8 }}>
            <div className="row" style={{ justifyContent: 'space-between' }}>
              <span className="small muted">Individuals: {followersList.length}</span>
              <button className="button" onClick={() => setSelectedFollowerId(null)}>Clear</button>
            </div>
            <div className="col" style={{ maxHeight: 160, overflow: 'auto', gap: 4 }}>
              {followersList.slice(-50).map(f => (
                <button key={f.id} className={`listItem ${selectedFollowerId===f.id ? 'selected' : ''}`} onClick={() => setSelectedFollowerId(f.id)}>
                  <div className="row" style={{ justifyContent: 'space-between', width: '100%' }}>
                    <span className="small">{f.name}</span>
                    <span className="badge" title={`Focus: ${f.focus}`}>{f.loyalty.toFixed(0)}%</span>
                  </div>
                </button>
              ))}
            </div>
            {selectedFollower && (
              <div className="col" style={{ gap: 6, marginTop: 6 }}>
                <span className="small">{selectedFollower.name}</span>
                <div className="col" style={{ gap: 6 }}>
                  <ProgressBar label="Physiological" value={selectedFollower.needs.physiological} color="#22c55e" />
                  <ProgressBar label="Safety" value={selectedFollower.needs.safety} color="#60a5fa" />
                  <ProgressBar label="Love/Belonging" value={selectedFollower.needs.love} color="#a78bfa" />
                  <ProgressBar label="Esteem" value={selectedFollower.needs.esteem} color="#f59e0b" />
                  <ProgressBar label="Self-Actualization" value={selectedFollower.needs.self} color="#f472b6" />
                </div>
                <div className="row" style={{ justifyContent: 'space-between' }}>
                  <span className="badge">Focus: {selectedFollower.focus}</span>
                  <span className="badge">Intent: {selectedFollower.intent}</span>
                </div>
              </div>
            )}
          </div>
        </Panel>
        <Panel title="Doctrine & Traits">
          {state.doctrine.archetype === null ? (
            <div className="col">
              <span className="small muted">Choose an archetype. This locks your style and grants unique synergies. You’ll earn points to tweak traits, but you can’t freely swap archetypes.</span>
              <div className="grid">
                {[
                  { key: 'charisma', name: 'Charisma', desc: 'Media and ritual flourish. Status rises.' },
                  { key: 'terror', name: 'Terror', desc: 'Order through fear. Mishaps less likely.' },
                  { key: 'technocracy', name: 'Technocracy', desc: 'Industry and truth hum. Identity thins.' },
                  { key: 'mystic', name: 'Mystic', desc: 'Ritual and ambiguity sway minds.' },
                ].map(a => (
                  <div key={a.key} className="listItem">
                    <div className="col">
                      <strong>{a.name}</strong>
                      <span className="small muted">{a.desc}</span>
                    </div>
                    <button className="button primary" onClick={() => setState(s => ({ ...s, doctrine: { ...s.doctrine, archetype: a.key as any } }))}>Select</button>
                  </div>
                ))}
              </div>
            </div>
          ) : (
            <div className="col">
              <div className="row" style={{ justifyContent: 'space-between' }}>
                <span className="badge">Archetype: {state.doctrine.archetype}</span>
                <span className="badge">Points: {state.doctrine.points}</span>
              </div>
              <span className="small muted">Spend points to nudge traits. Points accrue slowly over time; respec is limited.</span>
              <TraitSpend label="Rigidity" value={state.traits.rigidity} min={0} max={100} step={5}
                canChange={(delta) => state.doctrine.points >= 1 && !(delta < 0 && state.traits.rigidity <= 0)}
                onChange={(delta) => setState(s => ({ ...s, traits: { ...s.traits, rigidity: clampNum(s.traits.rigidity + delta, 0, 100) }, doctrine: { ...s.doctrine, points: s.doctrine.points - 1 } }))} />
              {(
                [
                  ['charisma','Charisma'], ['terror','Terror'], ['temptation','Temptation'], ['mysticism','Mysticism'], ['technocracy','Technocracy']
                ] as const
              ).map(([k, label]) => (
                <TraitSpend key={k} label={label} value={(state.traits as any)[k]} min={0} max={5} step={1}
                  canChange={(delta) => state.doctrine.points >= 1 && !((state.traits as any)[k] + delta < 0)}
                  onChange={(delta) => setState(s => ({ ...s, traits: { ...s.traits, [k]: clampNum((s.traits as any)[k] + delta, 0, 5) as any }, doctrine: { ...s.doctrine, points: s.doctrine.points - 1 } }))} />
              ))}
            </div>
          )}
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

function TraitSpend({ label, value, min, max, step = 1, canChange, onChange }: { label: string; value: number; min: number; max: number; step?: number; canChange: (delta: number) => boolean; onChange: (delta: number) => void }) {
  return (
    <div className="row" style={{ justifyContent: 'space-between', alignItems: 'center' }}>
      <span className="small" style={{ width: 96 }}>{label}</span>
      <span className="badge" style={{ minWidth: 36, textAlign: 'center' }}>{value}</span>
      <div className="row" style={{ gap: 6 }}>
        <button className="button" onClick={() => canChange(-step) && onChange(-step)}>-{step}</button>
        <button className="button" onClick={() => canChange(step) && onChange(step)}>+{step}</button>
      </div>
    </div>
  )
}

function clampNum(n: number, min: number, max: number) { return Math.max(min, Math.min(max, n)) }
