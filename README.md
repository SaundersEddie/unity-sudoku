# Unity Sudoku

A playable Unity 6 Sudoku game built with **UI Toolkit** and the **New Input System**.

This project started as the Unity version of a Sudoku idea first proven in Python. The Unity build now has the core game loop working: menu, difficulty selection, playable Sudoku board, notes, timer, pause behavior, theme switching, completion detection, and a game-over screen.

## Current Status

**Working prototype / playable game.**

The project is functional in Unity and ready for continued polish, platform testing, and eventual release-style packaging.

Current focus areas:

- Keep the gameplay loop stable
- Continue responsive layout testing
- Polish desktop, WebGL, mobile, and tablet presentation
- Preserve test coverage while features are added
- Commit before major UI Toolkit layout changes — because USS can absolutely go sideways if you let it

## Project Goals

Unity Sudoku is intended to be a clean, tested, multi-platform Sudoku game that can grow from prototype into a polished portfolio/game-site project.

Target platforms:

- Windows desktop
- macOS desktop
- WebGL
- Android / mobile
- Tablet layouts

## Unity Version

Built with:

```text
Unity 6000.3.9f1
```

## Main Technology

- Unity 6
- C#
- UI Toolkit
- UXML
- USS
- `UIDocument`
- New Input System
- Unity EditMode tests

Key package references include:

- `com.unity.inputsystem`
- `com.unity.test-framework`
- `com.unity.modules.uielements`

## Scene Flow

The project uses three main scenes:

```text
mainMenu -> gamePlay -> gameOver
```

Scene files:

```text
Assets/Scenes/mainMenu.unity
Assets/Scenes/gamePlay.unity
Assets/Scenes/gameOver.unity
```

### Main Menu

The main menu handles difficulty selection and starts the game.

### Gameplay

The gameplay scene contains the Sudoku board, number controls, notes mode, timer, pause behavior, theme toggle, and completion logic.

### Game Over

The game-over scene displays the finished result and lets the player replay or return to the main menu.

## Difficulty Levels

Difficulty controls how many starting clues are shown.

| Difficulty | Starting Clues |
| ---------- | -------------: |
| Easy       |             30 |
| Medium     |             22 |
| Hard       |             15 |
| Godlike    |              5 |

The game uses a generated answer key as the authoritative solution. This keeps digital validation straightforward, even when very low-clue boards could theoretically allow multiple Sudoku-valid completions.

## Implemented Gameplay Features

- Sudoku solution generation
- Difficulty-based clue hiding
- Player cell selection
- Number entry
- Correct / wrong answer styling
- Clear selected cell
- Move count
- Timer
- Notes mode
- Notes start the timer but do not count as moves
- Pause mode
- Board hidden while paused
- Light / dark theme toggle in gameplay
- Completed-number button graying
- Completed-number input guard
- Keyboard input
- Game completion detection
- Game-over result screen

## Keyboard Controls

| Key                    | Action                      |
| ---------------------- | --------------------------- |
| `1`-`9`                | Enter number or toggle note |
| Numpad `1`-`9`         | Enter number or toggle note |
| `Backspace` / `Delete` | Clear selected cell         |
| `N`                    | Toggle notes                |
| `P` / `Escape`         | Pause / resume              |

## Testing

The project includes Unity EditMode tests.

Current test checkpoint:

```text
52 passing EditMode tests
```

Test coverage includes:

- Sudoku solution generation
- Sudoku solution validation
- Difficulty clue counts
- Puzzle clue visibility
- Game state behavior
- Given-cell protection
- Player value entry
- Move count behavior
- Correct / wrong value detection
- Completion detection
- Notes behavior
- Completed-number detection

## Running the Project

1. Clone the repository.
2. Open the project in Unity `6000.3.9f1` or a compatible Unity 6 editor.
3. Open the `mainMenu` scene.
4. Press Play.
5. Select a difficulty and play.

## Running Tests

Use Unity Test Runner:

```text
Window -> General -> Test Runner
```

Then run the EditMode test suite.

Expected checkpoint:

```text
52 passing EditMode tests
```

## WebGL Testing Notes

For WebGL builds, test through a local server instead of opening `index.html` directly.

Example:

```bash
python -m http.server 8000
```

Then open:

```text
http://localhost:8000
```

Useful WebGL checks:

- Player Settings -> WebGL -> Resolution and Presentation
- Canvas width / height
- WebGL template
- Compression settings
- Browser scaling behavior
- UI Toolkit Panel Settings

## UI Toolkit Notes

The UI is built with scene-based UI Toolkit documents and panel settings.

The project uses:

- UXML for layout structure
- USS for styling
- `UIDocument` for scene UI
- Panel Settings assets for rendering behavior

Panel assets used during development:

- `MainMenuPanelAsset`
- `GamePlayPanelAsset`
- `GameOverPanelAsset`

## Assembly Definition Notes

The project uses assembly definitions so runtime code and EditMode tests are separated cleanly.

General structure:

- Runtime scripts live in the runtime assembly
- EditMode tests live in a dedicated test assembly
- The test assembly references the runtime assembly

This lets Unity discover and run the tests properly.

## Development Rules

Before broad layout work, especially full USS replacements:

```text
Commit first. Experiment second.
```

Unity UI Toolkit changes can cascade quickly. A clean commit before a large layout pass saves pain, swearing, and unnecessary archaeology.

Recommended workflow:

1. Commit a working state.
2. Make one focused change.
3. Test in the Unity Editor.
4. Test at target resolutions.
5. Re-run EditMode tests.
6. Commit again once stable.

## Suggested Next Steps

Good next tasks:

1. Confirm the latest gameplay layout across Editor, desktop build, WebGL, mobile simulator, and tablet sizes.
2. Extend light/dark theme support to main menu and game-over screens if not already complete.
3. Add final visual polish.
4. Add screenshots or GIFs to this README.
5. Add platform-specific build notes.
6. Create a release checklist.
7. Consider a WebGL demo link once hosted.

## Repository

```text
https://github.com/SaundersEddie/unity-sudoku
```

## Project Purpose

This project shows a tested Unity implementation of Sudoku with a real game loop, difficulty modes, input handling, UI Toolkit screens, and automated EditMode coverage.

It is both a playable prototype and a solid foundation for a polished Sudoku release.
