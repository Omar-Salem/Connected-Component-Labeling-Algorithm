using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace PatternRecognition
{
    public class PreProcess : ConnectedComponentLabeling
    {
        readonly Bitmap Input;

        public PreProcess(Bitmap input)
            : base(input)
        {
            this.Input = input;
        }

        public List<Bitmap> Extract()
        {
            int w, h, widthShift, heightShift;
            Bitmap output;
            List<Bitmap> images = new List<Bitmap>();
            Dictionary<int, List<Pixel>> Patterns = Find();
            foreach (KeyValuePair<int, List<Pixel>> shape in Patterns)
            {
                w = GetDimension(shape.Value, out widthShift, true);
                h = GetDimension(shape.Value, out heightShift, false);
                output = new Bitmap(w, h);
                foreach (Pixel pix in shape.Value)
                {
                    output.SetPixel(pix.Position.X - widthShift, pix.Position.Y - heightShift, pix.color);
                }
                images.Add(output);
            }
            return images;
        }
    }
}
