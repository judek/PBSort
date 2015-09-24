using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace PBSort
{
    [Serializable]
    public class SortDef
    {
        //Enums
        public enum Type { CSV, FixedLength }
        public enum RecordTermination { None, CRLF }
        public enum CharacterSets { ASCII, Unicode }

        public string InputFileName = @"C:\Users\judek\Desktop\RunExample\LargeTest1MM.dat.txt";
        public string OutputFileName = @"C:\Users\judek\Desktop\RunExample\LargeTest1MM.dat.Sorted.txt";
        public int RecordLength = 0;
        public SortDef.RecordTermination Termination = SortDef.RecordTermination.CRLF;
        public CharacterSets CharacterSet = CharacterSets.ASCII;
        public Type RecordType = Type.FixedLength;
        public string SortDefinition = "193:8,30:30,125:20";
        public UInt64 MemoryCap = 0;

    }

 
    
}
