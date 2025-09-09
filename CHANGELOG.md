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
    *   Added separate, configurable physics parameters for the double jump.
    *   `StairSpawner` and `LevelManager` now handle both jump types correctly.
*   **Variative Stair System:**
    *   Created a flexible system to spawn different stair types (`Normal`, `Broken`, `Coin`, etc.).
    *   Logic is split between `StairSpawner` (mechanics) and a new `StairSelector` (content).
    *   `StairSelector` supports weighted spawn chances for each stair type.
    *   Added a "fairness" rule (`Prevent Immediate Repeat`) to avoid unwinnable situations.
*   **Project Structure:**
    *   Organized the scene hierarchy into logical groups (`GAME SYSTEMS`, `ENVIRONMENT`, etc.).
    *   Set up a Git repository with a Unity-specific `.gitignore` for version control.
    *   Uploaded the project to GitHub.

---

### Iteration 3: Scoring & UI Hooks
*   **Scoring:**
    *   Added `StepCounter` to count steps on landings and persist a high score via `PlayerPrefs` (`HighScore_Steps`).
    *   Level HUD now shows only current steps; high score updates silently in the background.
*   **Menu Record Display:**
    *   Added `HighScoreText` to read and display the saved record on the Menu scene.
*   **Setup Notes:**
    *   Attach `StepCounter` to Player in `Level`. Attach `HighScoreText` to a `TMP_Text` in `Menu`.
    *   Ensure stairs have `StairController` for landings to count.

---

### Iteration 4: Scoring polish & UI labels
*   **HUD label:** In-Level HUD now shows `SCORE {n}` and always starts at `SCORE 0` on level load.
*   **Menu label:** Menu shows `BEST {n}` (with a space) by default; customizable via `HighScoreText.prefix`.
*   **Landing rules:** Count only on actual landings (downward or near-zero vertical speed) on objects with `StairController` and an upward contact normal.
*   **First platform:** The very first valid landing is ignored to prevent starting platform from counting.
*   **Double-count protection:** The same stair instance will not increment the counter twice.
*   **Scene reset:** Score UI is reset on scene load and `OnEnable` to guarantee correct label at start.

---

### Iteration 5: Startup input gate & double jump scoring
*   **Startup gate:** Added `StartupJumpGate` to disable player jumping until initial stairs finish appearing. No changes to existing scripts required.
*   **Double jump scoring:** After a double jump (RMB), the next valid landing now awards `+2` instead of `+1`.

---

### Iteration 6: StepCounter event API & skin reward system
*   **StepCounter API:**
    *   Добавлено публичное свойство `CurrentSteps` для получения текущего количества шагов.
    *   Добавлено событие `OnStepsChanged(int steps)`, вызывающееся при каждом изменении количества шагов.
*   **StepCountdownSkinReward:**
    *   Новый скрипт для наград за шаги: подписывается на StepCounter, показывает сколько осталось до награды, меняет скин игрока при достижении нужного количества шагов.
    *   В инспекторе: TMP_Text для UI, SpriteRenderer игрока, список наград (шаги + спрайт).

### Next Steps (Planned)

*   **Variative Stair System:**
    *   Implement different types of stairs (Normal, Broken, Coin, Fake Coin).
    *   Create a spawner system that can randomly select from a list of stair prefabs.
*   **Game Loop:**
    *   Add win/loss conditions (e.g., landing on a broken stair).
    *   Implement a scoring system.
    *   Create a simple UI to display score and game-over messages.
