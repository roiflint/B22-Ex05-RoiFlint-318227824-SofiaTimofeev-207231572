using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CheckersLogic
{
    public class Board
    {
        private int m_BoardSize;
        private ePawnTypes[,] m_Board;

        public Board(int i_BoardSize)
        {
            m_BoardSize = i_BoardSize;
            m_Board = new ePawnTypes[m_BoardSize, m_BoardSize];
            
            InitBoard();
        }

        public void InitBoard()
        {
            int numberOfStartingRows = m_BoardSize - 2;
            int numberOfRowsPerPlayer = numberOfStartingRows / 2;
            ePawnTypes piece = ePawnTypes.PlayerTwo;

            for(int i = 0; i < m_BoardSize; i++)
            {
                for(int j = 0; j < m_BoardSize; j++)
                {
                    if((i % 2 == 0 && j % 2 != 0) || (i % 2 != 0 && j % 2 == 0))
                    {
                        m_Board[i, j] = piece;
                    }

                    else
                    {
                        m_Board[i, j] = ePawnTypes.Empty;
                    }
                }

                if(i == (numberOfRowsPerPlayer + 1))
                {
                    piece = ePawnTypes.PlayerOne;
                }

                if(i == (numberOfRowsPerPlayer - 1))
                {
                    piece = ePawnTypes.Empty;
                }

            }
        }

        public ePawnTypes[,] GetBoard()
        {
            return m_Board;
        }

        public int GetBoardSize()
        {
            return m_BoardSize;
        }

        public void MovePiece(int i_FromRow, int i_FromCol, int i_ToRow, int i_ToCol)
        {
            m_Board[i_ToRow, i_ToCol] = m_Board[i_FromRow, i_FromCol];
            m_Board[i_FromRow, i_FromCol] = ePawnTypes.Empty;
        }

        public ePawnTypes GetSymbolAtLocation(int i_Row, int i_Col)
        {
            return m_Board[i_Row, i_Col];
        }

        public void SetSymbolAtLocation(int i_Row, int i_Col, ePawnTypes i_PawnType)
        {
            m_Board[i_Row, i_Col] = i_PawnType;
        }

    }
}
