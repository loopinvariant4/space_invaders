using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SI
{
    public class GameOver : IGameStage
    {
        #region vars
        private SpriteFont font;
        public event EventHandler End;
        private TimeSpan WAITTIME = TimeSpan.FromSeconds(1);
        private TimeSpan currentWaitTime = TimeSpan.Zero;
        #endregion


        #region ctor
        public GameOver(string id)
        {
            Id = id;
        }
        #endregion

        #region interface methods
        public string Id {get;}

        public IGameStage Next { get; set; }

        public void BeforeStart(GameStageSettings settings = null)
        {
            font = DIContainer.Get<AssetLoader>("AssetLoader").Content.Load<SpriteFont>("courier");
        }

        public void Draw(GameTime gt, SpriteBatch spriteBatch)
        {
            var caption = "GAME OVER";
            var instruction = "Press any key to continue";
            var captionSize = font.MeasureString(caption);
            spriteBatch.DrawString(font, caption, new Vector2(Env.Screen.Width / 2 - captionSize.Length() / 2, Env.Screen.Height / 2), Color.White);

            if (currentWaitTime >= WAITTIME)
            {
                var instructionSize = font.MeasureString(instruction);
                spriteBatch.DrawString(font, instruction, new Vector2(Env.Screen.Width / 2 - instructionSize.Length() / 2, (Env.Screen.Height / 2) + 50), Color.White);
            }
        }

        public void Start()
        {
        }

        public void BeforeEnd()
        {

        }

        public void Update(GameTime gameTime, GameInput input)
        {
            currentWaitTime += gameTime.ElapsedGameTime;
            if (currentWaitTime >= WAITTIME)
            {
                var keys = input.Keys;
                if (keys.Length > 0)
                {
                    End?.Invoke(this, null);
                }
            }
        }
        #endregion
    }
}
