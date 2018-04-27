using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using reaccsLoad.Properties;


namespace reaccsLoad
{
    public partial class Form1 : Form
    {
        static private string connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1550)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=chemdb.prod-wos.com)));";


        static private string ccrbibConnectionString = connectionString + "User Id=ccrbib;Password=ccrbib;";
        static private string ccrpdnConnectionString = connectionString + "User Id=ccrpdn;Password=ccrpdn;";
        static private string icannConnectionString = connectionString + "User Id=icann;Password=icann;";
        static private string ccrannConnectionString = connectionString + "User Id=ccrann;Password=ccrann;";
        static private Properties.Settings settings = Properties.Settings.Default;


        static private string bibpath = "";
        static private DataTable jrnTable = new DataTable("JOURNAL");
        static private BindingSource jrbs = new BindingSource();
        static private DataTable autTable = new DataTable("AUTHOR");
        static private BindingSource aubs = new BindingSource();

        static private DataTable molTable = new DataTable("MOL");
        static private DataTable symTable = new DataTable("SYM");
        static private BindingSource smbs = new BindingSource();
        static private DataTable bioTable = new DataTable("BIO");
        static private BindingSource bibs = new BindingSource();
        static private DataTable cttTable = new DataTable("CTT");
        static private BindingSource ctbs = new BindingSource();

        static private DataTable acsnTable = new DataTable("ACSN");

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
        static private DataTable keyTable = new DataTable("KEY");
        static private BindingSource kybs = new BindingSource();
        static private DataTable ccrTable = new DataTable("CCR");
        static private BindingSource crbs = new BindingSource();


        static private OracleConnection OraConn = new OracleConnection();
        static private OracleCommand cmd = new OracleCommand();
        static private OracleDataAdapter cpdAdapter = new OracleDataAdapter();

        static private Int64 mireg = 0;

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
            ccrbibConnectionString = connectionString + "User Id=ccrbib;Password=ccrbib;";
            ccrpdnConnectionString = connectionString + "User Id=ccrpdn;Password=ccrpdn;";
            icannConnectionString = connectionString + "User Id=icann;Password=icann;";
            ccrannConnectionString = connectionString + "User Id=ccrann;Password=ccrann;";

            panel2.Visible = false;

            MainMenu MyMenu;

            MyMenu = new MainMenu();

            MenuItem m1 = new MenuItem("File");
            MyMenu.MenuItems.Add(m1);

            MenuItem subm1 = new MenuItem("Bibliographic");
            m1.MenuItems.Add(subm1);

            MenuItem subm2 = new MenuItem("Reaccs RdFile");
            m1.MenuItems.Add(subm2);

            MenuItem subm3 = new MenuItem("Database Configuration");
            m1.MenuItems.Add(subm3);

            MenuItem subm4 = new MenuItem("Exit");
            m1.MenuItems.Add(subm4);

            subm1.Click += new EventHandler(MMBibClick);
            subm2.Click += new EventHandler(MMRdfClick);
            subm3.Click += new EventHandler(MMConfigClick);
            subm4.Click += new EventHandler(MMExitClick);

            panel2.Visible = false;

            mTextBox3.Mask = "P00000";
            mTextBox4.Mask = "P00000";
            mTextBox5.Mask = "S00000";
            mTextBox6.Mask = "S00000";
    
            Menu = MyMenu;

            

            bibtabledef();
            rxntabledef();

            mTextBox1.Focus();
        }


        protected void MMExitClick(object who, EventArgs e)
        {
            Application.Exit();
        }

        protected void MMBibClick(object who, EventArgs e)
        {
            button1.Visible = true;
//            button2.Visible = true;
            if (OraConn.State == ConnectionState.Open) OraConn.Close();
            panel1.Visible = false;
            panel2.Visible = false;
        }

        protected void MMConfigClick(object who, EventArgs e)
        {
            panel2.Visible = true;
        }

        protected void MMRdfClick(object who, EventArgs e)
        {

            mTextBox1.Focus();

            button1.Visible = false;
            button2.Visible = false;

            panel1.Visible = true;
            panel2.Visible = false;
        } 



        private void button1_Click(object sender, EventArgs e)
        {
            button1.Visible = false;
            Cursor = Cursors.WaitCursor;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();

            openFileDialog1.Title = "Select CCR Bibliographic File:";
            openFileDialog1.Filter = "Reaction Files|*.dat";
            openFileDialog1.InitialDirectory = "\\\\34.212.24.69\\Applications\\IC\\";
            
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;

            bibpath = openFileDialog1.FileName;

            

            readbibfile(bibpath);
            jrbs.DataSource = jrnTable;
            jrbs.Sort = "ABSNUM";

//            dataGridView1.DataSource = autTable;
            Refresh();

            button1.Visible = true;
            Cursor = Cursors.Default;
            label4.Text = bibpath + " Done";
        }

        private void bibtabledef()
        {
            DataColumn myDataColumn;

            myDataColumn = new DataColumn("ABSNUM", System.Type.GetType("System.String"));
            jrnTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("JRNL", System.Type.GetType("System.String"));
            jrnTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("UT", System.Type.GetType("System.String"));
            jrnTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("ACSN", System.Type.GetType("System.String"));
            jrnTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("ARTL", System.Type.GetType("System.String"));
            jrnTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("TITLE", System.Type.GetType("System.String"));
            jrnTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("NAME", System.Type.GetType("System.String"));
            jrnTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("ADDRESS", System.Type.GetType("System.String"));
            jrnTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("ABSTRACT", System.Type.GetType("System.String"));
            jrnTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("ABST", System.Type.GetType("System.String"));
            jrnTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("VOL", System.Type.GetType("System.String"));
            jrnTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("NO", System.Type.GetType("System.String"));
            jrnTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("PAGE.START", System.Type.GetType("System.String"));
            jrnTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("PAGE.END", System.Type.GetType("System.String"));
            jrnTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("YEAR", System.Type.GetType("System.String"));
            jrnTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("LANGUAGE", System.Type.GetType("System.String"));
            jrnTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("LITTEXT", System.Type.GetType("System.String"));
            jrnTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("RDF", System.Type.GetType("System.String"));
            jrnTable.Columns.Add(myDataColumn);
            jrbs.DataSource = jrnTable;

            myDataColumn = new DataColumn("UT", System.Type.GetType("System.String"));
            autTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("AUTHOR_POSITION", System.Type.GetType("System.String"));
            autTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("AUTHOR", System.Type.GetType("System.String"));
            autTable.Columns.Add(myDataColumn);
            aubs.DataSource = autTable;
        }


        private void rxntabledef()
        {
            DataColumn myDataColumn;

            myDataColumn = new DataColumn("RXNID", System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("CHIME", System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("ACCESN.NO", System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("ABSNUM", System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("ARTL.NO", System.Type.GetType("System.Decimal"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("PATH", System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("STEP", System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("NSM", System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("CCRREF", System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("REACTANT", System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("CATALYST", System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("SOLVENT", System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("RXNTEXT", System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("PRODUCT", System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("CONDITIONS", System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("COMMENTS", System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("KEYPHRASES", System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("CCRNUMBER", System.Type.GetType("System.String"));
            layTable.Columns.Add(myDataColumn);


        }


        private string writemol()
        {

            molTable.Rows.Clear();
            symTable.Rows.Clear();
            bioTable.Rows.Clear();
            cttTable.Rows.Clear();

            molTable.Dispose();
            symTable.Dispose();
            bioTable.Dispose();
            cttTable.Dispose();

            try
            {
                OraConn.Open();
                cmd.Connection = OraConn;
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = "select t.rid, t.molregno, molfile(m.ctab) molfile, isnostruct(ctab) nostruct from ccr_moltable m, regnotmp t where m.molregno = t.molregno order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(molTable);

                cmd.CommandText = "select t.rid, t.molregno, m.idx, m.symbol from ccr_symbol m, regnotmp t where m.molregno = t.molregno order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(symTable);

                cmd.CommandText = "select t.rid, t.molregno, m.idx, m.ccr_biolact from ccr_biolact m, regnotmp t where m.molregno = t.molregno order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(bioTable);

                cmd.CommandText = "select t.rid, t.molregno, m.idx, m.CAT_TYPE_TEXT from cat_type m, regnotmp t where m.molregno = t.molregno order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(cttTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraConn.Close();
                this.Cursor = Cursors.Default;
                return ex.ToString();
            }
            OraConn.Close();

            string outmol = "", outdata = "", nodata = "";

            for (int i = 0; i < molTable.Rows.Count; i++)
            {
                outmol = "";
                string ctbl = "";
                StringReader sr = new StringReader(molTable.Rows[i][2].ToString());
                while (sr.Peek() > 0)
                {
                    string dline = sr.ReadLine();
                    ctbl = ctbl + dline + "\r\n";
                }

                outmol = outmol + "$MFMT $MIREG " + molTable.Rows[i][0].ToString() + "\r\n" + ctbl;

                smbs.DataSource = symTable;
                smbs.Filter = "MOLREGNO=" + molTable.Rows[i][1].ToString();
                smbs.Sort = "idx";
                smbs.MoveFirst();
                for (int j = 0; j < smbs.Count; j++)
                {
                    DataRowView dr = (DataRowView)smbs.Current;
                    outmol = outmol + "$DTYPE MOL:SYMBOL(" + dr.Row["idx"].ToString() + ")\r\n" + rdformat(dr.Row["SYMBOL"].ToString());
                    smbs.MoveNext();
                }

                bibs.DataSource = bioTable;
                bibs.Filter = "MOLREGNO=" + molTable.Rows[i][1].ToString();
                bibs.Sort = "idx";
                bibs.MoveFirst();
                for (int j = 0; j < bibs.Count; j++)
                {
                    DataRowView dr = (DataRowView)bibs.Current;
                    outmol = outmol + "$DTYPE MOL:BIOLACT(" + dr.Row["idx"].ToString() + ")\r\n" + rdformat(dr.Row["CCR_BIOLACT"].ToString());
                    bibs.MoveNext();
                }

                ctbs.DataSource = cttTable;
                ctbs.Filter = "MOLREGNO=" + molTable.Rows[i][1].ToString();
                ctbs.Sort = "idx";
                ctbs.MoveFirst();
                for (int j = 0; j < ctbs.Count; j++)
                {
                    DataRowView dr = (DataRowView)ctbs.Current;
                    outmol = outmol + "$DTYPE MOL:CAT.TYPE(" + dr.Row["idx"].ToString() + ")\r\n" + rdformat(dr.Row["CAT_TYPE_TEXT"].ToString());
                    ctbs.MoveNext();
                }

                if (molTable.Rows[i][3].ToString() == "0")
                {
                    outdata = outdata + outmol;
                }
                else
                {
                    nodata = nodata + outmol;
                }
            }

            molTable.Dispose();

            return nodata + "\r\nnodatanodata\r\n" + outdata;
        }


        private string writepat(int n)
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
            keyTable.Rows.Clear();
            jrnTable.Rows.Clear();
            ccrTable.Rows.Clear();

            refTable.Dispose();
            layTable.Dispose();
            artTable.Dispose();
            rctTable.Dispose();
            catTable.Dispose();
            solTable.Dispose();
            prdTable.Dispose();
            cndTable.Dispose();
            cmtTable.Dispose();
            keyTable.Dispose();
            jrnTable.Dispose();
            ccrTable.Dispose();

            string pnumber = acsnTable.Rows[n]["ACSN"].ToString();

            querypat(pnumber);

            refform();
            rdfpop(pnumber);

            string outdata = "";

            laybs.DataSource = layTable;
            laybs.Sort = "ABSNUM, PATH, STEP";

            laybs.MoveFirst();
            for (int j = 0; j < laybs.Count; j++)
            {
                DataRowView dr = (DataRowView)laybs.Current;

                string artno = dr.Row["ABSNUM"].ToString();
                jrbs.DataSource = jrnTable;
                jrbs.Filter = "ABSNUM='" + artno + "'";
                jrbs.MoveFirst();
                DataRowView d2 = (DataRowView)jrbs.Current;


                string ctbl = "";
                StringReader sr = new StringReader(dr.Row["CHIME"].ToString());
                while (sr.Peek() > 0)
                {
                    string dline = sr.ReadLine();
                    ctbl = ctbl + dline + "\r\n";
                }

                

                outdata = outdata + "$RFMT $RIREG " + dr.Row["RXNID"].ToString() + "\r\n" + ctbl + dr.Row["RXNTEXT"].ToString() + dr.Row["CCRREF"].ToString();

                outdata = outdata + dr.Row["REACTANT"].ToString() + dr.Row["PRODUCT"].ToString();

                outdata = outdata + dr.Row["CATALYST"].ToString() + dr.Row["SOLVENT"].ToString();

                outdata = outdata + dr.Row["CONDITIONS"].ToString() + dr.Row["COMMENTS"].ToString() + dr.Row["KEYPHRASES"].ToString();

                outdata = outdata + d2.Row["RDF"].ToString();

                outdata = outdata + dr.Row["CCRNUMBER"].ToString();

                laybs.MoveNext();
            }



            return outdata;
        }



        private string writeart(int n)
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
            keyTable.Rows.Clear();
            jrnTable.Rows.Clear();
            ccrTable.Rows.Clear();

            refTable.Dispose();
            layTable.Dispose();
            artTable.Dispose();
            rctTable.Dispose();
            catTable.Dispose();
            solTable.Dispose();
            prdTable.Dispose();
            cndTable.Dispose();
            cmtTable.Dispose();
            keyTable.Dispose();
            jrnTable.Dispose();
            ccrTable.Dispose();


            string acsn = acsnTable.Rows[n]["ACSN"].ToString();

            queryora(acsn);

            string outdata = "";

            refform();

            rdfpop(acsn);

            laybs.DataSource = layTable;
            laybs.Sort = "ABSNUM, PATH, STEP";

            laybs.MoveFirst();
            for (int j = 0; j < laybs.Count; j++)
            {
                DataRowView dr = (DataRowView)laybs.Current;

                string artno = dr.Row["ABSNUM"].ToString();
                jrbs.DataSource = jrnTable;
                jrbs.Filter = "ABSNUM='" + artno + "'";
                jrbs.MoveFirst();
                DataRowView d2 = (DataRowView)jrbs.Current;


                string ctbl = "";
                StringReader sr = new StringReader(dr.Row["CHIME"].ToString());
                while (sr.Peek() > 0)
                {
                    string dline = sr.ReadLine();
                    ctbl = ctbl + dline + "\r\n";
                }


                outdata = outdata + "$RFMT $RIREG " + dr.Row["RXNID"].ToString() + "\r\n" + ctbl + dr.Row["RXNTEXT"].ToString() + dr.Row["CCRREF"].ToString();

                outdata = outdata + dr.Row["REACTANT"].ToString() + dr.Row["PRODUCT"].ToString();

                outdata = outdata + dr.Row["CATALYST"].ToString() + dr.Row["SOLVENT"].ToString();

                outdata = outdata + dr.Row["CONDITIONS"].ToString() + dr.Row["COMMENTS"].ToString() + dr.Row["KEYPHRASES"].ToString();

                outdata = outdata + d2.Row["RDF"].ToString();

                if (dr.Row["PATH"].ToString() == "A1" && dr.Row["STEP"].ToString().IndexOf("Step") > -1) outdata = outdata + d2.Row["ABSTRACT"].ToString();

                outdata = outdata + dr.Row["CCRNUMBER"].ToString();

                laybs.MoveNext();
            }

            

            return outdata;

        }



        private void rdfpop(string acsn)
        {      
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
                        layTable.Rows[i]["ACCESN.NO"] = acsn;
//                        layTable.Rows[i]["ARTL.NO"] = dr.Row["artlno"].ToString();
                        layTable.Rows[i]["ABSNUM"] = dr.Row["absnum"].ToString();
//                        layTable.Rows[i]["CCRNUMBER"] = dr.Row["CCRNUMBER"].ToString();
                        layTable.Rows[i]["PATH"] = dr.Row["path"].ToString();
                        layTable.Rows[i]["STEP"] = dr.Row["sstep"].ToString();
                    }

                    rline = rline + "$DTYPE RXN:VARIATION(1):CCRREF(" + (j + 1) + "):EXTREG\r\n$DATUM " + dr.Row["absnum"].ToString() + "\r\n";
                    rline = rline + "$DTYPE RXN:VARIATION(1):CCRREF(" + (j + 1) + "):PATH\r\n$DATUM " + dr.Row["path"].ToString() + "\r\n";
                    rline = rline + "$DTYPE RXN:VARIATION(1):CCRREF(" + (j + 1) + "):STEP\r\n$DATUM " + dr.Row["sstep"].ToString() + "\r\n";
                    if (dr.Row["nsm"].ToString().Length > 0)
                        rline = rline + "$DTYPE RXN:VARIATION(1):CCRREF(" + (j + 1) + "):NSM\r\n$DATUM " + dr.Row["nsm"].ToString() + "\r\n";

                    rfbs.MoveNext();
                }
                layTable.Rows[i]["CCRREF"] = rline;


                crbs.DataSource = ccrTable;
                crbs.Filter = "RXNID='" + layTable.Rows[i][0].ToString() + "'";
                crbs.MoveFirst();

                

                for (int m2 = 0; m2 < crbs.Count; m2++)
                {
                    DataRowView dr = (DataRowView)crbs.Current;
                    layTable.Rows[i]["CCRNUMBER"] = "$DTYPE RXN:VARIATION(1):CCRNUMBER\r\n$DATUM " + dr.Row["CCRNUMBER"].ToString() + "\r\n";
                    
                    crbs.MoveNext();
                }


                rtbs.DataSource = rctTable;
                rtbs.Filter = "RXNID='" + layTable.Rows[i][0].ToString() + "'";
                rtbs.Sort = "REACTANT_POS";
                rline = "";
                rtbs.MoveFirst();
                for (int m2 = 0; m2 < rtbs.Count; m2++)
                {
                    DataRowView dr = (DataRowView)rtbs.Current;
                    if (dr.Row["GRADE"].ToString().Length > 0)
                        rline = rline + "$DTYPE RXN:VARIATION(1):REACTANT(" + dr.Row["REACTANT_POS"].ToString() + "):GRADE\r\n" + rdformat(dr.Row["GRADE"].ToString());
                    rtbs.MoveNext();
                }
                layTable.Rows[i]["REACTANT"] = rline;


                int irtxt1 = 0;
                int irtxt2 = 0;
                string srtxt1 = "";
                string srtxt2 = "";

                cabs.DataSource = catTable;
                cabs.Filter = "rxnid='" + layTable.Rows[i][0].ToString() + "'";
                cabs.Sort = "CATALYST_POS";
                rline = "";
                cabs.MoveFirst();
                for (int m2 = 0; m2 < cabs.Count; m2++)
                {
                    DataRowView dr = (DataRowView)cabs.Current;
                    irtxt1++; irtxt2++;
                    rline = rline + "$DTYPE RXN:VARIATION(1):CATALYST(" + dr.Row["CATALYST_POS"].ToString() + "):REGNO\r\n" + rdformat(dr.Row["SYMBOL"].ToString());
                    srtxt1 = srtxt1 + "$DTYPE RXN:VARIATION(1):RXNTEXT(" + irtxt1 + ")\r\n" + rdformat(dr.Row["SYMBOL"].ToString());
                    srtxt2 = srtxt2 + "$DTYPE RXN:VARIATION(1):RXNTEXT(" + irtxt2 + ")\r\n" + rdformat(dr.Row["SYMBOL"].ToString());

                    if (dr.Row["GRADE"].ToString().Length > 0)
                    {
                        irtxt2++;
                        rline = rline + "$DTYPE RXN:VARIATION(1):CATALYST(" + dr.Row["CATALYST_POS"].ToString() + "):GRADE\r\n" + rdformat(dr.Row["GRADE"].ToString());
                        srtxt2 = srtxt2 + "$DTYPE RXN:VARIATION(1):RXNTEXT(" + irtxt2 + ")\r\n" + rdformat("(" + dr.Row["GRADE"].ToString() + ")");
                    }
                    cabs.MoveNext();
                }
                layTable.Rows[i]["CATALYST"] = rline;

                slbs.DataSource = solTable;
                slbs.Filter = "rxnid='" + layTable.Rows[i][0].ToString() + "'";
                slbs.Sort = "SOLVENT_POS";
                rline = "";
                slbs.MoveFirst();
                for (int m2 = 0; m2 < slbs.Count; m2++)
                {
                    DataRowView dr = (DataRowView)slbs.Current;
                    rline = rline + "$DTYPE RXN:VARIATION(1):SOLVENT(" + dr.Row["SOLVENT_POS"].ToString() + "):REGNO\r\n" + rdformat(dr.Row["SYMBOL"].ToString());
                    irtxt1++; irtxt2++;
                    srtxt1 = srtxt1 + "$DTYPE RXN:VARIATION(1):RXNTEXT(" + irtxt1 + ")\r\n" + rdformat(dr.Row["SYMBOL"].ToString());
                    srtxt2 = srtxt2 + "$DTYPE RXN:VARIATION(1):RXNTEXT(" + irtxt2 + ")\r\n" + rdformat(dr.Row["SYMBOL"].ToString());

                    if (dr.Row["GRADE"].ToString().Length > 0)
                    {
                        irtxt2++;
                        rline = rline + "$DTYPE RXN:VARIATION(1):SOLVENT(" + dr.Row["SOLVENT_POS"].ToString() + "):GRADE\r\n" + rdformat(dr.Row["GRADE"].ToString());
                        srtxt2 = srtxt2 + "$DTYPE RXN:VARIATION(1):RXNTEXT(" + irtxt2 + ")\r\n" + rdformat("(" + dr.Row["GRADE"].ToString()+ ")");
                    }
                    
                    slbs.MoveNext();
                }
                layTable.Rows[i]["SOLVENT"] = rline;

                if (irtxt2 > 6) layTable.Rows[i]["RXNTEXT"] = srtxt1;
                else layTable.Rows[i]["RXNTEXT"] = srtxt2;


                prbs.DataSource = prdTable;
                prbs.Filter = "RXNID='" + layTable.Rows[i][0].ToString() + "'";
                prbs.Sort = "PRODUCT_POS";
                rline = "";
                prbs.MoveFirst();
                for (int m2 = 0; m2 < prbs.Count; m2++)
                {
                    DataRowView dr = (DataRowView)prbs.Current;
                    if (dr.Row["GRADE"].ToString().Length > 0)
                        rline = rline + "$DTYPE RXN:VARIATION(1):PRODUCT(" + dr.Row["PRODUCT_POS"].ToString() + "):PRODAT:GRADE\r\n" + rdformat(dr.Row["GRADE"].ToString());
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
                    if (dr.Row["ATM"].ToString().Length > 0)
                        rline = rline + "$DTYPE RXN:VARIATION(1):CONDITIONS(1):ATMOSPHERE\r\n$DATUM " + dr.Row["ATM"].ToString() + "\r\n";
                    if (dr.Row["PRESS"].ToString().Length > 0)
                        rline = rline + "$DTYPE RXN:VARIATION(1):CONDITIONS(1):PRESSURE\r\n$DATUM " + dr.Row["PRESS"].ToString() + " ATM\r\n";
                    if (dr.Row["TIME"].ToString().Length > 0)
                        rline = rline + "$DTYPE RXN:VARIATION(1):CONDITIONS(1):TIME\r\n$DATUM " + dr.Row["TIME"].ToString() + " HR\r\n";
                    if (dr.Row["TEMP"].ToString().Length > 0)
                        rline = rline + "$DTYPE RXN:VARIATION(1):CONDITIONS(1):TEMP\r\n$DATUM " + dr.Row["TEMP"].ToString() + " DEG C\r\n";
                    if (dr.Row["REFLUXED"].ToString().Length > 0)
                        rline = rline + "$DTYPE RXN:VARIATION(1):CONDITIONS(1):REFLUX\r\n$DATUM " + dr.Row["REFLUXED"].ToString() + "\r\n";
                    if (dr.Row["OTHER"].ToString().Length > 0)
                        rline = rline + "$DTYPE RXN:VARIATION(1):CONDITIONS(1):OTHER\r\n" + rdformat(dr.Row["OTHER"].ToString());

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
                    rline = rline + "$DTYPE RXN:VARIATION(1):COMMENTS(" + dr.Row["idx"].ToString() + ")\r\n" + rdformat(dr.Row["COMMENTS"].ToString());
                    cmbs.MoveNext();
                }
                layTable.Rows[i]["COMMENTS"] = rline;

                kybs.DataSource = keyTable;
                kybs.Filter = "RXNID='" + layTable.Rows[i][0].ToString() + "'";
                rline = "";
                kybs.MoveFirst();
                for (int j = 0; j < kybs.Count; j++)
                {
                    DataRowView dr = (DataRowView)kybs.Current;
                    rline = rline + "$DTYPE RXN:VARIATION(1):KEYPHRASES\r\n" + rdformat(dr.Row["KEYPHRASE"].ToString());
                    kybs.MoveNext();
                }
                layTable.Rows[i]["KEYPHRASES"] = rline;

            }   
        }
         



        private void refform()
        {

            rfbs.DataSource = refTable;
            for (int m1 = 0; m1 < artTable.Rows.Count; m1++)
            {
                rfbs.Filter = "absnum = '" + artTable.Rows[m1]["ABSNUM"].ToString() + "' and path = '" + artTable.Rows[m1][1].ToString() + "'";
                rfbs.Sort = "step";

                rfbs.MoveFirst();
                for (int m2 = 0; m2 < rfbs.Count; m2++)
                {
                    DataRowView dr = (DataRowView)rfbs.Current;

                    if (rfbs.Count == 1) dr.Row["sstep"] = dr.Row["step"] + " Step";
                    else if (m2 == rfbs.Count - 1) dr.Row["sstep"] = m2 + " Step";
                    else dr.Row["sstep"] = dr.Row["step"] + " of " + (rfbs.Count - 1);
                    rfbs.MoveNext();
                }
            }
        }


        private void querypat(string pnumber)
        {
            try
            {
                OraConn.Open();
                cmd.Connection = OraConn;
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = "select PAT_NUMBER, ABSNUM, RDF from ccrbib.ccr_article where PAT_NUMBER='" + pnumber + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(jrnTable);

                cmd.CommandText = "select b.rxnid, rxnfile(b.rctab) chime from ccrann.ccr_batch b, ccrann.rxn_pat c where b.rxnid=c.rxnid and c.PAT_NUMBER='" + pnumber + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(layTable);

                cmd.CommandText = "select c.rxnid, c.pat_number, r.path, '' sstep, r.step, r.nsm, a.absnum from ccrann.rxn_path r, ccrann.rxn_pat c, ccrbib.ccr_article a where r.rxnid= c.rxnid and a.PAT_NUMBER = c.PAT_NUMBER and c.PAT_NUMBER='" + pnumber + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(refTable);

                cmd.CommandText = "select c.rxnid, n.ccrnumber from ccrann.ccrnumber n, ccrann.rxn_pat c, ccrbib.ccr_article a where n.rxnid= c.rxnid and a.PAT_NUMBER = c.PAT_NUMBER and c.PAT_NUMBER='" + pnumber + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(ccrTable);

                cmd.CommandText = "select unique a.absnum, r.path from ccrann.rxn_path r, ccrann.rxn_pat c, ccrbib.ccr_article a where r.rxnid= c.rxnid and a.PAT_NUMBER = c.PAT_NUMBER and c.PAT_NUMBER='" + pnumber + "' order by 1, 2";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(artTable);

                cmd.CommandText = "select r.* from ccrann.reactant_grade r, ccrann.rxn_pat c where r.rxnid= c.rxnid and c.PAT_NUMBER='" + pnumber + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(rctTable);

                cmd.CommandText = "select r.rxnid, r.grade, s.symbol, r.CATALYST_POS from ccrann.catalyst_grade r, ccrann.rxn_pat c, ccr_symbol s where s.idx=1 and r.molregno= s.molregno and r.rxnid= c.rxnid and c.PAT_NUMBER='" + pnumber + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(catTable);

                cmd.CommandText = "select r.rxnid, r.grade, s.symbol, r.SOLVENT_POS  from ccrann.solvent_grade r, ccrann.rxn_pat c, ccr_symbol s where s.idx=1 and r.molregno= s.molregno and r.rxnid= c.rxnid and c.PAT_NUMBER='" + pnumber + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(solTable);

                cmd.CommandText = "select r.* from ccrann.product_grade r, ccrann.rxn_pat c where r.rxnid= c.rxnid and c.PAT_NUMBER='" + pnumber + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(prdTable);

                cmd.CommandText = "select r.* from ccrann.rxn_cond r, ccrann.rxn_pat c where r.rxnid= c.rxnid and c.PAT_NUMBER='" + pnumber + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(cndTable);

                cmd.CommandText = "select r.* from ccrann.ccr_comments r, ccrann.rxn_pat c where r.rxnid= c.rxnid and c.PAT_NUMBER='" + pnumber + "' order by 1, 2";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(cmtTable);

                cmd.CommandText = "select r.* from ccrann.ccr_keyphrases r, ccrann.rxn_pat c where r.rxnid= c.rxnid and c.PAT_NUMBER='" + pnumber + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(keyTable);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraConn.Close();
                this.Cursor = Cursors.Default;
                return;
            }

            OraConn.Close();
        }





        private void queryora(string acsn)
        {
            try
            {
                OraConn.Open();
                cmd.Connection = OraConn;
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = "select tgano ACSN, ABSNUM, artno ARTL, RDF, ABST ABSTRACT from ccrbib.ccr_article where tgano='" + acsn + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(jrnTable);


                cmd.CommandText = "select b.rxnid, rxnfile(b.rctab) chime from ccrann.ccr_batch b, ccrann.ccrnumber c where b.rxnid=c.rxnid and b.ACSN='" + acsn + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(layTable);

                cmd.CommandText = "select c.rxnid, c.acsn, c.artlno, r.path, '' sstep, r.step, r.nsm, n.ccrnumber, n.absnum from ccrann.rxn_path r, ccrann.ccr_batch c, ccrann.ccrnumber n where n.rxnid = r.rxnid and r.rxnid= c.rxnid and c.ACSN='" + acsn + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(refTable);

                cmd.CommandText = "select c.rxnid, n.ccrnumber from ccrann.ccr_batch c, ccrann.ccrnumber n where n.rxnid = c.rxnid and c.ACSN='" + acsn + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(ccrTable);

                cmd.CommandText = "select unique n.absnum, r.path from ccrann.rxn_path r, ccrann.ccr_batch c, ccrann.ccrnumber n where n.rxnid = r.rxnid and r.rxnid= c.rxnid and c.ACSN='" + acsn + "' order by 1, 2";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(artTable);

                cmd.CommandText = "select r.* from ccrann.reactant_grade r, ccrann.ccr_batch c where r.rxnid= c.rxnid and c.acsn='" + acsn + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(rctTable);

                cmd.CommandText = "select r.rxnid, r.grade, s.symbol, r.CATALYST_POS from ccrann.catalyst_grade r, ccrann.ccr_batch c, ccr_symbol s where s.idx=1 and r.molregno= s.molregno and r.rxnid= c.rxnid and c.acsn='" + acsn + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(catTable);

                cmd.CommandText = "select r.rxnid, r.grade, s.symbol, r.SOLVENT_POS  from ccrann.solvent_grade r, ccrann.ccr_batch c, ccr_symbol s where s.idx=1 and r.molregno= s.molregno and r.rxnid= c.rxnid and c.acsn='" + acsn + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(solTable);

                cmd.CommandText = "select r.* from ccrann.product_grade r, ccrann.ccr_batch c where r.rxnid= c.rxnid and c.acsn='" + acsn + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(prdTable);

                cmd.CommandText = "select r.* from rxn_cond r, ccrann.ccr_batch c where r.rxnid= c.rxnid and c.acsn='" + acsn + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(cndTable);

                cmd.CommandText = "select r.* from ccr_comments r, ccrann.ccr_batch c where r.rxnid= c.rxnid and c.acsn='" + acsn + "' order by 1, 2";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(cmtTable);

                cmd.CommandText = "select r.* from ccr_keyphrases r, ccrann.ccr_batch c where r.rxnid= c.rxnid and c.acsn='" + acsn + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(keyTable);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraConn.Close();
                this.Cursor = Cursors.Default;
                return;
            }

            OraConn.Close();
        }





        private void readbibfile(string filename)
        {
            int cnt = 0;
            int jn = 0;

//            button1.Visible = false;

            if (OraConn.State == ConnectionState.Open) OraConn.Close();

            OraConn.ConnectionString = ccrbibConnectionString;

            cmd.Connection = OraConn;
            cmd.CommandText = "ARTLOADPROC";
            cmd.CommandType = CommandType.StoredProcedure;


            try
            {
//                DataRow myDataRow; 
                StreamReader SR = File.OpenText(filename);
                string bibstr ="\n" + SR.ReadToEnd();
                SR.Close();

                string jrnl = "";
                int j = bibstr.IndexOf("\nE");
                int i;
                do{
                    i = bibstr.IndexOf("\nE", j+1);

                    string sart ="";
                    if (i == -1) sart = bibstr.Substring(j+1);
                    else sart = bibstr.Substring(j + 1, i - j);
                    
                    StringReader sr = new StringReader(sart);
                    jrnl = sr.ReadLine().Substring(5, 40).Trim();
                    if (jn < jrnl.Length) jn = jrnl.Length;

                    sr.Close();
//                    MessageBox.Show(jrnl);

                    int p2 = sart.IndexOf("\n#");
                    int p1;
                    do{
                        p1 = sart.IndexOf("\n#", p2+1);
                        string srec = "";
                        if (p1 == -1) srec = sart.Substring(p2 + 1);
                        else srec = sart.Substring(p2 + 1, p1 - p2);

//                        MessageBox.Show(srec);

//                        myDataRow = jrnTable.NewRow();

//                        myDataRow["JRNL"] = jrnl;

                        sr = new StringReader(srec);

                        string s2db = "";
                        string saut = "";

                        string absnum = "";
                        string artnum = "";
                        string dline = "";
                        string author = "";
                        string address = "";
                        string vol = "";
                        string no = "";
                        string pgst = "";
                        string pgnd = "";
                        string year = "";
                        string lang = "";
                        string title = "";
                        string litext = "";
                        string abstr = "";
                        string sabstr = "";
                        string ut = "";
                        string acsn = "";
                        int aidx = 1;

                        while (sr.Peek() > -1)
                        {
                            dline = sr.ReadLine();
//                            string absnum = dline;

                            if (dline.Substring(0, 1) == "#") { absnum = dline.Substring(5).Trim(); /*myDataRow["ABSNUM"] = absnum; */}

                            else if (dline.Substring(0, 1) == "U") { ut = dline.Substring(5).Trim(); /*myDataRow["UT"] = ut; */}

                            else if (dline.Substring(0, 1) == "C") { acsn = dline.Substring(5).Trim(); /*myDataRow["ACSN"] = acsn;*/ }

                            else if (dline.Substring(0, 1) == "D") { artnum = dline.Substring(5).Trim(); /*myDataRow["ARTL"] = artnum; */}

                            else if (dline.Substring(0, 1) == "T") title = title + dline.Substring(5);

                            else if (dline.Substring(0, 1) == "A")
                            {
                                author = author + dline.Substring(5).Trim() + ", ";
                                
                                saut = saut + "\tAUTHOR\t" + ut + "\t" + aidx + "\t" + dline.Substring(5).Trim() + "\t\t\t\t\t\t\t\r\n";

//                                DataRow myDataRow2 = autTable.NewRow();
//                                myDataRow2["UT"] = ut;
//                                myDataRow2["AUTHOR_POSITION"] = aidx++;
//                                myDataRow2["AUTHOR"] = dline.Substring(5).Trim();
//                                autTable.Rows.Add(myDataRow2);

                            }
                            else if (dline.Substring(0, 1) == "H") address = address + dline.Substring(5);

                            else if (dline.Substring(0, 1) == "G")
                            {
                                vol = dline.Substring(5).Trim();
                                litext = author + vol;

                                vol = vol.Substring(jrnl.Length).Trim();
                                int x1 = vol.IndexOf("(");
                                no = vol.Substring(x1 + 1);
                                vol = vol.Substring(0, x1);

                                x1 = no.IndexOf(")");
                                pgst = no.Substring(x1 + 2);
                                no = no.Substring(0, x1);

                                x1 = pgst.IndexOf("(");
                                year = pgst.Substring(x1 + 1);
                                pgst = pgst.Substring(0, x1);

                                x1 = pgst.IndexOf("-");
                                pgnd = pgst.Substring(x1 + 1);
                                pgst = pgst.Substring(0, x1);


                                x1 = year.IndexOf("IN ");
                                if (x1 > -1) { lang = year.Substring(x1 + 3); lang = lang.Substring(0, lang.Length - 1); }

                                x1 = year.IndexOf(")");
                                year = year.Substring(0, x1);

                            }
                            else if (dline.Substring(0, 1) == "B") abstr = abstr + dline.Substring(5);
//                            else MessageBox.Show("|" + absnum + "|");
                        }
                        sr.Close();

/*
                        myDataRow["NAME"] = author.Substring(0, author.Length - 2);
                        myDataRow["ADDRESS"] = address.Trim();
                        myDataRow["JRNL"] = jrnl;
                        myDataRow["VOL"] = vol;
                        myDataRow["NO"] = no;
                        myDataRow["PAGE.START"] = pgst;
                        myDataRow["PAGE.END"] = pgnd;
                        myDataRow["YEAR"] = year;
                        myDataRow["LANGUAGE"] = lang;
                        myDataRow["TITLE"] = title.Trim();
                        myDataRow["LITTEXT"] = litext;
                        myDataRow["ABST"] = abstr.Trim();
*/
                        string litref = "$DTYPE RXN:VARIATION(1):LITREF(1):";
                        string rdf = "";

                        rdf = litref + "AUTHOR:NAME\r\n" + rdformat(author.Substring(0, author.Length-2));
                        if(address.Length>0) 
                            rdf = rdf + litref + "AUTHOR:ADDRESS\r\n" + rdformat(address.Trim());
                        rdf = rdf + litref + "JOURNAL:JRNL\r\n" + rdformat(jrnl);
                        if (vol.Length > 0) 
                            rdf = rdf + litref + "JOURNAL:VOL.\r\n" + rdformat(vol);
                        if (no.Length > 0) 
                            rdf = rdf + litref + "JOURNAL:NO.\r\n" + rdformat(no);
                        rdf = rdf + litref + "JOURNAL:PAGE.START\r\n" + rdformat(pgst);
                        rdf = rdf + litref + "JOURNAL:PAGE.END\r\n" + rdformat(pgnd);
                        rdf = rdf + litref + "JOURNAL:YEAR\r\n" + rdformat(year);
                        if (lang.Length > 0) 
                            rdf = rdf + litref + "JOURNAL:LANGUAGE\r\n" + rdformat(lang);
                        rdf = rdf + litref + "JOURNAL:TITLE\r\n" + rdformat(title.Trim());
                        rdf = rdf + litref + "TGA.NO\r\n" + rdformat(acsn);

                        rdf = rdf + "$DTYPE RXN:VARIATION(1):LITTEXT\r\n" + rdformat(litext);

//                        myDataRow["RDF"] = rdf;

                        string abscut = abstr.Trim();
                        sabstr = "";
                        int nvar = 1;
                        while (abscut.Length > 0) 
                        {
                            int ic = cut(abscut, 776);

                            sabstr = sabstr + "$DTYPE RXN:VARIATION(1):ABSTRACT(" + nvar + ")\r\n" + rdformat(abscut.Substring(0, ic));
                            abscut = abscut.Substring(ic);

                            nvar += 1;
                        } 

//                        myDataRow["ABSTRACT"] = sabstr;

//                        jrnTable.Rows.Add(myDataRow);

                        s2db = "\tARTICLE\t" + ut + "\t" + jrnl + "\tJ\t" + lang + "\t" + year + "\t" + vol + "\t" + no + "\t" + pgst + "\t" + pgnd + "\t" + acsn + "\t" + title.Trim() + "\t" + absnum + "\t" + abstr.Trim() + "\t" + artnum + "\t" + address.Trim() + "\t" + author.Substring(0, author.Length - 2) + "\t" + sabstr + "\t" + rdf + "\t\t\t\t\t\t\t\r\n";
                        s2db = s2db + saut;
                        s2db = s2db + "\t\t\t\t\t\t\t\r\n";

                        if ((cnt++) % 10 == 0)
                        {
                            label4.Text = bibpath + "    " + (cnt-1);
                            Refresh();
                            Application.DoEvents();
                        }
                        

                        if(OraConn.State == ConnectionState.Closed)OraConn.Open();

                        cmd.Parameters.Add(new OracleParameter("sart", OracleDbType.Clob)).Value = s2db;


                        try
                        {
                            cmd.ExecuteNonQuery();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.ToString());
                            cmd.Parameters.Clear();
                            OraConn.Close();
                        }
                        OraConn.Close();
                        cmd.Parameters.Clear();

                        p2 = p1;
                    } while (p1 > -1);

                    j = i;

                }while( i>-1);
            }  
            catch (IOException exp)
            {
                MessageBox.Show(exp.ToString());
            }
//            MessageBox.Show("" + jn);

//            dataGridView1.DataSource = autTable;
            button1.Visible = true;

        }



        private int cut(string inline, int len)
        {
            if (inline.Length < len + 1) return inline.Length;
            string inseg = inline.Substring(0, len);

            int incut = inseg.LastIndexOf(". ");
            if (incut < 1) incut = inseg.LastIndexOf("; ");
            if (incut < 1) incut = inseg.LastIndexOf(", ");
            if (incut < 1) incut = inseg.LastIndexOf(" ");

            return incut+1;
        }

        private string rdformat(string inline)
        {
            string outline = "";
            if (inline.Length > 0)
            {
                inline = "$DATUM " + inline;
                int i = 0;
                for (i = 0; i < ((inline.Length - 1) / 80); i++)
                {
                    outline = outline + inline.Substring(i * 80, 80) + "+\r\n";
                }
                outline = outline + inline.Substring(i * 80) + "\r\n";
            }
            return outline;
        }


        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            textBox1.Text = "|" + dataGridView1.CurrentCell.FormattedValue.ToString() + "|";

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (OraConn.State == ConnectionState.Open) OraConn.Close();
            OraConn.ConnectionString = ccrbibConnectionString;

            try
            {
                OraConn.Open();
                cmd.Connection = OraConn;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraConn.Close();
                this.Cursor = Cursors.Default;
                return;
            }
            cmd.CommandType = CommandType.Text;


            for (int i = 0; i < jrnTable.Rows.Count; i++)
            {
                cmd.CommandText = "select count(ARTICLE_KEY) from CCR_ARTICLE where ARTICLE_KEY='" + jrnTable.Rows[i]["UT"].ToString() + "'";
                OracleDataReader dr = cmd.ExecuteReader();
                
                if (dr.Read())
                {
                    int cnt = (int)dr.GetDecimal(0);
                    dr.Close();

                    if (cnt == 0)
                    {
                        string langcd = "";
                        if (jrnTable.Rows[i]["LANGUAGE"].ToString().Length > 0)
                        {
                            cmd.CommandText = "select LANGUAGE_CODE from CCR_LANGUAGE where LANGUAGE_TEXT='" + jrnTable.Rows[i]["LANGUAGE"].ToString() + "'";
                            dr = cmd.ExecuteReader();

                            if (dr.Read()) langcd = dr.GetString(0);
                            dr.Close();
                        }

                        cmd.Parameters.Add(new OracleParameter("tit", OracleDbType.Clob)).Value = jrnTable.Rows[i]["TITLE"].ToString();
                        cmd.Parameters.Add(new OracleParameter("aut", OracleDbType.Clob)).Value = jrnTable.Rows[i]["NAME"].ToString();
                        cmd.Parameters.Add(new OracleParameter("adr", OracleDbType.Clob)).Value = jrnTable.Rows[i]["ADDRESS"].ToString();
                        cmd.Parameters.Add(new OracleParameter("abs", OracleDbType.Clob)).Value = jrnTable.Rows[i]["ABST"].ToString();
                        cmd.Parameters.Add(new OracleParameter("ab2", OracleDbType.Clob)).Value = jrnTable.Rows[i]["ABSTRACT"].ToString();
                        cmd.Parameters.Add(new OracleParameter("rdf", OracleDbType.Clob)).Value = jrnTable.Rows[i]["RDF"].ToString();

                        int artn = Convert.ToInt16(jrnTable.Rows[i]["ARTL"].ToString());
                        
                        cmd.CommandText = "insert into CCR_ARTICLE (ARTICLE_KEY, JOURNAL_ABBREV, ARTICLE_TYPE, LANGUAGE_CODE, PUBYEAR, VOLUME, ISSUE, START_PAGE, END_PAGE, TGANO, TITLE, ABSNUM, ARTNO, author, address, ABSTRACT, abst, rdf) ";
                        cmd.CommandText = cmd.CommandText + "values('" + jrnTable.Rows[i]["UT"].ToString() + "', '" + jrnTable.Rows[i]["JRNL"].ToString() + "', 'J', '" + langcd + "', " + jrnTable.Rows[i]["YEAR"].ToString() + ", '" + jrnTable.Rows[i]["VOL"].ToString() + "', '" + jrnTable.Rows[i]["NO"].ToString() + "', '" + jrnTable.Rows[i]["PAGE.START"].ToString() + "', '" + jrnTable.Rows[i]["PAGE.END"].ToString() + "', '" + jrnTable.Rows[i]["ACSN"].ToString() + "', :1, '" + jrnTable.Rows[i]["ABSNUM"].ToString() + "', " + artn + ",:2, :3, :4, :5, :6)";

                        try
                        {
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

                        if (jrnTable.Rows[i]["ADDRESS"].ToString().Length > 0)
                        {
                            cmd.Parameters.Add(new OracleParameter("addr", OracleDbType.Clob)).Value = jrnTable.Rows[i]["ADDRESS"].ToString();
                            cmd.CommandText = "insert into CCR_ADDRESS (ARTICLE_KEY, ADDRESS_POSITION, ADDRESS) values('" + jrnTable.Rows[i]["UT"].ToString() + "', 1, :1)";
                            try
                            {
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
                        }

                        aubs.Filter = "ut='" + jrnTable.Rows[i]["UT"].ToString() + "'";
                        aubs.MoveFirst();
                        for (int j = 0; j < aubs.Count; j++)
                        {
                            DataRowView dtr = (DataRowView)aubs.Current;
                            cmd.Parameters.Add(new OracleParameter("auth", OracleDbType.Clob)).Value = dtr.Row["AUTHOR"].ToString();
                            cmd.CommandText = "insert into CCR_AUTHOR (ARTICLE_KEY, AUTHOR_POSITION, AUTHOR_NAME) values('" + jrnTable.Rows[i]["UT"].ToString() + "'," + dtr.Row["AUTHOR_POSITION"].ToString() + ", :1)";
                            try
                            {
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
                            aubs.MoveNext();
                        }

                    }
                }
                
            }

            OraConn.Close();

        }

        private void button3_Click(object sender, EventArgs e)
        {

            TextWriter tw = new StreamWriter("\\\\34.212.24.69\\Applications\\IC\\date.rdf");
            string outdata = "$RDFILE 1\r\n$DATM    " + DateTime.Now.Date.ToString("MM/dd/yy HH:MM") + "\r\n";
            tw.Write(outdata);

            Cursor = Cursors.WaitCursor;
            for (int n = 1; n </*jrnTable.Rows.Count / */2; n++)
            {
                tw.Write(writeart(n));
            }
            tw.Close();
            Cursor = Cursors.Default;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label4.Text = "";
            dataGridView1.Focus();

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            

            string abs1 = mTextBox1.Text.Trim();
            string abs2 = mTextBox2.Text.Trim();
            string abs3 = mTextBox3.Text.Trim();
            string abs4 = mTextBox4.Text.Trim();
            string abs5 = mTextBox5.Text.Trim();
            string abs6 = mTextBox6.Text.Trim();
            string abs7 = mTextBox7.Text.Trim();
            string abs8 = mTextBox8.Text.Trim();


            string babs = "";

            if (abs5.Length < 6 || abs6.Length < 6) {abs5 = ""; abs6 = ""; }
            else babs = abs5;

            if (abs3.Length < 6 || abs4.Length < 6) {abs3 = ""; abs4 = ""; }
            else babs = abs3;

            if (abs1.Length < 6 || abs2.Length < 6) { abs1 = ""; abs2 = ""; }
            else babs = abs1;

            string iabs = "";
            if (abs7.Length < 6 || abs8.Length < 6) { abs7 = ""; abs8 = ""; }
            else iabs = abs7;

            if (OraConn.State == ConnectionState.Open) OraConn.Close();
            OraConn.ConnectionString = ccrpdnConnectionString;

            this.Cursor = Cursors.WaitCursor;
            try
            {
                OraConn.Open();
                cmd.Connection = OraConn;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("babr", OracleDbType.Varchar2).Value = abs1;
                cmd.Parameters.Add("eabr", OracleDbType.Varchar2).Value = abs2;
                cmd.Parameters.Add("babp", OracleDbType.Varchar2).Value = abs3;
                cmd.Parameters.Add("eabp", OracleDbType.Varchar2).Value = abs4; 
                cmd.Parameters.Add("babs", OracleDbType.Varchar2).Value = abs5;
                cmd.Parameters.Add("eabs", OracleDbType.Varchar2).Value = abs6;
                cmd.CommandText = "reaccsmol";

                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraConn.Close();
                return;
            }
            
            OraConn.Close();
            this.Cursor = Cursors.WaitCursor;
            label4.Text = "CCR Molecule";
            Refresh();

            saveFileDialog1.InitialDirectory = "\\\\34.212.24.69\\mdl\\rdfile\\";
            saveFileDialog1.Filter = "RDFile|*.rdf";


            saveFileDialog1.FileName = saveFileDialog1.InitialDirectory + "ccr" + babs + "mol.rdf";
            
            System.IO.FileStream fs = (System.IO.FileStream)saveFileDialog1.OpenFile();

            string outdata = "";

            outdata = "$RDFILE 1\r\n$DATM    " + DateTime.Now.Date.ToString("MM/dd/yy HH:MM") + "\r\n";

            string ccrmoldata = writemol();
            int idx = ccrmoldata.IndexOf("\r\nnodatanodata\r\n");

            byte[] info = new UTF8Encoding(true).GetBytes(outdata + ccrmoldata.Substring(idx+16));
            fs.Write(info, 0, info.Length);
            fs.Close();

            saveFileDialog1.FileName = saveFileDialog1.InitialDirectory + "ccr" + babs + "mol_nostruct.rdf";
            fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
            info = new UTF8Encoding(true).GetBytes(outdata + ccrmoldata.Substring(0, idx));
            fs.Write(info, 0, info.Length);
            fs.Close();



            label4.Text = "CCR Regular";
            acsnTable.Rows.Clear();
            acsnTable.Dispose();
            dataGridView1.DataSource = acsnTable;
            try
            {
                OraConn.Open();
                cmd.Connection = OraConn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select TGANO acsn, min(absnum) absmin from ccrbib.ccr_article where absnum between '" + abs1 + "' and '" + abs2 + "' group by tgano order by 2";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(acsnTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraConn.Close();
                return;
            }
            OraConn.Close();
            Refresh();

            saveFileDialog1.FileName = saveFileDialog1.InitialDirectory + "ccr" + babs + "rxn.rdf";
            fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
            outdata = "$RDFILE 1\r\n$DATM    " + DateTime.Now.Date.ToString("MM/dd/yy HH:MM") + "\r\n";
            info = new UTF8Encoding(true).GetBytes(outdata);
            fs.Write(info, 0, info.Length);


            Cursor = Cursors.WaitCursor;
            dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
            for (int n = 0; n <acsnTable.Rows.Count; n++)
            {

                info = new UTF8Encoding(true).GetBytes(writeart(n));
                fs.Write(info, 0, info.Length);
                

                dataGridView1.CurrentCell = dataGridView1.Rows[n+1].Cells[0];
                Application.DoEvents();
                Refresh();
            }


            label4.Text = "CCR Patent";
            acsnTable.Rows.Clear();
            acsnTable.Dispose();
            try
            {
                OraConn.Open();
                cmd.Connection = OraConn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select PAT_NUMBER acsn, min(absnum) absmin from ccrbib.ccr_article where absnum between '" + abs3 + "' and '" + abs4 + "' group by PAT_NUMBER order by 2";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(acsnTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraConn.Close();
                return;
            }
            OraConn.Close();
            Refresh();

            Cursor = Cursors.WaitCursor;
            dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
            for (int n = 0; n < acsnTable.Rows.Count; n++)
            {
                info = new UTF8Encoding(true).GetBytes(writepat(n));
                fs.Write(info, 0, info.Length);
                dataGridView1.CurrentCell = dataGridView1.Rows[n + 1].Cells[0];
                Application.DoEvents();
                Refresh();
            }

            label4.Text = "CCR Supplemental";
            acsnTable.Rows.Clear();
            acsnTable.Dispose();
            dataGridView1.DataSource = acsnTable;
            try
            {
                OraConn.Open();
                cmd.Connection = OraConn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select TGANO acsn, min(absnum) absmin from ccrbib.ccr_article where absnum between '" + abs5 + "' and '" + abs6 + "' group by tgano order by 2";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(acsnTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraConn.Close();
                return;
            }
            OraConn.Close();
            Refresh();

            Cursor = Cursors.WaitCursor;
            dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
            for (int n = 0; n < acsnTable.Rows.Count; n++)
            {
                info = new UTF8Encoding(true).GetBytes(writeart(n));
                fs.Write(info, 0, info.Length);
                dataGridView1.CurrentCell = dataGridView1.Rows[n + 1].Cells[0];
                Application.DoEvents();
                Refresh();
            }


            fs.Close();

            if (iabs.Length == 6)
            {
                if (OraConn.State == ConnectionState.Open) OraConn.Close();
                OraConn.ConnectionString = icannConnectionString;

                label4.Text = "IC";
                acsnTable.Rows.Clear();
                acsnTable.Dispose();
                dataGridView1.DataSource = acsnTable;
                try
                {
                    OraConn.Open();
                    cmd.Connection = OraConn;
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "select min(acsn), min(artlnum), count(*) i9, absnum from ic_moltable where absnum between '" + abs7 + "' and '" + abs8 + "' group by absnum order by absnum";
                    cpdAdapter.SelectCommand = cmd;
                    cpdAdapter.Fill(acsnTable);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    OraConn.Close();
                    return;
                }
                OraConn.Close();
                Refresh();


                saveFileDialog1.FileName = saveFileDialog1.InitialDirectory + "ic" + iabs + "mol.rdf";
                fs = (System.IO.FileStream)saveFileDialog1.OpenFile();
                outdata = "$RDFILE 1\r\n$DATM    " + DateTime.Now.Date.ToString("MM/dd/yy HH:MM") + "\r\n";
                info = new UTF8Encoding(true).GetBytes(outdata);
                fs.Write(info, 0, info.Length);


                Cursor = Cursors.WaitCursor;
//                dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
                mireg = 1;

                for (int n = 0; n < acsnTable.Rows.Count; n++)
                {
                    string hao = writeic(n);
                    info = new UTF8Encoding(true).GetBytes(hao);
                    fs.Write(info, 0, info.Length);


//                    dataGridView1.CurrentCell = dataGridView1.Rows[n + 1].Cells[0];
                    Application.DoEvents();
                    Refresh();
                }

                fs.Close();


            }

            Cursor = Cursors.Default;

            label4.Text = "Done";

        }



        private string writeic(int n)
        {
            molTable.Rows.Clear();
            symTable.Rows.Clear();
            bioTable.Rows.Clear();
            molTable.Dispose();
            symTable.Dispose();
            bioTable.Dispose();

            string absnum = acsnTable.Rows[n]["ABSNUM"].ToString();
            string i9 = acsnTable.Rows[n]["i9"].ToString();

            label4.Text = "IC: " + absnum;

            queryic(absnum);

            string outdata = "";



            for (int i = 0; i < molTable.Rows.Count; i++)
            {
                string ctbl = "";
                StringReader sr = new StringReader(molTable.Rows[i]["ml"].ToString());
                while (sr.Peek() > 0)
                {
                    string dline = sr.ReadLine();
                    ctbl = ctbl + dline + "\r\n";
                }


                outdata = outdata + "$MFMT $MIREG " + (mireg++) + "\r\n" + ctbl + "$DTYPE MOL:CPD.NO\r\n$DATUM " + molTable.Rows[i]["cpdnumber"].ToString() + "\r\n"; ;
                if(molTable.Rows[i]["grade"].ToString().Trim().Length>0)
                    outdata = outdata + "$DTYPE MOL:GRADE\r\n$DATUM " + molTable.Rows[i]["grade"].ToString() + "\r\n";
                outdata = outdata + "$DTYPE MOL:TGA.NO\r\n$DATUM " + molTable.Rows[i]["acsn"].ToString() + "\r\n$DTYPE MOL:ARTL.NO\r\n$DATUM " + molTable.Rows[i]["artlnum"].ToString() + "\r\n";
                outdata = outdata + "$DTYPE MOL:AUTH.NO\r\n$DATUM " + molTable.Rows[i]["authnum"].ToString() + "\r\n$DTYPE MOL:EXTREG\r\n$DATUM " + molTable.Rows[i]["absnum"].ToString() + "\r\n";


                DataView dv1 = new DataView(bioTable, "cpdnumber='" + molTable.Rows[i]["cpdnumber"].ToString() + "'", "", DataViewRowState.CurrentRows);
                for (int j =1; j<dv1.Count+1; j++)
                {
                    outdata = outdata + "$DTYPE MOL:BIOLACT(" + j + ")\r\n" + rdformat(dv1[j-1].Row["biol_activity"].ToString());
                    outdata = outdata + "$DTYPE MOL:STATUS(" + j + ")\r\n$DATUM " + dv1[j - 1].Row["status"].ToString() + "\r\n";
                }

                dv1 = new DataView(symTable, "cpdnumber='" + molTable.Rows[i]["cpdnumber"].ToString() + "'", "", DataViewRowState.CurrentRows);
                for (int j = 1; j < dv1.Count + 1; j++)
                {
                    outdata = outdata + "$DTYPE MOL:SYMBOL(" + j + ")\r\n" + rdformat(dv1[j - 1].Row["symbol"].ToString());
                }

                outdata = outdata + "$DTYPE MOL:TOTAL.NO\r\n$DATUM " + i9 + "\r\n";

                


            }



            return outdata;

        }



        private void queryic(string absnum)
        {
            try
            {
                if (OraConn.State == ConnectionState.Open) OraConn.Close();
                OraConn.ConnectionString = icannConnectionString;
                OraConn.Open();
                cmd.Connection = OraConn;
                cmd.CommandType = CommandType.Text;

                cmd.CommandText = "select cpdnumber, grade, ACSN, artlnum, authnum, ABSNUM, molfile(ctab) ml from ic_moltable where absnum='" + absnum + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(molTable);


                cmd.CommandText = "select m.cpdnumber, s.symbol from ic_symbol s, ic_moltable m where m.cpdnumber = s.cpdnumber and absnum='" + absnum + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(symTable);

                cmd.CommandText = "select m.cpdnumber, b.status, a.biol_activity from ccrpdn.ba_main a, ic_biolact b, ic_moltable m where a.ba_code = b.ba_code and m.cpdnumber = b.cpdnumber and absnum='" + absnum + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(bioTable);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraConn.Close();
                this.Cursor = Cursors.Default;
                return;
            }

            OraConn.Close();
        }






        private void button5_Click(object sender, EventArgs e)
        {

            string abs1 = mTextBox3.Text.Trim();
            string abs2 = mTextBox4.Text.Trim();

            acsnTable.Rows.Clear();
            acsnTable.Dispose();

            artTable.Rows.Clear();
            artTable.Dispose();

            refTable.Rows.Clear();
            refTable.Dispose();

            bioTable.Rows.Clear();
            bioTable.Dispose();
            symTable.Rows.Clear();
            symTable.Dispose();

            if (OraConn.State == ConnectionState.Open) OraConn.Close();
            OraConn.ConnectionString = ccrannConnectionString;


            try
            {
                OraConn.Open();
                cmd.Connection = OraConn;
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "select * from article where CCR_REGISTRY_NUMBER between '" + abs1 + "' and '" + abs2 + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(acsnTable);

                dataGridView1.DataSource = acsnTable;

                MessageBox.Show(cmd.CommandText + "  " + acsnTable.Rows.Count);

                cmd.CommandText = "select u.* from article a, author u where u.article_key = a.article_key and CCR_REGISTRY_NUMBER between '" + abs1 + "' and '" + abs2 + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(symTable);


                cmd.CommandText = "select u.* from article a, address u where u.article_key = a.article_key and CCR_REGISTRY_NUMBER between '" + abs1 + "' and '" + abs2 + "' order by 1";
                cpdAdapter.SelectCommand = cmd;
                cpdAdapter.Fill(bioTable);

//                dataGridView1.DataSource = bioTable;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraConn.Close();
                return;
            }
            OraConn.Close();

            if (OraConn.State == ConnectionState.Open) OraConn.Close();
            OraConn.ConnectionString = ccrbibConnectionString;

            OraConn.Open();

            for (int i = 0; i < acsnTable.Rows.Count; i++)
            {

                cmd.CommandText = "select ARTICLE_KEY from ccr_article where ARTICLE_KEY='" + acsnTable.Rows[i]["ARTICLE_KEY"].ToString() + "'";

                OracleDataReader dr = cmd.ExecuteReader();
                if (!dr.Read())
                {
                    smbs.DataSource = symTable;
                    smbs.Filter = "ARTICLE_KEY='" + acsnTable.Rows[i]["ARTICLE_KEY"].ToString() + "'";
                    smbs.Sort = "AUTHOR_POSITION";

                    string authorline = "";
                    smbs.MoveFirst();
                    DataRowView drv;
                    for (int j = 0; j < smbs.Count; j++)
                    {
                        drv = (DataRowView)smbs.Current;
                        authorline = authorline + ", " + drv.Row["AUTHOR_NAME"].ToString();
                        smbs.MoveNext();
                    }
                    if(authorline.Length>2) authorline = authorline.Substring(2);

                    bibs.DataSource = bioTable;
                    bibs.Filter = "ARTICLE_KEY='" + acsnTable.Rows[i]["ARTICLE_KEY"].ToString() + "'";
                    bibs.Sort = "ADDRESS_POSITION";

                    string addressline = "";
                    bibs.MoveFirst();
                    drv = (DataRowView)bibs.Current;
                    addressline = drv.Row["INSTITUTION"].ToString();
                    cmd.Parameters.Add("addr", OracleDbType.Varchar2).Value = addressline;
                    cmd.Parameters.Add("auth", OracleDbType.Varchar2).Value = authorline;
                    cmd.Parameters.Add("title", OracleDbType.Varchar2).Value = acsnTable.Rows[i]["TITLE"].ToString();

                    cmd.CommandText = "insert into ccr_article (ADDRESS, AUTHOR, ARTICLE_KEY, ARTICLE_TYPE, TITLE, ABSNUM, PAT_COUNTRY_CODE, PAT_NUMBER, PAT_PRIOR_LATEST_DATE, PAT_PRIOR_LATEST_CC, PAT_PRIOR_LATEST_NUM, PAT_PRIOR_EARLIEST_DATE, PAT_PRIOR_EARLIEST_CC, PAT_PRIOR_EARLIEST_NUM, PAT_KIND, PAT_PUB_DATE) values";
                    cmd.CommandText = cmd.CommandText + "(:1, :2, '" + acsnTable.Rows[i]["ARTICLE_KEY"].ToString() + "', '" + acsnTable.Rows[i]["ARTICLE_TYPE"].ToString() + "', :3, '" + acsnTable.Rows[i]["CCR_REGISTRY_NUMBER"].ToString() + "', '";
                    cmd.CommandText = cmd.CommandText + acsnTable.Rows[i]["PAT_COUNTRY_CODE"].ToString() + "', '" + acsnTable.Rows[i]["PAT_NUMBER"].ToString() + "', to_date('" + acsnTable.Rows[i]["PAT_PRIOR_LATEST_DATE"].ToString() + "', 'mm/dd/YYYY'), '" + acsnTable.Rows[i]["PAT_PRIOR_LATEST_CC"].ToString() + "', '" + acsnTable.Rows[i]["PAT_PRIOR_LATEST_NUM"].ToString();
                    cmd.CommandText = cmd.CommandText + "', to_date('" + acsnTable.Rows[i]["PAT_PRIOR_EARLIEST_DATE"].ToString() + "', 'mm/dd/YYYY'), '" + acsnTable.Rows[i]["PAT_PRIOR_EARLIEST_CC"].ToString() + "', '" + acsnTable.Rows[i]["PAT_PRIOR_EARLIEST_NUM"].ToString() + "', '" + acsnTable.Rows[i]["PAT_KIND"].ToString() + "', to_date('" + acsnTable.Rows[i]["PAT_PUB_DATE"].ToString() + "', 'mm/dd/YYYY') )";

                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
                dr.Close();


            }

            catTable.Rows.Clear();
            catTable.Dispose();

            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "select * from ccr_article where ARTICLE_TYPE = '+'";
            cpdAdapter.SelectCommand = cmd;
            cpdAdapter.Fill(catTable);

            dataGridView1.DataSource = catTable;

            OraConn.Close();

        }



        private void mTextBox3_Click(object sender, EventArgs e)
        {
            mTextBox3.SelectionStart = 1;
        }

        private void mTextBox3_Enter(object sender, EventArgs e)
        {
            mTextBox3.SelectionStart = 1;
        }

        private void mTextBox4_Enter(object sender, EventArgs e)
        {
            mTextBox4.SelectionStart = 1;
        }

        private void mTextBox4_Click(object sender, EventArgs e)
        {
            mTextBox4.SelectionStart = 1;
        }

        private void mTextBox5_Enter(object sender, EventArgs e)
        {
            mTextBox5.SelectionStart = 1;
        }

        private void mTextBox5_Click(object sender, EventArgs e)
        {
            mTextBox5.SelectionStart = 1;
        }

        private void mTextBox6_Enter(object sender, EventArgs e)
        {
            mTextBox6.SelectionStart = 1;
        }

        private void mTextBox6_Click(object sender, EventArgs e)
        {
            mTextBox6.SelectionStart = 1;
        }

        

        private void panel1_VisibleChanged(object sender, EventArgs e)
        {
            if (panel1.Visible == true)
            {

                listBox1.Items.Clear();

                OracleCommand comm = new OracleCommand();

                if (OraConn.State == ConnectionState.Open) OraConn.Close();
                OraConn.ConnectionString = ccrannConnectionString;

                comm.Connection = OraConn;
                comm.CommandType = CommandType.Text;
                comm.CommandText = "select unique batchno from icann.ic_moltable union select unique batchno from ccr_batch where batchno between 200000 and 210000 order by 1 desc";

                try
                {
                    OraConn.Open();
                    OracleDataReader dr = comm.ExecuteReader();

                    while (dr.Read())
                    {
                        listBox1.Items.Add(dr.GetDecimal(0));
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

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            OracleCommand comm = new OracleCommand();
            if (OraConn.State == ConnectionState.Open) OraConn.Close();

            OraConn.ConnectionString = ccrannConnectionString;

            comm.Connection = OraConn;
            comm.CommandType = CommandType.Text;

            try
            {
                comm.CommandText = "select nvl(min(absnum), ' '), nvl(max(absnum), ' ') from ccrnumber p, ccr_batch b where p.rxnid = b.rxnid and p.absnum < 'P00000' and b.batchno between '" + listBox1.SelectedItems[listBox1.SelectedItems.Count - 1].ToString() + "' and '" + listBox1.SelectedItems[0].ToString() + "'";
//                comm.CommandText = "select nvl(min(absnum), ' '), nvl(max(absnum), ' ') from ccrbib.ccr_article where absnum < 'P00000' and tgano in (select unique acsn from ccr_batch where batchno between " + listBox1.SelectedItems[listBox1.SelectedItems.Count - 1].ToString() + " and " + listBox1.SelectedItems[0].ToString() + ")";
                OraConn.Open();
                OracleDataReader dr = comm.ExecuteReader();

                if (dr.Read())
                {
                    mTextBox1.Text = dr.GetString(0);
                    mTextBox2.Text = dr.GetString(1);
                }
                dr.Close();

                comm.CommandText = "select nvl(min(absnum), 'P'), nvl(max(absnum), 'P') from ccrnumber p, ccr_batch b where p.rxnid = b.rxnid and p.absnum like 'P%' and b.batchno between '" + listBox1.SelectedItems[listBox1.SelectedItems.Count - 1].ToString() + "' and '" + listBox1.SelectedItems[0].ToString() + "'";
                dr = comm.ExecuteReader();

                if (dr.Read())
                {
                    mTextBox3.Text = dr.GetString(0);
                    mTextBox4.Text = dr.GetString(1);
                }
                dr.Close();

                comm.CommandText = "select nvl(min(absnum), 'S'), nvl(max(absnum), 'S') from ccrnumber p, ccr_batch b where p.rxnid = b.rxnid and p.absnum like 'S%' and b.batchno between '" + listBox1.SelectedItems[listBox1.SelectedItems.Count - 1].ToString() + "' and '" + listBox1.SelectedItems[0].ToString() + "'";
                dr = comm.ExecuteReader();

                if (dr.Read())
                {
                    mTextBox5.Text = dr.GetString(0);
                    mTextBox6.Text = dr.GetString(1);
                }
                dr.Close();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                OraConn.Close();
            }

            OraConn.Close();


            OraConn.ConnectionString = icannConnectionString;

            OraConn.Open();
            try
            {
                comm.CommandText = "select nvl(min(absnum), ' '), nvl(max(absnum), ' ') from ic_moltable where batchno between " + listBox1.SelectedItems[listBox1.SelectedItems.Count - 1].ToString() + " and " + listBox1.SelectedItems[0].ToString();
                OracleDataReader dr = comm.ExecuteReader();
                if (dr.Read())
                {
                    mTextBox7.Text = dr.GetString(0).ToString();
                    mTextBox8.Text = dr.GetString(1).ToString();
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

                connectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + host + ")(PORT=" + port + ")))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" + service + ")));";
                settings.Save();
            }

        }




        


    }
}