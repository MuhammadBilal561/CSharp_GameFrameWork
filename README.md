# 🎮 SPACE DEFENDER - COMPLETE PROJECT DOCUMENTATION

## TABLE OF CONTENTS
1. [Project Overview](#1-project-overview)
2. [Game Story](#2-game-story)
3. [Controls](#3-controls)
4. [Project Architecture](#4-project-architecture)
5. [OOP Concepts Used](#5-oop-concepts-used)
6. [File-by-File Explanation](#6-file-by-file-explanation)
7. [Design Patterns](#7-design-patterns)
8. [Game Systems](#8-game-systems)
9. [Common Evaluation Questions](#9-common-evaluation-questions)

---

## 1. PROJECT OVERVIEW

**Project Name:** Space Defender
**Technology:** C# (.NET 8) with Windows Forms
**Type:** 2D Side-Scrolling Shooter Game
**Architecture:** Object-Oriented Programming (OOP)

### Key Features:
- 3 playable levels with increasing difficulty
- Multiple enemy types (Human, Drone, Ship, Boss)
- Scoring system with high score tracking
- Player health bar with damage and healing
- Sound effects and background music
- Frame-based animations for all characters
- Main menu with settings
- Persistent save data (TXT format)

---

## 2. GAME STORY

**Year 2157.** Earth faces its greatest threat.

Alien forces have invaded our solar system. **You are Commander Nova**, a veteran pilot who must infiltrate alien strongholds and eliminate the threat.

### Level Breakdown:
| Level | Name | Enemies | Total |
|-------|------|---------|-------|
| 1 | Outpost Alpha | 4 Humans + 5 Drones | 9 |
| 2 | Orbital Station | 6 Humans + 7 Ships | 13 |
| 3 | Command Ship | 6 Humans + 5 Ships + 2 Drones + 2 Bosses | 15 |

---

## 3. CONTROLS

| Key | Action |
|-----|--------|
| Arrow Keys (← →) or A/D | Move Left/Right |
| Arrow Up / W / Space | Jump |
| X / Shift | Shoot Horizontal |
| Q | Shoot Upward |
| ESC | Pause Game |
| Enter | Select Menu Option |

---

## 4. PROJECT ARCHITECTURE

### Folder Structure:
```
├── Core/                  # Core game engine
│   ├── Game.cs           # Main game loop manager
│   ├── GameTime.cs       # Time tracking for updates
│   └── ShootingGameFormBase.cs  # Base class for all levels
│
├── Entities/             # Game objects (inheritance hierarchy)
│   ├── GameObject.cs     # BASE CLASS for all objects
│   ├── Player.cs         # Base player (inherits GameObject)
│   ├── ShootingPlayer.cs # Actual player (inherits Player)
│   ├── Enemy.cs          # Base enemy (inherits GameObject)
│   ├── ShootingEnemy.cs  # Actual enemy (inherits Enemy)
│   ├── Platform.cs       # Ground and platforms
│   ├── PlayerBullet.cs   # Player's projectiles
│   ├── EnemyBullet.cs    # Enemy's projectiles
│   └── HealthPickup.cs   # Health drops
│
├── Extensions/           # Base classes for game objects
│   ├── Bullet.cs         # Base bullet class
│   └── PowerUp.cs        # Base powerup class
│
├── Interfaces/           # Contracts for behaviors
│   ├── IMovement.cs      # Movement behavior contract
│   ├── IDrawable.cs      # Rendering contract
│   ├── IUpdatable.cs     # Update loop contract
│   ├── ICollidable.cs    # Collision contract
│   ├── IMovable.cs       # Movement contract
│   └── IPhysicsObject.cs # Physics contract
│
├── Movements/            # Movement behaviors (Strategy Pattern)
│   ├── PatrolMovement.cs       # Left-right patrol
│   ├── ChaseMovement.cs        # Follow player
│   ├── ZigZagMovement.cs       # Diagonal bouncing
│   ├── VerticalMovement.cs     # Up-down movement
│   ├── JumpMovement.cs         # Periodic jumping
│   ├── RandomPatrolMovement.cs # Random wandering
│   ├── DroneMovement.cs        # Flying enemy movement
│   ├── KeyboardMovement.cs     # Basic keyboard control
│   └── ShootingPlayerMovement.cs # Player movement with jump
│
├── Systems/              # Game systems
│   ├── AnimationSystem.cs      # Frame-based animation
│   ├── PhysicsSystem.cs        # Gravity application
│   ├── CollisionSystem.cs      # Collision detection
│   ├── SoundManager.cs         # Audio playback
│   ├── GameStateManager.cs     # Runtime state tracking
│   ├── GameDataManager.cs      # Save/Load (file handling)
│   ├── ResourceLoader.cs       # Asset loading
│   └── ScrollingBackground.cs  # Background rendering
│
├── Levels/               # Game levels
│   ├── Level1Form.cs     # Level 1 - Easy
│   ├── Level2Form.cs     # Level 2 - Medium
│   └── Level3Form.cs     # Level 3 - Hard (Boss)
│
└── UI/                   # User interface
    ├── MainMenuForm.cs   # Main menu
    ├── SettingsForm.cs   # Settings screen
    └── UIRenderer.cs     # HUD rendering
```

### Inheritance Hierarchy:
```
GameObject (BASE CLASS)
    │
    ├── Player
    │      └── ShootingPlayer (your controllable character)
    │
    ├── Enemy
    │      └── ShootingEnemy (enemies you fight)
    │
    ├── Bullet
    │      ├── PlayerBullet
    │      └── EnemyBullet
    │
    ├── PowerUp
    │      └── HealthPickup
    │
    └── Platform
```

---

## 5. OOP CONCEPTS USED

### 5.1 INHERITANCE
**Definition:** Child class gets all properties and methods from parent class.

**Example in your code:**
```csharp
public class Player : GameObject { }        // Player inherits from GameObject
public class ShootingPlayer : Player { }    // ShootingPlayer inherits from Player
```

**Inheritance Chain:** `ShootingPlayer → Player → GameObject`
- ShootingPlayer has EVERYTHING from Player AND GameObject

---

### 5.2 POLYMORPHISM
**Definition:** Same method name, different behavior in different classes.

**Example in your code:**
```csharp
// In GameObject
public virtual void Draw(Graphics g) { /* draws image */ }

// In ShootingEnemy - DIFFERENT behavior
public override void Draw(Graphics g) { /* draws enemy + health bar */ }

// In ShootingPlayer - DIFFERENT behavior  
public override void Draw(Graphics g) { /* draws player + invincibility blink */ }
```

**Why it matters:** `game.Draw()` calls Draw() on all objects, but each draws differently!

---

### 5.3 ENCAPSULATION
**Definition:** Hiding internal details, exposing only what's needed.

**Example in your code:**
```csharp
public bool IsDead { get; private set; }  // Others can READ, only this class can WRITE
private float shootTimer = 0;              // Completely hidden from other classes
```

---

### 5.4 ABSTRACTION
**Definition:** Hiding complex implementation behind simple interface.

**Example in your code:**
```csharp
public interface IMovement
{
    void Move(GameObject obj, GameTime gameTime);
}
```
- User just calls `Movement.Move()` 
- Doesn't need to know HOW patrol/chase/zigzag works internally

---

### 5.5 INTERFACES
**Definition:** A contract that classes must follow.

**Your 6 Interfaces:**
| Interface | Contract | Who Implements |
|-----------|----------|----------------|
| IDrawable | Must have Draw() | GameObject |
| IUpdatable | Must have Update() | GameObject |
| IMovable | Must have Velocity | GameObject |
| ICollidable | Must have Bounds, OnCollision() | GameObject |
| IPhysicsObject | Must have HasPhysics, IsRigidBody | GameObject |
| IMovement | Must have Move() | All movement classes |

---

## 6. FILE-BY-FILE EXPLANATION

### 6.1 Program.cs - ENTRY POINT

**Purpose:** Starting point of the application.

**Key Components:**
```csharp
[STAThread]  // Required for Windows Forms (Single Thread Apartment)
static void Main()
{
    ApplicationConfiguration.Initialize();  // Setup Windows Forms
    Application.Run(new GameContext());     // Start app with custom context
}
```

**Why GameContext?**
- Normal: App closes when main form closes
- GameContext: App stays alive when switching forms (Menu → Level → Menu)
- Only closes when `Application.Exit()` is called

---

### 6.2 GameTime.cs - TIME TRACKING

**Purpose:** Track time between frames for smooth movement.

```csharp
public float DeltaTime { get; set; } = 0.016f;  // 16ms = 60 FPS
```

**Why DeltaTime?**
- Without: Fast computer = fast game, slow computer = slow game
- With: `position += speed * DeltaTime` = same speed on ALL computers

---

### 6.3 GameObject.cs - BASE CLASS (MOST IMPORTANT!)

**Purpose:** Parent class for ALL game objects.

**Properties:**
| Property | Type | Purpose |
|----------|------|---------|
| Tag | string | Identifies object type ("Player", "Enemy") |
| Position | PointF | X,Y location on screen |
| Size | SizeF | Width and Height |
| Velocity | PointF | Speed and direction (X=horizontal, Y=vertical) |
| IsActive | bool | If false, object is removed |
| HasPhysics | bool | If true, gravity affects it |
| IsRigidBody | bool | If true, doesn't move when hit (platforms) |
| Bounds | RectangleF | Collision box (auto-calculated) |
| Sprite | Image | Static image to display |
| Animation | AnimationSystem | For animated sprites |

**Key Methods:**
```csharp
public virtual void Update(GameTime gameTime)
{
    Position = new PointF(Position.X + Velocity.X, Position.Y + Velocity.Y);
    Animation?.Update(gameTime);
}

public virtual void Draw(Graphics g) { /* draws sprite or animation */ }

public virtual void OnCollision(GameObject other) { /* empty - children override */ }
```

**Why `virtual`?** Allows child classes to override with their own behavior.

---

### 6.4 Player.cs - PLAYER BASE CLASS

**Purpose:** Adds health, score, and movement to GameObject.

**New Properties:**
- `IMovement Movement` - Movement behavior (Strategy Pattern)
- `int Health` - Player's hit points (default 100)
- `int Score` - Points earned

**Key Override:**
```csharp
public override void Update(GameTime gameTime)
{
    Movement?.Move(this, gameTime);  // Execute movement behavior
    base.Update(gameTime);           // Call parent's Update
}
```

---

### 6.5 ShootingPlayer.cs - YOUR ACTUAL PLAYER

**Purpose:** The player you control in the game.

**Inheritance Chain:** `ShootingPlayer → Player → GameObject`

**Key Features:**
- Facing direction (Left, Right, Up)
- Shooting with cooldown (0.2 seconds between shots)
- Invincibility after taking damage (1.5 seconds)
- Multiple animations (idle, run, shoot, death)
- Event system for shooting (`OnShoot` event)

**Important Properties:**
```csharp
public FacingDirection Facing { get; set; }     // Which way player looks
public float ShootCooldown { get; set; } = 0.2f; // Time between shots
public bool IsDead { get; private set; }         // Can only be set internally
public bool IsInvincible => invincibilityTimer > 0; // Calculated property
```

**Event System:**
```csharp
public event Action<PointF, FacingDirection> OnShoot;

// When player shoots:
OnShoot?.Invoke(bulletPosition, direction);

// In level form - subscribe to event:
player.OnShoot += SpawnPlayerBullet;
```

---

### 6.6 Enemy.cs & ShootingEnemy.cs - ENEMIES

**Enemy.cs Purpose:** Base enemy class with movement.

**ShootingEnemy.cs Purpose:** Actual enemies in the game.

**Enemy Types:**
| Type | Health | Score | Behavior |
|------|--------|-------|----------|
| Human | 50 | 100 | Ground, chases player, shoots |
| Drone | 30 | 150 | Flying, patrols horizontally |
| Ship | 40 | 150 | Flying, patrols, faces downward |
| Boss | 400 | 1000 | Large, high HP, rapid fire |

**Key Features:**
- Health bar display (shows when damaged)
- Shooting at player when in range
- Death animation before removal
- Event when killed (`OnDeath` event)

---

### 6.7 Movement Classes (Strategy Pattern)

**Purpose:** Define HOW objects move. Can be swapped at runtime.

| Class | Behavior |
|-------|----------|
| PatrolMovement | Moves left-right between two X positions |
| ChaseMovement | Follows the player horizontally |
| ZigZagMovement | Bounces diagonally within bounds |
| VerticalMovement | Moves up-down between two Y positions |
| DroneMovement | Flying pattern with sine wave |
| RandomPatrolMovement | Random target positions |
| ShootingPlayerMovement | Keyboard control + jumping |

**How to use:**
```csharp
enemy.Movement = new PatrolMovement(0, 500, 2f);   // Patrol
enemy.Movement = new ChaseMovement(2f, 0, player); // Chase
```

---

### 6.8 PhysicsSystem.cs - GRAVITY

**Purpose:** Apply gravity to objects with `HasPhysics = true`.

```csharp
public void Apply(List<GameObject> objects)
{
    foreach (var obj in objects.Where(o => o.HasPhysics))
    {
        obj.Velocity = new PointF(
            obj.Velocity.X,
            obj.Velocity.Y + Gravity  // Add gravity to Y velocity
        );
    }
}
```

**Gravity = 0.5f** means objects accelerate downward by 0.5 pixels per frame.

---

### 6.9 CollisionSystem.cs - COLLISION DETECTION

**Purpose:** Detect when objects touch and notify them.

**How it works:**
1. Get all objects that implement `ICollidable`
2. Compare every pair of objects
3. Check if their `Bounds` rectangles overlap
4. If overlap: call `OnCollision()` on BOTH objects

```csharp
if (collidables[i].Bounds.IntersectsWith(collidables[j].Bounds))
{
    collidables[i].OnCollision((GameObject)collidables[j]);
    collidables[j].OnCollision((GameObject)collidables[i]);
}
```

**Rigid Body Handling:**
- If platform (IsRigidBody=true) collides with player
- Player gets pushed out, platform stays still

---

### 6.10 AnimationSystem.cs - FRAME ANIMATION

**Purpose:** Cycle through images to create animation.

```csharp
private List<Image> frames;    // List of images
private int currentFrame;       // Current frame index
private float timer;            // Time counter
private float speed;            // Time per frame

public void Update(GameTime gameTime)
{
    timer += gameTime.DeltaTime;
    if (timer > speed)
    {
        timer = 0;
        currentFrame++;
        if (currentFrame >= frames.Count)
            currentFrame = 0;  // Loop back to start
    }
}
```

---

### 6.11 SoundManager.cs - AUDIO

**Purpose:** Play music and sound effects.

**Key Methods:**
- `LoadSound(name, fileName)` - Load sound into library
- `PlayMusic(name, volume)` - Play background music (loops)
- `PlaySound(name, volume)` - Play sound effect (can overlap)
- `StopMusic()` - Stop background music

**Why separate Music and Sound?**
- Music: Only ONE plays at a time, loops continuously
- Sound: Multiple can play simultaneously (shooting, explosions)

---

### 6.12 GameDataManager.cs - FILE HANDLING (SAVE/LOAD)

**Purpose:** Save and load game data to/from file.

**Save Location:** `%LocalAppData%/SpaceDefender/gamedata.txt`

**File Format (Plain Text):**
```
HighScore=5000
MaxLevelUnlocked=2
MusicVolume=0.5
SfxVolume=1.0
PlayerName=Player
TotalEnemiesKilled=50
TotalPlayTime=3600
```

**Key Methods:**
```csharp
public static void Save()  // Write data to file
{
    using (StreamWriter writer = new StreamWriter(SaveFile))
    {
        writer.WriteLine("HighScore=" + CurrentData.HighScore);
        // ... more lines
    }
}

public static void Load()  // Read data from file
{
    string[] lines = File.ReadAllLines(SaveFile);
    foreach (string line in lines)
    {
        string[] parts = line.Split('=');
        // Parse key-value pairs
    }
}
```

---

### 6.13 GameStateManager.cs - RUNTIME STATE

**Purpose:** Track game state during gameplay (not saved to file).

**Properties:**
- `CurrentLevel` - Which level (1, 2, 3)
- `CurrentScore` - Points earned this session
- `PlayerHealth` - Current health (0-100)

**Difference from GameDataManager:**
- GameStateManager = temporary (resets when game closes)
- GameDataManager = permanent (saved to file)

---

### 6.14 ShootingGameFormBase.cs - LEVEL BASE CLASS

**Purpose:** Base class for all level forms. Contains shared game logic.

**Key Components:**
- Game loop (60 FPS timer)
- Player creation
- Enemy spawning system
- Physics and collision systems
- UI rendering
- Pause functionality

**Abstract Methods (levels MUST implement):**
```csharp
public abstract int LevelNumber { get; }
protected abstract Image BackgroundImage { get; }
protected abstract void SetupEnemies();
protected abstract void CreatePlatforms();
protected abstract void OnLevelComplete();
```

**Game Loop:**
```csharp
private void GameLoop(object sender, EventArgs e)
{
    HandleInput();           // Check for pause
    if (!isPaused)
        UpdateGame();        // Update all systems
    Invalidate();            // Trigger repaint
}

private void UpdateGame()
{
    game.Update(gameTime);   // Update all objects
    physics.Apply(game.Objects);  // Apply gravity
    collisions.Check(game.Objects);  // Check collisions
    game.Cleanup();          // Remove dead objects
}
```

---

### 6.15 Level Forms (Level1Form, Level2Form, Level3Form)

**Purpose:** Define specific level content.

**Each level defines:**
- `LevelNumber` - Which level (1, 2, 3)
- `BackgroundImage` - Level background
- `CreatePlatforms()` - Platform layout
- `SetupEnemies()` - Enemy spawn queue
- `OnLevelComplete()` - What happens when done

**Example (Level1Form):**
```csharp
protected override void SetupEnemies()
{
    totalEnemies = 9;
    enemiesRemaining = 9;
    
    // Queue enemies to spawn over time
    spawnQueue.Enqueue(() => SpawnHumanEnemy(x, y));
    spawnQueue.Enqueue(() => SpawnDrone(x, y));
}
```

---

## 7. DESIGN PATTERNS

### 7.1 STRATEGY PATTERN (Movement System)

**What:** Define a family of algorithms, encapsulate each one, make them interchangeable.

**Where:** IMovement interface and movement classes.

**How it works:**
```csharp
// Interface defines the contract
public interface IMovement
{
    void Move(GameObject obj, GameTime gameTime);
}

// Different strategies implement it
public class PatrolMovement : IMovement { /* patrol logic */ }
public class ChaseMovement : IMovement { /* chase logic */ }

// Usage - swap behaviors easily
enemy.Movement = new PatrolMovement(0, 500, 2f);
enemy.Movement = new ChaseMovement(2f, 0, player);  // Change behavior!
```

**Benefits:**
- Add new movement types without changing existing code
- Change behavior at runtime
- Each movement class has single responsibility

---

### 7.2 OBSERVER PATTERN (Events)

**What:** Object notifies other objects when something happens.

**Where:** `OnShoot` event in ShootingPlayer, `OnDeath` event in ShootingEnemy.

**How it works:**
```csharp
// Publisher (ShootingPlayer)
public event Action<PointF, FacingDirection> OnShoot;

// Fire event when shooting
OnShoot?.Invoke(position, direction);

// Subscriber (ShootingGameFormBase)
player.OnShoot += SpawnPlayerBullet;  // Subscribe

// SpawnPlayerBullet is called whenever player shoots
```

**Benefits:**
- Loose coupling - ShootingPlayer doesn't know about bullets
- Multiple subscribers can listen to same event

---

### 7.3 TEMPLATE METHOD PATTERN (Level Forms)

**What:** Define skeleton of algorithm, let subclasses fill in specific steps.

**Where:** ShootingGameFormBase and Level forms.

**How it works:**
```csharp
// Base class defines the template
public abstract class ShootingGameFormBase : Form
{
    // Template method - defines the order
    protected override void OnShown(EventArgs e)
    {
        CreatePlayer();       // Same for all levels
        CreatePlatforms();    // ABSTRACT - each level defines
        SetupEnemies();       // ABSTRACT - each level defines
        gameTimer.Start();    // Same for all levels
    }
    
    // Abstract methods - subclasses MUST implement
    protected abstract void CreatePlatforms();
    protected abstract void SetupEnemies();
}

// Subclass fills in the blanks
public class Level1Form : ShootingGameFormBase
{
    protected override void CreatePlatforms() { /* Level 1 platforms */ }
    protected override void SetupEnemies() { /* Level 1 enemies */ }
}
```

---

## 8. GAME SYSTEMS

### 8.1 GAME LOOP (60 FPS)

```
Every 16 milliseconds (60 times per second):
    │
    ├── 1. HandleInput() - Check for pause (ESC key)
    │
    ├── 2. UpdateGame()
    │       ├── Process spawn queue (spawn enemies over time)
    │       ├── game.Update() - Update all objects
    │       ├── physics.Apply() - Apply gravity
    │       ├── collisions.Check() - Detect collisions
    │       └── game.Cleanup() - Remove dead objects
    │
    └── 3. Invalidate() - Trigger OnPaint() to redraw
            └── OnPaint()
                ├── Draw background
                ├── game.Draw() - Draw all objects
                └── Draw UI (health, score, etc.)
```

### 8.2 COLLISION FLOW

```
CollisionSystem.Check()
    │
    ├── For each pair of objects:
    │       │
    │       ├── Check: Do Bounds rectangles overlap?
    │       │
    │       └── If YES:
    │               ├── Call objectA.OnCollision(objectB)
    │               └── Call objectB.OnCollision(objectA)
    │
    └── Each object decides what to do:
            ├── Player hit by EnemyBullet → TakeDamage()
            ├── Enemy hit by PlayerBullet → TakeDamage()
            ├── Player touches HealthPickup → Heal()
            └── Bullet hits anything → IsActive = false
```

### 8.3 ENEMY SPAWN SYSTEM

```csharp
// Queue holds spawn actions
Queue<Action> spawnQueue = new Queue<Action>();

// Setup adds enemies to queue
spawnQueue.Enqueue(() => SpawnHumanEnemy(x, y));
spawnQueue.Enqueue(() => SpawnDrone(x, y));

// Every frame, check if time to spawn
spawnTimer += 0.016f;
if (spawnTimer >= spawnDelay)
{
    spawnTimer = 0;
    spawnQueue.Dequeue()();  // Execute next spawn action
}
```

---

## 9. COMMON EVALUATION QUESTIONS

### Q1: What is inheritance? Give an example from your code.
**Answer:** Inheritance is when a child class gets all properties and methods from a parent class.
**Example:** `ShootingPlayer : Player : GameObject` - ShootingPlayer inherits from Player, which inherits from GameObject. ShootingPlayer has Position, Size, Velocity (from GameObject), Health, Score (from Player), and its own Facing, ShootCooldown.

---

### Q2: What is polymorphism? Give an example from your code.
**Answer:** Polymorphism means same method name, different behavior in different classes.
**Example:** `Draw()` method - GameObject draws a simple image, ShootingPlayer draws with invincibility blink effect, ShootingEnemy draws with health bar. When `game.Draw()` is called, each object draws differently.

---

### Q3: What is an interface? Why use them?
**Answer:** An interface is a contract that says "any class implementing me MUST have these methods."
**Why:** Systems can work with ANY object that implements the interface. PhysicsSystem works with anything that has `IPhysicsObject`, without knowing the specific type.

---

### Q4: What is the Strategy Pattern? Where is it used?
**Answer:** Strategy Pattern lets you swap algorithms at runtime.
**Where:** Movement system - `IMovement` interface with PatrolMovement, ChaseMovement, etc. You can change enemy behavior by just changing `enemy.Movement = new ChaseMovement(...)`.

---

### Q5: What does `virtual` and `override` mean?
**Answer:** 
- `virtual` in parent = "Child classes CAN replace this method"
- `override` in child = "I AM replacing the parent's method"

---

### Q6: What is `base.Update(gameTime)`?
**Answer:** Calls the parent class's Update method. Used to keep parent functionality while adding new behavior.

---

### Q7: Why use DeltaTime?
**Answer:** To make movement consistent on all computers. Without it, fast computers run faster. With `position += speed * DeltaTime`, movement is the same regardless of frame rate.

---

### Q8: What is the difference between `Sprite` and `Animation`?
**Answer:** 
- Sprite = single static image
- Animation = cycles through multiple images to create movement

---

### Q9: How does collision detection work?
**Answer:** CollisionSystem checks if `Bounds` rectangles of two objects overlap using `IntersectsWith()`. If they overlap, it calls `OnCollision()` on both objects.

---

### Q10: What is `private set` in `public bool IsDead { get; private set; }`?
**Answer:** Other classes can READ the value, but only THIS class can CHANGE it. Protects the property from external modification.

---

### Q11: What is an event? Why use it?
**Answer:** An event notifies other classes when something happens. Used for loose coupling - ShootingPlayer fires `OnShoot` event, doesn't need to know about bullets. ShootingGameFormBase subscribes and creates bullets.

---

### Q12: What is the game loop?
**Answer:** Code that runs 60 times per second:
1. Update all objects (movement, physics)
2. Check collisions
3. Remove dead objects
4. Draw everything

---

### Q13: Why use `GameContext` instead of `Application.Run(new MainMenuForm())`?
**Answer:** To keep the app running when switching forms. Without GameContext, closing MainMenuForm would close the entire app.

---

### Q14: What is file handling in your project?
**Answer:** GameDataManager saves/loads game data to `%LocalAppData%/SpaceDefender/gamedata.txt` using StreamWriter/StreamReader. Stores high score, unlocked levels, volume settings.

---

### Q15: What is the difference between GameStateManager and GameDataManager?
**Answer:**
- GameStateManager = temporary runtime state (current score, health) - lost when game closes
- GameDataManager = permanent saved data (high score, settings) - saved to file

---

## 10. QUICK REFERENCE

### Key Classes:
| Class | Purpose |
|-------|---------|
| GameObject | Base class for all game objects |
| ShootingPlayer | Player you control |
| ShootingEnemy | Enemies you fight |
| Game | Manages all objects (add, update, draw, remove) |
| PhysicsSystem | Applies gravity |
| CollisionSystem | Detects collisions |
| AnimationSystem | Frame-based animation |
| SoundManager | Audio playback |
| GameDataManager | Save/Load to file |

### Key Keywords:
| Keyword | Meaning |
|---------|---------|
| virtual | Child can override |
| override | Replaces parent method |
| abstract | Must be implemented by child |
| static | No instance needed |
| const | Value never changes |
| base | Refers to parent class |
| event | Notification system |
| interface | Contract/promise |

---

*Documentation created for Space Defender - Final Project Evaluation*
