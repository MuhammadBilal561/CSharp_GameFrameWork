/*
 * SettingsForm.cs
 * Settings screen for Space Defender game
 * Handles volume controls and progress reset
 */

using FirstDesktopApp.Systems;
using GameFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FirstDesktopApp.UI
{
    public class SettingsForm : Form
    {
        // Settings options
        private readonly string[] settingsItems = { "Music Volume", "SFX Volume", "Reset Progress", "Back" };
        private int selectedIndex = 0;
        
        // For mouse hover detection
        private List<RectangleF> itemBounds = new List<RectangleF>();
        
        // Refresh timer
        private System.Windows.Forms.Timer renderTimer;

        public SettingsForm()
        {
            InitializeForm();
            SetupTimer();
        }

        private void InitializeForm()
        {
            Text = "Settings";
            ClientSize = new Size(1920, 1080);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            
            KeyDown += OnKeyDown;
            MouseClick += OnMouseClick;
            MouseMove += OnMouseMove;
        }

        private void SetupTimer()
        {
            renderTimer = new System.Windows.Forms.Timer { Interval = 16 };
            renderTimer.Tick += (s, e) => Invalidate();
            renderTimer.Start();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    MoveSelection(-1);
                    break;
                case Keys.Down:
                    MoveSelection(1);
                    break;
                case Keys.Left:
                    AdjustValue(-0.1f);
                    break;
                case Keys.Right:
                    AdjustValue(0.1f);
                    break;
                case Keys.Enter:
                    if (settingsItems[selectedIndex] == "Back")
                        Close();
                    break;
                case Keys.Escape:
                    Close();
                    break;
            }
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (settingsItems[selectedIndex] == "Back")
                Close();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < itemBounds.Count; i++)
            {
                if (itemBounds[i].Contains(e.X, e.Y))
                {
                    selectedIndex = i;
                    break;
                }
            }
        }

        private void MoveSelection(int direction)
        {
            selectedIndex += direction;
            
            if (selectedIndex < 0) selectedIndex = settingsItems.Length - 1;
            if (selectedIndex >= settingsItems.Length) selectedIndex = 0;
        }

        private void AdjustValue(float change)
        {
            switch (selectedIndex)
            {
                case 0: // Music Volume
                    GameDataManager.CurrentData.MusicVolume = Math.Clamp(
                        GameDataManager.CurrentData.MusicVolume + change, 0f, 1f);
                    SoundManager.SetMusicVolume(GameDataManager.CurrentData.MusicVolume);
                    GameDataManager.Save();
                    break;
                    
                case 1: // SFX Volume
                    GameDataManager.CurrentData.SfxVolume = Math.Clamp(
                        GameDataManager.CurrentData.SfxVolume + change, 0f, 1f);
                    GameDataManager.Save();
                    break;
                    
                case 2: // Reset Progress (only on right arrow)
                    if (change > 0)
                        GameDataManager.ResetProgress();
                    break;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            itemBounds.Clear();
            
            DrawBackground(g);
            DrawTitle(g);
            DrawSettingsPanel(g);
            DrawInstructions(g);
        }

        private void DrawBackground(Graphics g)
        {
            int w = ClientSize.Width;
            int h = ClientSize.Height;
            
            using (var brush = new LinearGradientBrush(
                new Point(0, 0), new Point(0, h),
                Color.FromArgb(255, 15, 15, 45), Color.FromArgb(255, 35, 35, 75)))
            {
                g.FillRectangle(brush, 0, 0, w, h);
            }
        }

        private void DrawTitle(Graphics g)
        {
            using (var titleFont = new Font("Impact", 60, FontStyle.Bold))
            {
                string title = "SETTINGS";
                var titleSize = g.MeasureString(title, titleFont);
                float titleX = (ClientSize.Width - titleSize.Width) / 2;
                g.DrawString(title, titleFont, Brushes.Cyan, titleX, 80);
            }
        }

        private void DrawSettingsPanel(Graphics g)
        {
            int w = ClientSize.Width;
            
            // Panel dimensions
            float panelWidth = 800;
            float panelHeight = 480;
            float panelX = (w - panelWidth) / 2;
            float panelY = 220;

            // Panel background
            g.FillRectangle(new SolidBrush(Color.FromArgb(180, 0, 0, 30)), panelX, panelY, panelWidth, panelHeight);
            g.DrawRectangle(new Pen(Color.Cyan, 3), panelX, panelY, panelWidth, panelHeight);

            using (var menuFont = new Font("Arial", 24, FontStyle.Bold))
            using (var smallFont = new Font("Arial", 14))
            {
                float startY = panelY + 50;
                float rowHeight = 100;

                for (int i = 0; i < settingsItems.Length; i++)
                {
                    bool isSelected = i == selectedIndex;
                    string item = settingsItems[i];
                    float rowY = startY + i * rowHeight;

                    // Store bounds for mouse detection
                    itemBounds.Add(new RectangleF(panelX, rowY, panelWidth, rowHeight - 10));

                    // Item label
                    Color textColor = isSelected ? Color.Yellow : Color.White;
                    string prefix = isSelected ? "►  " : "    ";
                    g.DrawString(prefix + item, menuFont, new SolidBrush(textColor), panelX + 40, rowY + 10);

                    // Volume bars for first two items
                    if (i < 2)
                    {
                        float volume = i == 0 ? GameDataManager.CurrentData.MusicVolume : GameDataManager.CurrentData.SfxVolume;

                        float barX = panelX + panelWidth - 320;
                        float barY = rowY + 12;
                        float barWidth = 200;
                        float barHeight = 28;

                        // Bar background and fill
                        g.FillRectangle(Brushes.DarkGray, barX, barY, barWidth, barHeight);
                        g.FillRectangle(Brushes.Cyan, barX, barY, barWidth * volume, barHeight);
                        g.DrawRectangle(Pens.White, barX, barY, barWidth, barHeight);

                        // Percentage text
                        string pctText = ((int)(volume * 100)) + "%";
                        g.DrawString(pctText, smallFont, Brushes.White, barX + barWidth + 15, barY + 5);

                        // Arrow hint when selected
                        if (isSelected)
                            g.DrawString("◄  ►", smallFont, Brushes.Yellow, barX + barWidth / 2 - 20, barY + barHeight + 5);
                    }
                    else if (i == 2 && isSelected)
                    {
                        // Reset progress hint
                        g.DrawString("Press RIGHT to confirm", smallFont, Brushes.OrangeRed, panelX + panelWidth - 280, rowY + 15);
                    }
                }
            }
        }

        private void DrawInstructions(Graphics g)
        {
            using (var font = new Font("Arial", 16))
            {
                string text = "UP/DOWN: Navigate  |  LEFT/RIGHT: Adjust  |  ENTER: Select  |  ESC: Back";
                var size = g.MeasureString(text, font);
                g.DrawString(text, font, Brushes.Gray, (ClientSize.Width - size.Width) / 2, ClientSize.Height - 70);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            renderTimer?.Stop();
            base.OnFormClosing(e);
        }
    }
}
