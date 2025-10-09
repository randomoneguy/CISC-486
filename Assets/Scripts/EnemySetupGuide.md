# Robot Kyle Enemy Setup Guide - Hierarchical State Machine

This guide will help you convert Robot Kyle from a player character into an enemy with a hierarchical state machine system.

## Step 1: Set Up Tags and Layers

The following tags and layers have been added to your project:
- **Tags**: Player, Enemy, Ground
- **Layers**: Enemy, Player

## Step 2: Modify Robot Kyle Prefab

### Remove Player Components:
1. Open the Robot Kyle prefab: `Assets/UnityTechnologies/SpaceRobotKyle/Prefabs/RobotKyle.prefab`
2. Remove or disable these components:
   - `ThirdPersonController` script
   - `PlayerInput` component
   - `StarterAssetsInputs` script (if present)
   - Any camera-related components

### Add Enemy Components:
1. Add `NavMeshAgent` component:
   - Set Speed to 3
   - Set Angular Speed to 120
   - Set Stopping Distance to 0.5
   - Set Auto Braking to true
   - **IMPORTANT**: Uncheck "Update Rotation" (this is set automatically by the EnemyAI script)

2. Add the enemy scripts:
   - `EnemyStateMachine` script (main controller)
   - `EnemyAI` script
   - `EnemyHealth` script

3. Change the GameObject settings:
   - Set Tag to "Enemy"
   - Set Layer to "Enemy"

## Step 3: Configure Enemy State Machine

### EnemyAI Configuration:
- **Charge Range**: 5 (distance to start charging attack)
- **Melee Range**: 2 (distance for melee attacks)
- **Move Speed**: 3 (enemy movement speed)
- **Rotation Speed**: 5 (how fast the enemy turns)
- **Charge Speed**: 6 (speed during charge attack)
- **Charge Cooldown**: 10 (seconds before enemy can charge again)
- **Stopping Distance**: 0.5 (how close to get before stopping to attack)
- **Melee Damage**: 15 (damage for melee attacks)
- **Range Damage**: 10 (damage for ranged attacks)

### State Machine Settings:
- **Charge Duration**: 1 (seconds to charge before attacking)
- **Attack Delay**: 0.5 (delay before attack hits)
- **Melee Attack Delay**: 0.5 (delay before melee attack hits)
- **Melee Attack Duration**: 1.5 (duration of melee attack animation)
- **Range Attack Duration**: 2 (duration of range attack animation)
- **Projectile Speed**: 10 (speed of ranged projectiles)

### Laser Beam Settings:
- **Laser Beam Range**: 15 (range of laser beam attack)
- **Laser Beam Duration**: 3 (how long the laser beam lasts)
- **Laser Beam Cooldown**: 8 (seconds before enemy can use laser beam again)
- **Laser Beam Damage**: 25 (damage dealt by laser beam)
- **Laser Beam Width**: 0.5 (width of the laser beam visual)

### EnemyHealth Configuration:
- **Max Health**: 100 (enemy's maximum health)
- **Death Delay**: 2 (seconds before enemy is destroyed after death)
- **Death Effects**: Add particle effects or sounds for death
- **Damage Effects**: Add effects when enemy takes damage

## Step 4: Enemy State Machine Behavior

### State Flow:
1. **Walk State** (Default):
   - Enemy immediately starts walking towards player on spawn
   - Maintains comfortable distance (1.5 units) from player
   - Transitions to Idle when close enough but can't charge
   - Transitions to Laser Beam when player is within laser range (priority)
   - Transitions to Charge when player is within charge range (if laser on cooldown)

2. **Idle State**:
   - Enemy stops moving and faces player
   - Waits for attack cooldowns to expire
   - Transitions to Walk if player moves too far
   - Transitions to Laser Beam when cooldown is ready and player is in range
   - Transitions to Charge when laser beam on cooldown and player is close

3. **Laser Beam State** (Priority Attack):
   - Enemy stops moving and faces player instantly
   - Shoots laser beam from eyes in straight line
   - No movement or rotation during beam
   - Deals continuous damage to player in beam path
   - Transitions to Idle when beam duration ends

4. **Charge State**:
   - Enemy increases speed to charge speed (6 units)
   - Charges towards player's current position at high speed
   - **Immediately triggers kick attack when in melee range**
   - Has cooldown period before can charge again
   - Falls back to range attack if not in melee range after charge duration

4. **Melee Attack State**:
   - Triggers when player is within melee range
   - Performs melee attack with damage
   - Returns to Idle state after completion

5. **Range Attack State**:
   - Triggers when player is outside melee range
   - Fires projectile at player
   - Returns to Idle state after completion

### Animation Parameters Required:
- **IsWalking** (Bool)
- **IsCharging** (Bool) 
- **IsAttacking** (Bool)
- **Speed** (Float) - Controls movement speed
- **MotionSpeed** (Float) - Controls animation blending (set to 1f for proper walk animation)
- **MeleeAttack** (Trigger)
- **RangeAttack** (Trigger)

## Step 5: Set Up Player

### Add PlayerHealth to Player:
1. Find your player GameObject in the scene
2. Add the `PlayerHealth` script
3. Configure:
   - **Max Health**: 100
   - **Death Delay**: 2
   - **Damage Effects**: Add particle effects for taking damage
   - **Death Effects**: Add effects for player death

### Ensure Player Has Correct Tag:
- Set player GameObject tag to "Player"
- Set player GameObject layer to "Player"

## Step 6: Set Up NavMesh

1. Open Window > AI > Navigation
2. Select the ground/floor objects
3. In the Navigation window, mark them as "Navigation Static"
4. Click "Bake" to generate the NavMesh
5. The enemy will use this NavMesh for pathfinding

## Step 7: Set Up Projectiles (For Range Attacks)

1. Create a projectile prefab:
   - Create a simple GameObject (sphere or capsule)
   - Add the `EnemyProjectile` script
   - Add a Collider (set as Trigger)
   - Add a Rigidbody (set as Kinematic)
   - Create a prefab from this GameObject

2. Assign the projectile prefab to the EnemyAI's "Projectile Prefab" field

3. Configure projectile settings:
   - **Speed**: How fast the projectile travels
   - **Damage**: Damage dealt to player
   - **Lifetime**: How long before projectile is destroyed
   - **Hit Effect**: Particle effect when projectile hits

## Step 8: Configure Animation

The enemy scripts will work with the existing Robot Kyle animations:
- **Speed**: Controls movement animation
- **Attack**: Triggers attack animation
- **Death**: Triggers death animation

Make sure your Animator Controller has these parameters:
- `Speed` (Float)
- `Attack` (Trigger)
- `Death` (Trigger)

## Step 9: Testing

1. Place the Robot Kyle enemy in your scene
2. Make sure the player is also in the scene
3. Play the scene
4. The enemy should:
   - Patrol around (if patrol points are set)
   - Chase the player when they get close
   - Attack the player when in range
   - Take damage when attacked
   - Die when health reaches 0

## Troubleshooting

### Enemy Not Moving:
- Check if NavMesh is baked
- Ensure NavMeshAgent is enabled
- Check if enemy is on the NavMesh

### Enemy Not Attacking:
- Check if player has "Player" tag
- Verify attack range settings
- Check if PlayerHealth script is on player

### Enemy Not Taking Damage:
- Ensure enemy has EnemyHealth script
- Check if damage source is calling TakeDamage method

### Animation Issues:
- Verify Animator Controller has correct parameters
- Check if animation states are set up properly

## Customization Options

### Different Enemy Types:
You can create different enemy variants by:
- Adjusting health, damage, and speed values
- Changing detection and attack ranges
- Modifying patrol behavior
- Adding different attack patterns

### Advanced AI:
For more complex AI behaviors, you can:
- Add more states to the EnemyAI script
- Implement different attack patterns
- Add special abilities
- Create boss behaviors with phases

## Scripts Created:

1. **EnemyAI.cs** - Main AI behavior controller
2. **EnemyHealth.cs** - Health and damage system
3. **EnemyAttack.cs** - Attack system
4. **PlayerHealth.cs** - Player health system

These scripts work together to create a complete enemy system that can fight the player.
