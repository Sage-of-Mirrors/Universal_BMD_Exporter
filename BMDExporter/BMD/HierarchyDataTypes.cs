using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMDExporter.BMD
{
    enum HierarchyDataTypes : ushort
    {
        Finish = 0x0, //Terminator
        NewNode = 0x01, // Hierarchy down (insert node), new child
        EndNode = 0x02, // Hierarchy up, close child
        Joint = 0x10,
        Material = 0x11,
        Shape = 0x12, // Batch
    }
}
