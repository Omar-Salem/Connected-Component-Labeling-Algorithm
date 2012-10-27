using System.Collections.Generic;
using ConnectedComponentLabeling.Contracts;
using System.Drawing;
using System.ComponentModel.Composition;
using Entities;
using System.Linq;

namespace ConnectedComponentLabeling.Implementation
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

        public IList<Bitmap> Process(Bitmap input)
        {
            _input = input;
            int width = input.Width;
            int height = input.Height;
            _board = new int[width, height];


            int w, h, widthShift, heightShift;
            Bitmap output;
            List<Bitmap> images = new List<Bitmap>();
            Dictionary<int, List<Pixel>> patternsList = Find(width, height);

            foreach (KeyValuePair<int, List<Pixel>> pattern in patternsList)
            {
                w = GetDimension(pattern.Value, out widthShift, true);
                h = GetDimension(pattern.Value, out heightShift, false);
                output = new Bitmap(w, h);

                foreach (Pixel pix in pattern.Value)
                {
                    output.SetPixel(pix.Position.X - widthShift, pix.Position.Y - heightShift, pix.color);
                }

                images.Add(output);
            }

            return images;
        }

        #endregion

        #region Protected Functions

        protected virtual bool CheckIsForeGround(Pixel currentPixel)
        {
            return currentPixel.color.A == 255 && currentPixel.color.R == 0 && currentPixel.color.G == 0 && currentPixel.color.B == 0;
        }

        #endregion

        #region Private Functions

        private Dictionary<int, List<Pixel>> Find(int width, int height)
        {
            int labelCount = 0;
            Dictionary<int, Label> allLabels = new Dictionary<int, Label>();

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Pixel currentPixel = new Pixel(new Point(j, i), _input.GetPixel(j, i));

                    if (CheckIsForeGround(currentPixel))
                    {
                        Dictionary<int, int> neighboringLabels = GetDistinctNeighboringLabels(currentPixel, width, height);
                        int currentLabel;

                        if (neighboringLabels.Count == 0)
                        {
                            currentLabel = ++labelCount;
                            allLabels.Add(currentLabel, new Label(currentLabel));
                        }
                        else
                        {
                            currentLabel = neighboringLabels.ElementAt(0).Key;
                            UnifyNeighboringLabelsParents(currentLabel, neighboringLabels, allLabels);
                        }

                        _board[j, i] = currentLabel;
                    }
                }
            }


            Dictionary<int, List<Pixel>> Patterns = AggregatePatterns(allLabels, width, height);

            return Patterns;
        }

        private Dictionary<int, int> GetDistinctNeighboringLabels(Pixel pix, int width, int height)
        {
            Dictionary<int, int> neighboringLabels = new Dictionary<int, int>();
            int x = pix.Position.Y;
            int y = pix.Position.X;

            for (int i = x - 1; i <= x + 1 && i > -1 && i < height; i++)
            {
                for (int j = y - 1; j <= y + 1 && j > -1 && j < width; j++)
                {
                    if (_board[j, i] != 0)
                    {
                        if (!neighboringLabels.ContainsKey(_board[j, i]))
                        {
                            neighboringLabels.Add(_board[j, i], 0);
                        }
                    }
                }
            }

            return neighboringLabels;
        }

        private void UnifyNeighboringLabelsParents(int currentLabel, Dictionary<int, int> neighboringLabelsList, Dictionary<int, Label> allLabels)
        {
            Label root = allLabels[currentLabel].GetRoot();

            foreach (int neighborLabel in neighboringLabelsList.Keys)
            {
                if (neighborLabel != currentLabel)
                {
                    Label neighbor = allLabels[neighborLabel];

                    if (neighbor.GetRoot() != root)
                    {
                        neighbor.Root = root;
                    }
                }
            }
        }

        private Dictionary<int, List<Pixel>> AggregatePatterns(Dictionary<int, Label> allLabels, int width, int height)
        {
            Dictionary<int, List<Pixel>> Patterns = new Dictionary<int, List<Pixel>>();

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int patternNumber = _board[i, j];

                    if (patternNumber != 0)
                    {
                        patternNumber = allLabels[patternNumber].GetRoot().Name;

                        if (!Patterns.ContainsKey(patternNumber))
                        {
                            List<Pixel> pattern = new List<Pixel>();
                            pattern.Add(new Pixel(new Point(i, j), Color.Black));
                            Patterns.Add(patternNumber, pattern);
                        }
                        else
                        {
                            List<Pixel> pattern = Patterns[patternNumber];
                            pattern.Add(new Pixel(new Point(i, j), Color.Black));
                            Patterns[patternNumber] = pattern;
                        }
                    }
                }
            }

            return Patterns;
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

        #endregion
    }
}