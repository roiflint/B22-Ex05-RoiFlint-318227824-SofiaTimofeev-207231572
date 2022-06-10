using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.Linq;
using System.Text;
using CheckersLogic;
using System.Windows.Forms;

namespace CheckersUI
{
    public class GameUI
    {
        private Player m_PlayerOne;
        private Player m_PlayerTwo;
        private bool m_IsGameOver;
        private bool m_isFirstRound;
        private GameLogic m_gameLogic;
        private AI m_gameAi;

        public GameUI()
        {
            Application.Run(new GameSettings());
            createFirstPlayer();
            createSecondPlayer();
            m_gameLogic = new GameLogic(getBoardSize());
            m_gameAi = new AI(ref m_gameLogic);
            m_isFirstRound = true;
            //create board
        }

        public bool IsGameOver { get; set; }

        private void printWelcomeMessage()
        {
            Console.WriteLine("Welcome to checkers!");
        }

        public void PrintGoodByeMassage()
        {
            Console.WriteLine("Thank you for playing!");
        }

        private void createFirstPlayer()
        {
            Console.WriteLine("Please enter player name (20 letters): ");
            string playerName = getPlayerName();
            m_PlayerOne = new Player(playerName, true, ePawnTypes.PlayerOne);
        }

        private void createSecondPlayer()
        {
            string playerName = "COMP";
            bool isHuman = false;

            Console.WriteLine("Is the second player human? Y/N");
            isHuman = getYesOrNoInput();
            if(isHuman)
            {
                Console.WriteLine("Please enter player name (20 letters): ");
                playerName = getPlayerName();
            }

            m_PlayerTwo = new Player(playerName, isHuman, ePawnTypes.PlayerTwo);
        }

        private bool getYesOrNoInput()
        {
            string userInput = Console.ReadLine();
            if (userInput.ToLower() != "y" && userInput.ToLower() != "n")
            {
                Console.WriteLine("Invalid input, please enter Y or N");
                userInput = Console.ReadLine();
            }

            return userInput.ToLower() == "y";

        }

        private int getBoardSize()
        {
            int boardSize;
            string userInput = string.Empty;
            bool validInput = false;

            Console.WriteLine("Please enter a board size out of the following: 6-8-10");
            userInput = Console.ReadLine();
            validInput = int.TryParse(userInput, out boardSize);
            while(!validInput || (boardSize != 6 && boardSize != 8 && boardSize != 10))
            {
                Console.WriteLine("Invalid board size, please try again: ");
                userInput = Console.ReadLine();
                validInput = int.TryParse(userInput, out boardSize);
            }

            return boardSize;
        }

        private string getPlayerName()
        {
            bool validInput = false;
            string userInput = string.Empty;

            while (!validInput)
            {
                userInput = Console.ReadLine();
                if (userInput.Length > 20)
                {
                    Console.WriteLine("Name is too long, please try again: ");
                }

                else if (userInput.Contains(" "))
                {
                    Console.WriteLine("Name cannot contain spaces, please try again: ");
                }
                else
                {
                    validInput = true;
                }
            }

            return userInput;
        }

        public void PlayRound()
        {
            string playerMove = string.Empty;
            bool isPlayerOne = m_gameLogic.IsFirstPlayerTurn();
            int[,] lastComputerMove;
            bool isExit = false;
           
            if(m_isFirstRound)
            {
                printFirstMoveMassage();
            }

            if(isPlayerOne || m_PlayerTwo.IsHuman)
            {
                playerMove = getPlayerMove();
                if(playerMove[0].ToString().ToLower() != "q")
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
                playerMove = convertComputerMove(lastComputerMove);
            }

            if(!isExit)
            {
                printLastMadeMove(playerMove, isPlayerOne);
                checkIfGameEnd();
            }
        }

        private string convertComputerMove(int[,] i_LastCompMove)
        {
            StringBuilder computerMove = new StringBuilder();

            computerMove.Append((char)('A' + i_LastCompMove[0, 1]));
            computerMove.Append((char)('a' + i_LastCompMove[0, 0]));
            computerMove.Append('>');
            computerMove.Append((char)('A' + i_LastCompMove[1, 1]));
            computerMove.Append((char)('a' + i_LastCompMove[1, 0]));

            return computerMove.ToString();
        }

        private void checkIfGameEnd()
        {
            Player winningPlayer = null;

            if(m_gameLogic.CheckPlayerOneWin())
            {
                winningPlayer = m_PlayerOne;
            }

            else if(m_gameLogic.CheckPlayerTwoWin())
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

            if(i_IsPlayerOne)
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
            Console.WriteLine("{0} has won ({1})!", i_WinningPlayer.PlayerName, i_WinningPlayer.PawnType);
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
            continueGame = getYesOrNoInput();
            if(continueGame)
            {
                IsGameOver = false;
                m_gameLogic.NewGame();
                m_gameAi.InitAI();
                m_isFirstRound = true;
                PlayRound();
            }

            else
            {
                IsGameOver = true;
            }

        }

        private void printFirstMoveMassage()
        {
            printBoard();
            printCurrentPlayerMoveMassage();
            m_isFirstRound = false;
        }

        private void printLastMadeMove(string i_LastMove, bool i_IsPlayerOne)
        {
            printBoard();
            Player lastTurnPlayer = i_IsPlayerOne ? m_PlayerOne : m_PlayerTwo;
            Console.WriteLine("{0}'s turn was ({1}): {2}", lastTurnPlayer.PlayerName, lastTurnPlayer.PawnType, i_LastMove);
            printCurrentPlayerMoveMassage();
        }

        private void printCurrentPlayerMoveMassage()
        {
            Player currentPlayer = m_gameLogic.IsFirstPlayerTurn() ? m_PlayerOne : m_PlayerTwo;
            Console.WriteLine("{0}'s turn ({1}): ", currentPlayer.PlayerName, currentPlayer.PawnType);
        }

        private bool isMovePlayable(string i_PlayerMove)
        {
            int fromCol = i_PlayerMove[0] - 'A';
            int fromRow = i_PlayerMove[1] - 'a';
            int toCol = i_PlayerMove[3] - 'A';
            int toRow = i_PlayerMove[4] - 'a';

            return  m_gameLogic.CheckAndMove(fromRow, fromCol, toRow, toCol);
        }

        private string getPlayerMove()
        {
            string userInput = Console.ReadLine();

            while(!(isUserMoveLegal(userInput)))
            {
                Console.WriteLine("Invalid move, please enter COLrow>COLrow");
                userInput = Console.ReadLine();
            }

            return userInput;
        }

        private bool isUserMoveLegal(string i_UserInput)
        {
            bool isValid = false;

            if(i_UserInput.Length == 1 && i_UserInput[0].ToString().ToLower() == "q")
            {
                isValid = true;
            }

            else if(i_UserInput.Length == 5 && i_UserInput.Contains('>'))
            {
                for(int i = 0; i < i_UserInput.Length; i++)
                {
                    if(i == 0 || i == 3)
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

        private void printBoard()
        {
            int boardSize = m_gameLogic.GetBoardSize();
            char startingLetter = 'a';
            StringBuilder boardLine = new StringBuilder();
            ePawnTypes[,] gameBoard = m_gameLogic.GetBoard();

            printLetterOverhead(boardSize);
            for(int i = 0; i < boardSize; i++)
            {
                printSeparatorLine(boardSize);
                boardLine.Append((char)(startingLetter + i));
                boardLine.Append("| ");
                for(int j = 0; j < boardSize; j++)
                {
                    boardLine.Append((char)gameBoard[i, j]);
                    boardLine.Append(" | ");
                }
                
                Console.WriteLine(boardLine);
                boardLine.Clear();
            }

        }

        private void printSeparatorLine(int i_BoardSize)
        {
            string separatorLine = new string('=', 4 * i_BoardSize + 2);
            Console.WriteLine(separatorLine);
        }

        private void printLetterOverhead(int i_BoardSize)
        {
            const char k_StartingLetter = 'A';
            StringBuilder letterOverhead = new StringBuilder();

            letterOverhead.Append(' ', 3);
            for(int i = 0; i < i_BoardSize; i++)
            {
                letterOverhead.Append((char)(k_StartingLetter + i));
                letterOverhead.Append(' ', 3);
            }

            Console.WriteLine(letterOverhead);
        }
       
    }
}
