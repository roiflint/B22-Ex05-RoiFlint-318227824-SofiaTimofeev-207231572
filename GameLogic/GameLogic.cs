using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;

namespace CheckersLogic
{
    public class GameLogic
    {
        private Board m_Board;
        private int m_NumberOfPlayerOnePieces;
        private int m_NumberOfPlayerTwoPieces;
        private int m_TurnNumber;
        private bool m_IsContinuesTurn;


        public GameLogic(int i_BoardSize)
        {
            m_Board = new Board(i_BoardSize);
            NewGame();
        }

        public void NewGame()
        {
            m_NumberOfPlayerOnePieces = (m_Board.GetBoardSize() / 2) * ((m_Board.GetBoardSize() - 2) / 2);
            m_NumberOfPlayerTwoPieces = (m_Board.GetBoardSize() / 2) * ((m_Board.GetBoardSize() - 2) / 2);
            m_TurnNumber = 1;
            m_Board.InitBoard();
            m_IsContinuesTurn = false;
        }

        public ePawnTypes[,] GetBoard()
        {
            return m_Board.GetBoard();
        }

        public int GetBoardSize()
        {
            return m_Board.GetBoardSize();
        }

        private bool checkMoveLegalityPlayerOne(int i_FromRow, int i_FromCol, int i_ToRow, int i_ToCol, bool i_MustEat)
        {
            bool isLegal = false;

            if(checkValidMove(i_ToRow, i_ToCol))
            {
                ePawnTypes pieceSymbol = m_Board.GetSymbolAtLocation(i_FromRow, i_FromCol);
                ePawnTypes eatenPieceSymbol = ePawnTypes.Empty;
                ePawnTypes movedToSymbol = m_Board.GetSymbolAtLocation(i_ToRow, i_ToCol);

                if(pieceSymbol != ePawnTypes.Empty && movedToSymbol == ePawnTypes.Empty)
                {
                    if((pieceSymbol == ePawnTypes.PlayerOne || pieceSymbol == ePawnTypes.PlayerOneKing))
                    {
                        if(i_FromRow - 1 == i_ToRow && (i_FromCol + 1 == i_ToCol || i_FromCol - 1 == i_ToCol)
                                                    && !i_MustEat)
                        {
                            isLegal = true;
                        }

                        else if(i_FromRow - 2 == i_ToRow)
                        {
                            if(i_FromCol + 2 == i_ToCol)
                            {
                                eatenPieceSymbol = m_Board.GetSymbolAtLocation(i_FromRow - 1, i_FromCol + 1);
                                if(eatenPieceSymbol == ePawnTypes.PlayerTwo
                                   || eatenPieceSymbol == ePawnTypes.PlayerTwoKing)
                                {
                                    isLegal = true;
                                }

                            }

                            else if(i_FromCol - 2 == i_ToCol)
                            {
                                eatenPieceSymbol = m_Board.GetSymbolAtLocation(i_FromRow - 1, i_FromCol - 1);
                                if(eatenPieceSymbol == ePawnTypes.PlayerTwo
                                   || eatenPieceSymbol == ePawnTypes.PlayerTwoKing)
                                {
                                    isLegal = true;
                                }

                            }

                        }

                        else if(pieceSymbol == ePawnTypes.PlayerOneKing)
                        {
                            if(i_FromRow + 1 == i_ToRow && (i_FromCol + 1 == i_ToCol || i_FromCol - 1 == i_ToCol)
                                                        && !i_MustEat)
                            {
                                isLegal = true;
                            }

                            else if(i_FromRow + 2 == i_ToRow)
                            {
                                if(i_FromCol + 2 == i_ToCol)
                                {
                                    eatenPieceSymbol = m_Board.GetSymbolAtLocation(i_FromRow + 1, i_FromCol + 1);
                                    if(eatenPieceSymbol == ePawnTypes.PlayerTwo
                                       || eatenPieceSymbol == ePawnTypes.PlayerTwoKing)
                                    {
                                        isLegal = true;
                                    }

                                }

                                else if(i_FromCol - 2 == i_ToCol)
                                {
                                    eatenPieceSymbol = m_Board.GetSymbolAtLocation(i_FromRow + 1, i_FromCol - 1);
                                    if(eatenPieceSymbol == ePawnTypes.PlayerTwo
                                       || eatenPieceSymbol == ePawnTypes.PlayerTwoKing)
                                    {
                                        isLegal = true;
                                    }

                                }

                            }

                        }

                    }

                }

            }


            return isLegal;
        }

        public bool CheckMoveLegalityPlayerTwo(int i_FromRow, int i_FromCol, int i_ToRow, int i_ToCol, bool i_MustEat)
        {
            bool isLegal = false;

            if(checkValidMove(i_ToRow, i_ToCol))
            {
                ePawnTypes pieceSymbol = m_Board.GetSymbolAtLocation(i_FromRow, i_FromCol);
                ePawnTypes eatenPieceSymbol = ePawnTypes.Empty;
                ePawnTypes movedToSymbol = m_Board.GetSymbolAtLocation(i_ToRow, i_ToCol);

                if (pieceSymbol != ePawnTypes.Empty && movedToSymbol == ePawnTypes.Empty)
                {
                    if((pieceSymbol == ePawnTypes.PlayerTwo || pieceSymbol == ePawnTypes.PlayerTwoKing))
                    {
                        if(i_FromRow + 1 == i_ToRow && (i_FromCol + 1 == i_ToCol || i_FromCol - 1 == i_ToCol)
                                                    && !i_MustEat)
                        {
                            isLegal = true;
                        }

                        else if(i_FromRow + 2 == i_ToRow)
                        {
                            if(i_FromCol + 2 == i_ToCol)
                            {
                                eatenPieceSymbol = m_Board.GetSymbolAtLocation(i_FromRow + 1, i_FromCol + 1);
                                if(eatenPieceSymbol == ePawnTypes.PlayerOne
                                   || eatenPieceSymbol == ePawnTypes.PlayerOneKing)
                                {
                                    isLegal = true;
                                }

                            }

                            else if(i_FromCol - 2 == i_ToCol)
                            {
                                eatenPieceSymbol = m_Board.GetSymbolAtLocation(i_FromRow + 1, i_FromCol - 1);
                                if(eatenPieceSymbol == ePawnTypes.PlayerOne
                                   || eatenPieceSymbol == ePawnTypes.PlayerOneKing)
                                {
                                    isLegal = true;
                                }

                            }

                        }
                        else if (pieceSymbol == ePawnTypes.PlayerTwoKing)
                        {
                            if (i_FromRow - 1 == i_ToRow && (i_FromCol + 1 == i_ToCol || i_FromCol - 1 == i_ToCol)
                                                         && !i_MustEat)
                            {
                                isLegal = true;
                            }

                            else if (i_FromRow - 2 == i_ToRow)
                            {
                                if (i_FromCol + 2 == i_ToCol)
                                {
                                    eatenPieceSymbol = m_Board.GetSymbolAtLocation(i_FromRow - 1, i_FromCol + 1);
                                    if (eatenPieceSymbol == ePawnTypes.PlayerOne
                                        || eatenPieceSymbol == ePawnTypes.PlayerOneKing)
                                    {
                                        isLegal = true;
                                    }

                                }

                                else if (i_FromCol - 2 == i_ToCol)
                                {
                                    eatenPieceSymbol = m_Board.GetSymbolAtLocation(i_FromRow - 1, i_FromCol - 1);
                                    if (eatenPieceSymbol == ePawnTypes.PlayerOne
                                        || eatenPieceSymbol == ePawnTypes.PlayerOneKing)
                                    {
                                        isLegal = true;
                                    }

                                }

                            }

                        }

                    }

                    
                }
            }

            return isLegal;
        }

        private void checkAndCrownPiece(int i_Row, int i_Col)
        {
            ePawnTypes pieceSymbol = m_Board.GetSymbolAtLocation(i_Row, i_Col);

            if (pieceSymbol != ePawnTypes.Empty)
            {
                if (pieceSymbol == ePawnTypes.PlayerOne && i_Row == 0)
                {
                    m_Board.SetSymbolAtLocation(i_Row, i_Col, ePawnTypes.PlayerOneKing);
                }

                else if (pieceSymbol == ePawnTypes.PlayerTwo && i_Row == m_Board.GetBoardSize() - 1)
                {
                    m_Board.SetSymbolAtLocation(i_Row, i_Col, ePawnTypes.PlayerTwoKing);
                }

            }

        }

        private bool checkAndEatPiece(int i_FromRow, int i_FromCol, int i_ToRow, int i_ToCol)
        {
            bool wasEaten = false;

            if(i_FromRow == i_ToRow + 2)
            {
                if (i_FromCol == i_ToCol + 2)
                {
                    m_Board.SetSymbolAtLocation(i_ToRow + 1, i_ToCol + 1, ePawnTypes.Empty);
                    wasEaten = true;
                }

                else if (i_FromCol == i_ToCol - 2)
                {
                    m_Board.SetSymbolAtLocation(i_ToRow + 1, i_ToCol - 1, ePawnTypes.Empty);
                    wasEaten = true;
                }

            }

            else if(i_FromRow == i_ToRow - 2)
            {
                if (i_FromCol == i_ToCol + 2)
                {
                    m_Board.SetSymbolAtLocation(i_ToRow - 1, i_ToCol + 1, ePawnTypes.Empty);
                    wasEaten = true;
                }

                else if (i_FromCol == i_ToCol - 2)
                {
                    m_Board.SetSymbolAtLocation(i_ToRow - 1, i_ToCol - 1, ePawnTypes.Empty);
                    wasEaten = true;
                }

            }

            return wasEaten;
        }

        public bool CheckAndMove(int i_FromRow, int i_FromCol, int i_ToRow, int i_ToCol)
        {
            bool isLegal = false;
            bool wasEaten = false;

            if(IsFirstPlayerTurn())
            {
                if(checkMoveLegalityPlayerOne(i_FromRow, i_FromCol, i_ToRow, i_ToCol, m_IsContinuesTurn))
                {
                    if(checkAndEatPiece(i_FromRow, i_FromCol, i_ToRow, i_ToCol))
                    {
                        m_NumberOfPlayerTwoPieces--;
                        wasEaten = true;
                    }

                    m_Board.MovePiece(i_FromRow, i_FromCol, i_ToRow, i_ToCol);
                    checkAndCrownPiece(i_ToRow, i_ToCol);
                    isLegal = true;
                }

            }

            else if(CheckMoveLegalityPlayerTwo(i_FromRow, i_FromCol, i_ToRow, i_ToCol, m_IsContinuesTurn))
            {
                if (checkAndEatPiece(i_FromRow, i_FromCol, i_ToRow, i_ToCol))
                {
                    m_NumberOfPlayerOnePieces--;
                    wasEaten = true;
                }

                m_Board.MovePiece(i_FromRow, i_FromCol, i_ToRow, i_ToCol);
                checkAndCrownPiece(i_ToRow, i_ToCol);
                isLegal = true;
            }

            if(isLegal) 
            {
                if(wasEaten)
                {
                    int numberOfMoves = 0;
                    if (IsFirstPlayerTurn())
                    {
                        GetAllValidEatingMovesPlayerOne(i_ToRow, i_ToCol, true, out numberOfMoves);
                    }

                    else
                    {
                        GetAllValidEatingMovesPlayerTwo(i_ToRow, i_ToCol, true, out numberOfMoves);
                    }
                    
                    if(numberOfMoves != 0)
                    {
                        m_IsContinuesTurn = true;
                    }

                    else
                    {
                        m_TurnNumber++;
                        m_IsContinuesTurn = false;
                    }

                }

                else
                {
                    m_TurnNumber++;
                    m_IsContinuesTurn = false;
                }
                
            }

            
            return isLegal;
        }

        public bool CheckPlayerOneWin()
        {
            return (m_NumberOfPlayerTwoPieces == 0) ? true : false;
        }

        public bool CheckPlayerTwoWin()
        {
            return (m_NumberOfPlayerOnePieces == 0) ? true : false;
        }

        public bool IsTie()
        {
            bool isTie = true;
            int boardSize = m_Board.GetBoardSize();

            for (int i = 0; i < boardSize && isTie; i++)
            {
                for(int j = 0; j < boardSize && isTie; j++)
                {
                    if(IsAbleToMove(i , j))
                    {
                        isTie = false;
                    }
                }
            }

            return isTie;
        }

        private bool checkValidMove(int i_ToRow, int i_ToCol)
        {
            bool isValid = !(i_ToRow < 0 || i_ToRow > m_Board.GetBoardSize() - 1 || i_ToCol < 0 || i_ToCol > m_Board.GetBoardSize() - 1);

            return isValid;
        }

        public bool IsAbleToMove(int i_Row, int i_Col)
        {
            bool ableToMove = true;
            ePawnTypes pieceSymbol = m_Board.GetSymbolAtLocation(i_Row, i_Col);

            if (pieceSymbol == ePawnTypes.PlayerOne || pieceSymbol == ePawnTypes.PlayerOneKing)
            {
                if(!checkMoveLegalityPlayerOne(i_Row, i_Col, i_Row - 1, i_Col - 1, false))
                {
                    if (!checkMoveLegalityPlayerOne(i_Row, i_Col, i_Row - 1, i_Col + 1, false))
                    {
                        if (!checkMoveLegalityPlayerOne(i_Row, i_Col, i_Row + 1, i_Col - 1, false))
                        {
                            if (!checkMoveLegalityPlayerOne(i_Row, i_Col, i_Row + 1, i_Col + 1, false))
                            {
                                if (!checkMoveLegalityPlayerOne(i_Row, i_Col, i_Row - 2, i_Col - 2, false))
                                {
                                    if (!checkMoveLegalityPlayerOne(i_Row, i_Col, i_Row - 2, i_Col + 2, false))
                                    {
                                        if (!checkMoveLegalityPlayerOne(i_Row, i_Col, i_Row + 2, i_Col - 2, false))
                                        {
                                            if (!checkMoveLegalityPlayerOne(i_Row, i_Col, i_Row + 2, i_Col + 2, false))
                                            {
                                                ableToMove = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (pieceSymbol == ePawnTypes.PlayerTwo || pieceSymbol == ePawnTypes.PlayerTwoKing)
            {
                if (!CheckMoveLegalityPlayerTwo(i_Row, i_Col, i_Row - 1, i_Col - 1, false))
                {
                    if (!CheckMoveLegalityPlayerTwo(i_Row, i_Col, i_Row - 1, i_Col + 1, false))
                    {
                        if (!CheckMoveLegalityPlayerTwo(i_Row, i_Col, i_Row + 1, i_Col - 1, false))
                        {
                            if (!CheckMoveLegalityPlayerTwo(i_Row, i_Col, i_Row + 1, i_Col + 1, false))
                            {
                                if (!CheckMoveLegalityPlayerTwo(i_Row, i_Col, i_Row - 2, i_Col - 2, false))
                                {
                                    if (!CheckMoveLegalityPlayerTwo(i_Row, i_Col, i_Row - 2, i_Col + 2, false))
                                    {
                                        if (!CheckMoveLegalityPlayerTwo(i_Row, i_Col, i_Row + 2, i_Col - 2, false))
                                        {
                                            if (!CheckMoveLegalityPlayerTwo(i_Row, i_Col, i_Row + 2, i_Col + 2, false))
                                            {
                                                ableToMove = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                ableToMove = false;
            }

            return ableToMove;
        }
        
        public bool IsFirstPlayerTurn()
        {
            return (m_TurnNumber % 2 == 1) ? true : false;
        }

        public int GetWinnerScore()
        {
            return m_NumberOfPlayerOnePieces > m_NumberOfPlayerTwoPieces
                       ? m_NumberOfPlayerOnePieces
                       : m_NumberOfPlayerTwoPieces;
        }

        public int GetNumberOfPiecesLeftPlayerOne()
        {
            return m_NumberOfPlayerOnePieces;
        }

        public int GetNumberOfPiecesLeftPlayerTwo()
        {
            return m_NumberOfPlayerTwoPieces;
        }

        public int[,] GetAllValidEatingMovesPlayerOne(int i_Row, int i_Col, bool i_MustEat, out int o_NumberOfMoves)
        {
            int count = 0;
            int[,] validMoves = new int[4, 2];

            if(m_Board.GetSymbolAtLocation(i_Row, i_Col) == ePawnTypes.PlayerOneKing)
            {
                if(checkMoveLegalityPlayerOne(i_Row, i_Col, i_Row + 2, i_Col + 2, i_MustEat))
                {
                    validMoves[count, 0] = i_Row + 2;
                    validMoves[count, 1] = i_Col + 2;
                    count++;
                }

                if(checkMoveLegalityPlayerOne(i_Row, i_Col, i_Row + 2, i_Col - 2, i_MustEat))
                {
                    validMoves[count, 0] = i_Row + 2;
                    validMoves[count, 1] = i_Col - 2;
                    count++;
                }
            }

            if (checkMoveLegalityPlayerOne(i_Row, i_Col, i_Row - 2, i_Col + 2, i_MustEat))
            {
                validMoves[count, 0] = i_Row - 2;
                validMoves[count, 1] = i_Col + 2;
                count++;
            }

            if (checkMoveLegalityPlayerOne(i_Row, i_Col, i_Row - 2, i_Col - 2, i_MustEat))
            {
                validMoves[count, 0] = i_Row - 2;
                validMoves[count, 1] = i_Col - 2;
                count++;
            }

            o_NumberOfMoves = count;
            return validMoves;
        }
        
        public int[,] GetAllValidEatingMovesPlayerTwo(int i_Row, int i_Col, bool i_MustEat ,out int o_NumberOfMoves)
        {
            int count = 0;
            int[,] validMoves = new int[4, 2];

            if (CheckMoveLegalityPlayerTwo(i_Row, i_Col, i_Row + 2, i_Col + 2, i_MustEat))
            {
                validMoves[count, 0] = i_Row + 2;
                validMoves[count, 1] = i_Col + 2;
                count++;
            }

            if (CheckMoveLegalityPlayerTwo(i_Row, i_Col, i_Row + 2, i_Col - 2, i_MustEat))
            {
                validMoves[count, 0] = i_Row + 2;
                validMoves[count, 1] = i_Col - 2;
                count++;
            }

            if(m_Board.GetSymbolAtLocation(i_Row, i_Col) == ePawnTypes.PlayerTwoKing)
            {
                if(CheckMoveLegalityPlayerTwo(i_Row, i_Col, i_Row - 2, i_Col + 2, i_MustEat))
                {
                    validMoves[count, 0] = i_Row - 2;
                    validMoves[count, 1] = i_Col + 2;
                    count++;
                }

                if(CheckMoveLegalityPlayerTwo(i_Row, i_Col, i_Row - 2, i_Col - 2, i_MustEat))
                {
                    validMoves[count, 0] = i_Row - 2;
                    validMoves[count, 1] = i_Col - 2;
                    count++;
                }
            }

            o_NumberOfMoves = count;
            return validMoves;
        }

        public bool GetIsContinuesTurn()
        {
            return m_IsContinuesTurn;
        }

        public void SetIsContinuesTurn(bool i_isContinuesTurn)
        {
            m_IsContinuesTurn = i_isContinuesTurn;
        }
    }
}
