using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ConnectedComponentLabeling
{
    public class Pixel
    {
        #region Public Properties

        public Point Position { get; set; }
        public Color color { get; set; }

        #endregion

        #region Constructor

        public Pixel(Point Position, Color color)
        {
            this.Position = Position;
            this.color = color;
        }

        #endregion
    }
}