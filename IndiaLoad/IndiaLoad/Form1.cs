using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using ICSharpCode.SharpZipLib.Zip;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using IndiaLoad.Properties;

namespace IndiaLoad
{
    public partial class Form1 : Form
    {
        static private string ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1550)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=chemdb.prod-wos.com)));";
        static private Properties.Settings settings = Properties.Settings.Default;

        static private string icpdnConnectionString = ConnectionString + "User Id=icpdn;Password=icpdn;";
        static private string ccrpdnConnectionString = ConnectionString + "User Id=ccrpdn;Password=ccrpdn;";
        static private string icConnectionString = ConnectionString + "User Id=ic;Password=ic;";
        static private string ccrConnectionString = ConnectionString + "User Id=ccr;Password=ccr;";

        static private string sRxn = "";
        static private string sRxc = "";
        static private string sRxm = "";
        static private string sMol = "";
        static private string sSub = "";

        static private int nRxn = 0;
        static private int nRxc = 0;
        static private int nRxm = 0;
        static private int nMol = 0;

        static private OracleConnection OraConn = new OracleConnection();
        static private OracleConnection OraCon1 = new OracleConnection();
        static private OracleCommand cmd = new OracleCommand();

        static private DataTable msgTable = new DataTable("message");
        static private DataTable brfTable = new DataTable("brf");
        static private DataTable dupTable = new DataTable("dup");
        static private DataTable nsrTable = new DataTable("nos");
        static private DataTable rptTable = new DataTable("rpt");

        public Form1()
        {
            InitializeComponent();

            string host = settings.host;
            string port = settings.port;
            string service = settings.service;

            textBox2.Text = host;
            textBox3.Text = port;
            textBox4.Text = service;

            if (host.Length * port.Length * service.Length > 0)
            {
                ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + host + ")(PORT=" + port + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + service + ")));";
            }

            icpdnConnectionString = ConnectionString + "User Id=icpdn;Password=icpdn;";
            ccrpdnConnectionString = ConnectionString + "User Id=ccrpdn;Password=ccrpdn;";
            icConnectionString = ConnectionString + "User Id=ic;Password=ic;";
            ccrConnectionString = ConnectionString + "User Id=ccr;Password=ccr;";


            panel1.Visible = false;


            MainMenu MyMenu = new MainMenu();

            MenuItem m1 = new MenuItem("File");
            MyMenu.MenuItems.Add(m1);

            MenuItem subm1 = new MenuItem("Editing");
            m1.MenuItems.Add(subm1);

            MenuItem subm2 = new MenuItem("Database Configuration");
            m1.MenuItems.Add(subm2);

            MenuItem subm3 = new MenuItem("Exit");
            m1.MenuItems.Add(subm3);

            subm1.Click += new EventHandler(MMEditClick);
            subm2.Click += new EventHandler(MMConfigClick);
            subm3.Click += new EventHandler(MMExitClick);

            Menu = MyMenu;
        }

        protected void MMEditClick(object who, EventArgs e)
        {
            panel1.Visible = false;
        }

        protected void MMConfigClick(object who, EventArgs e)
        {
            panel1.Visible = true;
        }

        protected void MMExitClick(object who, EventArgs e)
        {
            Application.Exit();
        }


        private void button5_Click_1(object sender, EventArgs e)
        {
            string host = textBox2.Text;
            string port = textBox3.Text;
            string service = textBox4.Text;

            if (host.Length * port.Length * service.Length > 0)
            {
                settings.host = host;
                settings.port = port;
                settings.service = service;
                settings.Save();

                ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + host + ")(PORT=" + port + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + service + ")));";
                icpdnConnectionString = ConnectionString + "User Id=icpdn;Password=icpdn;";
                ccrpdnConnectionString = ConnectionString + "User Id=ccrpdn;Password=ccrpdn;";
                icConnectionString = ConnectionString + "User Id=ic;Password=ic;";
                ccrConnectionString = ConnectionString + "User Id=ccr;Password=ccr;";
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            button6.Visible = false;
            label2.Visible = false;
            label3.Visible = false;
            progressBar1.Visible = false;
            progressBar2.Visible = false;


            msgTable.Rows.Clear();
            nsrTable.Rows.Clear();
            dupTable.Rows.Clear();
            brfTable.Rows.Clear();
            rptTable.Rows.Clear();
            dataGridView1.Visible = false;
            dataGridView2.Visible = false;
            dataGridView3.Visible = false;
            dataGridView4.Visible = false;

            string indiapath = "";
            string batch = "";
            if (button1.Text == "Get Current Domex Files")
            {
                string sserver = "ftp://aws-isigate.isinet.com/to_isi/";
                string lstdir = "";

                FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(sserver));

                try
                {
                    reqFTP.Credentials = new NetworkCredential("imgindia", "imgpwd");
                    reqFTP.Proxy = null;
                    reqFTP.EnableSsl = false;
                    reqFTP.UseBinary = false;
                    reqFTP.Timeout = 600000;
                    reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    Stream ftpStream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(ftpStream);

                    lstdir = reader.ReadToEnd();

                    ftpStream.Close();
                    response.Close();
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                MessageBox.Show(lstdir);
                int diridx = lstdir.IndexOf("ESTIMATE-");

                string sbatch = lstdir.Substring(diridx + 9, 6);
                batch = sbatch;

                string locday = System.DateTime.Today.ToString("MMdd-") + sbatch.Substring(0, 4) + "-" + sbatch.Substring(4);

                //string locdir = "I:\\users\\shared\\domex\\" + locday + "\\";

                string locdir = "\\\\34.212.24.69\\chem\\domex\\" + locday + "\\";
                

                DirectoryInfo di = Directory.CreateDirectory(locdir);

                try
                {
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(sserver + "ESTIMATE-" + sbatch + ".xls"));
                    reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                    reqFTP.Credentials = new NetworkCredential("imgindia", "imgpwd");
                    reqFTP.Proxy = null;
                    //reqFTP.EnableSsl = false;
                    reqFTP.UseBinary = true;
                    reqFTP.UsePassive = false;
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    Stream ftpStream = response.GetResponseStream();

                    FileStream outputStream = new FileStream(locdir + "ESTIMATE-" + sbatch + ".xls", FileMode.Create);

                    int bufferSize = 2048 * 10;
                    int readCount;
                    byte[] buffer = new byte[bufferSize];

                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        outputStream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                    }

                    ftpStream.Close();
                    outputStream.Close();
                    response.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }



                try
                {
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(sserver + sbatch + "_isis.zip"));
                    reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                    reqFTP.Credentials = new NetworkCredential("imgindia", "imgpwd");
                    reqFTP.Proxy = null;
                    reqFTP.EnableSsl = false;
                    reqFTP.UseBinary = true;
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    Stream ftpStream = response.GetResponseStream();

                    FileStream outputStream = new FileStream(locdir + sbatch + "_isis.zip", FileMode.Create);

                    int bufferSize = 2048 * 10;
                    int readCount;
                    byte[] buffer = new byte[bufferSize];

                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        outputStream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                    }

                    ftpStream.Close();
                    outputStream.Close();
                    response.Close();

                    FastZip fz = new FastZip();
                    string zipFile = locdir + sbatch + "_isis.zip";
                    string targetDirectory = locdir;

                    fz.ExtractZip(zipFile, targetDirectory, "");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


                try
                {
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(sserver + "ICIndiardf.zip"));
                    reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                    reqFTP.Credentials = new NetworkCredential("imgindia", "imgpwd");
                    reqFTP.Proxy = null;
                    reqFTP.EnableSsl = false;
                    reqFTP.UseBinary = true;
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    Stream ftpStream = response.GetResponseStream();

                    FileStream outputStream = new FileStream(locdir + "ICIndiardf.zip", FileMode.Create);

                    int bufferSize = 2048 * 16;
                    int readCount;
                    byte[] buffer = new byte[bufferSize];

                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        outputStream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                    }

                    ftpStream.Close();
                    outputStream.Close();
                    response.Close();

                    FastZip fz = new FastZip();
                    string zipFile = locdir + "ICIndiardf.zip";
                    string targetDirectory = locdir;

                    fz.ExtractZip(zipFile, targetDirectory, "");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


                try
                {
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(sserver + "rxn.zip"));
                    reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                    reqFTP.Credentials = new NetworkCredential("imgindia", "imgpwd");
                    reqFTP.Proxy = null;
                    reqFTP.EnableSsl = false;
                    reqFTP.UseBinary = true;
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    Stream ftpStream = response.GetResponseStream();

                    FileStream outputStream = new FileStream(locdir + "rxn.zip", FileMode.Create);

                    int bufferSize = 2048 * 16;
                    int readCount;
                    byte[] buffer = new byte[bufferSize];

                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        outputStream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                    }

                    ftpStream.Close();
                    outputStream.Close();
                    response.Close();

                    FastZip fz = new FastZip();
                    string zipFile = locdir + "rxn.zip";
                    string targetDirectory = locdir;

                    fz.ExtractZip(zipFile, targetDirectory, "");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


                try
                {
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(sserver + "rxc.zip"));
                    reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                    reqFTP.Credentials = new NetworkCredential("imgindia", "imgpwd");
                    reqFTP.Proxy = null;
                    reqFTP.EnableSsl = false;
                    reqFTP.UseBinary = true;
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    Stream ftpStream = response.GetResponseStream();

                    FileStream outputStream = new FileStream(locdir + "rxc.zip", FileMode.Create);

                    int bufferSize = 2048 * 16;
                    int readCount;
                    byte[] buffer = new byte[bufferSize];

                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        outputStream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                    }

                    ftpStream.Close();
                    outputStream.Close();
                    response.Close();

                    FastZip fz = new FastZip();
                    string zipFile = locdir + "rxc.zip";
                    string targetDirectory = locdir;

                    fz.ExtractZip(zipFile, targetDirectory, "");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


                try
                {
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(sserver + "rxm.zip"));
                    reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                    reqFTP.Credentials = new NetworkCredential("imgindia", "imgpwd");
                    reqFTP.Proxy = null;
                    reqFTP.EnableSsl = false;
                    reqFTP.UseBinary = true;
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    Stream ftpStream = response.GetResponseStream();

                    FileStream outputStream = new FileStream(locdir + "rxm.zip", FileMode.Create);

                    int bufferSize = 2048 * 16;
                    int readCount;
                    byte[] buffer = new byte[bufferSize];

                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        outputStream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                    }

                    ftpStream.Close();
                    outputStream.Close();
                    response.Close();

                    FastZip fz = new FastZip();
                    string zipFile = locdir + "rxm.zip";
                    string targetDirectory = locdir;

                    fz.ExtractZip(zipFile, targetDirectory, "");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


                try
                {
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(sserver + "report.zip"));
                    reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                    reqFTP.Credentials = new NetworkCredential("imgindia", "imgpwd");
                    reqFTP.Proxy = null;
                    reqFTP.EnableSsl = false;
                    reqFTP.UseBinary = true;
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    Stream ftpStream = response.GetResponseStream();

                    FileStream outputStream = new FileStream(locdir + "report.zip", FileMode.Create);

                    int bufferSize = 2048 * 16;
                    int readCount;
                    byte[] buffer = new byte[bufferSize];

                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        outputStream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                    }

                    ftpStream.Close();
                    outputStream.Close();
                    response.Close();

                    FastZip fz = new FastZip();
                    string zipFile = locdir + "report.zip";
                    string targetDirectory = locdir;

                    fz.ExtractZip(zipFile, targetDirectory, "");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


                try
                {
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(sserver + "imgann.zip"));
                    reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                    reqFTP.Credentials = new NetworkCredential("imgindia", "imgpwd");
                    reqFTP.Proxy = null;
                    reqFTP.EnableSsl = false;
                    reqFTP.UseBinary = true;
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    Stream ftpStream = response.GetResponseStream();

                    FileStream outputStream = new FileStream(locdir + "imgann.zip", FileMode.Create);

                    int bufferSize = 2048 * 16;
                    int readCount;
                    byte[] buffer = new byte[bufferSize];

                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        outputStream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                    }

                    ftpStream.Close();
                    outputStream.Close();
                    response.Close();

                    FastZip fz = new FastZip();
                    string zipFile = locdir + "imgann.zip";
                    string targetDirectory = "\\\\34.212.24.69\\chem\\images";

                    fz.ExtractZip(zipFile, targetDirectory, "");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


                try
                {
                    reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(sserver + "SubjectIndex.dat"));
                    reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;
                    reqFTP.Credentials = new NetworkCredential("imgindia", "imgpwd");
                    reqFTP.Proxy = null;
                    reqFTP.EnableSsl = false;
                    reqFTP.UseBinary = false;
                    FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                    Stream ftpStream = response.GetResponseStream();

                    FileStream outputStream = new FileStream(locdir + "SubjectIndex.dat", FileMode.Create);

                    int bufferSize = 2048 * 16;
                    int readCount;
                    byte[] buffer = new byte[bufferSize];

                    readCount = ftpStream.Read(buffer, 0, bufferSize);
                    while (readCount > 0)
                    {
                        outputStream.Write(buffer, 0, readCount);
                        readCount = ftpStream.Read(buffer, 0, bufferSize);
                    }

                    ftpStream.Close();
                    outputStream.Close();
                    response.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


                indiapath = locdir;

            }
            else if (button1.Text == "Choose Current Domex Folder")
            {
                OpenFileDialog ofd1 = new OpenFileDialog();

                ofd1.Title = "Select .rxn file from current India folder:";
                ofd1.Filter = "Reaction Files|*.rdf";
                ofd1.InitialDirectory = "\\\\34.212.24.69\\chem\\images";

                if (ofd1.ShowDialog() != DialogResult.OK) return;
                label1.Text = "";

                this.Cursor = Cursors.WaitCursor;

                indiapath = ofd1.FileName;
                int lidx = indiapath.LastIndexOf("\\");
                indiapath = indiapath.Substring(0, lidx + 1);

                int i = 0, j = 0;

                i = indiapath.IndexOf("domex\\", 1);
                if (i < 0) return;
                i = indiapath.IndexOf("-", i);
                j = indiapath.IndexOf("-", i + 1);
                int k = indiapath.IndexOf("\\", j);

                batch = indiapath.Substring(i + 1, j - i - 1) + indiapath.Substring(j + 1, 2);

            }

            label1.Text = "Files Found in Directory " + indiapath + ":";
            listBox1.Items.Clear();

            try
            {
                StreamReader SR = File.OpenText(indiapath + "\\" + batch + "rxn.rdf");
                sRxn = SR.ReadToEnd();
                SR.Close();
                
                listBox1.Items.Add(batch + "rxn.rdf");
            }
            catch (IOException exp)
            {
                MessageBox.Show(exp.ToString());
                return;
            }

            try
            {
                StreamReader SR = File.OpenText(indiapath + "\\" + batch + "rxm.rdf");
                sRxm = SR.ReadToEnd();
                SR.Close();

                listBox1.Items.Add(batch + "rxm.rdf");
            }
            catch (IOException exp)
            {
                sRxm = "";
            }

            try
            {
                StreamReader SR = File.OpenText(indiapath + "\\" + batch + "rxc.rdf");
                sRxc = SR.ReadToEnd();
                SR.Close();

                listBox1.Items.Add(batch + "rxc.rdf");
            }
            catch (IOException exp)
            {
                sRxc = "";
            }


            try
            {
                StreamReader SR = File.OpenText(indiapath + "\\" + "ICIndia.rdf");

                //StreamReader SR = File.OpenText("Y:\\users\\shared\\domex\\0409-2015-20\\ICIndia.rdf");
                sMol = SR.ReadToEnd();
                SR.Close();

                listBox1.Items.Add("ICIndia.rdf");
            }
            catch (IOException exp)
            {
                sMol = "";
            }
            

            try
            {
                StreamReader SR = File.OpenText(indiapath + "\\" + "SubjectIndex.dat");
                sSub = SR.ReadToEnd();
                SR.Close();

                listBox1.Items.Add("SubjectIndex.dat");
            }
            catch (IOException exp)
            {
                sSub = "";
            }

            rptTable.Clear();

            try
            {
                String exfile = indiapath + "\\REPORT-" + batch + ".xls";
                String sConnectionString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + exfile + ";" + "Extended Properties=\"Excel 8.0;HDR=No;IMEX=1;\";";
                OleDbConnection objConn = new OleDbConnection(sConnectionString);

                objConn.Open();
                DataTable dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);   

                String pgname = dt.Rows[0]["TABLE_NAME"].ToString();
                
                OleDbCommand objCmdSelect = new OleDbCommand("SELECT * FROM [" + pgname + "]", objConn);
                OleDbDataAdapter objAdapter1 = new OleDbDataAdapter();
                objAdapter1.SelectCommand = objCmdSelect;
                objAdapter1.Fill(rptTable);
                 
                objConn.Close();

                listBox1.Items.Add("REPORT-" + batch + ".xls");
            }
            catch (IOException exp)
            {
                ;
            }



            button7.Visible = true;
            button2.Visible = true;

            this.Cursor = Cursors.Default;

        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            button7.Visible = false;
            button2.Visible = false;
            label2.Visible = true;
            label2.Text = "Loading to IC Weekly........";
            if (sMol.Length > 0)
            {
                progressBar1.Visible = true;
                insic(sMol);
            }

            this.Cursor = Cursors.Default;
            button2.Visible = !(progressBar2.Visible);

            if (rptTable.Rows.Count > 0)
            {
                insrpt();
            }

        }



        private void insrpt()
        {
            string srpt = "";
            string bat = rptTable.Rows[0][0].ToString().Trim();
            for (int i = 0; i < rptTable.Rows.Count; i++)
            {
                if (rptTable.Rows[i][1].ToString().Trim().Length == 5)
                {
                    string rlin = "\t" + bat;

                    for (int j = 0; j < 12; j++)
                    {
                        rlin = rlin + "\t" + rptTable.Rows[i][j].ToString().Trim();
                    }

                    srpt = srpt + rlin + "\t\t\t\t\t\r\n";
                }
            }


            OraCon1.ConnectionString = icConnectionString;

            try
            {
                OraCon1.Open();
                cmd.Connection = OraCon1;

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("clobtodb", OracleDbType.Clob).Value = srpt + "\r\n";
                cmd.CommandText = "indiarptProc";

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                cmd.Parameters.Clear();
                OraCon1.Close();
                MessageBox.Show(ex.ToString());

                return;
            }

            cmd.Parameters.Clear();
            OraCon1.Close();
        }



        private void insic(string S)
        {
            
            int idx1 = 0, idx2 = 0, idx = 0;
            int mireg = -1;
            int mmreg = 0;
            string s1 = "", s2 = "";
            string ml2db = "";

            
            OraCon1.ConnectionString = icConnectionString;

            progressBar1.Value = 10;

            try
            {
                OraCon1.Open();
                cmd.Connection = OraCon1;

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "cleanICMolProc";

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                OraCon1.Close();
                return;
            }
            OraCon1.Close();
            progressBar1.Value = 20;
            
            idx2 = S.IndexOf("$MFMT $MIREG", 0);
            while (idx2 > -1)
            {
                
                idx1 = idx2;
                idx2 = S.IndexOf("$MFMT $MIREG", idx1 + 1);

                if (idx2 > idx1)
                {
                    s1 = S.Substring(idx1, idx2 - idx1);
                }
                else
                {
                    s1 = S.Substring(idx1);
                }

                idx = s1.IndexOf("\n");
                try
                {
                    mireg = Convert.ToInt32(s1.Substring(12, idx - 12));
                }
                catch (Exception e)
                {
                    ;
                }

                if (mireg % 100 == 0)
                {
                    progressBar1.Value = (progressBar1.Value+1)%100;
                    Refresh();
                    Application.DoEvents();

                }
                {
                    insert2ic(ml2db);
                    ml2db = "";
                }

                s1 = s1.Substring(idx + 1);

                idx = s1.IndexOf("$DTYPE", 0);
                s2 = s1.Substring(idx);
                s1 = s1.Substring(0, idx);

                string acsn = getfdata(s2, "$DTYPE MOL:ACCESN.NO");
                string artl = getfdata(s2, "$DTYPE MOL:ARTL.NO");
                string auth = getfdata(s2, "$DTYPE MOL:AUTH.NO");
                string batc = getfdata(s2, "$DTYPE MOL:BATCH_NO");
                string grad = getfdata(s2, "$DTYPE MOL:GRADE");

                ml2db = ml2db + "\tMOLECULE\t" + mireg + "\t" + acsn + "\t" + artl + "\t" + auth + "\t" + batc + "\t" + grad + "\t" + s1 + "\t\t\t\t\t\r\n";

                int i = 1;
                string rva = "$DTYPE MOL:IC_SYMBOL(";
                int st = s2.IndexOf(rva + i + "):SYMBOL", 0);
                while (st > -1)
                {
                    ml2db = ml2db + "\tSYMBOL\t" + mireg + "\t" + i + "\t" + getfdata(s2, rva + i + "):SYMBOL") + "\t\t\t\t\t\r\n";

                    i++;
                    st = s2.IndexOf(rva + i + "):SYMBOL", 0);
                }

                i = 1;
                rva = "$DTYPE MOL:IC_BIOLACT(";
                st = s2.IndexOf(rva + i + "):BA_CODE", 0);
                while (st > -1)
                {
                    string bacd = getfdata(s2, rva + i + "):BA_CODE");
                    string bast = getfdata(s2, rva + i + "):STATUS");
                    ml2db = ml2db + "\tBIOLACT\t" + mireg + "\t" + i + "\t" + bacd + "\t" + bast + "\t\t\t\t\t\r\n";

                    i++;
                    st = s2.IndexOf(rva + i + "):BA_CODE", 0);
                }

                i = 1;
                rva = "$DTYPE MOL:IC_CPD_CLASS(";
                st = s2.IndexOf(rva + i + "):CLASS_ID", 0);
                while (st > -1)
                {
                    string bacd = getfdata(s2, rva + i + "):CLASS_ID");
                    ml2db = ml2db + "\tCPDCLASS\t" + mireg + "\t" + i + "\t" + bacd + "\t\t\t\t\t\r\n";

                    i++;
                    st = s2.IndexOf(rva + i + "):CLASS_ID", 0);
                }

                i = 1;
                rva = "$DTYPE MOL:IC_USE_PROFILE(";
                st = s2.IndexOf(rva + i + "):PROFILE_ID", 0);
                while (st > -1)
                {
                    string bacd = getfdata(s2, rva + i + "):PROFILE_ID");
                    ml2db = ml2db + "\tPROFILE\t" + mireg + "\t" + i + "\t" + bacd + "\t\t\t\t\t\r\n";

                    i++;
                    st = s2.IndexOf(rva + i + "):PROFILE_ID", 0);
                }
                label4.Text = "mireg: " + mireg;
                Refresh();

                if (mireg % 100 == 0)
                {
                    insert2ic(ml2db);
                    ml2db = "";
                }
            }

            insert2ic(ml2db);
            progressBar1.Value = 100;
            Refresh();

            label2.Text = "Loading to ICPDN........";
            progressBar1.Value = 10;
            Refresh();
            OraCon1.ConnectionString = icpdnConnectionString;
            try
            {
                OraCon1.Open();
                cmd.Connection = OraCon1;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "dupchkload";

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraCon1.Close();
                return;
            }
            OraCon1.Close();

            label2.Text = "\r\nUpdate Fastsearch Index........";
            progressBar1.Value = 60;
            Refresh();
            try
            {
                OraCon1.Open();
                cmd.Connection = OraCon1;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "FASTSRCHUPDATE";

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraCon1.Close();
                return;
            }
            OraCon1.Close();

            label2.Text = label2.Text = "Load Subject Index........";
            progressBar1.Value = 90;
            try
            {
                OraCon1.Open();
                cmd.Connection = OraCon1;

                cmd.Parameters.Add("clobtodb", OracleDbType.Clob).Value = sSub + "\r\n";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "INDIASUBIDXLOAD";

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                cmd.Parameters.Clear();
                OraCon1.Close();
                MessageBox.Show(ex.ToString());
                
            }
            cmd.Parameters.Clear();
            OraCon1.Close();


            label2.Text = label2.Text = "IC Load finished.";
            progressBar1.Value = 100;

        }


        private void insert2ic(string s)
        {
            OraCon1.ConnectionString = icConnectionString;

            try
            {
                OraCon1.Open();
                cmd.Connection = OraCon1;

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("clobtodb", OracleDbType.Clob).Value = s;
                cmd.CommandText = "indiaICMolProc";

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                cmd.Parameters.Clear();
                OraCon1.Close();
                MessageBox.Show(ex.ToString());

                return;
            }

            cmd.Parameters.Clear();
            OraCon1.Close();
        }



        private string getfdata(string s, string stp)
        {
            string gdata = "";
            int rf = s.IndexOf(stp, 0);

            if (rf > -1)
            {
                int st = s.IndexOf("$DATUM", rf);
                int nd = s.IndexOf("$DTYPE", st);
                if (nd == -1) nd = s.Length - 1;

                string rdata = s.Substring(st, nd - st);

                StringReader sr = new StringReader(rdata);

                string slin = sr.ReadLine();

                while (slin != null)
                {
                    if (slin.Length > 80) slin = slin.Substring(0, 80);
                    gdata = gdata + slin;
                    slin = sr.ReadLine();
                }

                gdata = gdata.Substring(7);
            }

            return gdata.Trim();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            String icpdnConString = "";
            String ccrpdnConString = "";
            String icConString = "";
            String ccrConString = "";

            try
            {
                String connection_config = "connection_dontdelete.ora";
                StreamReader SR = File.OpenText(connection_config);
                String conString = SR.ReadToEnd();
                SR.Close();

                int idx = conString.IndexOf("\r\nicpdn\t");
                if (idx > -1) icpdnConString = conString.Substring(idx + 8);
                idx = icpdnConString.IndexOf("\r\n");
                if (idx > -1) icpdnConString = icpdnConString.Substring(0, idx);

                idx = conString.IndexOf("\r\nccrpdn\t");
                if (idx > -1) ccrpdnConString = conString.Substring(idx + 9);
                idx = ccrpdnConString.IndexOf("\r\n");
                if (idx > -1) ccrpdnConString = ccrpdnConString.Substring(0, idx);

                idx = conString.IndexOf("\r\nic\t");
                if (idx > -1) icConString = conString.Substring(idx + 5);
                idx = icConString.IndexOf("\r\n");
                if (idx > -1) icConString = icConString.Substring(0, idx);

                idx = conString.IndexOf("\r\nccr\t");
                if (idx > -1) ccrConString = conString.Substring(idx + 6);
                idx = ccrConString.IndexOf("\r\n");
                if (idx > -1) ccrConString = ccrConString.Substring(0, idx);


            }
            catch (Exception ee)
            {
                ;
            }

            if (icpdnConString != "") icpdnConnectionString = icpdnConString;
            if (ccrpdnConString != "") ccrpdnConnectionString = ccrpdnConString;
            if (icConString != "") icConnectionString = icConString;
            if (ccrConString != "") ccrConnectionString = ccrConString;




            label1.Text = "";
            label2.Text = "";
            label3.Text = "";

            DataColumn myDataColumn = new DataColumn("Object", System.Type.GetType("System.String"));
            msgTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("In File", System.Type.GetType("System.Decimal"));
            msgTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("Loaded", System.Type.GetType("System.Decimal"));
            msgTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("No Structure", System.Type.GetType("System.Decimal"));
            msgTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("Duplicate Molecule", System.Type.GetType("System.Decimal"));
            msgTable.Columns.Add(myDataColumn);


        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            progressBar2.Visible = true;
            button2.Visible = false;
            button7.Visible = false;
            label3.Visible = true;

            rxnload();



            this.Cursor = Cursors.Default;

            button7.Visible = !(progressBar1.Visible);
            button6.Visible = true;
        }


        private void rxnload()
        {
            label3.Text = "Cleaning up Oracle database....";
            progressBar2.Value = 5;
            Refresh();

            try
            {
                OraConn.ConnectionString = ccrConnectionString;
                OraConn.Open();
                cmd.Connection = OraConn;

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "batch_cleanup";

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                OraConn.Close();
            }
            OraConn.Close();
            this.Cursor = Cursors.Default;
            progressBar2.Value = 10;
            Refresh();
            Application.DoEvents();

            this.Cursor = Cursors.WaitCursor;

            sendrxn();
            this.Cursor = Cursors.Default;
            progressBar2.Value = 80;
            Refresh();
            Application.DoEvents();

            this.Cursor = Cursors.WaitCursor;
            loadstatistics();

            this.Cursor = Cursors.Default;
            progressBar2.Value = 100;
            Refresh();
            label3.Text = "CCR Load Finished";

        }


        private string insert2db(string s)
        {
            try
            {
                OraConn.ConnectionString = ccrConnectionString;
                OraConn.Open();
                cmd.Connection = OraConn;

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("clobtodb", OracleDbType.Clob).Value = s;
                cmd.CommandText = "indiaLoadProc";

                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                cmd.Parameters.Clear();
                OraConn.Close();
                return ex.ToString();
            }

            cmd.Parameters.Clear();
            OraConn.Close();
            return "Inserted!";
        }





        private void sendrxn()
        {
            label3.Text = "Loading Catalysts and Solvents........";
            Refresh();

            string msg = insert2db(inscat(sRxc));

            if (msg.IndexOf("onnection", 0) > -1)
            {
                MessageBox.Show(msg);
                label3.Text = "Can't connect to Oracle!";

                return;
            }
            progressBar2.Value = 20;
            label3.Text = "Loading Molecules..........";
            Refresh();
            msg = insert2db(insmol(sRxm));

            if (msg.IndexOf("onnection", 0) > -1)
            {
                MessageBox.Show(msg);
                label2.Text = "Can't connect to Oracle!";

                return;
            }
            progressBar2.Value = 30;
            label3.Text = "Loading Reactions..........";
            Refresh();

            msg = insert2db(insrxn(sRxn));

            if (msg.IndexOf("onnection", 0) > -1)
            {
                MessageBox.Show(msg);
                label2.Text = "Can't connect to Oracle!";

                return;
            }

            progressBar2.Value = 70;
            Refresh();
        }


        private string insrxn(string S)
        {
            string rxn2db = "";
            nRxn = 0;

            int idx1 = 0, idx2 = 0, idx = 0;
            int rireg = -1;
            string s1 = "", s2 = "";

            idx2 = S.IndexOf("$RFMT $RIREG", 0);

            while (idx2 > -1)
            {
                nRxn++;
                idx1 = idx2;
                idx2 = S.IndexOf("$RFMT $RIREG", idx1 + 1);

                if (idx2 > idx1)
                {
                    s1 = S.Substring(idx1, idx2 - idx1);
                }
                else
                {
                    s1 = S.Substring(idx1);
                }

                StringReader sr = new StringReader(s1);
                string slin = sr.ReadLine();

                try
                {
                    rireg = Convert.ToInt32(slin.Substring(12));
                }
                catch (Exception e)
                {
                    ;
                }

                s1 = s1.Substring(idx + 1);

                idx = s1.IndexOf("$DTYPE", 0);
                s2 = s1.Substring(idx);
                s1 = s1.Substring(0, idx);

                idx = s1.IndexOf("$RXN", 0);
                s1 = s1.Substring(idx);

                int i = 1;
                string rline = "";
                string acsn = getfdata(s2, "RXN:VARIATION(1):CCRREF(" + i + "):ACCESN.NO");
                while (acsn.Length > 0)
                {
                    string artl = getfdata(s2, "RXN:VARIATION(1):CCRREF(" + i + "):ARTL.NO");
                    string path = getfdata(s2, "RXN:VARIATION(1):CCRREF(" + i + "):PATH");
                    string step = getfdata(s2, "RXN:VARIATION(1):CCRREF(" + i + "):STEP");
                    string nsm = getfdata(s2, "RXN:VARIATION(1):CCRREF(" + i + "):NSM");

                    if( step.Length >8)
                    {
                        MessageBox.Show(step);
                    }

                    rline = "\tCCRREF\t" + getfdata(s2, "RXN:VARIATION(1):BATCHNO") + "\t" + acsn + "\t" + artl + "\t" + path + "\t" + step + "\t" + nsm + "\t" + rireg + "\t\t\t\t\t\r\n";
                    rxn2db = rxn2db + rline;

                    i++;
                    acsn = getfdata(s2, "RXN:VARIATION(1):CCRREF(" + i + "):ACCESN.NO");
                }


                rline = "\tREACTION\t" + rireg + "\t" + getfdata(s2, "RXN:VARIATION(1):BATCHNO") + "\t" + getfdata(s2, "RXN:VARIATION(1):KEYPHRASES") + "\t" + s1 + "\t\t\t\t\t\r\n";
                rxn2db = rxn2db + rline;


                i = 1;
                string cdata = getfdata(s2, "RXN:VARIATION(1):COMMENTS(" + i + ")");
                while (cdata.Length > 0)
                {
                    rline = "\tCOMMENT\t" + i + "\t" + cdata + "\t\t\t\t\t\r\n";
                    rxn2db = rxn2db + rline;

                    i++;
                    cdata = getfdata(s2, "RXN:VARIATION(1):COMMENTS(" + i + ")");
                }


                i = 1;
                cdata = getfdata(s2, "RXN:VARIATION(1):PROD.CODE(" + i + ")");
                while (cdata.Length > 0)
                {
                    rline = "\tPRODCODE\t" + cdata + "\t\t\t\t\t\r\n";
                    rxn2db = rxn2db + rline;

                    i++;
                    cdata = getfdata(s2, "RXN:VARIATION(1):PROD.CODE(" + i + ")");
                }


                sr = new StringReader(s1);
                for (i = 0; i < 5; i++) slin = sr.ReadLine();

                string map = slin.Substring(0, 3);
                int p1 = Convert.ToInt32(map);
                map = slin.Substring(3, 3);
                int p2 = Convert.ToInt32(map);

                int q1 = s1.IndexOf("$MOL", 0);
                q1 = s1.IndexOf("\n", q1);
                int q2 = s1.IndexOf("$MOL", q1);
                for (i = 0; i < p1 + p2; i++)
                {
                    string scomp = "";
                    if (q2 > -1)
                    {
                        scomp = s1.Substring(q1 + 1, q2 - q1 - 1);
                        q1 = s1.IndexOf("\n", q2);
                        q2 = s1.IndexOf("$MOL", q1);
                    }
                    else
                        scomp = s1.Substring(q1 + 1);

                    if (i < p1)
                    {
                        string sgrd = getfdata(s2, "$DTYPE RXN:VARIATION(1):REACTANT(" + (i + 1) + "):GRADE");

                        rline = "\tREACTANT\t" + (i + 1) + "\t" + sgrd + "\t" + scomp + "\t\t\t\t\t\r\n";
                        rxn2db = rxn2db + rline;
                    }
                    else
                    {
                        string sgrd = getfdata(s2, "$DTYPE RXN:VARIATION(1):PRODUCT(" + (i + 1 - p1) + "):PRODAT:GRADE");
                        string syld = getfdata(s2, "$DTYPE RXN:VARIATION(1):PRODUCT(" + (i + 1 - p1) + "):PRODAT:YIELD");

                        rline = "\tPRODUCT\t" + (i + 1 - p1) + "\t" + sgrd + "\t" + syld + "\t" + scomp + "\t\t\t\t\t\r\n";
                        rxn2db = rxn2db + rline;
                    }
                }
/*
                i = 1;
                string acsn = getfdata(s2, "RXN:VARIATION(1):CCRREF(" + i + "):ACCESN.NO");
                while (acsn.Length > 0)
                {
                    string artl = getfdata(s2, "RXN:VARIATION(1):CCRREF(" + i + "):ARTL.NO");
                    string path = getfdata(s2, "RXN:VARIATION(1):CCRREF(" + i + "):PATH");
                    string step = getfdata(s2, "RXN:VARIATION(1):CCRREF(" + i + "):STEP");
                    string nsm = getfdata(s2, "RXN:VARIATION(1):CCRREF(" + i + "):NSM");

                    rline = "\tCCRREF\t" + getfdata(s2, "RXN:VARIATION(1):BATCHNO") + "\t" + acsn + "\t" + artl + "\t" + path + "\t" + step + "\t" + nsm + "\t" + rireg + "\t\t\t\t\t\r\n";
                    rxn2db = rxn2db + rline;

                    i++;
                    acsn = getfdata(s2, "RXN:VARIATION(1):CCRREF(" + i + "):ACCESN.NO");
                }
*/

                i = 1;
                string reg = getfdata(s2, "RXN:VARIATION(1):CATALYST(" + i + "):REGNO");
                while (reg.Length > 0)
                {
                    string sgrd = getfdata(s2, "RXN:VARIATION(1):CATALYST(" + i + "):GRADE");

                    rline = "\tCATALYST\t" + i + "\t" + sgrd + "\t" + reg.Substring(7) + "\t\t\t\t\t\r\n";
                    rxn2db = rxn2db + rline;

                    i++;
                    reg = getfdata(s2, "RXN:VARIATION(1):CATALYST(" + i + "):REGNO");
                }

                i = 1;
                reg = getfdata(s2, "RXN:VARIATION(1):SOLVENT(" + i + "):REGNO");
                while (reg.Length > 0)
                {
                    string sgrd = getfdata(s2, "RXN:VARIATION(1):SOLVENT(" + i + "):GRADE");

                    rline = "\tSOLVENT\t" + i + "\t" + sgrd + "\t" + reg.Substring(7) + "\t\t\t\t\t\r\n";
                    rxn2db = rxn2db + rline;

                    i++;
                    reg = getfdata(s2, "RXN:VARIATION(1):SOLVENT(" + i + "):REGNO");
                }


                string temp = getfdata(s2, "RXN:VARIATION(1):CONDITIONS:TEMP");
                string pres = getfdata(s2, "RXN:VARIATION(1):CONDITIONS:PRESSURE");
                string time = getfdata(s2, "RXN:VARIATION(1):CONDITIONS:TIME");
                string atom = getfdata(s2, "RXN:VARIATION(1):CONDITIONS:ATMOSPHERE");
                string other = getfdata(s2, "RXN:VARIATION(1):CONDITIONS:OTHER");
                string reflux= getfdata(s2, "RXN:VARIATION(1):CONDITIONS:REFLUX");

                rline = "\tCONDITION\t" + temp + "\t" + pres + "\t" + time + "\t" + atom + "\t" + other + "\t" + reflux + "\t\t\t\t\t\r\n";
                rxn2db = rxn2db + rline;
            }

            return rxn2db;
        }


        private string insmol(string S)
        {
            string rxn2db = "";
            nRxm = 0;

            int idx1 = 0, idx2 = 0, idx = 0;
            int mireg = -1;
            string s1 = "", s2 = "";

            idx2 = S.IndexOf("$MFMT $MIREG", 0);

            while (idx2 > -1)
            {
                idx1 = idx2;
                idx2 = S.IndexOf("$MFMT $MIREG", idx1 + 1);

                nRxm++;

                if (idx2 > idx1)
                {
                    s1 = S.Substring(idx1, idx2 - idx1);
                }
                else
                {
                    s1 = S.Substring(idx1);
                }

                idx = s1.IndexOf("\n");
                try
                {
                    mireg = Convert.ToInt32(s1.Substring(12, idx - 12));
                }
                catch (Exception e)
                {
                    ;
                }

                mireg = mireg + 100000;
                s1 = s1.Substring(idx + 1);

                idx = s1.IndexOf("$DTYPE", 0);
                s2 = s1.Substring(idx);
                s1 = s1.Substring(0, idx);

                string rline = "\tMOLECULE\t" + mireg + "\t" + s1 + "\t\t\t\t\t\r\n";
                rxn2db = rxn2db + rline;

                int i = 1;
                string rva = "$DTYPE MOL:SYMBOL(";
                int st = s2.IndexOf(rva + i + ")", 0);
                while (st > -1)
                {
                    rline = "\tSYMBOL\t" + i + "\t" + getfdata(s2, rva + i + ")") + "\t\t\t\t\t\r\n";
                    rxn2db = rxn2db + rline;

                    i++;
                    st = s2.IndexOf(rva + i + ")", 0);
                }

                i = 1;
                rva = "$DTYPE MOL:BIOLACT(";
                st = s2.IndexOf(rva + i + ")", 0);
                while (st > -1)
                {
                    rline = "\tBIOLACT\t" + i + "\t" + getfdata(s2, rva + i + ")") + "\t\t\t\t\t\r\n";
                    rxn2db = rxn2db + rline;

                    i++;
                    st = s2.IndexOf(rva + i + ")", 0);
                }
            }

            return rxn2db;
        }



        private string inscat(string S)
        {
            string rxn2db = "";
            nRxc = 0;
            int idx1 = 0, idx2 = 0, idx = 0;
            int mireg = -1;
            string s1 = "", s2 = "";

            idx2 = S.IndexOf("$MFMT $MIREG", 0);

            while (idx2 > -1)
            {
                idx1 = idx2;
                idx2 = S.IndexOf("$MFMT $MIREG", idx1 + 1);

                nRxc++;
                if (idx2 > idx1)
                {
                    s1 = S.Substring(idx1, idx2 - idx1);
                }
                else
                {
                    s1 = S.Substring(idx1);
                }

                idx = s1.IndexOf("\n");
                try
                {
                    mireg = Convert.ToInt32(s1.Substring(12, idx - 12));
                }
                catch (Exception e)
                {
                    ;
                }

                s1 = s1.Substring(idx + 1);

                idx = s1.IndexOf("$DTYPE", 0);
                s2 = s1.Substring(idx);
                s1 = s1.Substring(0, idx);

                string rline = "\tMOLECULE\t" + mireg + "\t" + s1 + "\t\t\t\t\t\r\n";
                rxn2db = rxn2db + rline;

                int i = 1;
                string rva = "$DTYPE MOL:SYMBOL(";
                int st = s2.IndexOf(rva + i + ")", 0);
                while (st > -1)
                {
                    rline = "\tSYMBOL\t" + i + "\t" + getfdata(s2, rva + i + ")") + "\t\t\t\t\t\r\n";
                    rxn2db = rxn2db + rline;

                    i++;
                    st = s2.IndexOf(rva + i + ")", 0);
                }

                i = 1;
                rva = "$DTYPE MOL:CAT.TYPE(";
                st = s2.IndexOf(rva + i + ")", 0);
                while (st > -1)
                {
                    rline = "\tCAT.TYPE\t" + i + "\t" + getfdata(s2, rva + i + ")") + "\t\t\t\t\t\r\n";
                    rxn2db = rxn2db + rline;

                    i++;
                    st = s2.IndexOf(rva + i + ")", 0);
                }
            }

            return rxn2db;
        }



        private void loadstatistics()
        {

            DataRow myDataRow;

            myDataRow = msgTable.NewRow();
            myDataRow["Object"] = "Catalyst & Solvent";
            myDataRow["In File"] = nRxc;
            cmd.CommandText = "SELECT count(molregno) count FROM moltable where molregno <100000";
            cmd.CommandType = System.Data.CommandType.Text;
            OraConn.Open();
            OracleDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                myDataRow["Loaded"] = (int)dr.GetDecimal(0);
            }
            dr.Close();


            cmd.CommandText = "SELECT molregno, dupcatsol Original_Catalyst FROM moltable where dupcatsol is not null";
            dr = cmd.ExecuteReader();

//            DataTable dupTable = new DataTable("dup");

            dupTable.Load(dr);
            int dupcnt = dupTable.Rows.Count;
            dataGridView3.DataSource = dupTable;
            dr.Close();
//            if (dupcnt > 0) dataGridView3.Visible = true;


            cmd.CommandText = "select 'Catalyst' component, rireg, molregno from Catalyst where molregno in (select unique molregno from catalyst minus select molregno from moltable) and molregno > 9868";
            dr = cmd.ExecuteReader();
//            DataTable brfTable = new DataTable("brf");
            brfTable.Load(dr);
            dataGridView4.DataSource = brfTable;
            dr.Close();
//            if (brfTable.Rows.Count > 0) dataGridView4.Visible = true;


            cmd.CommandText = "select 'Solvent' component, rireg, molregno from solvent where molregno in (select unique molregno from solvent minus select molregno from moltable) and molregno > 9868";
            dr = cmd.ExecuteReader();
            brfTable.Load(dr);
            dr.Close();

            dataGridView4.Columns[0].Width = 70;
            dataGridView4.Columns[1].Width = 50;
            dataGridView4.Columns[2].Width = 80;

            if (brfTable.Rows.Count > 0) dataGridView4.Visible = true;

            myDataRow["Duplicate Molecule"] = dupcnt;

            msgTable.Rows.Add(myDataRow);

            
            myDataRow = msgTable.NewRow();
            myDataRow["Object"] = "Molecules in RXM";
            myDataRow["In File"] = nRxm;
            cmd.CommandText = "SELECT count(unique molregno) count FROM (select molregno from symbol where molregno >100000 union select molregno from biolact where molregno >100000)";
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                myDataRow["Loaded"] = (int)dr.GetDecimal(0);
            }
            dr.Close();

            msgTable.Rows.Add(myDataRow);


            myDataRow = msgTable.NewRow();

            myDataRow["Object"] = "Reaction";
            myDataRow["In File"] = nRxn;
            cmd.CommandText = "SELECT count(rireg) count FROM REACTION";
            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                myDataRow["Loaded"] = (int)dr.GetDecimal(0);
            }
            dr.Close();


            cmd.CommandText = "SELECT rireg FROM REACTION where hasnostructs(rctab)=1";
            dr = cmd.ExecuteReader();

            int nostruct = 0;
            string rf = "rireg in (";
            while (dr.Read())
            {
                rf = rf + dr.GetDecimal(0) + ",";
                nostruct += 1;
            }
            nsrTable.Load(dr);
            dr.Close();
            dataGridView2.DataSource = nsrTable;

            myDataRow["No Structure"] = nostruct;
            msgTable.Rows.Add(myDataRow);

            OraConn.Close();
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.DataSource = msgTable;
            dataGridView1.Visible = true;

            if (nostruct > 0)
                dataGridView2.Visible = true;

            if (dupcnt > 0)
            {
                dataGridView3.Columns[1].Width = 120;
                dataGridView3.Visible = true;
            }





        }

        private void button6_Click(object sender, EventArgs e)
        {
            label6.Visible = true;
            label6.Text = "Loading to CCRPDN....";
            button6.Visible = false;
            Refresh();
            Application.DoEvents();
            this.Cursor = Cursors.WaitCursor;
            OraCon1.ConnectionString = ccrpdnConnectionString;
            try
            {
                OraCon1.Open();
                cmd.Connection = OraCon1;

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "load2ccrpdn";

                cmd.ExecuteNonQuery();

                label6.Text = "Loading to CCRPDN....Done!";
                Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                label6.Text = ex.Message;
                OraCon1.Close();
            }

            OraCon1.Close();
            this.Cursor = Cursors.Default;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            /*
            if (rptTable.Rows.Count > 0)
            {
                insrpt();
            }
             * */


            OraConn.ConnectionString = ccrConnectionString;

            OraConn.Open();
            cmd.Connection = OraConn;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.CommandText = "CATSOLCLEAN";
            cmd.ExecuteNonQuery();
            OraConn.Close();


            int idx1 = 0, idx2 = 0, idx = 0;
            int mireg = -1;
            string s1 = "", s2 = "";
            string scat, rxn2db;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select the RXN File";
            openFileDialog1.Filter = "RDF Files|*.rdf";

            openFileDialog1.ShowDialog();

            string catsol = openFileDialog1.FileName;

            try
            {
                StreamReader SR = File.OpenText(catsol);
                scat = SR.ReadToEnd();
                SR.Close();
            }
            catch (IOException exp)
            {
                MessageBox.Show(" Can't open rxn file: " + catsol);
                return;
            }


            MessageBox.Show(scat.Substring(1, 40));

            idx2 = scat.IndexOf("$MFMT $MIREG", 0);

            while (idx2 > -1)
            {
                rxn2db = "";
                idx1 = idx2;
                idx2 = scat.IndexOf("$MFMT $MIREG", idx1 + 1);

                if (idx2 > idx1)
                {
                    s1 = scat.Substring(idx1, idx2 - idx1);
                }
                else
                {
                    s1 = scat.Substring(idx1);
                }

                idx = s1.IndexOf("\n");
                try
                {
                    mireg = Convert.ToInt32(s1.Substring(12, idx - 12));
                }
                catch (Exception ee)
                {
                    ;
                }

                s1 = s1.Substring(idx + 1);

                idx = s1.IndexOf("$DTYPE", 0);
                s2 = s1.Substring(idx);
                s1 = s1.Substring(0, idx);
//                rend.MolfileString = s1;


                string rline = "\tMOLECULE\t" + mireg + "\t" + s1 + "\t\t\t\t\t\r\n";
                rxn2db = rxn2db + rline;

                int i = 1;
                string rva = "$DTYPE MOL:SYMBOL(";
                int st = s2.IndexOf(rva + i + "):SYMBOL", 0);
                while (st > -1)
                {
                    rline = "\tSYMBOL\t" + i + "\t" + getfdata(s2, rva + i + ")") + "\t\t\t\t\t\r\n";
                    rxn2db = rxn2db + rline;

                    i++;
                    st = s2.IndexOf(rva + i + ")", 0);
                }

                i = 1;
                rva = "$DTYPE MOL:CATALYST(";
                st = s2.IndexOf(rva + i + "):CAT.TYPE", 0);
                while (st > -1)
                {
                    rline = "\tCAT.TYPE\t" + i + "\t" + getfdata(s2, rva + i + ")") + "\t\t\t\t\t\r\n";
                    rxn2db = rxn2db + rline;

                    i++;
                    st = s2.IndexOf(rva + i + ")", 0);
                }

                try
                {
                    OraConn.ConnectionString = ccrConnectionString;

                    OraConn.Open();
                    cmd.Connection = OraConn;
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("clobtodb", OracleDbType.Clob).Value = rxn2db;
                    cmd.CommandText = "CATSOL_LOAD";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    cmd.Parameters.Clear();
                    OraConn.Close();
                    return;
                }

                cmd.Parameters.Clear();
                OraConn.Close();


            }

            OraConn.Open();
            cmd.Connection = OraConn;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.CommandText = "CATSOLFASTINDEX";
            cmd.ExecuteNonQuery();
            OraConn.Close();
               
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            if (button3.Visible == true)
            {
                button3.Visible = false;
                btnrpt.Visible = false;
            }
            else
            {
                button3.Visible = true;
                btnrpt.Visible = true;
            }
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if(button1.Text == "Choose Current Domex Folder") button1.Text = "Get Current Domex Files";
                else if (button1.Text == "Get Current Domex Files") button1.Text = "Choose Current Domex Folder";
            }
        }

        private void btnrpt_Click(object sender, EventArgs e)
        {
            if (rptTable.Rows.Count > 0)
            {
                insrpt();
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            OraConn.ConnectionString = ccrConnectionString;

            OraConn.Open();
            cmd.Connection = OraConn;
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.CommandText = "CATSOLFASTINDEX";
            cmd.ExecuteNonQuery();
            OraConn.Close();

        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            string indiapath = "";
            string batch = "";
            
            string sserver = "ftp://84.18.185.5/to_isi/";
            string lstdir = "";

            FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(sserver));

            try
            {
                reqFTP.Credentials = new NetworkCredential("imgindia", "imgpwd");
                reqFTP.Proxy = null;
                reqFTP.EnableSsl = false;
                reqFTP.UseBinary = false;
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                Stream ftpStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(ftpStream);

                lstdir = reader.ReadToEnd();

                ftpStream.Close();
                response.Close();
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            int diridx = lstdir.IndexOf("ESTIMATE-");

            MessageBox.Show(lstdir);

            string sbatch = lstdir.Substring(diridx + 9, 6);
            batch = sbatch;

            string locday = System.DateTime.Today.ToString("MMdd-") + sbatch.Substring(0, 4) + "-" + sbatch.Substring(4);

            string locdir = "\\\\tshuspaphichmfs\\TSHUSPAPHISUS01\\chem\\users\\shared\\domex\\" + locday + "\\";

            MessageBox.Show(locdir);
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }




    }
}
