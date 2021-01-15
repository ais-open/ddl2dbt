using System;
using System.Collections.Generic;
using Microsoft.VisualBasic.CompilerServices;

namespace DDLParser
{
    public class CreateTable
    {
        public string TableName;
        public List<ColumnDetail> Columns;
        public string PolicyHk;
        public string PolicyNumber;
        public string LoadTimestamp;
        public string RecordSource;
    }

    public class ColumnDetail
    {
        public string Name;
        public string DataType;

    }
}