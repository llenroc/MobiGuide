using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector
{
    public class DataList
    {

        private readonly List<DataRow> datas;

        public DataList()
        {
            datas = new List<DataRow>();
            Error = ERROR.NotSet;
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

        public DataRow GetListAt(int index)
        {
            return datas[index];
        }

        public void Add(DataRow data)
        {
            datas.Add(data);
        }

        public IEnumerator GetEnumerator()
        {
            return (datas as IEnumerable).GetEnumerator();
        }
    }
}
