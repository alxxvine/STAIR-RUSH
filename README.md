# Stair Rush

A simple, fast-paced game about climbing a treacherous tower, one risky jump at a time.

## Core Mechanics
- Procedurally generated infinite stairs.
- Single jump (LMB) to advance one step.
- Double jump (RMB) to skip a step, moving further and faster.
- Dynamic camera that follows the player's ascent.
- Physics-based jumping for a "game feel" approach.

## Scoring
- In Level, HUD shows `SCORE {n}` and starts as `SCORE 0` on scene load.
- Current run steps are tracked by `StepCounter` (attach to Player). Counts only on actual landings on stairs.
- The very first valid landing is ignored (starting platform doesn’t count).
- Double-count protection: the same stair cannot be counted twice.
- High score is saved automatically in `PlayerPrefs` under key `HighScore_Steps`.
- In Menu, `HighScoreText` displays `BEST {n}` (space between label and number).

## Scene Setup
### Level
- Attach `StepCounter` to Player. Optionally assign a HUD `TMP_Text` to `stepsText`.
- Ensure stairs have `StairController` so landings count.

### Menu
- Add a `TMP_Text` and attach `HighScoreText` (same GameObject). Optionally set `prefix` (default `BEST `).
- Ensure both scenes (`Level`, `Menu`) are included in Build Settings.

## Scripts Added
- `Assets/Scripts/StepCounter.cs`: counts landings, saves best score, updates HUD.
- `Assets/Scripts/HighScoreText.cs`: reads and displays high score in Menu.
