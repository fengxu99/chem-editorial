namespace ic_editorial
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            MDL.Draw.Renderer.Preferences.DisplayPreferences displayPreferences3 = new MDL.Draw.Renderer.Preferences.DisplayPreferences();
            MDL.Draw.Renderer.Preferences.DisplayPreferences displayPreferences1 = new MDL.Draw.Renderer.Preferences.DisplayPreferences();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label1 = new System.Windows.Forms.Label();
            this.comboChoice = new System.Windows.Forms.ComboBox();
            this.dataGridCpd = new System.Windows.Forms.DataGridView();
            this.comart = new System.Windows.Forms.ComboBox();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.comacsn = new System.Windows.Forms.ComboBox();
            this.combatch = new System.Windows.Forms.ComboBox();
            this.textaut = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button4 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textbat = new System.Windows.Forms.TextBox();
            this.textacsn = new System.Windows.Forms.TextBox();
            this.textart = new System.Windows.Forms.TextBox();
            this.button5 = new System.Windows.Forms.Button();
            this.renditor1 = new MDL.Draw.Renditor.Renditor();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.markLeadCPDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printDoc = new System.Drawing.Printing.PrintDocument();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button6 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.renditor2 = new MDL.Draw.Renditor.Renditor();
            this.dataGridView0 = new System.Windows.Forms.DataGridView();
            this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.addSubjectListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel4 = new System.Windows.Forms.Panel();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.comboLst = new System.Windows.Forms.ComboBox();
            this.combosta = new System.Windows.Forms.ComboBox();
            this.button2 = new System.Windows.Forms.Button();
            this.dataGridLst = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.button7 = new System.Windows.Forms.Button();
            this.panel5 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridCpd)).BeginInit();
            this.contextMenuStrip2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView0)).BeginInit();
            this.contextMenuStrip3.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridLst)).BeginInit();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(313, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 20);
            this.label1.TabIndex = 33;
            this.label1.Text = "label1";
            // 
            // comboChoice
            // 
            this.comboChoice.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.comboChoice.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboChoice.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboChoice.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboChoice.FormattingEnabled = true;
            this.comboChoice.Items.AddRange(new object[] {
            "Default View",
            "Grade",
            "Biolact",
            "CPD_Class",
            "Use_Profile"});
            this.comboChoice.Location = new System.Drawing.Point(76, 54);
            this.comboChoice.Name = "comboChoice";
            this.comboChoice.Size = new System.Drawing.Size(132, 28);
            this.comboChoice.TabIndex = 32;
            this.comboChoice.SelectedIndexChanged += new System.EventHandler(this.comboChoice_SelectedIndexChanged);
            // 
            // dataGridCpd
            // 
            this.dataGridCpd.AllowUserToAddRows = false;
            this.dataGridCpd.AllowUserToResizeColumns = false;
            this.dataGridCpd.AllowUserToResizeRows = false;
            this.dataGridCpd.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridCpd.ColumnHeadersVisible = false;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridCpd.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridCpd.Location = new System.Drawing.Point(4, 88);
            this.dataGridCpd.MultiSelect = false;
            this.dataGridCpd.Name = "dataGridCpd";
            this.dataGridCpd.RowHeadersVisible = false;
            this.dataGridCpd.Size = new System.Drawing.Size(66, 584);
            this.dataGridCpd.TabIndex = 31;
            this.dataGridCpd.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dataGridCpd_CellMouseDown);
            this.dataGridCpd.CellContentDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridCpd_CellContentDoubleClick);
            this.dataGridCpd.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridCpd_CellEndEdit);
            this.dataGridCpd.CurrentCellChanged += new System.EventHandler(this.dataGridCpd_CurrentCellChanged);
            // 
            // comart
            // 
            this.comart.ContextMenuStrip = this.contextMenuStrip2;
            this.comart.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comart.FormattingEnabled = true;
            this.comart.Location = new System.Drawing.Point(229, 13);
            this.comart.MaxDropDownItems = 20;
            this.comart.Name = "comart";
            this.comart.Size = new System.Drawing.Size(52, 28);
            this.comart.TabIndex = 30;
            this.comart.SelectedIndexChanged += new System.EventHandler(this.comart_SelectedIndexChanged);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.contextMenuStrip2.Name = "contextMenuStrip1";
            this.contextMenuStrip2.Size = new System.Drawing.Size(143, 26);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(142, 22);
            this.toolStripMenuItem1.Text = "Print Deleted";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // comacsn
            // 
            this.comacsn.ContextMenuStrip = this.contextMenuStrip2;
            this.comacsn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comacsn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comacsn.FormattingEnabled = true;
            this.comacsn.Location = new System.Drawing.Point(128, 13);
            this.comacsn.MaxDropDownItems = 20;
            this.comacsn.Name = "comacsn";
            this.comacsn.Size = new System.Drawing.Size(86, 28);
            this.comacsn.TabIndex = 29;
            this.comacsn.SelectedIndexChanged += new System.EventHandler(this.comacsn_SelectedIndexChanged);
            // 
            // combatch
            // 
            this.combatch.ContextMenuStrip = this.contextMenuStrip2;
            this.combatch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combatch.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.combatch.FormattingEnabled = true;
            this.combatch.Location = new System.Drawing.Point(34, 13);
            this.combatch.Name = "combatch";
            this.combatch.Size = new System.Drawing.Size(85, 28);
            this.combatch.TabIndex = 28;
            this.combatch.SelectedIndexChanged += new System.EventHandler(this.combatch_SelectedIndexChanged);
            // 
            // textaut
            // 
            this.textaut.Font = new System.Drawing.Font("Franklin Gothic Book", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textaut.Location = new System.Drawing.Point(179, 2);
            this.textaut.MaxLength = 3;
            this.textaut.Name = "textaut";
            this.textaut.Size = new System.Drawing.Size(48, 26);
            this.textaut.TabIndex = 17;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.button5);
            this.panel1.Controls.Add(this.renditor1);
            this.panel1.Location = new System.Drawing.Point(458, 49);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(553, 634);
            this.panel1.TabIndex = 34;
            // 
            // button4
            // 
            this.button4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button4.Location = new System.Drawing.Point(30, 557);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(154, 35);
            this.button4.TabIndex = 13;
            this.button4.Text = "New Compound";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textbat);
            this.panel2.Controls.Add(this.textaut);
            this.panel2.Controls.Add(this.textacsn);
            this.panel2.Controls.Add(this.textart);
            this.panel2.Location = new System.Drawing.Point(180, 561);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(235, 37);
            this.panel2.TabIndex = 18;
            this.panel2.Visible = false;
            // 
            // textbat
            // 
            this.textbat.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textbat.Location = new System.Drawing.Point(6, 2);
            this.textbat.MaxLength = 6;
            this.textbat.Name = "textbat";
            this.textbat.Size = new System.Drawing.Size(65, 26);
            this.textbat.TabIndex = 14;
            // 
            // textacsn
            // 
            this.textacsn.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textacsn.Location = new System.Drawing.Point(72, 2);
            this.textacsn.MaxLength = 5;
            this.textacsn.Name = "textacsn";
            this.textacsn.Size = new System.Drawing.Size(68, 26);
            this.textacsn.TabIndex = 15;
            // 
            // textart
            // 
            this.textart.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textart.Location = new System.Drawing.Point(142, 2);
            this.textart.MaxLength = 3;
            this.textart.Name = "textart";
            this.textart.Size = new System.Drawing.Size(34, 26);
            this.textart.TabIndex = 16;
            // 
            // button5
            // 
            this.button5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button5.Location = new System.Drawing.Point(415, 557);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(135, 35);
            this.button5.TabIndex = 19;
            this.button5.Text = "Save Compound";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // renditor1
            // 
            this.renditor1.AutoSizeStructure = true;
            this.renditor1.ChimeString = null;
            this.renditor1.ClearingEnabled = true;
            this.renditor1.CopyingEnabled = true;
            this.renditor1.DisplayOnEmpty = null;
            this.renditor1.EditingEnabled = true;
            this.renditor1.FileName = null;
            this.renditor1.HighlightInfo = null;
            this.renditor1.IsBitmapFromOLE = false;
            this.renditor1.Location = new System.Drawing.Point(40, 37);
            this.renditor1.Molecule = null;
            this.renditor1.MolfileString = null;
            this.renditor1.Name = "renditor1";
            this.renditor1.OldScalingMode = MDL.Draw.Renderer.Preferences.StructureScalingMode.ScaleToFitBox;
            this.renditor1.PastingEnabled = true;
            displayPreferences3.AtomAtomDisplayMode = MDL.Draw.Renderer.Preferences.AtomAtomMappingDisplayMode.On;
            displayPreferences3.ColorAtomsByType = false;
            this.renditor1.Preferences = displayPreferences3;
            this.renditor1.PreferencesFileName = "default.xml";
            this.renditor1.RendererBorderStyle = System.Windows.Forms.ButtonBorderStyle.Inset;
            this.renditor1.RenditorMolecule = null;
            this.renditor1.RenditorName = "Renditor";
            this.renditor1.Size = new System.Drawing.Size(506, 514);
            this.renditor1.SketchString = null;
            this.renditor1.SmilesString = "";
            this.renditor1.TabIndex = 20;
            this.renditor1.URLEncodedMolfileString = null;
            this.renditor1.UseLocalXMLConfig = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,
            this.editToolStripMenuItem,
            this.markLeadCPDToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(156, 70);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.deleteToolStripMenuItem.Text = "Edit";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.editToolStripMenuItem.Text = "Delete";
            this.editToolStripMenuItem.Click += new System.EventHandler(this.editToolStripMenuItem_Click);
            // 
            // markLeadCPDToolStripMenuItem
            // 
            this.markLeadCPDToolStripMenuItem.Name = "markLeadCPDToolStripMenuItem";
            this.markLeadCPDToolStripMenuItem.Size = new System.Drawing.Size(155, 22);
            this.markLeadCPDToolStripMenuItem.Text = "Mark Lead CPD";
            this.markLeadCPDToolStripMenuItem.Click += new System.EventHandler(this.markLeadCPDToolStripMenuItem_Click);
            // 
            // printDoc
            // 
            this.printDoc.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDoc_PrintPage);
            this.printDoc.BeginPrint += new System.Drawing.Printing.PrintEventHandler(this.printDoc_BeginPrint);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(484, 47);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(234, 434);
            this.textBox1.TabIndex = 37;
            this.textBox1.Visible = false;
            this.textBox1.Click += new System.EventHandler(this.textBox1_Click);
            // 
            // button6
            // 
            this.button6.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button6.Location = new System.Drawing.Point(305, 44);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(83, 33);
            this.button6.TabIndex = 15;
            this.button6.Text = "Save";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(262, 120);
            this.dataGridView1.TabIndex = 14;
            this.dataGridView1.CurrentCellChanged += new System.EventHandler(this.dataGridView1_CurrentCellChanged);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.panel3.Controls.Add(this.renditor2);
            this.panel3.Controls.Add(this.button6);
            this.panel3.Controls.Add(this.dataGridView1);
            this.panel3.Location = new System.Drawing.Point(71, 48);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(421, 548);
            this.panel3.TabIndex = 38;
            this.panel3.Visible = false;
            // 
            // renditor2
            // 
            this.renditor2.AutoSizeStructure = true;
            this.renditor2.ChimeString = null;
            this.renditor2.ClearingEnabled = true;
            this.renditor2.CopyingEnabled = true;
            this.renditor2.DisplayOnEmpty = null;
            this.renditor2.EditingEnabled = true;
            this.renditor2.FileName = null;
            this.renditor2.HighlightInfo = null;
            this.renditor2.IsBitmapFromOLE = false;
            this.renditor2.Location = new System.Drawing.Point(3, 130);
            this.renditor2.Molecule = null;
            this.renditor2.MolfileString = null;
            this.renditor2.Name = "renditor2";
            this.renditor2.OldScalingMode = MDL.Draw.Renderer.Preferences.StructureScalingMode.ScaleToFitBox;
            this.renditor2.PastingEnabled = true;
            displayPreferences1.AtomAtomDisplayMode = MDL.Draw.Renderer.Preferences.AtomAtomMappingDisplayMode.On;
            displayPreferences1.ColorAtomsByType = false;
            this.renditor2.Preferences = displayPreferences1;
            this.renditor2.PreferencesFileName = "default.xml";
            this.renditor2.RendererBorderStyle = System.Windows.Forms.ButtonBorderStyle.Inset;
            this.renditor2.RenditorMolecule = null;
            this.renditor2.RenditorName = "Renditor";
            this.renditor2.Size = new System.Drawing.Size(416, 375);
            this.renditor2.SketchString = null;
            this.renditor2.SmilesString = "";
            this.renditor2.TabIndex = 0;
            this.renditor2.URLEncodedMolfileString = null;
            this.renditor2.UseLocalXMLConfig = false;
            // 
            // dataGridView0
            // 
            this.dataGridView0.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView0.ColumnHeadersVisible = false;
            this.dataGridView0.ContextMenuStrip = this.contextMenuStrip3;
            this.dataGridView0.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnKeystroke;
            this.dataGridView0.Location = new System.Drawing.Point(71, 89);
            this.dataGridView0.MultiSelect = false;
            this.dataGridView0.Name = "dataGridView0";
            this.dataGridView0.RowHeadersVisible = false;
            this.dataGridView0.Size = new System.Drawing.Size(432, 313);
            this.dataGridView0.TabIndex = 39;
            this.dataGridView0.DoubleClick += new System.EventHandler(this.dataGridView0_DoubleClick);
            this.dataGridView0.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView0_RowsAdded);
            this.dataGridView0.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView0_CellEndEdit);
            this.dataGridView0.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView0_CellEnter);
            // 
            // contextMenuStrip3
            // 
            this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.addSubjectListToolStripMenuItem});
            this.contextMenuStrip3.Name = "contextMenuStrip1";
            this.contextMenuStrip3.Size = new System.Drawing.Size(160, 48);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(159, 22);
            this.toolStripMenuItem2.Text = "Add Symbol";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // addSubjectListToolStripMenuItem
            // 
            this.addSubjectListToolStripMenuItem.Name = "addSubjectListToolStripMenuItem";
            this.addSubjectListToolStripMenuItem.Size = new System.Drawing.Size(159, 22);
            this.addSubjectListToolStripMenuItem.Text = "Add Subject List";
            this.addSubjectListToolStripMenuItem.Click += new System.EventHandler(this.addSubjectListToolStripMenuItem_Click);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.dataGridView2);
            this.panel4.Controls.Add(this.comboLst);
            this.panel4.Controls.Add(this.combosta);
            this.panel4.Controls.Add(this.button2);
            this.panel4.Controls.Add(this.dataGridLst);
            this.panel4.Controls.Add(this.button1);
            this.panel4.Location = new System.Drawing.Point(69, 88);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(439, 598);
            this.panel4.TabIndex = 40;
            this.panel4.Visible = false;
            // 
            // dataGridView2
            // 
            this.dataGridView2.AllowUserToAddRows = false;
            this.dataGridView2.AllowUserToDeleteRows = false;
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.ColumnHeadersVisible = false;
            this.dataGridView2.Location = new System.Drawing.Point(0, 5);
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowHeadersVisible = false;
            this.dataGridView2.Size = new System.Drawing.Size(371, 151);
            this.dataGridView2.TabIndex = 34;
            this.dataGridView2.DoubleClick += new System.EventHandler(this.dataGridView2_DoubleClick);
            this.dataGridView2.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView2_RowsAdded);
            this.dataGridView2.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView2_CellEndEdit);
            this.dataGridView2.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView2_CellEnter);
            // 
            // comboLst
            // 
            this.comboLst.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Append;
            this.comboLst.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.comboLst.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboLst.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboLst.FormattingEnabled = true;
            this.comboLst.Location = new System.Drawing.Point(36, 217);
            this.comboLst.MaxDropDownItems = 15;
            this.comboLst.Name = "comboLst";
            this.comboLst.Size = new System.Drawing.Size(332, 23);
            this.comboLst.TabIndex = 31;
            this.comboLst.SelectedIndexChanged += new System.EventHandler(this.comboLst_SelectedIndexChanged);
            // 
            // combosta
            // 
            this.combosta.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combosta.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.combosta.FormattingEnabled = true;
            this.combosta.Items.AddRange(new object[] {
            "T",
            "P"});
            this.combosta.Location = new System.Drawing.Point(4, 217);
            this.combosta.Name = "combosta";
            this.combosta.Size = new System.Drawing.Size(33, 23);
            this.combosta.TabIndex = 33;
            this.combosta.SelectedIndexChanged += new System.EventHandler(this.combosta_SelectedIndexChanged);
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(134, 246);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(120, 41);
            this.button2.TabIndex = 32;
            this.button2.Text = "<< REMOVE";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // dataGridLst
            // 
            this.dataGridLst.AllowUserToAddRows = false;
            this.dataGridLst.AllowUserToDeleteRows = false;
            this.dataGridLst.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridLst.ColumnHeadersVisible = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Times New Roman", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridLst.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridLst.Location = new System.Drawing.Point(371, -2);
            this.dataGridLst.Name = "dataGridLst";
            this.dataGridLst.RowHeadersVisible = false;
            this.dataGridLst.Size = new System.Drawing.Size(66, 584);
            this.dataGridLst.TabIndex = 30;
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ImageKey = "(none)";
            this.button1.Location = new System.Drawing.Point(147, 169);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(89, 40);
            this.button1.TabIndex = 29;
            this.button1.Text = "ADD >>";
            this.button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(295, 53);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(119, 28);
            this.button3.TabIndex = 41;
            this.button3.Text = "Print Article";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(571, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 18);
            this.label2.TabIndex = 43;
            this.label2.Text = "label2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(708, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 18);
            this.label3.TabIndex = 44;
            this.label3.Text = "label3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(919, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 20);
            this.label4.TabIndex = 45;
            this.label4.Text = "label4";
            this.label4.Visible = false;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(237, 119);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(150, 20);
            this.textBox2.TabIndex = 46;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(237, 180);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(150, 20);
            this.textBox3.TabIndex = 47;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(237, 245);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(150, 20);
            this.textBox4.TabIndex = 48;
            // 
            // button7
            // 
            this.button7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button7.Location = new System.Drawing.Point(274, 322);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(113, 28);
            this.button7.TabIndex = 49;
            this.button7.Text = "Save Setting";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.label8);
            this.panel5.Controls.Add(this.label7);
            this.panel5.Controls.Add(this.label6);
            this.panel5.Controls.Add(this.label5);
            this.panel5.Controls.Add(this.button7);
            this.panel5.Controls.Add(this.textBox2);
            this.panel5.Controls.Add(this.textBox3);
            this.panel5.Controls.Add(this.textBox4);
            this.panel5.Location = new System.Drawing.Point(4, 12);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1032, 687);
            this.panel5.TabIndex = 50;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(121, 119);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 50;
            this.label5.Text = "Host:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(121, 180);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(29, 13);
            this.label6.TabIndex = 51;
            this.label6.Text = "Port:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(121, 245);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(46, 13);
            this.label7.TabIndex = 52;
            this.label7.Text = "Service:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(115, 39);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(178, 20);
            this.label8.TabIndex = 53;
            this.label8.Text = "Database Configuration";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1048, 710);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.dataGridView0);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboChoice);
            this.Controls.Add(this.dataGridCpd);
            this.Controls.Add(this.comart);
            this.Controls.Add(this.comacsn);
            this.Controls.Add(this.combatch);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel3);
            this.Name = "Form1";
            this.Text = "IC Editing";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridCpd)).EndInit();
            this.contextMenuStrip2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView0)).EndInit();
            this.contextMenuStrip3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridLst)).EndInit();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboChoice;
        private System.Windows.Forms.DataGridView dataGridCpd;
        private System.Windows.Forms.ComboBox comart;
        private System.Windows.Forms.ComboBox comacsn;
        private System.Windows.Forms.ComboBox combatch;
        private System.Windows.Forms.TextBox textaut;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textbat;
        private System.Windows.Forms.TextBox textacsn;
        private System.Windows.Forms.TextBox textart;
        private System.Windows.Forms.Button button5;
        private MDL.Draw.Renditor.Renditor renditor1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Drawing.Printing.PrintDocument printDoc;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel3;
        private MDL.Draw.Renditor.Renditor renditor2;
        private System.Windows.Forms.DataGridView dataGridView0;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ComboBox comboLst;
        private System.Windows.Forms.ComboBox combosta;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.DataGridView dataGridLst;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.ToolStripMenuItem addSubjectListToolStripMenuItem;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem markLeadCPDToolStripMenuItem;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
    }
}

