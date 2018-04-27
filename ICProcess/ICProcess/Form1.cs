using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading;
//using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using System.Runtime.InteropServices;
using ICProcess.Properties;

namespace ICProcess
{



    public partial class Form1 : Form
    {
        static private string connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1550)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=chemdb.prod-wos.com)));";

        static private string icpdnConnectionString = "";
        static private string ccrpdnConnectionString = "";
        static private string icannConnectionString = "";
        static private string icwosConnectionString = "";

        static private OracleConnection OraConn = new OracleConnection();
        static private OracleCommand comm = new OracleCommand();
        static private OracleDataAdapter oda = new OracleDataAdapter();
        static private OpenFileDialog ofd1 = new OpenFileDialog();


        static private DataTable difTable = new DataTable("dif");
        static private DataTable cpdTable = new DataTable("cpd");
        static private DataTable dupTable = new DataTable("dup");
        static private DataTable jrnTable = new DataTable("jrn");
        static private BindingSource bs = new BindingSource();

        static private MDL.Draw.Renditor.Renditor renditor00 = new MDL.Draw.Renditor.Renditor();
        static private DataSet ds = new DataSet();
        static private ToolTip ttp1 = new ToolTip();
        System.Diagnostics.Process proc = new System.Diagnostics.Process();
        static private string smilestr = "";
        static private bool done = false;
        static private bool go = true;
        static private Properties.Settings settings = Properties.Settings.Default;




        public Form1()
        {
            InitializeComponent();

            DataColumn myDataColumn;

            myDataColumn = new DataColumn("CB", System.Type.GetType("System.String"));
            cpdTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("CK", System.Type.GetType("System.String"));
            cpdTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("CG", System.Type.GetType("System.String"));
            cpdTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("LO", System.Type.GetType("System.String"));
            cpdTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("mn", System.Type.GetType("System.Boolean"));
            cpdTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("ac", System.Type.GetType("System.String"));
            cpdTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("ut", System.Type.GetType("System.String"));
            cpdTable.Columns.Add(myDataColumn);
        }


        private void textBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            string dpath = textBox1.Text;
            int pidx = dpath.LastIndexOf("\\");

            if (pidx > 0) dpath = dpath.Substring(0, pidx);

            ofd1.Title = "Select ICDB BIB file:";
            ofd1.Filter = "Datafactory Files|*.TXT;*.DAT";
            ofd1.InitialDirectory = dpath;

            if (ofd1.ShowDialog() != DialogResult.OK) return;

            pidx = ofd1.FileName.LastIndexOf("\\");

            string fnm = ofd1.FileName;

            dpath = ofd1.FileName.Substring(0, pidx + 1);
            textBox1.Text = "";
            if (fnm.IndexOf("ICDB") > -1)
            {
                textBox1.Text = dpath + "ICDBBIB.TXT";
                textBox2.Text = dpath + "ICDBCPD.TXT";
            }
            else if (fnm.IndexOf("CPDCTR") > -1)
            {
                textBox1.Text = dpath + "CPDCTRBI.DAT";
                textBox2.Text = dpath + "CPDCTRCP.DAT";
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            Cursor = Cursors.WaitCursor;

            if (button1.Text == "Check Mismatches")
            {
                string bibfile = "";
                string cpdfile = "";
                cpdTable.Rows.Clear();
                difTable.Rows.Clear();
                dataGridView1.Visible = false;
                dataGridView2.Visible = false;

                try
                {
                    StreamReader SR = File.OpenText(textBox1.Text);
                    bibfile = SR.ReadToEnd();
                    SR.Close();

                    SR = File.OpenText(textBox2.Text);
                    cpdfile = SR.ReadToEnd();
                    SR.Close();

                }
                catch (IOException exp)
                {
                    MessageBox.Show(exp.ToString());
                    return;
                }

                //                MessageBox.Show(bibfile);

                if (bibfile.Substring(0, 10) == "FN ICDBBIB")
                    df2bibload(cpdfile, bibfile);
                else if (bibfile.Substring(0, 10) == "FN CPDCTRB")
                {
                    df2bibload(cpdfile, bibfile);
                    //                    Cursor = Cursors.Default;
                    //                    return;
                }
                else return;


                jrnTable.Rows.Clear();
                difTable.Rows.Clear();

                comm.CommandType = CommandType.Text;
                try
                {
                    if (OraConn.State == ConnectionState.Open) OraConn.Close();
                    OraConn.ConnectionString = icpdnConnectionString;

                    comm.Connection = OraConn;

                    OraConn.Open();

                    comm.CommandText = "select unique substr(lo, 0, 5) acsn from dfcpd";
                    OracleDataReader dr = comm.ExecuteReader();
                    jrnTable.Load(dr);

                    comm.CommandText = "select acsn || lpad(to_char(ARTLNUM),3,'0')lo, lpad(AUTHNUM, 3,' ') authnum, 'Not in Datafactory' mismatch from ic_moltabref where acsn in ( select unique substr(lo, 1,5) from dfcpd) minus select lo,cg authnum, 'Not in Datafactory' mismatch from dfcpd";
                    dr = comm.ExecuteReader();
                    difTable.Load(dr);

                    comm.CommandText = "select lo, cg authnum, 'Not in ICpdn' mismatch from dfcpd minus select acsn || lpad(to_char(ARTLNUM),3,'0') lo, lpad(AUTHNUM, 3,' ') authnum, 'Not in ICpdn' mismatch from ic_moltabref where acsn in ( select unique substr(lo, 1,5) from dfcpd)";
                    dr = comm.ExecuteReader();
                    difTable.Load(dr);

                }
                catch (Exception ex)
                {
                    OraConn.Close();
                    MessageBox.Show(ex.ToString());
                    Cursor = Cursors.Default;
                    return;
                }
                OraConn.Close();


                dataGridView2.DataSource = jrnTable;
                dataGridView1.DataSource = difTable;
                dataGridView1.Visible = true;
                dataGridView2.Visible = true;

                if (difTable.Rows.Count == 0) button1.Text = "Update cpdnumbers";
                button1.Visible = true;

            }
            else if (button1.Text == "Update cpdnumbers")
            {
                OraConn.ConnectionString = icannConnectionString;

                comm.CommandType = CommandType.StoredProcedure;
                comm.CommandText = "DFACTPNDLOAD";

                try
                {
                    OraConn.Open();
                    comm.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    OraConn.Close();
                    MessageBox.Show(ex.ToString());
                    Cursor = Cursors.Default;
                    return;
                }
                OraConn.Close();

                button1.Visible = false;
                MessageBox.Show("Loaded to icann.");

            }

            Cursor = Cursors.Default;

        }



        private void df2bibload(string cpdfile, string bibfile)
        {
            int ns = cpdfile.IndexOf("\r\nRE \r\n") + 7;
            int ne = cpdfile.IndexOf("\r\nRE \r\n", ns);

            string df2db = bibfile.Replace("\r\n-- ", "");


            int ist = 0;

            ist = df2db.IndexOf("\r\nCK ", ist + 1);
            while (ist > -1)
            {
                string sck = df2db.Substring(ist, 11);

                int icpd = 0;
                string scpd = "";
                icpd = cpdfile.IndexOf(sck, icpd + 1);
                while (icpd > -1)
                {
                    scpd += cpdfile.Substring(icpd - 12, 33).Replace(sck + "\r\nCG ", "");

                    icpd = cpdfile.IndexOf(sck, icpd + 1);
                }

                df2db = df2db.Insert(ist + 13, scpd);

                //                MessageBox.Show(df2db.Substring(ist, 5000));

                ist = df2db.IndexOf("\r\nCK ", ist + 1);

            }

            df2db = df2db.Replace("\r\nRE ", "\r\nRE \r\nEX ");

            //            MessageBox.Show(df2db.Substring(0, 5000));


            if (OraConn.State == ConnectionState.Open) OraConn.Close();
            OraConn.ConnectionString = icpdnConnectionString;
            comm.Connection = OraConn;

            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandText = "dfactbibload";
            comm.Parameters.Add("clobtodb", OracleDbType.Clob).Value = df2db;

            try
            {
                OraConn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                comm.Parameters.Clear();
                OraConn.Close();
                MessageBox.Show(ex.ToString());
                Cursor = Cursors.Default;
                return;
            }
            comm.Parameters.Clear();
            OraConn.Close();

        }


        private void df2dbload(string cpdfile, string bibfile)
        {
            int ns = cpdfile.IndexOf("\r\nRE \r\n") + 7;
            int ne = cpdfile.IndexOf("\r\nRE \r\n", ns);

            while (ne > 0)
            {
                string sart = cpdfile.Substring(ns, ne - ns);

                DataRow myDataRow = cpdTable.NewRow();
                myDataRow["CB"] = sart.Substring(3, 9);
                myDataRow["CK"] = sart.Substring(sart.IndexOf("\r\nCK ") + 5, 6);
                myDataRow["CG"] = sart.Substring(sart.IndexOf("\r\nCG ") + 5, 3);
                cpdTable.Rows.Add(myDataRow);

                ns = ne + 7;
                ne = cpdfile.IndexOf("\r\nRE \r\n", ns);
            }

            string df2db = "";

            bs.DataSource = cpdTable;

            ns = bibfile.IndexOf("\r\nUT ");
            ne = bibfile.IndexOf("\r\nRE ", ns);
            string acsn = "";
            while (ne > 0)
            {
                string sart = bibfile.Substring(ns, ne - ns);
                string ut = sart.Substring(5, sart.IndexOf("\r\nLO ") - 5);
                string lo = sart.Substring(sart.IndexOf("\r\nLO ") + 5, 8);

                bs.Filter = "CK=" + sart.Substring(sart.IndexOf("\r\nCK ") + 5, 6); ;
                bs.Sort = "CB";
                bs.MoveFirst();
                for (int j = 0; j < bs.Count; j++)
                {
                    DataRowView dr = (DataRowView)bs.Current;
                    dr.Row[3] = lo;
                    dr.Row[6] = ut;
                    if (acsn != lo.Substring(0, 5))
                    {
                        dr.Row[4] = true;
                        acsn = lo.Substring(0, 5);
                    }
                    else dr.Row[4] = false;

                    dr.Row[5] = acsn;

                    df2db = df2db + dr.Row[0].ToString() + "\t" + dr.Row[1].ToString() + "\t" + dr.Row[2].ToString() + "\t" + dr.Row[3].ToString() + "\t" + dr.Row[6].ToString() + "\t\r\n";

                    bs.MoveNext();
                }

                ns = bibfile.IndexOf("\r\nUT ", ne);
                if (ns > 0) ne = bibfile.IndexOf("\r\nRE ", ns);
                else ne = -1;
            }

            bs.Filter = "";

            df2db = df2db + "\t\t\t\r\n";

            if (OraConn.State == ConnectionState.Open) OraConn.Close();
            OraConn.ConnectionString = icpdnConnectionString;
            comm.Connection = OraConn;

            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandText = "dfactcpdload";
            comm.Parameters.Add("clobtodb", OracleDbType.Clob).Value = df2db;

            try
            {
                OraConn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                comm.Parameters.Clear();
                OraConn.Close();
                MessageBox.Show(ex.ToString());
                Cursor = Cursors.Default;
                return;
            }
            comm.Parameters.Clear();
            OraConn.Close();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            string host = settings.host;
            string port = settings.port;
            string service = settings.service;

            textBox4.Text = host;
            textBox5.Text = port;
            textBox6.Text = service;

            if (host.Length * port.Length * service.Length > 0)
            {
                connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + host + ")(PORT=" + port + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + service + ")));";
            }

            icpdnConnectionString = connectionString + "User Id=icpdn;Password=icpdn;";
            ccrpdnConnectionString = connectionString + "User Id=ccrpdn;Password=ccrpdn;";
            icannConnectionString = connectionString + "User Id=icann;Password=icann;";
            icwosConnectionString = connectionString + "User Id=ic_wos;Password=ic_wos;";

            panel1.Visible = false;
            panel2.Visible = false;
            panel6.Visible = false;
            panel7.Visible = false;

            MainMenu MyMenu = new MainMenu();

            MenuItem m1 = new MenuItem("File");
            MyMenu.MenuItems.Add(m1);

            MenuItem subm1 = new MenuItem("Datafactory Tagfile");
            m1.MenuItems.Add(subm1);

            MenuItem subm2 = new MenuItem("Moving JRNL Between Batches");
            m1.MenuItems.Add(subm2);

            MenuItem subm3 = new MenuItem("Update Cpdnumber");
            m1.MenuItems.Add(subm3);

            MenuItem subm4 = new MenuItem("Check Duplicates Against MasterDB");
            m1.MenuItems.Add(subm4);

            MenuItem subm6 = new MenuItem("Load IC to MasterDB");
            m1.MenuItems.Add(subm6);

            MenuItem subm7 = new MenuItem("Database Configuration");
            m1.MenuItems.Add(subm7);

            MenuItem subm5 = new MenuItem("Exit");
            m1.MenuItems.Add(subm5);

            subm1.Click += new EventHandler(MMTagClick);
            subm2.Click += new EventHandler(MMMovClick);
            subm3.Click += new EventHandler(MMCPDClick);
            subm4.Click += new EventHandler(MMDupClick);
            subm6.Click += new EventHandler(MMMasClick);
            subm7.Click += new EventHandler(MMDbsClick);
            subm5.Click += new EventHandler(MMExitClick);

            Menu = MyMenu;

            //textBox1.Text = "o:\\ic_editing\\datafactory\\ICDBBIB.TXT";
            //textBox2.Text = "o:\\ic_editing\\datafactory\\ICDBCPD.TXT";

            textBox1.Text = "\\\\34.212.24.69\\users\\datafactory\\ICDBBIB.TXT";
            textBox2.Text = "\\\\34.212.24.69\\users\\datafactory\\ICDBCPD.TXT";

            ttp1.SetToolTip(textBox1, "DoubleClick to browse Datafactory files");



        }


        private void button2_Click(object sender, EventArgs e)
        {

            string jcnt = "";
            int acnt = 0;

            string tyear = comboBox1.Text.Substring(0, 4) + "_" + comboBox1.Text.Substring(4, 2);
            string tagfile = "FN CBL_" + tyear + ".TAG\r\n";
            string ccrfile = "FN CCR_" + tyear + ".TAG\r\n";
            //string path = "\\\\tshuspaphichem2\\Applications\\IC\\datafactory\\";
            string path = "\\\\34.212.24.69\\chem\\users\\TagFile\\";

            Cursor = Cursors.WaitCursor;
            ds.Clear();

            if (OraConn.State == ConnectionState.Open) OraConn.Close();
            OraConn.ConnectionString = icpdnConnectionString;
            comm.Connection = OraConn;
            comm.CommandType = CommandType.Text;

            try
            {
                OraConn.Open();
                OracleDataAdapter adapter = new OracleDataAdapter();
                comm.CommandText = "select distinct acsn, artlnum from ic_moltable where batchno = '" + comboBox1.Text + "' order by 1, 2";
                adapter.SelectCommand = comm;
                adapter.Fill(ds, "ART");
                //                comm.CommandText = "select distinct acsn, artlnum from ic_molref where batchno = '" + comboBox1.Text + "' order by 1, 2";
                //                adapter.Fill(ds, "ART");

                comm.CommandText = "select count(distinct acsn) cnt from ic_moltable where batchno = '" + comboBox1.Text + "' order by 1";
                adapter.Fill(ds, "ACSN");
                //                comm.CommandText = "select count(distinct acsn) cnt from ic_molref where batchno = '" + comboBox1.Text + "' order by 1";
                //                adapter.Fill(ds, "ACSN");

                comm.CommandText = "select acsn, artlnum, authnum, grade, regno, lpad(authnum, 3) autsrt from ic_moltable where batchno = '" + comboBox1.Text + "' order by 1, 2, lpad(authnum, 3)";
                adapter.Fill(ds, "cpd");
                //                comm.CommandText = "select acsn, artlnum, authnum, grade, regno, lpad(authnum, 3) autsrt from ic_molref where batchno = '" + comboBox1.Text + "' order by 1, 2, lpad(authnum, 3)";
                //                adapter.Fill(ds, "cpd");

                comm.CommandText = "select m.regno, symbol from ic_moltable m, ic_symbol s where m.regno = s.regno and batchno = '" + comboBox1.Text + "' order by 1, 2";
                adapter.Fill(ds, "sym");

                comm.CommandText = "select m.acsn, m.artlnum, m.authnum, b.ba_code, b.status from ic_moltable m, ic_biolact b where m.regno = b.regno and batchno = '" + comboBox1.Text + "' order by 1, 2, lpad(m.authnum, 3)";
                adapter.Fill(ds, "bio");

                comm.CommandText = "select unique m.acsn, m.artlnum, b.ba_code, b.status, upper(n.biol_activity) biolact from ic_moltable m, ic_biolact b, ccrpdn.ba_main n where b.ba_code=n.ba_code and m.regno = b.regno and batchno = '" + comboBox1.Text + "' order by 1, 2";
                adapter.Fill(ds, "bac");

                comm.CommandText = "select acsn, artlnum, actual_data data from subject_index where prod_code = 'IC' and batchno = '" + comboBox1.Text + "' order by 1, 2, 3";
                adapter.Fill(ds, "isb");

                comm.CommandText = "select acsn, artlnum, subj_type, seq_num, actual_data data from subject_index where prod_code = 'CR' and batchno = '" + comboBox1.Text + "' order by 1, 2, 4, 3, 5";
                adapter.Fill(ds, "csb");

                comm.CommandText = "select unique acsn, artlnum from subject_index where prod_code = 'CR' and batchno = '" + comboBox1.Text + "' order by 1, 2";
                adapter.Fill(ds, "car");

                comm.CommandText = "select count(distinct acsn) cnt from subject_index where prod_code = 'CR' and batchno = '" + comboBox1.Text + "'";
                adapter.Fill(ds, "cac");

                jcnt = ds.Tables["ACSN"].Rows[0][0].ToString();
                acnt = ds.Tables["ART"].Rows.Count;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraConn.Close();
            }


            for (int i = 0; i < ds.Tables["ART"].Rows.Count; i++)
            {
                tagfile = tagfile + "AC " + ds.Tables["ART"].Rows[i]["acsn"].ToString() + "\r\nFA " + ds.Tables["ART"].Rows[i]["artlnum"].ToString() + "\r\nS3\r\n";

                string aut = "", alist = "";
                int lnc = -1;

                DataView dv = new DataView(ds.Tables["cpd"], "acsn='" + ds.Tables["ART"].Rows[i]["acsn"].ToString() + "' and artlnum= " + ds.Tables["ART"].Rows[i]["artlnum"].ToString(), "autsrt", DataViewRowState.CurrentRows);
                for (int j = 0; j < dv.Count; j++)
                {
                    string aut1 = dv[j].Row["authnum"].ToString();
                    int nc = compare(aut, aut1);
                    if (nc == -1)
                    {
                        alist = alist + "," + aut1;
                    }
                    else if (lnc == -1)
                    {
                        alist = alist + "-" + aut1;
                    }
                    else if (lnc != nc)
                    {
                        alist = alist + "," + aut1;
                    }
                    else
                    {
                        alist = alist.Substring(0, alist.Length - aut.Length);
                        if (alist[alist.Length - 1] != '-') alist = alist + aut + "-";

                        alist = alist + aut1;
                    }
                    lnc = nc;
                    aut = aut1;
                }

                tagfile = tagfile + "1Q " + sconvert(alist.Substring(1)) + "\r\n";

                dv = new DataView(ds.Tables["isb"], "acsn='" + ds.Tables["ART"].Rows[i]["acsn"].ToString() + "' and artlnum= " + ds.Tables["ART"].Rows[i]["artlnum"].ToString(), "", DataViewRowState.CurrentRows);
                for (int j = 0; j < dv.Count; j++)
                {
                    tagfile = tagfile + "S4\r\nPR IC\r\n1R " + sconvert(dv[j].Row["data"].ToString()) + "\r\nE4\r\n";
                }


                dv = new DataView(ds.Tables["cpd"], "acsn='" + ds.Tables["ART"].Rows[i]["acsn"].ToString() + "' and artlnum= " + ds.Tables["ART"].Rows[i]["artlnum"].ToString(), "autsrt", DataViewRowState.CurrentRows);
                for (int j = 0; j < dv.Count; j++)
                {
                    tagfile = tagfile + "S5\r\n1C " + dv[j].Row["authnum"].ToString() + "\r\n1N " + (j + 1) + "\r\n";

                    DataView dv1 = new DataView(ds.Tables["sym"], "regno=" + dv[j].Row["regno"].ToString(), "", DataViewRowState.CurrentRows);
                    for (int k = 0; k < dv1.Count; k++)
                    {
                        tagfile = tagfile + "1D " + sconvert(dv1[k].Row["symbol"].ToString()) + "\r\n";
                    }

                    if (dv[j].Row["grade"].ToString().Length > 0)
                        tagfile = tagfile + "1E " + sconvert(dv[j].Row["grade"].ToString()) + "\r\n";
                    tagfile = tagfile + "E5\r\n";

                }


                dv = new DataView(ds.Tables["bac"], "acsn='" + ds.Tables["ART"].Rows[i]["acsn"].ToString() + "' and artlnum= " + ds.Tables["ART"].Rows[i]["artlnum"].ToString(), "", DataViewRowState.CurrentRows);
                for (int j = 0; j < dv.Count; j++)
                {
                    DataView dv1 = new DataView(ds.Tables["bio"], "acsn='" + ds.Tables["ART"].Rows[i]["acsn"].ToString() + "' and artlnum= " + ds.Tables["ART"].Rows[i]["artlnum"].ToString() + " and status = '" + dv[j].Row["status"].ToString() + "' and ba_code = " + dv[j].Row["ba_code"].ToString(), "", DataViewRowState.CurrentRows);

                    aut = ""; alist = "";
                    lnc = -1;
                    for (int k = 0; k < dv1.Count; k++)
                    {
                        string aut1 = dv1[k].Row["authnum"].ToString();
                        int nc = compare(aut, aut1);
                        if (nc == -1)
                        {
                            alist = alist + "," + aut1;
                        }
                        else if (lnc == -1)
                        {
                            alist = alist + "-" + aut1;
                        }
                        else if (lnc != nc)
                        {
                            alist = alist + "," + aut1;
                        }
                        else
                        {
                            alist = alist.Substring(0, alist.Length - aut.Length);
                            if (alist[alist.Length - 1] != '-') alist = alist + aut + "-";

                            alist = alist + aut1;
                        }
                        lnc = nc;
                        aut = aut1;
                    }

                    tagfile = tagfile + "S7\r\n1K " + sconvert(alist.Substring(1)) + "\r\n";
                    tagfile = tagfile + "1O " + sconvert(dv[j].Row["biolact"].ToString()) + "\r\n1L " + dv[j].Row["status"].ToString() + "\r\nE7\r\n";

                }

                tagfile = tagfile + "E3\r\nRE\r\n";

            }

            tagfile = tagfile + "H5 " + jcnt + "\r\n";
            tagfile = tagfile + "H6 " + acnt + "\r\n";

            //            MessageBox.Show(tagfile.Length + "");
            //            string tmpfile = tagfile.Replace("\r\n", "");

            tagfile = tagfile + "H7 " + ((tagfile.Length - tagfile.Replace("\r\n", "").Length) / 2 + 2) + "\r\nEF\r\n";

            //            textBox3.Text = tagfile;

            //            MessageBox.Show(tagfile);



            for (int i = 0; i < ds.Tables["car"].Rows.Count; i++)
            {
                ccrfile = ccrfile + "AC " + ds.Tables["car"].Rows[i]["acsn"].ToString() + "\r\nFA " + ds.Tables["car"].Rows[i]["artlnum"].ToString() + "\r\nS3\r\n";

                int mn = 0;
                DataView dv = new DataView(ds.Tables["csb"], "acsn='" + ds.Tables["car"].Rows[i]["acsn"].ToString() + "' and artlnum= " + ds.Tables["car"].Rows[i]["artlnum"].ToString(), "", DataViewRowState.CurrentRows);
                for (int j = 0; j < dv.Count; j++)
                {

                    if (dv[j].Row["subj_type"].ToString() == "R")
                        ccrfile = ccrfile + "1B " + sconvert(dv[j].Row["data"].ToString()) + "\r\n";
                    else if (dv[j].Row["subj_type"].ToString() == "M")
                    {
                        if (mn == 1) ccrfile = ccrfile + "E4\r\n"; mn = 0;
                        ccrfile = ccrfile + "S4\r\nPR CCR\r\n1R " + sconvert(dv[j].Row["data"].ToString()) + "\r\n";
                    }
                    else if (dv[j].Row["subj_type"].ToString() == "S")
                    {
                        ccrfile = ccrfile + "1S " + sconvert(dv[j].Row["data"].ToString()) + "\r\n";
                        mn = 1;
                    }

                    //                    ccrfile = ccrfile + "S4\r\nPR IC\r\n1R " + sconvert(dv[j].Row["data"].ToString()) + "\r\nE4\r\n";
                }
                if (mn == 1) ccrfile = ccrfile + "E4\r\n";
                ccrfile = ccrfile + "E3\r\nRE\r\n";

            }

            jcnt = ds.Tables["cac"].Rows[0][0].ToString();
            acnt = ds.Tables["car"].Rows.Count;

            ccrfile = ccrfile + "H5 " + jcnt + "\r\n";
            ccrfile = ccrfile + "H6 " + acnt + "\r\n";

            ccrfile = ccrfile + "H7 " + ((ccrfile.Length - ccrfile.Replace("\r\n", "").Length) / 2 + 2) + "\r\nEF\r\n";

            textBox3.Text = ccrfile;


            using (StreamWriter writer = new StreamWriter(path + "CBL_" + tyear + ".TAG"))
            {
                writer.Write(tagfile);
            }
            using (StreamWriter writer = new StreamWriter(path + "CBL_" + tyear + ".done"))
            {
                writer.Write(tagfile);
            }



            if (comboBox2.Text == "With CCR")
            {
                using (StreamWriter writer = new StreamWriter(path + "CCR_" + tyear + ".TAG"))
                {
                    writer.Write(ccrfile);
                }
                using (StreamWriter writer = new StreamWriter(path + "CCR_" + tyear + ".done"))
                {
                    writer.Write(ccrfile);
                }
            }

            string ftpcmd = "open c508wqrdfplus.int.thomsonreuters.com\r\nchemprd\r\nprdchem\r\n\r\ncd PROGRAMS/LoadDataFromIndia/Input/Incoming/\r\n\r\n";
            ftpcmd = ftpcmd + "put " + path + "CBL_" + tyear + ".TAG \r\n\r\n";
            ftpcmd = ftpcmd + "put " + path + "CBL_" + tyear + ".TAG CBL_" + tyear + ".done\r\n\r\n";
            if (comboBox2.Text == "With CCR")
            {
                ftpcmd = ftpcmd + "put " + path + "CCR_" + tyear + ".TAG \r\n\r\n";
                ftpcmd = ftpcmd + "put " + path + "CCR_" + tyear + ".TAG CCR_" + tyear + ".done\r\n\r\n";
            }
            ftpcmd = ftpcmd + "quit\r\n";

            using (StreamWriter writer = new StreamWriter(path + "programs\\ftpdfact.cmd"))
            {
                writer.Write(ftpcmd);
            }

            using (StreamWriter writer = new StreamWriter(path + "programs\\send_dfact_tagfile.bat"))
            {
                writer.Write("ftp -s:" + path + "programs\\ftpdfact.cmd\r\n");
            }

            string filePath = path + "programs\\send_dfact_tagfile.bat";
            if (File.Exists(filePath))
            {
                string argument = @"/select, " + filePath;
                System.Diagnostics.Process.Start("explorer.exe", argument);

                System.Diagnostics.Process.Start(path + "programs\\send_dfact_tagfile.bat");
            }

            Cursor = Cursors.Default;

        }




        protected void MMTagClick(object who, EventArgs e)
        {
            panel2.Visible = true;
            panel1.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            panel6.Visible = false;
            panel7.Visible = false;

            if (OraConn.State == ConnectionState.Open) OraConn.Close();
            OraConn.ConnectionString = icpdnConnectionString;
            comm.Connection = OraConn;
            comm.CommandType = CommandType.Text;
            comm.CommandText = "select unique batchno from ic_moltable order by 1 desc";

            try
            {
                OraConn.Open();
                OracleDataReader dr = comm.ExecuteReader();


                while (dr.Read())
                {
                    comboBox1.Items.Add(dr.GetDecimal(0));
                }
                dr.Close();
                if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraConn.Close();
            }

        }


        protected void MMDupClick(object who, EventArgs e)
        {
            panel4.Visible = true;
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel6.Visible = false;
            panel7.Visible = false;


            //            Application.Exit();
        }


        protected void MMMovClick(object who, EventArgs e)
        {
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = true;
            panel4.Visible = false;
            panel6.Visible = false;
            panel7.Visible = false;
        }

        protected void MMCPDClick(object who, EventArgs e)
        {
            panel1.Visible = true;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            panel6.Visible = false;
            panel7.Visible = false;
        }

        protected void MMMasClick(object who, EventArgs e)
        {
            panel6.Visible = true;
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
            panel7.Visible = false;
        }

        protected void MMDbsClick(object who, EventArgs e)
        {
            panel7.Visible = true;
            panel6.Visible = false;
            panel1.Visible = false;
            panel2.Visible = false;
            panel3.Visible = false;
            panel4.Visible = false;
        }

        protected void MMExitClick(object who, EventArgs e)
        {
            Application.Exit();
        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            button1.Text = "Check Mismatches";
            button1.Visible = true;
            dataGridView1.Visible = false;
            dataGridView2.Visible = false;
        }





        private string sconvert(string ss)
        {
            int ln = 75, i;

            i = ss.Length / ln;

            while (i > 0)
            {
                ss = ss.Substring(0, i * ln) + "\r\n-- " + ss.Substring(i * ln);
                i--;
            }

            return ss;
        }


        private int compare(string s1, string s2)
        {
            int i = 0, j = -1, n1 = 0, n2 = 0, n = 0;
            string[] a, b;

            a = new string[3];
            b = new string[3];

            while (i < s1.Length)
            {
                if (!Char.IsDigit(s1[i]))
                {
                    if (i > j + 1) a[n1++] = s1.Substring(j + 1, i - j - 1);
                    j = i;
                }

                if (i == j) a[n1++] = s1.Substring(i, 1);

                i++;
            }
            if (i > j + 1) a[n1++] = s1.Substring(j + 1, i - j - 1);

            i = 0; j = -1;
            while (i < s2.Length)
            {
                if (!Char.IsDigit(s2[i]))
                {
                    if (i > j + 1) b[n2++] = s2.Substring(j + 1, i - j - 1);
                    j = i;
                }

                if (i == j) b[n2++] = s2.Substring(i, 1);

                i++;
            }
            if (i > j + 1) b[n2++] = s2.Substring(j + 1, i - j - 1);

            if (n1 != n2) return -1;

            n = 0;
            for (i = 0; i < n1; i++)
            {
                string t1 = a[i], t2 = b[i];
                if (t1 != t2)
                {
                    n++; j = i;
                }
            }

            if (n != 1) return -1;

            if (Char.IsDigit(a[j][0]) && Char.IsDigit(b[j][0]))
            {
                if (Convert.ToInt32(b[j]) - Convert.ToInt32(a[j]) == 1) return j;
            }
            else if (Char.IsLetter(a[j][0]) && Char.IsLetter(b[j][0]))
            {
                if ((int)b[j][0] - (int)a[j][0] == 1) return j;
            }

            return -1;
        }

        private void panel3_VisibleChanged(object sender, EventArgs e)
        {
            if (panel3.Visible == true)
            {


                rpcomb();
                if (comboBox3.Items.Count > 0) comboBox3.SelectedIndex = 0;

            }
        }


        private void rpcomb()
        {
            comboBox3.Items.Clear();
            comboBox4.Items.Clear();

            if (OraConn.State == ConnectionState.Open) OraConn.Close();
            OraConn.ConnectionString = icpdnConnectionString;
            comm.Connection = OraConn;
            comm.CommandType = CommandType.Text;
            comm.CommandText = "select unique batchno from ic_moltable union select unique nvl(batchno, 111111) from ccrpdn.ccr_batch order by 1 desc";


            try
            {
                OraConn.Open();
                OracleDataReader dr = comm.ExecuteReader();

                while (dr.Read())
                {
                    try
                    {
                        comboBox3.Items.Add(dr.GetDecimal(0));
                        comboBox4.Items.Add(dr.GetDecimal(0));
                    }
                    catch (Exception ex1)
                    {
                        MessageBox.Show(dr.GetString(0));
                    }
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraConn.Close();
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex > -1)
            {

                rflst1(comboBox3.Text);
            }
        }



        private void rflst1(string batch1)
        {
            Cursor = Cursors.WaitCursor;

            listBox1.Items.Clear();

            if (OraConn.State == ConnectionState.Open) OraConn.Close();
            OraConn.ConnectionString = icpdnConnectionString;
            comm.Connection = OraConn;
            comm.CommandType = CommandType.Text;

            comm.CommandText = "select unique acsn from ic_moltabref where batchno = " + batch1 + " order by 1 desc";

            try
            {
                OraConn.Open();
                OracleDataReader dr = comm.ExecuteReader();

                while (dr.Read())
                {
                    if (listBox1.FindStringExact(dr.GetString(0)) == ListBox.NoMatches)
                        listBox1.Items.Add(dr.GetString(0));
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraConn.Close();
            }
            OraConn.Close();

            OraConn.ConnectionString = ccrpdnConnectionString;
            comm.Connection = OraConn;
            comm.CommandType = CommandType.Text;
            comm.CommandText = "select unique acsn from ccr_batch where batchno = " + batch1 + " order by 1 desc";

            if (batch1 == "111111") comm.CommandText = "select unique acsn from ccr_batch where batchno is null order by 1 desc";
            else comm.CommandText = "select unique acsn from ccr_batch where batchno = " + batch1 + " order by 1 desc";


            try
            {
                OraConn.Open();
                OracleDataReader dr = comm.ExecuteReader();

                while (dr.Read())
                {
                    if (listBox1.FindStringExact(dr.GetString(0)) == ListBox.NoMatches)
                        listBox1.Items.Add(dr.GetString(0));
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraConn.Close();
            }
            OraConn.Close();

            Cursor = Cursors.Default;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string sacsn = "'";

            Cursor = Cursors.WaitCursor;

            for (int i = 0; i < listBox1.SelectedItems.Count; i++)
            {
                sacsn = sacsn + listBox1.SelectedItems[i].ToString() + "','";
            }

            if (sacsn.Length > 3) sacsn = sacsn.Substring(0, sacsn.Length - 2);

            int ebatch = 0;
            try
            {
                ebatch = (int)Convert.ToDecimal(comboBox4.Text);
            }
            catch (Exception ex)
            {
                ;
            }

            if (sacsn.Length > 2 && ebatch > 200000 && ebatch < 210000)
            {

                if (OraConn.State == ConnectionState.Open) OraConn.Close();
                OraConn.ConnectionString = icpdnConnectionString;
                comm.Connection = OraConn;
                comm.CommandType = CommandType.Text;

                try
                {
                    OraConn.Open();
                    comm.CommandText = "update ic_moltable set batchno = " + ebatch + " where acsn in (" + sacsn + ")";
                    comm.ExecuteNonQuery();
                    //                    comm.CommandText = "update ic_molref set batchno = " + ebatch + " where acsn in (" + sacsn + ")";
                    //                    comm.ExecuteNonQuery();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    OraConn.Close();
                }
                OraConn.Close();

                OraConn.ConnectionString = ccrpdnConnectionString;
                comm.CommandText = "update ccr_batch set batchno = " + ebatch + " where acsn in (" + sacsn + ")";

                try
                {
                    OraConn.Open();
                    comm.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    OraConn.Close();
                }

                OraConn.Close();

                string c3txt = comboBox3.Text;
                rflst1(comboBox3.Text);

                rflst2(ebatch.ToString());

                rpcomb();

                comboBox3.Text = c3txt;
                int i = 1;
                while (sacsn.Length > i + 5)
                {
                    string iacsn = sacsn.Substring(i, 5);
                    int im = listBox2.FindStringExact(iacsn);
                    listBox2.SetSelected(im, true);
                    i += 8;
                }
            }

            Cursor = Cursors.Default;
        }



        private void comboBox4_TextChanged(object sender, EventArgs e)
        {
            int ebatch = 0;
            try
            {
                ebatch = (int)Convert.ToDecimal(comboBox4.Text);
            }
            catch (Exception ex)
            {
                ;
            }

            if (ebatch > 200000 && ebatch < 210000)
            {
                rflst2(ebatch.ToString());

            }
        }


        private void rflst2(string ebatch)
        {
            listBox2.Items.Clear();
            if (OraConn.State == ConnectionState.Open) OraConn.Close();
            OraConn.ConnectionString = icpdnConnectionString;
            comm.Connection = OraConn;
            comm.CommandType = CommandType.Text;
            comm.CommandText = "select unique acsn from ic_moltabref where batchno = " + ebatch + " order by 1 desc";

            try
            {
                OraConn.Open();
                OracleDataReader dr = comm.ExecuteReader();

                while (dr.Read())
                {
                    listBox2.Items.Add(dr.GetString(0));
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraConn.Close();
            }
            OraConn.Close();

            OraConn.ConnectionString = ccrpdnConnectionString;
            comm.Connection = OraConn;
            comm.CommandType = CommandType.Text;

            if (ebatch == "111111") comm.CommandText = "select unique acsn from ccr_batch where batchno is null order by 1 desc";
            else comm.CommandText = "select unique acsn from ccr_batch where batchno = " + ebatch + " order by 1 desc";

            try
            {
                OraConn.Open();
                OracleDataReader dr = comm.ExecuteReader();

                while (dr.Read())
                {
                    if (listBox2.FindStringExact(dr.GetString(0)) == ListBox.NoMatches)
                        listBox2.Items.Add(dr.GetString(0));
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraConn.Close();
            }
            OraConn.Close();


        }



        private void comboBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsNumber(e.KeyChar)) e.Handled = true;
        }

        private void panel4_VisibleChanged(object sender, EventArgs e)
        {
            if (panel4.Visible == true)
            {
                comboBox5.Items.Clear();

                dataGridView4.Visible = false;
                label6.Visible = false;
                panel5.Visible = false;


                if (OraConn.State == ConnectionState.Open) OraConn.Close();
                OraConn.ConnectionString = icpdnConnectionString;
                comm.Connection = OraConn;
                comm.CommandType = CommandType.Text;
                comm.CommandText = "select unique batchno from ic_moltable order by 1 desc";

                try
                {
                    OraConn.Open();
                    OracleDataReader dr = comm.ExecuteReader();

                    while (dr.Read())
                    {
                        comboBox5.Items.Add(dr.GetDecimal(0));
                    }
                    dr.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    OraConn.Close();
                }

            }
        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            dupTable.Rows.Clear();

            comm.CommandType = CommandType.Text;
            comm.CommandText = "select * from (select m1.acsn || lpad(m1.artlnum, 6, ' ') || lpad(m1.authnum, 6, ' ')cpd, m2.cpd_number, m1.dupinmaster dup, m1.regno, molfile(m1.ctab) ml1, molfile(m2.ctab) ml2, flexmatchtimeout(99) Timeout from ic_moltable m1, ic_wos.moltable m2 where flexmatch(m2.ctab, m1.ctab, 'ALL', 99)=1 and batchno =" + comboBox5.Text + ") where Timeout=0 and regno in ( select regno from ic_moltable minus select regno from ic_biolact)";

            oda.SelectCommand = comm;

            oda.Fill(dupTable);

            OraConn.Close();

            if (dupTable.Rows.Count == 0)
            {
                dataGridView4.Visible = false;
                panel5.Visible = false;
                label6.Visible = true;
                label6.Text = "No Duplicates Found";
            }
            else
            {
                dataGridView4.Visible = true;
                panel5.Visible = true;
                dataGridView4.DataSource = dupTable;


                dataGridView4.Columns[2].Visible = false;
                dataGridView4.Columns[3].Visible = false;
                dataGridView4.Columns[4].Visible = false;
                dataGridView4.Columns[5].Visible = false;
                dataGridView4.Columns[6].Visible = false;
                //                dataGridView4.Columns.Remove("dup");

                if (!dataGridView4.Columns.Contains("dupcheck"))
                {
                    DataGridViewCheckBoxColumn myCheck = new DataGridViewCheckBoxColumn();
                    myCheck.HeaderText = "Duplicate_Check";
                    myCheck.Name = "dupcheck";
                    myCheck.DataPropertyName = "dup";
                    myCheck.FalseValue = "0";
                    myCheck.TrueValue = "1";
                    dataGridView4.AutoGenerateColumns = false;
                    dataGridView4.Columns.Insert(7, myCheck);
                }
                dataGridView4.Columns[0].ReadOnly = true;
                dataGridView4.Columns[1].ReadOnly = true;

                dataGridView4.Columns[0].HeaderText = "ICPDN_Compounds";
                dataGridView4.Columns[0].Width = 110;
                dataGridView4.Columns[1].HeaderText = "Master_CPD_Number";
                dataGridView4.Columns[1].Width = 115;
                dataGridView4.Columns[7].Width = 95;

                dataGridView4.CurrentCell = dataGridView4[1, 0];

            }

            Cursor = Cursors.Default;

        }

        private void dataGridView4_CurrentCellChanged(object sender, EventArgs e)
        {

            if (dataGridView4.CurrentRow != null)
            {
                renditor1.MolfileString = dataGridView4.CurrentRow.Cells[4].Value.ToString();
                renditor2.MolfileString = dataGridView4.CurrentRow.Cells[5].Value.ToString();
            }

        }

        private void button4_Click(object sender, EventArgs e)
        {
            string mdup = "";
            string ndup = "";

            DataView dv1 = new DataView(dupTable, "", "", DataViewRowState.ModifiedCurrent);

            for (int i = 0; i < dv1.Count; i++)
            {
                if (dv1[i].Row["dup"].ToString() != dv1[i].Row["dup", DataRowVersion.Original].ToString())
                {

                    if (dv1[i].Row["dup"].ToString() == "0") ndup = ndup + "," + dv1[i].Row["regno"].ToString();
                    else mdup = mdup + "," + dv1[i].Row["regno"].ToString();
                }
            }

            if (OraConn.State == ConnectionState.Open) OraConn.Close();
            OraConn.ConnectionString = icpdnConnectionString;
            comm.Connection = OraConn;
            comm.CommandType = CommandType.Text;

            if (mdup.Length > 0)
            {
                comm.CommandText = "update ic_moltable set dupinmaster =1 where regno in (" + mdup.Substring(1) + ")";
                comm.ExecuteNonQuery();
            }

            if (ndup.Length > 0)
            {
                comm.CommandText = "update ic_moltable set dupinmaster =0 where regno in (" + ndup.Substring(1) + ")";
                comm.ExecuteNonQuery();
            }

            OraConn.Close();

            dupTable.AcceptChanges();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string bibfile = "";
            string cpdfile = "";


            //            MessageBox.Show(textBox1.Text);
            try
            {
                StreamReader SR = File.OpenText(textBox1.Text);
                bibfile = SR.ReadToEnd();
                SR.Close();

                SR = File.OpenText(textBox2.Text);
                cpdfile = SR.ReadToEnd();
                SR.Close();

            }
            catch (IOException exp)
            {
                MessageBox.Show(exp.ToString());

            }

        }

        private void panel6_VisibleChanged(object sender, EventArgs e)
        {
            if (panel6.Visible == true)
            {

                Application.DoEvents();
                Refresh();

                fillabsnumlist();
            }
        }

        private void fillabsnumlist()
        {
            if (OraConn.State == ConnectionState.Open) OraConn.Close();

            DataSet molrgset = new DataSet();
            OracleDataAdapter molAdapter = new OracleDataAdapter();

            OraConn.ConnectionString = icwosConnectionString;
            comm.Connection = OraConn;


            label9.Text = "Fetching Article List..........";
            label10.Text = "";
            Refresh();

            Cursor = Cursors.WaitCursor;

            OraConn.Open();
            comm.CommandType = CommandType.Text;
            comm.CommandText = "select absnum from icann.article minus select absnum from cpd_link_ic_ut order by 1";
            OracleDataReader dr = comm.ExecuteReader();

            while (dr.Read())
            {
                listBox3.Items.Add(dr.GetString(0));
            }
            dr.Close();


            comm.CommandText = "select absnum from icann.article intersect select absnum from cpd_link_ic_ut order by 1 desc";
            dr = comm.ExecuteReader();

            while (dr.Read())
            {
                listBox2.Items.Add(dr.GetString(0));
            }

            label9.Text = "ICANNUAL";
            label10.Text = "IC_WOS";

            dr.Close();
            OraConn.Close();

            button1.Visible = true;
            Cursor = Cursors.Default;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (listBox3.Items.Count == 0) return;

            button8.Visible = false;
            button7.Visible = true;
            go = true;

            //File.Delete("\\\\tshuspaphichm01\\mdl\\IC_WOS\\ic_inchi.sdf");

            string abs1 = listBox3.Items[0].ToString();
            string abs2 = listBox3.Items[listBox3.Items.Count - 1].ToString();
            MessageBox.Show(abs1 + "  " + abs2);
            while (listBox3.Items.Count > 0 && go)
            {
                Cursor = Cursors.WaitCursor;

                artload(listBox3.Items[0].ToString());
                listBox4.Items.Insert(0, listBox3.Items[0].ToString());
                listBox3.Items.RemoveAt(0);

                Refresh();
                Application.DoEvents();
                Cursor = Cursors.Default;

            }

            label11.Text = "Updating IC_WOS.......";
            label11.Visible = true;
            /* No need for inchi and autonom
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.EnableRaisingEvents = false;
            proc.StartInfo.FileName = "\\\\tshuspaphichm01\\mdl\\IC_WOS\\inchi_pdn.bat";
            proc.Start();
            */
            comm.CommandType = CommandType.StoredProcedure;
            comm.CommandText = "ANNCPDLOAD";
            comm.Parameters.Add("abs1", OracleDbType.Varchar2).Value = abs1;
            comm.Parameters.Add("abs2", OracleDbType.Varchar2).Value = abs2;

            try
            {
                if (OraConn.State == ConnectionState.Open) OraConn.Close();
                OraConn.Open();
                comm.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                comm.Parameters.Clear();
                OraConn.Close();
            }
            comm.Parameters.Clear();
            OraConn.Close();

            label11.Text = "Updating IC_WOS.......DONE!";
            button8.Visible = true;

        }


        private void artload(string absnum)
        {
            ds.Clear();
            ds.AcceptChanges();

            try
            {
                string sdfile = "";

                if (OraConn.State == ConnectionState.Open) OraConn.Close();
                OraConn.ConnectionString = icwosConnectionString;

                comm.Connection = OraConn;

                OraConn.Open();
                comm.CommandType = CommandType.Text;
                comm.CommandText = "select cpdnumber, molfile(ctab) molstr, '' smiles from icann.ic_moltable where cpdnumber like '" + absnum + "%' order by 1";
                //                comm.CommandText = "select cpdnumber, molfile(ctab) molstr, '' smiles from moltable where absnum =" + absnum + " order by 1";
                OracleDataReader dr = comm.ExecuteReader();

                //                ds.Tables.Add(moltable);
                ds.Load(dr, LoadOption.OverwriteChanges, "moltable");

                dr.Close();
                OraConn.Close();

                progressBar1.Value = 100;
                for (int i = 0; i < ds.Tables["moltable"].Rows.Count; i++)
                {
                    sdfile = sdfile + ds.Tables["moltable"].Rows[i][1].ToString() + ">  <CPD_NUMBER>\r\n" + ds.Tables["moltable"].Rows[i][0].ToString() + "\r\n\r\n$$$$\r\n";

                    progressBar1.Value = 100 - (100 * (i + 1)) / ds.Tables["moltable"].Rows.Count;
                    label3.Text = i + "     " + progressBar1.Value;
                    Refresh();
                    Application.DoEvents();

                }

                //using (StreamWriter w = File.AppendText("\\\\tshuspaphichm01\\mdl\\IC_WOS\\ic_inchi.sdf"))
                //{
                //    w.Write(sdfile);
                //    w.Close();
                //}
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

        }


        private void button7_Click(object sender, EventArgs e)
        {
            go = false;
        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            string host = textBox4.Text;
            string port = textBox5.Text;
            string service = textBox6.Text;


            if (host.Length * port.Length * service.Length > 0)
            {
                settings.host = host;
                settings.port = port;
                settings.service = service;

                connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + host + ")(PORT=" + port + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + service + ")));";
                settings.Save();

                icpdnConnectionString = connectionString + "User Id=icpdn;Password=icpdn;";
                ccrpdnConnectionString = connectionString + "User Id=ccrpdn;Password=ccrpdn;";
                icannConnectionString = connectionString + "User Id=icann;Password=icann;";
                icwosConnectionString = connectionString + "User Id=ic_wos;Password=ic_wos;";
            }
        }

        private void panel7_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }




    }
}
