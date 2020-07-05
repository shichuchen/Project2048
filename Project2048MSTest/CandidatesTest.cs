using Microsoft.VisualStudio.TestTools.UnitTesting;
using Project2048;

namespace Project2048MSTest
{
    [TestClass]
    public class CandidatesTest
    {
        [TestMethod]
        public void CandidatesChooseTest()
        {
            ChessBoard chessBoard = new ChessBoard();
            var newBoard = new ChessBoard(chessBoard);
            var candidates = new Candidates(chessBoard);
            Assert.AreEqual(newBoard, chessBoard);
        }
    }
}
