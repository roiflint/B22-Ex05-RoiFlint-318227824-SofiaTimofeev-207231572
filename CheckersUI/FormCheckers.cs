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
        private Player m_PlayerOne;
        private Player m_PlayerTwo;
        private bool m_IsGameOver = false;
        private GameLogic m_gameLogic;
        private AI m_gameAi;

        public FormCheckers()
        {
            r_GameSettings.ShowDialog();
            InitializeComponent();
            createPlayerOne();
            createPlayerTwo();
            createGameLogic();
            createBoard();
            ClientSize = new Size(30 + 80 * r_GameSettings.BoardSize,
                50 + 80 * r_GameSettings.BoardSize);
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private void createPlayerOne()
        {
            m_PlayerOne = new Player(r_GameSettings.Player1Name, true);
            labelPlayer1Name.Text = r_GameSettings.Player1Name + ": ";
        }

        private void createPlayerTwo()
        {
            m_PlayerTwo = new Player(r_GameSettings.Player2Name, r_GameSettings.IsComputer);
            labelPlayer2Name.Text = r_GameSettings.Player2Name + ": ";
        }

        private void createGameLogic()
        {
            m_gameLogic = new GameLogic(r_GameSettings.BoardSize);
            m_gameAi = new AI(ref m_gameLogic);
        }
        private void createBoard()
        {
            int boardSize = r_GameSettings.BoardSize;
            int numberOfStartingRows = boardSize - 2;
            int numberOfRowsPerPlayer = numberOfStartingRows / 2;
            string piece = "O";

            for (int i = 0; i < boardSize; i++)
            {
                for (int j = 0; j < boardSize; j++)
                {
                    Button button = new Button();
                    int currentHeight = 30 + 80 * i;
                    int currentWidth = 10 + 80 * j + 5;
                    button.Location = new Point(currentWidth, currentHeight);
                    button.Name = string.Format("{0}, {1}", i + 1, j + 1);
                    button.Size = new Size(80, 80);
                    button.Enabled = false;
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
            }
        }
        public bool IsGameOver { get; set; }

        public void PlayRound()
        {
            string playerMove = string.Empty;
            bool isPlayerOne = m_gameLogic.IsFirstPlayerTurn();
            int[,] lastComputerMove;
            bool isExit = false;

            if (isPlayerOne || m_PlayerTwo.IsHuman)
            {
                playerMove = getPlayerMove();
                if (playerMove[0].ToString().ToLower() != "q")
                {
                    while (!isMovePlayable(playerMove))
                    {
                        Console.WriteLine("The move is unplayable, please try again: ");
                        playerMove = getPlayerMove();
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
                printScore();
                checkForAnotherRound();
            }

            else if (m_gameLogic.IsTie())
            {
                printTieMassage();
                printScore();
                checkForAnotherRound();
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
            printScore();
            checkForAnotherRound();
        }

        private void printWinningMassage(Player i_WinningPlayer)
        {
        }

        private void printTieMassage()
        {
            Console.WriteLine("There is a tie!");
        }

        private void printScore()
        {
            Console.WriteLine("The score is: {0}:{1}  {2}:{3}", m_PlayerOne.PlayerName, m_PlayerOne.Score,
                m_PlayerTwo.PlayerName, m_PlayerTwo.Score);
        }

        private void checkForAnotherRound()
        {
            bool continueGame = false;

            Console.WriteLine("Play another round? Y/N");
            if (continueGame)
            {
                IsGameOver = false;
                m_gameLogic.NewGame();
                m_gameAi.InitAI();
                PlayRound();
            }

            else
            {
                IsGameOver = true;
            }

        }

        private bool isMovePlayable(string i_PlayerMove)
        {
            int fromCol = i_PlayerMove[0] - 'A';
            int fromRow = i_PlayerMove[1] - 'a';
            int toCol = i_PlayerMove[3] - 'A';
            int toRow = i_PlayerMove[4] - 'a';

            return m_gameLogic.CheckAndMove(fromRow, fromCol, toRow, toCol);
        }

        private string getPlayerMove()
        {
            string userInput = Console.ReadLine();

            while (!(isUserMoveLegal(userInput)))
            {
                Console.WriteLine("Invalid move, please enter COLrow>COLrow");
                userInput = Console.ReadLine();
            }

            return userInput;
        }

        private bool isUserMoveLegal(string i_UserInput)
        {
            bool isValid = false;

            if (i_UserInput.Length == 1 && i_UserInput[0].ToString().ToLower() == "q")
            {
                isValid = true;
            }

            else if (i_UserInput.Length == 5 && i_UserInput.Contains('>'))
            {
                for (int i = 0; i < i_UserInput.Length; i++)
                {
                    if (i == 0 || i == 3)
                    {
                        isValid = i_UserInput[i] >= 'A' && i_UserInput[i] <= (char)('A' + m_gameLogic.GetBoardSize());
                    }

                    else
                    {
                        isValid = i_UserInput[i] >= 'a' && i_UserInput[i] <= (char)('a' + m_gameLogic.GetBoardSize());
                    }

                }

            }

            return isValid;
        }
    }
}
