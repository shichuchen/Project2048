using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project2048;

namespace Project2048MSTest
{
    using Direction = Settings.Direction;
    [TestClass]
    public class ChessBoardMoveTests
    {
        [TestMethod]
        public void TestToplLineMoveUpFail()
        {
            ChessBoard chessBoard = new ChessBoard();

            chessBoard.AddNew(new Position(0, 0), ChessBoardHandler.RandomLevel());
            chessBoard.AddNew(new Position(0, 1), ChessBoardHandler.RandomLevel());
            chessBoard.AddNew(new Position(0, 2), ChessBoardHandler.RandomLevel());
            chessBoard.AddNew(new Position(0, 3), ChessBoardHandler.RandomLevel());

            Assert.AreEqual(chessBoard.Move(Direction.Up), false);
        }

        [TestMethod]
        public void TestDistinctLineMoveUpFail()
        {
            for (int col = 0; col < 4; ++col)
            {
                ChessBoard chessBoard = new ChessBoard();
                int[] levels = RandomGenerator.GetDistinctInts(4, 16);

                chessBoard.AddNew(new Position(0, col), levels[0]);
                chessBoard.AddNew(new Position(1, col), levels[1]);
                chessBoard.AddNew(new Position(2, col), levels[2]);
                chessBoard.AddNew(new Position(3, col), levels[3]);

                Assert.AreEqual(chessBoard.Move(Direction.Up), false);
            }
        }
        [TestMethod]
        public void TestLineMoveUpSuccess()
        {
            for (int col = 0; col < 4; ++col)
            {
                ChessBoard chessBoard = new ChessBoard();
                int[] levels = RandomGenerator.GetDistinctInts(2, 16);
                int[] rows = RandomGenerator.GetDistinctInts(2, 1, 4);

                chessBoard.AddNew(new Position(rows[0], col), levels[0]);
                chessBoard.AddNew(new Position(rows[1], col), levels[1]);

                Assert.AreEqual(chessBoard.Move(Direction.Up), true);
                Assert.AreEqual(chessBoard.EmptyCount, 14);
            }
        }
        [TestMethod]
        public void TestLineMoveUpMergeOnce()
        {
            for (int col = 0; col < 4; ++col)
            {
                ChessBoard chessBoard = new ChessBoard();
                int level = RandomGenerator.Next(16);
                int[] rows = RandomGenerator.GetDistinctInts(2, 4);

                chessBoard.AddNew(new Position(rows[0], col), level);
                chessBoard.AddNew(new Position(rows[1], col), level);

                Assert.AreEqual(chessBoard.Move(Direction.Up), true);
                Assert.AreEqual(chessBoard.EmptyCount, 15);
            }
        }
        [TestMethod]
        public void TestLineMoveUpMergeTwice()
        {
            for (int col = 0; col < 4; ++col)
            {
                ChessBoard chessBoard = new ChessBoard();
                int[] levels = RandomGenerator.GetInts(2, 16);

                chessBoard.AddNew(new Position(0, col), levels[0]);
                chessBoard.AddNew(new Position(1, col), levels[0]);
                chessBoard.AddNew(new Position(2, col), levels[1]);
                chessBoard.AddNew(new Position(3, col), levels[1]);

                Assert.AreEqual(chessBoard.Move(Direction.Up), true);
                Assert.AreEqual(chessBoard.EmptyCount, 14);
            }
        }
        [TestMethod]
        public void TestBottomLineMoveDownFail()
        {
            ChessBoard chessBoard = new ChessBoard();

            chessBoard.AddNew(new Position(3, 0), ChessBoardHandler.RandomLevel());
            chessBoard.AddNew(new Position(3, 1), ChessBoardHandler.RandomLevel());
            chessBoard.AddNew(new Position(3, 2), ChessBoardHandler.RandomLevel());
            chessBoard.AddNew(new Position(3, 3), ChessBoardHandler.RandomLevel());

            Assert.AreEqual(chessBoard.Move(Direction.Down), false);
        }
        [TestMethod]
        public void TestDistinctLineMoveDownFail()
        {
            for (int col = 0; col < 4; ++col)
            {
                ChessBoard chessBoard = new ChessBoard();
                int[] levels = RandomGenerator.GetDistinctInts(4, 16);

                chessBoard.AddNew(new Position(0, col), levels[0]);
                chessBoard.AddNew(new Position(1, col), levels[1]);
                chessBoard.AddNew(new Position(2, col), levels[2]);
                chessBoard.AddNew(new Position(3, col), levels[3]);

                Assert.AreEqual(chessBoard.Move(Direction.Down), false);
            }
        }
        [TestMethod]
        public void TestLineMoveDownSuccess()
        {
            for (int col = 0; col < 4; ++col)
            {
                ChessBoard chessBoard = new ChessBoard();
                int[] levels = RandomGenerator.GetDistinctInts(2, 16);
                int[] rows = RandomGenerator.GetDistinctInts(2, 0, 3);

                chessBoard.AddNew(new Position(rows[0], col), levels[0]);
                chessBoard.AddNew(new Position(rows[1], col), levels[1]);

                Assert.AreEqual(chessBoard.Move(Direction.Up), true);
                Assert.AreEqual(chessBoard.EmptyCount, 14);
            }
        }
        [TestMethod]
        public void TestLineMoveDownMergeOnce()
        {
            for (int col = 0; col < 4; ++col)
            {
                ChessBoard chessBoard = new ChessBoard();
                int level = RandomGenerator.Next(16);
                int[] rows = RandomGenerator.GetDistinctInts(2, 4);

                chessBoard.AddNew(new Position(rows[0], col), level);
                chessBoard.AddNew(new Position(rows[1], col), level);
                Assert.AreEqual(chessBoard.Move(Direction.Down), true);

                Assert.AreEqual(chessBoard.EmptyCount, 15);
            }
        }
        [TestMethod]
        public void TestLineMoveDownMergeTwice()
        {
            for (int col = 0; col < 4; ++col)
            {
                ChessBoard chessBoard = new ChessBoard();
                int[] levels = RandomGenerator.GetInts(2, 16);

                chessBoard.AddNew(new Position(0, col), levels[0]);
                chessBoard.AddNew(new Position(1, col), levels[0]);
                chessBoard.AddNew(new Position(2, col), levels[1]);
                chessBoard.AddNew(new Position(3, col), levels[1]);
                Assert.AreEqual(chessBoard.Move(Direction.Down), true);

                Assert.AreEqual(chessBoard.EmptyCount, 14);
            }
        }
        [TestMethod]
        public void TestLeftLineMoveLeftFail()
        {
            ChessBoard chessBoard = new ChessBoard();

            chessBoard.AddNew(new Position(0, 0), ChessBoardHandler.RandomLevel());
            chessBoard.AddNew(new Position(1, 0), ChessBoardHandler.RandomLevel());
            chessBoard.AddNew(new Position(2, 0), ChessBoardHandler.RandomLevel());
            chessBoard.AddNew(new Position(3, 0), ChessBoardHandler.RandomLevel());

            Assert.AreEqual(chessBoard.Move(Direction.Left), false);
        }
        [TestMethod]
        public void TestMoveLeftOneDistinctLineFail()
        {
            for (int row = 0; row < 4; ++row)
            {
                ChessBoard chessBoard = new ChessBoard();
                int[] levels = RandomGenerator.GetDistinctInts(4, 16);

                chessBoard.AddNew(new Position(row, 0), levels[0]);
                chessBoard.AddNew(new Position(row, 1), levels[1]);
                chessBoard.AddNew(new Position(row, 2), levels[2]);
                chessBoard.AddNew(new Position(row, 3), levels[3]);

                Assert.AreEqual(chessBoard.Move(Direction.Left), false);
            }
        }
        [TestMethod]
        public void TestLineMoveLeftSuccess()
        {
            for (int row = 0; row < 4; ++row)
            {
                ChessBoard chessBoard = new ChessBoard();
                int[] levels = RandomGenerator.GetDistinctInts(2, 16);
                int[] cols = RandomGenerator.GetDistinctInts(2, 1, 4);

                chessBoard.AddNew(new Position(row, cols[0]), levels[0]);
                chessBoard.AddNew(new Position(row, cols[1]), levels[1]);

                Assert.AreEqual(chessBoard.Move(Direction.Left), true);
                Assert.AreEqual(chessBoard.EmptyCount, 15);
            }
        }
        [TestMethod]
        public void TestLineMoveLeftMergeOnce()
        {
            for (int row = 0; row < 4; ++row)
            {
                ChessBoard chessBoard = new ChessBoard();
                int level = RandomGenerator.Next(16);
                int[] cols = RandomGenerator.GetDistinctInts(2, 4);

                chessBoard.AddNew(new Position(row, cols[0]), level);
                chessBoard.AddNew(new Position(row, cols[1]), level);

                Assert.AreEqual(chessBoard.Move(Direction.Left), true);
                Assert.AreEqual(chessBoard.EmptyCount, 15);
            }
        }
        [TestMethod]
        public void TestLineMoveLeftMergeTwice()
        {
            for (int i = 0; i < 4; ++i)
            {
                ChessBoard chessBoard = new ChessBoard();
                int[] levels = RandomGenerator.GetInts(2, 16);

                chessBoard.AddNew(new Position(i, 0), levels[0]);
                chessBoard.AddNew(new Position(i, 1), levels[0]);
                chessBoard.AddNew(new Position(i, 2), levels[1]);
                chessBoard.AddNew(new Position(i, 3), levels[1]);

                Assert.AreEqual(chessBoard.Move(Direction.Left), true);
                Assert.AreEqual(chessBoard.EmptyCount, 14);
            }
        }
        [TestMethod]
        public void TestRightLineMoveRightFail()
        {
            ChessBoard chessBoard = new ChessBoard();

            chessBoard.AddNew(new Position(0, 3), ChessBoardHandler.RandomLevel());
            chessBoard.AddNew(new Position(1, 3), ChessBoardHandler.RandomLevel());
            chessBoard.AddNew(new Position(2, 3), ChessBoardHandler.RandomLevel());
            chessBoard.AddNew(new Position(3, 3), ChessBoardHandler.RandomLevel());

            Assert.AreEqual(chessBoard.Move(Direction.Right), false);
        }
        [TestMethod]
        public void TestMoveRightOneDistinctLineFail()
        {
            for (int row = 0; row < 4; ++row)
            {
                ChessBoard chessBoard = new ChessBoard();
                int[] levels = RandomGenerator.GetDistinctInts(4, 16);

                chessBoard.AddNew(new Position(row, 0), levels[0]);
                chessBoard.AddNew(new Position(row, 1), levels[1]);
                chessBoard.AddNew(new Position(row, 2), levels[2]);
                chessBoard.AddNew(new Position(row, 3), levels[3]);

                Assert.AreEqual(chessBoard.Move(Direction.Right), false);
            }
        }
        [TestMethod]
        public void TestLineMoveRightSuccess()
        {
            for (int row = 0; row < 4; ++row)
            {
                ChessBoard chessBoard = new ChessBoard();
                int[] levels = RandomGenerator.GetDistinctInts(2, 16);
                int[] cols = RandomGenerator.GetDistinctInts(2, 0, 3);

                chessBoard.AddNew(new Position(row, cols[0]), levels[0]);
                chessBoard.AddNew(new Position(row, cols[1]), levels[1]);

                Assert.AreEqual(chessBoard.Move(Direction.Right), true);
                Assert.AreEqual(chessBoard.EmptyCount, 15);
            }
        }
        [TestMethod]
        public void TestLineMoveRightMergeOnce()
        {
            for (int row = 0; row < 4; ++row)
            {
                ChessBoard chessBoard = new ChessBoard();
                int level = RandomGenerator.Next(16);
                int[] cols = RandomGenerator.GetDistinctInts(2, 4);

                chessBoard.AddNew(new Position(row, cols[0]), level);
                chessBoard.AddNew(new Position(row, cols[1]), level);

                Assert.AreEqual(chessBoard.Move(Direction.Right), true);
                Assert.AreEqual(chessBoard.EmptyCount, 15);
            }
        }
        [TestMethod]
        public void TestLineMoveRightMergeTwice()
        {
            for (int row = 0; row < 4; ++row)
            {
                ChessBoard chessBoard = new ChessBoard();
                int[] levels = RandomGenerator.GetInts(2, 16);

                chessBoard.AddNew(new Position(row, 0), levels[0]);
                chessBoard.AddNew(new Position(row, 1), levels[0]);
                chessBoard.AddNew(new Position(row, 2), levels[1]);
                chessBoard.AddNew(new Position(row, 3), levels[1]);

                Assert.AreEqual(chessBoard.Move(Direction.Right), true);
                Assert.AreEqual(chessBoard.EmptyCount, 14);
            }
        }
    }
}
