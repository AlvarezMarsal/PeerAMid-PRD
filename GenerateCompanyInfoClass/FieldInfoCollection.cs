using System;
using System.Collections;
using System.Collections.Generic;

namespace GenerateCompanyInfoClass
{
    internal class FieldInfoCollection : IEnumerable<FieldInfo>
    {
        private List<FieldInfo> list = new List<FieldInfo>(200); // 0-based
        private SortedList<string, FieldInfo> byName = new SortedList<string, FieldInfo>();
        private SortedList<string, FieldInfo> byColumnName = new SortedList<string, FieldInfo>();

        public FieldInfo Add(FieldInfo item)
        {
            try
            {
                if (item.ColumnNumber != (list.Count + 1))
                    throw new Exception();
                list.Add(item);
                byName.Add(item.Name, item);
                byColumnName.Add(item.ColumnName, item);
            }
            catch
            {
                throw new Exception();
            }
            return item;
        }

        public int Count => list.Count;

        public FieldInfo ByIndex(int index)
        {
            return list[index];
        }

        public FieldInfo ByName(string name)
        {
            return byName[name];
        }

        public bool TryToGetByName(string name, out FieldInfo info)
        {
            return byName.TryGetValue(name, out info);
        }

        public FieldInfo ByColumnName(string columName)
        {
            return byColumnName[columName];
        }

        public FieldInfo ByColumnNumber(int columnNumber)
        {
            var i = list[columnNumber - 1];
            if (i.ColumnNumber != columnNumber)
                throw new Exception();
            return i;
        }

        //public FieldInfo this[int columNumber] => ByColumnNumber(columNumber);

        //public FieldInfo this[string nameOrColumnName]
        //{
        //    get
        //     {
        //        if (byName.TryGetValue(nameOrColumnName, out FieldInfo columnInfo))
        //           return columnInfo;
        //       if (byColumnName.TryGetValue(nameOrColumnName, out columnInfo))
        //             return columnInfo;
        //          throw new Exception();
        //            }
        //        }

        public IEnumerator<FieldInfo> GetEnumerator()
        {
            return ((IEnumerable<FieldInfo>)list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)list).GetEnumerator();
        }

        public IEnumerable<FieldInfo> GetGeneration(int g)
        {
            foreach (var item in list)
                if (item.Generation == g)
                    yield return item;
        }

        public IEnumerable<FieldInfo> EnumerateByColumnName()
        {
            foreach (var item in byColumnName.Values)
                yield return item;
        }

        public IEnumerable<FieldInfo> EnumerateByName()
        {
            foreach (var item in byName.Values)
                yield return item;
        }

        public void ItemRenamed(FieldInfo item, string oldName, string newName)
        {
            byName.Remove(oldName);
            byName.Add(newName, item);
        }
    }
}