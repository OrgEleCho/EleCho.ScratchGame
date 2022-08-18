using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace EleCho.ScratchGame
{

    public partial class Game
    {
        public record struct GameSpriteCacheKey
        {
            public readonly Image Sprite;
            public readonly float Scale;
            public readonly float Rotation;

            public GameSpriteCacheKey(Image sprite, float scale, float rotation)
            {
                Sprite = sprite;
                Scale = scale;
                Rotation = rotation;
            }
        }

        public record class GameSpriteCacheItem
        {
            public readonly Bitmap CachedSprite;
            public readonly float Time;
            public float AccessTime;

            public GameSpriteCacheItem(Bitmap cachedSprite, float time)
            {
                CachedSprite = cachedSprite;
                AccessTime = time;
                Time = time;
            }
        }
        public class GameSpriteCache : IDictionary<GameSpriteCacheKey, Bitmap>
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
            float Timeout { get; set; } = 30;

            public void Update()
            {
                while (list.First is LinkedListNode<KeyValuePair<GameSpriteCacheKey, GameSpriteCacheItem>> firstNode && 
                       game.Time - firstNode.ValueRef.Value.Time > Timeout)
                {
                    list.RemoveFirst();

                    if (game.Time - firstNode.ValueRef.Value.AccessTime < Timeout)
                    {
                        list.AddLast(new LinkedListNode<KeyValuePair<GameSpriteCacheKey, GameSpriteCacheItem>>(new KeyValuePair<Game.GameSpriteCacheKey, Game.GameSpriteCacheItem>(
                            firstNode.ValueRef.Key,
                            new GameSpriteCacheItem(firstNode.ValueRef.Value.CachedSprite, game.Time))));
                    }
                }
            }

            public Bitmap this[GameSpriteCacheKey key]
            {
                get
                {
                    GameSpriteCacheItem item = dict[key];
                    item.AccessTime = game.Time;
                    return item.CachedSprite;
                }
                set
                {
                    if (!dict.ContainsKey(key))
                    {
                        list.AddLast(new KeyValuePair<GameSpriteCacheKey, GameSpriteCacheItem>(key, new GameSpriteCacheItem(value, game.Time)));
                    }

                    dict[key] = new GameSpriteCacheItem(value, game.Time);
                }
            }

            public ICollection<GameSpriteCacheKey> Keys => dict.Keys;
            public ICollection<Bitmap> Values => dict.Values.Select(v => v.CachedSprite).ToList();

            public int Count => list.Count;

            bool ICollection<KeyValuePair<GameSpriteCacheKey, Bitmap>>.IsReadOnly => false;

            public void Add(GameSpriteCacheKey key, Bitmap value)
            {
                GameSpriteCacheItem newItem = new GameSpriteCacheItem(value, game.Time);
                dict.Add(key, newItem);
                list.AddLast(new KeyValuePair<GameSpriteCacheKey, GameSpriteCacheItem>(key, newItem));
            }
            public void Add(KeyValuePair<GameSpriteCacheKey, Bitmap> item)
            {
                GameSpriteCacheItem newItem = new GameSpriteCacheItem(item.Value, game.Time);
                dict.Add(item.Key, newItem);
                list.AddLast(new KeyValuePair<GameSpriteCacheKey, GameSpriteCacheItem>(item.Key, newItem));
            }
            public void Clear()
            {
                dict.Clear();
                list.Clear();
            }
            public bool Contains(KeyValuePair<GameSpriteCacheKey, Bitmap> item)
            {
                if (dict.TryGetValue(item.Key, out var cachedSprite))
                {
                    return item.Value == cachedSprite.CachedSprite;
                }

                return false;
            }

            public bool ContainsKey(GameSpriteCacheKey key) => dict.ContainsKey(key);
            void ICollection<KeyValuePair<GameSpriteCacheKey, Bitmap>>.CopyTo(KeyValuePair<GameSpriteCacheKey, Bitmap>[] array, int arrayIndex) =>
                throw new InvalidOperationException("Not supported");
            public bool Remove(GameSpriteCacheKey key) => throw new InvalidOperationException();
            public bool Remove(KeyValuePair<GameSpriteCacheKey, Bitmap> item) => throw new InvalidOperationException();
            public bool TryGetValue(GameSpriteCacheKey key, [MaybeNullWhen(false)] out Bitmap value)
            {
                if (dict.TryGetValue(key, out var cachedSprite))
                {
                    cachedSprite.AccessTime = game.Time;
                    value = cachedSprite.CachedSprite;
                    return true;
                }

                value = null;
                return false;
            }

            IEnumerator<KeyValuePair<GameSpriteCacheKey, Bitmap>> EnumAsBitmap()
            {
                foreach (var kv in dict)
                {
                    yield return new KeyValuePair<GameSpriteCacheKey, Bitmap>(kv.Key, kv.Value.CachedSprite);
                }
            }

            public IEnumerator<KeyValuePair<GameSpriteCacheKey, Bitmap>> GetEnumerator() => EnumAsBitmap();
            IEnumerator IEnumerable.GetEnumerator() => dict.GetEnumerator();
        }
    }
}
