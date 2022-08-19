
using SkyWar;
using SkyWar.Properties;
using System.Drawing;
using System.Windows.Forms;

namespace EleCho.ScratchGame
{
    public partial class MainForm : Form
    {
        Game game;
        GameScene startScene;
        GameScene mainScene;

        public MainForm()
        {
            InitializeComponent();

            game = new(gamePanel, 480, 700)
            {
                CanvasColor = Color.Pink
            };

            gamePanel.Game = game;

            GameSound gs = new GameSound();           // 背景音乐
            Background bg1 = new Background();        // 背景1
            Background bg2 = new Background();        // 背景2  (背景1和2交替滚动实现无限的背景)
            Warplane player = new Warplane();       // 玩家
            EnermyGen enermyGen = new EnermyGen();    // 敌人生成 (控制敌人生成的

            bg2.Position = new PointF(0, bg2.Sprite!.Height);

            mainScene = new GameScene();

            mainScene.AddObject(gs);
            mainScene.AddObject(bg1);
            mainScene.AddObject(bg2);
            mainScene.AddObject(player);
            mainScene.AddObject(enermyGen);

            game.AddObject(new RotatingText()
            {
                Text = "Hello world",
                Scale = 3,
                Rotation = 45
            });
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            MainForm_Resize(this, null!);

            game.StartGame();
            gamePanel.StartRender();
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            Point center = (Point)((ClientSize - gamePanel.Size) / 2);
            gamePanel.Location = center;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            game.LoadScene(mainScene);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (game.IsRunning)
                game.StopGame();
        }
    }
}