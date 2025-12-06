# Pikmin Game Systems - Implementation Guide

## Overview
This guide explains the newly created scripts for your Pikmin game based on TodoGame.txt requirements.

## Created Scripts

### 1. Pellet & Onion System

#### **PelletFlower.cs**
- Manages flowers that contain pellets
- Features:
  - Takes damage and drops pellets when destroyed
  - Automatically respawns after a configurable time
  - Can be attacked by Pikmin
- Setup:
  - Attach to flower GameObjects
  - Assign pellet prefab
  - Configure respawn time (default: 30s)

#### **Pellet.cs** (Already existed - Enhanced)
- Represents pellets that create new Pikmin
- Features:
  - Can be carried by Pikmin to the Onion
  - Converts to Pikmin when absorbed
  - Different types: Number, Flower, Enemy corpse
- Setup:
  - Configure pellet value and weight
  - Set color and number display

#### **PikminOnion.cs** (Already existed - Verified)
- Stores and spawns Pikmin
- Features:
  - Starts buried underground, rises when player approaches
  - Accepts pellets and converts them to Pikmin
  - Spawns Pikmin from underground with emergence animation
  - Manages Pikmin population limit
- Setup:
  - Assign Pikmin prefab
  - Configure spawn settings
  - Set activation requirements

### 2. Pikmin Type System

#### **PikminType.cs** (Base Class)
- Abstract base class for all Pikmin types
- Defines:
  - Resistances (fire, water, electricity, poison, cold, dark)
  - Special abilities (swimming, flying, digging, etc.)
  - Stat multipliers (strength, speed, jump height)
- Usage:
  - All Pikmin type scripts inherit from this
  - Attach alongside Pikmin.cs component

#### **RedPikmin.cs**
- Fire resistant combat specialists
- Abilities:
  - Immune to fire damage
  - 1.5x attack damage bonus
  - Can extinguish fires
  - Destroys fire hazards
- Setup:
  - Attach to Pikmin prefab
  - Configure fire extinguish radius
  - Add fire resist particle effect

#### **BluePikmin.cs**
- Aquatic Pikmin that can swim
- Abilities:
  - Can swim and breathe underwater
  - Rescue drowning Pikmin
  - Apply buoyancy in water
  - Create water splash pushes
- Setup:
  - Attach to Pikmin prefab
  - Configure swim speed and water drag
  - Set water layer mask
  - Add bubble/splash effects

#### **YellowPikmin.cs**
- High jumpers with electricity resistance
- Abilities:
  - Jump 3x higher than normal
  - Immune to electricity
  - Auto-jump over obstacles
  - Destroy electric walls
  - Dig/excavate 2x faster
- Setup:
  - Attach to Pikmin prefab
  - Configure jump force and cooldown
  - Set electric layer mask
  - Add jump trail and spark effects

#### **WhitePikmin.cs**
- Fast treasure hunters with poison resistance
- Abilities:
  - 1.5x movement speed (fastest!)
  - Detect buried treasures underground
  - Immune to poison
  - Neutralize poison gas
  - Toxic when eaten (damages enemies)
- Setup:
  - Attach to Pikmin prefab
  - Configure treasure detection radius
  - Set treasure and poison layer masks
  - Add detection and poison effects

### 3. Hazard & Obstacle System

#### **FireHazard.cs**
- Fire zones that damage non-resistant Pikmin
- Features:
  - Damages Pikmin over time
  - Can be extinguished by Red Pikmin
  - Optionally respawns after time
  - Visual effects scale with fire strength
- Setup:
  - Attach to fire zone GameObjects
  - Tag as "Fire" or assign to fire layer
  - Add particle system and light

#### **ElectricWall.cs**
- Electric barriers blocking passage
- Features:
  - Damages non-electric-resistant Pikmin
  - Can be destroyed by Yellow Pikmin
  - Visual effects show damage state
- Setup:
  - Attach to wall GameObjects
  - Tag as "Electric" or assign to electric layer
  - Configure health and damage
  - Add electric particle effects

#### **PoisonHazard.cs**
- Poison gas areas
- Features:
  - Damages non-poison-resistant Pikmin
  - Can be neutralized by White Pikmin
  - Optional respawn after neutralization
- Setup:
  - Attach to poison zone GameObjects
  - Tag as "Poison" or assign to poison layer
  - Add poison gas particle system

#### **BuriedTreasure.cs**
- Hidden treasures underground
- Features:
  - Starts buried and invisible
  - Revealed by White Pikmin detection
  - Requires digging to excavate
  - Rises to surface when fully dug up
- Setup:
  - Attach to treasure GameObjects
  - Configure treasure value
  - Set buried depth
  - Assign to treasure layer

## How Systems Work Together

### Pellet Flow
1. PelletFlower exists in world
2. Pikmin attacks PelletFlower
3. Flower dies and spawns Pellet
4. Pikmin carry Pellet to PikminOnion
5. Onion absorbs Pellet and creates new Pikmin
6. PelletFlower respawns after timer

### Pikmin Type Interactions
1. Attach appropriate PikminType script to Pikmin prefab
2. PikminType automatically sets resistances and abilities
3. When entering hazards:
   - Script checks `CanSurviveHazard(hazardType)`
   - Returns true if resistant, false if vulnerable
4. Hazards check Pikmin type before applying damage

### Treasure Hunting
1. WhitePikmin periodically scans for buried treasures
2. When found, calls `BuriedTreasure.Reveal()`
3. Treasure becomes visible with indicator
4. Pikmin can then dig it up with `Dig(amount)`
5. When fully excavated, treasure rises to surface
6. Can be carried to base/collected

## Unity Setup Checklist

### For Each Pikmin Type:
- [ ] Create prefab variant of base Pikmin
- [ ] Add appropriate PikminType component (RedPikmin, BluePikmin, etc.)
- [ ] Assign particle effects for abilities
- [ ] Set correct color in inspector
- [ ] Configure type-specific settings

### For Hazards:
- [ ] Create hazard GameObject with collider (trigger)
- [ ] Add appropriate hazard script
- [ ] Tag correctly or assign to proper layer
- [ ] Add visual effects (particles, lights)
- [ ] Test with resistant and non-resistant Pikmin

### For Pellet System:
- [ ] Create PelletFlower prefabs with Pellet prefabs assigned
- [ ] Set up PikminOnion in scene with Pikmin prefab assigned
- [ ] Configure layers for pellet detection
- [ ] Test pellet carrying and absorption

## Layer Mask Setup Recommendations

Create these layers in Unity:
- **Ground** - For ground detection
- **Water** - For water zones
- **Fire** - For fire hazards
- **Electric** - For electric obstacles
- **Poison** - For poison zones
- **Treasure** - For buried treasures
- **Obstacles** - For walls/barriers

## Tags Setup

Create these tags:
- **Player** - For player character
- **Pikmin** - For Pikmin
- **Fire** - For fire hazards
- **Water** - For water zones
- **Electric** - For electric hazards
- **Poison** - For poison zones

## Testing Tips

1. **Test Pellet System:**
   - Attack a PelletFlower
   - Verify pellet drops
   - Carry to onion
   - Check Pikmin spawns from ground
   - Wait for flower respawn

2. **Test Pikmin Types:**
   - Red: Walk into fire (should survive)
   - Blue: Enter water (should swim)
   - Yellow: Try jumping, enter electric zone
   - White: Place near buried treasure (should detect)

3. **Test Hazards:**
   - Non-resistant Pikmin should take damage
   - Resistant Pikmin should survive
   - Red should extinguish fire
   - Yellow should destroy electric walls
   - White should neutralize poison

## Next Steps

To fully implement the system:
1. Create Pikmin prefab variants for each type
2. Set up hazard zones in your levels
3. Place PelletFlowers around the map
4. Configure PikminOnion spawn settings
5. Create buried treasures for White Pikmin
6. Test all interactions

## Notes

- All scripts have debug logging - check Console for feedback
- Gizmos are enabled - use Scene view to see detection radii
- Scripts are modular - can be used independently
- Health system uses existing TestHealth component
- Compatible with existing Pikmin.cs and PikminManager.cs
