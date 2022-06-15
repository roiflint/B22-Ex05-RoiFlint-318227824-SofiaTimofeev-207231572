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
        private bool m_IsGameOver = false;
        private GameButton[,] m_ButtonGrid;
        private GameLogic m_GameLogic;
        private AI m_gameAi;
        private int[] m_MoveFrom = new[] { -1, -1 };
        private int[] m_MoveTo = new[] {-1, -1};
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
            createPlayerOne();
            createPlayerTwo();
            createGameLogic();
            createButtonGrid();
            createBoard();
            ClientSize = new Size(k_WeightPadding + GameButton.GameButtonSize * r_GameSettings.BoardSize,
                k_HeightPadding + GameButton.GameButtonSize * r_GameSettings.BoardSize);
        }

        private void switchPlayerActiveButtons()
        {
            for(int i = 0; i < r_GameSettings.BoardSize; i++)
            {
                for(int j = 0; j < r_GameSettings.BoardSize; j++)
                {
                    if(m_ButtonGrid[i,j].Text == "X" || m_ButtonGrid[i, j].Text == "K")
                    {
                        if(m_GameLogic.IsFirstPlayerTurn())
                        {
                            if(m_GameLogic.IsAbleToMove(i,j,m_GameLogic.GetIsContinuesTurn(),out List<List<int>> o_ValidMoves))
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
                        if (m_GameLogic.IsFirstPlayerTurn())
                        {
                            m_ButtonGrid[i, j].Enabled = false;
                            m_ButtonGrid[i, j].BackColor = Color.White;
                        }
                        else
                        {
                            if (m_GameLogic.IsAbleToMove(i, j, m_GameLogic.GetIsContinuesTurn(), out List<List<int>> o_ValidMoves))
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
                            if(m_GameLogic.IsAbleToMove(i,j,m_GameLogic.GetIsContinuesTurn(), out List<List<int>> o_ValidMoves) && i_ButtonEnabled)
                            {
                                m_ButtonGrid[i, j].Enabled = true;
                                m_ButtonGrid[i, j].BackColor =
                                    m_ButtonGrid[i, j].BackColor =Color.LightGreen;
                            }
                            else
                            {
                                m_ButtonGrid[i, j].Enabled = false;
                                m_ButtonGrid[i, j].BackColor =
                                    m_ButtonGrid[i, j].BackColor = Color.White;
                            }
                        }
                    }
                    else
                    {
                        if ((m_ButtonGrid[i, j].Text == "O" || m_ButtonGrid[i, j].Text == "U"))
                        {
                            if (m_GameLogic.IsAbleToMove(i, j, m_GameLogic.GetIsContinuesTurn(), out List<List<int>> o_ValidMoves) && i_ButtonEnabled)
                            {
                                m_ButtonGrid[i, j].Enabled = true;
                                m_ButtonGrid[i, j].BackColor =
                                    m_ButtonGrid[i, j].BackColor = Color.LightGreen;
                            }
                            else
                            {
                                m_ButtonGrid[i, j].Enabled = false;
                                m_ButtonGrid[i, j].BackColor =
                                    m_ButtonGrid[i, j].BackColor = Color.White;
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
            labelPlayer1Score.Left = labelPlayer1Name.Right + 10;
        }

        private void createPlayerTwo()
        {
            m_PlayerTwo = new Player(r_GameSettings.Player2Name, r_GameSettings.IsComputer);
            labelPlayer2Name.Text = r_GameSettings.Player2Name + ": ";
            labelPlayer2Score.Left = labelPlayer2Name.Right + 10;
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

            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    GameButton button = new GameButton(i, j);
                    if ((i % 2 == 0 && j % 2 != 0) || (i % 2 != 0 && j % 2 == 0))
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

                if (i == (numberOfRowsPerPlayer + 1))
                {
                    piece = "X";
                }

                if (i == (numberOfRowsPerPlayer - 1))
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
                if(b.BackColor == Color.Blue && !m_GameLogic.GetIsContinuesTurn())
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
                b.BackColor =  Color.Blue;
                int[] buttonLocation = b.GetButtonLocation();
                if(m_MoveFrom[0] == -1)
                {
                    m_MoveFrom[0] = buttonLocation[0];
                    m_MoveFrom[1] = buttonLocation[1];
                    if (m_GameLogic.IsAbleToMove(m_MoveFrom[0], m_MoveFrom[1], m_GameLogic.GetIsContinuesTurn(), out m_ValidMoves))
                    {
                        flipAvailableMoves(m_ValidMoves);
                        changeButtonActivity(m_GameLogic.IsFirstPlayerTurn(), false);
                        m_ButtonGrid[m_MoveFrom[0], m_MoveFrom[1]].Enabled = true;
                        m_ButtonGrid[m_MoveFrom[0], m_MoveFrom[1]].BackColor = Color.Blue;
                    }
                }
                else
                {
                    m_MoveTo[0] = buttonLocation[0];
                    m_MoveTo[1] = buttonLocation[1];
                    if(m_GameLogic.CheckAndMove(m_MoveFrom[0], m_MoveFrom[1], m_MoveTo[0], m_MoveTo[1]))
                    {
                        refreshBoard();
                        printBoard();
                        if (m_GameLogic.GetIsContinuesTurn())
                        {
                            flipAvailableMoves(m_ValidMoves);
                            m_ButtonGrid[m_MoveFrom[0], m_MoveFrom[1]].Enabled = false;
                            m_ButtonGrid[m_MoveFrom[0], m_MoveFrom[1]].BackColor = Color.White;
                            m_MoveFrom[0] = m_MoveTo[0];
                            m_MoveFrom[1] = m_MoveTo[1];
                            m_MoveTo[0] = -1;
                            m_MoveTo[1] = -1;

                            m_ButtonGrid[m_MoveFrom[0], m_MoveFrom[1]].Enabled = true;
                            m_ButtonGrid[m_MoveFrom[0], m_MoveFrom[1]].BackColor = Color.Blue;
                            m_GameLogic.IsAbleToMove(m_MoveFrom[0], m_MoveFrom[1], m_GameLogic.GetIsContinuesTurn(), out m_ValidMoves);
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
            }
            else if(m_GameLogic.CheckPlayerTwoWin())
            {
                message = "Player Two Win! Play Another Game?";
                title = "Player Two Win";
            }
            else
            {
                return;
            }
            var selectedOption = MessageBox.Show(message, title, MessageBoxButtons.YesNoCancel);

            if(selectedOption == DialogResult.Yes)
            {
                //reset board
            }
            else
            {
                //exit
            }


        }

        private void refreshBoard()
        {
            ePawnTypes[,] gameBoard = m_GameLogic.GetBoard();
            for(int i = 0; i < m_GameLogic.GetBoardSize(); i++)
            {
                for(int j = 0; j < m_GameLogic.GetBoardSize(); j++)
                {
                    switch(gameBoard[i,j])
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

        private void FormCheckers_Resize(object i_Sender, EventArgs e)
        {
            int buttonAmount = r_GameSettings.BoardSize;
            /*Resize the tiles of the game (depends on the client size) */
            int buttonWidth = (ClientSize.Width - 5 * (buttonAmount - 1) - 10) / buttonAmount;
            int buttonHeight = (ClientSize.Height - 5 * (buttonAmount - 1) - 10 - 30) / buttonAmount;

            for (int i = 4; i < this.Controls.Count; i++)
            {
                if (Controls[i] is GameButton button)
                {
                    button.Width = buttonWidth;
                    button.Height = buttonHeight;
                    int currentWidth = 10 + button.Width * ((i - 4) % buttonAmount) + 5;
                    int currentHeight = 10 + button.Height * ((i - 4) / buttonAmount) + 5;
                    button.Location = new Point(currentWidth, currentHeight);
                }
            }

            /*Change the location of the players name plates*/
            labelPlayer1Name.Location = new Point(ClientSize.Width / 12, ClientSize.Height - 30);
            labelPlayer1Score.Location = new Point(ClientSize.Width / 12 + labelPlayer1Name.Width, ClientSize.Height - 30);
            labelPlayer2Name.Location = new Point((ClientSize.Width / 2) + ClientSize.Width / 14, ClientSize.Height - 30);
            labelPlayer2Score.Location = new Point((ClientSize.Width / 2) + ClientSize.Width / 14 + labelPlayer2Name.Width, ClientSize.Height - 30);
        }

        public void PlayRound()
        {
            string playerMove = string.Empty;
            bool isPlayerOne = m_GameLogic.IsFirstPlayerTurn();
            int[,] lastComputerMove;
            bool isExit = false;

            if (isPlayerOne || m_PlayerTwo.IsHuman)
            {
                //playerMove = getPlayerMove();
                if (playerMove[0].ToString().ToLower() != "q")
                {
                    while (!isMovePlayable(playerMove))
                    {
                        Console.WriteLine("The move is unplayable, please try again: ");
                        //playerMove = getPlayerMove();
                    }
                }
                else
                {
                    exitRound(isPlayerOne);
                    isExit = true;
                }
            }

            else
            {
                System.Threading.Thread.Sleep(3000);
                m_gameAi.MakeMove(out lastComputerMove);
                // playerMove = convertComputerMove(lastComputerMove);
            }

            if (!isExit)
            {
                checkIfGameEnd();
            }
        }

        private void checkIfGameEnd()
        {
            Player winningPlayer = null;

            if (m_GameLogic.CheckPlayerOneWin())
            {
                winningPlayer = m_PlayerOne;
            }

            else if (m_GameLogic.CheckPlayerTwoWin())
            {
                winningPlayer = m_PlayerTwo;
            }

            if (winningPlayer != null)
            {
                winningPlayer.Score += m_GameLogic.GetWinnerScore();
                printWinningMassage(winningPlayer);
            }

            else if (m_GameLogic.IsTie())
            {
                printTieMassage();
            }

        }

        private void exitRound(bool i_IsPlayerOne)
        {
            Player winningPlayer;

            if (i_IsPlayerOne)
            {
                winningPlayer = m_PlayerTwo;
                winningPlayer.Score += m_GameLogic.GetNumberOfPiecesLeftPlayerTwo();
            }

            else
            {
                winningPlayer = m_PlayerOne;
                winningPlayer.Score += m_GameLogic.GetNumberOfPiecesLeftPlayerOne();
            }

            printWinningMassage(winningPlayer);
        }

        private bool printWinningMassage(Player i_WinningPlayer)
        {
            StringBuilder winMessage = new StringBuilder();
            winMessage.Append($"{i_WinningPlayer.PlayerName} has won!");
            winMessage.Append(Environment.NewLine);
            winMessage.Append("Play another round?");
            DialogResult result = MessageBox.Show(winMessage.ToString(), "Win", MessageBoxButtons.YesNo);
            return result == DialogResult.Yes;
        }

        private bool printTieMassage()
        {
            StringBuilder tieMessage = new StringBuilder();
            tieMessage.Append("There was a tie!");
            tieMessage.Append(Environment.NewLine);
            tieMessage.Append("Play another round?");
            DialogResult result = MessageBox.Show(tieMessage.ToString(), "Tie", MessageBoxButtons.YesNo);
            return result == DialogResult.Yes;
        }
        private bool isMovePlayable(string i_PlayerMove)
        {
            int fromCol = i_PlayerMove[0] - 'A';
            int fromRow = i_PlayerMove[1] - 'a';
            int toCol = i_PlayerMove[3] - 'A';
            int toRow = i_PlayerMove[4] - 'a';

            return m_GameLogic.CheckAndMove(fromRow, fromCol, toRow, toCol);
        }

        private void printBoard()
        {
            int boardSize = m_GameLogic.GetBoardSize();
            char startingLetter = 'a';
            StringBuilder boardLine = new StringBuilder();
            ePawnTypes[,] gameBoard = m_GameLogic.GetBoard();

            printLetterOverhead(boardSize);
            for (int i = 0; i < boardSize; i++)
            {
                printSeparatorLine(boardSize);
                boardLine.Append((char)(startingLetter + i));
                boardLine.Append("| ");
                for (int j = 0; j < boardSize; j++)
                {
                    boardLine.Append((char)gameBoard[i, j]);
                    boardLine.Append(" | ");
                }

                Console.WriteLine(boardLine);
                boardLine.Clear();
            }

        }
        private void printLetterOverhead(int i_BoardSize)
        {
            const char k_StartingLetter = 'A';
            StringBuilder letterOverhead = new StringBuilder();

            letterOverhead.Append(' ', 3);
            for (int i = 0; i < i_BoardSize; i++)
            {
                letterOverhead.Append((char)(k_StartingLetter + i));
                letterOverhead.Append(' ', 3);
            }

            Console.WriteLine(letterOverhead);
        }

        private void printSeparatorLine(int i_BoardSize)
        {
            string separatorLine = new string('=', 4 * i_BoardSize + 2);
            Console.WriteLine(separatorLine);
        }
    }
}
