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
    public class MainScreen : IGameStage
    {
        #region vars
        private SpriteFont font;
        #endregion

        #region ctor
        public MainScreen(string id)
        {
            Id = id;
        }
        #endregion

        #region interface methods
        public string Id {get;}

        public IGameStage Next { get; set; }

        public event EventHandler End;

        public void BeforeStart(GameStageSettings settings = null)
        {
            font = DIContainer.Get<AssetLoader>("AssetLoader").Content.Load<SpriteFont>("courier");
        }

        public void BeforeEnd()
        {

        }

        public void Draw(GameTime gt, SpriteBatch spriteBatch)
        {
            var caption = "SPACE INVADERS";
            var instruction = "Press any key to begin";
            var captionSize = font.MeasureString(caption);
            var instructionSize = font.MeasureString(instruction);
            spriteBatch.DrawString(font, caption, new Vector2(Env.Screen.Width / 2 - captionSize.Length() / 2, Env.Screen.Height / 2), Color.White);
            spriteBatch.DrawString(font, instruction, new Vector2(Env.Screen.Width / 2 - instructionSize.Length() / 2, (Env.Screen.Height / 2) + 50), Color.White);
        }


        public void Start()
        {
        }

        public void Update(GameTime gameTime, GameInput input)
        {
            var keys = input.Keys;
            if(keys.Length > 0)
            {
                End?.Invoke(this, null);
            }
        }
        #endregion
    }
}
