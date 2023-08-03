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

        if (board.IsWhiteToMove)
        {
            double bestEval = double.NegativeInfinity;

            if (board.PlyCount == 0)
            {
                //legalMoves = whiteOpens;
            }

            foreach (Move move in legalMoves)
            {
                board.MakeMove(move);
                double currentEval = Minimax(board, depth, false);
                board.UndoMove(move);
                if (currentEval > bestEval)
                {
                    bestEval = currentEval;
                    bestMove = move;
                }
            }
        }
        else
        {
            double bestEval = double.PositiveInfinity;

            if (board.PlyCount == 1)
            {
                //legalMoves = blackOpens;
            }

            foreach (Move move in legalMoves)
            {
                board.MakeMove(move);
                double currentEval = Minimax(board, depth, true);
                board.UndoMove(move);
                if (currentEval < bestEval)
                {
                    bestEval = currentEval;
                    bestMove = move;
                }
            }
        }
        Console.WriteLine(EvaluateBoard(board));
        return bestMove;
    }

    double EvaluateBoard(Board board)
    {
        double boardValue = 0;

        if (board.IsInCheckmate())
        {
            if (board.SquareIsAttackedByOpponent(board.GetKingSquare(true)))
            {
                return double.NegativeInfinity;
            }
            else
            {
                return double.PositiveInfinity;
            }
        }
        else if (board.IsDraw())
        {
            return 0;
        }

        foreach (PieceList list in board.GetAllPieceLists())
        {
            foreach (Piece piece in list)
            {
                double negativeModifier = piece.IsWhite ? 1 : -1;

                boardValue += negativeModifier * pieceValues[(int)piece.PieceType];
            }
        }

        // knights on edge
        foreach (Piece piece in board.GetPieceList(PieceType.Knight, true))
        {
            if (piece.Square.File == 0 || piece.Square.File == 7)
            {
                boardValue += -100;
            }
        }
        foreach (Piece piece in board.GetPieceList(PieceType.Knight, false))
        {
            if (piece.Square.File == 0 || piece.Square.File == 7)
            {
                boardValue += 100;
            }
        }

        return boardValue;
    }

    double Minimax(Board board, int depth, bool isMaximizingPlayer)
    {
        if (depth == 0 || board.IsInCheckmate() || board.IsDraw())
        {
            return EvaluateBoard(board);
        }

        Move[] legalMoves = board.GetLegalMoves();
        
        if (isMaximizingPlayer)
        {
            double bestEval = double.NegativeInfinity;

            foreach (Move move in legalMoves)
            {
                board.MakeMove(move);
                double currentEval = Minimax(board, depth - 1, false);
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
                double currentEval = Minimax(board, depth - 1, true);
                board.UndoMove(move);
                if (currentEval < bestEval)
                {
                    bestEval = currentEval;
                }
            }
            return bestEval;
        }
    }
}