# Stair Rush

A simple, fast-paced game about climbing a treacherous tower, one risky jump at a time.

## Core Mechanics
- Procedurally generated infinite stairs.
- Single jump (LMB) to advance one step.
- Double jump (RMB) to skip a step, moving further and faster.
- Dynamic camera that follows the player's ascent.
- Physics-based jumping for a "game feel" approach.

## Scoring
- Current run steps are tracked in `StepCounter` (attach to Player).
- High score is saved automatically in `PlayerPrefs` under key `HighScore_Steps`.
- In Level, HUD shows only the current step count.
- In Menu, `HighScoreText` displays the saved record.

## Scene Setup
### Level
- Attach `StepCounter` to Player. Optionally assign a HUD `TMP_Text` to `stepsText`.
- Ensure stairs have `StairController` so landings count.

### Menu
- Add a `TMP_Text` and attach `HighScoreText` (same GameObject). Optionally set `prefix`.
- Ensure both scenes (`Level`, `Menu`) are included in Build Settings.

## Scripts Added
- `Assets/Scripts/StepCounter.cs`: counts landings, saves best score, updates HUD.
- `Assets/Scripts/HighScoreText.cs`: reads and displays high score in Menu.
