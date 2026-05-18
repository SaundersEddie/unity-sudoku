using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnitySudoku.Core;
using UnitySudoku.Core.Sudoku;
using UnitySudoku.SceneFlow;

namespace UnitySudoku.UI
{
    [RequireComponent(typeof(UIDocument))]
    public sealed class GamePlayController : MonoBehaviour
    {
        private const int GridSize = 9;
        private const int BoxSize = 3;
        private float _elapsedSeconds;
        private bool _timerIsRunning;
        private bool _notesModeEnabled;
        private bool _isPaused;
        private bool _timerWasRunningBeforePause;

        private Label _difficultyLabel;
        private Label _timerLabel;
        private Label _movesLabel;

        private VisualElement _sudokuBoard;

        private Button _notesButton;
        private Button _clearButton;
        private Button _pauseButton;
        private Button _menuButton;
        private Button _resumeButton;
        private Button _pauseMenuButton;
        private Button _themeButton;
        private VisualElement _pauseOverlay;
        private VisualElement _root;
        private VisualElement _gameCard;

        private readonly List<Label> _cellLabels = new();
        private readonly List<Button> _numberButtons = new();
        private readonly Dictionary<Button, EventCallback<ClickEvent>> _numberButtonCallbacks = new();

        private int[,] _solutionBoard;
        private SudokuPuzzle _puzzle;
        private SudokuGameState _gameState;

        private int _selectedRow = -1;
        private int _selectedCol = -1;

        private void OnEnable()
        {
            UIDocument document = GetComponent<UIDocument>();
            VisualElement root = document.rootVisualElement;
            _root = root;
            _gameCard = root.Q<VisualElement>("gameCard");
            _root.focusable = true;
            _root.RegisterCallback<KeyDownEvent>(HandleKeyDown);

            _difficultyLabel = root.Q<Label>("difficultyLabel");
            _timerLabel = root.Q<Label>("timerLabel");
            _movesLabel = root.Q<Label>("movesLabel");

            _sudokuBoard = root.Q<VisualElement>("sudokuBoard");

            _notesButton = root.Q<Button>("notesButton");
            _clearButton = root.Q<Button>("clearButton");
            _pauseButton = root.Q<Button>("pauseButton");
            _menuButton = root.Q<Button>("menuButton");
            _resumeButton = root.Q<Button>("resumeButton");
            _pauseMenuButton = root.Q<Button>("pauseMenuButton");
            _themeButton = root.Q<Button>("themeButton");
            _pauseOverlay = root.Q<VisualElement>("pauseOverlay");

            CacheNumberButtons(root);

            _clearButton.clicked += ClearSelectedCell;
            _menuButton.clicked += ReturnToMainMenu;
            _notesButton.clicked += ToggleNotesMode;
            _pauseButton.clicked += PauseGame;
            _resumeButton.clicked += ResumeGame;
            _pauseMenuButton.clicked += ReturnToMainMenu;
            _themeButton.clicked += ToggleTheme;

            SetInitialLabels();
            BuildEmptyBoard();
            GenerateAndRenderPuzzle();
            ApplyTheme();
            _root.Focus();
        }

        private void OnDisable()
        {
            if (_clearButton != null)
            {
                _clearButton.clicked -= ClearSelectedCell;
            }

            if (_menuButton != null)
            {
                _menuButton.clicked -= ReturnToMainMenu;
            }

            if (_notesButton != null)
            {
                _notesButton.clicked -= ToggleNotesMode;
            }

            if (_pauseButton != null)
            {
                _pauseButton.clicked -= PauseGame;
            }

            if (_resumeButton != null)
            {
                _resumeButton.clicked -= ResumeGame;
            }

            if (_pauseMenuButton != null)
            {
                _pauseMenuButton.clicked -= ReturnToMainMenu;
            }

            if (_themeButton != null)
            {
                _themeButton.clicked -= ToggleTheme;
            }

            if (_root != null)
            {
                _root.UnregisterCallback<KeyDownEvent>(HandleKeyDown);
            }

            foreach (KeyValuePair<Button, EventCallback<ClickEvent>> pair in _numberButtonCallbacks)
            {
                pair.Key.UnregisterCallback(pair.Value);
            }

            _numberButtonCallbacks.Clear();
        }

        private void Update()
        {
            if (!_timerIsRunning)
            {
                return;
            }

            _elapsedSeconds += Time.deltaTime;
            UpdateTimerLabel();
        }

        private void CacheNumberButtons(VisualElement root)
        {
            _numberButtons.Clear();
            _numberButtonCallbacks.Clear();

            for (int number = 1; number <= 9; number++)
            {
                int capturedNumber = number;
                Button button = root.Q<Button>($"numberButton{number}");

                EventCallback<ClickEvent> callback = _ => SetSelectedCellValue(capturedNumber);

                button.RegisterCallback(callback);
                _numberButtons.Add(button);
                _numberButtonCallbacks.Add(button, callback);
            }
        }

        private void SetInitialLabels()
        {
            DifficultyLevel difficulty = GameSettings.SelectedDifficulty;
            int clueCount = GameSettings.GetVisibleClueCount(difficulty);

            _difficultyLabel.text = $"Difficulty: {difficulty} ({clueCount} clues)";
            _timerLabel.text = "Time: 00:00";
            _movesLabel.text = "Moves: 0";
        }

        private void BuildEmptyBoard()
        {
            _sudokuBoard.Clear();
            _cellLabels.Clear();

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    int capturedRow = row;
                    int capturedCol = col;

                    Label cell = new Label(string.Empty)
                    {
                        name = $"cell-{row}-{col}"
                    };

                    cell.AddToClassList("sudoku-cell");
                    cell.RegisterCallback<ClickEvent>(_ => SelectCell(capturedRow, capturedCol));

                    if (col == 2 || col == 5)
                    {
                        cell.AddToClassList("box-right");
                    }

                    if (row == 2 || row == 5)
                    {
                        cell.AddToClassList("box-bottom");
                    }

                    _sudokuBoard.Add(cell);
                    _cellLabels.Add(cell);
                }
            }
        }

        private void GenerateAndRenderPuzzle()
        {
            DifficultyLevel difficulty = GameSettings.SelectedDifficulty;
            int visibleClueCount = GameSettings.GetVisibleClueCount(difficulty);

            _solutionBoard = SudokuGenerator.GenerateSolution();

            _puzzle = SudokuPuzzleBuilder.CreatePuzzle(
                _solutionBoard,
                visibleClueCount
            );

            _gameState = new SudokuGameState(
                _solutionBoard,
                _puzzle.VisibleCells
            );

            _selectedRow = -1;
            _selectedCol = -1;

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    Label cell = GetCellLabel(row, col);

                    cell.RemoveFromClassList("selected-cell");
                    cell.RemoveFromClassList("related-cell");
                    cell.RemoveFromClassList("matching-cell");
                    cell.RemoveFromClassList("given-cell");
                    cell.RemoveFromClassList("correct-cell");
                    cell.RemoveFromClassList("wrong-cell");

                    if (_puzzle.VisibleCells[row, col])
                    {
                        cell.text = _solutionBoard[row, col].ToString();
                        cell.AddToClassList("given-cell");
                    }
                    else
                    {
                        cell.text = string.Empty;
                    }
                }
            }

            UpdateMovesLabel();
            ResetTimer();
            UpdateCompletedNumberButtons();
        }

        private void PauseGame()
        {
            if (_isPaused)
            {
                return;
            }

            _timerWasRunningBeforePause = _timerIsRunning;
            StopTimer();

            _isPaused = true;
            _pauseOverlay.RemoveFromClassList("hidden");
            _sudokuBoard.AddToClassList("board-paused");
        }

        private void ResumeGame()
        {
            if (!_isPaused)
            {
                return;
            }

            _isPaused = false;
            _pauseOverlay.AddToClassList("hidden");
            _sudokuBoard.RemoveFromClassList("board-paused");

            if (_timerWasRunningBeforePause)
            {
                StartTimerIfNeeded();
            }

            _timerWasRunningBeforePause = false;
            _root.Focus();
        }

        private void SelectCell(int row, int col)
        {
            if (_isPaused)
            {
                return;
            }

            _selectedRow = row;
            _selectedCol = col;

            RefreshCellHighlights();
            _root.Focus();
        }

        private void SetSelectedCellValue(int value)
        {
            if (!HasSelectedCell() || _isPaused)
            {
                return;
            }

            if (_gameState.IsNumberComplete(value))
            {
                return;
            }

            if (_notesModeEnabled)
            {
                bool noteChanged = _gameState.ToggleNote(_selectedRow, _selectedCol, value);

                if (!noteChanged)
                {
                    return;
                }

                StartTimerIfNeeded();

                RenderCell(_selectedRow, _selectedCol);
                RefreshCellHighlights();
                return;
            }

            bool changed = _gameState.TrySetPlayerValue(_selectedRow, _selectedCol, value);

            if (!changed)
            {
                return;
            }

            StartTimerIfNeeded();

            RenderCell(_selectedRow, _selectedCol);
            UpdateMovesLabel();
            RefreshCellHighlights();
            UpdateCompletedNumberButtons();
            CheckForCompletion();
            _root.Focus();
        }

        private void HandleKeyDown(KeyDownEvent evt)
        {
            if (evt == null)
            {
                return;
            }

            if (evt.keyCode >= KeyCode.Alpha1 && evt.keyCode <= KeyCode.Alpha9)
            {
                int value = evt.keyCode - KeyCode.Alpha0;
                SetSelectedCellValue(value);
                evt.StopPropagation();
                return;
            }

            if (evt.keyCode >= KeyCode.Keypad1 && evt.keyCode <= KeyCode.Keypad9)
            {
                int value = evt.keyCode - KeyCode.Keypad0;
                SetSelectedCellValue(value);
                evt.StopPropagation();
                return;
            }

            if (evt.keyCode == KeyCode.Backspace || evt.keyCode == KeyCode.Delete)
            {
                ClearSelectedCell();
                evt.StopPropagation();
                return;
            }

            if (evt.keyCode == KeyCode.N)
            {
                ToggleNotesMode();
                evt.StopPropagation();
                return;
            }

            if (evt.keyCode == KeyCode.P || evt.keyCode == KeyCode.Escape)
            {
                TogglePauseFromKeyboard();
                evt.StopPropagation();
            }
        }

        private void CheckForCompletion()
        {
            if (!_gameState.IsCompleteAndCorrect())
            {
                return;
            }

            StopTimer();

            GameSettings.SaveCompletedGame(
                GameSettings.SelectedDifficulty,
                _gameState.MoveCount,
                GetElapsedTimeSeconds()
            );

            SceneManager.LoadScene(SceneNames.GameOver);
        }

        private void ClearSelectedCell()
        {
            if (!HasSelectedCell() || _isPaused)
            {
                return;
            }

            bool hadPlayerValue = _gameState.GetPlayerValue(_selectedRow, _selectedCol) != 0;
            bool hadNotes = _gameState.HasAnyNotes(_selectedRow, _selectedCol);

            bool changed = _gameState.TryClearPlayerValue(_selectedRow, _selectedCol);

            if (!changed && !hadNotes)
            {
                return;
            }

            if (hadNotes)
            {
                _gameState.ClearNotes(_selectedRow, _selectedCol);
            }

            RenderCell(_selectedRow, _selectedCol);

            if (hadPlayerValue)
            {
                UpdateMovesLabel();
                UpdateCompletedNumberButtons();
            }

            RefreshCellHighlights();
            _root.Focus();
        }

        private void RenderCell(int row, int col)
        {
            Label cell = GetCellLabel(row, col);

            cell.RemoveFromClassList("correct-cell");
            cell.RemoveFromClassList("wrong-cell");
            cell.RemoveFromClassList("notes-cell");

            if (_gameState.IsGivenCell(row, col))
            {
                cell.text = _solutionBoard[row, col].ToString();
                cell.AddToClassList("given-cell");
                return;
            }

            int playerValue = _gameState.GetPlayerValue(row, col);

            if (playerValue != 0)
            {
                cell.text = playerValue.ToString();

                if (_gameState.IsCorrectValue(row, col))
                {
                    cell.AddToClassList("correct-cell");
                }
                else if (_gameState.IsWrongValue(row, col))
                {
                    cell.AddToClassList("wrong-cell");
                }

                return;
            }

            cell.text = BuildNotesText(row, col);

            if (_gameState.HasAnyNotes(row, col))
            {
                cell.AddToClassList("notes-cell");
            }
        }

        private void RefreshCellHighlights()
        {
            ClearHighlightStyles();

            if (!HasSelectedCell())
            {
                return;
            }

            int selectedValue = GetDisplayedCellValue(_selectedRow, _selectedCol);

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    Label cell = GetCellLabel(row, col);

                    bool sameRow = row == _selectedRow;
                    bool sameCol = col == _selectedCol;
                    bool sameBox = IsSameBox(row, col, _selectedRow, _selectedCol);

                    if (sameRow || sameCol || sameBox)
                    {
                        cell.AddToClassList("related-cell");
                    }

                    int cellValue = GetDisplayedCellValue(row, col);

                    if (selectedValue != 0 && cellValue == selectedValue)
                    {
                        cell.AddToClassList("matching-cell");
                    }
                }
            }

            GetCellLabel(_selectedRow, _selectedCol).AddToClassList("selected-cell");
        }

        private void ClearHighlightStyles()
        {
            foreach (Label cell in _cellLabels)
            {
                cell.RemoveFromClassList("selected-cell");
                cell.RemoveFromClassList("related-cell");
                cell.RemoveFromClassList("matching-cell");
            }
        }

        private int GetDisplayedCellValue(int row, int col)
        {
            if (_gameState.IsGivenCell(row, col))
            {
                return _solutionBoard[row, col];
            }

            return _gameState.GetPlayerValue(row, col);
        }

        private static bool IsSameBox(int firstRow, int firstCol, int secondRow, int secondCol)
        {
            return firstRow / BoxSize == secondRow / BoxSize &&
                   firstCol / BoxSize == secondCol / BoxSize;
        }

        private bool HasSelectedCell()
        {
            return _selectedRow >= 0 && _selectedCol >= 0;
        }

        private Label GetCellLabel(int row, int col)
        {
            int cellIndex = row * GridSize + col;
            return _cellLabels[cellIndex];
        }

        private void StartTimerIfNeeded()
        {
            if (_timerIsRunning)
            {
                return;
            }

            _timerIsRunning = true;
        }

        private void ResetTimer()
        {
            _elapsedSeconds = 0f;
            _timerIsRunning = false;
            UpdateTimerLabel();
        }

        private void StopTimer()
        {
            _timerIsRunning = false;
        }       

        private void UpdateTimerLabel()
        {
            int totalSeconds = Mathf.FloorToInt(_elapsedSeconds);
            int minutes = totalSeconds / 60;
            int seconds = totalSeconds % 60;

            _timerLabel.text = $"Time: {minutes:00}:{seconds:00}";
        }

        private int GetElapsedTimeSeconds()
        {
            return Mathf.FloorToInt(_elapsedSeconds);
        }

        private void UpdateMovesLabel()
        {
            _movesLabel.text = $"Moves: {_gameState.MoveCount}";
        }

        private void ReturnToMainMenu()
        {
            SceneManager.LoadScene(SceneNames.MainMenu);
        }

        private string BuildNotesText(int row, int col)
        {
            string notesText = string.Empty;

            for (int value = 1; value <= GridSize; value++)
            {
                notesText += _gameState.HasNote(row, col, value) ? value.ToString() : " ";

                if (value == 3 || value == 6)
                {
                    notesText += "\n";
                }
            }

            return notesText.TrimEnd();
        }

        private void UpdateCompletedNumberButtons()
        {
            int[] numberCounts = new int[GridSize + 1];

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    int value = GetDisplayedCellValue(row, col);

                    if (value >= 1 && value <= GridSize)
                    {
                        numberCounts[value]++;
                    }
                }
            }

            for (int index = 0; index < _numberButtons.Count; index++)
            {
                int number = index + 1;
                Button button = _numberButtons[index];

                if (numberCounts[number] >= GridSize)
                {
                    button.AddToClassList("number-complete");
                }
                else
                {
                    button.RemoveFromClassList("number-complete");
                }
            }
        }

        // private bool IsNumberComplete(int number)
        // {
        //     if (number < 1 || number > GridSize)
        //     {
        //         return false;
        //     }

        //     int count = 0;

        //     for (int row = 0; row < GridSize; row++)
        //     {
        //         for (int col = 0; col < GridSize; col++)
        //         {
        //             if (GetDisplayedCellValue(row, col) == number)
        //             {
        //                 count++;
        //             }
        //         }
        //     }

        //     return count >= GridSize;
        // }

        private void ToggleNotesMode()
        {
            if (_isPaused)
            {
                return;
            }

            _notesModeEnabled = !_notesModeEnabled;

            _notesButton.text = _notesModeEnabled ? "Notes: On" : "Notes: Off";

            if (_notesModeEnabled)
            {
                _notesButton.AddToClassList("active-button");
            }
            else
            {
                _notesButton.RemoveFromClassList("active-button");
            }

            _root.Focus();
        }

        private void ToggleTheme()
        {
            GameSettings.UseDarkTheme = !GameSettings.UseDarkTheme;
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            if (GameSettings.UseDarkTheme)
            {
                _root.AddToClassList("dark-theme");
                _themeButton.text = "Theme: Dark";
            }
            else
            {
                _root.RemoveFromClassList("dark-theme");
                _themeButton.text = "Theme: Light";
            }
        }

        private void TogglePauseFromKeyboard()
        {
            if (_isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
}
