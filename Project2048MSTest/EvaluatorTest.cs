using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project2048;

namespace Project2048MSTest
{
    [TestClass]
    public class EvaluatorTest
    {
        [TestMethod]
        public void TransposeLeftTest()
        {
            for (int i = 0; i < 100; ++i)
            {
                ChessBoard chessBoard = new ChessBoard();
                ChessBoardHandler.AddLeftDiagonal(chessBoard);
                chessBoard.AddNew(new Position(0, 3), 10);
                chessBoard.AddNew(new Position(3, 0), 10);
                Assert.AreEqual(chessBoard, chessBoard.ToTransposeLeft());
                chessBoard.RandomAdd();
                Assert.AreNotEqual(chessBoard, chessBoard.ToTransposeLeft());
                Assert.AreEqual(Evaluator.EvalForMove(chessBoard), Evaluator.EvalForMove(chessBoard.ToTransposeLeft()), 0.01);
                Assert.AreEqual(Evaluator.EvalForAdd(chessBoard), Evaluator.EvalForAdd(chessBoard.ToTransposeLeft()), 0.01);
            }
        }
        [TestMethod]
        public void TransposeRightTest()
        {
            for (int i = 0; i < 100; ++i)
            {
                ChessBoard chessBoard = new ChessBoard();
                ChessBoardHandler.AddRightDiagonal(chessBoard);
                chessBoard.AddNew(new Position(0, 0), 10);
                chessBoard.AddNew(new Position(3, 3), 10);
                Assert.AreEqual(chessBoard, chessBoard.ToTransposeRight());
                chessBoard.RandomAdd();
                Assert.AreNotEqual(chessBoard, chessBoard.ToTransposeRight());
                Assert.AreEqual(Evaluator.EvalForMove(chessBoard), Evaluator.EvalForMove(chessBoard.ToTransposeRight()), 0.01);
                Assert.AreEqual(Evaluator.EvalForAdd(chessBoard), Evaluator.EvalForAdd(chessBoard.ToTransposeRight()), 0.01);
            }
        }


    }
}
