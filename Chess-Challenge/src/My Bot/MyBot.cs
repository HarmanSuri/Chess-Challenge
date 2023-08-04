using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };

    public Move Think(Board board, Timer timer)
    {
        Move[] legalMoves = board.GetLegalMoves();
        Move[] whiteOpens = new Move[2] { new Move("e2e4", board), new Move("d2d4", board) };
        Move[] blackOpens = new Move[2] { new Move("e7e5", board), new Move("d7d5", board) };

        Move bestMove = Move.NullMove;
        int depth = 2;

        double bestEval = double.NegativeInfinity;

        if (board.PlyCount == 0)
        {
            legalMoves = whiteOpens;
        }
        else if (board.PlyCount == 1)
        {
            legalMoves = blackOpens;
        }

        foreach (Move move in legalMoves)
        {
            board.MakeMove(move);
            double currentEval = -Search(board, depth, false);
            board.UndoMove(move);
            if (currentEval > bestEval)
            {
                bestEval = currentEval;
                bestMove = move;
            }
        }
        //Console.WriteLine(EvaluateBoard(board));
        return bestMove;
    }

    double EvaluateBoard(Board board)
    {
        double boardValue = 0;

        // Material Score
        foreach (PieceList list in board.GetAllPieceLists())
        {
            foreach (Piece piece in list)
            {
                double negativeModifier = piece.IsWhite ? 1 : -1;

                boardValue += negativeModifier * pieceValues[(int)piece.PieceType];
            }
        }

        int perspective = board.IsWhiteToMove ? 1 : -1;
        return perspective * boardValue;
    }

    double Search(Board board, int depth, bool isMaximizingPlayer)
    {
        if (depth == 0)
        {
            return EvaluateBoard(board);
        }

        Move[] legalMoves = board.GetLegalMoves();

        if (legalMoves.Length == 0)
        {
            if (board.IsInCheck())
            {

                return double.NegativeInfinity;

            }
            return 0;
        }

        double bestEval = double.NegativeInfinity;

        foreach (Move move in legalMoves)
        {
            board.MakeMove(move);
            double currentEval = -Search(board, depth - 1, false);
            bestEval = Math.Max(bestEval, currentEval);
            board.UndoMove(move);
        }
        return bestEval;

        /*if (isMaximizingPlayer)
        {
            double bestEval = double.NegativeInfinity;

            foreach (Move move in legalMoves)
            {
                board.MakeMove(move);
                double currentEval = Search(board, depth - 1, false);
                board.UndoMove(move); 
                if (currentEval > bestEval)
                {
                    bestEval = currentEval;
                }
            }
            return bestEval;
        }
        else
        {
            double bestEval = double.PositiveInfinity;

            foreach (Move move in legalMoves)
            {
                board.MakeMove(move);
                double currentEval = Search(board, depth - 1, true);
                board.UndoMove(move);
                if (currentEval < bestEval)
                {
                    bestEval = currentEval;
                }
            }
            return bestEval;
        }*/
    }
}