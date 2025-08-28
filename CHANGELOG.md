# Changelog

This document tracks the development iterations of the "Stair Rush" project.

---

### Iteration 1: Core Mechanics & Setup (Current)
*   **Player Controller (`PlayerJump.cs`):**
    *   Implemented a simple, physics-based jump using `Rigidbody2D`.
    *   Jump is triggered by Left Mouse Button (LMB).
*   **Level Movement (`LevelManager.cs`):**
    *   Created a system to move the entire level (a container of stairs) in response to the player's jump, creating an illusion of forward movement.
    *   Movement is handled via a subscription to the `OnPlayerJumped` event.
*   **Procedural Generation (`StairSpawner.cs`):**
    *   Implemented initial procedural generation to create a starting set of stairs.
    *   Added logic to continuously spawn new stairs ahead and despawn old stairs behind the player.
*   **Camera (`CameraFollow.cs`):**
    *   Created a smooth camera that follows the player's vertical movement (`Y-axis`) while remaining stationary horizontally.
    *   Implemented `Rigidbody2D` interpolation to fix camera jitter.
*   **Physics & Feel Tuning:**
    *   Separated parameters for jump height (`Jump Force`, `Gravity Scale`) and level movement (`Step Move Distance`, `Move Duration`) to allow for fine-tuning of game feel.

---

### Iteration 2: Advanced Mechanics & Gameplay Loop
*   **Double Jump:**
    *   Implemented a "skip-step" jump on Right Mouse Button (RMB).
    *   Added separate, configurable physics parameters for the double jump (`doubleJumpForce`, `doubleJumpMoveDistance`, etc.).
    *   `StairSpawner` now spawns two stairs on a double jump.
*   **Project Structure:**
    *   Organized the scene hierarchy into logical groups (`GAME SYSTEMS`, `ENVIRONMENT`, etc.).
    *   Set up a Git repository with a Unity-specific `.gitignore` for version control.

---

### Next Steps (Planned)

*   **Variative Stair System:**
    *   Implement different types of stairs (Normal, Broken, Coin, Fake Coin).
    *   Create a spawner system that can randomly select from a list of stair prefabs.
*   **Game Loop:**
    *   Add win/loss conditions (e.g., landing on a broken stair).
    *   Implement a scoring system.
    *   Create a simple UI to display score and game-over messages.
