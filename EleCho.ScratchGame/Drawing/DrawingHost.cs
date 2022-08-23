using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EleCho.ScratchGame.Drawing
{
    internal interface IDrawingHost
    {
        IDrawingContext GetContext(int width, int height);
    }
}
