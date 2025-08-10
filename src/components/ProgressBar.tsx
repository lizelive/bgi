import React from 'react'

type Props = {
  value: number
  color?: string
  label?: string
}

export default function ProgressBar({ value, color = '#9b87f5', label }: Props) {
  const v = Math.max(0, Math.min(100, value))
  return (
    <div>
      {label && (
        <div className="row" style={{ justifyContent: 'space-between', marginBottom: 4 }}>
          <span className="small">{label}</span>
          <span className="small muted">{v.toFixed(0)}%</span>
        </div>
      )}
      <div className="progress" title={`${v.toFixed(0)}%`}>
        <span style={{ width: `${v}%`, background: color }} />
      </div>
    </div>
  )
}
