using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckersLogic
{
    public class AI
    {
        private GameLogic m_GameLogic;
        private int m_LastRow;
        private int m_LastCol;

        public AI(ref GameLogic i_GameLogic)
        {
            m_GameLogic = i_GameLogic;
            InitAI();
        }

        public void InitAI()
        {
            m_LastRow = ((m_GameLogic.GetBoardSize() - 2) / 2) - 1;
            m_LastCol = 0;
        }

        private int[,] getAllPiecesAbleToMove(out int o_NumberOfPieces)
        {
            int numberOfPiecesLeft = m_GameLogic.GetNumberOfPiecesLeftPlayerTwo();
            int[,] pieces = new int[numberOfPiecesLeft, 2];
            ePawnTypes[,] board = m_GameLogic.GetBoard();
            int currentPieceNumber = 0;

            for(int i = 0; i < m_GameLogic.GetBoardSize(); i++)
            {
                for(int j = 0; j < m_GameLogic.GetBoardSize(); j++)
                {
                    if(board[i, j] == ePawnTypes.PlayerTwo || board[i, j] == ePawnTypes.PlayerTwoKing)
                    {
                        if(m_GameLogic.IsAbleToMove(i, j))
                        {
                            pieces[currentPieceNumber, 0] = i;
                            pieces[currentPieceNumber, 1] = j;
                            currentPieceNumber++;
                        }
                    }
                }
            }

            o_NumberOfPieces = currentPieceNumber;
            return pieces;
        }

        private int[,] getPieceLegalMoves(int i_FromRow, int i_FromCol,bool i_MustEat, out int o_NumberOfLegalMoves)
        {
            int numberOfLegalMoves = 0;
            int[,] legalMoves = new int[8, 2];

            if(m_GameLogic.CheckMoveLegalityPlayerTwo(i_FromRow, i_FromCol, i_FromRow - 1, i_FromCol - 1, i_MustEat))
            {
                legalMoves[numberOfLegalMoves, 0] = i_FromRow - 1;
                legalMoves[numberOfLegalMoves, 1] = i_FromCol - 1;
                numberOfLegalMoves++;

            }

            if(m_GameLogic.CheckMoveLegalityPlayerTwo(i_FromRow, i_FromCol, i_FromRow - 1, i_FromCol + 1, i_MustEat))
            {
                legalMoves[numberOfLegalMoves, 0] = i_FromRow - 1;
                legalMoves[numberOfLegalMoves, 1] = i_FromCol + 1;
                numberOfLegalMoves++;

            }

            if(m_GameLogic.CheckMoveLegalityPlayerTwo(i_FromRow, i_FromCol, i_FromRow + 1, i_FromCol - 1, i_MustEat))
            {
                legalMoves[numberOfLegalMoves, 0] = i_FromRow + 1;
                legalMoves[numberOfLegalMoves, 1] = i_FromCol - 1;
                numberOfLegalMoves++;

            }

            if(m_GameLogic.CheckMoveLegalityPlayerTwo(i_FromRow, i_FromCol, i_FromRow + 1, i_FromCol + 1, i_MustEat))
            {
                legalMoves[numberOfLegalMoves, 0] = i_FromRow + 1;
                legalMoves[numberOfLegalMoves, 1] = i_FromCol + 1;
                numberOfLegalMoves++;

            }

            if(m_GameLogic.CheckMoveLegalityPlayerTwo(i_FromRow, i_FromCol, i_FromRow - 2, i_FromCol - 2, i_MustEat))
            {
                legalMoves[numberOfLegalMoves, 0] = i_FromRow - 2;
                legalMoves[numberOfLegalMoves, 1] = i_FromCol - 2;
                numberOfLegalMoves++;

            }

            if(m_GameLogic.CheckMoveLegalityPlayerTwo(i_FromRow, i_FromCol, i_FromRow - 2, i_FromCol + 2, i_MustEat))
            {
                legalMoves[numberOfLegalMoves, 0] = i_FromRow - 2;
                legalMoves[numberOfLegalMoves, 1] = i_FromCol + 2;
                numberOfLegalMoves++;

            }

            if(m_GameLogic.CheckMoveLegalityPlayerTwo(i_FromRow, i_FromCol, i_FromRow + 2, i_FromCol - 2, i_MustEat))
            {
                legalMoves[numberOfLegalMoves, 0] = i_FromRow + 2;
                legalMoves[numberOfLegalMoves, 1] = i_FromCol - 2;
                numberOfLegalMoves++;

            }

            if(m_GameLogic.CheckMoveLegalityPlayerTwo(i_FromRow, i_FromCol, i_FromRow + 2, i_FromCol + 2, i_MustEat))
            {
                legalMoves[numberOfLegalMoves, 0] = i_FromRow + 2;
                legalMoves[numberOfLegalMoves, 1] = i_FromCol + 2;
                numberOfLegalMoves++;

            }

            o_NumberOfLegalMoves = numberOfLegalMoves;

            return legalMoves;
        }

        private int[,] getRandomMove(int[,] i_Pieces, int i_NumberOfPieces, bool i_MustEat)
        {
            Random random = new Random();
            int[,] randomMove = new int[2, 2];
            int randomPieceNumber = random.Next(i_NumberOfPieces);
            int[,] pieceMoves = getPieceLegalMoves(i_Pieces[randomPieceNumber, 0], i_Pieces[randomPieceNumber, 1],i_MustEat , out int numberOfMoves);
            int randomMoveNumber = random.Next(numberOfMoves);
            
            randomMove[0, 0] = i_Pieces[randomPieceNumber, 0];
            randomMove[0, 1] = i_Pieces[randomPieceNumber, 1];
            randomMove[1, 0] = pieceMoves[randomMoveNumber, 0];
            randomMove[1, 1] = pieceMoves[randomMoveNumber, 1];

            return randomMove;
        }

        public bool MakeMove(out int[,] i_OChosenMove)
        {
            bool success = false;
            int[,] pieces;
            int numberOfPiecesAbleToMove = 0;
            i_OChosenMove = new int[2,2];

            if(m_GameLogic.GetIsContinuesTurn())
            {
                pieces = new int[1, 2] { { m_LastRow, m_LastCol } };
                m_GameLogic.GetAllValidEatingMovesPlayerTwo(m_LastRow, m_LastCol, true, out numberOfPiecesAbleToMove);
            }
            else

            {
                pieces = getAllPiecesAbleToMove(out numberOfPiecesAbleToMove);
            }
            
            if(numberOfPiecesAbleToMove != 0)
            {
                int[,] move = getRandomMove(pieces, numberOfPiecesAbleToMove, m_GameLogic.GetIsContinuesTurn());
                i_OChosenMove = move;
                if(m_GameLogic.CheckAndMove(move[0,0], move[0,1], move[1,0], move[1,1]))
                {
                    m_LastRow = move[1, 0];
                    m_LastCol = move[1, 1];

                    success = true;
                }
            }

            return success;
        }

    }
}
