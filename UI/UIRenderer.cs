/*
 * UIRenderer.cs
 * Renders in-game UI elements during gameplay
 * Health bar, score display, level info, pause overlay
 */

using FirstDesktopApp.Systems;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace FirstDesktopApp.UI
{
    public class UIRenderer
    {
        private Font titleFont;
        private Font normalFont;
        private Font smallFont;
        private Image? heartSprite;

        public UIRenderer()
        {
            titleFont = new Font("Arial", 24, FontStyle.Bold);
            normalFont = new Font("Arial", 16, FontStyle.Bold);
            smallFont = new Font("Arial", 12);
            heartSprite = ResourceLoader.Heart;
        }

        // Draw the main game HUD (health, score, level info)
        public void DrawGameUI(Graphics g, int health, int score, int level, int enemiesRemaining)
        {
            // Health bar section
            g.FillRectangle(new SolidBrush(Color.FromArgb(100, 0, 0, 0)), 10, 10, 220, 40);

            // Heart icon
            if (heartSprite != null)
                g.DrawImage(heartSprite, 15, 15, 30, 30);
            else
                g.FillEllipse(Brushes.Red, 15, 15, 30, 30);

            // Health bar
            float healthPercent = health / 100f;
            g.FillRectangle(Brushes.DarkRed, 50, 20, 170, 20);
            g.FillRectangle(Brushes.LimeGreen, 50, 20, 170 * healthPercent, 20);
            g.DrawRectangle(Pens.White, 50, 20, 170, 20);
            g.DrawString($"{health}%", smallFont, Brushes.White, 120, 22);

            // Score display
            g.FillRectangle(new SolidBrush(Color.FromArgb(100, 0, 0, 0)), 10, 55, 150, 30);
            g.DrawString($"Score: {score}", normalFont, Brushes.Gold, 15, 58);

            // Level and enemy count
            g.FillRectangle(new SolidBrush(Color.FromArgb(100, 0, 0, 0)), 10, 90, 180, 30);
            g.DrawString($"Level {level} - Enemies: {enemiesRemaining}", smallFont, Brushes.White, 15, 95);
        }

        // Draw pause screen overlay
        public void DrawPausedOverlay(Graphics g, int screenWidth, int screenHeight)
        {
            // Dark overlay
            g.FillRectangle(new SolidBrush(Color.FromArgb(150, 0, 0, 0)), 0, 0, screenWidth, screenHeight);

            // Paused text
            string text = "PAUSED";
            var size = g.MeasureString(text, titleFont);
            g.DrawString(text, titleFont, Brushes.White, (screenWidth - size.Width) / 2, screenHeight / 2 - 50);
            g.DrawString("Press ESC to Resume", normalFont, Brushes.Gray, (screenWidth - 180) / 2, screenHeight / 2);
        }

        // Draw game over screen
        public void DrawGameOver(Graphics g, int screenWidth, int screenHeight, int finalScore)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(200, 0, 0, 0)), 0, 0, screenWidth, screenHeight);

            string title = "GAME OVER";
            var titleSize = g.MeasureString(title, titleFont);
            g.DrawString(title, titleFont, Brushes.Red, (screenWidth - titleSize.Width) / 2, screenHeight / 2 - 80);

            string scoreText = $"Final Score: {finalScore}";
            var scoreSize = g.MeasureString(scoreText, normalFont);
            g.DrawString(scoreText, normalFont, Brushes.White, (screenWidth - scoreSize.Width) / 2, screenHeight / 2 - 20);

            string highScore = $"High Score: {GameDataManager.CurrentData.HighScore}";
            var hsSize = g.MeasureString(highScore, normalFont);
            g.DrawString(highScore, normalFont, Brushes.Gold, (screenWidth - hsSize.Width) / 2, screenHeight / 2 + 20);

            g.DrawString("Press ENTER to return to Menu", smallFont, Brushes.Gray, (screenWidth - 200) / 2, screenHeight / 2 + 80);
        }

        // Draw level complete screen
        public void DrawLevelComplete(Graphics g, int screenWidth, int screenHeight, int level, int score)
        {
            g.FillRectangle(new SolidBrush(Color.FromArgb(180, 0, 50, 0)), 0, 0, screenWidth, screenHeight);

            string title = $"LEVEL {level} COMPLETE!";
            var titleSize = g.MeasureString(title, titleFont);
            g.DrawString(title, titleFont, Brushes.LimeGreen, (screenWidth - titleSize.Width) / 2, screenHeight / 2 - 60);

            string scoreText = $"Score: {score}";
            var scoreSize = g.MeasureString(scoreText, normalFont);
            g.DrawString(scoreText, normalFont, Brushes.White, (screenWidth - scoreSize.Width) / 2, screenHeight / 2);

            g.DrawString("Press ENTER to continue", smallFont, Brushes.White, (screenWidth - 160) / 2, screenHeight / 2 + 60);
        }

        // Draw victory screen
        public void DrawVictory(Graphics g, int screenWidth, int screenHeight, int finalScore)
        {
            using (var brush = new LinearGradientBrush(
                new Point(0, 0), new Point(0, screenHeight),
                Color.FromArgb(200, 0, 50, 100), Color.FromArgb(200, 0, 100, 50)))
            {
                g.FillRectangle(brush, 0, 0, screenWidth, screenHeight);
            }

            string title = "VICTORY!";
            var titleSize = g.MeasureString(title, titleFont);
            g.DrawString(title, titleFont, Brushes.Gold, (screenWidth - titleSize.Width) / 2, screenHeight / 2 - 100);

            string story = "Earth is saved! The alien invasion has been stopped!";
            var storySize = g.MeasureString(story, smallFont);
            g.DrawString(story, smallFont, Brushes.White, (screenWidth - storySize.Width) / 2, screenHeight / 2 - 40);

            string scoreText = $"Final Score: {finalScore}";
            var scoreSize = g.MeasureString(scoreText, normalFont);
            g.DrawString(scoreText, normalFont, Brushes.White, (screenWidth - scoreSize.Width) / 2, screenHeight / 2 + 20);

            string highScore = $"High Score: {GameDataManager.CurrentData.HighScore}";
            var hsSize = g.MeasureString(highScore, normalFont);
            g.DrawString(highScore, normalFont, Brushes.Gold, (screenWidth - hsSize.Width) / 2, screenHeight / 2 + 60);

            g.DrawString("Press ENTER to return to Menu", smallFont, Brushes.White, (screenWidth - 200) / 2, screenHeight / 2 + 120);
        }
    }
}
