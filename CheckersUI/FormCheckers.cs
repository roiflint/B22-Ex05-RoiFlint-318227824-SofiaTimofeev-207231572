using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
        private GameLogic m_gameLogic;
        private AI m_gameAi;

        internal class GameButton : Button
        {

            public static int GameButtonSize => 60;
            private const int k_HeightPadding = 30;
            private const int k_WeightPadding = 10;
            private const int k_LeftPadding = 5;
            public GameButton(int i_Row, int i_Col)
            {
                int currentHeight = k_HeightPadding + GameButtonSize * i_Row + k_LeftPadding;
                int currentWidth = k_WeightPadding + GameButtonSize * i_Col + k_LeftPadding;
                Location = new Point(currentWidth, currentHeight);
                Name = string.Format("{0}{1}", i_Row + "A", i_Col + "a");
                Size = new Size(GameButtonSize, GameButtonSize);
                Enabled = false;
            }
        }
        public FormCheckers()
        {
            r_GameSettings.ShowDialog();
            InitializeComponent();
            createPlayerOne();
            createPlayerTwo();
            createGameLogic();
            createBoard();
            ClientSize = new Size(k_WeightPadding + GameButton.GameButtonSize * r_GameSettings.BoardSize,
                k_HeightPadding + GameButton.GameButtonSize * r_GameSettings.BoardSize);
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
            m_gameLogic = new GameLogic(r_GameSettings.BoardSize);
            m_gameAi = new AI(ref m_gameLogic);
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
                        if(piece != " ")
                            button.Enabled = true;
                    }

                    else
                    {
                        button.BackColor = Color.Gray;
                    }
                    button.Click += GameButton_OnClick;
                    this.Controls.Add(button);
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
        
        private void GameButton_OnClick(object i_Sender, EventArgs i_e)
        {
            if(i_Sender is Button)
            {
                Button b = i_Sender as Button;
                b.BackColor = b.BackColor == Color.White ? Color.Blue : Color.White;
                //makes the right buttons (only those he can move to) enabled
                //make the move(?) and move the piece
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
            bool isPlayerOne = m_gameLogic.IsFirstPlayerTurn();
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

            if (m_gameLogic.CheckPlayerOneWin())
            {
                winningPlayer = m_PlayerOne;
            }

            else if (m_gameLogic.CheckPlayerTwoWin())
            {
                winningPlayer = m_PlayerTwo;
            }

            if (winningPlayer != null)
            {
                winningPlayer.Score += m_gameLogic.GetWinnerScore();
                printWinningMassage(winningPlayer);
            }

            else if (m_gameLogic.IsTie())
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
                winningPlayer.Score += m_gameLogic.GetNumberOfPiecesLeftPlayerTwo();
            }

            else
            {
                winningPlayer = m_PlayerOne;
                winningPlayer.Score += m_gameLogic.GetNumberOfPiecesLeftPlayerOne();
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

            return m_gameLogic.CheckAndMove(fromRow, fromCol, toRow, toCol);
        }
        
    }
}
