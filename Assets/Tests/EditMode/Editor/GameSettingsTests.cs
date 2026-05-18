using NUnit.Framework;
using UnitySudoku.Core;

namespace UnitySudoku.Tests.EditMode
{
    public sealed class GameSettingsTests
    {
        [Test]
        public void EasyDifficulty_UsesThirtyVisibleClues()
        {
            Assert.AreEqual(30, GameSettings.GetVisibleClueCount(DifficultyLevel.Easy));
        }

        [Test]
        public void MediumDifficulty_UsesTwentyTwoVisibleClues()
        {
            Assert.AreEqual(22, GameSettings.GetVisibleClueCount(DifficultyLevel.Medium));
        }

        [Test]
        public void HardDifficulty_UsesFifteenVisibleClues()
        {
            Assert.AreEqual(15, GameSettings.GetVisibleClueCount(DifficultyLevel.Hard));
        }

        [Test]
        public void GodlikeDifficulty_UsesFiveVisibleClues()
        {
            Assert.AreEqual(5, GameSettings.GetVisibleClueCount(DifficultyLevel.Godlike));
        }

        [Test]
        public void SelectedDifficulty_DefaultsToEasy()
        {
            Assert.AreEqual(DifficultyLevel.Easy, GameSettings.SelectedDifficulty);
        }
    }
}
