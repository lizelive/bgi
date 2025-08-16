import React, { useState } from 'react';

const API_URL = 'http://blathers.local:8000';

export default function OpenAIChat() {
  const [messages, setMessages] = useState([
    { role: 'system', content: 'You are chatting with Blathers (OpenAI endpoint).' }
  ]);
  const [input, setInput] = useState('');
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  async function sendMessage(e: React.FormEvent) {
    e.preventDefault();
    if (!input.trim()) return;
    setLoading(true);
    setError('');
    const newMessages = [...messages, { role: 'user', content: input }];
    setMessages(newMessages);
    setInput('');
    try {
      const res = await fetch(API_URL + '/v1/chat/completions', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ messages: newMessages })
      });
      if (!res.ok) throw new Error('API error: ' + res.status);
      const data = await res.json();
      const aiMsg = data.choices?.[0]?.message?.content || '[No response]';
      setMessages([...newMessages, { role: 'assistant', content: aiMsg }]);
    } catch (err: any) {
      setError(err.message || 'Unknown error');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div style={{ maxWidth: 600, margin: '2rem auto', padding: 24, background: '#222', borderRadius: 8, color: '#fff' }}>
      <h2>Chat with Blathers (OpenAI endpoint)</h2>
      <div style={{ minHeight: 200, marginBottom: 16, background: '#111', padding: 12, borderRadius: 6 }}>
        {messages.map((m, i) => (
          <div key={i} style={{ margin: '8px 0', color: m.role === 'user' ? '#8cf' : m.role === 'assistant' ? '#cfc' : '#aaa' }}>
            <b>{m.role}:</b> {m.content}
          </div>
        ))}
        {loading && <div style={{ color: '#aaa' }}>Blathers is thinking...</div>}
      </div>
      <form onSubmit={sendMessage} style={{ display: 'flex', gap: 8 }}>
        <input
          value={input}
          onChange={e => setInput(e.target.value)}
          disabled={loading}
          style={{ flex: 1, padding: 8, borderRadius: 4, border: '1px solid #444', background: '#222', color: '#fff' }}
          placeholder="Type your message..."
        />
        <button type="submit" disabled={loading || !input.trim()} style={{ padding: '8px 16px', borderRadius: 4, background: '#444', color: '#fff', border: 'none' }}>
          Send
        </button>
      </form>
      {error && <div style={{ color: '#f88', marginTop: 8 }}>{error}</div>}
    </div>
  );
}
