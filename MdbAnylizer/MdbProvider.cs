using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.OleDb;
using ADODB;
using ADOX;

namespace MdbAnylizer
{
    class MdbProvider
    {
        private ADODB.Connection con;
        private List<Query> queryLst = null;
        //private List<string> insertSqlLst = null;

        public MdbProvider()
        {
            //
        }

        public void Connect(string path)
        {
            //
            if (con != null)
                return;

            con = new ADODB.Connection();
            con.Open("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";", "", "", 0);
        }

        public void DisConnect()
        {
            //
            if (con == null)
                return;

            con.Close();
            con = null;
        }

        private void _InitSqlLst()
        {
            //
            if (queryLst != null)
                return;

            if (con == null)
                throw new InvalidOperationException();

            var querylst = new List<string>();
            string s;
            var cat = new ADOX.Catalog();
            var cmd = new ADODB.Command();
            queryLst = new List<Query>();


            //アクションクエリ
            var rs = con.OpenSchema(ADODB.SchemaEnum.adSchemaProcedures);
            while (!rs.EOF)
            {
                s = rs.Fields["PROCEDURE_NAME"].Value.ToString();
                rs.MoveNext();
                if (!s.StartsWith("~"))
                    querylst.Add(s);
            }

            cat.ActiveConnection = con;
            foreach (var q in querylst)
            {
                //
                cmd = (ADODB.Command)cat.Procedures[q].Command;
                var sql = cmd.CommandText.ToUpper();
                //var name = cmd.Name;

                var name = q;

                if (!(sql.StartsWith("UPDATE") || sql.StartsWith("INSERT")))
                    return;

                var query = new Query(name, sql);
                queryLst.Add(query);

                //if (sql.StartsWith("UPDATE"))
                //    updateSqlLst.Add(sql);

                //if (sql.StartsWith("INSERT"))
                //    insertSqlLst.Add(sql);
            }

        }

        public IList<Query> GetActionQueryList()
        {
            //
            _InitSqlLst();

            return this.queryLst;
        }
    }

    public class Query
    {
        //
        private string m_name;
        private string m_sql;

        public string Name
        {
            get { return m_name; }
        }
        public string Sql
        {
            get { return m_sql; }
        }
        public QueryType Type
        {
            get
            {
                if (m_sql.ToUpper().StartsWith("UPDATE"))
                    return QueryType.UPDATE;
                else
                    return QueryType.INSERT;
            }
        }
        public Query(string name, string sql)
        {
            m_name = name;
            m_sql = sql;
        }
    }

    public enum QueryType
    {
        UPDATE,
        INSERT
    }
}
