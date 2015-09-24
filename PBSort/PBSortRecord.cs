using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PBSort
{

    public class PBSortRecord : Sorter
    {

        public byte[] RecordBuffer  { get { return base.Buffer; } }

        public PBSortRecord(int length, List<SortCriteria> sortCriterias) 
            : base(length, sortCriterias) { }

    }
}
