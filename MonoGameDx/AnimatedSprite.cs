using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace SI
{
    public class AnimatedSprite
    {
        private Texture2D texture;
        private readonly int rows;
        private readonly int cols;
        private readonly int width;
        private readonly int height;
        private int currentFrame;
        private readonly int totalFrames;
        private int frameRate;
        public int FrameRate
        {
            get
            {
                return frameRate;
            }
            set
            {
                frameRate = value;
                this.frameInterval = frameRate > 0 ? TimeSpan.FromMilliseconds(1000 / FrameRate) : TimeSpan.FromMilliseconds(0);
            }
        }

        private TimeSpan frameInterval;
        private TimeSpan lastElapsedTime;
        public Point Position;
        private bool isHidden = false;
        private readonly bool shouldLoop = true;
        private readonly Action animationEnd;
        private readonly string id;
        private Rectangle box = new Rectangle();

        public int Width => width;

        public int Height => height;

        public event EventHandler AnimationComplete;

        public Rectangle Box
        {
            get
            {
                box.X = Position.X;
                box.Y = Position.Y;
                box.Width = width;
                box.Height = height;

                return box;
            }
        }

        public Texture2D Texture => texture;

        public AnimatedSprite(string spriteId, int rows, int cols, int frameRate, bool shouldLoop, Point startPosition, Action animationEnd)
        {
            this.id = spriteId;
            this.texture = DIContainer.Get<AssetLoader>("AssetLoader").LoadTexture(id);
            this.rows = rows;
            this.cols = cols;
            this.width = texture.Width;
            this.height = texture.Height / rows;
            this.FrameRate = frameRate;
            this.Position = startPosition;
            this.lastElapsedTime = TimeSpan.FromMilliseconds(0);
            this.totalFrames = rows * cols;
            this.shouldLoop = shouldLoop;
            this.animationEnd = animationEnd;

        }

        public void Update(GameTime gameTime)
        {
            if (isHidden == true) return;

            if (totalFrames > 1)
            {
                lastElapsedTime += gameTime.ElapsedGameTime;

                if (lastElapsedTime > frameInterval)
                {
                    currentFrame = (currentFrame + 1) % totalFrames;
                    lastElapsedTime = TimeSpan.FromMilliseconds(0);
                    if(currentFrame == 0)
                    {
                        AnimationComplete?.Invoke(this, null);
                    }
                    if (shouldLoop == false && currentFrame == 0)
                    {
                        animationEnd();
                        return;
                    }
                }
            }
        }

        public void Draw(SpriteBatch batch)
        {
            if (isHidden) return;

            int row = currentFrame / cols;
            int col = currentFrame % cols;

            Rectangle src = new Rectangle(width * col, height * row, width, height);
            Rectangle dest = new Rectangle(Position.X, Position.Y, width, height);

            batch.Draw(texture, dest, src, Color.White);
        }

        public void Show()
        {
            this.isHidden = false;
        }

        public void Hide()
        {
            this.isHidden = true;
        }
    }
}
