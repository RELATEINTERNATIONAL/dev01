using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;

namespace FCASPLITTER
{
    public partial class Form1 : Form
    {
        
        public string pbConnString = "";
        public string pbFca;
        public string PbDocNo;
        ArrayList arrObject = new ArrayList();  
        private string pvCurrAmount = "";

        private ArrayList ArrCarrier = new ArrayList();
        private ArrayList ArrAirports = new ArrayList();
        private ArrayList ArrFareBasis = new ArrayList();

        private ArrayList ArrCPNFareBasis = new ArrayList();

        private ArrayList ArrCity = new ArrayList();
        private ArrayList ArrCurrency = new ArrayList();
        private ArrayList ArrProration = new ArrayList();
        private ArrayList ArrRefences = new ArrayList();

        private ArrayList ArrFCA = new ArrayList();
        private ArrayList ArrTkt = new ArrayList();
        private ArrayList ArrFCBreakDown = new ArrayList();

        string[] FCABKDN;

        private string DocNo="";

        SqlConnection conn = new SqlConnection();

        public Form1()
        {
            InitializeComponent();
        }


        string ConnectionString()        {

            string ConnString = pbConnString;
            // "Data Source=" + SERVER + " ;Initial Catalog=" + DATABASE + ";User ID=" + USER + ";Password=" + PASSWORD + ";Connect Timeout = 0";
            return ConnString;       
        }


#region Main 1

        private void CpnFareBasis()
        {
            ArrCPNFareBasis.Clear();
            String Sql = "Select   distinct SDC.FareBasisTicketDesignator from pax.SalesDocumentHeader SDH " +
        " left join pax.SalesRelatedDocumentInformation SRI on SDH.HdrGuid = SRI.HdrGuid " +
        " left join pax.SalesDocumentCoupon SDC on SRI.RelatedDocumentGuid=SDC.RelatedDocumentGuid " +
        " where SDH.DocumentNumber = '" + PbDocNo + "'";

            GeDatabases(Sql, ArrCPNFareBasis);

           

        }
        public void LoadFCA(string Param1,string Param2)
        {
            PbDocNo = Param1;
            pbFca = Param2;

            arrObject.Clear();

            DocNo = PbDocNo;
            textBox1.Text = pbFca;
            lblDocNo.Text = PbDocNo;

            CpnFareBasis();

            Pattern();
            CheckAirport();
            CheckCarrier();
            CheckfareComponent();
            CheckfareBasis();
            checkEnd();
            CheckTaxes();
            BuildFCA();


            GetIicketInformation();
            GeCouponsInformation();
            GePaymentInformation();
            GetOtherPaymentsInformation();
            GetProrationDetailsInformation();
            GetProrationExceptionInformation();

            GetExchangeInformation(PbDocNo);
            arrObject.Reverse();
            GetExchangeInformation2(PbDocNo);

            GetExchangeInformationRecords(arrObject);
            DgvFareComp();

            CheckPanel();
        }

        private void CheckPanel()
        {

            if (dgvTicketDocNo.Rows.Count > 0) { } else { }

            Button[] btns = new  Button[9];
          
            btns[0]=button13;
            btns[1]=button12;
            btns[2]=button10;
            btns[3]=button9;
            btns[4]=button8;
            btns[5]=button4;
            btns[6]=button7;
            btns[7]=button6;
            btns[8]=button5;
         
           


            DataGridView[] dgvs = new DataGridView[9];
            DataGridView dgvxx01 = new DataGridView();
            dgvxx01.Name = "TEST";
           // /-dgvs[0] =  dgvxx01;
            dgvs[0] =  dgvxx01; 
            dgvs[1] =  dgvExchange;
            dgvs[2] =  dgvInterline;
            dgvs[3] =  dgvProrationExceptions;
            dgvs[4] =  dgvProtionDetails;
            dgvs[5] =  dgvOtherPayments;
            dgvs[6] =  dgvPayments;
            dgvs[7] =  dgvCoupons;
            dgvs[8] =  dgvTicketDocNo; 
            try
            {
                int x = 0;
                foreach (DataGridView dgv in dgvs)
                {
                    Console.WriteLine("=====================================");
                    Console.WriteLine(dgv.Name.ToString());

                    string BTNX = btns[x].Name; ;
                    Control[] Btnx = this.Controls.Find(BTNX, true);
                    Console.WriteLine(Btnx[0].Name.ToString());
                    string pnlx = "pnl" + Btnx[0].Tag.ToString();
                    Control[] Pnl = this.Controls.Find(pnlx, true);
                    Console.WriteLine(Pnl[0].Name.ToString());
                    if (dgv.RowCount == 0)
                    {
                        if (Btnx != null && Btnx.Length > 0)
                        {
                            Btnx[0].Enabled = false;
                            Btnx[0].Visible = false;
                            Btnx[0].BackColor = Color.Red;
                        }


                    }
                    else
                    {
                        if (Btnx != null && Btnx.Length > 0)
                        {
                            Btnx[0].Enabled = true;
                            Btnx[0].Visible = true;
                            Btnx[0].BackColor = Color.PaleGreen;
                        }


                    }

                    if (Pnl != null && Pnl.Length > 0)
                    {
                        if (dgv.RowCount == 0)
                        {
                            Pnl[0].Height =0;
                            Pnl[0].Tag = "0";

                        }
                        else
                        {
                            Pnl[0].Height = dgv.Height+10;// dgv.RowCount * 53;
                            Pnl[0].Height = (dgv.RowCount * 50) + dgv.ColumnHeadersHeight + 10;
                            
                            Pnl[0].Tag = "1";

                        }

                        Pnl[0].Invalidate();
                        this.Validate();

                    }




                    x++;
                }

            }
            catch
            {
                int v = 0;
                v = 999;
            
            }
        }
        private void GetExchangeInformationRecords(ArrayList arrObject)        
         {

             string DocNo = "";
            for(int i =0;i<arrObject.Count;i++)
            {

                string[] Dno = arrObject[i].ToString().Split('|');
                DocNo = DocNo + "'" + Dno[0] + "'";
                if (i < arrObject.Count - 1) { DocNo = DocNo + " ,"; } else { }

            }

            String Sql = "SELECT  * " +
            "FROM [Pax].[SalesDocumentHeader]  where " +
            " DocumentNumber in (" + DocNo+")";
            GetData(Sql, dgvExchange);
        
         }

        public void LoadReferences()
        {
            GeDatabases("SELECT [AirlineCode] FROM [Ref].[Airlines]", ArrCarrier);
            GeDatabases("SELECT  [AirportCode]  FROM [Ref].[City]", ArrAirports);
            GeDatabases("SELECT DISTINCT [FareBasisTicketDesignator] FROM pax.[SalesDocumentCoupon] where [FareBasisTicketDesignator] is not null  order by  [FareBasisTicketDesignator]", ArrFareBasis);
            GeDatabases("SELECT [CurrISOCode] FROM [Ref].[Currency]  order by  [CurrISOCode]", ArrCurrency);
           // getFCA();
      
        }

        
        private void GeDatabases(string Sql, ArrayList arrObject)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString()))
            {
                connection.Open();

                using (SqlCommand command = connection.CreateCommand())
                {

                    command.CommandText = @Sql; 

                    using (SqlDataReader reader = command.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            string Carrier = reader.GetString(0);
                            arrObject.Add(Carrier);

                            // your code goes here...
                        }

                    }
                }
                connection.Close();
            }

        }

  
        private void Pattern()   
        {
            string fca=  textBox1.Text.Trim();
            string fca2 = "";
            string Pat = "";
            string Word = "";
            int Entry = 0;


            for (int f = 0; f < ArrCPNFareBasis.Count; f++)
            { 
           
               int Locate= fca.IndexOf(ArrCPNFareBasis[f].ToString());
               if (Locate > 0)
               {
                 fca= fca.Replace(ArrCPNFareBasis[f].ToString(),"XXXXXXXX");
               
               }
            
            }
                
               
            for (int x = 0; x < fca.Length; x++)
            {
            
                string Letter=fca.Substring(x,1);

                if ("0123456789.".Contains(Letter)) 
                {
                   
                   
                     if (Entry == 0) { Pat = Pat + "|"; fca2 = fca2 + "|"; }
                     Entry = 1;  Pat = Pat + "9";
                     fca2 = fca2 + Letter;
                }
                else 
                {

                    if (Entry == 1) { Pat = Pat + "|"; fca2 = fca2 + "|"; Entry = 0; }
                    if (Letter == " ") { Pat = Pat + " "; Letter = "|"; }
                     else { Pat = Pat + "X"; }
                     fca2 = fca2 + Letter;
                    
                }

               
              
            
            }

         
           
            try 
            { 
                
                int c1=-1;
                





                // c1 = fca2.IndexOf("NUC");
                //if (c1 != -1) { fca2 = fca2.Insert(c1+3, "="); }

                //c1 = fca2.IndexOf("I-");
                //if (c1 != -1) { fca2 = fca2.Insert(c1 + 2, "|"); }

                c1 = fca2.IndexOf("*");
                if (c1 != -1) { fca2 = fca2.Insert(c1+1 , "|"); }




                if (Pat.StartsWith("XX|99|XXX|99")) { fca2 = fca2.Substring(12, fca2.Length - 12); }
                if (Pat.StartsWith("|99|XXX|99|")) { fca2 = fca2.Substring(11, fca2.Length - 11); }
                if (Pat.StartsWith("X|99|XXX|99|")) { fca2 = fca2.Substring(13, fca2.Length - 13); }                
                 if (Pat.StartsWith("|99|XXX XXX")) { fca2 = fca2.Substring(8, fca2.Length - 8); }
                 if (Pat.StartsWith("XX XXXXX XX |99|XXX")) { fca2 = fca2.Substring(19, fca2.Length - 19).Trim(); }
                 if (Pat.StartsWith("XX XXXXXXX XX|9999999999999|XX XXX |99|XXX|99|XXX XXX")) { fca2 = fca2.Substring(53, fca2.Length - 53).Trim(); }
                
                 
                int c4 = fca2.IndexOf("XT");
                if (c4 > -1) { fca2 = fca2.Insert(c4, "|"); }
               



                if (fca2.StartsWith("FP"))
                {
                    c1 = fca2.IndexOf("FC");
                    fca2 = fca2.Substring(c1 + 2, fca2.Length - (c1 + 2));


                   
                   
                     c1 = fca2.IndexOf("JAN");
                     if(c1==-1)   
                     {  
                         c1 = fca2.IndexOf("FEB");

                     if(c1==-1)   
                     {   
                      c1 = fca2.IndexOf("MAR");
                     }
                        
                     if(c1==-1)   
                     {   
                     c1 = fca2.IndexOf("APR");
                     }
                         
                     if(c1==-1)   
                     {   
                     c1 = fca2.IndexOf("MAY");
                     }
                         
                     if(c1==-1)   
                     {   
                      c1 = fca2.IndexOf("JUN");
                     }
                        
                     if(c1==-1)   
                     {   
                      c1 = fca2.IndexOf("JUL");
                     }
                        
                     if(c1==-1)   
                     {   
                        c1 = fca2.IndexOf("AUG");
                     }
                         
                     if(c1==-1)   
                     {   
                        c1 = fca2.IndexOf("SEP");
                     }
                         
                     if(c1==-1)   
                     {   
                        c1 = fca2.IndexOf("OCT");
                     }
                         
                     if(c1==-1)   
                     {   
                        c1 = fca2.IndexOf("NOV");
                     }
                         
                     if(c1==-1)   
                     {   
                        c1 = fca2.IndexOf("DEC");
                     }


                     
                     }

                     if (c1 > -1) { fca2 = fca2.Substring(c1 + 4, fca2.Length - (c1 + 4)); }

                    


                }
            }



            catch { }
          

            textBox2.Text = Pat;
             
            Dg();

            int c2 = fca2.IndexOf("*");
            if (c2 > 0) {fca2=fca2.Substring(0,c2); }

            c2 = fca2.IndexOf("MGA5");
            if (c2 > 0) { fca2 = fca2.Substring(0, c2); }

            fca2 = fca2.Replace("|MGA5", "|MGA?|");
            fca2 = fca2.Replace("Z|4", "Z4|");
            fca2 = fca2.Replace("G|9", "G9|");
            fca2 = fca2.Replace("|J|2", "|J2|");
            fca2 = fca2.Replace("|A|5", "|A?|");
            fca2 = fca2.Replace("||", "|");

      if (fca2.Substring(0, 1) == "*") { fca2 = fca2.Substring(2, fca2.Length - 2); }

      textBox3.Text = fca2;

           
          
            string[] datapart = fca2.Split('|');

            int V=0;
            for (int x = 0; x < datapart.Length; x++)
            {
                dataGridView1.Rows.Add();

                if (datapart[x].ToString().Trim() == "XXXXXXXX")
                {
                    datapart[x] = ArrCPNFareBasis[V].ToString();
                    V++;
                    dataGridView1[0, x].Value = "Fare Basis";
                }


                 dataGridView1[0, x].Value ="Unknown";
                 dataGridView1[1, x].Value = datapart[x].ToString();
            }



            FCABKDN = fca2.Split('|');
        }



        private void DgvFareComp()
        {
            dgvFareComponent.Rows.Clear();
            dgvFareComponent.Columns.Clear();
            dgvFareComponent.Columns.Add("FareComponent", "Fare Component");
            dgvFareComponent.Columns[0].Width = 150;            
            dgvFareComponent.Rows.Add();
            dgvFareComponent[0, 0].Value = "FC NUC";
            dgvFareComponent.Rows.Add();
            dgvFareComponent[0, 1].Value = "FC HIP";
            dgvFareComponent.Rows.Add();
            dgvFareComponent[0, 2].Value = "Fare Component"; 
            dgvFareComponent.Rows.Add();
            dgvFareComponent[0, 3].Value = "Fare Basis";
            string Start = "";
            string End = "";

            string Start1 = "";
            string End1 = "";

            string NUC = "";
            string FC = "";
            string FB = "";

            Start = dataGridView1[1, 0].Value.ToString();
            int fcCnt = 1;
            for (int x = 1; x < dataGridView1.Rows.Count-1; x++)
            {

                if (dataGridView1[0, x].Value.ToString() == "City") { End = dataGridView1[1, x].Value.ToString(); FC = Start + End; }
                if (dataGridView1[0, x].Value.ToString().Contains("Fare Component") || dataGridView1[0, x].Value.ToString().Contains("Millage"))
                {
                    NUC= dataGridView1[1, x].Value.ToString();
                    dgvFareComponent.Columns.Add(fcCnt.ToString(), fcCnt.ToString());
                    dgvFareComponent.Columns[fcCnt].Width = 250;

                    dgvFareComponent[fcCnt, 0].Value = NUC;
                  
                    dgvFareComponent[fcCnt, 1].Value = "";
                    
                    dgvFareComponent[fcCnt, 2].Value = FC;
                    

                  // string FB GetFareBasis()
                    dgvFareComponent[fcCnt, 3].Value = "Fare Basis";

                    for(int r=0;r<dgvCoupons.Rows.Count;r++)
                    {//dgvCoupons[16, r].Value.ToString() + 
                        if (Start+dgvCoupons[17, r].Value.ToString() == FC )
                        {
                            FB = dgvCoupons[20, r].Value.ToString();
                            dgvFareComponent[fcCnt, 3].Value = FB;
                            break;
                        }
                        else 
                        {
                            if (Start+dgvCoupons[52, r].Value.ToString() == FC)
                            {
                                FB = dgvCoupons[20, r].Value.ToString();
                                dgvFareComponent[fcCnt, 3].Value = FB;
                                break;
                            }
                        
                        }


                    }




                   
                   

                     Start = End;
                     End = "";
                     NUC = "";
                     FC = "";
                     FB = "";
                     fcCnt++;

                }
           


            
            }
            GridFormat(dgvFareComponent);
       

         
        

           sectors();
        
        
        }

        private void sectors()
        {

            dgvSectors.Rows.Clear();
            dgvSectors.Columns.Clear();
            dgvSectors.Columns.Add("Sector", "Sector");
            dgvSectors.Columns[0].Width = 150;
            int col = 1;
            for (int c = 1; c < dgvFareComponent.Columns.Count; c++)
            {
                Color colr=dgvFareComponent.Columns[c].DefaultCellStyle.BackColor;

                      dgvSectors.Columns.Add("Begin","Begin");
                      dgvSectors.Columns[col++].Width = 40;
                     
                    dgvSectors.Columns.Add("Carrier","Carrier");
                    dgvSectors.Columns[col++].Width = 40;
           
                    dgvSectors.Columns.Add("End","End");
                    dgvSectors.Columns[col++].Width = 40;
           
                    dgvSectors.Columns.Add("Q","Q");
                    dgvSectors.Columns[col++].Width = 65;

                   dgvSectors.Columns.Add("S","S");
                   dgvSectors.Columns[col++].Width = 65;

                  
            
            }
             dgvSectors.Columns[0].DefaultCellStyle.BackColor = dgvFareComponent.Columns[0].DefaultCellStyle.BackColor;

             int st = 1;

             for (int c = 1; c < dgvFareComponent.Columns.Count; c++)
             {
                 for (int cx = 1; cx <= 5; cx++)
                 {
                     try
                     {
                         dgvSectors.Columns[st].DefaultCellStyle.BackColor = dgvFareComponent.Columns[c].DefaultCellStyle.BackColor;

                         st++;
                     }
                     catch { }

                 }

             }

           string Start = "";
           string Carrier = "";
           string NextCity = "";
           string Q = "";
           string P = "";
           
       
           string Sector = "";
          

           Start = dataGridView1[1, 0].Value.ToString();
           
           int SecCount = 0;
           int fcCnt = 0;
            
        
          
           int Cell = 1;
           int R = 0;
           ArrayList FC1 = new ArrayList();

          
           string Data = "";
           int Pass = 0;
           int LastCol=1;

           int CarrCnt=0;

           string Line="";
           string Previous = "";
           for (int x = 0; x < dataGridView1.Rows.Count; x++)
           {


               string Parts = dataGridView1[0, x].Value.ToString() + "xxxxxx";
               Parts = Parts.Substring(0, 4);
               if (Parts == "Tran") { Parts = "City"; }
               if (Parts == "Mill") { Parts = "Fare"; }

               switch (Parts)
               {
                   case "City":
                       if (Line.Trim().Length == 0)
                       {
                           NextCity = dataGridView1[1, x].Value.ToString();
                           Line +=  NextCity + "|";
                       }
                       else
                       {
                          
                            NextCity = dataGridView1[1, x].Value.ToString();

                        
                           Line += NextCity + "|";
                           }
                       

                     
                           Previous = Parts;

                       break;
                   case "Carr":

                         Carrier = dataGridView1[1, x].Value.ToString();


                         if (CarrCnt == 0) { Line += Carrier + "|"; }
                         else { Line += "^"+NextCity+"|"+Carrier + "|"; }
                          CarrCnt++;
                       

                         
                       break;
                  
                       break;
                   case "Q-Su":

                       Line += "q:" + dataGridView1[1, x].Value.ToString() + ":";
                       break;
                   case "Plus":
                       Line += "p:" + dataGridView1[1, x].Value.ToString() + ":";
                       break;

                   case "Fare":

                       string xxx = Data;
                       String A = dataGridView1[0, x - 1].Value.ToString();
                       String B = dataGridView1[0, x].Value.ToString();
                       String C = dataGridView1[0, x + 1].Value.ToString();
                       String A1 = dataGridView1[1, x - 1].Value.ToString();
                       String B2 = dataGridView1[1, x].Value.ToString();
                       String C1 = dataGridView1[1, x + 1].Value.ToString();

                       fcCnt++;
                       

                       Line=fcCnt.ToString()+"+"+Line;
                       Line = Line.Replace("+^", "+");
                       if (Line.Length > 3) 
                       { 
                        FC1.Add(Line);
                       }
                       
                        Line="";
                       if (dataGridView1[0, x + 1].Value.ToString().Contains("Carrier"))
                       {                            
                           //Line += "%" + NextCity + "|"; ;


                       }



                       //  NextCity = dataGridView1[1, x].Value.ToString();
                       //    Data += NextCity + "|^" + NextCity + "|";


                       break;


               }


               
           }


          



           Data = Data.Replace("X/E/", "");
           Data = Data.Replace("X/", "");
           Data = Data.Replace("/-", "");
           Data = Data.Replace("//", "");
           Data = Data.Replace("/*", "");
           Data = Data.Replace("*/", "");
           Data = Data.Replace("(", "");
           Data = Data.Replace(")", "");
           Data = Data.Replace("I-", "");
           Data = Data.Replace("/E", "");
           Data = Data.Replace("//", "");
           Data = Data.Replace("/*", "");
           Data = Data.Replace("*/", "");
           Data = Data.Replace("(", "");
           Data = Data.Replace(")", "");


           int Row = 0;
           int GRow= 0;
          // int col=1;

            int i=-1;
           int sets = 0;
           foreach (string strData in FC1)
           {
               i++;GRow = 0;


               string str = strData.Substring(2, strData.Length - 2);

               int X1 =Convert.ToInt16( strData.Substring(0, 1));

                string[] Sec;
                if (str.IndexOf("^") > 0)
                {    
                    Sec = str.Split('^');

                }  
                else
                { 
                    Sec = str.Split('^');
                }


                  int X = Sec.Length; ;


                 

                      for (int z = 0; z < Sec.Length; z++)
                      {

                          if (dgvSectors.Rows.Count < z + 1)
                          {
                              dgvSectors.Rows.Add();

                              GRow = dgvSectors.Rows.Count - 1;
                          }
                          else
                          { GRow = z; }

                           if (i == 0) { col = 1;  }
                           if (i == 1) { col = 6; }
                           if (i == 2) { col = 11; }
                           if (i == 3) { col = 16; }
                           if (i == 4) { col = 1; }
                           if (i == 5) { col = 1; }
                           if (i == 6) { col = 1; }
                           if (i == 7) { col = 1; }
                           if (i == 8) { col = 1; }
              
                          




                          string[] SecA = Sec[z].ToString().Split('|');
                            for (int c = 0; c < SecA.Length; c++)
                                {
                                    if (SecA[c].ToString().Trim().Length > 0)
                                    {


                                        Data=SecA[c].ToString();
                                        Data = Data.Replace("X/E/", "");
                                        Data = Data.Replace("X/", "");
                                        Data = Data.Replace("/-", "");
                                        Data = Data.Replace("//", "");
                                        Data = Data.Replace("/*", "");
                                        Data = Data.Replace("*/", "");
                                        Data = Data.Replace("(", "");
                                        Data = Data.Replace(")", "");

                                        Data = Data.Replace("/E", "");
                                        Data = Data.Replace("//", "");
                                        Data = Data.Replace("/*", "");
                                        Data = Data.Replace("*/", "");
                                        Data = Data.Replace("(", "");
                                        Data = Data.Replace(")", "");
                                        Data = Data.Replace("q:", "");




                                        dgvSectors[col, GRow].Value = Data;
                                        col++;
                                    }
                                    

                                }

                         

                      }


                    
           }     
                   





           int c1 = 0;
           foreach (DataGridViewColumn column in dgvSectors.Columns)
           {
               column.SortMode = DataGridViewColumnSortMode.NotSortable;
               column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
               column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
               column.DefaultCellStyle.ForeColor = Color.Navy;


              

             
           }
                        
           
         
               this.dgvSectors.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
          
        }



        private void GridFormat(DataGridView dgv)
        {

            foreach (DataGridViewColumn column in dgv.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.DefaultCellStyle.ForeColor = Color.Navy;
                int x = column.Index;
                switch (x)
                {
                    case 0:
                        column.DefaultCellStyle.BackColor = Color.LightPink;
                        column.DefaultCellStyle.ForeColor = Color.Navy;
                        break;
                    case 1:
                        column.DefaultCellStyle.BackColor = Color.LightGreen;
                        column.DefaultCellStyle.ForeColor = Color.Navy;
                        break;
                    case 2:

                        column.DefaultCellStyle.BackColor = Color.Azure;
                        column.DefaultCellStyle.ForeColor = Color.Navy;
                        break;
                    case 3:
                        column.DefaultCellStyle.BackColor = Color.Khaki;
                        column.DefaultCellStyle.ForeColor = Color.Navy;
                        break;
                    case 4:
                        column.DefaultCellStyle.BackColor = Color.Lavender;
                        column.DefaultCellStyle.ForeColor = Color.Navy;
                        break;
                    case 5:
                        break;
                    case 6:
                        break;
                    case 7:
                        break;
                    case 8:
                        break;
                    case 9:
                        break;

                }

            }
        
        
        }
        private void Dg()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("Key", "Key");
            dataGridView1.Columns.Add("Value", "Value");
            dataGridView1.Columns[0].Width=125;
            dataGridView1.Columns[1].Width = 125;

            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        
        }

        
        private void BuildFCA()
        {
            string f = textBox3.Text;
            string[] fb = f.Split('|'); ;
            string dummy = f.Replace("|", " ");
             dummy=dataGridView1[1, 0].Value.ToString();
             int PASS = 0;
            for (int i = 1; i < dataGridView1.Rows.Count-1; i++)
            {
                if (dataGridView1[0, i].Value.ToString() == "TAXES") { break; }
               string data1 = dataGridView1[1, i-1].Value.ToString();
                string data2 = dataGridView1[1, i].Value.ToString();
                string data3 = dataGridView1[0, i].Value.ToString();

                if (data3 == "END") { data2 = data3; PASS = 1; }
                if (data3 == "ROE") 
                {
                    data2 = data3 + data2;
                    PASS = 0;
                }
                if (data3 == "NUC")
                {
                    data2 =data3+data2  ;
                    PASS = 0;
                }
               try
               {
                 decimal DEC= Convert.ToDecimal(data2);
                 dummy = dummy.Trim() + data2;
                 PASS = 1;
               }
                catch
               {
                   if (PASS == 1) { dummy = dummy +  data2; } else { dummy = dummy + " " + data2; }
                   PASS = 0;
                }

               
            }
            textBox4.Text = dummy.Replace(":","");
        
        }
        private void CheckAirport()
                {
                    int IsAirport = -1;

                    for (int R = 0; R < dataGridView1.Rows.Count; R++)
                    {

                        try
                        {
                            IsAirport = -1;
                            string Data = dataGridView1[1, R].Value.ToString().Trim();
                            string Data1 = dataGridView1[1, R].Value.ToString().Trim();
                            Data = Data.Replace("X/E/", "");
                            Data = Data.Replace("X/", "");
                            Data = Data.Replace("/-", "");
                            Data = Data.Replace("//", "");
                            Data = Data.Replace("/*", "");
                            Data = Data.Replace("*/", "");
                            Data = Data.Replace("(", "");
                            Data = Data.Replace(")", "");
                            Data = Data.Replace("I-", "");
                            Data = Data.Replace("/E", "");
                            Data = Data.Replace("//", "");
                            Data = Data.Replace("/*", "");
                            Data = Data.Replace("*/", "");
                            Data = Data.Replace("(", "");
                            Data = Data.Replace(")", "");
                            Data = Data.Replace("I-", "");




                            if (Data.ToString().Equals("END"))
                            {
                                dataGridView1[0, R].Value = "END";
                                dataGridView1[1, R].Value = " ";
                            }

                            if (Data.ToString().Equals("ROE"))
                            {
                                dataGridView1[0, R].Value = "ROE";
                                dataGridView1[1, R].Value = dataGridView1[1, R + 1].Value;
                                dataGridView1[1, R + 1].Value = "Unknown";
                                dataGridView1[1, R + 1].Value = null;
                            }


                            if (Data.Contains("X/") || Data.Contains("/-") || Data.Contains("//") || Data.Contains("/*") || Data.Contains("*/") || Data.Contains("(") || Data.Contains(")"))
                            {
                                IsAirport = ArrAirports.BinarySearch(Data.Substring(2, 3));

                                dataGridView1[0, R].Value = "City";

                                if (Data1.Contains("X/")) { dataGridView1[0, R].Value = "Transit City"; }
                                if (Data1.Contains("/-")) { dataGridView1[0, R].Value = "Surface/City"; }
                            }
                            else
                            {
                                if (Data.Length == 3)
                                {
                                    IsAirport = ArrAirports.BinarySearch(Data.Substring(0, 3));

                                    if (IsAirport > -1)
                                    {
                                        dataGridView1[0, R].Value = "City";

                                        if (Data1.Contains("X/")) { dataGridView1[0, R].Value = "Transit City"; }
                                        if (Data1.Contains("/-")) { dataGridView1[0, R].Value = "Surface/City"; }

                                    }


                                }



                                var T1 = dataGridView1[0, R].Value;

                                if (T1 == null)
                                {

                                    if (IsAirport > -1)
                                    {

                                        if (dataGridView1[1, R].Value.ToString() != "MGA")
                                        {
                                            dataGridView1[0, R].Value = "City";
                                            if (Data1.Contains("X/")) { dataGridView1[0, R].Value = "Transit City"; }
                                            if (Data1.Contains("/-")) { dataGridView1[0, R].Value = "Surface/City"; }
                                        }
                                        else { dataGridView1[0, R].Value = "Unknown"; }

                                    }
                                    else { dataGridView1[0, R].Value = "Unknown"; }
                                }

                            }
                        }
                        catch { }
                    }




                }
        private void CheckCarrier()
        {
            int IsCarrier = -1;

            for (int R = 0; R < dataGridView1.Rows.Count; R++)
            {
                if (dataGridView1[1, R].Value == "END" || dataGridView1[0, R].Value == "ROE") { break; }
                try
                {
                    var tmp = dataGridView1[0, R].Value;

                    if (dataGridView1[0, R].Value == "Unknown" || tmp == null)
                    {
                        string Data = dataGridView1[1, R].Value.ToString().Trim();
                        Data = Data.Replace("X/E/", "");
                        Data = Data.Replace("X/", "");
                        Data = Data.Replace("/-", "");
                        Data = Data.Replace("//", "");
                        Data = Data.Replace("/*", "");
                        Data = Data.Replace("*/", "");
                        Data = Data.Replace("(", "");
                        Data = Data.Replace(")", "");

                        Data = Data.Replace("/E", "");
                        Data = Data.Replace("//", "");
                        Data = Data.Replace("/*", "");
                        Data = Data.Replace("*/", "");
                        Data = Data.Replace("(", "");
                        Data = Data.Replace(")", "");

                        if (Data.Length == 2)
                        {
                            IsCarrier = ArrCarrier.BinarySearch(Data);
                            if (IsCarrier > -1) { dataGridView1[0, R].Value = "Carrier"; }
                            else 
                            {
                               // dataGridView1[0, R].Value = "Unknown"; 
                            }

                        }
                        else
                        {
                            if (Data.Length == 1)
                                if (dataGridView1[0, R - 1].Value.ToString().Trim().IndexOf("City") != -1)
                                {
                                    string Tmp = dataGridView1[1, R].Value.ToString().Trim() + dataGridView1[1, R + 1].Value.ToString().Trim();

                                    IsCarrier = ArrCarrier.BinarySearch(Tmp);
                                    if (IsCarrier > -1)
                                    {
                                        dataGridView1[0, R].Value = "Carrier";
                                        dataGridView1[1, R].Value = Tmp;
                                        dataGridView1[1, R + 1].Value = null;

                                        for (int R1 = 0; R1 < dataGridView1.Rows.Count; R1++)
                                        {
                                            if (dataGridView1[0, R1].Value == "Unknown" && (dataGridView1[1, R1].Value == null || dataGridView1[1, R1].Value == ""))
                                            {
                                                //dataGridView1.Rows.RemoveAt(R1);

                                            }

                                        }

                                    }
                                    else { dataGridView1[0, R].Value = "Unknown"; }

                                }
                        }

                    }



                }




                catch { }


            }

        }       

        private void CheckfareComponent()
        {
            int X = 0;
            int IsfareBasis = -1;
            
            for (int R = 0; R < dataGridView1.Rows.Count; R++)
            {if (dataGridView1[0, R].Value == "END" || dataGridView1[0, R].Value == "ROE") { break; }
            try
            {
                if (dataGridView1[0, R].Value == "Unknown")
                {


                    if (dataGridView1[1, R].Value.ToString().Trim().IndexOf(".") != -1)
                    {
                        try
                        {
                            decimal tmp = Convert.ToDecimal((dataGridView1[1, R].Value.ToString().Trim()));
                            X++;
                            dataGridView1[0, R].Value = "Fare Component " + X.ToString();
                        }
                        catch
                        {


                        }
                    }
                    else 
                    {
                           decimal tmp = Convert.ToDecimal((dataGridView1[1, R].Value.ToString().Trim()));
                            X++;
                            dataGridView1[0, R].Value = "Fare Component " + X.ToString();
                         
                        }
                    }
                }
                
            
            catch { }
                }
            }

        private void CheckfareBasis()
        {
            int X = 0;
            int IsfareBasis = -1;
            
            for (int R = 0; R < dataGridView1.Rows.Count; R++)
            {if (dataGridView1[0, R].Value == "END" || dataGridView1[0, R].Value == "ROE") { break; }
                try
                {
                                       
                    
                    if (dataGridView1[0, R].Value == "Unknown")
                    {
                        

                        string Data1 = dataGridView1[0, R - 1].Value.ToString().Trim();
                        string Data2 = dataGridView1[0, R].Value.ToString().Trim();
                        string Data3 = dataGridView1[0, R + 1].Value.ToString().Trim();

                        string DataA = "";
                        string DataB = "";
                        string DataC = "";

                        if (Data1 == "Unknown") { DataA = dataGridView1[1, R-1].Value.ToString().Trim();}
                        if (Data2 == "Unknown") { DataB = dataGridView1[1, R].Value.ToString().Trim(); }
                        if (Data3 == "Unknown") { DataC = dataGridView1[1, R+1].Value.ToString().Trim(); }

                        
                        


                        string Data =DataA+DataB+DataC;



                        if (Data.Trim().Length > 0)
                        {
                            string LastSearch="";
                            for (X = 1; X <= Data.Length; X++)
                            {
                                string Sch = Data.Trim().Substring(0, X);
                                IsfareBasis = ArrFareBasis.BinarySearch(Sch);
                                if (IsfareBasis > -1)
                                {
                                    LastSearch = LastSearch + Sch.Trim();
                                }
                            }
                            string[] matchesA = ArrFareBasis.Cast<string>()
                            .Where(i => i.StartsWith(LastSearch.Trim())).ToArray();
                            if (matchesA.Count() == 1) { dataGridView1[0, R].Value = "Fare Basis"; }
                            else { }

                           
                       
                                                    
                            
                            
                            }


                        }



                    }                
                catch { }

        }

            //////////for (int R1 = 0; R1 < dataGridView1.Rows.Count; R1++)
            //////////{
            //////////    if (dataGridView1[0, R1].Value == "Unknown" && (dataGridView1[1, R1].Value == null || dataGridView1[1, R1].Value == ""))
            //////////    {
            //////////        try {
            //////////       //     dataGridView1.Rows.RemoveAt(R1); 
            //////////           R1 = R1 - 1;
            //////////        }
            //////////        catch { }
                   
                    

            //////////    }

            //////////}

            }

        private void checkEnd()         
        {  
            bool  XTPass=false;
            pvCurrAmount = "";
            int IsCurrency = 0;
            for (int R = 0; R < dataGridView1.Rows.Count; R++)
            {
                try
                {
                    string Data2 = "";
                    string Data3 = "";

                   // 
                    {
                    if (dataGridView1[0, R].Value == "Unknown")
                    {

                        if (dataGridView1[1, R].Value != null)
                        {

                        

                            Data2 = dataGridView1[1, R].Value.ToString();
                            Data3 = dataGridView1[1, R].Value.ToString();


                            {




                                if (Data3.ToString().Equals("M"))
                                {

                                    dataGridView1[0, R].Value = "Millage ";
                                    dataGridView1[1, R].Value = dataGridView1[1, R + 1].Value;
                                    dataGridView1[1, R + 1].Value = "";


                                    int pos = Array.IndexOf(FCABKDN, "M");
                                    if (pos > -1)
                                    {

                                    }
                                }

                                if (Data3.ToString().Equals("P"))
                                {
                                    dataGridView1[0, R].Value = "Plus Up ";
                                   
                                                                      
                                    int pos = Array.IndexOf(FCABKDN, "P");
                                    if (pos > -1)
                                    {
                                        string ele1 = FCABKDN[pos].ToString();
                                        string ele2 = FCABKDN[pos+1].ToString();
                                        int IsAirport1 = ArrAirports.BinarySearch(ele2.Substring(0, 3));
                                        int IsAirport2 = ArrAirports.BinarySearch(ele2.Substring(3, 3));
                                        string ele3 = FCABKDN[pos+2].ToString();


                                        if (IsAirport1 > 0 && IsAirport2 > 0)
                                        {  
                                            dataGridView1[0, R].Value += dataGridView1[1, R + 1].Value.ToString();
                                            dataGridView1[1, R].Value = ele3;
                                            dataGridView1[1, R + 1].Value = null;
                                            dataGridView1[1, R + 2].Value = null;
                                            dataGridView1[0, R + 1].Value = "Unknown";
                                            dataGridView1[0, R + 2].Value = "Unknown";
                                        }
                                    }
                                }
                                if (Data3.ToString().Equals("S"))
                                {
                                    dataGridView1[0, R].Value = "Surplus ";
                                    dataGridView1[1, R].Value = dataGridView1[1, R + 1].Value;
                                    dataGridView1[1, R + 1].Value = "";
                                }
                                if (Data3.ToString().Equals("D"))
                                {
                                    dataGridView1[0, R].Value = "Differential ";
                                    //dataGridView1[1, R].Value = dataGridView1[1, R + 1].Value;
                                    //dataGridView1[1, R + 1].Value = "";
                                    try
                                    {
                                        decimal tmp = Convert.ToDecimal((dataGridView1[1, R + 1].Value.ToString().Trim()));
                                        dataGridView1[1, R].Value = dataGridView1[1, R + 1].Value;
                                        dataGridView1[1, R + 1].Value = "";
                                        // dataGridView1[0, R + 1].Value = "Unknown";

                                    }
                                    catch
                                    {

                                        dataGridView1[0, R].Value = dataGridView1[0, R].Value + "[" + dataGridView1[1, R + 1].Value + "]";

                                        dataGridView1[1, R].Value = dataGridView1[1, R + 2].Value;

                                        dataGridView1[0, R + 2].Value = "Unknown";
                                        dataGridView1[0, R + 1].Value = "Unknown";

                                        dataGridView1[1, R + 2].Value = null;
                                        dataGridView1[1, R + 1].Value = null;

                                    }
                                }

                                if (Data3.ToString().Equals("Q"))
                                {
                                    dataGridView1[0, R].Value = "Q-Surcharge ";







                                    try
                                    {
                                        decimal tmp = Convert.ToDecimal((dataGridView1[1, R + 1].Value.ToString().Trim()));
                                        dataGridView1[1, R].Value = dataGridView1[1, R + 1].Value;
                                        dataGridView1[1, R + 1].Value = "";
                                        // dataGridView1[0, R + 1].Value = "Unknown";

                                    }
                                    catch
                                    {

                                        dataGridView1[0, R].Value = dataGridView1[0, R].Value + "[" + dataGridView1[1, R + 1].Value + "]";

                                        dataGridView1[1, R].Value = dataGridView1[1, R + 2].Value;

                                        dataGridView1[0, R + 2].Value = "Unknown";
                                        dataGridView1[0, R + 1].Value = "Unknown";

                                        dataGridView1[1, R + 2].Value = null;
                                        dataGridView1[1, R + 1].Value = null;

                                    }



                                }




                                if (Data3.ToString().Equals("NUC"))
                                {
                                    dataGridView1[0, R].Value = "NUC";
                                    dataGridView1[1, R].Value = dataGridView1[1, R + 1].Value;
                                    dataGridView1[1, R + 1].Value = null;
                                    dataGridView1[0, R + 1].Value = "Unknown";



                                }
                                else
                                {
                                    string Data = "";
                                    if (Data2.ToString().Length == 3)
                                    {
                                        IsCurrency = ArrCurrency.BinarySearch(Data2);
                                        if (IsCurrency > -1)
                                        {
                                            dataGridView1[0, R].Value = "Currency";
                                           
                                            for (int C = R; C < R + 3; C++)
                                            {
                                                try 
                                                {
                                                    decimal tmp = Convert.ToDecimal((dataGridView1[1, C].Value.ToString().Trim()));
                                                    dataGridView1[1, R].Value = Data2 + ":" + tmp.ToString();
                                                    dataGridView1[0, C].Value = "Unknown";
                                                    dataGridView1[1, C].Value = null;
                                                }
                                                catch { }
                                                
                                            
                                            }
                                                
                                              

                                            pvCurrAmount = dataGridView1[1, R + 2].Value.ToString();
                                            //  dataGridView2.Rows[R]
                                            //  dataGridView2.Rows.Remove(R-1);

                                            // if (Convert.ToBoolean(dataGridView1.Rows[R].Cells[[yourCheckBoxColIndex].Value) == true)
                                            {
                                                dataGridView1.Rows.RemoveAt(R + 1);
                                            }

                                            break;
                                        }
                                        else
                                        {
                                             dataGridView1[0, R].Value = "Unknown"; 
                                        }

                                    }
                                }


                                if (Data3.ToString().Contains("M/IT"))
                                {
                                    dataGridView1[0, R].Value = dataGridView1[1, R].Value;
                                    // dataGridView1[1, R].Value = dataGridView1[1, R + 1].Value;

                                }
                                if (Data3.ToString().Contains("/BT"))
                                {
                                    dataGridView1[0, R].Value = dataGridView1[1, R].Value;
                                    // dataGridView1[1, R].Value = dataGridView1[1, R + 1].Value;

                                }
                                if (Data3.ToString().Contains("PLUS"))
                                {
                                    dataGridView1[0, R].Value = dataGridView1[1, R].Value;
                                    // dataGridView1[1, R].Value = dataGridView1[1, R + 1].Value;

                                }
                                if (Data3.ToString().Contains("/IT"))
                                {
                                    dataGridView1[0, R].Value = dataGridView1[1, R].Value;
                                    // dataGridView1[1, R].Value = dataGridView1[1, R + 1].Value;

                                }


                               

                            }
                    
                            
                            
                            }

                    


                           

                        }
                    }





                    for (int R1 = 0; R1 < dataGridView1.Rows.Count; R1++)
                    {
                        if (dataGridView1[0, R1].Value == "Unknown" && (dataGridView1[1, R1].Value == null || dataGridView1[1, R1].Value == ""))
                        {
                            dataGridView1.Rows.RemoveAt(R1);
                            R1 = R1 - 1;
                        }

                    }


                }
                catch
                {

                    int x = 0;
                    x = -9099;
                }

            }
        }

        private void CheckTaxes()
 {

     bool XTPass = false;
     string Data2 = "";
     string Data3 = "";
     int R = 0;
     int Pos = 0;
     for ( R = 0; R < dataGridView1.Rows.Count; R++)
     {
         try 
         {

             Data2 =  dataGridView1[1, R].Value.ToString();
             Data3 =  dataGridView1[1, R].Value.ToString();
             if (Data3.Contains("*")) { Pos = R; break; }
             if (Data3.ToString().Contains("XT") || Data3.ToString().Contains("XF") || Data3.ToString().Contains("PD"))
             {
                 dataGridView1[0, R].Value = "TAXES"; XTPass = true;
                 dataGridView1[1, R].Value = Data3;

             }
             else 
             {

                 if (XTPass == true)
                 {

                     decimal tmp = 0;
                     try { tmp = Convert.ToDecimal(dataGridView1[1, R].Value); }
                     catch { }


                     if (dataGridView1[1, R].Value.ToString().Trim().IndexOf(".") != -1 || tmp > 0)
                     {
                         try
                         {
                             dataGridView1[0, R].Value = dataGridView1[1, R + 1].Value;
                             dataGridView1[1, R + 1].Value = null;
                             dataGridView1[0, R + 1].Value = "Unknown";



                         }
                         catch { }



                     }
                     else
                     {

                         dataGridView1[0, R].Value = dataGridView1[1, R + 1].Value;
                         dataGridView1[1, R + 1].Value = null;
                         dataGridView1[0, R + 1].Value = "Unknown";


                     }


                 }
             
             
             }


           
         
         
         }
         catch { }
       




     }

     for (int R1 = 0; R1<dataGridView1.Rows.Count; R1++)
     {
         
         {
             try
             {
                 if (dataGridView1[0, R1].Value == null  && (dataGridView1[1, R1].Value == null)) { dataGridView1.Rows.RemoveAt(R1); }
                 else
                 { 
                     if (dataGridView1[0, R1].Value== "Unknown" || (dataGridView1[1, R1].Value == null || dataGridView1[1, R1].Value.ToString() == ""))
                     { dataGridView1.Rows.RemoveAt(R1); }
                 }
                 
                

             }
             catch { }



         }
     }
 }               

        private void button1_Click(object sender, EventArgs e)
        {
            arrObject.Clear();
            textBox1.Text = pbFca;
            lblDocNo.Text = PbDocNo;
            Pattern();
            CheckAirport();
            CheckCarrier();
            CheckfareComponent();
            CheckfareBasis();
            checkEnd();
            CheckTaxes();
            BuildFCA();


            GetIicketInformation();
            GeCouponsInformation();
            GePaymentInformation();
            GetOtherPaymentsInformation();
            GetProrationDetailsInformation();
            GetProrationExceptionInformation();
            DgvFareComp();
            
        }

        private void button3_Click(object sender, EventArgs e)
        {


            string[] Target = new string[9];
            Target[0]="TEST,button13,pnl10";
            //=====================================
            Target[1]="dgvExchange,button12,pnl9";
            //=====================================
            Target[2]="dgvInterline,button10,pnl8";
            //=====================================
            Target[3]="dgvProrationExceptions,button9,pnl7";
            //=====================================
            Target[4]="dgvProtionDetails,button8,pnl6";
            //=====================================
            Target[5]="dgvOtherPayments,button4,pnl5";
            //=====================================
            Target[6]="dgvPayments,button7,pnl4";
            //=====================================
            Target[7]="dgvCoupons,button6,pnl3";
            //=====================================
            Target[8]="dgvTicketDocNo,button5,pnl2";

            Button btn = (Button)(sender);
            string dgvx="";
            string pnlx = "pnl" + btn.Tag.ToString();
            int C = -1;
            for (int s = 0; s < Target.Length; s++)
            {
                C = Target[s].ToString().IndexOf(pnlx);
                if (C >= 0) 
                {
                    string[] d = Target[s].ToString().Split(',');
                    dgvx = d[0].ToString();
                    break;
                }
            }

            Control[] Pnl = this.Controls.Find(pnlx, true);
            if (Pnl != null && Pnl.Length > 0)
            {
                if (Pnl[0].Tag.ToString() == "1")
                {
                    Pnl[0].Height = 20;
                    Pnl[0].Tag = "0";
               
            }
                else
                {
                    Control[] dgv;
                  
                    try 
                    {

                    dgv = this.Controls.Find(dgvx, true);
                    if (dgv != null && dgv.Length > 0)
                    {
                      

                        DataGridView dgv1 =  (DataGridView)(dgv[0]);
                        Pnl[0].Height = (dgv1.RowCount * 50) + dgv1.ColumnHeadersHeight+10;
                        Pnl[0].Tag = "1";
                    }

                    }
                    catch 
                    {
                     Pnl[0].Height = 300;
                    
                    }
                  
                  
                   
              
            }

            Pnl[0].Invalidate();
            this.Validate();
            }
            

          
            
        }
   
        private void button11_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
#endregion 1

#region GetRecords
        private void GetIicketInformation()
        {
            String Sql = "SELECT  * " +
            "FROM [Pax].[SalesDocumentHeader]  where "+
            " DocumentNumber = '" + PbDocNo +"'";
            GetData(Sql, dgvTicketDocNo);
        
        }
        private void GeCouponsInformation()
        {


        String Sql = "Select SRI.* , SDC.* from pax.SalesDocumentHeader SDH "+
        " left join pax.SalesRelatedDocumentInformation SRI on SDH.HdrGuid = SRI.HdrGuid "+
        " left join pax.SalesDocumentCoupon SDC on SRI.RelatedDocumentGuid=SDC.RelatedDocumentGuid "+
        " where SDH.DocumentNumber = '" + PbDocNo +"'";
        GetData(Sql, dgvCoupons);


        
        }
        private void GePaymentInformation()
        { 
       string Sql=" Select SRI.* , SDP.* from pax.SalesDocumentHeader SDH  "+
                  "left join pax.SalesRelatedDocumentInformation SRI on SDH.HdrGuid = SRI.HdrGuid  "+
                  "left join pax.SalesDocumentPayment SDP on SRI.RelatedDocumentGuid=SDP.RelatedDocumentGuid "+
                  " where SDH.DocumentNumber = '" + PbDocNo + "'";
                 GetData(Sql, dgvPayments);
        
        
        }
        private void GetOtherPaymentsInformation()
        {
            string Sql="Select SDOA.* from pax.SalesDocumentHeader SDH "+
            "left join pax.SalesRelatedDocumentInformation SRI on SDH.HdrGuid = SRI.HdrGuid  "+
            "left join pax.SalesDocumentOtherAmount SDOA on SRI.RelatedDocumentGuid=SDOA.RelatedDocumentGuid  " +
            " where SDH.DocumentNumber = '" + PbDocNo + "'";        
            GetData(Sql,dgvOtherPayments);
        
        
        }     
        private void GetProrationDetailsInformation()
        { 
        
        string Sql="Select PD.* from pax.SalesDocumentHeader SDH "+
                   " left join pax.SalesRelatedDocumentInformation SRI on SDH.HdrGuid = SRI.HdrGuid "+  
                   " left join pax.ProrationDetail PD on SRI.RelatedDocumentGuid=PD.RelatedDocumentGuid "+  
                   " where SDH.DocumentNumber = '" + PbDocNo + "'";        
                    GetData(Sql,dgvProtionDetails);
        




        }
        private void GetProrationExceptionInformation()
        {

            string Sql ="Select PE.* from pax.SalesDocumentHeader SDH " +
                       " left join pax.SalesRelatedDocumentInformation SRI on SDH.HdrGuid = SRI.HdrGuid" +
                       " left join pax.[ProrationException] PE on SRI.RelatedDocumentGuid=PE.RelatedDocumentGuid" +
                       " where SDH.DocumentNumber = '" + PbDocNo + "'";
                       GetData(Sql, dgvProrationExceptions);
        
        
        
        }
        private void GetExchangeInformation(string tktno)

         {    string OriginalIssueDocumentNumber = "";
              string DocumentNumber = tktno;


            

            try{
                string Sql = " Select DocumentNumber,OriginalIssueDocumentNumber  from pax.SalesDocumentHeader SDH " +
                             " where DocumentNumber = '" + tktno + "'";



                           using (SqlConnection connection = new SqlConnection(ConnectionString()))
                           {
                               connection.Open();

                               using (SqlCommand command = connection.CreateCommand())
                               {

                                   command.CommandText = @Sql;

                                   using (SqlDataReader reader = command.ExecuteReader())
                                   {

                                       while (reader.Read())
                                       {
                                           try { OriginalIssueDocumentNumber = reader.GetValue(1).ToString(); }catch { }
                                           
                                          

                                           DocumentNumber  = reader.GetString(0);
                                          
                                           arrObject.Add( DocumentNumber + "|" + OriginalIssueDocumentNumber);
                                           connection.Close();
                                           break;
                                       }

                                   }
                               }
                              
                           }

                           if (OriginalIssueDocumentNumber.Trim().Length > 0) { GetExchangeInformation(OriginalIssueDocumentNumber); }
                           else 
                           {



                           }

                           


            }
            catch
            {
            
            }
        
        }
        private void GetExchangeInformation2(string tktno)
        {
            string OriginalIssueDocumentNumber = "";
            string DocumentNumber = tktno;




            try
            {
                  string Sql = " Select DocumentNumber,OriginalIssueDocumentNumber  from pax.SalesDocumentHeader SDH " +
                             " where OriginalIssueDocumentNumber = '" + tktno + "'";
                ////GetData(Sql, dgvExchange);

               


                using (SqlConnection connection = new SqlConnection(ConnectionString()))
                {
                    connection.Open();

                    using (SqlCommand command = connection.CreateCommand())
                    {

                        command.CommandText = @Sql;

                        using (SqlDataReader reader = command.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                try { OriginalIssueDocumentNumber = reader.GetValue(1).ToString(); }
                                catch { }



                                DocumentNumber = reader.GetString(0);

                                arrObject.Add(DocumentNumber + "|" + OriginalIssueDocumentNumber);
                               
                               // break;
                            }

                        }
                    }
                  connection.Close();
                }

                if (OriginalIssueDocumentNumber.Trim().Length > 0) { GetExchangeInformation2(DocumentNumber); }
                else
                {
                    int X = 0;
                    X = 999;
                    

                }


            }
            catch
            {

            }

        }
        private void GetData(string Sql, DataGridView dgv)
            {
            
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

            dgv.DataSource = ds.Tables[0];

            con.Close();

            }
        #endregion GetRecords

        private void button15_Click(object sender, EventArgs e)
        {

            WordEditor.frmwp wp = new WordEditor.frmwp();
            wp.pbConnectionString = ConnectionString();
            wp.REFID = lblDocNo.Text;
            wp.LinkedDataItem = "Customer: ";
            wp.label10.Text = lblDocNo.Text; ;// txtEmpId.Text.Trim() + "  " + txtSurname.Text.Trim() + "  " + txtForename.Text.Trim();
            wp.UpdateMode = "I";
            wp.label1.Text = "Document Ref:";
            wp.ShowDialog();
            GetObservation();
            wp = null;
        }
        int gRowIndex = -1;
        private void dgvUObv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int Cnt = dgvUObv.Rows.Count;
            if (Cnt == 0) { return; }
            gRowIndex = dgvUObv.CurrentCell.RowIndex;
            var OBV = dgvUObv[0, dgvUObv.CurrentCell.RowIndex].Value;
            Int64 intObv = Convert.ToInt64(OBV.ToString());
            WordEditor.frmwp wp = new WordEditor.frmwp();
            wp.pbConnectionString = ConnectionString();
            wp.REFID = lblDocNo.Text;
            wp.LinkedDataItem = "Customer";
            wp.UpdateMode = "U";
            wp.ObvID = intObv;
            wp.label10.Text = lblDocNo.Text; ;// txtEmpId.Text.Trim() + "  " + txtSurname.Text.Trim() + "  " + txtForename.Text.Trim();
            wp.LoadObv();
            wp.ShowDialog();
            GetObservation();
            wp = null;
        }

        private void GetObservation()
        {
            try
            {
                string Sql = "Select ObvId, date,rtrim(ltrim(Subject)) as [Subject],rtrim(ltrim(Author)) as [Author] from dbo.observations where Refid=' xxx" + "' order by date DESC ,OBVID";
                SqlConnection con = new SqlConnection(pbConnString);

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

                dgvUObv.DataSource = ds.Tables[0];

                con.Close();

                dgvUObv.RowHeadersVisible = false;
                dgvUObv.MultiSelect = false;
                dgvUObv.EnableHeadersVisualStyles = false;
                dgvUObv.ColumnHeadersDefaultCellStyle.BackColor = Color.Maroon;
                dgvUObv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

                for (int i = 0; i < dgvUObv.ColumnCount; i++)
                {
                    try
                    {
                        this.dgvUObv.Columns[i].ReadOnly = true;
                        this.dgvUObv.Columns[i].DefaultCellStyle.BackColor = Color.White;
                        this.dgvUObv.Columns[i].Width = 75;
                        this.dgvUObv.Columns[i].Resizable = 0;
                        this.dgvUObv.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                        this.dgvUObv.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
                        this.dgvUObv.Columns[i].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    }
                    catch { }
                }
                try
                {
                    this.dgvUObv.Columns[0].Visible = false;
                    this.dgvUObv.Columns[0].Width = 0;
                    this.dgvUObv.Columns[1].Width = 75;
                    this.dgvUObv.Columns[2].Width = 600;
                    this.dgvUObv.Columns[3].Width = 185;



                }
                catch { }

            }
            catch { }
        }
        //========================================================================================================================================================================
    }
}
