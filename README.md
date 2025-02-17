# Celeste (PICO-8) clone

A project I chose to work on for my Game Programming assignment, which recreates the well-known Celeste (PICO-8) game.

<p align="center">
  <img src="Media/demo.gif"><br/>
  *<i>Low frame rate caused by gif limitations</i>*
</p>

## Overview

Throughout the course, we studied various concepts concerning Game Programming as a whole, all in the context of the GXP 2D Game Engine - a code-based small engine designed and developed by the teachers to introduce students to how a game engine may opperate.

## Features

### Gameplay

- **Player Movement:**
  1. Moving left and right
  2. Jumping
  3. Dashing
  4. Sliding
  5. Wall jumping
- **Interactables:**
  1. Spikes
  2. Trampoline
  3. Destructible blocks - start destructing if you step on them
  4. Balloons - regain your dash ability
- **Collectables:**
  1. Normal Strawberry
  2. Flying Strawberry - flies away if you dash
  3. Key and Chest - unlock a hidden strawberry

### UI/UX, SFX, VFX

- **Start and end screen**
- **HUD:**
  1. Current level
  2. Time passed
  3. Final level - Time, Death count and Strawberry count
- **Sounds**
  1. Background sounds
  2. Sound effects
- **Particles:**
  1. Clouds at the background
  2. Snowflakes
 
### Level editing
Done by using [Tiled](https://www.mapeditor.org/) level editor

## Controls

- **Moving and looking:**
  1. Move left or right by pressing **A** or **D**/**LEFT_ARROW_KEY** or **RIGHT_ARROW_KEY** respectively
  2. Look down or up by pressing **S** or **W**/**DOWN_ARROW_KEY** or **UP_ARROW_KEY** respectively
- **Jumping/Wall jumping:** Jump by pressing **SPACE**/**C**
- **Dashing:** Dash by pressing **LEFT_SHIFT**/**X**
- **Sliding:** Slide by holding the key corresponding to the wall's direction according to the player

## Art
From: https://github.com/NoelFB/Celeste/blob/master/Source/PICO-8/Graphics/atlas.png
