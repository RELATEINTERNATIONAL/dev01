using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FCASPLITTER;
using System.Data.SqlClient;

namespace FCASPLITTER
{
    public partial class TestBed : Form
    {

        private FCASPLITTER.Form1 FS = new FCASPLITTER.Form1();

        //public string SERVER = "  ONTRACK\\ONTRACK";
        //public string DATABASE = "OnsiteBiatss_KK";
        //public string USER = "sa";
        //public string PASSWORD = "1234";


        public string SERVER = "RELATE-INTERNAT";
        public string DATABASE = "OnsiteBiatss_KK";
        public string USER = "sa";
        public string PASSWORD = "1234";

        //public string SERVER = "RELATE-INTERNAT";
        //public string DATABASE = "TESTORIGINALTKT";
        //public string USER = "sa";
        //public string PASSWORD = "1234";



        public TestBed()
        {
            InitializeComponent();
            FS.pbConnString = ConnectionString();
            FS.LoadReferences();
        }


        string ConnectionString()
        {

            string ConnString = "Data Source=" + SERVER + " ;Initial Catalog=" + DATABASE + ";User ID=" + USER + ";Password=" + PASSWORD + ";Connect Timeout = 0";
            return ConnString;
        }



        private void getFCA()
        {
           

            //String Sql = "SELECT distinct [DocumentNumber]+'|'+[FareCalculationArea]+'|'+transactioncode+'|'+ cast(DateofIssue as varchar) as Documents " +
            //"FROM [Pax].[SalesDocumentHeader]  where  transactioncode like '%TKT%' order by DateofIssue desc";



            String Sql = "SELECT   [DocumentNumber],[FareCalculationArea] as [Fare Calculation Area] , OriginalIssueDocumentNumber " +
            "FROM [Pax].[SalesDocumentHeader]  where  transactioncode like '%TKT%'  and OriginalIssueDocumentNumber   is not NULL";
            // "AND [FareCalculationArea]  like '%L %'" +
            // "order by FareCalculationArea asc";


            //using (SqlConnection connection = new SqlConnection(ConnectionString()))
            //{
            //    connection.Open();

            //    using (SqlCommand command = connection.CreateCommand())
            //    {

            //        command.CommandText = @Sql;

            //        using (SqlDataReader reader = command.ExecuteReader())
            //        {

            //            while (reader.Read())
            //            {
            //                string FCA = reader.GetString(0);
            //                listBox1.Items.Add(FCA);
            //                //ArrTkt.Add(reader.GetString(0));


            //            }

            //        }
            //        connection.Close();
            //    }


            SqlConnection con = new SqlConnection(ConnectionString());

            con.Open();

            SqlCommand com = new SqlCommand(Sql, con);

            SqlDataReader read = com.ExecuteReader();

            DataSet ds = new DataSet();

            DataTable dt = new DataTable("Table1");

            ds.Clear();
            ds.Reset();
            dt.Clear();
            dt.Reset();

            ds.Tables.Add(dt);

            ds.Load(read, LoadOption.PreserveChanges, ds.Tables[0]);

            dgvTicket.DataSource = ds.Tables[0];

            con.Close();
            


    
        }

      
        private void TestBed_Load(object sender, EventArgs e)
        {
           getFCA();
        }

        private void dgvTicket_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try 
            {
                var DocNo= dgvTicket[0,e.RowIndex].Value;
                var FCA= dgvTicket[1,e.RowIndex].Value;
                FS.pbConnString = ConnectionString();
                FS.LoadFCA(DocNo.ToString(), FCA.ToString());
                FS.Show();
            }
            catch 
            {
            
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FS.pbConnString = ConnectionString();
            //++ FS.LoadFCA(DocNo.ToString(), FCA.ToString());

            FS.LoadFCA("j", "");
            FS.Show();
        }
    }
}
