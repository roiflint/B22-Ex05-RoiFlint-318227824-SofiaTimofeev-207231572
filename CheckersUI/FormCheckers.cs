using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using CheckersLogic;

namespace CheckersUI
{
    public partial class FormCheckers : Form
    {
        private readonly FormGameSettings r_GameSettings = new FormGameSettings();
        private const int k_HeightPadding = 50;
        private const int k_WeightPadding = 30;
        private Player m_PlayerOne;
        private Player m_PlayerTwo;
        private GameButton[,] m_ButtonGrid;
        private GameLogic m_GameLogic;
        private AI m_gameAi;
        private int[] m_MoveFrom = new[] { -1, -1 };
        private int[] m_MoveTo = new[] { -1, -1 };
        private List<List<int>> m_ValidMoves;

        internal class GameButton : Button
        {

            public static int GameButtonSize => 60;
            private const int k_HeightPadding = 30;
            private const int k_WeightPadding = 10;
            private const int k_LeftPadding = 5;
            private int[] m_ButtonLocation = new int[2];

            public GameButton(int i_Row, int i_Col)
            {
                int currentHeight = k_HeightPadding + GameButtonSize * i_Row + k_LeftPadding;
                int currentWidth = k_WeightPadding + GameButtonSize * i_Col + k_LeftPadding;
                Location = new Point(currentWidth, currentHeight);
                Name = string.Format("{0}{1}", i_Row + "A", i_Col + "a");
                Size = new Size(GameButtonSize, GameButtonSize);
                Enabled = false;
                m_ButtonLocation[0] = i_Row;
                m_ButtonLocation[1] = i_Col;
            }

            public int[] GetButtonLocation()
            {
                return m_ButtonLocation;
            }
        }

        public FormCheckers()
        {
            r_GameSettings.ShowDialog();
            InitializeComponent();
            createGameLogic();
            createButtonGrid();
            createBoard();
            createPlayerOne();
            createPlayerTwo();
            ClientSize = new Size(
                k_WeightPadding + GameButton.GameButtonSize * r_GameSettings.BoardSize,
                k_HeightPadding + GameButton.GameButtonSize * r_GameSettings.BoardSize);
        }

        private void switchPlayerActiveButtons()
        {
            for(int i = 0; i < r_GameSettings.BoardSize; i++)
            {
                for(int j = 0; j < r_GameSettings.BoardSize; j++)
                {
                    if(m_ButtonGrid[i, j].Text == "X" || m_ButtonGrid[i, j].Text == "K")
                    {
                        if(m_GameLogic.IsFirstPlayerTurn())
                        {
                            if(m_GameLogic.IsAbleToMove(
                                   i,
                                   j,
                                   m_GameLogic.GetIsContinuesTurn(),
                                   out List<List<int>> o_ValidMoves))
                            {
                                m_ButtonGrid[i, j].Enabled = true;
                                m_ButtonGrid[i, j].BackColor = Color.LightGreen;
                            }
                        }
                        else
                        {
                            m_ButtonGrid[i, j].Enabled = false;
                            m_ButtonGrid[i, j].BackColor = Color.White;
                        }
                    }
                    else if(m_ButtonGrid[i, j].Text == "O" || m_ButtonGrid[i, j].Text == "U")
                    {
                        if(m_GameLogic.IsFirstPlayerTurn())
                        {
                            m_ButtonGrid[i, j].Enabled = false;
                            m_ButtonGrid[i, j].BackColor = Color.White;
                        }
                        else
                        {
                            if(m_GameLogic.IsAbleToMove(
                                   i,
                                   j,
                                   m_GameLogic.GetIsContinuesTurn(),
                                   out List<List<int>> o_ValidMoves))
                            {
                                m_ButtonGrid[i, j].Enabled = true;
                                m_ButtonGrid[i, j].BackColor = Color.LightGreen;
                            }
                        }
                    }
                }
            }
        }

        private void changeButtonActivity(bool i_IsPlayerOne, bool i_ButtonEnabled)
        {
            for(int i = 0; i < r_GameSettings.BoardSize; i++)
            {
                for(int j = 0; j < r_GameSettings.BoardSize; j++)
                {
                    if(i_IsPlayerOne)
                    {
                        if((m_ButtonGrid[i, j].Text == "X" || m_ButtonGrid[i, j].Text == "K"))
                        {
                            if(m_GameLogic.IsAbleToMove(
                                   i,
                                   j,
                                   m_GameLogic.GetIsContinuesTurn(),
                                   out List<List<int>> o_ValidMoves) && i_ButtonEnabled)
                            {
                                m_ButtonGrid[i, j].Enabled = true;
                                m_ButtonGrid[i, j].BackColor = m_ButtonGrid[i, j].BackColor = Color.LightGreen;
                            }
                            else
                            {
                                m_ButtonGrid[i, j].Enabled = false;
                                m_ButtonGrid[i, j].BackColor = m_ButtonGrid[i, j].BackColor = Color.White;
                            }
                        }
                    }
                    else
                    {
                        if((m_ButtonGrid[i, j].Text == "O" || m_ButtonGrid[i, j].Text == "U"))
                        {
                            if(m_GameLogic.IsAbleToMove(
                                   i,
                                   j,
                                   m_GameLogic.GetIsContinuesTurn(),
                                   out List<List<int>> o_ValidMoves) && i_ButtonEnabled)
                            {
                                m_ButtonGrid[i, j].Enabled = true;
                                m_ButtonGrid[i, j].BackColor = m_ButtonGrid[i, j].BackColor = Color.LightGreen;
                            }
                            else
                            {
                                m_ButtonGrid[i, j].Enabled = false;
                                m_ButtonGrid[i, j].BackColor = m_ButtonGrid[i, j].BackColor = Color.White;
                            }
                        }
                    }
                }
            }
        }

        private void flipAvailableMoves(List<List<int>> i_ValidMoves)
        {
            foreach(List<int> move in i_ValidMoves)
            {
                m_ButtonGrid[move[0], move[1]].BackColor =
                    m_ButtonGrid[move[0], move[1]].BackColor == Color.White ? Color.LightGreen : Color.White;

                m_ButtonGrid[move[0], move[1]].Enabled = !m_ButtonGrid[move[0], move[1]].Enabled;
            }
        }

        private void createButtonGrid()
        {
            m_ButtonGrid = new GameButton[r_GameSettings.BoardSize, r_GameSettings.BoardSize];
        }

        private void createPlayerOne()
        {
            m_PlayerOne = new Player(r_GameSettings.Player1Name, true);
            labelPlayer1Name.Text = r_GameSettings.Player1Name + ": ";
            labelPlayer1Name.Left = m_ButtonGrid[0, 0].Left;
            labelPlayer1Score.Left = labelPlayer1Name.Right + 5;
        }

        private void createPlayerTwo()
        {
            m_PlayerTwo = new Player(r_GameSettings.Player2Name, r_GameSettings.IsComputer);
            labelPlayer2Name.Text = r_GameSettings.Player2Name + ": ";
            labelPlayer2Name.Left = labelPlayer1Score.Right + 10;
            labelPlayer2Score.Left = labelPlayer2Name.Right + 5;
        }

        private void createGameLogic()
        {
            m_GameLogic = new GameLogic(r_GameSettings.BoardSize);
            m_gameAi = new AI(ref m_GameLogic);
        }

        private void createBoard()
        {
            int boardSize = r_GameSettings.BoardSize;
            int numberOfRowsPerPlayer = (boardSize - 2) / 2;
            string piece = "O";

            for(int i = 0; i < boardSize; i++)
            {
                for(int j = 0; j < boardSize; j++)
                {
                    GameButton button = new GameButton(i, j);
                    if((i % 2 == 0 && j % 2 != 0) || (i % 2 != 0 && j % 2 == 0))
                    {
                        button.BackColor = Color.White;
                        button.Text = piece;
                        if(piece != " " && piece != "O" && i == ((m_GameLogic.GetBoardSize() / 2) + 1))
                        {
                            button.Enabled = true;
                            button.BackColor = Color.LightGreen;
                        }
                    }

                    else
                    {
                        button.BackColor = Color.Gray;
                    }

                    button.Click += GameButton_OnClick;
                    this.Controls.Add(button);
                    m_ButtonGrid[i, j] = button;
                }

                if(i == (numberOfRowsPerPlayer + 1))
                {
                    piece = "X";
                }

                if(i == (numberOfRowsPerPlayer - 1))
                {
                    piece = " ";
                }

            }
        }

        private void fixGreyBackGround()
        {
            for(int i = 0; i < r_GameSettings.BoardSize; i++)
            {
                for(int j = 0; j < r_GameSettings.BoardSize; j++)
                {
                    if((i % 2 == j % 2))
                    {
                        m_ButtonGrid[i, j].BackColor = Color.Gray;
                    }
                }
            }
        }

        private void GameButton_OnClick(object i_Sender, EventArgs i_e)
        {
            if(i_Sender is Button)
            {
                GameButton b = i_Sender as GameButton;
                if(b.BackColor == Color.LightBlue && !m_GameLogic.GetIsContinuesTurn())
                {
                    m_MoveFrom[0] = -1;
                    m_MoveFrom[1] = -1;
                    m_MoveTo[0] = -1;
                    m_MoveTo[1] = -1;
                    changeButtonActivity(m_GameLogic.IsFirstPlayerTurn(), true);
                    flipAvailableMoves(m_ValidMoves);
                    b.Enabled = true;
                    b.BackColor = Color.LightGreen;
                    return;
                }

                b.BackColor = Color.LightBlue;
                int[] buttonLocation = b.GetButtonLocation();
                if(m_MoveFrom[0] == -1)
                {
                    m_MoveFrom[0] = buttonLocation[0];
                    m_MoveFrom[1] = buttonLocation[1];
                    if(m_GameLogic.IsAbleToMove(
                           m_MoveFrom[0],
                           m_MoveFrom[1],
                           m_GameLogic.GetIsContinuesTurn(),
                           out m_ValidMoves))
                    {
                        flipAvailableMoves(m_ValidMoves);
                        changeButtonActivity(m_GameLogic.IsFirstPlayerTurn(), false);
                        m_ButtonGrid[m_MoveFrom[0], m_MoveFrom[1]].Enabled = true;
                        m_ButtonGrid[m_MoveFrom[0], m_MoveFrom[1]].BackColor = Color.LightBlue;
                    }
                }
                else
                {
                    m_MoveTo[0] = buttonLocation[0];
                    m_MoveTo[1] = buttonLocation[1];
                    if(m_GameLogic.CheckAndMove(m_MoveFrom[0], m_MoveFrom[1], m_MoveTo[0], m_MoveTo[1]))
                    {
                        refreshBoard();
                        if(m_GameLogic.GetIsContinuesTurn())
                        {
                            flipAvailableMoves(m_ValidMoves);
                            m_ButtonGrid[m_MoveFrom[0], m_MoveFrom[1]].Enabled = false;
                            m_ButtonGrid[m_MoveFrom[0], m_MoveFrom[1]].BackColor = Color.White;
                            m_MoveFrom[0] = m_MoveTo[0];
                            m_MoveFrom[1] = m_MoveTo[1];
                            m_MoveTo[0] = -1;
                            m_MoveTo[1] = -1;

                            m_ButtonGrid[m_MoveFrom[0], m_MoveFrom[1]].Enabled = true;
                            m_ButtonGrid[m_MoveFrom[0], m_MoveFrom[1]].BackColor = Color.LightBlue;
                            m_GameLogic.IsAbleToMove(
                                m_MoveFrom[0],
                                m_MoveFrom[1],
                                m_GameLogic.GetIsContinuesTurn(),
                                out m_ValidMoves);
                            flipAvailableMoves(m_ValidMoves);
                        }
                        else
                        {
                            flipAvailableMoves(m_ValidMoves);
                            switchPlayerActiveButtons();
                            m_ButtonGrid[m_MoveFrom[0], m_MoveFrom[1]].BackColor = Color.White;
                            m_ButtonGrid[m_MoveFrom[0], m_MoveFrom[1]].Enabled = false;
                            m_ButtonGrid[m_MoveTo[0], m_MoveFrom[1]].BackColor = Color.White;
                            m_ButtonGrid[m_MoveTo[0], m_MoveFrom[1]].Enabled = false;
                            m_MoveFrom[0] = -1;
                            m_MoveFrom[1] = -1;
                            m_MoveTo[0] = -1;
                            m_MoveTo[1] = -1;
                        }

                    }
                }
            }
            fixGreyBackGround();
            checkEndGame();
        }

        private void checkEndGame()
        {
            string message = "";
            string title = "";
            if(m_GameLogic.IsTie())
            {
                message = "Tie! Play Another Game?";
                title = "Tie";
            }
            else if(m_GameLogic.CheckPlayerOneWin())
            {
                message = "Player One Win! Play Another Game?";
                title = "Player One Win";
                m_PlayerOne.Score += m_GameLogic.GetWinnerScore();
            }
            else if(m_GameLogic.CheckPlayerTwoWin())
            {
                message = "Player Two Win! Play Another Game?";
                title = "Player Two Win";
                m_PlayerTwo.Score += m_GameLogic.GetWinnerScore();
            }
            else
            {
                return;
            }

            var selectedOption = MessageBox.Show(message, title, MessageBoxButtons.YesNoCancel);

            if(selectedOption == DialogResult.Yes)
            {
                m_GameLogic.NewGame();
                refreshBoard();
                labelPlayer1Score.Text = m_PlayerOne.Score.ToString();
                labelPlayer2Score.Text = m_PlayerTwo.Score.ToString();
            }
            else
            {
                this.Close();
            }
        }

        private void refreshBoard()
        {
            ePawnTypes[,] gameBoard = m_GameLogic.GetBoard();
            for(int i = 0; i < m_GameLogic.GetBoardSize(); i++)
            {
                for(int j = 0; j < m_GameLogic.GetBoardSize(); j++)
                {
                    switch(gameBoard[i, j])
                    {
                        case ePawnTypes.Empty:
                            m_ButtonGrid[i, j].Text = " ";
                            break;
                        case ePawnTypes.PlayerOne:
                            m_ButtonGrid[i, j].Text = "X";
                            break;
                        case ePawnTypes.PlayerOneKing:
                            m_ButtonGrid[i, j].Text = "K";
                            break;
                        case ePawnTypes.PlayerTwo:
                            m_ButtonGrid[i, j].Text = "O";
                            break;
                        case ePawnTypes.PlayerTwoKing:
                            m_ButtonGrid[i, j].Text = "U";
                            break;
                    }
                }
            }
        }

        private void FormCheckers_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Are you sure you want to exit?","Exit", MessageBoxButtons.OKCancel);
            e.Cancel = dialogResult != DialogResult.OK;
        }
    }
    
}
    
