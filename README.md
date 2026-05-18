# Unity Sudoku

A Unity Sudoku project being built as the next version after the completed Python Sudoku prototype.

The Python version proved the core concepts first: generated Sudoku boards, solution validation, clue hiding, difficulty settings, notes, timer, move count, pause, theme support, pytest coverage, and batch puzzle testing. This Unity version now uses that work as the reference point while rebuilding the game for desktop, mobile, tablet, and WebGL targets.

## Project Status

This project is currently in early Unity development.

Completed so far:

- Unity project structure created
- Main menu scene created
- Gameplay scene created
- Game over scene reserved for later
- UI Toolkit is being used for UI
- New Input System is the default input direction for the project
- Main menu UI created
- Difficulty selection added
- Main menu scene flow to gameplay works
- Gameplay UI shell created
- Sudoku board placeholder created
- Full solved Sudoku board generation ported to C#
- Sudoku validation added in C#
- Puzzle clue hiding added by difficulty
- EditMode test setup created and working
- Runtime assembly definition added
- EditMode test assembly definition added
- Tests are passing for the current core logic

## Planned Platforms

The Unity version is being planned for:

- Windows desktop
- macOS desktop
- Android
- WebGL

Mobile, tablet, and WebGL testing will be handled periodically during development rather than after every small change. Most of the early work is logic and game structure, so the first focus is correctness and scene flow.

## Current Scenes

The project currently uses three scenes:

- `mainMenu`
- `gamePlay`
- `gameOver`

### mainMenu

The main menu currently includes:

- Title
- Difficulty selection
- Play Level button
- About button
- Quit button

Difficulty options:

- Easy
- Medium
- Hard
- Godlike

The selected difficulty is stored and passed to the gameplay scene through `GameSettings`.

### gamePlay

The gameplay scene currently includes:

- Title/header area
- Difficulty display
- Timer placeholder
- Move count placeholder
- 9x9 Sudoku board
- Number pad placeholder
- Notes, Clear, Pause, and Menu buttons

The scene currently generates a solved Sudoku board, hides clues based on the selected difficulty, and renders the visible starting clues.

### gameOver

The game over scene exists as part of the planned scene flow but has not been built out yet.

## Current Difficulty Settings

The Unity version uses a digital answer-key model. The generated solution is the accepted solution for the round.

Current visible clue counts:

- Easy: 30 clues
- Medium: 22 clues
- Hard: 15 clues
- Godlike: 5 clues

These values are expected to be adjusted as the game develops.

## Digital Sudoku Rule

This project is not trying to behave exactly like a paperback Sudoku puzzle.

The generated solution is the authority. Lower-clue modes may technically allow more than one valid Sudoku completion, but the game validates player input against the generated answer key for that round.

That rule is intentional for the digital game version.

## Current Folder Structure

Current main folders:

    Assets/
      Scenes/
      Scripts/
      GameUI/
      Visuals/
      Audio/
      Tests/

Current important scripts and UI files include:

    Assets/GameUI/MainMenu/MainMenu.uxml
    Assets/GameUI/MainMenu/MainMenu.uss
    Assets/GameUI/GamePlay/GamePlay.uxml
    Assets/GameUI/GamePlay/GamePlay.uss

    Assets/Scripts/Core/DifficultyLevel.cs
    Assets/Scripts/Core/GameSettings.cs
    Assets/Scripts/Core/Sudoku/SudokuGenerator.cs
    Assets/Scripts/Core/Sudoku/SudokuValidator.cs
    Assets/Scripts/Core/Sudoku/SudokuPuzzle.cs
    Assets/Scripts/SceneFlow/SceneNames.cs
    Assets/Scripts/UI/MainMenuController.cs
    Assets/Scripts/UI/GamePlayController.cs

    Assets/Scripts/UnitySudoku.Runtime.asmdef
    Assets/Tests/EditMode/UnitySudoku.EditModeTests.asmdef

## Testing

Unity EditMode tests have been added and are passing.

The current test setup includes:

- Runtime assembly definition for game scripts
- EditMode test assembly definition
- NUnit-based Unity EditMode tests

Current test coverage includes:

- Sudoku solution generation returns a 9x9 board
- Generated Sudoku solutions are valid
- Same seed creates the same solved board
- Multiple generated boards validate successfully
- Validator rejects duplicate values in rows
- Validator rejects duplicate values in columns
- Validator rejects duplicate values in 3x3 boxes
- Validator rejects invalid values such as 0 and 10
- Validator rejects null boards
- Difficulty clue counts match expected values
- Puzzle builder uses the requested visible clue count
- Puzzle builder keeps the generated solution reference
- Puzzle builder creates deterministic visible clue layouts when seeded
- Puzzle builder rejects null solutions
- Puzzle builder rejects invalid visible clue counts
- Difficulty clue counts can create matching puzzle boards

Current expected test count:

    22 passing EditMode tests

## Running Tests

Open Unity Test Runner:

    Window > General > Test Runner

Then select:

    EditMode

Run all EditMode tests.

Do not use Player tests for this stage of development. The current tests are pure logic tests and belong in EditMode.

## Current Development Approach

The Unity version is being built in small controlled steps.

Completed steps:

1. Main menu UI shell
2. Difficulty selection
3. Scene flow to gameplay
4. Gameplay UI shell
5. Solved board generation
6. Solved board validation
7. EditMode test setup
8. Difficulty-based clue hiding

Next planned steps:

1. Cell selection
2. Number input
3. Answer-key validation
4. Clear cell behavior
5. Move count
6. Timer
7. Notes mode
8. Pause mode
9. Game completion detection
10. Game over scene
11. Mobile/tablet/WebGL layout pass

## Notes

This project is being developed as a Unity version of a previously completed Python Sudoku prototype. The Python version acts as the reference implementation for puzzle logic and testing philosophy.

The current Unity priority is to build a reliable, tested core before spending too much time on visual polish.
