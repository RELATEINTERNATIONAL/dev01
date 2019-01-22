using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Text;
//using DATACONNECTION;

namespace WordEditor
{
    public partial class frmwp : Form
    {

        public string pbConnectionString = "";
        public string LinkedDataItem = "";
        public string REFID = "";
        public Int64 ObvID=0;
        public string UserId = "";
        public string UpdateMode = "";

       // DATACONNECTION.DbConnection Dc = new DATACONNECTION.DbConnection();

        public frmwp()
        {
            InitializeComponent();
        }



        private void UpdateRecord()
        {
            bool updFlag = false;
             string Sql ="";
            if (UpdateMode == "I") 
            {
                Sql = "DECLARE @MaxRecId bigint;" + Environment.NewLine;
                Sql = Sql + "set @MaxRecId = (select iif(MAX(ObvID) is null,1, MAX(ObvID)+1) As MaxLineid from dbo.observations) ;";
                Sql = Sql + "insert into dbo.observations  (" + Environment.NewLine;
                Sql = Sql + "[ObvID],[LinkedDataItem],[REFID],[Date],[subject],[Author])" + Environment.NewLine;
                Sql = Sql + "VALUES( @MaxRecId,'"+ LinkedDataItem+ "','"+REFID.Trim()+ "','"+dateTimePicker1.Value.ToString()+ "','"+txtsubject.Text.Trim()+ "','"+txtAuthor.Text.Trim () +"')" + Environment.NewLine;
            
              // Dc.pbConnectionString = pbConnectionString;
              //  updFlag = Dc.DbUpdate(Sql);
                GetlastOBVID();
            
            }

            if (UpdateMode == "U")
            {
                Sql = "Update dbo.observations  SET " + Environment.NewLine;
                 Sql = Sql + "[Date]='"+dateTimePicker1.Text.ToString()+"',"+ Environment.NewLine;;
                 Sql = Sql + "[subject]='" + txtsubject.Text.Trim()+"'";
                 
                 Sql = Sql + " WHERE ObvID ="+ObvID+ Environment.NewLine;
                 Sql = Sql + " AND REFID ='"+REFID.Trim()+"'"+ Environment.NewLine;
                 Sql = Sql + " AND [LinkedDataItem]='" + LinkedDataItem + "'";
               //  Dc.pbConnectionString = pbConnectionString;
              //   updFlag = Dc.DbUpdate(Sql);    
            }

            try
            {
                
                if (updFlag == true)
                {
                    if (richTextBox1.Text.Trim().Length > 0)
                    { UpdateObservation(); }
                }
            }
            catch { }
        }


        private void GetlastOBVID()
        { 

          string Sql="select MAX(ObvID) As MaxLineid from dbo.observations;";
            SqlConnection con = new SqlConnection(pbConnectionString);

            con.Open();

            SqlCommand com = new SqlCommand(Sql, con);

            SqlDataReader read = com.ExecuteReader();
            if (read != null)
            {
                if (read.HasRows)
                {
                    read.Read();

                    ObvID = read.GetInt64(0);
                        
                }
            }



            read.Close();
            con.Close();

        
        
        }

        private void UpdateObservation()
        {
            FileStream stream = null;
            SqlConnection cn = null;
            SqlCommand cmd = null;
            try
            {
                richTextBox1.SaveFile("temp.rtf");
                stream = new FileStream("temp.rtf", FileMode.Open, FileAccess.Read);
                int size = Convert.ToInt32(stream.Length);
                Byte[] rtf = new Byte[size];
                stream.Read(rtf, 0, size);

                cn = new SqlConnection(pbConnectionString);
                cn.Open();

                cmd = new SqlCommand("UPDATE dbo.observations SET observation=@Document WHERE obvId="+ObvID + "AND REFID ='"+REFID+"'", cn);

                SqlParameter paramRTF =
                    new SqlParameter("@Document",
                                                     SqlDbType.VarBinary,
                                                     rtf.Length,
                                                     ParameterDirection.Input,
                                                     false,
                                                     0, 0, null,
                                                     DataRowVersion.Current,
                                                     rtf);
                cmd.Parameters.Add(paramRTF);

                int rowsUpdated = Convert.ToInt32(cmd.ExecuteNonQuery());
                MessageBox.Show(String.Format("{0} rows updated", rowsUpdated));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (null != stream) stream.Close();
                if (null != cmd) cmd.Parameters.Clear();
                if (null != cn) cn.Close();
            }
        
        
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateRecord();

           
        }



        public void LoadObv()
        { 
        
        
        
        richTextBox1.Clear();

			SqlConnection cn = null;
			SqlCommand cmd = null;
			SqlDataReader MyReader = null;
			try
			{
				cn = new SqlConnection(pbConnectionString);
				cn.Open();
				cmd = new SqlCommand("SELECT [ObvID],[LinkedDataItem],[REFID],[Date],[subject],[Author],[Observation] from dbo.observations WHERE obvId="+ObvID + "AND REFID ='"+REFID+"'", cn);
				MyReader = cmd.ExecuteReader();
				MyReader.Read();
				if (MyReader.HasRows)
				{


                    if (!MyReader.IsDBNull(0))
                            {
                            //label24.Text = ReaMyReaderder.GetValue(0).ToString();                                
                            }
                           
                             if (!MyReader.IsDBNull(1))// PCode
                            {
                              ////txtManuAndModel.Text = Reader.GetValue(1).ToString();
                            }

                             if (!MyReader.IsDBNull(2))// PCode
                             {
                                 //txtRegistrationNo.Text = MyReader.GetValue(2).ToString();
                             }

                            if (!MyReader.IsDBNull(3))// PCode
                            {
                             dateTimePicker1.Text  = MyReader.GetValue(3).ToString();
                            }

                            if (!MyReader.IsDBNull(4))// PCode
                            {
                             txtsubject.Text = MyReader.GetValue(4).ToString();
                            }

                            if (!MyReader.IsDBNull(5))// PCode
                            {
                             txtAuthor.Text = MyReader.GetValue(5).ToString();
                            }
                    


					if (!MyReader.IsDBNull(6))
					{
						Byte[] rtf = new Byte[Convert.ToInt32((MyReader.GetBytes(6, 0, null, 0, Int32.MaxValue)))];
						long bytesReceived = MyReader.GetBytes(6, 0, rtf, 0, rtf.Length);

						ASCIIEncoding encoding = new ASCIIEncoding();
						richTextBox1.Rtf = encoding.GetString(rtf, 0, Convert.ToInt32(bytesReceived));
					}
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				if (null != MyReader) MyReader.Close();
				if (null != cn) cn.Close();
			}
		 
        
        }
        private void frmwp_Load(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
          richTextBox1.Clear();

			SqlConnection cn = null;
			SqlCommand cmd = null;
			SqlDataReader reader = null;
			try
			{
				cn = new SqlConnection(pbConnectionString);
				cn.Open();
				cmd = new SqlCommand("SELECT observation FROM dbo.Observations WHERE OBVID=1", cn);
				reader = cmd.ExecuteReader();
				reader.Read();
				if (reader.HasRows)
				{
					if (!reader.IsDBNull(0))
					{
						Byte[] rtf = new Byte[Convert.ToInt32((reader.GetBytes(0, 0, null, 0, Int32.MaxValue)))];
						long bytesReceived = reader.GetBytes(0, 0, rtf, 0, rtf.Length);

						ASCIIEncoding encoding = new ASCIIEncoding();
						richTextBox1.Rtf = encoding.GetString(rtf, 0, Convert.ToInt32(bytesReceived));
					}
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				if (null != reader) reader.Close();
				if (null != cn) cn.Close();
			}
		}

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
             richTextBox1.Redo();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Copy();
        }

        private void paseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectAll();
        }

        private void wordWarpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.WordWrap = true;
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Get the characteristics of the selected text
            // Apply them to the Font dialog box
            dlgFont.Font = richTextBox1.SelectionFont;
            dlgFont.Color = richTextBox1.SelectionColor;

            if (dlgFont.ShowDialog() == DialogResult.OK)
            {
                // Display the Font dialog box
                // If the user clicks OK, get the characteristics of the font
                // Apply them to the selected text of the Rich Edit control
                richTextBox1.SelectionFont = dlgFont.Font;
                richTextBox1.SelectionColor = dlgFont.Color;
            }
        }

        private void alignLeftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectionAlignment = HorizontalAlignment.Left;

        }

        private void alignCenterToolStripMenuItem_Click(object sender, EventArgs e)
        {
              richTextBox1.SelectionAlignment = HorizontalAlignment.Center;

        }

        private void alignRightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectionAlignment = HorizontalAlignment.Right;
        }

        private void leftIndentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectionIndent += 10;
        }

        private void rightIndentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectionRightIndent += 10;
        }

        private void bulletListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            richTextBox1.SelectionBullet = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.OnPrintPage);

            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.DefaultPageSettings.PrinterSettings.PrintToFile = true;

                printDocument1.Print();
            }
        }

        private int linesPrinted;
        private string[] lines;

        private void OnPrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            int x = 5;// e.MarginBounds.Left;
            int y = 5;//e.MarginBounds.Top;
            Brush brush = new SolidBrush(richTextBox1.ForeColor);

            while (linesPrinted < lines.Length)
            {
                e.Graphics.DrawString(lines[linesPrinted++],
                    richTextBox1.Font, brush, x, y);
                y += 15;
                if (y >= e.MarginBounds.Bottom)
                {
                    e.HasMorePages = true;
                    return;
                }
            }

            linesPrinted = 0;
            e.HasMorePages = false;
        }

        private void printDocument1_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {
            char[] param = { '\n' };

            if (printDialog1.PrinterSettings.PrintRange == System.Drawing.Printing.PrintRange.Selection)
            {
                lines = richTextBox1.SelectedText.Split(param);
            }
            else
            {
                lines = richTextBox1.Text.Split(param);
            }

            int i = 0;
            char[] trimParam = { '\r' };
            foreach (string s in lines)
            {
                lines[i++] = s.TrimEnd(trimParam);
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void prinrToolStripMenuItem_Click(object sender, EventArgs e)
        {

            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.OnPrintPage);

            if (printDialog1.ShowDialog() == DialogResult.OK)
            {
                printDocument1.DefaultPageSettings.PrinterSettings.PrintToFile = true;

                printDocument1.Print();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UpdateRecord();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // Get the characteristics of the selected text
            // Apply them to the Font dialog box
            //dlgFont.Font = richTextBox1.SelectionFont;
           // dlgFont.Color = richTextBox1.SelectionColor;

            

        }

        private void fontColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                richTextBox1.SelectionColor = colorDialog1.Color;
            }

        }
        
    }
}
