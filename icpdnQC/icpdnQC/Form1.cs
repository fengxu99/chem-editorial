using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using icpdnQC.Properties;

namespace icpdnQC
{
    public partial class Form1 : Form
    {

        static private string icpdnConnectionString = "Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=localhost)(PORT=1550)))(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=chemdb.prod-wos.com)));User Id=icpdn;Password=icpdn;";

        static private OracleCommand comm1 = new OracleCommand();
        static private OracleConnection oraconn = new OracleConnection();
        static private DataSet cpdDataSet = new DataSet();
        static private BindingSource bs = new BindingSource();
        static private DataView dv;
        static private string bats = "";
        static private OracleDataAdapter cpdAdapter = new OracleDataAdapter();
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

            panel2.Visible = false;


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

        }

        protected void MMEditClick(object who, EventArgs e)
        {
            panel2.Visible = false;
        }

        protected void MMConfigClick(object who, EventArgs e)
        {
            panel2.Visible = true;
        }

        protected void MMExitClick(object who, EventArgs e)
        {
            Application.Exit();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            listBox1.Items.Clear();
            bats = "";
            bs.DataSource = null;
            renditor1.MolfileString = "";
            Refresh();

            cpdDataSet.Clear();
            cpdDataSet.Tables.Clear();
            qcqry();

            dv = new DataView(cpdDataSet.Tables["cpd"]);

            for (int i = 0; i < cpdDataSet.Tables["bat"].Rows.Count; i++)
                listBox1.Items.Add(cpdDataSet.Tables["bat"].Rows[i]["batchno"].ToString());


            bs.DataSource = dv;
            dataGridView1.DataSource = bs;
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[1].Visible = false;
            dataGridView1.Columns[2].Visible = false;
            dataGridView1.Columns[3].Visible = false;
            dataGridView1.Columns[4].Visible = false;
            dataGridView1.Columns["strtype"].Visible = false;
            dataGridView1.Columns["strtype"].ReadOnly = true;
            dataGridView1.Columns["qryname"].HeaderText = "Query Name";
            dataGridView1.Columns["qryname"].Width = 130;
            dataGridView1.Columns["qryname"].ReadOnly = true;
            dataGridView1.Columns["batchno"].ReadOnly = true;
            dataGridView1.Columns["iceditor"].ReadOnly = true;

            dataGridView1.Columns["regno"].Visible = false;
            dataGridView1.Columns["ml"].Visible = false;

            if(dataGridView1.Columns.Contains("chk")) dataGridView1.Columns.Remove("chk");
            DataGridViewCheckBoxColumn myCheck = new DataGridViewCheckBoxColumn();
            myCheck.Name = "Checked";
            myCheck.HeaderText = "Checked";
            myCheck.DataPropertyName = "chk";
            myCheck.Width = 55;
            myCheck.FalseValue = "0";
            myCheck.TrueValue = "1";
            dataGridView1.AutoGenerateColumns = false;
            if (!dataGridView1.Columns.Contains("Checked")) dataGridView1.Columns.Insert(3, myCheck);

            if (dataGridView1.Columns.Contains("class")) dataGridView1.Columns.Remove("class");
            myCheck = new DataGridViewCheckBoxColumn();
            myCheck.Name = "Classes";
            myCheck.HeaderText = "Classes";
            myCheck.DataPropertyName = "class";
            myCheck.Width = 55;
            myCheck.FalseValue = "0";
            myCheck.TrueValue = "1";
            dataGridView1.AutoGenerateColumns = false;
            if (!dataGridView1.Columns.Contains("Classes")) dataGridView1.Columns.Insert(8, myCheck);

            listBox1.SelectedIndex = -1;
            if (listBox1.Items.Count > 0) listBox1.SelectedIndex = 0;


            Cursor = Cursors.Default;
        }

        private void qcqry()
        {

            if (cpdDataSet.Tables.Contains("cpd")) cpdDataSet.Tables["cpd"].Rows.Clear();

            if (oraconn.State == ConnectionState.Closed) oraconn.Open();

            comm1.CommandType = CommandType.Text;

            comm1.CommandText = "select q.acsn || lpad(to_char(q.artlnum), 5, ' ') || lpad(q.authnum, 5, ' ') compound, q.*, w.iceditor from simpleqry q, work w where q.artlnum = w.art (+) and q.acsn = w.acsn (+)";
            cpdAdapter.SelectCommand = comm1;
            cpdAdapter.Fill(cpdDataSet, "cpd");

            comm1.CommandText = "select q.acsn || lpad(to_char(q.artlnum), 5, ' ') || lpad(q.authnum, 5, ' ') compound, q.*, w.iceditor from complexqry q, work w where q.artlnum = w.art (+) and q.acsn = w.acsn (+)";
            cpdAdapter.SelectCommand = comm1;
            cpdAdapter.Fill(cpdDataSet, "cpd");


            comm1.CommandText = "select m.acsn || lpad(to_char(m.artlnum), 5, ' ') || lpad(m.authnum, 5, ' ') compound, q.*, m.batchno, m.acsn, m.artlnum, m.authnum, molfile(m.ctab) ml, w.iceditor from LABELQRY q, ic_moltable m, work w where m.regno = q.regno and m.artlnum = w.art (+) and m.acsn = w.acsn (+)";
            cpdAdapter.Fill(cpdDataSet, "cpd");


            comm1.CommandText = "select unique m.batchno, m.acsn, m.artlnum, a.status, a.ba_code from ic_biolact a, ic_moltable m where a.regno = m.regno";
            cpdAdapter.Fill(cpdDataSet, "bio");

            comm1.CommandText = "select * from subject_index where prod_code = 'IC'";
            cpdAdapter.Fill(cpdDataSet, "lbl");

            comm1.CommandText = "select unique batchno from ic_moltable order by 1 desc";
            cpdAdapter.Fill(cpdDataSet, "bat");


            oraconn.Close();
        }

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell == null) return;
            renditor1.MolfileString = dataGridView1.CurrentRow.Cells["ml"].Value.ToString();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
//            MessageBox.Show("list " + listBox1.SelectedIndex );
            if(listBox1.SelectedIndex < 0) return;
            bats = "";

            for (int i = 0; i < listBox1.SelectedItems.Count; i++)
            {
                bats = bats + ", " + listBox1.SelectedItems[i].ToString();
            }

            bs.Filter = "batchno in ( " + bats.Substring(2) + ")";

            comboBox2.Items.Clear();
            DataView dv1 = new DataView(cpdDataSet.Tables["cpd"]);

            dv1.RowFilter = "batchno in ( " + bats.Substring(2) + ") and strtype = 'simple'";
            if (dv1.Count > 0) comboBox2.Items.Add("simple");

            dv1.RowFilter = "batchno in ( " + bats.Substring(2) + ") and strtype = 'complex'";
            if (dv1.Count > 0) comboBox2.Items.Add("complex");

            dv1.RowFilter = "batchno in ( " + bats.Substring(2) + ") and strtype = 'Organometallic'";
            if (dv1.Count > 0) comboBox2.Items.Add("Organometallic");

            dv1.RowFilter = "batchno in ( " + bats.Substring(2) + ") and strtype = 'Labeled compound'";
            if (dv1.Count > 0) comboBox2.Items.Add("Labeled compound");

            comboBox2.SelectedIndex = -1;
            if (comboBox2.Items.Count > 0) comboBox2.SelectedIndex = 0;
            else dv.RowFilter = "batchno in ( " + bats.Substring(2) + ")";

            comboBox3.Items.Clear();
            dv1.Sort = "iceditor";
            dv1.RowFilter = "batchno in ( " + bats.Substring(2) + ")";

            for (int i = 0; i < dv1.Count; i++)
                if (!comboBox3.Items.Contains(dv1[i].Row["iceditor"].ToString()) && dv1[i].Row["iceditor"].ToString().Length>0 ) comboBox3.Items.Add(dv1[i].Row["iceditor"].ToString());

            dv1 = new DataView(cpdDataSet.Tables["bio"]);
            dv1.RowFilter = "batchno in ( " + bats.Substring(2) + ")";
            label1.Text = "Biolactivity Count: " + dv1.Count;

            dv1 = new DataView(cpdDataSet.Tables["lbl"]);
            dv1.RowFilter = "batchno in ( " + bats.Substring(2) + ")";
            label2.Text = "Labled CPD Count: " + dv1.Count;


        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
//            MessageBox.Show("combo2 " + comboBox2.SelectedIndex );
            if (comboBox2.SelectedIndex < 0) return;

            if (bats.Length > 2)
            {
                bs.Filter = "batchno in ( " + bats.Substring(2) + ") and strtype='" + comboBox2.Text + "'";

                dv.Sort = "qryname, compound";
                comboBox3.SelectedIndex = -1;

                if(dataGridView1.Columns.Contains("strtype")) dataGridView1.Columns["strtype"].Visible = false;
                if (dataGridView1.Columns.Contains("iceditor")) dataGridView1.Columns["iceditor"].Visible = true;

                if (comboBox2.Text == "Organometallic" || comboBox2.Text == "Labeled compound" )
                {
                    if (dataGridView1.Columns.Contains("Classes")) dataGridView1.Columns["Classes"].Visible = true;
                }
                else
                {
                    if (dataGridView1.Columns.Contains("Classes")) dataGridView1.Columns["Classes"].Visible = false;
                }

                if (dataGridView1.Columns.Contains("Checked")) dataGridView1.Columns["Checked"].Visible = true;
                if (dataGridView1.Columns.Contains("batchno")) dataGridView1.Columns["batchno"].Visible = false;
            }
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex < 0) { return; }

            if (bats.Length > 2) bs.Filter = "batchno in ( " + bats.Substring(2) + ") and iceditor='" + comboBox3.Text + "'";

            dv.Sort = "strtype, qryname, compound";

            comboBox2.SelectedIndex = -1;

            dataGridView1.Columns["strtype"].Visible = true;
            dataGridView1.Columns["batchno"].Visible = true;
            dataGridView1.Columns["iceditor"].Visible = false;
            dataGridView1.Columns["Checked"].Visible = false;
            dataGridView1.Columns["Classes"].Visible = false;

        }

        private void renditor1_EditorReturned(object sender, MDL.Draw.Renditor.EditorReturnedEventArgs e)
        {
            dataGridView1.CurrentRow.Cells["ml"].Value = renditor1.MolfileString;

        }

        private void button2_Click(object sender, EventArgs e)
        {
            savechanges();
        }


        private void savechanges()
        {
            string sdb = "";

            DataView dv1 = new DataView(cpdDataSet.Tables["cpd"], "", "", DataViewRowState.CurrentRows);
            for (int i = 0; i < dv1.Count; i++)
            {
                if (dv1[i].Row["ml"].ToString() != dv1[i].Row["ml", DataRowVersion.Original].ToString())
                    sdb = sdb + "\tMOLECULE\tUPDATE\t" + dv1[i].Row["REGNO"].ToString() + "\t" + dv1[i].Row["ml"].ToString() + "\t\t\t\t\t\r\n";

                if (dv1[i].Row["class"].ToString() != dv1[i].Row["class", DataRowVersion.Original].ToString())
                {
                    int classid = 0;
                    if (dv1[i].Row["strtype"].ToString() == "Organometallic") classid = 30;
                    else classid = 19;

                    if (dv1[i].Row["class"].ToString() == "1") 
                        sdb = sdb + "\tCPDCLASS\tINSERT\t" + dv1[i].Row["REGNO"].ToString() + "\t" + classid + "\t\t\t\t\t\r\n";
                    else if (dv1[i].Row["class"].ToString() == "0")
                        sdb = sdb + "\tCPDCLASS\tDELETE\t" + dv1[i].Row["REGNO"].ToString() + "\t" + classid + "\t\t\t\t\t\r\n";
                }
            
            }


            try
            {
                if (oraconn.State != ConnectionState.Open) oraconn.Open();
                comm1.Connection = oraconn;

                comm1.Parameters.Add("clobtodb", OracleDbType.Clob).Value = sdb + "\r\n";
                comm1.CommandType = CommandType.StoredProcedure;
                comm1.CommandText = "qcsaveproc";

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
            }
        }



    }
}
