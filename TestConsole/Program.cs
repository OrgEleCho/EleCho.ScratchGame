using EleCho.ScratchGame;

Dictionary<Game.GameTextCacheKey, string> dict = new Dictionary<Game.GameTextCacheKey, string>();

Font font = SystemFonts.DefaultFont;
Brush brush = Brushes.Black;

dict[new Game.GameTextCacheKey(font, brush, "hello", 10, 1)] = "114514";
dict[new Game.GameTextCacheKey(font, brush, "hello", 10, 1)] = "qwqqwq";

Console.WriteLine(dict.Count);
