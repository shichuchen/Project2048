using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Project2048.Core;

namespace Project2048MSTest
{   
    [TestClass]
    public class ChessBoardValuesTest
    {   
        [TestMethod]
        public void TestMaxValue()
        {
            var chessBoard = new ChessBoard();
            var levels = ChessBoardHandler.GetRandomCountDistinctLevels();
            int maxLevel = levels.Max();
            int levelIndex = 0;
            for(int row = 0; row < 4; ++row)
            {
                for(int col = 0; col < 4; ++col)
                {   
                    if(levelIndex == levels.Length)
                    {
                        break;
                    }
                    chessBoard.AddNew(new Position(row, col), levels[levelIndex]);
                    ++levelIndex;
                }
            }
            Assert.AreEqual(chessBoard.MaxValue, BitBoardHandler.ToValue(maxLevel));
        }
        [TestMethod]
        public void TestDistinctCount()
        {
            var chessBoard = new ChessBoard();
            var levels = ChessBoardHandler.GetRandomCountDistinctLevels();
            int levelIndex = 0;
            for (int row = 0; row < 4; ++row)
            {
                for (int col = 0; col < 4; ++col)
                {
                    if (levelIndex == levels.Length)
                    {
                        break;
                    }
                    chessBoard.AddNew(new Position(row, col), levels[levelIndex]);
                    ++levelIndex;
                }
            }
            Assert.AreEqual(chessBoard.DistinctCount, levels.Length);
        }
    }
}
