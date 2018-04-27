namespace reaccsLoad
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
            this.button1 = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.button5 = new System.Windows.Forms.Button();
            this.mTextBox1 = new System.Windows.Forms.MaskedTextBox();
            this.mTextBox2 = new System.Windows.Forms.MaskedTextBox();
            this.mTextBox3 = new System.Windows.Forms.MaskedTextBox();
            this.mTextBox4 = new System.Windows.Forms.MaskedTextBox();
            this.mTextBox5 = new System.Windows.Forms.MaskedTextBox();
            this.mTextBox6 = new System.Windows.Forms.MaskedTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.mTextBox8 = new System.Windows.Forms.MaskedTextBox();
            this.mTextBox7 = new System.Windows.Forms.MaskedTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label8 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.button7 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 24);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(180, 45);
            this.button1.TabIndex = 0;
            this.button1.Text = "Open CCR Bibliographic File";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(15, 230);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(260, 270);
            this.dataGridView1.TabIndex = 1;
            this.dataGridView1.Visible = false;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(1, 496);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(500, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.Visible = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(15, 174);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(177, 40);
            this.button2.TabIndex = 3;
            this.button2.Text = "Load Bibliogaphic to Oracle";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(189, 179);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(123, 29);
            this.button4.TabIndex = 5;
            this.button4.Text = "Extract RDFiles";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(141, 53);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Regular";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(146, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 13;
            this.label2.Text = "Patent";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(114, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 14;
            this.label3.Text = "Supplemental";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(113, 179);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(0, 25);
            this.label4.TabIndex = 15;
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(276, 172);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(113, 45);
            this.button5.TabIndex = 16;
            this.button5.Text = "button5";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Visible = false;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // mTextBox1
            // 
            this.mTextBox1.AsciiOnly = true;
            this.mTextBox1.Location = new System.Drawing.Point(189, 50);
            this.mTextBox1.Mask = "000000";
            this.mTextBox1.Name = "mTextBox1";
            this.mTextBox1.PromptChar = ' ';
            this.mTextBox1.Size = new System.Drawing.Size(102, 20);
            this.mTextBox1.TabIndex = 17;
            // 
            // mTextBox2
            // 
            this.mTextBox2.AsciiOnly = true;
            this.mTextBox2.Location = new System.Drawing.Point(309, 48);
            this.mTextBox2.Mask = "000000";
            this.mTextBox2.Name = "mTextBox2";
            this.mTextBox2.PromptChar = ' ';
            this.mTextBox2.Size = new System.Drawing.Size(102, 20);
            this.mTextBox2.TabIndex = 18;
            // 
            // mTextBox3
            // 
            this.mTextBox3.AsciiOnly = true;
            this.mTextBox3.Location = new System.Drawing.Point(189, 73);
            this.mTextBox3.Mask = "000000";
            this.mTextBox3.Name = "mTextBox3";
            this.mTextBox3.PromptChar = ' ';
            this.mTextBox3.Size = new System.Drawing.Size(102, 20);
            this.mTextBox3.TabIndex = 19;
            this.mTextBox3.Enter += new System.EventHandler(this.mTextBox3_Enter);
            this.mTextBox3.Click += new System.EventHandler(this.mTextBox3_Click);
            // 
            // mTextBox4
            // 
            this.mTextBox4.AsciiOnly = true;
            this.mTextBox4.Location = new System.Drawing.Point(309, 73);
            this.mTextBox4.Mask = "00000";
            this.mTextBox4.Name = "mTextBox4";
            this.mTextBox4.PromptChar = ' ';
            this.mTextBox4.Size = new System.Drawing.Size(102, 20);
            this.mTextBox4.TabIndex = 20;
            this.mTextBox4.ValidatingType = typeof(int);
            this.mTextBox4.Enter += new System.EventHandler(this.mTextBox4_Enter);
            this.mTextBox4.Click += new System.EventHandler(this.mTextBox4_Click);
            // 
            // mTextBox5
            // 
            this.mTextBox5.AsciiOnly = true;
            this.mTextBox5.Location = new System.Drawing.Point(189, 99);
            this.mTextBox5.Mask = "000000";
            this.mTextBox5.Name = "mTextBox5";
            this.mTextBox5.PromptChar = ' ';
            this.mTextBox5.Size = new System.Drawing.Size(102, 20);
            this.mTextBox5.TabIndex = 21;
            this.mTextBox5.Enter += new System.EventHandler(this.mTextBox5_Enter);
            this.mTextBox5.Click += new System.EventHandler(this.mTextBox5_Click);
            // 
            // mTextBox6
            // 
            this.mTextBox6.AsciiOnly = true;
            this.mTextBox6.Location = new System.Drawing.Point(309, 99);
            this.mTextBox6.Mask = "000000";
            this.mTextBox6.Name = "mTextBox6";
            this.mTextBox6.PromptChar = ' ';
            this.mTextBox6.Size = new System.Drawing.Size(102, 20);
            this.mTextBox6.TabIndex = 22;
            this.mTextBox6.Enter += new System.EventHandler(this.mTextBox6_Enter);
            this.mTextBox6.Click += new System.EventHandler(this.mTextBox6_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(195, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 13);
            this.label5.TabIndex = 23;
            this.label5.Text = "Starting Article";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(313, 16);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 13);
            this.label6.TabIndex = 24;
            this.label6.Text = "Ending Article";
            // 
            // mTextBox8
            // 
            this.mTextBox8.AsciiOnly = true;
            this.mTextBox8.Location = new System.Drawing.Point(309, 125);
            this.mTextBox8.Mask = "000000";
            this.mTextBox8.Name = "mTextBox8";
            this.mTextBox8.PromptChar = ' ';
            this.mTextBox8.Size = new System.Drawing.Size(102, 20);
            this.mTextBox8.TabIndex = 27;
            // 
            // mTextBox7
            // 
            this.mTextBox7.AsciiOnly = true;
            this.mTextBox7.Location = new System.Drawing.Point(189, 125);
            this.mTextBox7.Mask = "000000";
            this.mTextBox7.Name = "mTextBox7";
            this.mTextBox7.PromptChar = ' ';
            this.mTextBox7.Size = new System.Drawing.Size(102, 20);
            this.mTextBox7.TabIndex = 26;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(164, 132);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 13);
            this.label7.TabIndex = 25;
            this.label7.Text = "IC";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.listBox1);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.mTextBox6);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.mTextBox8);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.mTextBox7);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.mTextBox2);
            this.panel1.Controls.Add(this.mTextBox4);
            this.panel1.Controls.Add(this.mTextBox3);
            this.panel1.Controls.Add(this.mTextBox5);
            this.panel1.Controls.Add(this.mTextBox1);
            this.panel1.Location = new System.Drawing.Point(10, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(424, 463);
            this.panel1.TabIndex = 28;
            this.panel1.VisibleChanged += new System.EventHandler(this.panel1_VisibleChanged);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(18, 45);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.listBox1.Size = new System.Drawing.Size(71, 394);
            this.listBox1.TabIndex = 30;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(20, 18);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(68, 13);
            this.label8.TabIndex = 29;
            this.label8.Text = "Batch Select";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.label12);
            this.panel2.Controls.Add(this.button7);
            this.panel2.Controls.Add(this.textBox2);
            this.panel2.Controls.Add(this.textBox3);
            this.panel2.Controls.Add(this.textBox4);
            this.panel2.Location = new System.Drawing.Point(1, -1);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(666, 637);
            this.panel2.TabIndex = 29;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(108, 63);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(178, 20);
            this.label9.TabIndex = 61;
            this.label9.Text = "Database Configuration";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(114, 269);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(46, 13);
            this.label10.TabIndex = 60;
            this.label10.Text = "Service:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(114, 204);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(29, 13);
            this.label11.TabIndex = 59;
            this.label11.Text = "Port:";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(114, 143);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(32, 13);
            this.label12.TabIndex = 58;
            this.label12.Text = "Host:";
            // 
            // button7
            // 
            this.button7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button7.Location = new System.Drawing.Point(267, 346);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(113, 28);
            this.button7.TabIndex = 57;
            this.button7.Text = "Save Setting";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(230, 143);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(150, 20);
            this.textBox2.TabIndex = 54;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(230, 204);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(150, 20);
            this.textBox3.TabIndex = 55;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(230, 269);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(150, 20);
            this.textBox4.TabIndex = 56;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(697, 675);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.MaskedTextBox mTextBox1;
        private System.Windows.Forms.MaskedTextBox mTextBox2;
        private System.Windows.Forms.MaskedTextBox mTextBox3;
        private System.Windows.Forms.MaskedTextBox mTextBox4;
        private System.Windows.Forms.MaskedTextBox mTextBox5;
        private System.Windows.Forms.MaskedTextBox mTextBox6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.MaskedTextBox mTextBox8;
        private System.Windows.Forms.MaskedTextBox mTextBox7;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
    }
}

