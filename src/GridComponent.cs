using System;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace TicTacToe
{
    public static class GridComponent
    {
        private static Button[,] buttons = new Button[3, 3];
        private static PlayerSymbol currentPlayer = PlayerSymbol.X;
        private static Timer flashTimer;
        private static bool flashState;
        private static (int, int)[] winningLine = new (int, int)[0];
        private static Label winLabel;
        private static Button playAgainButton;
        private static Button quitButton;
        private static Panel smallGridPanel;
        private static Button[,] smallGridButtons = new Button[3, 3];
        private static TableLayoutPanel mainGridPanel;
        private static FlowLayoutPanel buttonPanel; // Declare buttonPanel as a class-level variable

        public static void Create(WindowView windowView)
        {
            mainGridPanel = new TableLayoutPanel
            {
                RowCount = 3,
                ColumnCount = 3,
                Dock = DockStyle.Fill,
                Padding = new Padding(10)
            };

            for (int i = 0; i < 3; i++)
            {
                mainGridPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
                mainGridPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            }

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    Button button = new Button
                    {
                        Dock = DockStyle.Fill,
                        Tag = new { X = col, Y = row },
                        ForeColor = Color.Black,
                        Font = new Font("Arial", 24, FontStyle.Bold),
                        TextAlign = ContentAlignment.MiddleCenter,
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.White,
                        FlatAppearance = { BorderSize = 0 }
                    };
                    button.FlatAppearance.BorderColor = Color.FromArgb(255, 255, 255, 255);
                    button.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
                    button.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 200, 200);
                    button.Click += Button_Click;
                    buttons[row, col] = button;
                    mainGridPanel.Controls.Add(button, col, row);
                }
            }

            windowView.Controls.Add(mainGridPanel);

            flashTimer = new Timer();
            flashTimer.Interval = 500;
            flashTimer.Tick += FlashTimer_Tick;

            winLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 36, FontStyle.Bold),
                ForeColor = Color.Red,
                BackColor = Color.FromArgb(128, 255, 255, 255), // Semi-transparent white
                Visible = false,
                Height = 100
            };
            windowView.Controls.Add(winLabel);
            windowView.Controls.SetChildIndex(winLabel, 0); // Bring to front

            buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                FlowDirection = FlowDirection.LeftToRight,
                Padding = new Padding(10),
                AutoSize = true
            };

            playAgainButton = new Button
            {
                Text = "Play Again",
                Font = new Font("Arial", 18, FontStyle.Bold),
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                FlatAppearance = { BorderSize = 0 },
                Visible = false
            };
            playAgainButton.FlatAppearance.BorderColor = Color.FromArgb(255, 255, 255, 255);
            playAgainButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
            playAgainButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 200, 200);
            playAgainButton.Click += PlayAgainButton_Click;
            buttonPanel.Controls.Add(playAgainButton);

            quitButton = new Button
            {
                Text = "Quit",
                Font = new Font("Arial", 18, FontStyle.Bold),
                AutoSize = true,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                FlatAppearance = { BorderSize = 0 },
                Visible = false
            };
            quitButton.FlatAppearance.BorderColor = Color.FromArgb(255, 255, 255, 255);
            quitButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(230, 230, 230);
            quitButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 200, 200);
            quitButton.Click += QuitButton_Click;
            buttonPanel.Controls.Add(quitButton);

            windowView.Controls.Add(buttonPanel);

            smallGridPanel = new Panel
            {
                Size = new Size(150, 150),
                BackColor = Color.Transparent,
                Visible = false
            };

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    Button button = new Button
                    {
                        Size = new Size(50, 50),
                        Location = new Point(col * 50, row * 50),
                        Enabled = false,
                        FlatStyle = FlatStyle.Flat,
                        BackColor = Color.White,
                        FlatAppearance = { BorderSize = 0 }
                    };
                    button.FlatAppearance.BorderColor = Color.FromArgb(255, 255, 255, 255);
                    smallGridButtons[row, col] = button;
                    smallGridPanel.Controls.Add(button);
                }
            }

            windowView.Controls.Add(smallGridPanel);
            windowView.Controls.SetChildIndex(smallGridPanel, 0); // Bring to front
        }

        private static void Button_Click(object? sender, EventArgs e)
        {
            Button? button = sender as Button;
            if (button?.Tag != null)
            {
                var position = (dynamic)button.Tag;
                PlaceMove(currentPlayer, position.X, position.Y);
                if (CheckWin(currentPlayer, out winningLine))
                {
                    flashTimer?.Start();
                    ShowWinMessage(currentPlayer);
                }
                else
                {
                    TogglePlayer();
                }
            }
        }

        public static void PlaceMove(PlayerSymbol playerSymbol, int x, int y)
        {
            Button button = buttons[y, x];
            button.Text = playerSymbol.ToString();
            button.Enabled = false;
        }

        private static void TogglePlayer()
        {
            currentPlayer = currentPlayer == PlayerSymbol.X ? PlayerSymbol.O : PlayerSymbol.X;
        }

        private static bool CheckWin(PlayerSymbol playerSymbol, out (int, int)[] winningLine)
        {
            winningLine = new (int, int)[3];

            for (int i = 0; i < 3; i++)
            {
                if (buttons[i, 0].Text == playerSymbol.ToString() && buttons[i, 1].Text == playerSymbol.ToString() && buttons[i, 2].Text == playerSymbol.ToString())
                {
                    winningLine = new[] { (i, 0), (i, 1), (i, 2) };
                    return true;
                }
                if (buttons[0, i].Text == playerSymbol.ToString() && buttons[1, i].Text == playerSymbol.ToString() && buttons[2, i].Text == playerSymbol.ToString())
                {
                    winningLine = new[] { (0, i), (1, i), (2, i) };
                    return true;
                }
            }

            if (buttons[0, 0].Text == playerSymbol.ToString() && buttons[1, 1].Text == playerSymbol.ToString() && buttons[2, 2].Text == playerSymbol.ToString())
            {
                winningLine = new[] { (0, 0), (1, 1), (2, 2) };
                return true;
            }
            if (buttons[0, 2].Text == playerSymbol.ToString() && buttons[1, 1].Text == playerSymbol.ToString() && buttons[2, 0].Text == playerSymbol.ToString())
            {
                winningLine = new[] { (0, 2), (1, 1), (2, 0) };
                return true;
            }

            return false;
        }

        private static void FlashTimer_Tick(object? sender, EventArgs e)
        {
            flashState = !flashState;
            foreach (var (row, col) in winningLine)
            {
                buttons[row, col].BackColor = flashState ? Color.Black : Color.White;
            }
        }

        private static void ShowWinMessage(PlayerSymbol winner)
        {
            flashTimer?.Stop();
            winLabel.Text = $"{winner} wins!";
            winLabel.Visible = true;
            playAgainButton.Visible = true;
            quitButton.Visible = true;

            UpdateSmallGrid(winner);

            // Hide the main grid panel
            mainGridPanel.Visible = false;

            // Position the small grid panel near the win label
            smallGridPanel.Location = new Point(winLabel.Location.X + winLabel.Width / 2 - smallGridPanel.Width / 2, winLabel.Location.Y + winLabel.Height + 10);
            smallGridPanel.Visible = true;

            // Position the button panel below the small grid panel
            buttonPanel.Location = new Point(smallGridPanel.Location.X, smallGridPanel.Location.Y + smallGridPanel.Height + 10);
            buttonPanel.Visible = true;
        }

        private static void UpdateSmallGrid(PlayerSymbol winner)
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    smallGridButtons[row, col].Text = buttons[row, col].Text;
                    smallGridButtons[row, col].BackColor = buttons[row, col].BackColor;
                }
            }

            foreach (var (row, col) in winningLine)
            {
                smallGridButtons[row, col].BackColor = Color.Yellow; // Highlight winning line
            }
        }

        private static void PlayAgainButton_Click(object? sender, EventArgs e)
        {
            ResetBoard();
        }

        private static void QuitButton_Click(object? sender, EventArgs e)
        {
            Application.Exit();
        }

        private static void ResetBoard()
        {
            winLabel.Visible = false;
            playAgainButton.Visible = false;
            quitButton.Visible = false;
            smallGridPanel.Visible = false; // Hide small grid
            mainGridPanel.Visible = true; // Show main grid
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    buttons[row, col].Text = string.Empty;
                    buttons[row, col].Enabled = true;
                    buttons[row, col].BackColor = Color.White;
                }
            }
            currentPlayer = PlayerSymbol.X;
        }
    }
}