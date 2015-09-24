using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Management;


namespace PBSort
{
    
    
    class PBSort
    {

        static List<PBSortRecord> _InputLineList = new List<PBSortRecord>();

        public static PBSortRecord[] InputbufferArray =null;

        public static SortDef sortDef = null;

        //public static UInt64 SuggestedMemoryCap = 2500000000;// = 2.5GB
        //public static UInt64 SuggestedMemoryCap = 1300000000;// = 1.3GB
        //public static  UInt64 SuggestedMemoryCap =   100000000;// = 100.MB

        //public static UInt64 SuggestedMemoryCap = 0;// turn off let system decide


        public static UInt64 FreePhysicalMemory = 0;
        public static UInt64 TotalVisibleMemorySize = 0;
        public static UInt64 TotalVirtualMemorySize = 0;
        public static UInt64 MaxProcessMemorySize = 0;

        public static UInt64 RealMemorycap = 0;
        
        
        //public static int _RecordLength = 298;
        //public static bool blnCRLFTerminated = true;
        public static int Bufferlength
        {
            get
            {
                if (SortDef.RecordTermination.CRLF == sortDef.Termination)
                    return sortDef.RecordLength + 2;
                else
                {
                    //return sortDef.RecordLength;
                    throw new NotImplementedException("RecordTermination other than CRLF is not yet supported");
                }
            }
        }


        static List<SortCriteria> _sortCriterias = new List<SortCriteria>();

        static void DoSort(string InputFileFullName, string OutputFileFullName)
        {

           //Perform presort tests

            FileInfo inputFileInfo = new FileInfo(InputFileFullName);
            FileInfo outputFileInfo = new FileInfo(OutputFileFullName);

            DateTime SortStartTime = DateTime.Now;
            DateTime StopWatch;


            if (false == inputFileInfo.Exists)
                throw new ApplicationException("Input file does not exist");


            
            if (false == outputFileInfo.Directory.Exists)
                throw new ApplicationException("Output directory does not exist");

            if (outputFileInfo.Exists)
                File.Delete(outputFileInfo.FullName);


            if (inputFileInfo.Length % Bufferlength != 0)
                throw new ApplicationException("Not all records in input file are of the same length");

            

            long lInputBytesRead = 0;
            long lInputRecordsRead = 0;
            

            List<FileInfo> temporaryFiles = new List<FileInfo>();

            //Find the maxnumber of records we can initially read into memory without self destructing
            long HowManyRecordsToReadAtOnce;

            //if (inputFileInfo.Length < MemoryCap)
            //{
            //    HowManyRecordsToReadAtOnce = inputFileInfo.Length / Bufferlength;
            //}
            //else
            //{
            //    HowManyRecordsToReadAtOnce = MemoryCap / Bufferlength;
            //}

            foreach (SortCriteria sc in _sortCriterias)
            {
                if ((sc.Start + sc.Length) > sortDef.RecordLength)
                    throw new ApplicationException("A sort criteria's start plus length exceeds record length " + sc.Start + ":" + sc.Length);
            }

            //Set to a dummy value to avoid null pointer later in code 
            InputbufferArray = new PBSortRecord[1];


            //Console.WriteLine(DateTime.Now + ":Allocating memory...");
            //StopWatch = DateTime.Now;
            //InputbufferArray = new PBSortRecord[HowManyRecordsToReadAtOnce];

            //for (int i = 0; i < InputbufferArray.Length;i++)
            //{
            //    InputbufferArray[i] = new PBSortRecord(Bufferlength, _sortCriterias);
            //}

            //Console.WriteLine(DateTime.Now + ":Done. Took:" + (DateTime.Now - StopWatch));
            
            
            FileStream InputStream = null;


            //TO DO check file size.

            InputStream = new FileStream(inputFileInfo.FullName, FileMode.Open, FileAccess.Read);
            try
            {

                do
                {//Read->Sort->Write TempFile

                    //Read into memory
                    #region Read Input



                    //We need to calculate this value again just in case we are at the end of our file
                    //So we calculate it again based on lInputBytesRead
                    //At the end of the we do not need as many records
                    //Our buffer array must match the number of records we read in for sorting to work

                    if ((inputFileInfo.Length - lInputBytesRead) < (long)RealMemorycap)
                    {
                        HowManyRecordsToReadAtOnce = (inputFileInfo.Length - lInputBytesRead) / Bufferlength;
                    }
                    else
                    {
                        HowManyRecordsToReadAtOnce = (long)RealMemorycap / Bufferlength;
                    }


                    if (HowManyRecordsToReadAtOnce != InputbufferArray.Length)
                    {
                       //Adjust the buffer accordingly
                        Console.WriteLine(DateTime.Now + ":Allocating memory. Memory caps is " + RealMemorycap + " bytes...");

                        StopWatch = DateTime.Now;

                        Console.WriteLine(DateTime.Now + ":Creating " + HowManyRecordsToReadAtOnce + " sort objects...");
                        InputbufferArray = new PBSortRecord[HowManyRecordsToReadAtOnce];

                        for (int i = 0; i < InputbufferArray.Length; i++)
                        {
                            InputbufferArray[i] = new PBSortRecord(Bufferlength, _sortCriterias);
                        }

                        Console.WriteLine(DateTime.Now + ":Done. Took:" + (DateTime.Now - StopWatch));
                    }




                    Console.WriteLine(DateTime.Now + ":Reading input...");
                    StopWatch = DateTime.Now;
                    int nBytesRead = 0;


                    try
                    {

                        for (int i = 0; i < InputbufferArray.Length; i++)
                        {
                            //InputbufferArray[i] = new PBSortRecord(Bufferlength, _sortCriterias);

                            nBytesRead = InputStream.Read(InputbufferArray[i].RecordBuffer, 0, InputbufferArray[i].RecordBuffer.Length);

                            if (nBytesRead != InputbufferArray[i].RecordBuffer.Length)
                                throw new ApplicationException("Could not read full record");

                            lInputBytesRead += nBytesRead;

                            lInputRecordsRead++;

                        }


                        Console.WriteLine(DateTime.Now + ":Done. Read " + lInputBytesRead + " Bytes Took:" + (DateTime.Now - StopWatch));



                        #region TextRead
                        //int lineCounter = 0;
                        //string line;


                        //InputFileStream = new StreamReader(inputFileInfo.FullName);

                        //_InputLineList = new List<PBSortRecord>();



                        //while (((line = InputFileStream.ReadLine()) != null) && (lineCounter < 100))
                        ////while ((line = InputFileStream.ReadLine()) != null)
                        //{

                        //   _InputLineList.Add(new PBSortRecord(line, _sortCriterias));
                        //    lineCounter++;
                        //}
                        //Console.WriteLine(" Done." + lineCounter + " lines." + " Timecheck:" + (DateTime.Now - SortStartTime));

                        #endregion


                    }
                    finally
                    {

                    }

                    #endregion


                    #region Sort Input

                    //Sort and Write into temp file.

                    Console.WriteLine(DateTime.Now + ":Sorting...");
                    StopWatch = DateTime.Now;
                    Array.Sort(InputbufferArray);
                    Console.WriteLine(DateTime.Now + ":Sorting pass done" + " Took:" + (DateTime.Now - StopWatch));

                    #endregion


                    #region Write to temporary file

                    TextWriter OutputFileStream = null;
                    FileStream OutputBinaryStream = null;


                    try
                    {
                        Console.WriteLine(DateTime.Now + ":Writing temporary output file...");
                        StopWatch = DateTime.Now;

                        #region Binary Writer
                        //OutputBinaryStream = new FileStream(outputFileInfo.FullName, FileMode.Create, FileAccess.Write);
                        string tempFileName = outputFileInfo.DirectoryName + "\\Sort." + temporaryFiles.Count.ToString() + ".tmp";

                        temporaryFiles.Add(new FileInfo(tempFileName));


                        OutputBinaryStream = new FileStream(tempFileName, FileMode.Create, FileAccess.Write);
                        for (int i = 0; i < InputbufferArray.Length; i++)
                        {
                            OutputBinaryStream.Write(InputbufferArray[i].RecordBuffer, 0, InputbufferArray[i].RecordBuffer.Length);
                        }



                        #region TextWriter
                        //OutputFileStream = new FileStream(outputFileInfo.FullName, FileMode.Create, FileAccess.Write);
                        //OutputFileStream = new StreamWriter(outputFileInfo.FullName);

                        //foreach (PBSortRecord line in _InputLineList)
                        //{
                        //    OutputFileStream.WriteLine(line.Value);
                        //    //byte[] bytes = ASCIIEncoder.GetBytes(line + "\r\n");
                        //    //OutputFileStream.Write(bytes, 0, bytes.Length);
                        //}

                        #endregion


                        Console.WriteLine(DateTime.Now + ":Done." + " Took:" + (DateTime.Now - StopWatch));



                    }

                    finally
                    {
                        if (null != OutputFileStream) { OutputFileStream.Close(); OutputFileStream.Dispose(); }
                        if (null != OutputBinaryStream) { OutputBinaryStream.Close(); OutputBinaryStream.Dispose(); }
                    }

                        #endregion


                    #endregion



                }
                while (lInputBytesRead < inputFileInfo.Length);
            }
            finally
            {
                if (null != InputStream) { InputStream.Close(); InputStream.Dispose(); }
            }



            //Free now for performance;make it free by setting it to a very small array
            InputbufferArray = new PBSortRecord[32];


            StopWatch = DateTime.Now;
            Console.WriteLine(StopWatch + ":Merging " + temporaryFiles.Count + " temporary output file(s)...");

            

            if (temporaryFiles.Count > 1)
            {
                MergeOutput(temporaryFiles, outputFileInfo, sortDef.RecordLength + 2);
            }
            else if (temporaryFiles.Count == 1)
            {
                File.Move(temporaryFiles[0].FullName, outputFileInfo.FullName);
            }
            else
            {
                throw new ApplicationException("Unexpected number of temporary files:" + temporaryFiles.Count);
            }

            

            //outputFileInfo



            Console.WriteLine(DateTime.Now + ":Done." + " Took:" + (DateTime.Now - StopWatch));


            Console.WriteLine("Sorting operation complete. Total Processing time:" + (DateTime.Now - SortStartTime));

        }

        static void MergeOutput(List<FileInfo> tempfiles, FileInfo OuputFile, int recordLength)
        {
            TempFileMerger fileMerger = null;
            try
            {
                fileMerger = new TempFileMerger(tempfiles.ToArray(), OuputFile, recordLength, _sortCriterias);
                fileMerger.DoMerge();

            }
            finally
            {
                if (null != fileMerger) { fileMerger.Dispose(); }
            }
        }

              
        static int Main(string[] args)
        {


            if (args.Length != 1)
            {
                Console.WriteLine("usage:PBSort.exe <SORTDEF XML File>");
                return -1;
            }

            try
            { sortDef = XMLUtil.Deserialize<SortDef>(File.ReadAllText(args[0])); }
            catch (Exception exp)
            {
                Console.WriteLine("Unable to read sort definition file:" + args[0] + " : " + exp.Message);
                return -1;
            }


            #region Calculate system memory cap
            ObjectQuery winQuery = new ObjectQuery("SELECT * FROM Win32_OperatingSystem");
            ManagementObjectSearcher searcher = new ManagementObjectSearcher( winQuery);
            ManagementObjectCollection collection = searcher.Get();

            if (collection.Count == 1)
                foreach (ManagementObject item in collection)
                {
                    FreePhysicalMemory = (UInt64)item["FreePhysicalMemory"] * 1024;
                    TotalVisibleMemorySize = (UInt64)item["TotalVisibleMemorySize"] * 1024;
                    TotalVirtualMemorySize = (UInt64)item["TotalVirtualMemorySize"] * 1024;
                    MaxProcessMemorySize = (UInt64)item["MaxProcessMemorySize"] * 1024;

                    RealMemorycap = TotalVirtualMemorySize;

                    if (MaxProcessMemorySize < RealMemorycap)
                        RealMemorycap = MaxProcessMemorySize;

                    if (TotalVisibleMemorySize < RealMemorycap)
                        RealMemorycap = TotalVisibleMemorySize;


                }
            else
            {
                throw new ApplicationException("Cannot read system memory");
            }

            RealMemorycap = (UInt64)((float)RealMemorycap * (.5));

            #endregion

            
            


            if (sortDef.MemoryCap != 0)
            {
                if (sortDef.MemoryCap < RealMemorycap)
                {

                    RealMemorycap = sortDef.MemoryCap;
                }
                else
                {

                    Console.WriteLine("Warning: Memory cap defined in XML is too large for this system. Will use a memory cap of:" + RealMemorycap + " bytes instead.");
                }
            }
           

            try
            {

                string[] sortCriterias = sortDef.SortDefinition.Split(',');


                foreach (string s in sortCriterias)
                {
                    string[] s2 = s.Split(':');

                    if (s2.Length != 2)
                        throw new ApplicationException("Invaild Sort argument:" + s);

                    int start, length;

                    if (false == Int32.TryParse(s2[0], out start))
                        throw new ApplicationException("Invaild Sort argument:" + s);

                    if (start < 0)
                        throw new ApplicationException("Sort start cannot be negative:" + start);

                    if (false == Int32.TryParse(s2[1], out length))
                        throw new ApplicationException("Invaild Sort argument:" + s);

                    if (length < 1)
                        throw new ApplicationException("Sort start cannot be less than one:" + length);

                    _sortCriterias.Add(new SortCriteria(start, length));

                }




                //Sort()
                DoSort(sortDef.InputFileName, sortDef.OutputFileName);

                
            }
            catch (ApplicationException ae)
            {
                Console.WriteLine(ae.Message);
                return -1;
            }
            //catch (Exception exp)
            //{
            //    Console.WriteLine(exp.Message);
            //    Console.WriteLine(exp.StackTrace);
            //    return -1;
            //}
            
            


            return 0;

        }
    }

    





}
