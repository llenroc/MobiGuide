using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector
{
    public class DataRow
    {

        private readonly List<DataColumn> datas;

        public DataRow()
        {
            datas = new List<DataColumn>();
            Error = ERROR.NotSet;
        }

        public DataRow(params object[] parameters)
        {
            datas = new List<DataColumn>();
            Error = ERROR.NotSet;
            if (parameters.Length % 2 == 0)
                for (int i = 0; i < parameters.Length; i += 2)
                    datas.Add(new DataColumn(parameters[i].ToString(), parameters[i + 1]));
        }

        public int Count
        {
            get { return datas.Count; }
        }

        public bool HasData
        {
            get
            {
                return datas.Count > 0 ? true : false;
            }
        }

        public ERROR Error { get; set; }

        public object Get(string key)
        {
            foreach (DataColumn column in datas)
                if (column.Key.Equals(key)) return column.Value;
            return null;
        }

        public void Set(string key, object data)
        {
            datas.Add(new DataColumn(key, data));
        }

        public string GetKeyAt(int index)
        {
            return datas.ElementAt(index).Key;
        }

        public object GetAt(int index)
        {
            return datas.ElementAt(index).Value;
        }

        public DataRow RemoveAt(int index)
        {
            datas.RemoveAt(index);
            return this;
        }

        public bool ContainKey(string key)
        {
            for (int i = 0; i < datas.Count; i++)
                if (datas.ElementAt(i).Key.Equals(key)) return true;
            return false;
        }

        public int IndexOf(string key)
        {
            for (int i = 0; i < datas.Count; i++)
                if (datas.ElementAt(i).Key.Equals(key)) return i;
            return -1;
        }

        public IEnumerator GetEnumerator()
        {
            return (datas as IEnumerable).GetEnumerator();
        }
    }
}
