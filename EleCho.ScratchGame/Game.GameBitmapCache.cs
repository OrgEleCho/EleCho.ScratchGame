using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace EleCho.ScratchGame
{

    public partial class Game
    {
        public interface IGameBitmapCacheKey
        {

        }

        public struct GameSpriteCacheKey : IGameBitmapCacheKey
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

            public override int GetHashCode()
            {
                return HashCode.Combine(Sprite, Scale, Rotation);
            }
        }

        public struct GameTextCacheKey : IGameBitmapCacheKey
        {
            public readonly string Text;
            public readonly Font Font;
            public readonly float Scale;
            public readonly float Rotation;

            public GameTextCacheKey(string text, Font font, float scale, float rotation)
            {
                Text = text;
                Font = font;
                Scale = scale;
                Rotation = rotation;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(Text, Font, Scale, Rotation);
            }
        }

        public record class GameBitmapCacheItem
        {
            public readonly Bitmap CachedBitmap;
            public readonly float Time;
            public float AccessTime;

            public GameBitmapCacheItem(Bitmap cachedSprite, float time)
            {
                CachedBitmap = cachedSprite;
                AccessTime = time;
                Time = time;
            }

            public override int GetHashCode()
            {
                return HashCode.Combine(CachedBitmap, Time, AccessTime);
            }
        }
        public class GameBitmapCache : IDictionary<IGameBitmapCacheKey, Bitmap>
        {
            LinkedList<KeyValuePair<IGameBitmapCacheKey, GameBitmapCacheItem>> list = new LinkedList<KeyValuePair<IGameBitmapCacheKey, GameBitmapCacheItem>>();
            Dictionary<IGameBitmapCacheKey, GameBitmapCacheItem> dict = new Dictionary<IGameBitmapCacheKey, GameBitmapCacheItem>();

            Game game;
            public GameBitmapCache(Game game)
            {
                this.game = game;
            }

            /// <summary>
            /// Timeout in second
            /// </summary>
            float Timeout { get; set; } = 30;

            public void Update()
            {
                while (list.First is LinkedListNode<KeyValuePair<IGameBitmapCacheKey, GameBitmapCacheItem>> firstNode &&
                       game.Time - firstNode.ValueRef.Value.Time > Timeout)
                {
                    list.RemoveFirst();

                    if (game.Time - firstNode.ValueRef.Value.AccessTime < Timeout)
                    {
                        list.AddLast(new LinkedListNode<KeyValuePair<IGameBitmapCacheKey, GameBitmapCacheItem>>(
                            new KeyValuePair<Game.IGameBitmapCacheKey, Game.GameBitmapCacheItem>(
                                firstNode.ValueRef.Key,
                                new GameBitmapCacheItem(firstNode.ValueRef.Value.CachedBitmap, game.Time))));
                    }
                    else
                    {
                        dict.Remove(firstNode.ValueRef.Key);
                    }
                }
            }

            public Bitmap this[IGameBitmapCacheKey key]
            {
                get
                {
                    GameBitmapCacheItem item = dict[key];
                    item.AccessTime = game.Time;
                    return item.CachedBitmap;
                }
                set
                {
                    if (!dict.ContainsKey(key))
                    {
                        list.AddLast(new KeyValuePair<IGameBitmapCacheKey, GameBitmapCacheItem>(key, new GameBitmapCacheItem(value, game.Time)));
                    }

                    dict[key] = new GameBitmapCacheItem(value, game.Time);
                }
            }

            public ICollection<IGameBitmapCacheKey> Keys => dict.Keys;
            public ICollection<Bitmap> Values => dict.Values.Select(v => v.CachedBitmap).ToList();

            public int Count => dict.Count;

            bool ICollection<KeyValuePair<IGameBitmapCacheKey, Bitmap>>.IsReadOnly => false;

            public void Add(IGameBitmapCacheKey key, Bitmap value)
            {
                GameBitmapCacheItem newItem = new GameBitmapCacheItem(value, game.Time);
                dict.Add(key, newItem);
                list.AddLast(new KeyValuePair<IGameBitmapCacheKey, GameBitmapCacheItem>(key, newItem));
            }
            public void Add(KeyValuePair<IGameBitmapCacheKey, Bitmap> item)
            {
                GameBitmapCacheItem newItem = new GameBitmapCacheItem(item.Value, game.Time);
                dict.Add(item.Key, newItem);
                list.AddLast(new KeyValuePair<IGameBitmapCacheKey, GameBitmapCacheItem>(item.Key, newItem));
            }
            public void Clear()
            {
                dict.Clear();
                list.Clear();
            }
            public bool Contains(KeyValuePair<IGameBitmapCacheKey, Bitmap> item)
            {
                if (dict.TryGetValue(item.Key, out var cachedSprite))
                {
                    return item.Value == cachedSprite.CachedBitmap;
                }

                return false;
            }

            public bool ContainsKey(IGameBitmapCacheKey key) => dict.ContainsKey(key);
            void ICollection<KeyValuePair<IGameBitmapCacheKey, Bitmap>>.CopyTo(KeyValuePair<IGameBitmapCacheKey, Bitmap>[] array, int arrayIndex) =>
                throw new InvalidOperationException("Not supported");
            public bool Remove(IGameBitmapCacheKey key) => throw new InvalidOperationException();
            public bool Remove(KeyValuePair<IGameBitmapCacheKey, Bitmap> item) => throw new InvalidOperationException();
            public bool TryGetValue(IGameBitmapCacheKey key, [MaybeNullWhen(false)] out Bitmap value)
            {
                if (dict.TryGetValue(key, out var cachedSprite))
                {
                    cachedSprite.AccessTime = game.Time;
                    value = cachedSprite.CachedBitmap;
                    return true;
                }

                value = null;
                return false;
            }

            IEnumerator<KeyValuePair<IGameBitmapCacheKey, Bitmap>> EnumAsBitmap()
            {
                foreach (var kv in dict)
                {
                    yield return new KeyValuePair<IGameBitmapCacheKey, Bitmap>(kv.Key, kv.Value.CachedBitmap);
                }
            }

            public IEnumerator<KeyValuePair<IGameBitmapCacheKey, Bitmap>> GetEnumerator() => EnumAsBitmap();
            IEnumerator IEnumerable.GetEnumerator() => dict.GetEnumerator();
        }
    }
}
