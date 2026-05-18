using System;

namespace UnitySudoku.Core.Sudoku
{
    public sealed class SudokuGameState
    {
        private const int GridSize = 9;

        private readonly int[,] _solution;
        private readonly bool[,] _givenCells;
        private readonly int[,] _playerValues;
        private readonly bool[,,] _notes;

        public int MoveCount { get; private set; }

        public SudokuGameState(int[,] solution, bool[,] givenCells)
        {
            if (solution == null)
            {
                throw new ArgumentNullException(nameof(solution));
            }

            if (givenCells == null)
            {
                throw new ArgumentNullException(nameof(givenCells));
            }

            if (solution.GetLength(0) != GridSize || solution.GetLength(1) != GridSize)
            {
                throw new ArgumentException("Solution must be a 9x9 board.", nameof(solution));
            }

            if (givenCells.GetLength(0) != GridSize || givenCells.GetLength(1) != GridSize)
            {
                throw new ArgumentException("Given cells must be a 9x9 board.", nameof(givenCells));
            }

            _solution = solution;
            _givenCells = givenCells;
            _playerValues = new int[GridSize, GridSize];
            _notes = new bool[GridSize, GridSize, GridSize + 1];
        }

        public bool IsGivenCell(int row, int col)
        {
            ValidatePosition(row, col);

            return _givenCells[row, col];
        }

        public int GetSolutionValue(int row, int col)
        {
            ValidatePosition(row, col);

            return _solution[row, col];
        }

        public int GetPlayerValue(int row, int col)
        {
            ValidatePosition(row, col);

            return _playerValues[row, col];
        }

        public bool TrySetPlayerValue(int row, int col, int value)
        {
            ValidatePosition(row, col);

            if (value < 1 || value > GridSize)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    "Player value must be between 1 and 9."
                );
            }

            if (_givenCells[row, col])
            {
                return false;
            }

            if (_playerValues[row, col] == value)
            {
                return false;
            }

            _playerValues[row, col] = value;
            ClearNotes(row, col);
            MoveCount++;

            return true;
        }

        public bool TryClearPlayerValue(int row, int col)
        {
            ValidatePosition(row, col);

            if (_givenCells[row, col])
            {
                return false;
            }

            if (_playerValues[row, col] == 0)
            {
                return false;
            }

            _playerValues[row, col] = 0;
            MoveCount++;

            return true;
        }

        public bool IsCorrectValue(int row, int col)
        {
            ValidatePosition(row, col);

            int playerValue = _playerValues[row, col];

            return playerValue != 0 && playerValue == _solution[row, col];
        }

        public bool IsWrongValue(int row, int col)
        {
            ValidatePosition(row, col);

            int playerValue = _playerValues[row, col];

            return playerValue != 0 && playerValue != _solution[row, col];
        }

        private static void ValidatePosition(int row, int col)
        {
            if (row < 0 || row >= GridSize)
            {
                throw new ArgumentOutOfRangeException(nameof(row));
            }

            if (col < 0 || col >= GridSize)
            {
                throw new ArgumentOutOfRangeException(nameof(col));
            }
        }

        private static void ValidateNoteValue(int value)
        {
            if (value < 1 || value > GridSize)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }
        }
        
        public bool IsCompleteAndCorrect()
        {
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    if (_givenCells[row, col])
                    {
                        continue;
                    }

                    int playerValue = _playerValues[row, col];

                    if (playerValue == 0)
                    {
                        return false;
                    }

                    if (playerValue != _solution[row, col])
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public bool HasNote(int row, int col, int value)
        {
            ValidatePosition(row, col);
            ValidateNoteValue(value);

            return _notes[row, col, value];
        }

        public bool ToggleNote(int row, int col, int value)
        {
            ValidatePosition(row, col);
            ValidateNoteValue(value);

            if (_givenCells[row, col])
            {
                return false;
            }

            if (_playerValues[row, col] != 0)
            {
                return false;
            }

            _notes[row, col, value] = !_notes[row, col, value];

            return true;
        }

        public void ClearNotes(int row, int col)
        {
            ValidatePosition(row, col);

            for (int value = 1; value <= GridSize; value++)
            {
                _notes[row, col, value] = false;
            }
        }

        public bool HasAnyNotes(int row, int col)
        {
            ValidatePosition(row, col);

            for (int value = 1; value <= GridSize; value++)
            {
                if (_notes[row, col, value])
                {
                    return true;
                }
            }

            return false;
        }
    }
}
