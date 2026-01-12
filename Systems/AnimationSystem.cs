using GameFrameWork;
using System.Collections.Generic;
using System.Drawing;

namespace FirstDesktopApp.Systems
{
    public class AnimationSystem
    {
        private List<Image> frames;
        private int currentFrame;
        private float timer;
        private float speed;

        public AnimationSystem(List<Image> frames, float speed)
        {
            this.frames = frames;
            this.speed = speed;
            this.timer = 0;
            this.currentFrame = 0;
        }

        public Image GetCurrentFrame()
        {
            if (frames == null || frames.Count == 0)
                return null;
            return frames[currentFrame];
        }

        public void Update(GameTime gameTime)
        {
            if (frames == null || frames.Count == 0)
                return;

            timer += gameTime.DeltaTime;

            if (timer > speed)
            {
                timer = 0;
                currentFrame++;
                
                if (currentFrame >= frames.Count)
                    currentFrame = 0;
            }
        }

        public void SetFrames(List<Image> newFrames)
        {
            if (frames == newFrames)
                return;
                
            frames = newFrames;
            currentFrame = 0;
            timer = 0;
        }
    }
}
