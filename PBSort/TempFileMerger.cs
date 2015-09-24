using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PBSort
{
   
    
    public class TempFileMerger : IDisposable
    {
        List<PBSortRevolver> _revolvers = new List<PBSortRevolver>();
        FileInfo _ouputFileInfo;

        


        public TempFileMerger(FileInfo[] tempFiles, FileInfo ouputFileInfo, int RecordLength, List<SortCriteria> sortCriterias)
        {

            _ouputFileInfo = ouputFileInfo;
            
            foreach (FileInfo tempfile in tempFiles)
            {
                _revolvers.Add(new PBSortRevolver(tempfile, RecordLength, sortCriterias));
            }
        }


        public void DoMerge()
        {

            if (_revolvers.Count < 1)
                return;

            FileStream outputBinaryStream = null;


            try
            {
                foreach (PBSortRevolver revolver in _revolvers)
                {
                    revolver.CockFirstRecord();
                }
                
                outputBinaryStream = new FileStream(_ouputFileInfo.FullName, FileMode.Create, FileAccess.Write);


                do
                {
                    //Sort
                    _revolvers.Sort();

                    //Write First
                    outputBinaryStream.Write(_revolvers[0].ChamberBuffer, 0, _revolvers[0].ChamberBuffer.Length);

                    //CockFirst
                    int test = _revolvers[0].CockNextRecord();

                    //If revolver empty remove it from merge collection
                    if (test == 0)
                    {
                        _revolvers[0].Dispose();//Calling dispose will delete temporary file
                        _revolvers.RemoveAt(0);
                    }

                }
                //while (false == allRevolversEmpty);
                while (_revolvers.Count > 0);


                
            }
            finally
            {
                if (null != outputBinaryStream) { outputBinaryStream.Close(); outputBinaryStream.Dispose(); }
            }




        }
        
        
        public void WriteRecord(byte[] dataBuffer)
        {
            
        }
        
        
        public void Dispose()
        {
            if(null != _revolvers)
                foreach (PBSortRevolver temp in _revolvers)
                {
                    temp.Dispose();
                }
            
        }

        
    }

    
}
