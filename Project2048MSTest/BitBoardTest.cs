using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project2048.Core;

namespace Project2048MSTest
{
    [TestClass]
    public class BitBoardTest
    {
        [TestMethod]
        public void TransposeLeftTest()
        {
            for (int i = 0; i < 100; ++i)
            {
                var chessBoard = new ChessBoard();
                ChessBoardHandler.AddLeftDiagonal(chessBoard);
                ChessBoardHandler.AddSymmetryToLeftDiagonal(chessBoard);

                Assert.AreEqual(chessBoard, chessBoard.ToTransposeLeft());
                chessBoard.RandomAdd();
                Assert.AreNotEqual(chessBoard, chessBoard.ToTransposeLeft());
            }
        }
        [TestMethod]
        public void TransposeRightTest()
        {
            for (int i = 0; i < 100; ++i)
            {
                var chessBoard = new ChessBoard();
                ChessBoardHandler.AddRightDiagonal(chessBoard);
                ChessBoardHandler.AddSymmetryToRightDiagonal(chessBoard);

                Assert.AreEqual(chessBoard, chessBoard.ToTransposeRight());
                chessBoard.RandomAdd();
                Assert.AreNotEqual(chessBoard, chessBoard.ToTransposeRight());
            }
        }
    }
}
