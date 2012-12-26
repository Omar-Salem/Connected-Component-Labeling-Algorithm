using System.Collections.Generic;
using System.Drawing;
using System.ComponentModel.Composition;
using System.Linq;

namespace ConnectedComponentLabeling
{
    [Export(typeof(IConnectedComponentLabeling))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CCL : IConnectedComponentLabeling
    {
        #region Member Variables

        private int[,] _board;
        private Bitmap _input;

        #endregion

        #region IConnectedComponentLabeling

        public IDictionary<int, Bitmap> Process(Bitmap input)
        {
            _input = input;
            int width = input.Width;
            int height = input.Height;
            _board = new int[width, height];

            Dictionary<int, List<Pixel>> patterns = Find(width, height);
            var images = new Dictionary<int, Bitmap>();

            foreach (KeyValuePair<int, List<Pixel>> pattern in patterns)
            {
                Bitmap output = CreateBitmap(pattern.Value);
                images.Add(pattern.Key, output);
            }

            return images;
        }

        #endregion

        #region Protected Methods

        protected virtual bool CheckIsForeGround(Pixel currentPixel)
        {
            return currentPixel.color.A == 255 && currentPixel.color.R == 0 && currentPixel.color.G == 0 && currentPixel.color.B == 0;
        }

        #endregion

        #region Private Methods

        private Dictionary<int, List<Pixel>> Find(int width, int height)
        {
            int labelCount = 1;
            var allLabels = new Dictionary<int, Label>();

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Pixel currentPixel = new Pixel(new Point(j, i), _input.GetPixel(j, i));

                    if (CheckIsForeGround(currentPixel))
                    {
                        HashSet<int> neighboringLabels = GetNeighboringLabels(currentPixel, width, height);
                        int currentLabel;

                        if (neighboringLabels.Count == 0)
                        {
                            currentLabel = labelCount;
                            allLabels.Add(currentLabel, new Label(currentLabel));
                            labelCount++;
                        }
                        else
                        {
                            currentLabel = neighboringLabels.Min();
                            var root = allLabels[currentLabel].GetRoot();

                            foreach (var item in neighboringLabels)
                            {
                                var root2 = allLabels[item].GetRoot();

                                if (root.Name != root2.Name)
                                {
                                    allLabels[item].Join(allLabels[currentLabel]);
                                }
                            }
                        }

                        _board[j, i] = currentLabel;
                    }
                }
            }


            Dictionary<int, List<Pixel>> patterns = AggregatePatterns(allLabels, width, height);

            return patterns;
        }

        private HashSet<int> GetNeighboringLabels(Pixel pix, int width, int height)
        {
            var neighboringLabels = new HashSet<int>();
            int x = pix.Position.Y;
            int y = pix.Position.X;

            if (x > 0)//North
            {
                CheckCorner(neighboringLabels, x - 1, y);

                if (y > 0)//North West
                {
                    CheckCorner(neighboringLabels, x - 1, y - 1);
                }

                if (y < width - 1)//North East
                {
                    CheckCorner(neighboringLabels, x - 1, y + 1);
                }
            }
            if (y > 0)//West
            {
                CheckCorner(neighboringLabels, x, y - 1);

                if (x < height - 1)//South West
                {
                    CheckCorner(neighboringLabels, x + 1, y - 1);
                }
            }
            if (y < width - 1)//East
            {
                CheckCorner(neighboringLabels, x, y + 1);

                if (x < height - 1)//South East
                {
                    CheckCorner(neighboringLabels, x + 1, y + 1);
                }
            }

            return neighboringLabels;
        }

        private void CheckCorner(HashSet<int> neighboringLabels, int i, int j)
        {
            if (_board[j, i] != 0 && !neighboringLabels.Contains(_board[j, i]))
            {
                neighboringLabels.Add(_board[j, i]);
            }
        }

        private Bitmap CreateBitmap(List<Pixel> pixels)
        {
            int widthShift, heightShift;
            int w = GetDimension(pixels, out widthShift, true);
            int h = GetDimension(pixels, out heightShift, false);
            var output = new Bitmap(w, h);

            foreach (Pixel pix in pixels)
            {
                output.SetPixel(pix.Position.X - widthShift, pix.Position.Y - heightShift, pix.color);
            }

            return output;
        }

        private int GetDimension(List<Pixel> shape, out int dimensionShift, bool isWidth)
        {
            int result = dimensionShift = CheckDimensionType(shape[0], isWidth);

            for (int i = 1; i < shape.Count; i++)
            {
                int dimension = CheckDimensionType(shape[i], isWidth);

                if (result < dimension)
                {
                    result = dimension;
                }

                if (dimensionShift > dimension)
                {
                    dimensionShift = dimension;
                }
            }

            return (result + 1) - dimensionShift;
        }

        private int CheckDimensionType(Pixel shape, bool isWidth)
        {
            return isWidth ? shape.Position.X : shape.Position.Y;
        }

        private Dictionary<int, List<Pixel>> AggregatePatterns(Dictionary<int, Label> allLabels, int width, int height)
        {
            var patterns = new Dictionary<int, List<Pixel>>();

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int patternNumber = _board[j, i];

                    if (patternNumber != 0)
                    {
                        patternNumber = allLabels[patternNumber].GetRoot().Name;

                        if (!patterns.ContainsKey(patternNumber))
                        {
                            patterns.Add(patternNumber, new List<Pixel>());
                        }

                        patterns[patternNumber].Add(new Pixel(new Point(j, i), Color.Black));
                    }
                }
            }

            return patterns;
        }

        #endregion
    }
}
