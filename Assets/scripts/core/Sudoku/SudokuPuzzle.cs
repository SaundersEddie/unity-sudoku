using System;
using System.Collections.Generic;

namespace UnitySudoku.Core.Sudoku
{
    public sealed class SudokuPuzzle
    {
        public int[,] Solution { get; }
        public bool[,] VisibleCells { get; }

        public SudokuPuzzle(int[,] solution, bool[,] visibleCells)
        {
            Solution = solution;
            VisibleCells = visibleCells;
        }
    }

    public static class SudokuPuzzleBuilder
    {
        private const int GridSize = 9;

        public static SudokuPuzzle CreatePuzzle(int[,] solution, int visibleClueCount, int? seed = null)
        {
            if (solution == null)
            {
                throw new ArgumentNullException(nameof(solution));
            }

            if (solution.GetLength(0) != GridSize || solution.GetLength(1) != GridSize)
            {
                throw new ArgumentException("Solution must be a 9x9 board.", nameof(solution));
            }

            if (visibleClueCount < 0 || visibleClueCount > GridSize * GridSize)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(visibleClueCount),
                    "Visible clue count must be between 0 and 81."
                );
            }

            Random random = seed.HasValue ? new Random(seed.Value) : new Random();
            bool[,] visibleCells = new bool[GridSize, GridSize];

            List<(int Row, int Col)> positions = new();

            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    positions.Add((row, col));
                }
            }

            Shuffle(positions, random);

            for (int index = 0; index < visibleClueCount; index++)
            {
                (int row, int col) = positions[index];
                visibleCells[row, col] = true;
            }

            return new SudokuPuzzle(solution, visibleCells);
        }

        public static int CountVisibleCells(bool[,] visibleCells)
        {
            if (visibleCells == null)
            {
                return 0;
            }

            int count = 0;

            for (int row = 0; row < visibleCells.GetLength(0); row++)
            {
                for (int col = 0; col < visibleCells.GetLength(1); col++)
                {
                    if (visibleCells[row, col])
                    {
                        count++;
                    }
                }
            }

            return count;
        }

        private static void Shuffle<T>(List<T> values, Random random)
        {
            for (int index = values.Count - 1; index > 0; index--)
            {
                int swapIndex = random.Next(index + 1);

                (values[index], values[swapIndex]) = (values[swapIndex], values[index]);
            }
        }
    }
}
