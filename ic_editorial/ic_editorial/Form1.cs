using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Drawing.Printing;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using ic_editorial.Properties;

namespace ic_editorial
{
    public partial class Form1 : Form
    {
        static private string icpdnConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1550)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=chemdb.prod-wos.com)));User Id=icpdn;Password=icpdn;";

        static private OracleCommand comm1 = new OracleCommand();
        static private OracleConnection oraconn = new OracleConnection();
        static private DataSet cpdDataSet = new DataSet();
        static private OracleDataAdapter cpdAdapter = new OracleDataAdapter();

        static private BindingSource bs1 = new BindingSource();
        static private BindingSource bscpd = new BindingSource();
        static private DataView dvlst = new DataView();
        static private DataView dvshw = new DataView();

        static private ToolTip ttp1 = new ToolTip();
        static private ToolTip ttp2 = new ToolTip();
        static private ToolTip ttp3 = new ToolTip();
        static private ToolTip ttp4 = new ToolTip();

        static private string sbat = "", sacs = "", sart = "", saut = "";

        static private String textToPrint;
        static private string strPrint = "";
        static private Properties.Settings settings = Properties.Settings.Default;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string host = settings.host;
            string port = settings.port;
            string service = settings.service;

            textBox2.Text = host;
            textBox3.Text = port;
            textBox4.Text = service;

            if (host.Length * port.Length * service.Length > 0)
            {
                icpdnConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + host + ")(PORT=" + port + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + service + "))); User Id=icpdn;Password=icpdn;";
            }

            panel5.Visible = false;


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

            oraconn.ConnectionString = icpdnConnectionString;
            comm1.Connection = oraconn;

            refillbatch();
            if (combatch.Items.Count > 0) { combatch.SelectedIndex = 0; }

            dupinit();
        }

        protected void MMEditClick(object who, EventArgs e)
        {
            panel5.Visible = false;
        }

        protected void MMConfigClick(object who, EventArgs e)
        {
            panel5.Visible = true;
        }

        protected void MMExitClick(object who, EventArgs e)
        {
            Application.Exit();
        }

        private void dupinit()
        {
            if (oraconn.State != ConnectionState.Open) oraconn.Open();

            comm1.CommandText = "select 'a' master_regno, 'a' data, 1 dup from dual";
            cpdAdapter.Fill(cpdDataSet, "dpt");
            cpdDataSet.Tables["dpt"].Rows.Clear();

            dataGridView1.DataSource = cpdDataSet.Tables["dpt"];
            dataGridView1.Columns[1].Visible = false;

            dataGridView1.Columns.Remove("dup");
            DataGridViewCheckBoxColumn myCheck = new DataGridViewCheckBoxColumn();
            myCheck.HeaderText = "Duplicate";
            myCheck.DataPropertyName = "dup";
            myCheck.FalseValue = "0";
            myCheck.TrueValue = "1";
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.Columns.Insert(2, myCheck);
            oraconn.Close();

        }

        private void combatch_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combatch.SelectedIndex < 0) return;

            setbatchttp();

            refillacsn();

            if (comacsn.Items.Count == 0) { sacs = ""; comacsn.SelectedIndex = -1; refillbatch(); }
            else if (sacs == "") comacsn.SelectedIndex = 0;
            else comacsn.SelectedIndex = comacsn.FindStringExact(sacs);
            sacs = "";

        }

        private void comacsn_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (comacsn.SelectedIndex < 0) return;

            setacsnttp();

            refillart();

            if (comart.Items.Count == 0)
            {
                int k = comacsn.SelectedIndex;
                refillacsn();
                if (k > 0) comacsn.SelectedIndex = k - 1;
            }
            else if (sart == "") comart.SelectedIndex = 0;
            else comart.SelectedIndex = comart.FindStringExact(sart);
            sart = "";

        }

        private void comart_SelectedIndexChanged(object sender, EventArgs e)
        {
            updtdb();

            if (comart.SelectedIndex < 0) return;

            setartttp();
            refillcpd();

            if (dataGridCpd.Rows.Count == 0)
            {
                int k = comart.SelectedIndex;
                refillart();
                if (k > 0) comart.SelectedIndex = k - 1;
            }

            dataGridCpd.Columns[0].Width = 48;
            dataGridCpd.Columns[1].Visible = false;
            dataGridCpd.Columns[2].Visible = false;
            dataGridCpd.Columns[3].Visible = false;
            dataGridCpd.Columns[4].Visible = false;
            dataGridCpd.Columns[5].Visible = false;
            dataGridCpd.Columns[6].Visible = false;
            dataGridCpd.Columns[7].Visible = false;

            comboChoice.SelectedIndex = -1;
            comboChoice.SelectedIndex = 0;

            for (int i = 0; i < dataGridCpd.RowCount; i++)
            {
                if (dataGridCpd.Rows[i].Cells[3].Value.ToString() == "1")
                    dataGridCpd.Rows[i].Cells[0].Style.BackColor = System.Drawing.Color.Red;
                else if (dataGridCpd.Rows[i].Cells[5].Value.ToString() == "1")
                {
                    dataGridCpd.Rows[i].Cells[0].Style.BackColor = System.Drawing.Color.LightGreen;
                    dataGridCpd.Rows[i].Cells[0].Style.SelectionBackColor = System.Drawing.Color.LawnGreen;
                }
            }

        }

        private void refillcpd()
        {
            if (cpdDataSet.Tables.Contains("cpd")) cpdDataSet.Tables["cpd"].Rows.Clear();
            if (cpdDataSet.Tables.Contains("aut")) cpdDataSet.Tables["aut"].Rows.Clear();
            if (cpdDataSet.Tables.Contains("sbj")) cpdDataSet.Tables["sbj"].Rows.Clear();
            if (cpdDataSet.Tables.Contains("grd")) cpdDataSet.Tables["grd"].Rows.Clear();

            if (oraconn.State == ConnectionState.Closed) oraconn.Open();

            comm1.CommandType = CommandType.Text;
            comm1.CommandText = "select authnum, regno, molfile(ctab) ml, dupinmaster dup, lpad(authnum, 3, ' ') asrt, lead, molwt(ctab) molwt, molfmla(ctab) fmla from ic_moltable where artlnum = " + comart.Text + " and acsn = '" + comacsn.Text + "' order by lpad(authnum, 3, ' ')";
            cpdAdapter.SelectCommand = comm1;
            cpdAdapter.Fill(cpdDataSet, "cpd");

            comm1.CommandText = "select r.authnum, r.regno, molfile(m.ctab) ml, r.dupinmaster dup, lpad(r.authnum, 3, ' ') asrt, r.lead, molwt(m.ctab) molwt, molfmla(m.ctab) fmla from ic_molref r, ic_moltable m where r.refno = m.regno and r.artlnum = " + comart.Text + " and r.acsn = '" + comacsn.Text + "' order by lpad(r.authnum, 3, ' ')";
            cpdAdapter.Fill(cpdDataSet, "cpd");


            comm1.CommandText = "select     '' authnum,                  'Subject Lst' tabl, actual_data data,     0 regno, 0 code,            0 idx, batchno || CHR(9) || acsn || CHR(9) || artlnum status, 0 seq from subject_index where prod_code='IC' and artlnum = " + comart.Text + " and acsn = '" + comacsn.Text + "' order by seq_num";
            cpdAdapter.Fill(cpdDataSet, "aut");
            comm1.CommandText = "select lpad(  authnum, 3, ' ') authnum, 'Grade'       tabl, grade data,             regno, 0 code,            0 idx, '' status, 1 seq from ic_moltable where artlnum = " + comart.Text + " and acsn = '" + comacsn.Text + "'";
            cpdAdapter.Fill(cpdDataSet, "aut");
            comm1.CommandText = "select lpad(m.authnum, 3, ' ') authnum, 'Symbol'      tabl, s.symbol data,        m.regno, 0 code,            s.idx, '' status, 2 seq from ic_moltable m, ic_symbol s where m.regno = s.regno and m.artlnum = " + comart.Text + " and m.acsn = '" + comacsn.Text + "'";
            cpdAdapter.Fill(cpdDataSet, "aut");
            comm1.CommandText = "select lpad(m.authnum, 3, ' ') authnum, 'Biolact'     tabl, s.status || '  ' || INITCAP(lower(p.BIOL_ACTIVITY)) data, m.regno, s.ba_code code,    s.idx,  s.status, 3 seq from ic_moltable m, ic_biolact s, ccrpdn.ba_main p where p.ba_code = s.ba_code and m.regno = s.regno and m.artlnum = " + comart.Text + " and m.acsn = '" + comacsn.Text + "'";
            cpdAdapter.Fill(cpdDataSet, "aut");
            comm1.CommandText = "select lpad(m.authnum, 3, ' ') authnum, 'Cpd_Class'   tabl, p.cpd_class data,     m.regno, s.class_id code,   s.idx, '' status, 4 seq from ic_moltable m, ic_cpd_class s, cpd_class p where s.class_id = p.class_id and m.regno = s.regno and m.artlnum = " + comart.Text + " and m.acsn = '" + comacsn.Text + "'";
            cpdAdapter.Fill(cpdDataSet, "aut");
            comm1.CommandText = "select lpad(m.authnum, 3, ' ') authnum, 'Use_Profile' tabl, p.USE_PROFILE data,   m.regno, s.profile_id code, s.idx, '' status, 5 seq from ic_moltable m, ic_use_profile s, use_profile p where s.profile_id=p.profile_id and m.regno = s.regno and m.artlnum = " + comart.Text + " and m.acsn = '" + comacsn.Text + "'";
            cpdAdapter.Fill(cpdDataSet, "aut");

            comm1.CommandText = "select lpad(  authnum, 3, ' ') authnum, 'Grade'       tabl, grade data,             regno, 0 code,            0 idx, '' status, 1 seq from ic_molref where artlnum = " + comart.Text + " and acsn = '" + comacsn.Text + "'";
            cpdAdapter.Fill(cpdDataSet, "aut");
            comm1.CommandText = "select lpad(m.authnum, 3, ' ') authnum, 'Symbol'      tabl, s.symbol data,        m.regno, 0 code,            s.idx, '' status, 2 seq from ic_molref m, ic_symbol s where m.regno = s.regno and m.artlnum = " + comart.Text + " and m.acsn = '" + comacsn.Text + "'";
            cpdAdapter.Fill(cpdDataSet, "aut");
            comm1.CommandText = "select lpad(m.authnum, 3, ' ') authnum, 'Biolact'     tabl, s.status || '  ' || INITCAP(lower(p.BIOL_ACTIVITY)) data, m.regno, s.ba_code code,    s.idx,  s.status, 3 seq from ic_molref m, ic_biolact s, ccrpdn.ba_main p where p.ba_code = s.ba_code and m.regno = s.regno and m.artlnum = " + comart.Text + " and m.acsn = '" + comacsn.Text + "'";
            cpdAdapter.Fill(cpdDataSet, "aut");
            comm1.CommandText = "select lpad(m.authnum, 3, ' ') authnum, 'Cpd_Class'   tabl, p.cpd_class data,     m.regno, s.class_id code,   s.idx, '' status, 4 seq from ic_molref m, ic_cpd_class s, cpd_class p where s.class_id = p.class_id and m.regno = s.regno and m.artlnum = " + comart.Text + " and m.acsn = '" + comacsn.Text + "'";
            cpdAdapter.Fill(cpdDataSet, "aut");
            comm1.CommandText = "select lpad(m.authnum, 3, ' ') authnum, 'Use_Profile' tabl, p.USE_PROFILE data,   m.regno, s.profile_id code, s.idx, '' status, 5 seq from ic_molref m, ic_use_profile s, use_profile p where s.profile_id=p.profile_id and m.regno = s.regno and m.artlnum = " + comart.Text + " and m.acsn = '" + comacsn.Text + "'";
            cpdAdapter.Fill(cpdDataSet, "aut");

            comm1.CommandText = "select 1 code, actual_data data, batchno, acsn, artlnum from subject_index where prod_code='IC' and artlnum = " + comart.Text + " and acsn = '" + comacsn.Text + "' order by seq_num";
            cpdAdapter.Fill(cpdDataSet, "sbj");

            comm1.CommandText = "select 0 code, grade, 1 chosen from (select unique grade from ic_moltable where grade is not null and artlnum = " + comart.Text + " and acsn = '" + comacsn.Text + "')";
            cpdAdapter.Fill(cpdDataSet, "grd");
            comm1.CommandText = "select 0 code, grade, 1 chosen from (select unique grade from ic_molref where grade is not null and artlnum = " + comart.Text + " and acsn = '" + comacsn.Text + "')";
            cpdAdapter.Fill(cpdDataSet, "grd");

            //oraconn.Close();

            bs1.DataSource = cpdDataSet.Tables["aut"];
            //            bs1.Filter = "";
            dataGridView0.DataSource = bs1;
            dataGridView0.AllowUserToAddRows = false;
            dataGridView0.Columns[0].Visible = false;
            dataGridView0.Columns[1].Width = 70;
            dataGridView0.Columns[1].ReadOnly = true;
            dataGridView0.Columns[2].Width = 340;

            DataGridViewTextBoxColumn dgvt = (DataGridViewTextBoxColumn)dataGridView0.Columns[2];
            dgvt.MaxInputLength = 250;

            dataGridView0.Columns[3].Visible = false;
            dataGridView0.Columns[4].Visible = false;
            dataGridView0.Columns[5].Visible = false;
            dataGridView0.Columns[6].Visible = false;
            dataGridView0.Columns[7].Visible = false;

            chooslst();
            decorate();
            label1.Text = cpdDataSet.Tables["cpd"].Rows.Count + " compounds";

            cpdDataSet.Tables["cpd"].Columns[0].MaxLength = 3;
            bscpd.DataSource = cpdDataSet.Tables["cpd"];
            bscpd.Sort = "asrt";
            dataGridCpd.DataSource = bscpd;
        }


        private void chooslst()
        {
            dvlst.Table = cpdDataSet.Tables["bamain"];
            dvlst.RowFilter = "CHOSEN=1";
            foreach (DataRowView myDataRowView in dvlst) myDataRowView.Row["CHOSEN"] = 0;

            dvlst.Table = cpdDataSet.Tables["prmain"];
            dvlst.RowFilter = "CHOSEN=1";
            foreach (DataRowView myDataRowView in dvlst) myDataRowView.Row["CHOSEN"] = 0;

            dvlst.Table = cpdDataSet.Tables["clmain"];
            dvlst.RowFilter = "CHOSEN=1";
            foreach (DataRowView myDataRowView in dvlst) myDataRowView.Row["CHOSEN"] = 0;

            DataView dv = new DataView(cpdDataSet.Tables["aut"], "", "", DataViewRowState.CurrentRows);
            for (int i = 0; i < dv.Count; i++)
            {

                if (dv[i].Row["tabl"].ToString() == "Biolact")
                {
                    DataRow[] foundRows = cpdDataSet.Tables["bamain"].Select("BA_CODE=" + dv[i].Row["CODE"].ToString());
                    for (int j = 0; j < foundRows.Length; j++)
                    {
                        foundRows[j]["CHOSEN"] = 1;
                    }
                }
                else if (dv[i].Row["tabl"].ToString() == "Use_Profile")
                {
                    DataRow[] foundRows = cpdDataSet.Tables["prmain"].Select("PROFILE_ID=" + dv[i].Row["CODE"].ToString());
                    for (int j = 0; j < foundRows.Length; j++)
                    {
                        foundRows[j]["CHOSEN"] = 1;
                    }
                }
                else if (dv[i].Row["tabl"].ToString() == "Cpd_Class")
                {
                    DataRow[] foundRows = cpdDataSet.Tables["clmain"].Select("CLASS_ID=" + dv[i].Row["CODE"].ToString());
                    for (int j = 0; j < foundRows.Length; j++)
                    {
                        foundRows[j]["CHOSEN"] = 1;
                    }
                }
            }

        }

        private void setartttp()
        {
            comm1.CommandType = CommandType.Text;
            comm1.CommandText = "select 'Authnum:' || authnum authnum from (select unique authnum from dupcheck where timeout=0 and artlnum = " + comart.Text + " and acsn = '" + comacsn.Text + "' order by lpad(authnum, 3, ' '))";
            if (oraconn.State != ConnectionState.Open) oraconn.Open();
            
            OracleDataReader dr = comm1.ExecuteReader();

            string tstr = "";
            while (dr.Read())
            {
                tstr = tstr + dr.GetString(0) + "\r\n";
            }

            if (tstr.Length > 0)
            {
                comart.BackColor = System.Drawing.Color.LightSkyBlue;
                Font fnt = new Font(comart.Font, FontStyle.Italic);
                comart.Font = fnt;

                ttp3.SetToolTip(comart, tstr);
                ttp3.ToolTipTitle = "Deleted Compounds";
            }
            else
            {
                ttp3.SetToolTip(comart, "");
                ttp3.ToolTipTitle = "";
                comart.BackColor = System.Drawing.Color.White;
                Font fnt = new Font(comart.Font, FontStyle.Regular);
                comart.Font = fnt;
            }
            ttp3.ReshowDelay = 0;
            ttp3.AutomaticDelay = 0;
            dr.Close();
            oraconn.Close();

        }

        private void refillart()
        {
            if (oraconn.State == ConnectionState.Closed) oraconn.Open();

            comm1.CommandText = "select unique artlnum from ic_moltable where acsn ='" + comacsn.Text + "' union select unique artlnum from ic_molref where acsn ='" + comacsn.Text + "' order by 1";
            OracleDataReader dr = comm1.ExecuteReader();
            comart.Items.Clear();

            int idx = 0;
            while (dr.Read())
            {
                comart.Items.Add(dr.GetDecimal(0));
                idx++;
            }
            dr.Close();
            //oraconn.Close();

        }

        private void setacsnttp()
        {
            comm1.CommandType = CommandType.Text;
            comm1.CommandText = "select 'artlnum: ' || artlnum || ' authnum:' || authnum auth from dupcheck where acsn = '" + comacsn.Text + "' and artlnum in (select artlnum from dupcheck where timeout=0 and acsn = '" + comacsn.Text + "' minus select artlnum from ic_moltable where acsn = '" + comacsn.Text + "') order by artlnum, authnum";
            if (oraconn.State != ConnectionState.Open) oraconn.Open();

            OracleDataReader dr = comm1.ExecuteReader();

            string tstr = "";
            while (dr.Read())
            {
                tstr = tstr + dr.GetString(0) + "\r\n";
            }

            if (tstr.Length > 0)
            {
                comacsn.BackColor = System.Drawing.Color.LightSkyBlue;
                ttp2.SetToolTip(comacsn, tstr);
                ttp2.ToolTipTitle = "Deleted articles";
            }
            else
            {
                ttp2.SetToolTip(comacsn, "");
                ttp2.ToolTipTitle = "";
                comacsn.BackColor = System.Drawing.Color.White;
            }
            ttp2.ReshowDelay = 0;
            ttp2.AutomaticDelay = 0;
            dr.Close();
            //oraconn.Close();

        }

        private void refillacsn()
        {
            if (oraconn.State == ConnectionState.Closed) oraconn.Open();

            comm1.CommandText = "select unique acsn from ic_moltable where batchno =" + combatch.Text + " union select unique acsn from ic_molref where batchno =" + combatch.Text + " order by 1";
            OracleDataReader dr = comm1.ExecuteReader();
            comacsn.Items.Clear();

            int idx = 0;
            while (dr.Read())
            {
                comacsn.Items.Add(dr.GetOracleString(0));
                idx++;
            }
            dr.Close();
            //oraconn.Close();

        }

        private void setbatchttp()
        {
            comm1.CommandType = CommandType.Text;
            comm1.CommandText = "select 'acsn:' || acsn || ' artlnum: ' || artlnum || ' authnum:' || authnum auth from dupcheck where acsn in (select acsn from dupcheck where timeout=0 and batchno = " + combatch.Text + " minus select acsn from ic_moltable) order by acsn, artlnum, authnum";
            if (oraconn.State != ConnectionState.Open) oraconn.Open();

            OracleDataReader dr = comm1.ExecuteReader();

            string tstr = "";
            while (dr.Read())
            {
                tstr = tstr + dr.GetString(0) + "\r\n";
            }

            if (tstr.Length > 0)
            {
                combatch.BackColor = System.Drawing.Color.LightSkyBlue;
                ttp1.SetToolTip(combatch, tstr);
                ttp1.ToolTipTitle = "Deleted journals";
            }
            else
            {
                ttp1.SetToolTip(combatch, "");
                ttp1.ToolTipTitle = "";
                combatch.BackColor = System.Drawing.Color.White;
            }
            ttp1.ReshowDelay = 0;
            ttp1.AutomaticDelay = 0;
            dr.Close();
            //oraconn.Close();

        }

        private void refillbatch()
        {
            if (oraconn.State == ConnectionState.Closed) oraconn.Open();

            comm1.CommandText = "select unique batchno from ic_moltable order by 1 desc";
            comm1.CommandType = CommandType.Text;

            OracleDataReader dr = comm1.ExecuteReader();

            combatch.Items.Clear();
            int idx = 0;
            while (dr.Read())
            {
                combatch.Items.Add(dr.GetDecimal(0));
                idx++;
            }
            dr.Close();

            if (cpdDataSet.Tables.Contains("bamain")) cpdDataSet.Tables["bamain"].Rows.Clear();
            if (cpdDataSet.Tables.Contains("prmain")) cpdDataSet.Tables["prmain"].Rows.Clear();
            if (cpdDataSet.Tables.Contains("clmain")) cpdDataSet.Tables["clmain"].Rows.Clear();

            cpdAdapter.SelectCommand = comm1;
            comm1.CommandText = "select b.ba_code, b.biol_activity biolact, 0 chosen from ccrpdn.ba_main b order by 2";
            cpdAdapter.Fill(cpdDataSet, "bamain");
            comm1.CommandText = "select p.*, 0 chosen from use_profile p order by 2";
            cpdAdapter.Fill(cpdDataSet, "prmain");
            comm1.CommandText = "select c.*, 0 chosen from cpd_class c order by 2";
            cpdAdapter.Fill(cpdDataSet, "clmain");

            //oraconn.Close();

        }

        private void comboChoice_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = comboChoice.SelectedIndex;

            if (idx < 0) return;
            else if (idx == 0)
            {
                dataGridCpd.MultiSelect = false;
                dataGridCpd.ReadOnly = false;
                dataGridCpd.ContextMenuStrip = contextMenuStrip1;
                dataGridView0.Visible = true;
                panel4.Visible = false;
                //                dataGridView2.AllowUserToAddRows = true;
                button4.Visible = true;
                button5.Visible = true;
                decorate();
            }
            else
            {
                button4.Visible = false;
                button5.Visible = false;
                panel2.Visible = false;
                dataGridCpd.MultiSelect = true;
                dataGridCpd.ReadOnly = true;
                dataGridCpd.ContextMenuStrip = null;
                dataGridView0.Visible = false;
                panel4.Visible = true;
                dataGridView2.DataSource = null;
                dataGridView2.AllowUserToAddRows = false;

                dvshw.Table = cpdDataSet.Tables["aut"];
                dvshw.Sort = "authnum";
                dataGridLst.DataSource = dvshw;
                dataGridLst.Columns[0].Width = 48;
                dataGridLst.Columns[1].Visible = false;
                dataGridLst.Columns[2].Visible = false;
                dataGridLst.Columns[3].Visible = false;
                dataGridLst.Columns[4].Visible = false;
                dataGridLst.Columns[5].Visible = false;
                dataGridLst.Columns[6].Visible = false;
                dataGridLst.Columns[7].Visible = false;

                comboLst.DataSource = null;
                dvshw.RowFilter = "";
                dvlst.Table = null;

                if (idx == 1)
                {
                    dvshw.RowFilter = "tabl='grade'";
                    dvlst.Table = cpdDataSet.Tables["grd"];
                    dataGridView2.ReadOnly = false;
                    dataGridView2.AllowUserToAddRows = true;
                    combosta.Visible = false;
                    comboLst.Visible = false;
                }
                else if (idx == 2)
                {
                    dataGridView2.ReadOnly = true;
                    combosta.Visible = true;
                    comboLst.Visible = true;

                    combosta.SelectedIndex = 0;
                    comboLst.ValueMember = cpdDataSet.Tables["bamain"].Columns["BA_CODE"].ToString();
                    comboLst.DisplayMember = cpdDataSet.Tables["bamain"].Columns["BIOLACT"].ToString();
                    comboLst.DataSource = cpdDataSet.Tables["bamain"].DefaultView;

                    dvlst.Table = cpdDataSet.Tables["bamain"];

                }
                else if (idx == 3)
                {
                    dataGridView2.ReadOnly = true;
                    combosta.Visible = false;
                    comboLst.Visible = true;
                    comboLst.DisplayMember = cpdDataSet.Tables["clmain"].Columns["CPD_CLASS"].ToString();
                    comboLst.ValueMember = cpdDataSet.Tables["clmain"].Columns["CLASS_ID"].ToString();
                    comboLst.DataSource = cpdDataSet.Tables["clmain"].DefaultView;

                    dvlst.Table = cpdDataSet.Tables["clmain"];
                }
                else if (idx == 4)
                {
                    dataGridView2.ReadOnly = true;
                    combosta.Visible = false;
                    comboLst.Visible = true;
                    comboLst.DisplayMember = cpdDataSet.Tables["prmain"].Columns["USE_PROFILE"].ToString();
                    comboLst.ValueMember = cpdDataSet.Tables["prmain"].Columns["PROFILE_ID"].ToString();
                    comboLst.DataSource = cpdDataSet.Tables["prmain"].DefaultView;

                    dvlst.Table = cpdDataSet.Tables["prmain"];
                }


                dvlst.RowFilter = "CHOSEN=1";
                dataGridView2.DataSource = dvlst;

                dataGridView2.Columns[0].Visible = false;
                dataGridView2.Columns[1].Width = 350;
                dataGridView2.Columns[2].Visible = false;
                if (dataGridView2.Rows.Count > 0)
                {
                    dataGridView2.CurrentCell = dataGridView2[1, 0];
                    dataGridView2.CurrentCell.Selected = false;
                }

            }
        }

        private void dataGridCpd_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridCpd.CurrentCell == null)
            {
                renditor1.MolfileString = "";
                return;
            }

            //            DataGridViewTextBoxEditingControl editControl = (DataGridViewTextBoxEditingControl)dataGridCpd.EditingControl;
            //            editControl.SelectionLength = 0;

            dataGridCpd.CurrentCell.ReadOnly = true;



            renditor1.MolfileString = dataGridCpd.CurrentRow.Cells[2].Value.ToString();
            label2.Text = dataGridCpd.CurrentRow.Cells[6].Value.ToString();
            label3.Text = dataGridCpd.CurrentRow.Cells[7].Value.ToString();
            if (dataGridCpd.CurrentRow.Cells[3].Value.ToString() == "1") { chkdup(); }
            else panel3.Visible = false;

            bs1.Filter = "regno = 0 or regno=" + dataGridCpd.CurrentRow.Cells[1].Value.ToString();
            bs1.Sort = "seq";
            
            decorate();

            if (dataGridView0.RowCount > 0) dataGridView0.CurrentCell = dataGridView0[1, 0];
        }

        private void decorate()
        {
            if (dataGridView0.RowCount > 0)
                for (int j = 0; j < dataGridView0.RowCount; j++)
                {
                    //                    dataGridView0.Rows[j].ReadOnly = true;
                    string stype = "";
                    if (dataGridView0.Rows[j].Cells[1].Value != null)
                        stype = dataGridView0.Rows[j].Cells[1].Value.ToString();

                    if (stype == "Use_Profile")
                    {
                        dataGridView0.Rows[j].ReadOnly = true;
                        dataGridView0.Rows[j].Cells[1].Style.BackColor = System.Drawing.Color.Salmon;
                    }
                    else if (stype == "Cpd_Class")
                    {
                        dataGridView0.Rows[j].ReadOnly = true;
                        dataGridView0.Rows[j].Cells[1].Style.BackColor = System.Drawing.Color.Bisque;
                    }
                    else if (stype == "Biolact")
                    {
                        dataGridView0.Rows[j].ReadOnly = true;
                        dataGridView0.Rows[j].Cells[1].Style.BackColor = System.Drawing.Color.BlueViolet;
                    }
                    else if (stype == "Symbol")
                    {
                        dataGridView0.Rows[j].ReadOnly = false;
                        dataGridView0.Rows[j].Cells[2].Style.Font = new Font("Arial", 12);
                        dataGridView0.Rows[j].Height = 30;
                        dataGridView0.Rows[j].Cells[1].Style.BackColor = System.Drawing.Color.SandyBrown;
                        DataGridViewTextBoxCell dgvt = (DataGridViewTextBoxCell)dataGridView0.Rows[j].Cells[2];
                        dgvt.MaxInputLength = 250;
                    }
                    else if (stype == "Grade")
                    {
                        dataGridView0.Rows[j].ReadOnly = false;
                        dataGridView0.Rows[j].Cells[2].Style.Font = new Font("Arial", 12);
                        dataGridView0.Rows[j].Height = 30;
                        dataGridView0.Rows[j].Cells[1].Style.BackColor = System.Drawing.Color.Aqua;
                        DataGridViewTextBoxCell dgvt = (DataGridViewTextBoxCell)dataGridView0.Rows[j].Cells[2];
                        dgvt.MaxInputLength = 100;

                    }
                    else if (stype == "Subject Lst")
                    {
                        dataGridView0.Rows[j].ReadOnly = false;
                        dataGridView0.Rows[j].Cells[2].Style.Font = new Font("Arial", 12);
                        dataGridView0.Rows[j].Height = 30;
                        dataGridView0.Rows[j].Cells[1].Style.BackColor = System.Drawing.Color.Khaki;
                        DataGridViewTextBoxCell dgvt = (DataGridViewTextBoxCell)dataGridView0.Rows[j].Cells[2];
                        dgvt.MaxInputLength = 80;
                    }
                }

        }

        private void chkdup()
        {
            cpdDataSet.Tables["dpt"].Rows.Clear();
            comm1.CommandType = CommandType.Text;
            comm1.CommandText = "select cdbregno master_regno, molfile(ctab) data, 0 dup from dupcheck where timeout=1 and acsn = '" + comacsn.Text + "' and artlnum = " + comart.Text + " and authnum = :1";
            comm1.Parameters.Add("auth", OracleDbType.Varchar2).Value = dataGridCpd.CurrentRow.Cells[0].Value.ToString();
            if (oraconn.State != ConnectionState.Open) oraconn.Open();

            cpdAdapter.Fill(cpdDataSet, "dpt");
            if (oraconn.State == ConnectionState.Open) oraconn.Close();

            comm1.Parameters.Clear();

            dataGridView1.CurrentCell = null;
            panel3.Visible = true;
            panel3.BringToFront();
            dataGridView1.CurrentCell = dataGridView1[0, 0];

        }

        private void button4_Click(object sender, EventArgs e)
        {

            if (button4.Text == "Cancel Compound")
            {
                button4.Text = "New Compound";
                panel2.Visible = false;
            }
            else
            {
                textbat.Text = combatch.Text;
                textacsn.Text = comacsn.Text;
                textart.Text = comart.Text;
                textaut.Text = "";
                panel2.Visible = true;
                button4.Text = "Cancel Compound";
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {

            if (panel2.Visible == true)
            {
                sbat = textbat.Text;
                sacs = textacsn.Text;
                sart = textart.Text;
                saut = textaut.Text;

                int m = instdbmol();

                updtdb();
                cpdDataSet.AcceptChanges();

                panel2.Visible = false;
                button4.Text = "New Compound";

                if (sbat != combatch.Text)
                {
                    int k = combatch.FindStringExact(sbat);
                    if (k < 0) refillbatch();
                    combatch.SelectedIndex = combatch.FindStringExact(sbat); sbat = "";
                }
                else if (sacs != comacsn.Text)
                {
                    m = combatch.SelectedIndex;
                    combatch.SelectedIndex = -1;
                    combatch.SelectedIndex = m;
                }
                else
                {
                    int k = comart.FindStringExact(sart); sart = "";
                    if (k < 0)
                    {
                        m = comacsn.SelectedIndex;
                        comacsn.SelectedIndex = -1;
                        comacsn.SelectedIndex = m;
                    }
                    else if (k == comart.SelectedIndex)
                    {
                        comart.SelectedIndex = -1;
                        comart.SelectedIndex = k;
                    }
                }
            }
            else
            {
                int cidx = dataGridCpd.CurrentRow.Index;
                int aidx = comart.SelectedIndex;

                int m = updtdbmol();
                updtdb();
                cpdDataSet.AcceptChanges();

                comart.SelectedIndex = -1;
                comart.SelectedIndex = aidx;

                dataGridCpd.CurrentCell = dataGridCpd[0, cidx];
            }
        }

        private int instdbmol()
        {
            if (textbat.Text.Length != 6 || textacsn.Text.Length != 5 || textart.Text.Length > 3 || textart.Text.Length == 0 || textaut.Text.Length > 3 || textaut.Text.Length == 0)
            {
                MessageBox.Show("Invalid Text!");
                return 1;
            }

            string sdb = "\tMOLECULE\tINSERT\t" + dataGridCpd.CurrentRow.Cells[1].Value.ToString() + "\t" + renditor1.MolfileString + "\t" + textacsn.Text + "\t" + textart.Text + "\t" + textaut.Text + "\t" + textbat.Text + "\t\t\t\t\t\r\n";
            sdb = sdb + "\t\t\t\t\t\r\n";

            try
            {
                if (oraconn.State != ConnectionState.Open) oraconn.Open();

                comm1.Connection = oraconn;

                comm1.Parameters.Add("clobtodb", OracleDbType.Clob).Value = sdb + "\r\n";
                comm1.CommandType = CommandType.StoredProcedure;
                comm1.CommandText = "moleditproc";

                comm1.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                comm1.Parameters.Clear();
                oraconn.Close();
                return 1;
            }
            comm1.Parameters.Clear();
            oraconn.Close();

            return 0;
        }

        private int dltdbmol()
        {
            string sdb = "\tMOLECULE\tDELETE\t" + dataGridCpd.CurrentRow.Cells[1].Value.ToString() + "\t\t\t\t\t\t\r\n";
            sdb = sdb + "\t\t\t\t\t\r\n";

            try
            {
                if (oraconn.State != ConnectionState.Open) oraconn.Open();

                comm1.Connection = oraconn;

                comm1.Parameters.Add("clobtodb", OracleDbType.Clob).Value = sdb + "\r\n";
                comm1.CommandType = CommandType.StoredProcedure;
                comm1.CommandText = "moleditproc";

                comm1.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString().Substring(0, 114));
                comm1.Parameters.Clear();
                oraconn.Close();

                return 1;
            }
            comm1.Parameters.Clear();
            oraconn.Close();

            return 0;
        }

        private int updtdbmol()
        {
            string sdb = "\tMOLECULE\tUPDATE\t" + dataGridCpd.CurrentRow.Cells[1].Value.ToString() + "\t" + renditor1.MolfileString + "\t\t\t\t\t\r\n";
            sdb = sdb + "\t\t\t\t\t\r\n";

            try
            {
                if (oraconn.State != ConnectionState.Open) oraconn.Open();
                comm1.Connection = oraconn;

                comm1.Parameters.Add("clobtodb", OracleDbType.Clob).Value = sdb + "\r\n";
                comm1.CommandType = CommandType.StoredProcedure;
                comm1.CommandText = "moleditproc";

                comm1.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString().Substring(0, 114));
                comm1.Parameters.Clear();
                oraconn.Close();
                return 1;
            }
            comm1.Parameters.Clear();
            oraconn.Close();

            return 0;
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.Visible = false;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ToolStripDropDownItem menu = (ToolStripDropDownItem)sender;
            ContextMenuStrip strip = (ContextMenuStrip)menu.Owner;
            Control owner = strip.SourceControl;

            if (owner.Name == "combatch")
            {
                textBox1.Text = ttp1.ToolTipTitle + ":\r\n\r\n" + ttp1.GetToolTip(combatch);
                textBox1.Left = combatch.Left;
            }
            else if (owner.Name == "comacsn")
            {
                textBox1.Text = ttp2.ToolTipTitle + ":\r\n\r\n" + ttp2.GetToolTip(comacsn);
                textBox1.Left = comacsn.Left;
            }
            else if (owner.Name == "comart")
            {
                textBox1.Text = ttp3.ToolTipTitle + ":\r\n\r\n" + ttp3.GetToolTip(comart);
                textBox1.Left = comart.Left;
            }

            if (textBox1.Text.Length > 5) { textBox1.BringToFront(); textBox1.Visible = true; }

        }


        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            DataRow dr = cpdDataSet.Tables["aut"].NewRow();
            dr["tabl"] = "Symbol";
            dr["regno"] = dataGridCpd.CurrentRow.Cells[1].Value.ToString();
            dr["seq"] = 2;

            cpdDataSet.Tables["aut"].Rows.Add(dr);
            decorate();
        }

        private void addSubjectListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataRow dr = cpdDataSet.Tables["aut"].NewRow();
            dr["tabl"] = "Subject Lst";
            dr["regno"] = 0;
            dr["seq"] = 0;
            dr["status"] = combatch.Text + "\t" + comacsn.Text + "\t" + comart.Text;

            cpdDataSet.Tables["aut"].Rows.Add(dr);
            decorate();
        }


        private void dataGridCpd_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (comboChoice.SelectedIndex == 0) dataGridCpd.CurrentCell.ReadOnly = false;
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (comboChoice.SelectedIndex == 0 && dataGridCpd.Rows.Count > 0)
            {
                dataGridCpd.CurrentCell.ReadOnly = false;

                DataGridViewTextBoxCell dgvt = (DataGridViewTextBoxCell)dataGridCpd.Rows[dataGridCpd.CurrentCell.RowIndex].Cells[0];
                dgvt.MaxInputLength = 3;
            }
        }



        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripDropDownItem menu = (ToolStripDropDownItem)sender;
            ContextMenuStrip strip = (ContextMenuStrip)menu.Owner;
            Control owner = strip.SourceControl;


            if (MessageBox.Show("Really delete?", "Confirm delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.No)
            {
                return;
            }

            if (owner.Name == "dataGridCpd" && comboChoice.SelectedIndex == 0)
            {
                if (dataGridCpd.CurrentRow != null && dataGridCpd.CurrentRow.Index < dataGridCpd.RowCount)
                {
                    int m = dataGridCpd.CurrentRow.Index;

                    string regno = dataGridCpd.Rows[m].Cells[1].Value.ToString();

                    foreach (DataRow dr in cpdDataSet.Tables["aut"].Select("regno = " + regno))
                    {
                        dr.Delete();
                    }

                    dltdbmol();
                    updtdb();
                    cpdDataSet.AcceptChanges();

                    refillcpd();

                    if (m > 0) dataGridCpd.CurrentCell = dataGridCpd[0, m - 1];
                    /*
                                        int k = comart.SelectedIndex;
                                        if (dataGridCpd.RowCount == 0) refillart();
                                        comart.SelectedIndex = -1;

                                        if (comart.Items.Count > 0) comart.SelectedIndex = 0;
                     * */
                    //                    int k = comart.SelectedIndex;
                    //                    comart.SelectedIndex = -1;
                    //                    comart.SelectedIndex = k;
                }
            }
        }




        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (panel3.Visible && dataGridView1.CurrentCell != null)
                renditor2.MolfileString = dataGridView1.CurrentRow.Cells[1].Value.ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            int n = 0;
            for (int i = 0; i < cpdDataSet.Tables["dpt"].Rows.Count; i++)
                n = n + Convert.ToInt32(dataGridView1.Rows[i].Cells[2].Value.ToString());

            if (n == 0)
            {
                dataGridCpd.CurrentRow.Cells[3].Value = 0;
                dataGridCpd.CurrentCell.Style.BackColor = Color.White;
                comm1.CommandType = CommandType.Text;
                comm1.CommandText = "update ic_moltable set dupinmaster=null where regno=" + dataGridCpd.CurrentRow.Cells[1].Value.ToString();
                if (oraconn.State != ConnectionState.Open) oraconn.Open();
                comm1.ExecuteNonQuery();
                if (oraconn.State == ConnectionState.Open) oraconn.Close();
            }
            else
            {
                dltdbmol();
                int k = comart.SelectedIndex;
                comart.SelectedIndex = -1;
                comart.SelectedIndex = k;
            }

            panel3.Visible = false;

        }


        private void dataGridView0_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridView0.Size.Width < 500)
            {
                Size sz = new Size(900, dataGridView0.Size.Height);
                dataGridView0.Size = sz;
                dataGridView0.Columns[2].Width = 1600;
            }
            else
            {
                Size sz = new Size(413, dataGridView0.Size.Height);
                dataGridView0.Size = sz;
                dataGridView0.Columns[2].Width = 340;
            }
        }


        private void dataGridView2_DoubleClick(object sender, EventArgs e)
        {
            if (comboChoice.SelectedIndex == 1)
            {
                if (dataGridView2.Columns[1].Width < 400)
                {
                    dataGridView2.Columns[1].Width = 800;
                }
                else
                {
                    dataGridView2.Columns[1].Width = 350;
                }
            }
        }


        private void newsymbol()
        {
            DataRow dr = cpdDataSet.Tables["aut"].NewRow();
            dr["tabl"] = "Symbol";
            dr["regno"] = dataGridCpd.CurrentRow.Cells[1].Value.ToString();
            dr["seq"] = 2;
            cpdDataSet.Tables["aut"].Rows.Add(dr);
        }

        private void dataGridView0_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (e.RowIndex > 0)
            {
                dataGridView0.Rows[e.RowIndex].ReadOnly = false;
                if (dataGridView0.Rows[e.RowIndex].Cells[0].Value.ToString() == "Symbol")
                {
                    dataGridView0.Rows[e.RowIndex].Cells[0].Style.BackColor = System.Drawing.Color.SandyBrown;
                }
                else dataGridView0.Rows[e.RowIndex].Cells[0].Style.BackColor = System.Drawing.Color.Khaki;
            }
        }

        private void combosta_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboLst.SelectedIndex > -1)
                try
                {
                    dvshw.RowFilter = "tabl='biolact' and STATUS = '" + combosta.Text + "' and CODE=" + comboLst.SelectedValue.ToString();
                }
                catch (Exception ex)
                {
                    ;
                }
        }

        private void comboLst_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboLst.SelectedIndex > -1)
            {

                if (comboChoice.SelectedIndex == 2)
                {
                    dvshw.RowFilter = "tabl='biolact' and STATUS = '" + combosta.Text + "' and CODE=" + comboLst.SelectedValue.ToString();
                }
                else
                    dvshw.RowFilter = "tabl='" + comboChoice.Text + "' and CODE=" + comboLst.SelectedValue.ToString();

            }
        }

        private void dataGridView2_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            int idx = comboChoice.SelectedIndex;

            if (idx == 1)
            {
                dvshw.RowFilter = "tabl='" + comboChoice.Text + "'";
                if (e.ColumnIndex == 1) dvshw.RowFilter = "tabl='" + comboChoice.Text + "' and data='" + dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() + "'";
            }
            else if (idx > 1 && e.ColumnIndex == 1)
            {
                if (dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != comboLst.Text)
                {
                    int i = comboLst.FindStringExact(dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), 0);
                    comboLst.SelectedIndex = i;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int idx = comboChoice.SelectedIndex;

            if (idx == 1)
            {
                DataView vw = new DataView(cpdDataSet.Tables["aut"]);

                foreach (DataGridViewCell dgs in dataGridCpd.SelectedCells)
                {
                    foreach (DataRow dr in cpdDataSet.Tables["aut"].Select("tabl = 'Grade' and regno = " + dataGridCpd.Rows[dgs.RowIndex].Cells["regno"].Value.ToString()))
                    {
                        dr[2] = dataGridView2.CurrentCell.Value.ToString();
                    }
                }

                dvshw.RowFilter = "tabl='" + comboChoice.Text + "' and data='" + dataGridView2.CurrentCell.Value.ToString() + "'";

            }
            else if (idx == 2)
            {
                DataView view = new DataView(cpdDataSet.Tables["aut"]);

                foreach (DataGridViewCell dgs in dataGridCpd.SelectedCells)
                {
                    view.RowFilter = "tabl = 'Biolact' and regno = " + dataGridCpd.Rows[dgs.RowIndex].Cells["regno"].Value.ToString() + " and code = " + comboLst.SelectedValue.ToString();
                    if (view.Count == 0)
                    {
                        DataRow dr = cpdDataSet.Tables["aut"].NewRow();
                        dr["tabl"] = "Biolact";
                        dr["authnum"] = dataGridCpd.Rows[dgs.RowIndex].Cells["authnum"].Value.ToString().PadLeft(3, ' ');
                        dr["regno"] = dataGridCpd.Rows[dgs.RowIndex].Cells["regno"].Value.ToString();
                        dr["data"] = combosta.Text + "  " + comboLst.Text;
                        dr["code"] = comboLst.SelectedValue.ToString();
                        dr["status"] = combosta.Text;
                        dr["idx"] = 1;
                        dr["seq"] = 3;
                        cpdDataSet.Tables["aut"].Rows.Add(dr);

                        DataRow[] foundRows = cpdDataSet.Tables["bamain"].Select("ba_code=" + comboLst.SelectedValue.ToString());

                        foundRows[0]["CHOSEN"] = 1;
                    }
                }
            }
            else if (idx == 3)
            {
                DataView view = new DataView(cpdDataSet.Tables["aut"]);

                foreach (DataGridViewCell dgs in dataGridCpd.SelectedCells)
                {
                    view.RowFilter = "tabl = 'Cpd_Class' and regno = " + dataGridCpd.Rows[dgs.RowIndex].Cells["regno"].Value.ToString() + " and code = " + comboLst.SelectedValue.ToString();
                    if (view.Count == 0)
                    {
                        DataRow dr = cpdDataSet.Tables["aut"].NewRow();
                        dr["tabl"] = "Cpd_Class";
                        dr["authnum"] = dataGridCpd.Rows[dgs.RowIndex].Cells["authnum"].Value.ToString().PadLeft(3, ' ');
                        dr["regno"] = dataGridCpd.Rows[dgs.RowIndex].Cells["regno"].Value.ToString();
                        dr["data"] = comboLst.Text;
                        dr["code"] = comboLst.SelectedValue.ToString();
                        dr["status"] = "";
                        dr["idx"] = 1;
                        dr["seq"] = 4;
                        cpdDataSet.Tables["aut"].Rows.Add(dr);

                        DataRow[] foundRows = cpdDataSet.Tables["clmain"].Select("class_id=" + comboLst.SelectedValue.ToString());

                        foundRows[0]["CHOSEN"] = 1;
                    }
                }
            }
            else if (idx == 4)
            {
                DataView view = new DataView(cpdDataSet.Tables["aut"]);

                foreach (DataGridViewCell dgs in dataGridCpd.SelectedCells)
                {
                    view.RowFilter = "tabl = 'Use_Profile' and regno = " + dataGridCpd.Rows[dgs.RowIndex].Cells["regno"].Value.ToString() + " and code = " + comboLst.SelectedValue.ToString();
                    if (view.Count == 0)
                    {
                        DataRow dr = cpdDataSet.Tables["aut"].NewRow();
                        dr["tabl"] = "Use_Profile";
                        dr["authnum"] = dataGridCpd.Rows[dgs.RowIndex].Cells["authnum"].Value.ToString().PadLeft(3, ' ');
                        dr["regno"] = dataGridCpd.Rows[dgs.RowIndex].Cells["regno"].Value.ToString();
                        dr["data"] = comboLst.Text;
                        dr["code"] = comboLst.SelectedValue.ToString();
                        dr["status"] = "";
                        dr["idx"] = 1;
                        dr["seq"] = 5;
                        cpdDataSet.Tables["aut"].Rows.Add(dr);

                        DataRow[] foundRows = cpdDataSet.Tables["prmain"].Select("profile_id=" + comboLst.SelectedValue.ToString());

                        foundRows[0]["CHOSEN"] = 1;
                    }
                }
            }


        }

        private void dataGridView2_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            if (comboChoice.SelectedIndex == 1)
            {
                if (dataGridView2.CurrentRow != null && dataGridView2.ColumnCount > 1)
                {
                    dataGridView2.CurrentRow.Cells[0].Value = 0;
                    dataGridView2.CurrentRow.Cells[2].Value = 1;

                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int idx = comboChoice.SelectedIndex;

            if (idx == 1)
            {
                DataView vw = new DataView(cpdDataSet.Tables["aut"]);

                foreach (DataGridViewCell dgs in dataGridLst.SelectedCells)
                {
                    foreach (DataRow dr in cpdDataSet.Tables["aut"].Select("tabl = 'Grade' and regno = " + dataGridLst.Rows[dgs.RowIndex].Cells["regno"].Value.ToString()))
                    {
                        dr[2] = "";
                    }
                }

                dvshw.RowFilter = "tabl='" + comboChoice.Text + "' and data='" + dataGridView2.CurrentCell.Value.ToString() + "'";

            }
            else if (idx == 2)
            {
                DataView view = new DataView(cpdDataSet.Tables["aut"]);

                foreach (DataGridViewCell dgs in dataGridLst.SelectedCells)
                {
                    view.RowFilter = "tabl = 'Biolact' and regno = " + dataGridLst.Rows[dgs.RowIndex].Cells["regno"].Value.ToString() + " and code = " + comboLst.SelectedValue.ToString();
                    view.Delete(0);
                }

                view.RowFilter = "code = " + comboLst.SelectedValue.ToString();
                if (view.Count == 0)
                {
                    DataRow[] foundRows = cpdDataSet.Tables["bamain"].Select("ba_code=" + comboLst.SelectedValue.ToString());

                    foundRows[0]["CHOSEN"] = 0;
                }

            }
            else if (idx == 3)
            {
                DataView view = new DataView(cpdDataSet.Tables["aut"]);

                foreach (DataGridViewCell dgs in dataGridLst.SelectedCells)
                {
                    view.RowFilter = "tabl = 'Cpd_Class' and regno = " + dataGridLst.Rows[dgs.RowIndex].Cells["regno"].Value.ToString() + " and code = " + comboLst.SelectedValue.ToString();
                    view.Delete(0);
                }

                view.RowFilter = "code = " + comboLst.SelectedValue.ToString();
                if (view.Count == 0)
                {
                    DataRow[] foundRows = cpdDataSet.Tables["clmain"].Select("class_id=" + comboLst.SelectedValue.ToString());

                    foundRows[0]["CHOSEN"] = 0;
                }
            }
            else if (idx == 4)
            {
                DataView view = new DataView(cpdDataSet.Tables["aut"]);

                foreach (DataGridViewCell dgs in dataGridLst.SelectedCells)
                {
                    view.RowFilter = "tabl = 'Use_Profile' and regno = " + dataGridLst.Rows[dgs.RowIndex].Cells["regno"].Value.ToString() + " and code = " + comboLst.SelectedValue.ToString();
                    view.Delete(0);
                }

                view.RowFilter = "code = " + comboLst.SelectedValue.ToString();
                if (view.Count == 0)
                {
                    DataRow[] foundRows = cpdDataSet.Tables["prmain"].Select("profile_id=" + comboLst.SelectedValue.ToString());

                    foundRows[0]["CHOSEN"] = 0;
                }
            }

        }




        private void updtdb()
        {
            string sdb = "";

            if (!cpdDataSet.Tables.Contains("aut")) return;

            DataView dv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Symbol'", "", DataViewRowState.CurrentRows);
            for (int i = dv1.Count; i > 0; i--)
            {
                dv1[i - 1].Row["data"].ToString().Trim();
                if (dv1[i - 1].Row["data"].ToString().Length == 0) dv1[i - 1].Delete();
            }

            dv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Subject Lst'", "", DataViewRowState.CurrentRows);
            for (int i = dv1.Count; i > 0; i--)
            {
                dv1[i - 1].Row["data"].ToString().Trim();
                if (dv1[i - 1].Row["data"].ToString().Length == 0) dv1[i - 1].Delete();
            }


            dv1 = new DataView(cpdDataSet.Tables["cpd"], "", "", DataViewRowState.ModifiedCurrent);
            for (int i = 0; i < dv1.Count; i++)
            {

                if (dv1[i].Row["authnum"].ToString() != dv1[i].Row["authnum", DataRowVersion.Original].ToString())
                    sdb = sdb + "\tAUTHNUM\tUPDATE\t" + dv1[i].Row["REGNO"].ToString() + "\t" + dv1[i].Row["authnum"].ToString() + "\t\t\t\t\t\r\n";

                if (dv1[i].Row["lead"].ToString() != dv1[i].Row["lead", DataRowVersion.Original].ToString())
                    sdb = sdb + "\tLEAD\tUPDATE\t" + dv1[i].Row["REGNO"].ToString() + "\t" + dv1[i].Row["lead"].ToString() + "\t\t\t\t\t\r\n";
            }


            dv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Grade'", "", DataViewRowState.ModifiedCurrent);
            for (int i = 0; i < dv1.Count; i++)
            {
                dv1[i].Row["data"].ToString().Trim();
                sdb = sdb + "\tGRADE\t" + dv1[i].Row["REGNO"].ToString() + "\t" + dv1[i].Row["data"].ToString() + "\t\t\t\t\t\r\n";
            }

            dv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Symbol'", "", DataViewRowState.Deleted);
            for (int i = 0; i < dv1.Count; i++)
            {
                sdb = sdb + "\tSYMBOL\tDELETE\t" + dv1[i].Row["REGNO", DataRowVersion.Original].ToString() + "\t" + dv1[i].Row["data", DataRowVersion.Original].ToString() + "\t\t\t\t\t\r\n";
            }

            dv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Symbol'", "", DataViewRowState.ModifiedCurrent);
            for (int i = 0; i < dv1.Count; i++)
            {
                if (dv1[i].Row["data"].ToString() != dv1[i].Row["data", DataRowVersion.Original].ToString())
                    sdb = sdb + "\tSYMBOL\tUPDATE\t" + dv1[i].Row["REGNO"].ToString() + "\t" + dv1[i].Row["data"].ToString() + "\t" + dv1[i].Row["data", DataRowVersion.Original].ToString() + "\t\t\t\t\t\r\n";
            }

            dv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Symbol'", "", DataViewRowState.Added);
            for (int i = 0; i < dv1.Count; i++)
            {
                sdb = sdb + "\tSYMBOL\tINSERT\t" + dv1[i].Row["REGNO"].ToString() + "\t" + dv1[i].Row["data"].ToString() + "\t\t\t\t\t\r\n";
            }

            dv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Biolact'", "", DataViewRowState.Deleted);
            for (int i = 0; i < dv1.Count; i++)
            {
                sdb = sdb + "\tBIOLACT\tDELETE\t" + dv1[i].Row["REGNO", DataRowVersion.Original].ToString() + "\t" + dv1[i].Row["code", DataRowVersion.Original].ToString() + "\t" + dv1[i].Row["status", DataRowVersion.Original].ToString() + "\t\t\t\t\t\r\n";
            }
            dv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Biolact'", "", DataViewRowState.Added);
            for (int i = 0; i < dv1.Count; i++)
            {
                sdb = sdb + "\tBIOLACT\tINSERT\t" + dv1[i].Row["REGNO"].ToString() + "\t" + dv1[i].Row["code"].ToString() + "\t" + dv1[i].Row["status"].ToString() + "\t\t\t\t\t\r\n";
            }

            dv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Cpd_Class'", "", DataViewRowState.Deleted);
            for (int i = 0; i < dv1.Count; i++)
            {
                sdb = sdb + "\tCPDCLASS\tDELETE\t" + dv1[i].Row["REGNO", DataRowVersion.Original].ToString() + "\t" + dv1[i].Row["code", DataRowVersion.Original].ToString() + "\t\t\t\t\t\r\n";
            }
            dv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Cpd_Class'", "", DataViewRowState.Added);
            for (int i = 0; i < dv1.Count; i++)
            {
                sdb = sdb + "\tCPDCLASS\tINSERT\t" + dv1[i].Row["REGNO"].ToString() + "\t" + dv1[i].Row["code"].ToString() + "\t\t\t\t\t\r\n";
            }

            dv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Use_Profile'", "", DataViewRowState.Deleted);
            for (int i = 0; i < dv1.Count; i++)
            {
                sdb = sdb + "\tPROFILE\tDELETE\t" + dv1[i].Row["REGNO", DataRowVersion.Original].ToString() + "\t" + dv1[i].Row["code", DataRowVersion.Original].ToString() + "\t\t\t\t\t\r\n";
            }
            dv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Use_Profile'", "", DataViewRowState.Added);
            for (int i = 0; i < dv1.Count; i++)
            {
                sdb = sdb + "\tPROFILE\tINSERT\t" + dv1[i].Row["REGNO"].ToString() + "\t" + dv1[i].Row["code"].ToString() + "\t\t\t\t\t\r\n";
            }
            sdb = sdb + "\t\t\t\t\t\r\n";


            dv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Subject Lst'", "", DataViewRowState.Deleted);
            for (int i = 0; i < dv1.Count; i++)
            {
                sdb = sdb + "\tSUBJECT\tDELETE\t" + dv1[i].Row["status", DataRowVersion.Original].ToString() + "\t" + dv1[i].Row["data", DataRowVersion.Original].ToString() + "\t\t\t\t\t\r\n";
            }
            dv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Subject Lst'", "", DataViewRowState.ModifiedCurrent);
            for (int i = 0; i < dv1.Count; i++)
            {
                sdb = sdb + "\tSUBJECT\tUPDATE\t" + dv1[i].Row["status"].ToString() + "\t" + dv1[i].Row["data"].ToString() + "\t" + dv1[i].Row["data", DataRowVersion.Original].ToString() + "\t\t\t\t\t\r\n";
            }
            dv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Subject Lst'", "", DataViewRowState.Added);
            for (int i = 0; i < dv1.Count; i++)
            {
                sdb = sdb + "\tSUBJECT\tINSERT\t" + dv1[i].Row["status"].ToString() + "\t" + dv1[i].Row["data"].ToString() + "\t\t\t\t\t\r\n";
            }

            //            if (sdb.IndexOf("SYMBOL")>0) MessageBox.Show(sdb);

            try
            {
                if (oraconn.State != ConnectionState.Open) oraconn.Open();
                comm1.Connection = oraconn;

                comm1.Parameters.Add("clobtodb", OracleDbType.Clob).Value = sdb + "\r\n";
                comm1.CommandType = CommandType.StoredProcedure;
                comm1.CommandText = "moleditproc";

                comm1.ExecuteNonQuery();

                cpdDataSet.AcceptChanges();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                comm1.Parameters.Clear();
                oraconn.Close();
                return;
            }
            comm1.Parameters.Clear();
            oraconn.Close();

        }



        private void button3_Click(object sender, EventArgs e)
        {
            updtdb();
            cpdDataSet.AcceptChanges();
            textToPrint = "";
            string journal = "";

            if (oraconn.State == ConnectionState.Open) oraconn.Close();

            try
            {
                oraconn.Open();
                comm1.Connection = oraconn;
                comm1.CommandType = CommandType.Text;
                comm1.CommandText = "select l.journaltitle || ' ' || j.volume || '(' || j.issue || '), ' || j.year from cardex.journal_issues j, cardex.cid_journal_list l where l.seqnum= j.seqnum and j.acsn = '" + comacsn.Text + "'";

                OracleDataReader dr = comm1.ExecuteReader();
                if (dr.Read()) journal = dr.GetString(0);
                //                comm1.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                oraconn.Close();
                return;
            }
            oraconn.Close();

            textToPrint = textToPrint + "Batch: " + combatch.Text + "  Accesion: " + comacsn.Text + "  Article: " + comart.Text + "\r\n";
            DataView dv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Subject Lst'", "", DataViewRowState.CurrentRows);

            if (dv1.Count > 0) textToPrint = textToPrint + "\r\nSubject Listing:\r\n";
            for (int i = 0; i < dv1.Count; i++)
            {
                textToPrint = textToPrint + "    " + dv1[i].Row["data"].ToString() + "\r\n";
            }

            DataView dv0 = new DataView(cpdDataSet.Tables["cpd"], "", "asrt", DataViewRowState.CurrentRows);


            string alist = "";
            //            int lnc = -1;
            for (int i = 0; i < dv0.Count; i++)
            {
                alist = alist + ", " + dv0[i].Row["authnum"].ToString();
                /*                int nc = compare(aut, aut1);

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
                 * */
            }

            if (alist.Length < 3) MessageBox.Show(alist + "  " + alist.Length);
            alist = alist.Substring(2);

            textToPrint = textToPrint + "\r\nCompounds(Total " + cpdDataSet.Tables["cpd"].Rows.Count + "):\r\n";

            while (alist.Length > 90)
            {
                int j = 90;

                while (alist[j] != ',') j -= 1;
                textToPrint = textToPrint + "    " + alist.Substring(0, j + 1) + "\r\n";

                if (alist.Length < j + 2) MessageBox.Show(alist + "  " + alist.Length + "  " + j);
                alist = alist.Substring(j + 1);
            }
            textToPrint = textToPrint + "    " + alist + "\r\n";

            dv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Cpd_Class'", "", DataViewRowState.CurrentRows);
            if (dv1.Count > 0) textToPrint = textToPrint + "\r\nCompound Class:\r\n";

            DataView ndv1 = new DataView(cpdDataSet.Tables["clmain"], "chosen=1", "", DataViewRowState.CurrentRows);

            for (int i = 0; i < ndv1.Count; i++)
            {
                DataView ndv2 = new DataView(cpdDataSet.Tables["aut"], "tabl='Cpd_Class' and code=" + ndv1[i].Row["class_id"], "authnum", DataViewRowState.CurrentRows);

                alist = "";
                for (int j = 0; j < ndv2.Count; j++)
                {
                    alist = alist + ", " + ndv2[j].Row[0].ToString().Trim();
                }

                alist = ndv1[i].Row["cpd_class"].ToString() + "    " + alist.Substring(1);

                while (alist.Length > 90)
                {
                    int j = 90;

                    while (alist[j] != ',') j -= 1;
                    textToPrint = textToPrint + "    " + alist.Substring(0, j + 1) + "\r\n    ";

                    alist = alist.Substring(j + 1);
                }
                textToPrint = textToPrint + "    " + alist + "\r\n";
            }

            dv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Use_Profile'", "", DataViewRowState.CurrentRows);
            if (dv1.Count > 0) textToPrint = textToPrint + "\r\nUse Profile:\r\n";

            ndv1 = new DataView(cpdDataSet.Tables["prmain"], "chosen=1", "", DataViewRowState.CurrentRows);

            for (int i = 0; i < ndv1.Count; i++)
            {
                DataView ndv2 = new DataView(cpdDataSet.Tables["aut"], "tabl = 'Use_Profile' and code=" + ndv1[i].Row["profile_id"], "authnum", DataViewRowState.CurrentRows);

                alist = "";
                for (int j = 0; j < ndv2.Count; j++)
                {
                    alist = alist + ", " + ndv2[j].Row[0].ToString().Trim();
                }

                alist = ndv1[i].Row["use_profile"].ToString() + "    " + alist.Substring(2);

                while (alist.Length > 90)
                {
                    int j = 90;

                    while (alist[j] != ',') j -= 1;
                    textToPrint = textToPrint + "    " + alist.Substring(0, j + 1) + "\r\n    ";

                    alist = alist.Substring(j + 1);
                }
                textToPrint = textToPrint + "    " + alist + "\r\n";

            }

            dv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Biolact'", "", DataViewRowState.CurrentRows);
            if (dv1.Count > 0) textToPrint = textToPrint + "\r\nBiological Activity:\r\n";

            ndv1 = new DataView(cpdDataSet.Tables["bamain"], "chosen=1", "", DataViewRowState.CurrentRows);

            for (int i = 0; i < ndv1.Count; i++)
            {
                DataView ndv2 = new DataView(cpdDataSet.Tables["aut"], "tabl='Biolact' and status = 'T' and code=" + ndv1[i].Row["ba_code"], "authnum", DataViewRowState.CurrentRows);

                alist = "";
                for (int j = 0; j < ndv2.Count; j++)
                {
                    alist = alist + ", " + ndv2[j].Row[0].ToString().Trim();
                }

                if (alist.Length > 0)
                {
                    alist = "T  " + ndv1[i].Row["biolact"].ToString() + "    " + alist.Substring(2);

                    while (alist.Length > 90)
                    {
                        int j = 90;

                        while (alist[j] != ',') j -= 1;
                        textToPrint = textToPrint + "    " + alist.Substring(0, j + 1) + "\r\n    ";

                        alist = alist.Substring(j + 1);
                    }
                    textToPrint = textToPrint + "    " + alist + "\r\n";

                }

                ndv2 = new DataView(cpdDataSet.Tables["aut"], "tabl='Biolact' and status = 'P' and code=" + ndv1[i].Row["ba_code"], "authnum", DataViewRowState.CurrentRows);
                alist = "";
                for (int j = 0; j < ndv2.Count; j++)
                {
                    alist = alist + ", " + ndv2[j].Row[0].ToString().Trim();
                }

                if (alist.Length > 0)
                {
                    alist = "P  " + ndv1[i].Row["biolact"].ToString() + "    " + alist.Substring(2);

                    while (alist.Length > 90)
                    {
                        int j = 90;

                        while (alist[j] != ',') j -= 1;
                        textToPrint = textToPrint + "    " + alist.Substring(0, j + 1) + "\r\n    ";

                        alist = alist.Substring(j + 1);
                    }
                    textToPrint = textToPrint + "    " + alist + "\r\n";
                }
            }

            dv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Grade' and len(data)>0", "authnum", DataViewRowState.CurrentRows);
            if (dv1.Count > 0) textToPrint = textToPrint + "\r\nGrade:\r\n";

            for (int i = 0; i < dv1.Count; i++)
            {
                if (dv1[i].Row[2].ToString().Trim().Length > 0) textToPrint = textToPrint + "    " + dv1[i].Row[0].ToString().Trim() + "  " + dv1[i].Row[2].ToString() + "\r\n";
            }

            dv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Symbol'", "", DataViewRowState.CurrentRows);
            if (dv1.Count > 0) textToPrint = textToPrint + "\r\nSymbol:\r\n";

            for (int i = 0; i < cpdDataSet.Tables["cpd"].Rows.Count; i++)
            {
                ndv1 = new DataView(cpdDataSet.Tables["aut"], "tabl='Symbol' and regno=" + cpdDataSet.Tables["cpd"].Rows[i][1].ToString(), "", DataViewRowState.CurrentRows);

                for (int j = 0; j < ndv1.Count; j++)
                {
                    string symout = "    " + cpdDataSet.Tables["cpd"].Rows[i][0].ToString() + "  " + ndv1[j].Row[2].ToString();

                    while (symout.Length > 90)
                    {
                        textToPrint = textToPrint + "    " + symout.Substring(0, 90) + "\r\n";
                        symout = symout.Substring(90);
                    }
                    textToPrint = textToPrint + "    " + symout + "\r\n";
                }
            }

            PrintPreviewDialog dlg = new PrintPreviewDialog();

            ((Form)dlg).WindowState = FormWindowState.Maximized;
            strPrint = textToPrint;
            dlg.PrintPreviewControl.Zoom = 1.0;

            //            printDoc.DefaultPageSettings.Landscape = true;

            dlg.Document = printDoc;
            dlg.ShowDialog();
        }

        private void printDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            float yPos = 0;
            float leftMargin = e.MarginBounds.Left;
            float topMargin = e.MarginBounds.Top;
            Font printFont = new Font("Arial", 12);

            int count = 0;
            int i = 0;

            yPos = topMargin / 2 + (((float)1.5) * printFont.GetHeight(e.Graphics));

            while (count < 50 && strPrint.Length > 0)
            {
                i = strPrint.IndexOf("\r\n");
                string line = strPrint.Substring(0, i);
                strPrint = strPrint.Substring(i + 2);

                yPos += printFont.GetHeight(e.Graphics);
                if (line.IndexOf("Batch:") > -1)
                {
                    printFont = new Font("Arial", 14);
                    e.Graphics.DrawString(line, printFont, Brushes.Black, leftMargin / 3, yPos, new StringFormat());
                    count++;
                    yPos += printFont.GetHeight(e.Graphics);
                    e.Graphics.DrawLine(new Pen(Color.Black), leftMargin / 3, yPos, leftMargin + 680, yPos);
                }
                else
                {
                    printFont = new Font("Arial", 12);
                    e.Graphics.DrawString(line, printFont, Brushes.Black, leftMargin / 3, yPos, new StringFormat());
                }
                count++;
            }


            e.HasMorePages = (strPrint.Length > 0);

        }

        private void printDoc_BeginPrint(object sender, PrintEventArgs e)
        {
            strPrint = textToPrint;
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

        private void dataGridView0_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string cellval = dataGridView0.CurrentCell.Value.ToString();
            if (cellval.Trim().Length != cellval.Length) dataGridView0.CurrentCell.Value = cellval.Trim();
        }

        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView2.CurrentCell.Value != null)
            {
                string cellval = dataGridView2.CurrentCell.Value.ToString();
                if (cellval.Trim().Length != cellval.Length) dataGridView2.CurrentCell.Value = cellval.Trim();
            }
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            updtdb();
        }

        private void dataGridCpd_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string regno = dataGridCpd.Rows[e.RowIndex].Cells[e.ColumnIndex + 1].Value.ToString();

            dataGridCpd.Rows[e.RowIndex].Cells[4].Value = dataGridCpd.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().PadLeft(3, ' ');
            foreach (DataRow dr in cpdDataSet.Tables["aut"].Select("regno = " + dataGridCpd.Rows[e.RowIndex].Cells["regno"].Value.ToString()))
            {
                dr[0] = dataGridCpd.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().PadLeft(3, ' ');
            }
        }

        private void markLeadCPDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int i = dataGridCpd.CurrentRow.Index;
            if (markLeadCPDToolStripMenuItem.Text == "Mark Lead CPD")
            {
                dataGridCpd.CurrentRow.Cells[5].Value = 1;
                dataGridCpd.CurrentRow.Cells[0].Style.BackColor = System.Drawing.Color.LightGreen;
                dataGridCpd.CurrentRow.Cells[0].Style.SelectionBackColor = System.Drawing.Color.LawnGreen;
            }
            else
            {
                dataGridCpd.CurrentRow.Cells[5].Value = 0;
                dataGridCpd.CurrentRow.Cells[0].Style.BackColor = System.Drawing.Color.White;
                dataGridCpd.CurrentRow.Cells[0].Style.SelectionBackColor = SystemColors.Highlight;
            }

            //            dataGridCpd.CommitEdit(DataGridViewDataErrorContexts.Commit);
            bscpd.EndEdit();

        }

        private void dataGridCpd_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            dataGridView0.CurrentCell = null;
            if (e.Button == MouseButtons.Right)
            {
                if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
                {
                    dataGridCpd.CurrentCell = dataGridCpd.Rows[e.RowIndex].Cells[e.ColumnIndex];
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            DataView ndv = new DataView(cpdDataSet.Tables["aut"], "tabl='Biolact' and regno =" + dataGridCpd.CurrentRow.Cells[1].Value.ToString(), "", DataViewRowState.CurrentRows);

            if (ndv.Count > 0)
            {
                contextMenuStrip1.Items[2].Visible = true;

                if (dataGridCpd.CurrentRow.Cells[5].Value.ToString() == "1") contextMenuStrip1.Items[2].Text = "UnMark Lead CPD";
                else contextMenuStrip1.Items[2].Text = "Mark Lead CPD";

            }
            else contextMenuStrip1.Items[2].Visible = false;

        }

        private void dataGridView0_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView0.BeginEdit(false);

            TextBox tt = (TextBox)(DataGridViewTextBoxEditingControl)dataGridView0.EditingControl;
            if (tt != null)
            {
                tt.SelectionStart = 0;
                tt.SelectionLength = 0;
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            string host = textBox2.Text;
            string port = textBox3.Text;
            string service = textBox4.Text;

            if (host.Length * port.Length * service.Length > 0)
            {
                settings.host = host;
                settings.port = port;
                settings.service = service;

                icpdnConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + host + ")(PORT=" + port + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + service + "))); User Id=icpdn;Password=icpdn;";
                settings.Save();
                oraconn.ConnectionString = icpdnConnectionString;

                if (oraconn.State == ConnectionState.Closed)
                {
                    refillbatch();
                }
            }
        }


    }
}
