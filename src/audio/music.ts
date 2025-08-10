import { GameState } from '../game/types'

// Simple layered procedural music engine using Web Audio API
export class MusicEngine {
  private ctx: AudioContext
  private master: GainNode
  private layers: Record<string, { gain: GainNode }>
  private lfos: OscillatorNode[] = []
  private started = false
  private muted = false

  constructor() {
    const Ctx = (window as any).AudioContext || (window as any).webkitAudioContext
    this.ctx = new Ctx()
    this.master = this.ctx.createGain()
    this.master.gain.value = 0.0
    this.master.connect(this.ctx.destination)
    this.layers = {}

    // Create layers
    this.layers.pad = this.createPadLayer()
    this.layers.dread = this.createDreadLayer()
    this.layers.tech = this.createTechLayer()
    this.layers.pulse = this.createPulseLayer()
    this.layers.hiss = this.createHissLayer()
  }

  async start() {
    if (this.started) return
    if (this.ctx.state === 'suspended') await this.ctx.resume()
    this.fadeTo(this.master.gain, 0.6, 0.5)
    this.started = true
  }

  stop() {
    this.fadeTo(this.master.gain, 0, 0.3)
  }

  setMuted(m: boolean) {
    this.muted = m
    this.fadeTo(this.master.gain, m ? 0 : 0.6, 0.2)
  }

  setVolume(v: number) {
    if (this.muted) return
    this.fadeTo(this.master.gain, Math.max(0, Math.min(1, v)), 0.1)
  }

  update(state: GameState) {
    if (!this.started) return
    const b = state.bars
    const t = state.traits
    // Compute intensities 0..1
    const serene = clamp01((b.faith + b.ecstasy) / 200) * (1 - clamp01(b.fear / 100))
    const menace = clamp01((b.fear * 0.7 + (100 - b.security) * 0.5 + t.terror * 10) / 150)
    const tech = clamp01((t.technocracy * 20 + b.truth) / 150)
    const ritual = clamp01((t.rigidity * 0.6 + b.status * 0.5 + b.faith * 0.3) / 100)
    const hiss = clamp01((100 - b.truth) / 100)

    this.setLayer('pad', serene * 0.9)
    this.setLayer('dread', menace * 0.9)
    this.setLayer('tech', tech * 0.8)
    this.setLayer('pulse', ritual * 0.7)
    this.setLayer('hiss', hiss * 0.3)
  }

  private setLayer(name: string, level: number) {
    const l = this.layers[name]
    if (!l) return
    this.fadeTo(l.gain.gain, Math.max(0, Math.min(1, level)), 0.25)
  }

  private fadeTo(param: AudioParam, value: number, time: number) {
    const now = this.ctx.currentTime
    param.setTargetAtTime(value, now, Math.max(0.01, time / 3))
  }

  private createPadLayer() {
    const gain = this.ctx.createGain()
    gain.gain.value = 0
    const f1 = this.ctx.createBiquadFilter(); f1.type = 'lowpass'; f1.frequency.value = 1200
    const lfo = this.ctx.createOscillator(); lfo.type = 'sine'; lfo.frequency.value = 0.05
    const lfoGain = this.ctx.createGain(); lfoGain.gain.value = 300
    lfo.connect(lfoGain).connect(f1.frequency)
    this.lfos.push(lfo); lfo.start()

    const osc1 = this.ctx.createOscillator(); osc1.type = 'sine'; osc1.frequency.value = 220
    const osc2 = this.ctx.createOscillator(); osc2.type = 'sine'; osc2.frequency.value = 220 * Math.pow(2, 7 / 12)
    const detuneLfo = this.ctx.createOscillator(); detuneLfo.type = 'sine'; detuneLfo.frequency.value = 0.03
    const detuneGain = this.ctx.createGain(); detuneGain.gain.value = 4
    detuneLfo.connect(detuneGain)
    detuneGain.connect(osc1.detune)
    detuneGain.connect(osc2.detune)
    detuneLfo.start()

    osc1.connect(f1).connect(gain).connect(this.master)
    osc2.connect(f1)
    osc1.start(); osc2.start()
    return { gain }
  }

  private createDreadLayer() {
    const gain = this.ctx.createGain(); gain.gain.value = 0
    const lp = this.ctx.createBiquadFilter(); lp.type = 'lowpass'; lp.frequency.value = 400
    const osc = this.ctx.createOscillator(); osc.type = 'sawtooth'; osc.frequency.value = 55
    const sub = this.ctx.createOscillator(); sub.type = 'sine'; sub.frequency.value = 55 / 2
    const trem = this.ctx.createOscillator(); trem.type = 'triangle'; trem.frequency.value = 0.9
    const tremGain = this.ctx.createGain(); tremGain.gain.value = 0.4
    trem.connect(tremGain).connect(gain.gain)
    trem.start()
    osc.connect(lp).connect(gain).connect(this.master)
    sub.connect(lp)
    osc.start(); sub.start()
    return { gain }
  }

  private createTechLayer() {
    const gain = this.ctx.createGain(); gain.gain.value = 0
    const hp = this.ctx.createBiquadFilter(); hp.type = 'highpass'; hp.frequency.value = 800
    const delay = this.ctx.createDelay(1.5); delay.delayTime.value = 0.25
    const fb = this.ctx.createGain(); fb.gain.value = 0.25
    delay.connect(fb).connect(delay)
    const osc = this.ctx.createOscillator(); osc.type = 'square'; osc.frequency.value = 440
    const lfo = this.ctx.createOscillator(); lfo.type = 'sawtooth'; lfo.frequency.value = 0.2
    const lfoGain = this.ctx.createGain(); lfoGain.gain.value = 300
    lfo.connect(lfoGain).connect(hp.frequency)
    lfo.start()
    osc.connect(hp).connect(delay).connect(gain).connect(this.master)
    osc.start()
    return { gain }
  }

  private createPulseLayer() {
    const gain = this.ctx.createGain(); gain.gain.value = 0
    const mod = this.ctx.createOscillator(); mod.type = 'square'; mod.frequency.value = 2
    const modGain = this.ctx.createGain(); modGain.gain.value = 0.4
    mod.connect(modGain).connect(gain.gain); mod.start()
    const osc = this.ctx.createOscillator(); osc.type = 'sine'; osc.frequency.value = 110
    const bp = this.ctx.createBiquadFilter(); bp.type = 'bandpass'; bp.frequency.value = 220; bp.Q.value = 4
    osc.connect(bp).connect(gain).connect(this.master)
    osc.start()
    return { gain }
  }

  private createHissLayer() {
    const gain = this.ctx.createGain(); gain.gain.value = 0
    const hp = this.ctx.createBiquadFilter(); hp.type = 'highpass'; hp.frequency.value = 2000
    const buffer = this.ctx.createBuffer(1, this.ctx.sampleRate * 2, this.ctx.sampleRate)
    const data = buffer.getChannelData(0)
    for (let i = 0; i < data.length; i++) data[i] = (Math.random() * 2 - 1) * 0.5
    const src = this.ctx.createBufferSource(); src.buffer = buffer; src.loop = true
    src.connect(hp).connect(gain).connect(this.master)
    src.start()
    return { gain }
  }
}

function clamp01(n: number) { return Math.max(0, Math.min(1, n)) }
