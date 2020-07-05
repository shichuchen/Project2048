using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project2048;
using System;
using System.Collections.Generic;

namespace Project2048MSTest
{
    [TestClass]
    public class ChessBoardAddEmptyTest
    {

        [TestMethod]
        public static void SingleDifferentAddSetEmptyTest()
        {
            ChessBoard chessBoard = new ChessBoard();
            ChessBoard newBoard = new ChessBoard(chessBoard);
            List<Position> emptyPositions = chessBoard.CalculateAndGetEmptyPositions();
            int[] emptyIndices = RandomGenerator.GetDistinctInts(2, chessBoard.EmptyCount);

            chessBoard.AddNew(emptyPositions[emptyIndices[0]], 2);
            ChessBoard addedBoard = new ChessBoard(chessBoard);
            chessBoard.SetEmpty(emptyPositions[emptyIndices[1]]);

            Assert.AreEqual(chessBoard.EmptyCount, 15);
            Assert.AreNotEqual(chessBoard, newBoard);
            Assert.AreEqual(addedBoard, chessBoard);
        }

        [TestMethod]
        public void SingleRandomAddTheSamePlaceTest()
        {
            ChessBoard chessBoard = new ChessBoard();
            int randomRow = RandomGenerator.Next(4);
            int randomCol = RandomGenerator.Next(4);
            Position randomPosition = new Position(randomRow, randomCol);

            chessBoard.AddNew(randomPosition, ChessBoardHandler.RandomLevel());
            chessBoard.AddNew(randomPosition, ChessBoardHandler.RandomLevel());

            Assert.AreEqual(chessBoard.EmptyCount, 15);
        }

        [TestMethod]
        public void AddNewTest()
        {
            for (int i = 0; i < 100; ++i)
            {
                ChessBoard chessBoard = new ChessBoard();
                int occupyCount = RandomGenerator.Next(16);

                ChessBoardHandler.RandomAddLevelOne(chessBoard, occupyCount);
                List<Position> emptyPositions = chessBoard.CalculateAndGetEmptyPositions();

                Assert.AreEqual(emptyPositions.Count, 16 - occupyCount);
                Assert.AreEqual(chessBoard.EmptyCount, 16 - occupyCount);
            }
        }


        [TestMethod]
        public void AllAddNewSetEmptyTest()
        {
            ChessBoard addEmptyBoard = new ChessBoard();
            ChessBoard emptyBoard = new ChessBoard();
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
            ChessBoard addEmptyBoard = new ChessBoard();
            var emptyPositions = addEmptyBoard.CalculateAndGetEmptyPositions();
            int occupyCount = RandomGenerator.Next(emptyPositions.Count);
            for (int i = 0; i < occupyCount; ++i)
            {
                addEmptyBoard.AddNew(emptyPositions[i], ChessBoardHandler.RandomLevel());
                addEmptyBoard.SetEmpty(emptyPositions[i]);
            }
            ChessBoard emptyBoard = new ChessBoard();
            Assert.AreEqual(addEmptyBoard.EmptyCount, 16);
            Assert.AreEqual(addEmptyBoard, emptyBoard);
        }
    }
}
