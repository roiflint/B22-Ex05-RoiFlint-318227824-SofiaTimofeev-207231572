using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CheckersUI
{
    public partial class GameSettings : Form
    {
        public GameSettings()
        {
            InitializeComponent();
        }

        private void GameSettings_Load(object sender, EventArgs e)
        {

        }
        
        private void checkBoxPlayer2_CheckedChanged(object sender, EventArgs e)
        {
            if (textBoxPlayer2.ReadOnly == true)
            {
                textBoxPlayer2.ReadOnly = false;
                textBoxPlayer2.Text = string.Empty;
            }
            else
            {
                textBoxPlayer2.ReadOnly = true;
                textBoxPlayer2.Text = "[Computer]";
            }
        }

        private void buttonDone_Click(object sender, EventArgs e)
        {
            if(textBoxPlayer1.Text == String.Empty || textBoxPlayer2.Text == string.Empty)
            {
                MessageBox.Show("Player names cannot be empty.","Damka", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
            else if(textBoxPlayer1.Text.Contains(" ") || textBoxPlayer2.Text.Contains(" "))
            {
                MessageBox.Show("Player names cannot contain spaces.", "Damka", MessageBoxButtons.OK,
                    MessageBoxIcon.Exclamation);
            }
            else
            {
                GameBoard gameBoard = new GameBoard();
                this.Hide();
                this.DialogResult = DialogResult.OK;
                gameBoard.ShowDialog();
                this.Close();
            }
        }
        
    }
}
