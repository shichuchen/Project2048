using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Bson;
using Project2048;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Project2048MSTest
{   
    [TestClass]
    public class ChessBoardValuesTest
    {   
        [TestMethod]
        public void TestMaxValue()
        {
            ChessBoard chessBoard = new ChessBoard();
            int[] levels = ChessBoardHandler.GetRandomCountDistinctLevels();
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
            ChessBoard chessBoard = new ChessBoard();
            int[] levels = ChessBoardHandler.GetRandomCountDistinctLevels();
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
