using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project2048;

namespace Project2048MSTest
{
    [TestClass]
    public class AlphaBetaAITest
    {
        /// <summary>
        ///4       64      2048    4096
        ///16      2       64      128
        ///0       2       8       4
        ///0       0       0       4
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public ChessBoard GetTestBoard1()
        {
            ChessBoard chessBoard = new ChessBoard();
            chessBoard.AddNew(new Position(0, 0), 2);
            chessBoard.AddNew(new Position(0, 1), 6);
            chessBoard.AddNew(new Position(0, 2), 11);
            chessBoard.AddNew(new Position(0, 3), 12);

            chessBoard.AddNew(new Position(1, 0), 4);
            chessBoard.AddNew(new Position(1, 1), 1);
            chessBoard.AddNew(new Position(1, 2), 6);
            chessBoard.AddNew(new Position(1, 3), 7);

            chessBoard.AddNew(new Position(2, 0), 0);
            chessBoard.AddNew(new Position(2, 1), 1);
            chessBoard.AddNew(new Position(2, 2), 3);
            chessBoard.AddNew(new Position(2, 3), 2);

            chessBoard.AddNew(new Position(3, 0), 0);
            chessBoard.AddNew(new Position(3, 1), 0);
            chessBoard.AddNew(new Position(3, 2), 0);
            chessBoard.AddNew(new Position(3, 3), 2);
            return chessBoard;
        }
        /// <summary>
        ///0       0       0       0
        ///4       0       2       0
        ///8       8       1       0
        ///2048    256     8       0
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public ChessBoard GetTestBoard2()
        {
            ChessBoard chessBoard = new ChessBoard();
            chessBoard.AddNew(new Position(0, 0), 0);
            chessBoard.AddNew(new Position(0, 1), 0);
            chessBoard.AddNew(new Position(0, 2), 0);
            chessBoard.AddNew(new Position(0, 3), 0);

            chessBoard.AddNew(new Position(1, 0), 2);
            chessBoard.AddNew(new Position(1, 1), 0);
            chessBoard.AddNew(new Position(1, 2), 1);
            chessBoard.AddNew(new Position(1, 3), 0);

            chessBoard.AddNew(new Position(2, 0), 3);
            chessBoard.AddNew(new Position(2, 1), 3);
            chessBoard.AddNew(new Position(2, 2), 2);
            chessBoard.AddNew(new Position(2, 3), 0);

            chessBoard.AddNew(new Position(3, 0), 11);
            chessBoard.AddNew(new Position(3, 1), 8);
            chessBoard.AddNew(new Position(3, 2), 3);
            chessBoard.AddNew(new Position(3, 3), 0);
            return chessBoard;
        }
        [TestMethod]
        public void Board1Test()
        {
            var chessBoard = GetTestBoard1();
            chessBoard.Print();
            var AI = new AlphaBetaAI(chessBoard);
            var direction = AI.GetMoveDirection();
            Assert.AreNotEqual(direction, Settings.Direction.Down);
        }
        //[TestMethod]
        //public void Board2Test()
        //{
        //    var chessBoard = GetTestBoard2();
        //    chessBoard.Print();
        //    var AI = new AlphaBetaAI(chessBoard);
        //    var direction = AI.GetTestMoveDirection();
        //    Assert.AreNotEqual(direction, Settings.Direction.Right);
        //}

    }
}
