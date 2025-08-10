import React, { PropsWithChildren } from 'react'

export default function Panel({ title, children }: PropsWithChildren<{ title: string }>) {
  return (
    <section className="panel">
      <div className="sectionTitle">{title}</div>
      {children}
    </section>
  )
}
