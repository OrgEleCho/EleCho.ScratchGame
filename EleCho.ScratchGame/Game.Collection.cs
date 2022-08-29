using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleCho.ScratchGame
{

    public partial class Game : ICollection<GameObject>, IEnumerable<GameObject>
    {
        private readonly List<GameObject> sprites = new List<GameObject>();

        public int Count => sprites.Count;
        public bool IsReadOnly => false;

        /// <summary>
        /// Add a game object to the game
        /// </summary>
        /// <param name="obj"></param>
        public void AddObject(GameObject obj)
        {
            if (obj.game != null)
                throw new ArgumentException("Sprite is already in a game");
            if (obj.game == this)
                throw new ArgumentException("Sprite is already in this game");
            obj.game = this;
            sprites.Add(obj);
            obj.Load();

            if (IsRunning)
                obj.Start();
        }

        /// <summary>
        /// Remove a game object from the game
        /// </summary>
        /// <param name="obj">GameObject to remove</param>
        /// <returns>trus if the sprite removed from this game; otherwise, false</returns>
        public bool RemoveObject(GameObject obj)
        {
            if (sprites.Remove(obj))
            {
                obj.game = null;
                obj.Unload();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Remove all sprites in the game
        /// </summary>
        public void RemoveAllObjects()
        {
            foreach (GameObject obj in sprites)
            {
                obj.game = null;
                obj.Unload();
            }
            sprites.Clear();
        }

        /// <summary>
        /// Check if the game object is in the game
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool ContainsObject(GameObject obj) => sprites.Contains(obj);

        public IEnumerable<GameObject> EnumObjects()
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                yield return sprites[i];
            }
        }

        private IEnumerator<GameObject> GetObjectsEnumerator()
        {
            for (int i = 0; i < sprites.Count; i++)
            {
                yield return sprites[i];
            }
        }

        IEnumerator<GameObject> IEnumerable<GameObject>.GetEnumerator() => GetObjectsEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetObjectsEnumerator();
        void ICollection<GameObject>.Add(GameObject item) => AddObject(item);
        bool ICollection<GameObject>.Remove(GameObject item) => RemoveObject(item);
        void ICollection<GameObject>.Clear() => RemoveAllObjects();
        bool ICollection<GameObject>.Contains(GameObject item) => ContainsObject(item);
        void ICollection<GameObject>.CopyTo(GameObject[] array, int arrayIndex) => sprites.CopyTo(array, arrayIndex);
    }
}
