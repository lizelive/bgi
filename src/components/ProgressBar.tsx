import React from 'react'

type Props = {
  value: number
  color?: string
  label?: string
  delta?: number
}

export default function ProgressBar({ value, color = '#9b87f5', label, delta = 0 }: Props) {
  const v = Math.max(0, Math.min(100, value))
  const showDelta = Math.abs(delta) >= 0.01
  const deltaText = showDelta ? `${delta > 0 ? '+' : ''}${delta.toFixed(0)}` : ''
  return (
    <div>
      {label && (
        <div className="row" style={{ justifyContent: 'space-between', marginBottom: 4 }}>
          <span className="small">{label}</span>
          <span className={`small muted`}>{v.toFixed(0)}%</span>
        </div>
      )}
      <div className="barWrapper" title={`${v.toFixed(0)}%`}>
        <div className={`progress`}>
          <span className={`${showDelta ? 'pulse' : ''}`} style={{ width: `${v}%`, background: color }} />
        </div>
        {showDelta && <span className={`barDeltaBadge ${delta < 0 ? 'neg' : 'pos'}`}>{deltaText}</span>}
      </div>
    </div>
  )
}
