using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormationsTool
{
    public class PositionChangedEventArgs
    {
        public int Index { get; set; }
        public FVector Position { get; set; }

        public PositionChangedEventArgs(int index, FVector position)
        {
            Index = index;
            Position = position;
        }
    }
}
