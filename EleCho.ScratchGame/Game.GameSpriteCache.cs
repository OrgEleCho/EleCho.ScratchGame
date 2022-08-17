using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EleCho.ScratchGame
{

    public partial class Game
    {
        public class GameSpriteCache : IDictionary<GameSpriteCacheKey, GameSpriteCacheItem>
        {
            LinkedList<KeyValuePair<GameSpriteCacheKey, GameSpriteCacheItem>> list = new LinkedList<KeyValuePair<GameSpriteCacheKey, GameSpriteCacheItem>>();
            Dictionary<GameSpriteCacheKey, GameSpriteCacheItem> dict = new Dictionary<GameSpriteCacheKey, GameSpriteCacheItem>();

            Game game;
            public GameSpriteCache(Game game)
            {
                this.game = game;
            }

            /// <summary>
            /// Timeout in second
            /// </summary>
            float Timeout { get; set; } = 100;

            public void Update()
            {
                while (list.First is LinkedListNode<KeyValuePair<GameSpriteCacheKey, GameSpriteCacheItem>> firstNode && 
                    game.Time - firstNode.Value.Value.Time > Timeout)
                {
                    list.RemoveFirst();

                    if (game.Time - firstNode.Value.Value.AccessTime < Timeout)
                        list.AddLast(firstNode);
                }
            }

            public GameSpriteCacheItem this[GameSpriteCacheKey key]
            {
                get
                {

                    GameSpriteCacheItem item = dict[key];
                    item.AccessTime = game.Time;
                    return item;
                }
                set
                {
                    if (!dict.ContainsKey(key))
                    {
                        list.AddLast(new KeyValuePair<GameSpriteCacheKey, GameSpriteCacheItem>(key, value));
                    }

                    dict[key] = value;
                }
            }

            public ICollection<GameSpriteCacheKey> Keys => dict.Keys;
            public ICollection<GameSpriteCacheItem> Values => dict.Values;

            public int Count => list.Count;

            bool ICollection<KeyValuePair<GameSpriteCacheKey, GameSpriteCacheItem>>.IsReadOnly => false;

            public void Add(GameSpriteCacheKey key, GameSpriteCacheItem value)
            {
                dict.Add(key, value);
                list.AddLast(new KeyValuePair<GameSpriteCacheKey, GameSpriteCacheItem>(key, value));
            }
            public void Add(KeyValuePair<GameSpriteCacheKey, GameSpriteCacheItem> item)
            {
                dict.Add(item.Key, item.Value);
                list.AddLast(item);
            }
            public void Clear()
            {
                dict.Clear();
                list.Clear();
            }
            public bool Contains(KeyValuePair<GameSpriteCacheKey, GameSpriteCacheItem> item) => dict.Contains(item);
            public bool ContainsKey(GameSpriteCacheKey key) => dict.ContainsKey(key);
            public void CopyTo(KeyValuePair<GameSpriteCacheKey, GameSpriteCacheItem>[] array, int arrayIndex) => 
                (dict as IDictionary<GameSpriteCacheKey, GameSpriteCacheItem>).CopyTo(array, arrayIndex);
            public bool Remove(GameSpriteCacheKey key)
            {
                if (dict.TryGetValue(key, out var item))
                {
                    list.Remove(new KeyValuePair<GameSpriteCacheKey, GameSpriteCacheItem>(key, item));
                    return true;
                }

                return false;
            }
            public bool Remove(KeyValuePair<GameSpriteCacheKey, GameSpriteCacheItem> item)
            {
                if (dict.Remove(item.Key))
                {
                    list.Remove(item);
                    return true;
                }

                return false;
            }
            public bool TryGetValue(GameSpriteCacheKey key, [MaybeNullWhen(false)] out GameSpriteCacheItem value)
            {
                if (dict.TryGetValue(key, out value))
                {
                    value.AccessTime = game.Time;
                    return true;
                }

                return false;
            }

            public IEnumerator<KeyValuePair<GameSpriteCacheKey, GameSpriteCacheItem>> GetEnumerator() => dict.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => dict.GetEnumerator();
        }
    }
}
