# EleCho.ScratchGame

一个小型的的, 基于 GDI+ 的 2D 游戏引擎. [点击查看预览视频](https://www.bilibili.com/video/BV1RD4y167JH)

> 灵感来自于: MIT 的 [Scratch](https://scratch.mit.edu/)

## 功能 / Features

- [x] 游戏对象
- [x] 鼠标与键盘
- [x] 游戏角色缩放与旋转

## 入门 / Get started

要进行游戏对象的逻辑更新, 你需要创建一个 Game 对象, 它用来承载所有的游戏对象(GameObject)

```csharp
Game game = new Game();
```

接下来, 你可以向游戏中添加一个最简单的贴图:

```csharp
Bitmap bmp;  // 假设这是我们要显示的贴图
game.AddSprite(new GameSprite()
{
    Sprite = bmp    // 为 GameSprite 赋贴图
});
```

为了在 WinForm 窗口中进行游戏渲染, 你需要一个 GamePanel, 将其拖动到窗口, 然后在指定其要渲染的游戏

```csharp
void Load(object sender, EventArgs args)
{
    gamePanel.Game = game;    // 为 GamePanel 指定要渲染的游戏
}
```

要启动游戏逻辑更新循环以及渲染循环, 调用其方法即可:

```csharp
game.StartGame();
gamePanel.StartRender();
```

## 使用 / Usage

### 自定义游戏对象

实现复杂的功能, 你必须使用定义自己的类型, 继承 GameSprite 或者 GameText, 并重写相关方法.

下面是一个不断向右移动的游戏对象定义:

```csharp
class MoveRightForever : GameSprite
{
    public float Speed { get; set; } = 1f;
    public override void Update()
    {
        // SizeF 表示位移, 乘以 Game.DeltaTime 以使其速度不受帧率变化所影响
        Position += new SizeF(Speed, 0) * Game.DeltaTime;
    }
}
```

### 碰撞检测

GameSprite 包含默认的简单的碰撞检测, 它基于多边形的碰撞检测.

通过 Game 类的静态 IsCollided 方法来判断两个游戏对象是否碰撞

```csharp
Game.IsCollided(a, b)
```

你可以自定义游戏对象的碰撞器多边形, 只需要为其 Collider 属性赋值即可

```csharp
gameSprite.Collider = new GameObjectCollider(
    new PointF(.25f, .25f),          // 多边形顶点坐标 (0,0 为左上角, 1,1 为右下角)
    new PointF(.75f, .25f),
    new PointF(.75f, .75f),
    new PointF(.75f, .25f));
```

### 精灵动画

通过变换 Sprite 的值, 来实现简单的动画

```csharp
class Warplane : GameSprite
{
    Bitmap[] bodies;
    public Warplane(Bitmap[] bodies)
    {
        this.bodies = bodies;
        SpriteChangeLoop();
    }
    
    private async void SpriteChangeLoop()
    {
        while (true)
        {
            foreach (var me in bodies)
            {
                await Task.Delay(100);
                Sprite = me;
            }
        }
    }
}
```