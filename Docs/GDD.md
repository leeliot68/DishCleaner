# Dish Washer

## High level concept 

### Concept Statement

- As a player, you are faced with an impossibly tall mountain of dirty dishes to clean. Each one you clean and send in earns you some money, and with enough money, you might just be able to afford a power washer to speed up the process. As you continue slugging through the mountain, you gain access to more powerful, complex tools to aid you in conquering the tower.

### Genre(s)

- Idle/Incremental
- Automation / Base-building
- Casual Simulation

### Target Audience

- Casual Mobile gamers (12-35 years old primarily)
- Players who enjoy base-building games such as Clash of Clans or Farmville
- Players who enjoy automation / layout optimization games such as Factorio and Find the needle

## Product Design

### Player Experience and POV

- The player plays from a top-down view similar to base-building games such as Clash of Clans, and controls a sprite that represents the player. The player is able to control the sprite's movement and actions by moving to and clicking on interactble objects e.g. pile of plates to grab a plate. Building, placing and moving objects in the room is handled by dragging and dropping from a building menu.
- The player is kept engaged through explicit missions e.g. hitting a specific threshold of plates cleaned or plates/min benchmark, and implicit goals such as maximising throughput or optimizing the base layout.

### Visual and Audio Style

- TBD

### Platform(s) and Technology

- Unity Engine, top-down 2D game
- Mobile game (IOS/Android)

## Game Systems Design

### Core Loop

- The core loop consists of 3 distinct phases of the game as the player progresses.
- Early Game Loop: Collect plate from pile -> Bring to sink and wash -> Bring to terminal and submit -> Use earned money to purchase upgrades that increase loop speed or income.
- Mid Game Loop: Build tools and stations such as conveyor belts, plate grabbers, or auto-washers to begin automating the collecting and washing process -> Use earned money to upgrade existing tools or purchase new ones. Towards the tail-end of the mid game, the introduction of post-washing processing adds another facet for the player to increase potential income in order to progress to the late game easier.
- Late Game Loop: Continuing the established loop of building and upgrading tools in order to unlock new technologies and expand the room, the late game is distinguished by having sufficient income such that the limiting factor is now the available space, and gameplay is now dictated by optimizing available space instead of pure quantity of buildings.

### Objectives and Progression

- Primary Objective: Process the mountain of plates as efficiently as possible.
- There are 2 ways to pursue the primary objective, plates/min (achieved by upgrading plate processing tools and layout optimization), as well as income/min (achived by a mixture of plate processing as well as post-washing processing such as plate wrapping in order to raise the sell value of the plates).
- Throughout the game, upgrades and facilities are locked behind a mission-based progression system that incrementally get more difficult in order to explicitly encourage active progression beyond the typical incremental idle game. Unlocking components also unlocks their respective upgrade trees and auxillary building types to purchase, for example unlocking conveyor belts would allow you to purchase the blueprint for splitters and joiners as well as upgrade the conveyor belt speed. An example progression table is shown below.
| Mission    | Reward |
| -------- | ------- |
| Earn $10  | Unlock Auto-Washer |
| Earn $50 | Unlock Plate-grabber and conveyor belts |
| Earn $500    | Unlock Plate Wrapper |
| Sell 100 Wrapped plate bundles | Unlock Plate Crusher |
| Earn at least $200/min | Unlock Vase Maker|


### Game Systems

- Manual Actions: The game needs to be able to support manual dish-washing in the early game primarily, allowing for player movement and interaction with plates and sinks in order to complete the early game loop.
- Automation capability: The game needs to be able to handle a potentially large amount of facilities that automatically complete tasks such as plate grabbers, washers, and processors that have intake requirements, processing speeds, etc. that can all be upgraded depending on progression progress.
- Advanced routing: In the late-game, optimal layouts may include advanced routing systems where conveyor belts will have to be able to route objects based on type, weighted ratios, priority routing, etc.
- Economy and Upgrade Tree: The game state will have to keep track of the economy, income/min and plates/min, as well as the state of the current progression tree in order to determine mission progress, facilitiy performance, and available income to upgrade / purchase facilities.
