using System.Collections.Generic;
using System.Drawing;

namespace ConnectedComponentLabeling.Contracts
{
    public interface IConnectedComponentLabeling
    {
        IList<Bitmap> Process(Bitmap input);
    }
}