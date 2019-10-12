using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI
{
    /// <summary>
    /// This interface defines the different lifecycle stage of a game. Each stage has a pointer to the next stage and loads it when the current stage comes to an end
    /// E.g. the game might have 3 stages - loadscreen, gameplay, endgame
    /// </summary>
    public interface IGameStage
    {
        /// <summary>
        /// Unique identifier for this stage
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Reference to the next gamestage; It is an error to not point to a next gamestage. The last stage should circle back to the first one
        /// </summary>
        IGameStage Next { get; set; }

        /// <summary>
        /// Any actions to be performed before we start this stage 
        /// </summary>
        void BeforeStart(GameStageSettings settings = null);

        /// <summary>
        /// Start this game stage
        /// </summary>
        void Start();

        /// <summary>
        /// Perform any actions before the gamestage is ended
        /// </summary>
        void BeforeEnd();

        /// <summary>
        /// Any actions to be taken when this stage ends
        /// </summary>
        event EventHandler End;

        /// <summary>
        /// Called for each tick to update the game logic
        /// </summary>
        /// <param name="gameTime"></param>
        void Update(GameTime gameTime, GameInput input);

        /// <summary>
        /// Called for each tick of the game to render the objects on the screen from this stage
        /// </summary>
        /// <param name="gt"></param>
        /// <param name="spriteBatch"></param>
        void Draw(GameTime gt, SpriteBatch spriteBatch);
    }
}
