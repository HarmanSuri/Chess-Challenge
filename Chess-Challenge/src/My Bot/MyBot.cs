using ChessChallenge.API;
using System;

public class MyBot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
    int positiveInfinity = 9999999;
    int negativeInfinity = -9999999;

    public Move Think(Board board, Timer timer)
    {
        Move[] legalMoves = board.GetLegalMoves();
        Move[] whiteOpens = new Move[2] { new Move("e2e4", board), new Move("d2d4", board) };
        Move[] blackOpens = new Move[2] { new Move("e7e5", board), new Move("d7d5", board) };

        Move bestMove = Move.NullMove;
        int depth = 2;

        int bestEval = negativeInfinity;

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
            int currentEval = -Search(board, depth, negativeInfinity, positiveInfinity);
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

    int EvaluateBoard(Board board)
    {
        int boardValue = 0;

        // Material Score
        foreach (PieceList list in board.GetAllPieceLists())
        {
            foreach (Piece piece in list)
            {
                int negativeModifier = piece.IsWhite ? 1 : -1;

                boardValue += negativeModifier * pieceValues[(int)piece.PieceType];
            }
        }

        int perspective = board.IsWhiteToMove ? 1 : -1;
        return perspective * boardValue;
    }


    int Search(Board board, int depth, int alpha, int beta)
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
                return negativeInfinity;
            }
            return 0;
        }

        foreach (Move move in legalMoves)
        {
            board.MakeMove(move);
            int currentEval = -Search(board, depth - 1, -beta, -alpha);
            board.UndoMove(move);

            if (currentEval >= beta)
            {
                return beta;
            }
            alpha = Math.Max(alpha, currentEval);
        }
        return alpha;
    }

}