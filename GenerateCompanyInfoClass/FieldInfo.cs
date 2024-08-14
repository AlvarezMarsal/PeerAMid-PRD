namespace GenerateCompanyInfoClass
{
    internal class FieldInfo //: IComparable<ColumnInfo>
    {
        public readonly FieldInfoCollection Collection;
        public readonly int ColumnNumber;               // starts at 1, like Excel columns
        public string Name { get; private set; }        // From top row in SK's spreadsheet
        public string OriginalFormula;                  // As read from SK's spreadsheet
        public string TranslatedFormula;                // Emitted to generated code
        public readonly string Type;                    // in generated code
        public string Visibility = "public";            // in generated code
        public int Generation;                          // Calculation sequencing
        public string Comment1;
        public readonly string ColumnName;              // 'A' to about 'EB'

        public string DefaultValue
        {
            get
            {
                switch (Type)
                {
                    case "bool": return "false";
                    case "double": return "0";
                    case "double?": return "null";
                    case "int": return "0";
                    default: return "null";
                }
            }
        }

        public FieldInfo(FieldInfoCollection collection, int columnNumber, string name, string formula, string type)
        {
            Collection = collection;
            ColumnNumber = columnNumber;
            Name = name;
            OriginalFormula = formula;
            Type = type;
            ColumnName = Program.ColumnName(columnNumber);
        }

        public override string ToString()
        {
            return ColumnNumber.ToString() + ":" + ColumnName + ":" + Name;
        }

        public void Rename(string newName)
        {
            var oldName = Name;
            Name = newName;
            Collection.ItemRenamed(this, oldName, newName);
        }

        /*
        public int CompareTo(ColumnInfo other)
        {
            var c = this.Name.CompareTo(other.Name);
            if (c == 0)
                c = this.TargetColumn.CompareTo(other.TargetColumn);
            return c;
        }
        */
    }
}