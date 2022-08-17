using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleCho.ScratchGame
{
    public class GameScene : ICollection<GameSprite>
    {
        private readonly Game game;
        private readonly List<GameSprite> sprites = new List<GameSprite>();

        public Color CanvasColor { get; set; }
        public Image? Background { get; set; }
        public List<GameSprite> Sprites => sprites;

        public int Count => sprites.Count;

        public bool IsReadOnly => false;

        public GameScene(Game game)
        {
            this.game = game;
        }

        public void AddSprite(GameSprite sprite) => sprites.Add(sprite);
        public bool RemoveSprite(GameSprite sprite) => sprites.Remove(sprite);
        public bool ContainsSprite(GameSprite sprite) => sprites.Contains(sprite);
        public void RemoveAllSprites() => sprites.Clear();

        void ICollection<GameSprite>.CopyTo(GameSprite[] array, int arrayIndex) => sprites.CopyTo(array, arrayIndex);
        public IEnumerator<GameSprite> GetEnumerator() => sprites.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        void ICollection<GameSprite>.Add(GameSprite item) => sprites.Add(item);
        bool ICollection<GameSprite>.Remove(GameSprite item) => sprites.Remove(item);
        bool ICollection<GameSprite>.Contains(GameSprite item) => sprites.Contains(item);
        void ICollection<GameSprite>.Clear() => sprites.Clear();
    }
}
