using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace PatternRecognition
{
   public class Pixel
    {
        public Point Position { get; set; }
        public Color color { get; set; }

        public Pixel(Point Position, Color color)
        {
            this.Position = Position;
            this.color = color;
        }
    }
}
