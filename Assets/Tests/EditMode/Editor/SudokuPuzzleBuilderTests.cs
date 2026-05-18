using System;
using NUnit.Framework;
using UnitySudoku.Core;
using UnitySudoku.Core.Sudoku;

namespace UnitySudoku.Tests.EditMode
{
    public sealed class SudokuPuzzleBuilderTests
    {
        [Test]
        public void CreatePuzzle_UsesRequestedVisibleClueCount()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);

            SudokuPuzzle puzzle = SudokuPuzzleBuilder.CreatePuzzle(
                solution,
                visibleClueCount: 30,
                seed: 99
            );

            Assert.AreEqual(
                30,
                SudokuPuzzleBuilder.CountVisibleCells(puzzle.VisibleCells)
            );
        }

        [Test]
        public void CreatePuzzle_KeepsSolutionReference()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);

            SudokuPuzzle puzzle = SudokuPuzzleBuilder.CreatePuzzle(
                solution,
                visibleClueCount: 30,
                seed: 99
            );

            Assert.AreSame(solution, puzzle.Solution);
        }

        [Test]
        public void CreatePuzzle_WithSameSeed_CreatesSameVisibleCells()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);

            SudokuPuzzle firstPuzzle = SudokuPuzzleBuilder.CreatePuzzle(
                solution,
                visibleClueCount: 30,
                seed: 99
            );

            SudokuPuzzle secondPuzzle = SudokuPuzzleBuilder.CreatePuzzle(
                solution,
                visibleClueCount: 30,
                seed: 99
            );

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    Assert.AreEqual(
                        firstPuzzle.VisibleCells[row, col],
                        secondPuzzle.VisibleCells[row, col]
                    );
                }
            }
        }

        [Test]
        public void CreatePuzzle_RejectsNullSolution()
        {
            Assert.Throws<ArgumentNullException>(() =>
                SudokuPuzzleBuilder.CreatePuzzle(null, visibleClueCount: 30)
            );
        }

        [Test]
        public void CreatePuzzle_RejectsNegativeVisibleClueCount()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                SudokuPuzzleBuilder.CreatePuzzle(solution, visibleClueCount: -1)
            );
        }

        [Test]
        public void CreatePuzzle_RejectsVisibleClueCountOverEightyOne()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                SudokuPuzzleBuilder.CreatePuzzle(solution, visibleClueCount: 82)
            );
        }

        [Test]
        public void DifficultyClueCounts_CanCreateMatchingPuzzles()
        {
            int[,] solution = SudokuGenerator.GenerateSolution(seed: 12345);

            foreach (DifficultyLevel difficulty in Enum.GetValues(typeof(DifficultyLevel)))
            {
                int clueCount = GameSettings.GetVisibleClueCount(difficulty);

                SudokuPuzzle puzzle = SudokuPuzzleBuilder.CreatePuzzle(
                    solution,
                    clueCount,
                    seed: (int)difficulty + 100
                );

                Assert.AreEqual(
                    clueCount,
                    SudokuPuzzleBuilder.CountVisibleCells(puzzle.VisibleCells),
                    $"Wrong clue count for {difficulty}."
                );
            }
        }
    }
}
