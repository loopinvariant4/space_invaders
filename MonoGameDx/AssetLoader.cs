using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SI
{
    public class AssetLoader
    {
        public ContentManager Content { get; set; }
        public SpriteBatch SpriteBatch { get; set; }

        private Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();

        public AssetLoader(ContentManager mgr, SpriteBatch sbtch)
        {
            Content = mgr;
            SpriteBatch = sbtch;
        }

        public Texture2D LoadTexture(string name)
        {
            if(textureCache.ContainsKey(name))
            {
                return textureCache[name];
            }
            var texture = Content.Load<Texture2D>(name);
            textureCache.Add(name, texture);
            return texture;
        }
        
    }
}
