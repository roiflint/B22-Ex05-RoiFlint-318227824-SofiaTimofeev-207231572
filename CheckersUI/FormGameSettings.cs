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
    public partial class FormGameSettings : Form
    {
        public FormGameSettings()
        {
            InitializeComponent();
        }

        private void GameSettings_Load(object sender, EventArgs e)
        {

        }

        public string Player1Name => textBoxPlayer1.Text;

        public string Player2Name => textBoxPlayer2.Text;

        public bool IsComputer => checkBoxPlayer2.Checked;

        public int BoardSize
        {
            get
            {
                int boardSize = 6;
                if(radioButton6x6.Checked)
                    boardSize = 6;
                if(radioButton8x8.Checked)
                    boardSize = 8;
                if(radioButton10x10.Checked)
                    boardSize = 10;
                return boardSize;
            }
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
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        
    }
}
