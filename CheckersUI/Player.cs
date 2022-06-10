using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CheckersLogic;

namespace CheckersUI
{
    public class Player
    {
        private readonly ePawnTypes r_PawnType;
        public Player(string i_PlayerName, bool i_IsHuman, ePawnTypes i_PawnType)
        {
            PlayerName = i_PlayerName;
            IsHuman = i_IsHuman;
            Score = 0;
            r_PawnType = i_PawnType;
        }

        public string PlayerName { get; }

        public bool IsHuman { get; }
        
        public int Score { get; set; }

        public char PawnType => (char)r_PawnType;
    }
}
