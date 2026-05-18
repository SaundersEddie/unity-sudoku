# Unity Sudoku

A Unity 6 Sudoku prototype built with UI Toolkit and the New Input System approach in mind. This project is the Unity version of a Sudoku concept first proven in a Python/Flask prototype.

The target platforms are:

- Windows desktop
- macOS desktop
- WebGL
- Android / mobile
- Tablet layouts

## Current Status

The project reached a strong functional prototype state before being paused.

The core game logic is working, tested, and playable in the Unity Editor and Windows desktop build. WebGL layout testing exposed a UI Toolkit / panel scaling problem that caused the gameplay screen layout to regress during troubleshooting.

Plain English project note:

> Great project, temporarily fucked during OpenAI chatGPT AI-assisted layout changes.

The gameplay logic is solid. The current `GamePlay.uss` layout needs to be restored or rebuilt from a known-good version.

## Implemented Features

### Scene Flow

The project uses three scenes:

- `mainMenu`
- `gamePlay`
- `gameOver`

Current scene flow:

```text
mainMenu -> gamePlay -> gameOver
```

The main menu allows difficulty selection and starts the game. Gameplay completes into the game-over screen, where the player can play again or return to the main menu.

## Difficulty Levels

Difficulty controls how many starting clues are shown.

| Difficulty | Starting Clues |
| ---------- | -------------: |
| Easy       |             30 |
| Medium     |             22 |
| Hard       |             15 |
| Godlike    |              5 |

The game uses a generated answer key. This is a digital Sudoku implementation: the generated solution is authoritative, even if a very low-clue displayed board could theoretically allow multiple valid Sudoku completions.

## Gameplay Features

Implemented:

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

Current test count at the last stable logic checkpoint:

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

## Technical Notes

### UI Toolkit

The UI is built with Unity UI Toolkit using:

- UXML
- USS
- `UIDocument`
- Panel Settings assets

Each scene uses its own UI document setup.

Panel assets used during development:

- `MainMenuPanelAsset`
- `GamePlayPanelAsset`
- `GameOverPanelAsset`

### Assembly Definitions

Unity EditMode tests required explicit assembly definition setup.

Runtime scripts use a runtime assembly definition.

EditMode tests use a dedicated test assembly definition referencing the runtime assembly.

This was required so Unity could discover and run the tests properly.

## Known Issues

### Gameplay Layout Regression

The gameplay logic is working, but the gameplay UI layout was damaged during WebGL layout troubleshooting.

Symptoms included:

- Gameplay board overflowing in WebGL
- Board and controls overlapping
- Pause overlay not rendering cleanly after layout edits
- Gameplay card behaving differently between Editor, Windows build, mobile simulator, and WebGL

Recommended recovery path:

1. Restore `Assets/GameUI/GamePlay/GamePlay.uss` from the last known-good commit.
2. Avoid broad full-file layout replacements without a commit first.
3. Fix WebGL canvas sizing separately from the gameplay USS.
4. Verify Panel Settings before changing USS dimensions.
5. Retest in this order:
   - Unity Editor Full HD
   - Windows desktop build
   - Mobile simulator
   - WebGL local server

### WebGL Canvas

WebGL layout problems appear related to the browser canvas size and Unity panel scaling, not only USS styling.

Recommended WebGL checks:

- Player Settings -> WebGL -> Resolution and Presentation
- Canvas width / height
- WebGL template
- Compression settings
- Local server testing instead of opening `index.html` directly

For local WebGL testing:

```bash
python -m http.server 8000
```

Then open:

```text
http://localhost:8000
```

## Suggested Next Steps

When development resumes:

1. Recover the last good gameplay USS.
2. Reconfirm Editor and Windows layout.
3. Reconfirm the 52 EditMode tests.
4. Rebuild WebGL with a sane canvas size.
5. Do a controlled responsive layout pass.
6. Extend theme support to main menu and game-over screens.
7. Add final visual polish.
8. Add platform-specific build notes.

## Safety Rule Going Forward

Before broad UI changes, especially full USS replacements:

```text
Commit first. Experiment second.
```

Unity UI Toolkit layout changes can cascade quickly, and rollback safety matters.

## Project Purpose

This Unity project is intended as the next version of the Sudoku concept after the Python prototype. The Python version proved the core Sudoku idea and logic. The Unity version expands it toward a proper multi-platform game build.

Despite the current UI layout regression, the project has a strong tested foundation and is worth continuing after restoring the gameplay layout.
