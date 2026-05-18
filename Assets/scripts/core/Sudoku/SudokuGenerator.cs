using System;
using System.Collections.Generic;

namespace UnitySudoku.Core.Sudoku
{
    public static class SudokuGenerator
    {
        private const int GridSize = 9;
        private const int BoxSize = 3;

        public static int[,] GenerateSolution(int? seed = null)
        {
            Random random = seed.HasValue ? new Random(seed.Value) : new Random();

            List<int> rows = BuildShuffledGroups(random);
            List<int> cols = BuildShuffledGroups(random);
            List<int> numbers = BuildShuffledNumbers(random);

            int[,] board = new int[GridSize, GridSize];

            for (int rowIndex = 0; rowIndex < GridSize; rowIndex++)
            {
                for (int colIndex = 0; colIndex < GridSize; colIndex++)
                {
                    int patternIndex = Pattern(rows[rowIndex], cols[colIndex]);
                    board[rowIndex, colIndex] = numbers[patternIndex];
                }
            }

            return board;
        }

        private static int Pattern(int row, int col)
        {
            return (BoxSize * (row % BoxSize) + row / BoxSize + col) % GridSize;
        }

        private static List<int> BuildShuffledGroups(Random random)
        {
            List<int> groups = ShuffledRange(BoxSize, random);
            List<int> result = new List<int>();

            foreach (int group in groups)
            {
                List<int> innerRows = ShuffledRange(BoxSize, random);

                foreach (int innerRow in innerRows)
                {
                    result.Add(group * BoxSize + innerRow);
                }
            }

            return result;
        }

        private static List<int> BuildShuffledNumbers(Random random)
        {
            List<int> numbers = new List<int>();

            for (int number = 1; number <= GridSize; number++)
            {
                numbers.Add(number);
            }

            Shuffle(numbers, random);

            return numbers;
        }

        private static List<int> ShuffledRange(int count, Random random)
        {
            List<int> values = new List<int>();

            for (int value = 0; value < count; value++)
            {
                values.Add(value);
            }

            Shuffle(values, random);

            return values;
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
