<h1 align="center">DungeonCurses</h1> <h3 align="center"><i>🕯️ The Dungeon Remembers</i></h3>

<p align="center">
  <img src="https://img.shields.io/badge/Genre-Souls--like-6d2424?style=for-the-badge" alt="Souls-like">
  <img src="https://img.shields.io/badge/Dimension-2D-454545?style=for-the-badge" alt="2D">
  <img src="https://img.shields.io/badge/Perspective-Top--Down-343434?style=for-the-badge" alt="Top-Down">
  <img src="https://img.shields.io/badge/Unity-2022.3.62f3-222222?style=for-the-badge&logo=unity&logoColor=white" alt="Unity 2022.3.62f3">
</p>


<p align="center">
  <a href="README_RU.md">🇷🇺 Русская версия</a>
</p>



<h2 align="center">🕯️ About the Game</h2>

<p align="center">
  DungeonCurses is a 2D souls-like game with a top-down perspective, set in the dark depths of a mysterious dungeon.
</p>

<p align="center">
  Fight dangerous enemies, explore gloomy halls, and discover new possibilities. Face curses that can alter the course of your journey, and seek your way through a place whose secrets run far deeper than they might first appear.
</p>

<p align="center">
  Not all darkness hides monsters alone. Not every curse comes without a reason. And not every story ends where life does.
</p>


<h2 align="center">⚔️ Core Features</h2>

### 🗡️ Tactical Combat System

* **Dangerous Enemies** — enemies pose a serious threat. To survive, you must learn their attack patterns, choose the right moment to strike, and react to danger in time.
* **Dodging and Parrying** — a dash with a brief window of invulnerability helps you evade dangerous attacks, while a successful parry stuns the enemy and creates an opportunity to counterattack.
* **Active Items** — additional tools that expand your combat options.

### 🔥 Weapon Corruption and Elemental Effects

* **Weapon Corruption** — temporarily imbues your sword with elemental or other properties, allowing you to deal fire, ice, and other types of elemental damage for several strikes, or granting your attacks additional effects.
* **Status Effects** — fire, rot, bleeding, chilling, and freezing allow you to affect enemies in different ways.
* **Elemental Reactions** — combining effects can increase damage, instantly deal accumulated damage, stun enemies, and trigger additional effects.

#### Status Effect Interactions

| Effect 1 | Effect 2 | Result                                                                                          |
| -------- | -------- | ----------------------------------------------------------------------------------------------- |
| Fire     | Ice      | Removes freezing/chilling and creates a steam explosion that deals damage and stuns the target. |
| Fire     | Rot      | Immediately deals an additional 50% of rot tick damage, then removes rot entirely.              |
| Fire     | Bleeding | Immediately deals 50% of accumulated bleeding damage, then removes bleeding.                    |
| Bleeding | Rot      | Triggers an explosion of both effects, immediately dealing all accumulated damage.              |
| Bleeding | Chilling | Increases bleeding duration by 50% and immediately stuns the target.                            |
| Bleeding | Freezing | Increases bleeding duration by 50% and immediately stuns the target.                            |
| Rot      | Chilling | Removes rot and deals 50% of its damage.                                                        |
| Rot      | Bleeding | The effects' damage ticks stack, intensifying their combined effect.                            |
| Rot      | Fire     | Extends fire duration and removes rot.                                                          |
| Chilling | Fire     | Removes chilling and triggers a steam explosion.                                                |
| Chilling | Bleeding | Extends bleeding duration and stuns the target.                                                 |
| Chilling | Rot      | Has no effect.                                                                                  |

### 🛡️ Equipment and Items

A variety of items allows players to customize their character to suit their preferred playstyle.

* **Estus** — a consumable resource used to restore health.
* **Active Items** — items with effects that the player can activate manually.
* **Passive Items** — provide additional effects without requiring activation.

### ☠️ Curses and Build Customization

Curses are designed to change the way players approach their journey:

* **Beneficial Curses** — provide useful effects and strengthen the character.
* **Curses with a Price** — offer an advantage in exchange for a sacrifice.
* **Debuff Curses** — impose negative effects and make the journey more challenging.

Combining equipment, items, and elemental effects encourages experimentation with different builds and combat strategies.

### 🕯️ Dungeon Exploration

* **Branching Labyrinths** — winding corridors, crossroads, and hidden corners that reward exploration.
* **Interactive Environment** — candles react to fire, ice, and sword strikes.
* **Deadly Traps** — fireballs, ice projectiles, floor spikes, and saws force players to stay alert and potentially use the environment to their advantage.
* **Treasure and Trading** — loot-filled chests and merchants offering useful items.
* **Puzzles and Notes** — optional discoveries that expand the world's lore and raise moral questions.


<h2 align="center">🎬 Development Showcase</h2>

<p align="center"> This video showcases an earlier stage of DungeonCurses development, highlighting implemented gameplay systems and experimental features. It was originally created as part of my college graduation project. The videos will be replaced during the development process. </p>

<p align="center">
  <a href="https://www.youtube.com/watch?v=Y1_PqqWpjOI">
    <img src="https://img.youtube.com/vi/Y1_PqqWpjOI/hqdefault.jpg"
         alt="DungeonCurses Development Showcase"
         width="600">
  </a>
</p>


<h2 align="center">🛠️ Development Status</h2>

DungeonCurses began as a college graduation project and is now being reworked into a polished, standalone demo.

The project is undergoing code refactoring and improvements to its gameplay systems, with a focus on code quality, maintainability, and overall gameplay experience.


<h2 align="center">📁 Project Structure</h2>

```text
Assets/
├── Animations/
├── Editor/
├── Fonts/
├── GraphicsAssets/
├── Other/
├── Prefabs/
├── PrivateAssets/
├── RenderSettings/
├── Scenes/
├── ScriptableObjects/
├── Scripts/
└── Sounds/
```


<h2 align="center">📄 License</h2>

### Source code

The source code of DungeonCurses is released under the [MIT License] (Massachusetts Institute of Technology).

### Third-party resources

Some of the graphic resources used in the project were obtained using paid resource packages. Their licenses prohibit public distribution, so these resources are not included in the public repository.

The "PrivateAssets/" directory is intended for locally stored third-party resources that cannot be distributed publicly. It is not included in the public release.

As a result, ** when opening a project from this repository, most of the graphics resources ** may be missing, and some scenes or gameplay elements may not be displayed as originally intended.

Despite these limitations, the repository remains available to showcase the project's source code, architecture, and game systems. Free resources and other materials that can be legally distributed may be included.