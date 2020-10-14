using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Project2048.Core;

namespace Project2048MSTest
{
    [TestClass]
    public class ChessBoardAddEmptyTest
    {

        [TestMethod]
        public static void SingleDifferentAddSetEmptyTest()
        {
            var chessBoard = new ChessBoard();
            var newBoard = new ChessBoard(chessBoard);
            var emptyPositions = chessBoard.GetEmptyPositions();
            var emptyIndices = RandomGenerator.GetDistinctInts(2, chessBoard.EmptyCount);

            chessBoard.AddNew(emptyPositions[emptyIndices[0]], 2);
            var addedBoard = new ChessBoard(chessBoard);
            chessBoard.SetEmpty(emptyPositions[emptyIndices[1]]);

            Assert.AreEqual(chessBoard.EmptyCount, 15);
            Assert.AreNotEqual(chessBoard, newBoard);
            Assert.AreEqual(addedBoard, chessBoard);
        }

        [TestMethod]
        public void SingleRandomAddTheSamePlaceTest()
        {
            var chessBoard = new ChessBoard();
            int randomRow = RandomGenerator.Next(4);
            int randomCol = RandomGenerator.Next(4);
            var randomPosition = new Position(randomRow, randomCol);

            chessBoard.AddNew(randomPosition, ChessBoardHandler.RandomLevel());
            chessBoard.AddNew(randomPosition, ChessBoardHandler.RandomLevel());

            Assert.AreEqual(chessBoard.EmptyCount, 15);
        }

        [TestMethod]
        public void AddNewTest()
        {
            for (int i = 0; i < 100; ++i)
            {
                var chessBoard = new ChessBoard();
                int occupyCount = RandomGenerator.Next(16);

                ChessBoardHandler.RandomAddLevelOne(chessBoard, occupyCount);
                var emptyPositions = chessBoard.GetEmptyPositions();

                Assert.AreEqual(emptyPositions.Count, 16 - occupyCount);
                Assert.AreEqual(chessBoard.EmptyCount, 16 - occupyCount);
            }
        }


        [TestMethod]
        public void AllAddNewSetEmptyTest()
        {
            var addEmptyBoard = new ChessBoard();
            var emptyBoard = new ChessBoard();
            for (int row = 0; row < 4; ++row)
            {
                for (int col = 0; col < 4; ++col)
                {
                    addEmptyBoard.AddNew(new Position(row, col), 1);
                    addEmptyBoard.SetEmpty(new Position(row, col));

                    Assert.AreEqual(addEmptyBoard, emptyBoard);
                    Console.WriteLine("{0}\t{1}", row, col);
                }
            }
        }

        [TestMethod]
        public void RandomAddNewSetEmptyTest()
        {
            var addEmptyBoard = new ChessBoard();
            var emptyPositions = addEmptyBoard.GetEmptyPositions();
            int occupyCount = RandomGenerator.Next(emptyPositions.Count);
            for (int i = 0; i < occupyCount; ++i)
            {
                addEmptyBoard.AddNew(emptyPositions[i], ChessBoardHandler.RandomLevel());
                addEmptyBoard.SetEmpty(emptyPositions[i]);
            }
            var emptyBoard = new ChessBoard();
            Assert.AreEqual(addEmptyBoard.EmptyCount, 16);
            Assert.AreEqual(addEmptyBoard, emptyBoard);
        }
    }
}
