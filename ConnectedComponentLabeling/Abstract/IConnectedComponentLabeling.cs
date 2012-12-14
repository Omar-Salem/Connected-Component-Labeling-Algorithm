using System.Collections.Generic;
using System.Drawing;

namespace ConnectedComponentLabeling
{
    public interface IConnectedComponentLabeling
    {
        IDictionary<int, Bitmap> Process(Bitmap input);
    }
}