using System.Collections.Generic;
using System.Drawing;

namespace PatternRecognition
{
    public class ConnectedComponentLabeling
    {
        readonly int[,] board;
        readonly int w;
        readonly int h;
        readonly Bitmap input;

        public ConnectedComponentLabeling(Bitmap Input)
        {
            input = Input;
            w = input.Width;
            h = input.Height;
            board = new int[w, h];
        }

        protected Dictionary<int, List<Pixel>> Find()
        {
            Pixel currentPixel;
            Label currentLabel = new Label(0);
            int labelCount = 0;
            Dictionary<int, int> neighboringLabels;
            Dictionary<int, Label> allLabels = new Dictionary<int, Label>();

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
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
                        board[j, i] = currentLabel.Name;
                    }
                }
            }


            Dictionary<int, List<Pixel>> Patterns = AggregatePatterns(allLabels);

            return Patterns;
        }

        protected virtual bool IsForeGround(Pixel currentPixel)
        {
            return currentPixel.color.A == 255 && currentPixel.color.R == 0 && currentPixel.color.G == 0 && currentPixel.color.B == 0;
        }

        Dictionary<int, int> GetNeighboringLabels(Pixel pix)
        {
            Dictionary<int, int> neighboringLabels = new Dictionary<int, int>();
            int x = pix.Position.Y;
            int y = pix.Position.X;
            for (int i = x - 1; i < h; i++)
            {
                if (CheckWithinBoundaries(i, x + 2))
                {
                    for (int j = y - 1; j < w; j++)
                    {
                        if (CheckWithinBoundaries(j, y + 2))
                        {
                            if (board[j, i] != 0)
                            {
                                if (!neighboringLabels.ContainsKey(board[j, i]))
                                {
                                    neighboringLabels.Add(board[j, i], 0);
                                }
                            }
                        }
                    }
                }
            }
            return neighboringLabels;
        }

        bool CheckWithinBoundaries(int j, int y)
        {
            return j > -1 && j < y;
        }

        void MergeLabels(int currentLabel, Dictionary<int, int> neighboringLabels, Dictionary<int, Label> labels)
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

        Dictionary<int, List<Pixel>> AggregatePatterns(Dictionary<int, Label> allLabels)
        {
            int patternNumber;
            List<Pixel> shape;
            Dictionary<int, List<Pixel>> Patterns = new Dictionary<int, List<Pixel>>();
            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    patternNumber = board[i, j];
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
    }
}