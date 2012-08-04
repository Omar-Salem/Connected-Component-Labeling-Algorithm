using System.Collections.Generic;
using ConnectedComponentLabeling.Contracts;
using System.Drawing;
using System.ComponentModel.Composition;
using Entities;

namespace ConnectedComponentLabeling.Implementation
{
    [Export(typeof(IConnectedComponentLabeling))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CCL : IConnectedComponentLabeling
    {
        #region Member Variables

        private int[,] _board;
        private int _width;
        private int _height;
        private Bitmap input;

        #endregion

        #region IConnectedComponentLabeling

        public IList<Bitmap> Process(Bitmap input)
        {
            this.input = input;
            _width = input.Width;
            _height = input.Height;
            _board = new int[_width, _height];


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

        #endregion

        #region Protected Functions

        protected virtual bool IsForeGround(Pixel currentPixel)
        {
            return currentPixel.color.A == 255 && currentPixel.color.R == 0 && currentPixel.color.G == 0 && currentPixel.color.B == 0;
        }

        #endregion

        #region Private Functions

        private Dictionary<int, List<Pixel>> Find()
        {
            Pixel currentPixel;
            Label currentLabel = new Label(0);
            int labelCount = 0;
            Dictionary<int, int> neighboringLabels;
            Dictionary<int, Label> allLabels = new Dictionary<int, Label>();

            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    currentPixel = new Pixel(new Point(j, i), input.GetPixel(j, i));

                    if (IsForeGround(currentPixel))
                    {
                        neighboringLabels = GetNeighboringLabels(currentPixel);

                        if (neighboringLabels.Count == 0)
                        {
                            currentLabel.Name = ++labelCount;
                            allLabels.Add(currentLabel.Name, new Label(currentLabel.Name));
                        }
                        else
                        {
                            foreach (int label in neighboringLabels.Keys)
                            {
                                currentLabel.Name = label;//set currentLabel to the first label found in neighboring cells
                                break;
                            }

                            MergeLabels(currentLabel.Name, neighboringLabels, allLabels);
                        }

                        _board[j, i] = currentLabel.Name;
                    }
                }
            }


            Dictionary<int, List<Pixel>> Patterns = AggregatePatterns(allLabels);

            return Patterns;
        }

        private Dictionary<int, int> GetNeighboringLabels(Pixel pix)
        {
            Dictionary<int, int> neighboringLabels = new Dictionary<int, int>();
            int x = pix.Position.Y;
            int y = pix.Position.X;

            for (int i = x - 1; i < _height; i++)
            {
                if (CheckWithinBoundaries(i, x + 2))
                {
                    for (int j = y - 1; j < _width; j++)
                    {
                        if (CheckWithinBoundaries(j, y + 2))
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
                }
            }

            return neighboringLabels;
        }

        private bool CheckWithinBoundaries(int j, int y)
        {
            return j > -1 && j < y;
        }

        private void MergeLabels(int currentLabel, Dictionary<int, int> neighboringLabels, Dictionary<int, Label> labels)
        {
            Label root = labels[currentLabel].GetRoot();
            Label neighbor;

            foreach (int key in neighboringLabels.Keys)
            {
                if (key != currentLabel)
                {
                    neighbor = labels[key];

                    if (neighbor.GetRoot() != root)
                    {
                        neighbor.Root = root;
                    }
                }
            }
        }

        private Dictionary<int, List<Pixel>> AggregatePatterns(Dictionary<int, Label> allLabels)
        {
            int patternNumber;
            List<Pixel> shape;
            Dictionary<int, List<Pixel>> Patterns = new Dictionary<int, List<Pixel>>();

            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    patternNumber = _board[i, j];

                    if (patternNumber != 0)
                    {
                        patternNumber = allLabels[patternNumber].GetRoot().Name;

                        if (!Patterns.ContainsKey(patternNumber))
                        {
                            shape = new List<Pixel>();
                            shape.Add(new Pixel(new Point(i, j), Color.Black));
                            Patterns.Add(patternNumber, shape);
                        }
                        else
                        {
                            shape = Patterns[patternNumber];
                            shape.Add(new Pixel(new Point(i, j), Color.Black));
                            Patterns[patternNumber] = shape;
                        }
                    }
                }
            }

            return Patterns;
        }

        private int GetDimension(List<Pixel> shape, out int dimensionShift, bool isWidth)
        {
            int dimension;
            int result = dimensionShift = CheckDimensionType(shape[0], isWidth);

            for (int i = 1; i < shape.Count; i++)
            {
                dimension = CheckDimensionType(shape[i], isWidth);

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