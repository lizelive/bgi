# Bad Guy Inc

*The only diffrance between heros and villian is PR.*

Bad Guy Inc. is a satirical colony sim where you play as a villainous mastermind, building cults, managing follower psychology, and defending your faction from heroic invaders. Whether you're a blood-drenched prophet, a fungal hive consciousness, or a bureaucratic transhumanist, you'll shape doctrine, build buildings, and balance loyalty bars like Faith, Fear, and Ecstasy. Because in this world, the difference between a hero and a villain is branding.

## ⚙️ Key Systems (Dynamic Gameplay Anchors)

| System | Description | Example Mechanics |
|--------|-------------|-------------------|
| 🏛️ **Base Building & Aesthetic Control** | Design your cult HQ to reflect your villain style and grant bonuses. | Architecture grants Fear aura, Meme Studio boosts Ecstasy. |
| 📊 **Follower Management** | Maintain follower bars (Faith, Fear, Security, etc.) via infrastructure and doctrine choices. | Loyalty decays if Identity Dissolution is ignored in hive mind builds. |
| 🧬 **Doctrine Engine** | Customize ideological rules, communication style, and rituals. | Rigidity increases Faith retention but slows innovation. |
| 🎭 **PR Simulator** | Shape your public image via media, merch, rituals, and sabotage. | “Tragic Monarch” branding converts heroes into sympathetic assets. |
| ⚔️ **Hero Threat System** | Procedural hero raids evolve as your infamy grows. | Heroes adapt to your strategies — technocratic villains may face hacker paladins. |
| 🧠 **Leader Progression Tree** | Unlock traits and perks that transform colony mechanics. | “Charisma IV” allows mass euphoric rallies. “Technocracy III” unlocks drone mind-sync. |
| 🔀 **Event & Crisis Engine** | Inject narrative spikes — betrayals, miracles, internal schisms. | Follower dreams foretell collapse unless a relic is retrieved. |
| 🌍 **Territory & Diplomacy Map** | Expand across zones, each with unique follower types and hero factions. | Spore hive vs underground anarchists vs influencer paladins. |

## 🔁 Core Gameplay Loop: Villainous Colony in Action

**1. Cultivation & Construction**  
- Build facilities that reflect your doctrine: blood altars, propaganda studios, spore chambers, or bureaucratic void archives.  
- Choose architectural aesthetics based on faction (e.g. Gothic Bone for Blood Cult, Neon Sterile for Transhumanists).  
- Generate base resources: **Influence**, **Followers**, **Materials**, **Ideology Energy**.

**2. Indoctrination & PR Expansion**  
- Recruit new followers through events: sermons, upgrades, propaganda blasts, temptation contracts.  
- Balance follower needs (Faith, Fear, Ecstasy, etc.) to prevent revolt or apathy.  
- Launch PR campaigns: social media seductions, ritual livestreams, graffiti raids, or meme warfare.

**3. Doctrine Management & Leader Evolution**  
- Level up your villain traits: unlock perks (e.g. "Dark Charisma", "Optimized Hive Efficiency", "Apocalyptic Frenzy").  
- Tailor your doctrine: tighten orthodoxy, loosen for innovation, or splinter into sub-factions.  
- Choose ideological upgrades that transform gameplay — psychic bonding, mass hallucination generators, or bureaucratic reality filters.

**4. Hero Invasion & Faction Defense**  
- Heroes attack in waves — idealists, whistleblowers, liberated NPCs.  
- Design base defense: traps, elite enforcers, propaganda turrets, or sub-reality cloaking fields.  
- Respond with tactical deployments or narrative subversions (e.g. "convert the hero live on stream").

**5. Crisis Response & Internal Conflict**  
- Deal with faction splits, PR scandals, rogue AI outbreaks, or dwindling faith.  
- Make dramatic choices: purge dissidents, spin betrayal as growth, or transition to a new doctrine entirely.  
- Use mini-events to steer story (e.g. a follower becomes prophet, or a hero leaks your soul-exchange contract).

**6. Growth & Expansion**  
- Unlock new territories: digital dominions, astral planes, abandoned metro tunnels, influencer dreamscapes.  
- Begin new colony branches with different strategies or blended doctrines.  
- Challenge rival villain factions or ally through twisted diplomacy.

## 🧠 Leader Traits (Dictate Control Style & Follower Expectations)

| Trait Type             | Description & Gameplay Effects                                                                 | Sample Leader Concepts                    |
|------------------------|-----------------------------------------------------------------------------------------------|--------------------------------------------|
| **Charisma**           | Wins followers through charm, vision, and emotional resonance. May overpromise.               | Devil, False Messiah, Communist Leader     |
| **Terror**             | Rules by fear, punishment, and shows of dominance. Loyalty driven by survival.                | Blood Cult Priest, Beast, Apocalypse Prophet |
| **Doctrine Rigidity**  | Focuses on purity, rules, rituals. Deviations punished, orthodoxy rewarded.                   | Occult Bureaucracy, Hive Mind, Monarch     |
| **Temptation**         | Seduces followers with personal gain — power, wealth, beauty. Loyalty is transactional.       | Devil, Glam Villain, Transhuman Cult Leader |
| **Mysticism**          | Shrouds motivations in ambiguity, cosmic logic, or prophetic symbolism.                       | Prophet, Meme Cultist, Fungal Hive Mind     |
| **Technocracy**        | Relies on data, upgrades, and efficient control systems. May lose emotional rapport.          | Mad Scientist, Transhuman Cult             |

Each trait can unlock perks or gameplay modes. For example:
- *Charisma* unlocks rally bonuses but suffers from loyalty spikes.
- *Technocracy* allows automated governance but increases detachment meter.


## 📊 Follower Bars & Needs (Systemic Loyalty Mechanics)

| Bar / Need Name          | Description & Tied Leader Traits                                          |
|--------------------------|----------------------------------------------------------------------------|
| **Faith**                | Belief in leader's vision or divinity. High = resilient loyalty.          |
| **Fear**                 | Driven by punishment or existential dread. Overuse causes revolts.        |
| **Ecstasy**              | Cult-like euphoria tied to ritual, propaganda, or substances.             |
| **Security**             | Followers feel physically safe and economically stable.                   |
| **Status**               | Followers want recognition, rank, or symbolic elevation.                  |
| **Truth**                | Cognitive alignment with leader’s ideology or narrative.                  |
| **Identity Dissolution** | Hive-style loyalty. High = merged minds. Low = rebellion or confusion.    |
| **Personal Gain**        | Loyalty based on benefits — powers, upgrades, perks.                      |

## ▶️ Play the Prototype (Web)

An interactive web prototype now lives in this repo (Vite + React + TS):

- Start the dev server: `npm run dev` (or build + preview: `npm run build` then `npm run preview`)
- Open: http://localhost:5173/

What’s included:
- Real-time simulation loop (pause/speed 1x/2x/4x)
- Follower bars (Faith, Fear, Ecstasy, Security, Status, Truth, Identity Dissolution, Personal Gain)
- Facilities that shape bars and resources (build/dismantle)
- Doctrine sliders (traits) that steer system dynamics
- Autosave and Reset
 - Heat & Hero Attacks: Heat rises with Influence and facilities, drops with Security. The higher the Heat, the sooner a hero will strike. A timer shows when the next attack hits; heroes apply deterministic debuffs when they attack.
 - Manual Recruiting: Use Rally (Influence), Tempt (Materials), or Fear (no cost, risky) to gain followers on demand with a short cooldown.

This is a prototype; numbers and interactions are placeholder but wired for iteration.
