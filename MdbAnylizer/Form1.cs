using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace MdbAnylizer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            var mdbProvider = new MdbProvider();
            var path = Directory.GetCurrentDirectory() + "\\Database2.mdb";
            mdbProvider.Connect(path);

            var updateQueryLst = mdbProvider.GetActionQueryList();
            var resultSet = QueryAnylizer.Scan(updateQueryLst);
            
        }
    }
}
