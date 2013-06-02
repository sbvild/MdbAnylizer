using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MdbAnylizer
{
    class QueryAnylizer
    {
        public static IQueryAresult[] Scan(IList<Query> queries)
        {
            //
            var lst = new List<IQueryAresult>();
            foreach (var q in queries)
            {
                lst.Add(Scan(q.Sql, q.Name));
            }
            return lst.ToArray();
        }
        public static IQueryAresult[] Scan(IEnumerable<string> queries)
        {
            //
            var lst = new List<IQueryAresult>();
            foreach (var q in queries)
            {
                lst.Add(Scan(q));
            }
            return lst.ToArray();
        }
        public static IQueryAresult Scan(string query, string name = "")
        {
            //
            var q = query.ToUpper();
            if (q.StartsWith("UPDATE"))
                return _GetUpdateAr(q, name);

            if (q.StartsWith("INSERT"))
                return null;

            return null;
        }
        private static IQueryAresult _GetUpdateAr(string q, string name = "")
        {
            //
            //    var rs = @"update\s+(?<table_name>.*?)(\s+SET\s+(?<col_name>.*?)\s+=\s+(?<value>.*?)\s*(;|,))+";
            //    var reg = new Regex(rs, RegexOptions.IgnoreCase);
            //    var m = reg.Match(q);

            //    if(!m.Success)
            //         throw new ArgumentException();

            //    var qr = new UpdateQueryAresult();
            //    for (var i = 0;i < m.Groups["col_name"].Captures.Count; i++)
            //    {
            //        //
            //        qr.AddColumnValue(m.Groups["col_name"].Captures[i].Value, m.Groups["value"].Captures[i].Value);
            //    }

            //    return qr;
            //}
            var updateQuery = new UpdateQueryAresult(name, q);
            return updateQuery;
        }
    }

    public interface IQueryAresult
    {
        //
        string Name
        {
            get;
        }
        string Sql
        {
            get;
        }
        IList<string> ColumnLst
        {
            get;
        }
    }

    public class UpdateQueryAresult : IQueryAresult

    {
        //
        private readonly string name;
        private readonly string sql;
        private Dictionary<string, string> m_columnValueLst = new Dictionary<string, string>();


        public string Name
        {
            get { return name; }
        }
        public string Sql
        {
            get { return sql; }
        }

        public UpdateQueryAresult(string name, string sql)
        {
            //

            this.name = name;
            this.sql = sql;       

            var rs = @"update\s+(?<table_name>.*?)(\s+SET\s+(?<col_name>.*?)\s+=\s+(?<value>.*?)\s*(;|,))+";
            var reg = new Regex(rs, RegexOptions.IgnoreCase);
            var m = reg.Match(sql);

            if (!m.Success)
                throw new ArgumentException();

            for (var i = 0; i < m.Groups["col_name"].Captures.Count; i++)
            {
                //
                this.AddColumnValue(m.Groups["col_name"].Captures[i].Value, m.Groups["value"].Captures[i].Value);
            }


        }

        public IList<string> ColumnLst
        {
            get {
                var lst = new List<string>();
                lst.AddRange(m_columnValueLst.Keys);
                return lst;
            }
        }

        private void AddColumnValue(string col, string value)
        {
            //
            if (m_columnValueLst.ContainsKey(col))
                return;

            m_columnValueLst.Add(col, value);
        }
    }
}
