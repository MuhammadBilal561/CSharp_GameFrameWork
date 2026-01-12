/*
 * MainMenuForm.cs
 * Main menu screen for Space Defender game
 * Handles game start, continue, settings navigation and exit
 */

using FirstDesktopApp.Levels;
using FirstDesktopApp.Systems;
using GameFramework;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace FirstDesktopApp.UI
{
    public class MainMenuForm : Form
    {
        // Menu options
        private readonly string[] menuItems = { "Start Game", "Continue", "Settings", "Exit" };
        private int selectedIndex = 0;
        
        // For mouse hover detection
        private List<RectangleF> itemBounds = new List<RectangleF>();
        
        // Background image
        private Image bgImage;
        
        // Refresh timer for smooth rendering
        private System.Windows.Forms.Timer renderTimer;

        public MainMenuForm()
        {
            InitializeForm();
            LoadResources();
            SetupTimer();
        }

        private void InitializeForm()
        {
            Text = "Space Defender";
            ClientSize = new Size(1920, 1080);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            
            // Wire up input events
            KeyDown += OnKeyDown;
            MouseClick += OnMouseClick;
            MouseMove += OnMouseMove;
        }

        private void LoadResources()
        {
            bgImage = ResourceLoader.MenuBackground3;
        }

        private void SetupTimer()
        {
            renderTimer = new System.Windows.Forms.Timer { Interval = 16 };
            renderTimer.Tick += (s, e) => Invalidate();
            renderTimer.Start();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            
            // Load all game sounds
            LoadSounds();
            
            // Play menu music when form shows
            SoundManager.PlayMusic("MenuMusic", GameDataManager.CurrentData.MusicVolume);
        }

        private void LoadSounds()
        {
            // Load all sound effects and music
            SoundManager.LoadSound("MenuMusic", "menu_music.wav");
            SoundManager.LoadSound("LevelMusic", "main.mp3");
            SoundManager.LoadSound("Shoot", "shoot.wav");
            SoundManager.LoadSound("EnemyDie", "enemy_die.wav");
            SoundManager.LoadSound("Pickup", "pickup.wav");
            SoundManager.LoadSound("Jump", "Jump.wav");
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
                case Keys.Enter:
                    ExecuteSelection();
                    break;
            }
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            ExecuteSelection();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            // Update selection based on mouse position
            for (int i = 0; i < itemBounds.Count; i++)
            {
                if (itemBounds[i].Contains(e.X, e.Y) && !IsItemDisabled(i))
                {
                    selectedIndex = i;
                    break;
                }
            }
        }

        private void MoveSelection(int direction)
        {
            selectedIndex += direction;
            
            // Wrap around
            if (selectedIndex < 0) selectedIndex = menuItems.Length - 1;
            if (selectedIndex >= menuItems.Length) selectedIndex = 0;
            
            // Skip disabled items
            if (IsItemDisabled(selectedIndex))
                MoveSelection(direction);
        }

        private bool IsItemDisabled(int index)
        {
            // Continue is disabled if no progress saved
            return menuItems[index] == "Continue" && GameDataManager.CurrentData.MaxLevelUnlocked <= 1;
        }

        private void ExecuteSelection()
        {
            string selected = menuItems[selectedIndex];
            
            switch (selected)
            {
                case "Start Game":
                    StartLevel(1);
                    break;
                case "Continue":
                    if (!IsItemDisabled(selectedIndex))
                        StartLevel(GameDataManager.CurrentData.MaxLevelUnlocked);
                    break;
                case "Settings":
                    OpenSettings();
                    break;
                case "Exit":
                    Application.Exit();
                    break;
            }
        }

        private void StartLevel(int level)
        {
            renderTimer.Stop();
            SoundManager.StopMusic();
            
            // Create appropriate level form
            Form levelForm = level switch
            {
                1 => new Level1Form(),
                2 => new Level2Form(),
                3 => new Level3Form(),
                _ => new Level1Form()
            };
            
            Hide();
            levelForm.Show();
        }

        private void OpenSettings()
        {
            renderTimer.Stop();
            
            var settings = new SettingsForm();
            Hide();
            settings.FormClosed += (s, e) => 
            {
                Show();
                renderTimer.Start();
            };
            settings.Show();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            itemBounds.Clear();
            
            DrawBackground(g);
            DrawTitle(g);
            DrawMenuItems(g);
            DrawFooter(g);
        }

        private void DrawBackground(Graphics g)
        {
            int w = ClientSize.Width;
            int h = ClientSize.Height;
            
            if (bgImage != null)
            {
                g.DrawImage(bgImage, 0, 0, w, h);
                
                // Dark overlay for better text visibility
                using (var overlay = new SolidBrush(Color.FromArgb(100, 0, 0, 20)))
                    g.FillRectangle(overlay, 0, 0, w, h);
            }
            else
            {
                // Fallback gradient
                using (var brush = new LinearGradientBrush(
                    new Point(0, 0), new Point(0, h),
                    Color.FromArgb(255, 15, 15, 45), Color.FromArgb(255, 5, 5, 25)))
                {
                    g.FillRectangle(brush, 0, 0, w, h);
                }
            }
        }

        private void DrawTitle(Graphics g)
        {
            int w = ClientSize.Width;
            
            using (var titleFont = new Font("Impact", 80, FontStyle.Bold))
            using (var subtitleFont = new Font("Arial", 20, FontStyle.Italic))
            {
                string title = "SPACE DEFENDER";
                var titleSize = g.MeasureString(title, titleFont);
                float titleX = (w - titleSize.Width) / 2;
                float titleY = 100;

                // Glow effect
                for (int i = 4; i > 0; i--)
                {
                    using (var glowBrush = new SolidBrush(Color.FromArgb(25, 0, 200, 255)))
                        g.DrawString(title, titleFont, glowBrush, titleX - i * 2, titleY - i * 2);
                }

                // Main title with gradient
                using (var gradientBrush = new LinearGradientBrush(
                    new PointF(titleX, titleY), new PointF(titleX, titleY + titleSize.Height),
                    Color.Cyan, Color.DeepSkyBlue))
                {
                    g.DrawString(title, titleFont, gradientBrush, titleX, titleY);
                }

                // Subtitle
                string subtitle = "Defend Earth Against The Alien Invasion";
                var subSize = g.MeasureString(subtitle, subtitleFont);
                g.DrawString(subtitle, subtitleFont, Brushes.LightGray,
                    (w - subSize.Width) / 2, titleY + titleSize.Height + 15);
            }
        }

        private void DrawMenuItems(Graphics g)
        {
            int w = ClientSize.Width;
            int h = ClientSize.Height;
            
            using (var menuFont = new Font("Arial", 32, FontStyle.Bold))
            {
                float startY = h / 2 + 80;
                float spacing = 80;

                for (int i = 0; i < menuItems.Length; i++)
                {
                    string item = menuItems[i];
                    bool isSelected = i == selectedIndex;
                    bool isDisabled = IsItemDisabled(i);

                    var itemSize = g.MeasureString(item, menuFont);
                    float itemX = (w - itemSize.Width) / 2;
                    float itemY = startY + i * spacing;

                    // Store bounds for mouse detection
                    itemBounds.Add(new RectangleF(itemX - 50, itemY - 10, itemSize.Width + 100, itemSize.Height + 20));

                    if (isDisabled)
                    {
                        // Grayed out disabled items
                        g.DrawString(item, menuFont, new SolidBrush(Color.FromArgb(80, 100, 100, 100)), itemX, itemY);
                        continue;
                    }

                    if (isSelected)
                    {
                        // Highlight box for selected item
                        float boxPadding = 30;
                        float boxX = itemX - boxPadding - 40;
                        float boxY = itemY - 10;
                        float boxWidth = itemSize.Width + boxPadding * 2 + 80;
                        float boxHeight = itemSize.Height + 20;

                        using (var boxBrush = new LinearGradientBrush(
                            new PointF(boxX, boxY), new PointF(boxX + boxWidth, boxY),
                            Color.FromArgb(200, 0, 100, 180), Color.FromArgb(200, 0, 60, 120)))
                        {
                            g.FillRectangle(boxBrush, boxX, boxY, boxWidth, boxHeight);
                        }
                        g.DrawRectangle(new Pen(Color.Cyan, 3), boxX, boxY, boxWidth, boxHeight);

                        // Selected text with arrows
                        string displayText = "►  " + item + "  ◄";
                        var displaySize = g.MeasureString(displayText, menuFont);
                        g.DrawString(displayText, menuFont, Brushes.White, (w - displaySize.Width) / 2, itemY);
                    }
                    else
                    {
                        // Normal item
                        g.DrawString(item, menuFont, new SolidBrush(Color.FromArgb(200, 200, 200, 200)), itemX, itemY);
                    }
                }
            }
        }

        private void DrawFooter(Graphics g)
        {
            int w = ClientSize.Width;
            int h = ClientSize.Height;
            
            using (var smallFont = new Font("Arial", 16))
            using (var tinyFont = new Font("Arial", 14))
            {
                // High score and level info
                g.DrawString($"HIGH SCORE: {GameDataManager.CurrentData.HighScore}", smallFont, Brushes.Gold, 25, h - 90);
                g.DrawString($"MAX LEVEL: {GameDataManager.CurrentData.MaxLevelUnlocked}", smallFont, Brushes.LightGreen, 25, h - 55);

                // Controls help
                string controls = "ARROWS: Move  |  SPACE: Jump  |  X/SHIFT: Shoot  |  Q: Shoot Up  |  ENTER: Select  |  ESC: Pause";
                var controlsSize = g.MeasureString(controls, tinyFont);

                float controlsY = h - 40;
                using (var bgBrush = new SolidBrush(Color.FromArgb(150, 0, 0, 0)))
                    g.FillRectangle(bgBrush, 0, controlsY - 8, w, 40);

                g.DrawString(controls, tinyFont, Brushes.Gray, (w - controlsSize.Width) / 2, controlsY);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            renderTimer?.Stop();
            
            if (e.CloseReason == CloseReason.UserClosing)
                Application.Exit();
            
            base.OnFormClosing(e);
        }
    }
}
