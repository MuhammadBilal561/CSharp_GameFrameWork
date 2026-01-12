using System;
using System.Drawing;

namespace FirstDesktopApp.Systems
{
    public class ScrollingBackground
    {
        private Image sourceImage;
        private Bitmap scaledImage;
        private float scrollX = 0;
        private float scrollSpeed = 0.5f;
        private int screenWidth;
        private int screenHeight;
        private int imageWidth;
        private int imageHeight;
        private float maxScroll;

        public ScrollingBackground(int screenWidth, int screenHeight)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
        }

        public void SetBackground(Image image)
        {
            if (image == null) return;
            
            sourceImage = image;
            
            float zoomFactor = 1.3f;
            imageHeight = (int)(screenHeight * zoomFactor);
            
            float aspectRatio = (float)image.Width / image.Height;
            imageWidth = (int)(imageHeight * aspectRatio);
            
            if (imageWidth < screenWidth * 1.5f)
            {
                imageWidth = (int)(screenWidth * 1.5f);
                imageHeight = (int)(imageWidth / aspectRatio);
            }
            
            scaledImage = new Bitmap(image, imageWidth, imageHeight);
            maxScroll = imageWidth - screenWidth;
            scrollX = maxScroll / 2;
        }

        public void Update(float playerVelocityX)
        {
            if (scaledImage == null) return;
            
            scrollX += playerVelocityX * scrollSpeed;
            
            if (scrollX < 0) scrollX = 0;
            if (scrollX > maxScroll) scrollX = maxScroll;
        }

        public void Draw(Graphics g)
        {
            if (scaledImage == null)
            {
                g.Clear(Color.DarkSlateGray);
                return;
            }

            int srcX = (int)scrollX;
            int srcY = (imageHeight - screenHeight) / 2;
            
            g.DrawImage(scaledImage,
                new Rectangle(0, 0, screenWidth, screenHeight),
                new Rectangle(srcX, srcY, screenWidth, screenHeight),
                GraphicsUnit.Pixel);
        }

        public void Reset()
        {
            scrollX = maxScroll / 2;
        }
    }
}
