using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using layoutExtraction.Properties;

namespace layoutExtraction
{
    public partial class Form1 : Form
    {
        static private Oracle.DataAccess.Client.OracleConnection icOraConn = new OracleConnection();
        static private Oracle.DataAccess.Client.OracleConnection ccrOraConn = new OracleConnection();

        static private string connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1550)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=chemdb.prod-wos.com)));";
        static private string icpdnConnectionString = connectionString + "User Id=icpdn;Password=icpdn;";
        static private string ccrpdnConnectionString = connectionString + "User Id=ccrpdn;Password=ccrpdn;";
        static private Properties.Settings settings = Properties.Settings.Default;


        static private DataTable layTable = new DataTable("LAY");
        static private BindingSource laybs = new BindingSource();
        static private DataTable artTable = new DataTable("ART");
        static private DataTable refTable = new DataTable("REF");
        static private BindingSource rfbs = new BindingSource();
        static private DataTable rctTable = new DataTable("RCT");
        static private BindingSource rtbs = new BindingSource();
        static private DataTable catTable = new DataTable("CAT");
        static private BindingSource cabs = new BindingSource();
        static private DataTable solTable = new DataTable("SOL");
        static private BindingSource slbs = new BindingSource();
        static private DataTable prdTable = new DataTable("PRD");
        static private BindingSource prbs = new BindingSource();
        static private DataTable cndTable = new DataTable("CND");
        static private BindingSource cdbs = new BindingSource();
        static private DataTable cmtTable = new DataTable("CMT");
        static private BindingSource cmbs = new BindingSource();


        static private DataTable molTable = new DataTable("MOL");
        static private DataTable symTable = new DataTable("SYM");
        static private BindingSource smbs = new BindingSource();


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
                connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + host + ")(PORT=" + port + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + service + ")));";
            }
            icpdnConnectionString = connectionString + "User Id=icpdn;Password=icpdn;";
            ccrpdnConnectionString = connectionString + "User Id=ccrpdn;Password=ccrpdn;";
            panel1.Visible = false;

            icOraConn.ConnectionString = icpdnConnectionString;
            ccrOraConn.ConnectionString = ccrpdnConnectionString;



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


            rfbs.DataSource = refTable;
            DataColumn myDataColumn;

            myDataColumn = new DataColumn("RXNID",      System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("CHIME",      System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("ACCESN.NO",  System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("ARTL.NO",    System.Type.GetType("System.Decimal"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("PATH",       System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("STEP",       System.Type.GetType("System.Decimal"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("NSM",        System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("CCRREF",     System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("REACTANT",   System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("CATALYST",   System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("SOLVENT",    System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("PRODUCT",    System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("CONDITIONS", System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("COMMENTS",   System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);

            comboBox2.SelectedIndex = -1;
            comboBox2.SelectedIndex = 0;

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

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            if(comboBox2.Text == "IC")
                cmd.CommandText = "select unique acsn from ic_moltabref where batchno=" + comboBox1.SelectedItem.ToString() + " order by 1";
            else
                cmd.CommandText = "select unique acsn from ccr_batch where batchno=" + comboBox1.SelectedItem.ToString() + " order by 1";
            

            try
            {
                OraConn.Open();
                cmd.Connection = OraConn;
                cmd.CommandType = CommandType.Text;
                OracleDataReader dr1 = cmd.ExecuteReader();

                while (dr1.Read())
                {
                    listBox1.Items.Add(dr1.GetString(0));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraConn.Close();
                return;
            }
            OraConn.Close();

            button1.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox1.Focus();
            this.Cursor = Cursors.WaitCursor;
            button1.Visible = false;

            if (comboBox2.Text == "IC") icmdf();
            else ccrrdf();

            this.Cursor = Cursors.Default;
            button1.Visible = true;
        }


        private void icmdf()
        {
            

            try
            {
                if(OraConn.State != ConnectionState.Open) OraConn.Open();
                cmd.Connection = OraConn;
                cmd.CommandType = CommandType.Text;

                int scnt = listBox1.SelectedItems.Count;
                for (int i = 0; i < scnt; i++)
                {
                    
                        
                    molTable.Rows.Clear();
                    symTable.Rows.Clear();

                    cmd.CommandText = "select regno, acsn, artlnum, authnum, molfile(ctab) chime, grade from ic_moltable where acsn='" + listBox1.SelectedItems[0].ToString() + "' order by 1";
                    cpdAdapter.SelectCommand = cmd;
                    cpdAdapter.Fill(molTable);

                    cmd.CommandText = "select r.regno, r.acsn, r.artlnum, r.authnum, molfile(t.ctab) chime, r.grade from ic_molref r, ic_moltable t where r.refno = t.regno and r.acsn='" + listBox1.SelectedItems[0].ToString() + "' order by 1";
                    cpdAdapter.SelectCommand = cmd;
                    cpdAdapter.Fill(molTable);

                    cmd.CommandText = "select s.regno, s.symbol from ic_symbol s, ic_moltabref m where s.regno = m.regno and m.acsn='" + listBox1.SelectedItems[0].ToString() + "' order by 1";
                    cpdAdapter.SelectCommand = cmd;
                    cpdAdapter.Fill(symTable);


                    string outdata = "$RDFILE 1\r\n$DATM    " + DateTime.Now.Date.ToString("MM/dd/yy HH:MM") + "\r\n";

                    DataView dv = new DataView(molTable, "", "regno", DataViewRowState.CurrentRows);

                    for (int j = 0; j < dv.Count; j++)
                    {
                        string mf = dv[j]["chime"].ToString();

                        int idx = 0;
                        int nn=0;
                        while (nn < 2)
                        {
                            idx = mf.IndexOf("\n", idx + 1); nn++;
                        }
                        int natom = Convert.ToInt32(mf.Substring(idx + 1, 3));

                        int nidx = idx;
                        float xmin = 90000, xmax = -90000;
                        float ymin = 90000, ymax = -90000;
                        for (int n = 0; n < natom; n++)
                        {
                            idx = mf.IndexOf("\n", idx + 1);
                            float x = (float)Convert.ToDouble(mf.Substring(idx + 1, 10));
                            float y = (float)Convert.ToDouble(mf.Substring(idx + 11, 10));

                            if (xmin > x) xmin = x;
                            if (xmax < x) xmax = x;
                            if (ymin > y) ymin = y;
                            if (ymax < y) ymax = y;
                        }
                        float xmid = (xmin + xmax) / 2;
                        float ymid = (ymin + ymax) / 2;

                        idx = nidx;
                        for (int n = 0; n < natom; n++)
                        {
                            idx = mf.IndexOf("\n", idx + 1);
                            float x = (float)Convert.ToDouble(mf.Substring(idx + 1, 10));
                            float y = (float)Convert.ToDouble(mf.Substring(idx + 11, 10));

                            x = x - xmid;
                            y = y - ymid;
//                            x = 2 * x;
//                            y = 2 * y;
                            string cao = String.Format("{0,10:0.0000}", x);
                            string kao = String.Format("{0,10:0.0000}", y);
                            mf = mf.Substring(0, idx + 1) + cao + kao + mf.Substring(idx + 21);
                        }

                        outdata = outdata + "$MFMT $MIREG " + dv[j]["regno"].ToString() + "\r\n" + mf.Replace("\n","\r\n");

                        if (dv[j]["grade"].ToString().Length > 0)
                        {
                            outdata = outdata + "$DTYPE MOL:GRADE\r\n$DATUM " + dv[j]["grade"].ToString() + "\r\n";
                        }

                        outdata = outdata + "$DTYPE MOL:ACCESN.NO\r\n$DATUM " + dv[j]["acsn"].ToString() + "\r\n";
                        outdata = outdata + "$DTYPE MOL:ARTL.NO\r\n$DATUM " + dv[j]["artlnum"].ToString() + "\r\n";
                        outdata = outdata + "$DTYPE MOL:AUTH.NO\r\n$DATUM " + dv[j]["authnum"].ToString() + "\r\n";

                        smbs.DataSource = symTable;
                        smbs.Filter = "regno=" + dv[j]["regno"].ToString();
                        smbs.MoveFirst();
                        for (int k = 0; k < smbs.Count; k++)
                        {
                            DataRowView dr = (DataRowView)smbs.Current;
                            outdata = outdata + "$DTYPE MOL:SYMBOL(" + (k+1) + "):SYMBOL\r\n" + rdformat("$DATUM " + dr.Row["SYMBOL"].ToString());
                            smbs.MoveNext();
                        }
                    }

                    File.WriteAllText("I:\\users\\HDS\\IC\\" + listBox1.SelectedItems[0].ToString() + "IP.MDF", outdata);
                    int nix = Convert.ToInt32(listBox1.SelectedIndices[0].ToString());
//                    MessageBox.Show(listBox1.SelectedItems[0].ToString() + "   " + nix);
                    listBox1.SetSelected(nix, false);

                    Refresh();
                    Application.DoEvents();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraConn.Close();
                this.Cursor = Cursors.Default;
                return;
            }
        }

        private void ccrrdf()
        {
            try
            {
                int scnt = listBox1.SelectedItems.Count;
                for (int m = 0; m < scnt; m++)
                {
                    refTable.Rows.Clear();
                    layTable.Rows.Clear();
                    artTable.Rows.Clear();
                    rctTable.Rows.Clear();
                    catTable.Rows.Clear();
                    solTable.Rows.Clear();
                    prdTable.Rows.Clear();
                    cndTable.Rows.Clear();
                    cmtTable.Rows.Clear();

                    OraConn.Open();
                    cmd.Connection = OraConn;
                    cmd.CommandType = CommandType.Text;

                    cmd.CommandText = "select rxnid, rxnfile(rctab) chime from ccr_batch where rxnid in (select unique r.rxnid from rxn_path r, rxn_prodcode p where r.rxnid = p.rxnid and p.prodcode = '" + comboBox2.Text + "' and acsn='" + listBox1.SelectedItems[0].ToString() + "') order by 1";
                    cpdAdapter.SelectCommand = cmd;
                    cpdAdapter.Fill(layTable);

                    cmd.CommandText = "select c.rxnid, c.acsn, c.artlno, r.path, '' sstep, r.step, r.nsm from rxn_path r, ccr_batch c, rxn_prodcode p where p.rxnid= c.rxnid and p.prodcode = '" + comboBox2.Text + "' and r.rxnid = c.rxnid and c.acsn='" + listBox1.SelectedItems[0].ToString() + "' order by 1";
                    cpdAdapter.SelectCommand = cmd;
                    cpdAdapter.Fill(refTable);

                    cmd.CommandText = "select unique c.acsn, c.artlno, r.path from rxn_path r, ccr_batch c where r.rxnid= c.rxnid and c.acsn='" + listBox1.SelectedItems[0].ToString() + "' order by 1";
                    cpdAdapter.SelectCommand = cmd;
                    cpdAdapter.Fill(artTable);

                    cmd.CommandText = "select r.* from reactant_grade r, ccr_batch c where r.rxnid= c.rxnid and c.acsn='" + listBox1.SelectedItems[0].ToString() + "' order by 1";
                    cpdAdapter.SelectCommand = cmd;
                    cpdAdapter.Fill(rctTable);

                    cmd.CommandText = "select r.rxnid, r.grade, s.symbol, r.CATALYST_POS from catalyst_grade r, ccr_batch c, ccr_symbol s where r.molregno= s.molregno and r.rxnid= c.rxnid and c.acsn='" + listBox1.SelectedItems[0].ToString() + "' and s.idx=1 order by 1";
                    cpdAdapter.SelectCommand = cmd;
                    cpdAdapter.Fill(catTable);

                    cmd.CommandText = "select r.rxnid, r.grade, s.symbol, r.SOLVENT_POS  from solvent_grade r, ccr_batch c, ccr_symbol s where r.molregno= s.molregno and r.rxnid= c.rxnid and c.acsn='" + listBox1.SelectedItems[0].ToString() + "' and s.idx=1 order by 1";
                    cpdAdapter.SelectCommand = cmd;
                    cpdAdapter.Fill(solTable);

                    cmd.CommandText = "select r.* from product_grade r, ccr_batch c where r.rxnid= c.rxnid and c.acsn='" + listBox1.SelectedItems[0].ToString() + "' order by 1";
                    cpdAdapter.SelectCommand = cmd;
                    cpdAdapter.Fill(prdTable);

                    cmd.CommandText = "select r.* from rxn_cond r, ccr_batch c where r.rxnid= c.rxnid and c.acsn='" + listBox1.SelectedItems[0].ToString() + "' order by 1";
                    cpdAdapter.SelectCommand = cmd;
                    cpdAdapter.Fill(cndTable);

                    cmd.CommandText = "select r.* from ccr_comments r, ccr_batch c where r.rxnid= c.rxnid and c.acsn='" + listBox1.SelectedItems[0].ToString() + "' order by 1, 2";
                    cpdAdapter.SelectCommand = cmd;
                    cpdAdapter.Fill(cmtTable);

                    OraConn.Close();

                    for (int i = 0; i < layTable.Rows.Count; i++)
                    {
                        string rxnstr = layTable.Rows[i]["CHIME"].ToString();
                        string rxnid = layTable.Rows[i]["RXNID"].ToString();

                        int j = rxnstr.IndexOf("$MOL", 0);
                        string rxnmod = rxnstr.Substring(0, j);

                        int k = 0;
                        k = rxnstr.IndexOf("\n", k + 1);
                        k = rxnstr.IndexOf("\n", k + 1);
                        k = rxnstr.IndexOf("\n", k + 1);
                        k = rxnstr.IndexOf("\n", k + 1);

                        string dlin = rxnstr.Substring(k + 1, 3);
                        int r1 = Convert.ToInt32(dlin);
                        dlin = rxnstr.Substring(k + 4, 3);
                        int r2 = Convert.ToInt32(dlin);

                        k = -1; int mdx = 1;
                        do
                        {
                            string molregno = "";
                            if (mdx < r1 + 1)
                            {
                                rtbs.DataSource = rctTable;
                                rtbs.Filter = "RXNID='" + layTable.Rows[i][0].ToString() + "' and REACTANT_POS=" + mdx;
                                rtbs.MoveFirst();
                                DataRowView dr = (DataRowView)rtbs.Current;
                                molregno = dr.Row["MOLREGNO"].ToString();
                            }
                            else
                            {
                                prbs.DataSource = prdTable;
                                prbs.Filter = "RXNID='" + layTable.Rows[i][0].ToString() + "' and PRODUCT_POS=" + (mdx - r1);

                                prbs.MoveFirst();
                                DataRowView dr = (DataRowView)prbs.Current;
                                molregno = dr.Row["MOLREGNO"].ToString();
                            }

                            k = rxnstr.IndexOf("$MOL", j + 1);

                            String sr;

                            if (k > -1)
                                sr = rxnstr.Substring(j, k - j);
                            else
                                sr = rxnstr.Substring(j);

                            int p = 0;
                            for (int n = 0; n < 2; n++) p = sr.IndexOf("\n", p + 1);
                            string moln = "      ";
                            moln = moln.Substring(0, 6 - molregno.Length) + molregno;

                            sr = sr.Substring(0, p + 47) + moln + sr.Substring(p + 53);

                            for (int n = 0; n < 2; n++) p = sr.IndexOf("\n", p + 1);
                            dlin = sr.Substring(p + 1, 3);
                            int natom = Convert.ToInt32(dlin);

                            float xmin = 90000, xmax = -90000;
                            float ymin = 90000, ymax = -90000;
                            for (int n = 0; n < natom; n++)
                            {
                                p = sr.IndexOf("\n", p + 1);
                                dlin = sr.Substring(p + 1, 10);
                                float x = (float)Convert.ToDouble(dlin);
                                dlin = sr.Substring(p + 11, 10);
                                float y = (float)Convert.ToDouble(dlin);

                                if (xmin > x) xmin = x;
                                if (xmax < x) xmax = x;
                                if (ymin > y) ymin = y;
                                if (ymax < y) ymax = y;
                            }
                            float xmid = (xmin + xmax) / 2;
                            float ymid = (ymin + ymax) / 2;

                            p = 0;
                            for (int n = 0; n < 5; n++) p = sr.IndexOf("\n", p + 1);

                            for (int n = 0; n < natom; n++)
                            {
                                dlin = sr.Substring(p + 1, 10);
                                float x = (float)Convert.ToDouble(dlin);
                                dlin = sr.Substring(p + 11, 10);
                                float y = (float)Convert.ToDouble(dlin);
                                x = x - xmid;
                                y = y - ymid;
                                x = 2 * x;
                                y = 2 * y;
                                string hao = String.Format("{0,10:0.0000}", x);
                                string huai = String.Format("{0,10:0.0000}", y);
                                sr = sr.Substring(0, p + 1) + hao + huai + sr.Substring(p + 21);

                                p = sr.IndexOf("\n", p + 1);
                            }

                            rxnmod = rxnmod + sr;

                            j = k; mdx += 1;
                        } while (k > -1);


                        layTable.Rows[i]["CHIME"] = rxnmod;

                    }

                    for (int i = 0; i < artTable.Rows.Count; i++)
                    {
                        rfbs.Filter = "artlno=" + artTable.Rows[i][1].ToString() + " and path = '" + artTable.Rows[i][2].ToString() + "'";
                        rfbs.Sort = "step";

                        rfbs.MoveFirst();
                        for (int j = 0; j < rfbs.Count; j++)
                        {
                            DataRowView dr = (DataRowView)rfbs.Current;

                            if (rfbs.Count == 1) dr.Row["sstep"] = dr.Row["step"] + " Step";
                            else dr.Row["sstep"] = dr.Row["step"] + " of " + rfbs.Count;
                            rfbs.MoveNext();
                        }
                    }

                    //            dataGridView1.DataSource = rfbs;
                    for (int i = 0; i < layTable.Rows.Count; i++)
                    {
                        rfbs.Filter = "rxnid='" + layTable.Rows[i][0].ToString() + "'";
                        rfbs.Sort = "path";
                        string rline = "";

                        rfbs.MoveFirst();
                        for (int j = 0; j < rfbs.Count; j++)
                        {
                            DataRowView dr = (DataRowView)rfbs.Current;
                            if (j == 0)
                            {
                                layTable.Rows[i]["ACCESN.NO"] = dr.Row["acsn"].ToString();
                                layTable.Rows[i]["ARTL.NO"] = dr.Row["artlno"].ToString();
                                layTable.Rows[i]["PATH"] = dr.Row["path"].ToString();
                                layTable.Rows[i]["STEP"] = dr.Row["step"].ToString();
                            }

                            rline = rline + "$DTYPE RXN:VARIATION(1):CCRREF(" + (j + 1) + "):ACCESN.NO\r\n$DATUM " + dr.Row["acsn"].ToString() + "\r\n";
                            rline = rline + "$DTYPE RXN:VARIATION(1):CCRREF(" + (j + 1) + "):ARTL.NO\r\n$DATUM " + dr.Row["artlno"].ToString() + "\r\n";
                            rline = rline + "$DTYPE RXN:VARIATION(1):CCRREF(" + (j + 1) + "):PATH\r\n$DATUM " + dr.Row["path"].ToString() + "\r\n";
                            rline = rline + "$DTYPE RXN:VARIATION(1):CCRREF(" + (j + 1) + "):STEP\r\n$DATUM " + dr.Row["sstep"].ToString() + "\r\n";
                            if (dr.Row["nsm"].ToString().Length > 0)
                                rline = rline + "$DTYPE RXN:VARIATION(1):CCRREF(" + (j + 1) + "):NSM\r\n$DATUM " + dr.Row["nsm"].ToString() + "\r\n";

                            rfbs.MoveNext();
                        }
                        layTable.Rows[i]["CCRREF"] = rline;


                        rtbs.DataSource = rctTable;
                        rtbs.Filter = "RXNID='" + layTable.Rows[i][0].ToString() + "'";
                        rtbs.Sort = "REACTANT_POS";
                        rline = "";
                        rtbs.MoveFirst();
                        for (int j = 0; j < rtbs.Count; j++)
                        {
                            DataRowView dr = (DataRowView)rtbs.Current;
                            if (dr.Row["GRADE"].ToString().Length > 0)
                                rline = rline + "$DTYPE RXN:VARIATION(1):REACTANT(" + dr.Row["REACTANT_POS"].ToString() + "):GRADE\r\n" + rdformat("$DATUM " + dr.Row["GRADE"].ToString());
                            rtbs.MoveNext();
                        }
                        layTable.Rows[i]["REACTANT"] = rline;

                        cabs.DataSource = catTable;
                        cabs.Filter = "rxnid='" + layTable.Rows[i][0].ToString() + "'";
                        cabs.Sort = "CATALYST_POS";
                        rline = "";
                        cabs.MoveFirst();
                        for (int j = 0; j < cabs.Count; j++)
                        {
                            DataRowView dr = (DataRowView)cabs.Current;
                            rline = rline + "$DTYPE RXN:VARIATION(1):CATALYST(" + dr.Row["CATALYST_POS"].ToString() + "):REGNO\r\n" + rdformat("$DATUM " + dr.Row["SYMBOL"].ToString());
                            cabs.MoveNext();
                        }
                        layTable.Rows[i]["CATALYST"] = rline;

                        slbs.DataSource = solTable;
                        slbs.Filter = "rxnid='" + layTable.Rows[i][0].ToString() + "'";
                        slbs.Sort = "SOLVENT_POS";
                        rline = "";
                        slbs.MoveFirst();
                        for (int j = 0; j < slbs.Count; j++)
                        {
                            DataRowView dr = (DataRowView)slbs.Current;
                            rline = rline + "$DTYPE RXN:VARIATION(1):SOLVENT(" + dr.Row["SOLVENT_POS"].ToString() + "):REGNO\r\n" + rdformat("$DATUM " + dr.Row["SYMBOL"].ToString());
                            slbs.MoveNext();
                        }
                        layTable.Rows[i]["SOLVENT"] = rline;

                        prbs.DataSource = prdTable;
                        prbs.Filter = "RXNID='" + layTable.Rows[i][0].ToString() + "'";
                        prbs.Sort = "PRODUCT_POS";
                        rline = "";
                        prbs.MoveFirst();
                        for (int j = 0; j < prbs.Count; j++)
                        {
                            DataRowView dr = (DataRowView)prbs.Current;
                            if (dr.Row["GRADE"].ToString().Length > 0)
                                rline = rline + "$DTYPE RXN:VARIATION(1):PRODUCT(" + dr.Row["PRODUCT_POS"].ToString() + "):PRODAT:GRADE\r\n" + rdformat("$DATUM " + dr.Row["GRADE"].ToString());
                            if (dr.Row["YIELD"].ToString().Length > 0)
                                rline = rline + "$DTYPE RXN:VARIATION(1):PRODUCT(" + dr.Row["PRODUCT_POS"].ToString() + "):PRODAT:YIELD\r\n$DATUM " + dr.Row["YIELD"].ToString() + "\r\n";

                            prbs.MoveNext();
                        }
                        layTable.Rows[i]["PRODUCT"] = rline;


                        cdbs.DataSource = cndTable;
                        cdbs.Filter = "RXNID='" + layTable.Rows[i][0].ToString() + "'";
                        rline = "";
                        cdbs.MoveFirst();
                        for (int j = 0; j < cdbs.Count; j++)
                        {
                            DataRowView dr = (DataRowView)cdbs.Current;

                            if(dr.Row["ATM"].ToString().Length > 0)
                                 rline = rline + "$DTYPE RXN:VARIATION(1):CONDITIONS:ATMOSPHERE\r\n$DATUM " + dr.Row["ATM"].ToString() + "\r\n";
                            if (dr.Row["PRESS"].ToString().Length > 0)
                                rline = rline + "$DTYPE RXN:VARIATION(1):CONDITIONS:PRESSURE\r\n$DATUM " + dr.Row["PRESS"].ToString() + " ATM\r\n";
                            if (dr.Row["TIME"].ToString().Length > 0)
                                rline = rline + "$DTYPE RXN:VARIATION(1):CONDITIONS:TIME\r\n$DATUM " + dr.Row["TIME"].ToString() + " HR\r\n";
                            if (dr.Row["TEMP"].ToString().Length > 0)
                                rline = rline + "$DTYPE RXN:VARIATION(1):CONDITIONS:TEMP\r\n$DATUM " + dr.Row["TEMP"].ToString() + " DEG C\r\n";
                            if (dr.Row["REFLUXED"].ToString().Length > 0)
                                rline = rline + "$DTYPE RXN:VARIATION(1):CONDITIONS:REFLUXED\r\n$DATUM " + dr.Row["REFLUXED"].ToString() + "\r\n";
                            if (dr.Row["OTHER"].ToString().Length > 0)
                                rline = rline + "$DTYPE RXN:VARIATION(1):CONDITIONS:OTHER\r\n" + rdformat("$DATUM " + dr.Row["OTHER"].ToString());

                            cdbs.MoveNext();
                        }
                        layTable.Rows[i]["CONDITIONS"] = rline;

                        cmbs.DataSource = cmtTable;
                        cmbs.Filter = "RXNID='" + layTable.Rows[i][0].ToString() + "'";
                        cmbs.Sort = "idx";
                        rline = "";
                        cmbs.MoveFirst();
                        for (int j = 0; j < cmbs.Count; j++)
                        {
                            DataRowView dr = (DataRowView)cmbs.Current;
                            rline = rline + "$DTYPE RXN:VARIATION(1):COMMENTS(" + dr.Row["idx"].ToString() + ")\r\n" + rdformat("$DATUM " + dr.Row["COMMENTS"].ToString());
                            cmbs.MoveNext();
                        }
                        layTable.Rows[i]["COMMENTS"] = rline;

                    }

                    laybs.DataSource = layTable;
                    laybs.Sort = "ARTL.NO, PATH, STEP";

                    string outdata = "$RDFILE 1\r\n$DATM    " + DateTime.Now.Date.ToString("MM/dd/yy HH:MM") + "\r\n";
                    laybs.MoveFirst();
                    for (int j = 0; j < laybs.Count; j++)
                    {
                        DataRowView dr = (DataRowView)laybs.Current;
                        outdata = outdata + "$RFMT $RIREG " + dr.Row["RXNID"].ToString() + "\r\n" + dr.Row["CHIME"].ToString().Replace("\n","\r\n") + dr.Row["CCRREF"].ToString();

                        if (comboBox2.Text == "CP")
                            outdata = outdata + dr.Row["REACTANT"].ToString();

                        outdata = outdata + dr.Row["CATALYST"].ToString() + dr.Row["SOLVENT"].ToString();

                        if (comboBox2.Text == "CP")
                            outdata = outdata + dr.Row["PRODUCT"].ToString() + dr.Row["CONDITIONS"].ToString() + dr.Row["COMMENTS"].ToString();

                        laybs.MoveNext();
                    }

                    if (comboBox2.Text == "CP")
                        File.WriteAllText("I:\\users\\HDS\\CR\\" + listBox1.SelectedItems[0].ToString() + "CP.RDF", outdata);
                    else if (comboBox2.Text == "IP")
                        File.WriteAllText("I:\\users\\HDS\\IC\\" + listBox1.SelectedItems[0].ToString() + "IP.RDF", outdata);


                    int nix = Convert.ToInt32(listBox1.SelectedIndices[0].ToString());
                    //                    MessageBox.Show(listBox1.SelectedItems[0].ToString() + "   " + nix);
                    listBox1.SetSelected(nix, false);

                    Refresh();
                    Application.DoEvents();

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                this.Cursor = Cursors.Default;
                return;
            }

        }



        private void dataGridView1_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            renditor1.ChimeString = dataGridView1.CurrentRow.Cells["CHIME"].FormattedValue.ToString();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            renditor1.ChimeString = dataGridView1.CurrentRow.Cells["CHIME"].FormattedValue.ToString();

        }


        private string rdformat(string inline)
        {
            string outline = "";
            if (inline.Length > 0)
            {
                int i = 0;
                for (i = 0; i < ((inline.Length-1 ) / 80); i++)
                {
                    outline = outline + inline.Substring(i * 80, 80) + "+\r\n";
                }
                outline = outline + inline.Substring(i * 80) + "\r\n";
            }
            return outline;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (comboBox2.SelectedIndex < 0) return;

            comboBox1.Items.Clear();

            if (OraConn.State != ConnectionState.Closed) OraConn.Close();

            if (comboBox2.Text == "IC")
            {
                OraConn = icOraConn;
//                OraConn.ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.224.2.91)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=CHEMINFO)));User Id=icpdn;Password=icpdn;";
                cmd.CommandText = "select unique batchno from ic_moltable order by 1 desc";
            }
            else
            {
                OraConn = ccrOraConn;
//                OraConn.ConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=10.224.2.91)(PORT=1521)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=CHEMINFO)));User Id=ccrpdn;Password=ccrpdn;";
                cmd.CommandText = "select unique batchno from ccr_batch where batchno is not null order by 1 desc";
            }


            try
            {
                OraConn.Open();
                cmd.Connection = OraConn;
                cmd.CommandType = CommandType.Text;
                OracleDataReader dr1 = cmd.ExecuteReader();

                while (dr1.Read())
                {
                    comboBox1.Items.Add(dr1.GetDecimal(0));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraConn.Close();
                return;
            }

            OraConn.Close();

            if (comboBox1.Items.Count > 0) comboBox1.SelectedIndex = 0;

        }

        private void renditor1_Load(object sender, EventArgs e)
        {

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

                connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + host + ")(PORT=" + port + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + service + "))); User Id=icpdn;Password=icpdn;";
                settings.Save();

                icpdnConnectionString = connectionString + "User Id=icpdn;Password=icpdn;";
                ccrpdnConnectionString = connectionString + "User Id=ccrpdn;Password=ccrpdn;";
                icOraConn.ConnectionString = icpdnConnectionString;
                ccrOraConn.ConnectionString = ccrpdnConnectionString;
 
            }
        }


    }
}