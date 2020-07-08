using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project2048;
using System;
using System.Collections.Generic;
using System.Text;

namespace Project2048MSTest
{
    using Direction = Settings.Direction;
    [TestClass]
    public class DecisionCacheAITest
    {
        /// <summary>
        ///0       0       2        2048
        ///0       0       4        512
        ///2       4       32       128
        ///0       2       16       32
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public ChessBoard GetTestBoard1()
        {
            ChessBoard chessBoard = new ChessBoard();
            chessBoard.AddNew(new Position(0, 0), 0);
            chessBoard.AddNew(new Position(0, 1), 0);
            chessBoard.AddNew(new Position(0, 2), 1);
            chessBoard.AddNew(new Position(0, 3), 11);

            chessBoard.AddNew(new Position(1, 0), 0);
            chessBoard.AddNew(new Position(1, 1), 0);
            chessBoard.AddNew(new Position(1, 2), 2);
            chessBoard.AddNew(new Position(1, 3), 9);

            chessBoard.AddNew(new Position(2, 0), 1);
            chessBoard.AddNew(new Position(2, 1), 2);
            chessBoard.AddNew(new Position(2, 2), 5);
            chessBoard.AddNew(new Position(2, 3), 7);

            chessBoard.AddNew(new Position(3, 0), 0);
            chessBoard.AddNew(new Position(3, 1), 1);
            chessBoard.AddNew(new Position(3, 2), 4);
            chessBoard.AddNew(new Position(3, 3), 5);
            return chessBoard;
        }

        [TestMethod]
        public void Test1()
        {
            var chessBoard = GetTestBoard1();
            chessBoard.Print();
            var ai = new DecisionCacheAlphaBetaAI(chessBoard);
            var direction = ai.GetMoveDirection();
            Assert.AreNotEqual(direction, Direction.Left);
        }
    }
}
