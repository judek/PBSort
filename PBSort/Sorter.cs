using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PBSort
{
    public class Sorter : IComparable<Sorter>
    {

        List<SortCriteria> _sortCriterias;

        byte[] _Buffer = null;
        protected byte[] Buffer
        {
            get { return _Buffer; }
        }

        public Sorter(int length, List<SortCriteria> sortCriterias)
        {
            _Buffer = new byte[length];
            _sortCriterias = sortCriterias;
        }
        

        virtual public int CompareTo(Sorter other)
        {

            //Important be very careful in this method as this is call the most called out of a sort process.
            //EVERY INSTRUCTION you add to this method will slow down the sort considerably.


            //keep these variable on stack for fastest access
            int nCompareValue = 0;
            int iStart;
            int iStop;
            int CriteriaCount = _sortCriterias.Count;

            for (int i = 0; i < CriteriaCount; i++)
            {
                iStart = _sortCriterias[i].Start - 1;
                iStop = iStart + _sortCriterias[i].Length;


                for (int j = iStart; j < iStop; j++)
                {
                    nCompareValue = this.Buffer[j] - other.Buffer[j];

                    if (nCompareValue != 0)
                        return nCompareValue;//We are done comparing, get out quick
                }


                if (nCompareValue != 0)
                    return nCompareValue;//We are done comparing, get out quick

            }

            return 0;//We have a perfect match, should not happen too much
        }

    }
}
