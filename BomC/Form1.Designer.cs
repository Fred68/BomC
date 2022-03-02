namespace BomC
	{
	partial class Form1
		{
		/// <summary>
		/// Variabile di progettazione necessaria.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Pulire le risorse in uso.
		/// </summary>
		/// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
		protected override void Dispose(bool disposing)
			{
			if (disposing && (components != null))
				{
				components.Dispose();
				}
			base.Dispose(disposing);
			}

		#region Codice generato da Progettazione Windows Form

		/// <summary>
		/// Metodo necessario per il supporto della finestra di progettazione. Non modificare
		/// il contenuto del metodo con l'editor di codice.
		/// </summary>
		private void InitializeComponent()
			{
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.btFile1 = new System.Windows.Forms.Button();
			this.btFile2 = new System.Windows.Forms.Button();
			this.ofD = new System.Windows.Forms.OpenFileDialog();
			this.btRead = new System.Windows.Forms.Button();
			this.btFile3 = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.sfD = new System.Windows.Forms.SaveFileDialog();
			this.btTest = new System.Windows.Forms.Button();
			this.btReset = new System.Windows.Forms.Button();
			this.btCfr = new System.Windows.Forms.Button();
			this.tbDb0 = new System.Windows.Forms.TextBox();
			this.tbDb1 = new System.Windows.Forms.TextBox();
			this.btReport = new System.Windows.Forms.Button();
			this.btConfrontaDistinte = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(5, 51);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(543, 20);
			this.textBox1.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(8, 35);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(35, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "label1";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(8, 121);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(35, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "label2";
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(5, 137);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(543, 20);
			this.textBox2.TabIndex = 2;
			// 
			// btFile1
			// 
			this.btFile1.Location = new System.Drawing.Point(554, 51);
			this.btFile1.Name = "btFile1";
			this.btFile1.Size = new System.Drawing.Size(105, 36);
			this.btFile1.TabIndex = 4;
			this.btFile1.Text = "button1";
			this.btFile1.UseVisualStyleBackColor = true;
			this.btFile1.Click += new System.EventHandler(this.btFile1_Click);
			// 
			// btFile2
			// 
			this.btFile2.Location = new System.Drawing.Point(554, 137);
			this.btFile2.Name = "btFile2";
			this.btFile2.Size = new System.Drawing.Size(105, 36);
			this.btFile2.TabIndex = 5;
			this.btFile2.Text = "button2";
			this.btFile2.UseVisualStyleBackColor = true;
			this.btFile2.Click += new System.EventHandler(this.btFile2_Click);
			// 
			// ofD
			// 
			this.ofD.FileName = "openFileDialog1";
			// 
			// btRead
			// 
			this.btRead.Location = new System.Drawing.Point(443, 279);
			this.btRead.Name = "btRead";
			this.btRead.Size = new System.Drawing.Size(105, 36);
			this.btRead.TabIndex = 6;
			this.btRead.Text = "...leggi...";
			this.btRead.UseVisualStyleBackColor = true;
			this.btRead.Click += new System.EventHandler(this.btRead_Click);
			// 
			// btFile3
			// 
			this.btFile3.Location = new System.Drawing.Point(554, 217);
			this.btFile3.Name = "btFile3";
			this.btFile3.Size = new System.Drawing.Size(105, 36);
			this.btFile3.TabIndex = 9;
			this.btFile3.Text = "button3";
			this.btFile3.UseVisualStyleBackColor = true;
			this.btFile3.Click += new System.EventHandler(this.btFile3_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(8, 201);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(35, 13);
			this.label3.TabIndex = 8;
			this.label3.Text = "label3";
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point(5, 217);
			this.textBox3.Name = "textBox3";
			this.textBox3.Size = new System.Drawing.Size(543, 20);
			this.textBox3.TabIndex = 7;
			// 
			// btTest
			// 
			this.btTest.Location = new System.Drawing.Point(443, 250);
			this.btTest.Name = "btTest";
			this.btTest.Size = new System.Drawing.Size(75, 23);
			this.btTest.TabIndex = 10;
			this.btTest.Text = "Test";
			this.btTest.UseVisualStyleBackColor = true;
			this.btTest.Click += new System.EventHandler(this.btTest_Click);
			// 
			// btReset
			// 
			this.btReset.Location = new System.Drawing.Point(190, 250);
			this.btReset.Name = "btReset";
			this.btReset.Size = new System.Drawing.Size(105, 36);
			this.btReset.TabIndex = 11;
			this.btReset.Text = "...reset...";
			this.btReset.UseVisualStyleBackColor = true;
			this.btReset.Click += new System.EventHandler(this.btReset_Click);
			// 
			// btCfr
			// 
			this.btCfr.Location = new System.Drawing.Point(443, 321);
			this.btCfr.Name = "btCfr";
			this.btCfr.Size = new System.Drawing.Size(105, 36);
			this.btCfr.TabIndex = 12;
			this.btCfr.Text = "...cfr...";
			this.btCfr.UseVisualStyleBackColor = true;
			this.btCfr.Click += new System.EventHandler(this.btCfr_Click);
			// 
			// tbDb0
			// 
			this.tbDb0.Location = new System.Drawing.Point(5, 77);
			this.tbDb0.Name = "tbDb0";
			this.tbDb0.Size = new System.Drawing.Size(218, 20);
			this.tbDb0.TabIndex = 13;
			// 
			// tbDb1
			// 
			this.tbDb1.Location = new System.Drawing.Point(5, 163);
			this.tbDb1.Name = "tbDb1";
			this.tbDb1.Size = new System.Drawing.Size(218, 20);
			this.tbDb1.TabIndex = 14;
			// 
			// btReport
			// 
			this.btReport.Location = new System.Drawing.Point(554, 279);
			this.btReport.Name = "btReport";
			this.btReport.Size = new System.Drawing.Size(105, 36);
			this.btReport.TabIndex = 15;
			this.btReport.Text = "...Report...";
			this.btReport.UseVisualStyleBackColor = true;
			this.btReport.Click += new System.EventHandler(this.btReport_Click);
			// 
			// btConfrontaDistinte
			// 
			this.btConfrontaDistinte.Location = new System.Drawing.Point(5, 250);
			this.btConfrontaDistinte.Name = "btConfrontaDistinte";
			this.btConfrontaDistinte.Size = new System.Drawing.Size(170, 65);
			this.btConfrontaDistinte.TabIndex = 16;
			this.btConfrontaDistinte.Text = "...cfr distinte...";
			this.btConfrontaDistinte.UseVisualStyleBackColor = true;
			this.btConfrontaDistinte.Click += new System.EventHandler(this.btConfrontaDistinte_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(840, 488);
			this.Controls.Add(this.btConfrontaDistinte);
			this.Controls.Add(this.btReport);
			this.Controls.Add(this.tbDb1);
			this.Controls.Add(this.tbDb0);
			this.Controls.Add(this.btCfr);
			this.Controls.Add(this.btReset);
			this.Controls.Add(this.btTest);
			this.Controls.Add(this.btFile3);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBox3);
			this.Controls.Add(this.btRead);
			this.Controls.Add(this.btFile2);
			this.Controls.Add(this.btFile1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBox1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.Form1_HelpButtonClicked);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.Load += new System.EventHandler(this.Form1_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Form1_Paint);
			this.ResumeLayout(false);
			this.PerformLayout();

			}

		#endregion

		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Button btFile1;
		private System.Windows.Forms.Button btFile2;
		private System.Windows.Forms.OpenFileDialog ofD;
		private System.Windows.Forms.Button btRead;
		private System.Windows.Forms.Button btFile3;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.SaveFileDialog sfD;
		private System.Windows.Forms.Button btTest;
		private System.Windows.Forms.Button btReset;
		private System.Windows.Forms.Button btCfr;
		private System.Windows.Forms.TextBox tbDb0;
		private System.Windows.Forms.TextBox tbDb1;
		private System.Windows.Forms.Button btReport;
		private System.Windows.Forms.Button btConfrontaDistinte;
		}
	}

