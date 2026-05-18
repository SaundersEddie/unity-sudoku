# Unity Sudoku

Unity Sudoku is a cross-platform Sudoku game prototype built in Unity using UI Toolkit and the New Input System project direction.

This project follows the Python Sudoku prototype, which was used to prove the core Sudoku generation and validation concepts before building a Unity version.

## Current Status

The Unity version is now a playable prototype.

Current supported targets being planned around:

- Windows desktop
- macOS desktop
- Android
- WebGL

The project is being built logic-first, with mobile/tablet/web testing planned during development rather than after the entire game is complete.

## Scenes

The project uses three scenes:

    mainMenu
    gamePlay
    gameOver

### mainMenu

Current features:

- Title screen
- Difficulty selection
- Play Level button
- About panel
- Quit button
- Scene transition into gameplay

Difficulty options:

- Easy: 30 starting clues
- Medium: 22 starting clues
- Hard: 15 starting clues
- Godlike: 5 starting clues

### gamePlay

Current features:

- Sudoku board generation
- Starting clue hiding based on selected difficulty
- Internal generated solution used as the answer key
- Cell selection
- Number entry using UI buttons
- Keyboard input for desktop
- Correct/wrong answer validation
- Clear selected cell
- Move count
- Timer
- Notes mode
- Pause/resume
- Board hidden while paused
- Light/dark gameplay theme toggle
- Row, column, box, and matching-number highlights
- Completed-number button graying
- Completed-number input guard
- Completion detection
- Scene transition to gameOver

### gameOver

Current features:

- Puzzle complete screen
- Difficulty result
- Final time
- Final move count
- Play Again button
- Main Menu button

## Digital Sudoku Rule

Unity Sudoku is a digital Sudoku game built from a generated answer key.

Each round generates a complete valid Sudoku solution. The game then displays a limited number of starting clues depending on difficulty. Player answers are validated against the generated solution.

Some low-clue modes may technically allow more than one valid Sudoku completion if treated like a paper puzzle, but this game accepts only the generated solution for that round. That is intentional for this digital version.

## Project Structure

Current main folders:

    Assets/
      Scenes/
      Scripts/
      GameUI/
      Visuals/
      Audio/
      Tests/

Core script areas:

    Assets/Scripts/Core/
    Assets/Scripts/Core/Sudoku/
    Assets/Scripts/SceneFlow/
    Assets/Scripts/UI/

UI Toolkit files:

    Assets/GameUI/MainMenu/
    Assets/GameUI/GamePlay/
    Assets/GameUI/GameOver/

Tests:

    Assets/Tests/EditMode/

## Current Core Classes

### DifficultyLevel

Defines the available difficulty levels:

- Easy
- Medium
- Hard
- Godlike

### GameSettings

Stores shared session data, including:

- selected difficulty
- gameplay theme setting
- final completed difficulty
- final move count
- final elapsed time
- visible clue count per difficulty

### SudokuGenerator

Generates complete valid 9x9 Sudoku solution boards.

### SudokuValidator

Validates completed Sudoku solution boards by checking:

- rows
- columns
- 3x3 boxes
- valid values from 1 through 9

### SudokuPuzzleBuilder

Creates puzzle clue maps from completed solutions.

It controls which cells are visible at the start of a round based on the selected difficulty.

### SudokuGameState

Tracks gameplay state and rules, including:

- given cells
- player-entered values
- notes
- move count
- correct/wrong value checks
- completion detection

## Input

Current gameplay input supports:

- mouse/touch cell selection
- on-screen number buttons
- keyboard number input
- Backspace/Delete to clear
- N to toggle notes mode
- P or Escape to pause/resume

Keyboard support is intended mainly for desktop and WebGL play.

## Testing

The project includes Unity EditMode tests.

Current test coverage includes:

- Sudoku solution generation returns a 9x9 board
- Generated solutions are valid Sudoku boards
- Seeded generation can produce deterministic boards
- Multiple generated boards validate successfully
- Validator rejects row duplicates
- Validator rejects column duplicates
- Validator rejects 3x3 box duplicates
- Validator rejects invalid values
- Validator rejects null boards
- Difficulty clue counts are correct
- Puzzle builder uses requested clue counts
- Puzzle builder rejects invalid input
- Puzzle visibility can be reproduced with a seed
- Game state rejects invalid construction
- Given cells cannot be edited
- Editable cells accept values
- Same value does not increase move count twice
- Changed values increase move count
- Clear behavior is tracked correctly
- Correct and wrong values are detected
- Completion detection works
- Notes can be added and removed
- Notes do not count as moves
- Notes are rejected on given cells and filled cells
- Final values clear notes

Current expected test count at this checkpoint:

    46 passing EditMode tests

## Current Development Notes

The Unity version is intentionally following the same disciplined approach as the Python prototype:

1. Build the logic.
2. Test the logic.
3. Wire the UI.
4. Manually validate scene behavior.
5. Push stable checkpoints.

This keeps the project from turning into a giant UI-driven mess where gameplay rules are hard to test.

## Known Future Work

Likely next steps:

- Full game-wide theme support across mainMenu and gameOver
- Responsive layout pass for desktop, Android, tablet, and WebGL
- Build testing on Windows/macOS/WebGL/Android
- Mobile safe-area checks
- Audio feedback
- Visual polish
- Better gameOver presentation
- Optional saved preferences using PlayerPrefs

## Current Checkpoint

The project currently has a playable Unity Sudoku loop:

    mainMenu -> gamePlay -> gameOver

The core gameplay is working and tested, with the Python version serving as the reference implementation for logic and feature behavior.
