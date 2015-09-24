using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PBSort
{
    public class PBSortRevolver : Sorter, IDisposable 
    {
        public FileInfo _fileinfo;
        

        public FileStream _fileStream = null;

        public byte[] ChamberBuffer
        {
            get { return Buffer; }
        }



        public PBSortRevolver(FileInfo tempinfo, int RecordLength, List<SortCriteria> sortCriterias)
            : base(RecordLength,sortCriterias)
        {
            _fileinfo = tempinfo;
        }


        public int CockFirstRecord()
        {

            if (null != _fileStream)
            {
                _fileStream.Close();
                _fileStream.Dispose();
                _fileStream = null;
            }
            _fileStream = new FileStream(_fileinfo.FullName, FileMode.Open, FileAccess.Read);

            

            return CockNextRecord();
        }

        
        public int CockNextRecord()
        {
            //int bytesRead;

            if (null == _fileStream)
                return 0;

            return _fileStream.Read(Buffer, 0, Buffer.Length);


            //if (bytesRead == Buffer.Length)
            //{
            //    IsEmpty = false;
            //}
            //else if (bytesRead == 0)
            //{
            //    IsEmpty = true;
            //}
            //else
            //{
            //    IsEmpty = false;
            //    throw new ApplicationException("Could not read full record from:" + _fileinfo.FullName);
            //}

            //return bytesRead;
        }



        public void Dispose()
        {
            if (null != _fileStream) 
            { 
                
                _fileStream.Close(); 
                _fileStream.Dispose();
            }

            if (null != _fileinfo)
            {
                _fileinfo.Delete();
            }
        }


        
    }
}
