using System;
using NUnit.Framework;
using UnitySudoku.Core.Sudoku;

namespace UnitySudoku.Tests.EditMode
{
    public sealed class SudokuGameStateTests
    {
        [Test]
        public void Constructor_RejectsNullSolution()
        {
            bool[,] givenCells = new bool[9, 9];

            Assert.Throws<ArgumentNullException>(() =>
                new SudokuGameState(null, givenCells)
            );
        }

        [Test]
        public void Constructor_RejectsNullGivenCells()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);

            Assert.Throws<ArgumentNullException>(() =>
                new SudokuGameState(solution, null)
            );
        }

        [Test]
        public void IsGivenCell_ReturnsTrueForGivenCell()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];
            givenCells[0, 0] = true;

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            Assert.IsTrue(gameState.IsGivenCell(0, 0));
        }

        [Test]
        public void IsGivenCell_ReturnsFalseForEditableCell()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            Assert.IsFalse(gameState.IsGivenCell(0, 0));
        }

        [Test]
        public void TrySetPlayerValue_RejectsGivenCell()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];
            givenCells[0, 0] = true;

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            bool changed = gameState.TrySetPlayerValue(0, 0, 5);

            Assert.IsFalse(changed);
            Assert.AreEqual(0, gameState.GetPlayerValue(0, 0));
            Assert.AreEqual(0, gameState.MoveCount);
        }

        [Test]
        public void TrySetPlayerValue_AcceptsEditableCell()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            bool changed = gameState.TrySetPlayerValue(0, 0, 5);

            Assert.IsTrue(changed);
            Assert.AreEqual(5, gameState.GetPlayerValue(0, 0));
            Assert.AreEqual(1, gameState.MoveCount);
        }

        [Test]
        public void TrySetPlayerValue_DoesNotCountSameValueTwice()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            gameState.TrySetPlayerValue(0, 0, 5);
            bool changed = gameState.TrySetPlayerValue(0, 0, 5);

            Assert.IsFalse(changed);
            Assert.AreEqual(5, gameState.GetPlayerValue(0, 0));
            Assert.AreEqual(1, gameState.MoveCount);
        }

        [Test]
        public void TrySetPlayerValue_CountsChangedValue()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            gameState.TrySetPlayerValue(0, 0, 5);
            bool changed = gameState.TrySetPlayerValue(0, 0, 6);

            Assert.IsTrue(changed);
            Assert.AreEqual(6, gameState.GetPlayerValue(0, 0));
            Assert.AreEqual(2, gameState.MoveCount);
        }

        [Test]
        public void TryClearPlayerValue_ClearsEditableCell()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            gameState.TrySetPlayerValue(0, 0, 5);
            bool changed = gameState.TryClearPlayerValue(0, 0);

            Assert.IsTrue(changed);
            Assert.AreEqual(0, gameState.GetPlayerValue(0, 0));
            Assert.AreEqual(2, gameState.MoveCount);
        }

        [Test]
        public void TryClearPlayerValue_DoesNotCountEmptyCell()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            bool changed = gameState.TryClearPlayerValue(0, 0);

            Assert.IsFalse(changed);
            Assert.AreEqual(0, gameState.MoveCount);
        }

        [Test]
        public void TryClearPlayerValue_RejectsGivenCell()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];
            givenCells[0, 0] = true;

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            bool changed = gameState.TryClearPlayerValue(0, 0);

            Assert.IsFalse(changed);
            Assert.AreEqual(0, gameState.MoveCount);
        }

        [Test]
        public void IsCorrectValue_ReturnsTrueWhenPlayerValueMatchesSolution()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            int correctValue = solution[0, 0];
            gameState.TrySetPlayerValue(0, 0, correctValue);

            Assert.IsTrue(gameState.IsCorrectValue(0, 0));
            Assert.IsFalse(gameState.IsWrongValue(0, 0));
        }

        [Test]
        public void IsWrongValue_ReturnsTrueWhenPlayerValueDoesNotMatchSolution()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            int wrongValue = solution[0, 0] == 1 ? 2 : 1;
            gameState.TrySetPlayerValue(0, 0, wrongValue);

            Assert.IsTrue(gameState.IsWrongValue(0, 0));
            Assert.IsFalse(gameState.IsCorrectValue(0, 0));
        }

        [Test]
        public void IsCompleteAndCorrect_ReturnsFalseForEmptyEditableBoard()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            Assert.IsFalse(gameState.IsCompleteAndCorrect());
        }

        [Test]
        public void IsCompleteAndCorrect_ReturnsFalseForPartiallyFilledBoard()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            gameState.TrySetPlayerValue(0, 0, solution[0, 0]);

            Assert.IsFalse(gameState.IsCompleteAndCorrect());
        }

        [Test]
        public void IsCompleteAndCorrect_ReturnsFalseForWrongCompletedBoard()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    int wrongValue = solution[row, col] == 1 ? 2 : 1;
                    gameState.TrySetPlayerValue(row, col, wrongValue);
                }
            }

            Assert.IsFalse(gameState.IsCompleteAndCorrect());
        }

        [Test]
        public void IsCompleteAndCorrect_ReturnsTrueForCorrectCompletedBoard()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    gameState.TrySetPlayerValue(row, col, solution[row, col]);
                }
            }

            Assert.IsTrue(gameState.IsCompleteAndCorrect());
        }

        [Test]
        public void IsCompleteAndCorrect_HandlesGivenCells()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            givenCells[0, 0] = true;
            givenCells[1, 1] = true;

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (givenCells[row, col])
                    {
                        continue;
                    }

                    gameState.TrySetPlayerValue(row, col, solution[row, col]);
                }
            }

            Assert.IsTrue(gameState.IsCompleteAndCorrect());
        }

        [Test]
        public void ToggleNote_AddsNoteToEditableEmptyCell()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            bool changed = gameState.ToggleNote(0, 0, 5);

            Assert.IsTrue(changed);
            Assert.IsTrue(gameState.HasNote(0, 0, 5));
            Assert.AreEqual(0, gameState.MoveCount);
        }

        [Test]
        public void ToggleNote_RemovesExistingNote()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            gameState.ToggleNote(0, 0, 5);
            bool changed = gameState.ToggleNote(0, 0, 5);

            Assert.IsTrue(changed);
            Assert.IsFalse(gameState.HasNote(0, 0, 5));
            Assert.AreEqual(0, gameState.MoveCount);
        }

        [Test]
        public void ToggleNote_RejectsGivenCell()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];
            givenCells[0, 0] = true;

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            bool changed = gameState.ToggleNote(0, 0, 5);

            Assert.IsFalse(changed);
            Assert.IsFalse(gameState.HasNote(0, 0, 5));
        }

        [Test]
        public void ToggleNote_RejectsCellWithPlayerValue()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            gameState.TrySetPlayerValue(0, 0, 4);
            bool changed = gameState.ToggleNote(0, 0, 5);

            Assert.IsFalse(changed);
            Assert.IsFalse(gameState.HasNote(0, 0, 5));
        }

        [Test]
        public void TrySetPlayerValue_ClearsNotesInCell()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            gameState.ToggleNote(0, 0, 2);
            gameState.ToggleNote(0, 0, 5);

            gameState.TrySetPlayerValue(0, 0, 4);

            Assert.IsFalse(gameState.HasAnyNotes(0, 0));
        }

        [Test]
        public void ClearNotes_RemovesAllNotesInCell()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);
            bool[,] givenCells = new bool[9, 9];

            SudokuGameState gameState = new SudokuGameState(solution, givenCells);

            gameState.ToggleNote(0, 0, 2);
            gameState.ToggleNote(0, 0, 5);
            gameState.ToggleNote(0, 0, 9);

            gameState.ClearNotes(0, 0);

            Assert.IsFalse(gameState.HasAnyNotes(0, 0));
        }
    }
}
