# 🎮 Pikmin Clone - Complete Setup Guide

## 📋 Table of Contents
1. [Unity Project Setup](#unity-project-setup)
2. [Tags & Layers](#tags--layers)
3. [Player Setup](#player-setup)
4. [Pikmin Setup](#pikmin-setup)
5. [Enemy Setup](#enemy-setup)
6. [Obstacle Setup](#obstacle-setup)
7. [Onion Setup](#onion-setup)
8. [Pellet Setup](#pellet-setup)
9. [Testing Checklist](#testing-checklist)

---

## Unity Project Setup

### Requirements
- **Unity Version:** 6.0 or later
- **Render Pipeline:** URP or Built-in (code supports both)
- **Input System:** Legacy Input (uses Input.GetKey, Input.GetAxis)

### Initial Scene Setup
1. Create a new scene
2. Add a **Plane** for the ground (scale 10x1x10 or larger)
3. Add **Directional Light**
4. Ensure Main Camera exists

---

## Tags & Layers

### Required Tags
Go to **Edit > Project Settings > Tags and Layers**

#### Tags to Create:
1. `Player` - for the player character
2. `Fire` - for fire hazards
3. `Water` - for water hazards
4. `Poison` - for poison hazards
5. `Electric` - for electric hazards

### Required Layers
Create these layers (exact layer number doesn't matter):

1. **Ground** - for ground/terrain
2. **Pikmin** - for Pikmin characters
3. **Enemy** - for enemies
4. **Obstacle** - for obstacles Pikmin should avoid
5. **Carryable** - for objects Pikmin can carry
6. **Hazard** - for all hazards

---

## Player Setup

### 1. Create Player GameObject
1. Create **Empty GameObject** named "Player"
2. Add tag "Player"
3. Position at (0, 1, 0)

### 2. Add Visual (Character Model)
1. Add **Capsule** as child (Scale: 0.5, 2, 0.5)
2. Or import your own 3D model

### 3. Add Required Components

#### a) Collider
- Add **Capsule Collider**
  - Radius: 0.5
  - Height: 2
  - Center: (0, 1, 0)

#### b) Rigidbody
- Add **Rigidbody**
  - Mass: 1
  - Drag: 0
  - Angular Drag: 0.05
  - Use Gravity: ✓
  - Is Kinematic: ✗
  - Constraints: Freeze Rotation X, Y, Z ✓

#### c) Health Component
- Add **Scripts/Player/Health.cs**
  - Max Health: 100
  - Destroy On Death: ✗

#### d) Player Controller
- Add **Scripts/Player/PlayerController.cs**
  - Move Speed: 5
  - Sprint Speed: 8
  - Ground Layer: Select "Ground"
  - Camera Transform: Will auto-find Main Camera

#### e) Whistle Controller
- Add **Scripts/Player/WhistleController.cs**
  - Whistle Key: Mouse1 (Right Click)
  - Min Radius: 1
  - Max Radius: 10
  - Ground Layer: Select "Ground"
  - Pikmin Layer: Select "Pikmin"

### 4. Setup Camera

#### Option A: Use Existing Main Camera
1. Select Main Camera
2. Add **Scripts/Player/CameraController.cs**
3. Configure:
   - Target: Drag Player GameObject
   - Offset: (0, 15, -10)
   - Follow Speed: 5
   - Allow Rotation: ✓
   - Rotate Left Key: Q
   - Rotate Right Key: E

#### Option B: Create New Camera
1. Create new Camera GameObject
2. Tag as "MainCamera"
3. Follow steps above

---

## Pikmin Setup

### 1. Create Base Pikmin Prefab

#### a) Create GameObject
1. Create **Empty GameObject** named "Pikmin_Red"
2. Add layer "Pikmin"

#### b) Add Visual
1. Add **Capsule** as child
   - Scale: (0.3, 0.5, 0.3)
   - Position: (0, 0.5, 0)
2. Create material, set color to Red

#### c) Add Components

**Physics:**
- **Rigidbody**
  - Mass: 0.5
  - Drag: 0
  - Angular Drag: 0.05
  - Use Gravity: ✓
  - Freeze Rotation: X, Y, Z ✓

- **Capsule Collider**
  - Radius: 0.3
  - Height: 1
  - Center: (0, 0.5, 0)

**Core Scripts:**
- **Scripts/Player/Health.cs**
  - Max Health: 50
  - Destroy On Death: ✓
  - Destroy Delay: 0.5

- **Scripts/Pikmin/Pikmin.cs**
  - Move Speed: 5
  - Follow Distance: 2
  - Ground Layer: "Ground"
  - Obstacle Layer: "Obstacle"

- **Scripts/Pikmin/RedPikmin.cs**
  - Fire Layer: "Hazard"

- **Scripts/Pikmin/PikminCarrier.cs**
  - Detection Radius: 2
  - Carryable Layer: "Carryable"

- **Scripts/Pikmin/PikminCombat.cs**
  - Attack Damage: 5
  - Detection Radius: 5
  - Enemy Layer: "Enemy"

### 2. Create Prefab
1. Drag "Pikmin_Red" into **Assets/Prefabs/** folder
2. Delete from scene

### 3. Create Other Pikmin Types
Duplicate the Red Pikmin prefab and modify:

**Blue Pikmin:**
- Change color to Blue
- Replace `RedPikmin.cs` with `BluePikmin.cs`
- Water Layer: "Hazard"

**Yellow Pikmin:**
- Change color to Yellow
- Replace with `YellowPikmin.cs`
- Electric Layer: "Hazard"

**White Pikmin:**
- Change color to White
- Replace with `WhitePikmin.cs`
- Poison Layer: "Hazard"
- Speed Multiplier: 1.5

---

## Enemy Setup

### 1. Create Basic Enemy

#### a) Create GameObject
1. Create **Cube** named "Enemy_Basic"
2. Add layer "Enemy"
3. Scale: (2, 2, 2)
4. Create red material

#### b) Add Components

- **Rigidbody**
  - Mass: 5
  - Use Gravity: ✓
  - Freeze Rotation: X, Y, Z ✓

- **Box Collider**
  - Size: (2, 2, 2)

- **Scripts/Player/Health.cs**
  - Max Health: 100
  - Destroy On Death: ✓

- **Scripts/EnemyMovement.cs**
  - Player: Drag Player GameObject
  - Move Speed: 3
  - Stopping Distance: 1

- **Scripts/EnemyCombat.cs**
  - Attack Damage: 10
  - Attack Range: 2
  - Target Layers: Select "Pikmin" and "Player"
  - Corpse Weight: 5
  - Corpse Value: 5

### 2. Create Enemy Prefab
1. Drag into **Assets/Prefabs/** folder
2. Delete from scene

---

## Obstacle Setup

### Fire Hazard Example

1. Create **Cube** named "FireHazard"
2. Scale: (3, 1, 3)
3. Add layer "Hazard"
4. Tag as "Fire"

**Components:**
- **Box Collider**
  - Is Trigger: ✓

- **Scripts/Obstacles/FireHazard.cs**
  - Damage Per Second: 10
  - Fire Color: Orange
  - Respawn After Extinguish: ✓

**Optional Visual:**
- Add **Particle System** (fire particles)
- Add **Light** (orange, intensity 2)

### Water Hazard Example

1. Create **Cube** named "WaterHazard"
2. Scale: (5, 0.5, 5)
3. Position.y: 0
4. Add layer "Hazard"
5. Tag as "Water"

**Components:**
- **Box Collider**
  - Is Trigger: ✓

- **Scripts/Obstacles/WaterHazard.cs**
  - Drowning Damage: 20
  - Water Color: Blue with alpha 0.6

### Electric Wall Example

1. Create **Cube** named "ElectricWall"
2. Scale: (1, 3, 3)
3. Add layer "Hazard"
4. Tag as "Electric"

**Components:**
- **Box Collider**
  - Is Trigger: ✓

- **Scripts/Obstacles/ElectricWall.cs**
  - Damage Per Second: 15
  - Electric Color: Yellow

---

## Onion Setup

### 1. Create Onion GameObject

1. Create **Sphere** named "Onion_Red"
2. Scale: (2, 2, 2)
3. Position: (10, 3, 0) - start buried

**Visual:**
- Create red material
- Add glow effect (emission)

### 2. Add Components

- **Sphere Collider**
  - Radius: 1
  - Is Trigger: ✓

- **Scripts/Pikmin/PikminOnion.cs**
  - Pikmin Prefab: Drag "Pikmin_Red" prefab
  - Max Pikmin In Onion: 50
  - Current Pikmin Count: 5 (starting amount)
  - Spawn Point: Create empty child GameObject at (0, -2, 0)
  - Dig Depth: 2
  - Ground Layer: "Ground"
  - Start Deactivated: ✓ (starts buried)
  - Buried Depth: 3
  - Require Player Touch: ✓

### 3. Create Child Objects

**Spawn Point:**
1. Create empty child "SpawnPoint"
2. Position: (0, -2, 0)
3. Assign to Onion's "Spawn Point" field

**Pellet Receive Point:**
1. Create empty child "PelletReceivePoint"
2. Position: (0, 2, 0)
3. Assign to Onion's "Pellet Receive Point" field

---

## Pellet Setup

### 1. Create Pellet

1. Create **Sphere** named "Pellet_1"
2. Scale: (0.8, 0.8, 0.8)
3. Add layer "Carryable"

**Visual:**
- Create material (color matches Pikmin type)
- Add TextMesh for number "1"

### 2. Add Components

- **Rigidbody**
  - Mass: 1
  - Use Gravity: ✓

- **Sphere Collider**
  - Radius: 0.4
  - Is Trigger: ✗

- **Scripts/Pellet.cs**
  - Pikmin Value: 1
  - Weight: 1
  - Pellet Number: 1
  - Pellet Type: Number

### 3. Create Number Variants
Duplicate and change:
- Pellet_5: Value 5, Weight 3
- Pellet_10: Value 10, Weight 5
- Pellet_20: Value 20, Weight 10

---

## PikminManager Setup

### 1. Create Manager GameObject

1. Create **Empty GameObject** named "PikminManager"
2. Position: (0, 0, 0)

### 2. Add Component

- **Scripts/Pikmin/PikminManager.cs**
  - Player Transform: Drag Player
  - Max Pikmin: 100
  - Formation Type: Circle
  - Formation Spacing: 1
  - Whistle Key: C
  - Dismiss Key: X

---

## Testing Checklist

### Basic Movement
- [ ] Player moves with WASD
- [ ] Player rotates towards mouse cursor
- [ ] Camera follows player
- [ ] Camera rotates with Q/E

### Pikmin Spawning
- [ ] Walk to Onion to activate it
- [ ] Onion rises from ground
- [ ] Pikmin spawn from underground
- [ ] Pikmin emerge with digging animation

### Pikmin Following
- [ ] Pikmin follow player in formation
- [ ] Hold Right-Click to open whistle
- [ ] Whistle circle grows while held
- [ ] Release to call Pikmin in radius
- [ ] Called Pikmin join formation

### Pikmin Launching
- [ ] Hold Left-Click to aim
- [ ] Trajectory line appears
- [ ] Release to launch Pikmin
- [ ] Pikmin flies through air
- [ ] Pikmin lands and follows

### Pikmin Carrying
- [ ] Pikmin detect nearby pellets
- [ ] Multiple Pikmin attach to pellet
- [ ] Pellet starts moving when enough Pikmin attached
- [ ] Pellet delivered to Onion
- [ ] Onion absorbs pellet
- [ ] New Pikmin added to storage

### Pikmin Combat
- [ ] Pikmin detect enemies
- [ ] Pikmin approach enemies
- [ ] Pikmin latch onto enemies
- [ ] Pikmin deal damage over time
- [ ] Enemy health decreases
- [ ] Enemy dies when health = 0
- [ ] Enemy converts to corpse
- [ ] Pikmin can carry corpse

### Enemy Behavior
- [ ] Enemy moves towards player
- [ ] Enemy attacks player
- [ ] Enemy attacks Pikmin
- [ ] Enemy eats Pikmin (Pikmin disappears)
- [ ] Enemy shakes off latched Pikmin
- [ ] Enemy dies from Pikmin attacks

### Hazards
- [ ] Fire damages non-Red Pikmin
- [ ] Red Pikmin immune to fire
- [ ] Red Pikmin extinguish fire
- [ ] Water drowns non-Blue Pikmin
- [ ] Blue Pikmin swim in water
- [ ] Electric walls damage non-Yellow Pikmin
- [ ] Yellow Pikmin destroy electric walls

### UI/Feedback
- [ ] Whistle circle visible
- [ ] Whistle circle changes color when calling Pikmin
- [ ] Player health bar updates
- [ ] Pikmin count displays correctly

---

## Common Issues

### Pikmin Don't Follow
**Fix:**
- Check Player has tag "Player"
- Check Ground Layer is set correctly
- Ensure PikminManager exists in scene

### Whistle Doesn't Work
**Fix:**
- Check Camera is assigned
- Check Ground Layer for raycasting
- Check Pikmin Layer is set

### Pikmin Don't Attack
**Fix:**
- Check Enemy Layer is set
- Ensure enemy has Health component
- Check PikminCombat detection radius

### Carrying Doesn't Work
**Fix:**
- Check Carryable Layer is set
- Ensure pellet has Rigidbody
- Check PikminCarrier detection radius

---

## Next Steps

### Phase 2 Additions
1. Sound effects
2. UI/HUD system
3. Day/night cycle
4. Save/load system

### Additional Content
1. More Pikmin types (Purple, Rock, Winged)
2. More enemy types with varied AI
3. Boss enemies
4. More hazard types
5. Bridge building
6. Gate destruction

---

**🎉 You're ready to play! Have fun with your Pikmin clone!**
