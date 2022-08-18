using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleCho.ScratchGame
{
    public class GameScene : ICollection<GameObject>
    {
        private readonly List<GameObject> sprites = new List<GameObject>();

        public Color CanvasColor { get; set; }
        public Image? Background { get; set; }
        public List<GameObject> Sprites => sprites;

        public int Count => sprites.Count;

        public bool IsReadOnly => false;

        public GameScene()
        {

        }

        public void AddObject(GameObject sprite) => sprites.Add(sprite);
        public bool RemoveObject(GameObject sprite) => sprites.Remove(sprite);
        public bool ContainsObject(GameObject sprite) => sprites.Contains(sprite);
        public void RemoveAllObjects() => sprites.Clear();

        void ICollection<GameObject>.CopyTo(GameObject[] array, int arrayIndex) => sprites.CopyTo(array, arrayIndex);
        public IEnumerator<GameObject> GetEnumerator() => sprites.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        void ICollection<GameObject>.Add(GameObject item) => sprites.Add(item);
        bool ICollection<GameObject>.Remove(GameObject item) => sprites.Remove(item);
        bool ICollection<GameObject>.Contains(GameObject item) => sprites.Contains(item);
        void ICollection<GameObject>.Clear() => sprites.Clear();
    }
}
