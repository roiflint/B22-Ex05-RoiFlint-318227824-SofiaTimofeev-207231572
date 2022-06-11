using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CheckersLogic;

namespace CheckersUI
{
    public class Player
    {
        public Player(string i_PlayerName, bool i_IsHuman)
        {
            PlayerName = i_PlayerName;
            IsHuman = i_IsHuman;
            Score = 0;
        }

        public string PlayerName { get; }

        public bool IsHuman { get; }
        
        public int Score { get; set; }
        
    }
}
