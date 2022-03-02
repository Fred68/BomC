using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO;                            // Operazioni su file
using System.Diagnostics;                   // Per Process.Start()
using System.Reflection;                    // Per Assembly()
using System.Xml;                           // Per lettura packages.config

namespace BomC
	{
	public partial class Form1 : Form
		{

		const string SEP = "\\";                                    // Separatore di path
		const string TXTEXT = ".txt";                               // Estensione per file di testo
		const string filterTxt = "Text files|*.txt|" +				// Filtro per Open dialog
												"All files|*.*";
		const string packages = "packages.config";

		Dbc dbu;

		string dbNewFilename, dbOldFilename, resFilename;

		string[] root;		// 0 old, 1 new

		public Form1()
			{
			InitializeComponent();

			dbu = new Dbc();					// Classe gestione distinte base
			root = new string[2];				// 0 old, 1 new
			dbNewFilename = dbOldFilename = resFilename = string.Empty;
			}

		private void Form1_Load(object sender, EventArgs e)
			{
			this.Text = "Confronto distinte base";
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.HelpButton = true;
			label1.Text = "File di distinta da confrontare (nuova distinta)";
			label2.Text = "File di distinta rispetto a cui eseguire il confronto (vecchia distinta)";
			label3.Text = "File dei risultati";
			btFile1.Text = btFile2.Text = btFile3.Text = "File...";
			textBox1.ReadOnly = textBox2.ReadOnly = textBox3.ReadOnly = true;
			btRead.Text = "Importa distinte base";
			btReset.Text = "Azzera distinte e codici";
			tbDb0.Text = tbDb1.Text = String.Empty;
			tbDb0.ReadOnly = tbDb1.ReadOnly = true;
			btConfrontaDistinte.Text = "Confronta distinte";

			// Pulsanti di prova
			btTest.Visible = btRead.Visible = btCfr.Visible = btReport.Visible = false;
			}

		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
			{
			if (MessageBox.Show("Chiusura programma", "Uscire dal programma ?", MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
				e.Cancel = true;
				}
			}
		/// <summary>
		/// Versione del file
		/// e pacchetti utilizzati
		/// </summary>
		/// <returns></returns>
		public static string Version()
			{
			StringBuilder strb = new StringBuilder();
			FileInfo fi = new FileInfo(Assembly.GetExecutingAssembly().Location);
			Module[] mods = Assembly.GetExecutingAssembly().GetModules();

			var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
			strb.AppendLine("Informazioni sul programma" + System.Environment.NewLine);
			strb.AppendLine(versionInfo.ProductName + System.Environment.NewLine);
			strb.AppendLine(versionInfo.Comments + System.Environment.NewLine);
			strb.AppendLine("Autore: " + versionInfo.CompanyName);
			strb.AppendLine("Copyright: " + versionInfo.LegalCopyright);
			strb.AppendLine("Versione: " + versionInfo.ProductVersion);
			//strb.Append(Application.ProductName + System.Environment.NewLine);
			//strb.Append("Copyright " + Application.CompanyName + System.Environment.NewLine);
			//strb.Append("Versione: " + Application.ProductVersion + System.Environment.NewLine);
			strb.AppendLine("Build: " + Build());
			strb.AppendLine("Eseguibile: " + fi.FullName);
			//strb.Append("Moduli: ");
			//foreach (Module m in mods)
			//	{
			//	strb.AppendLine(" " + m.Name);
			//	}
			XmlReader reader = null;
			try
				{
				string packXml = Path.GetDirectoryName(fi.FullName) + SEP + packages;
				if (File.Exists(packXml))
					{
					// Process.Start("notepad.exe", packXml);
					reader = XmlReader.Create(packXml);
					strb.AppendLine("Elenco packages:");
					while (reader.ReadToFollowing("package"))
						{
						if (reader.HasAttributes)
							{
							strb.Append('\t');
							for (int i = 0; i < reader.AttributeCount; i++)
								{
								strb.Append(reader.GetAttribute(i) + " ");
								}
							strb.AppendLine();
							}
						}
					strb.AppendLine("Fine elenco packages.");
					}
				else
					{
					strb.AppendLine("File packages.config non trovato");
					}
				}
			catch (Exception ex)
				{
				strb.AppendLine($"Errore: {ex.Message}");
				}
			finally
				{
				if (reader != null) reader.Close();
				}

			return strb.ToString();
			}

		/// <summary>
		/// Compone una stringa con le informazioni dell'ultima complilazione
		/// </summary>
		/// <returns></returns>
		public static string Build()
			{
			StringBuilder strb = new StringBuilder();
			DateTime dt = new FileInfo(Assembly.GetExecutingAssembly().Location).LastWriteTime;
			strb.Append(dt.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture));
			strb.Append('.');
			strb.Append(dt.ToString("HHmm", System.Globalization.CultureInfo.InvariantCulture));
			return strb.ToString();
			}

		private void Form1_HelpButtonClicked(object sender, CancelEventArgs e)
			{
			MessageBox.Show(Version());
			e.Cancel = true;
			}

		private void btFile1_Click(object sender, EventArgs e)
			{
			SelectReadFile(textBox1);
			}

		private void btFile2_Click(object sender, EventArgs e)
			{
			SelectReadFile(textBox2);
			}
			
		void SelectReadFile(TextBox tb)
			{
			ofD.Filter = filterTxt;
			ofD.FilterIndex = 1;
			ofD.RestoreDirectory = true;
			ofD.FileName = string.Empty;
			ofD.CheckFileExists = true;
			try
				{
				if (ofD.ShowDialog() == DialogResult.OK)
					{
					
					tb.Text = ofD.FileName;
					}
				}
			catch (Exception ex)
				{
				MessageBox.Show("Errore nel file scelto\n\n" + ex.Message);
				}
			}

		void SelectSaveFile(TextBox tb)
			{
			sfD.Filter = filterTxt;
			sfD.FilterIndex = 1;
			sfD.RestoreDirectory = true;
			sfD.FileName = string.Empty;
			sfD.CheckFileExists = false;
			try
				{
				if (sfD.ShowDialog() == DialogResult.OK)
					{
					tb.Text = sfD.FileName;
					}
				}
			catch (Exception ex)
				{
				MessageBox.Show("Errore nel file scelto\n\n" + ex.Message);
				}
			}

		/// <summary>
		/// Legge le distinte da file
		/// </summary>
		private void LeggiDistinte()
			{
			bool ok = true;
			
			dbNewFilename = dbOldFilename = resFilename = string.Empty;
			string rootOld, rootNew;
			rootOld = rootNew = string.Empty;
			List<string> linee = new List<string>();
			StringBuilder _msg = new StringBuilder(); 
			try
				{

				// Ottiene i percorsi completi
				dbNewFilename = Path.GetFullPath(textBox1.Text);
				dbOldFilename = Path.GetFullPath(textBox2.Text);
				resFilename = Path.GetFullPath(textBox3.Text);

				// Controlla che le distinte da confrontare siano due file differenti e
				// che i due file esistano
				if (string.Compare(dbNewFilename, dbOldFilename, StringComparison.InvariantCultureIgnoreCase) == 0)
					{
					ok = false;
					_msg.AppendLine("Le due distinte devono essere file differenti");
					}
				else
					{
					if(!File.Exists(dbNewFilename))
						{
						ok = false;
						_msg.AppendLine($"Il file: {dbNewFilename} non esiste.");
						}
					if (!File.Exists(dbOldFilename))
						{
						ok = false;
						_msg.AppendLine($"Il file: {dbOldFilename} non esiste.");
						}
					}

				// Controlla che il file dei risultati non esista (o che l'utente voglia sovrascriverlo)
				// e che non sia in sola lettura
				if(ok)
					{
					if(File.Exists(resFilename))
						{
						if(MessageBox.Show($"Il file\n{resFilename}\nesiste già. Sovrascrivere ?", "Sovrascrivere ?", MessageBoxButtons.YesNo) != DialogResult.Yes)
							{
							ok = false;
							MessageBox.Show($"Annullata sovrascrittura del file: {resFilename}");
							}
						else
							{
							// Controlla che il file dei risultati, se esistente, non sia di sola lettura
							FileInfo info = new FileInfo(resFilename);
							if (info.IsReadOnly)
								{
								ok = false;
								_msg.AppendLine($"Il file:\n{resFilename}\nè di sola lettura");
								}
							}
						}
					}
				// Controlla che ci siano i permessi di scrittura nella cartella del file dei risultati
				if(ok)
					{
					string pathRes = Path.GetDirectoryName(resFilename);
					string tmp;
					do
						{
						tmp = pathRes + '\\' + Path.GetRandomFileName();       // File temporaneo casuale
						}
					while (File.Exists(tmp));
					try
						{
						(File.Create(tmp)).Close();     // Prova a scriverlo nella cartella di destinazione (chiude subito il filestream)
						File.Delete(tmp);               // Prova a cancellarlo
						}
					catch (UnauthorizedAccessException)
						{
						ok = false;
						_msg.AppendLine("Errore di accesso nella cartella di destinazione");
						}
					}

				}
			catch(Exception ex)
				{
				ok = false;
				_msg.AppendLine($"Errore:\n{ex.Message}");
				}

			if(_msg.Length > 1)		MessageBox.Show(_msg.ToString());		
			
			if(!ok)			MessageBox.Show("Conversione impossibile o annullata");
			
			if(ok)
				{
				// Importa vecchia distinta
				string msgOld, msgNew;
				rootOld = dbu.ImportaDistintaDaFile(dbOldFilename, out msgOld);
				rootNew = dbu.ImportaDistintaDaFile(dbNewFilename, out msgNew);
				
				// Prepara al'output
				linee.Clear();

				// Messaggi
				linee.Add($"Messaggi per la distinta {rootOld}");
				linee.Add(msgOld);	
				linee.Add(string.Empty);
				// Messaggi
				linee.Add($"Messaggi per la distinta {rootNew}");
				linee.Add(msgNew);	
				linee.Add(string.Empty);
				
				foreach(string root in new string[] {rootOld, rootNew})
					{
					// Radice
					Dbc.Item r = Dbc.Item.GetItem(root);
				
					// Distinta esplosa
					if(r != null)
						{
						linee.Add($"Distinta esplosa: {root}");
						linee.AddRange(r.DistintaEsplosa(true));
						linee.Add($"Fine distinta");
						}
					linee.Add(string.Empty);

					// Distinta esplosa
					if(r != null)
						{
						linee.Add($"Distinta raggruppata: {root}");
						linee.AddRange(r.DistintaRaggruppata(true));
						linee.Add($"Fine distinta");
						}
					linee.Add(string.Empty);

					// Distinta monolivello
					if(r!=null)
						{
						linee.Add($"Creazione distinta monolivello: {root + Dbc.EXPM}");
						Dbc.Item dbe = r.CreaDistintaMonolivello();
						linee.AddRange(dbe.DistintaEsplosa(true));
						linee.Add($"Fine distinta");
						}
					linee.Add(string.Empty);

					#if false
					// Distinta monolivello (solo lista)
					if(r != null)
						{
						linee.Add($"Distinta monolivello: {root}");
						linee.AddRange(r.DistintaMonolivello(true));
						linee.Add($"Fine distinta");
						}
					linee.Add(string.Empty);
					#endif
					}

				// Lista dei codici
				linee.Add("Codici importati");
				linee.Add(Dbc.Item.Codici());
				linee.Add(string.Empty);

				}

			// Salva nel file di riepilogo
			if(ok)
				{
				try
					{
					using (StreamWriter swr = new StreamWriter(resFilename))
						{
						foreach(string str in linee)
							{
							swr.WriteLine(str);	
							}
						}
					}
				catch(Exception ex)
					{
					MessageBox.Show("Errore: " + ex.Message);
					}
				}

			// Memorizza i due codici radice
			if(ok)
				{
				root[0] = rootOld;
				root[1] = rootNew;
				MessageBox.Show($"Lette distinte vecchia {rootOld} e nuova {rootNew}.");
				}
			else
				{
				root[0] = root[1] = String.Empty;
				MessageBox.Show($"Errore nella lettura delle distinte");
				}
			}

		/// <summary>
		/// Calcola la matrice delle somiglianze
		/// </summary>
		/// <returns>false se errore</returns>
		private bool CalcolaSomiglianze(bool esplose = false)
			{
			bool ok = false;

			string c1 = root[0] + (esplose ? Dbc.EXPM: string.Empty);
			string c2 = root[1] + (esplose ? Dbc.EXPM: string.Empty);

			Dbc.Item it1,it2;
			
			it1 = Dbc.Item.GetItem(c1);
			it2 = Dbc.Item.GetItem(c2);
			
			if( (it1 == null) || (it2 == null) )
				{
				MessageBox.Show($"Uno dei due item {c1} o {c2} non esiste.");
				return ok;
				}
			
			ok = dbu.CalcolaSomiglianze(it1,it2);
			
			if(ok)
				{
				Tuple<int,int> keys = dbu.SizeSomiglianze();
				MessageBox.Show($"Completata matrice delle somiglianze ({keys.Item1}x{keys.Item2}).");
				}
			else
				{
				MessageBox.Show("Errore nel calcolo della matrice delle somiglianze");
				}
			return ok;
			}

		/// <summary>
		/// Esegue il confronto della distinte e aggiunge il report al file dei risultati
		/// </summary>
		/// <returns>false se errore</returns>
		private bool ReportSuFile(bool esplose = false)
			{
			bool ok = false;

			List<string> msg;
			msg = dbu.ReportDistintaCompleta(
					Dbc.Item.GetItem(root[0]+ (esplose ? Dbc.EXPM: string.Empty)),
					Dbc.Item.GetItem(root[1]+ (esplose ? Dbc.EXPM: string.Empty))
					);

			try
				{
				using (StreamWriter swr = new StreamWriter(resFilename, true))
					{
					foreach(string str in msg)
						{
						swr.WriteLine(str);	
						}
					ok = true;
					}
				}
			catch(Exception ex)
				{
				MessageBox.Show("Errore: " + ex.Message);
				}
			
			if(ok)
				{
				MessageBox.Show($"Completato riepilogo su file {resFilename}.");
				}
			else
				{
				MessageBox.Show("Errore nella creazione del riepilogo.");
				}
			return ok;
				

			}

		private void btRead_Click(object sender, EventArgs e)
			{
			LeggiDistinte();
			}

		private void btFile3_Click(object sender, EventArgs e)
			{
			SelectSaveFile(textBox3);
			}

		private void btTest_Click(object sender, EventArgs e)
			{
			try
				{
				new Dbc.Item("700.10.100", "aaa");
				new Dbc.Item("700.10.101", "bbb");
				new Dbc.Item("700.10.102", "ccc", Dbc.Unità.cm);
				new Dbc.Item("700.10.001", "asm");
				new Dbc.Item("700.10.010", "asm");
				new Dbc.Item("601.08.110", "_a");
				new Dbc.Item("601.08.111", "_b");
				new Dbc.Item("601.08.112", "_c");

				Dbc.Item.AggiungeLineaDb("700.10.100", "700.10.001", 3);
				Dbc.Item.AggiungeLineaDb("700.10.102", "700.10.001", 1);
				Dbc.Item.AggiungeLineaDb("700.10.010", "700.10.001", 1);
				Dbc.Item.AggiungeLineaDb("601.08.110", "700.10.010", 2);
				Dbc.Item.AggiungeLineaDb("601.08.111", "700.10.010", 2);
				Dbc.Item.AggiungeLineaDb("601.08.112", "700.10.010");

				new Dbc.Item("700.10.002", "asm");
				Dbc.Item.AggiungeLineaDb("700.10.100", "700.10.002", 1);
				Dbc.Item.AggiungeLineaDb("700.10.010", "700.10.002", 1);

				Dbc.Item.AggiungeLineaDb("700.10.002", "700.10.001", 1);
				}
			catch (DbException ex)
				{
				MessageBox.Show(ex.Message);
				}

			try
				{
				new Dbc.Item("700.10.102", "xxx");							// Eccezione
				}
			catch (DbException ex)
				{
				MessageBox.Show(ex.Message);
				}
			
			int x=0;
			try
				{
				/*int a = 10/x; // non gestita*/
				Dbc.Item.AggiungeLineaDb("700.10.001", "601.08.111", x);		// Eccezione
				}
			catch (DbException ex)
				{
				MessageBox.Show(ex.Message);
				}

			try
				{
				Dbc.Item.AggiungeLineaDb("700.10.199", "700.10.001", 1);		// Eccezione
				}
			catch (DbException ex)
				{
				if(ex.Errore == DbException.TipoErr.Dipendenza_Ciclica)
					{
					MessageBox.Show(ex.Message);
					}
				else
					{
					MessageBox.Show("DbException non gestita: " + ex.Message);
					/* throw(ex);	// Genera dinuovo l'eccezione non gestita */
					}
				
				}

			string asm = "700.10.001";
			List<string> lc, lcp, lca;
			lc = new List<string>();
			lcp = new List<string>();
			lca = new List<string>();
			List<string> expl = new List<string>();
			List<string> dr = new List<string>();
			Dbc.Item iii = Dbc.Item.GetItem(asm);
			if(iii != null)
				{
				lc = iii.ListaCodici(true, Dbc.Item.TipoCodice.Tutti);
				lcp = iii.ListaCodici(true, Dbc.Item.TipoCodice.SenzaDb);
				lca = iii.ListaCodici(true, Dbc.Item.TipoCodice.ConDb);

				expl = iii.DistintaEsplosa(true);
				dr = iii.DistintaRaggruppata(true);
				}

			MessageBox.Show($"Lista codici:\n{Dbc.Item.Codici()}");

			StringBuilder strb = new StringBuilder();
			strb.AppendLine($"Lista codici di {asm}");
			foreach(string str in lc)
				{
				strb.AppendLine(str);
				}
			strb.AppendLine($"Lista parti di {asm}");
			foreach(string str in lcp)
				{
				strb.AppendLine(str);
				}
			strb.AppendLine($"Lista assiemi di {asm}");
			foreach(string str in lca)
				{
				strb.AppendLine(str);
				}
			MessageBox.Show(strb.ToString());


			strb.Clear();
			foreach (string str in expl)
				{
				strb.AppendLine(str);
				}
			MessageBox.Show($"Distinta esplosa {asm}:\n{strb.ToString()}");


			strb.Clear();
			foreach (string str in dr)
				{
				strb.AppendLine(str);
				}
			MessageBox.Show($"Distinta raggruppata {asm}:\n{strb.ToString()}");

			strb.Clear();
			string strx = "700.10.010";
			Dbc.Item xx = Dbc.Item.GetItem(strx);
			//foreach (Dbc.Relazione r in xx.Relazioni())
			//	{
			//	strb.AppendLine(r.ToString());
			//	}
			//MessageBox.Show($"Relazioni in {xx.ToString()}:\n{strb.ToString()}");
			MessageBox.Show($"Item ToString(true):\n{xx.ToString(true)}");

			strb.Clear();
			for(int i=0; i<xx.contaDistinta; i++)
				{
				strb.AppendLine(xx.DbItem(i).ToString() + Dbc.SEP + xx.DbQta(i).ToString());
				}
			MessageBox.Show($"DbItem():\n{strb.ToString()}");

			strb.Clear();
			for(int i=0; i<xx.contaDistinta; i++)
				{
				strb.AppendLine(xx[i].ToString());
				}
			MessageBox.Show($"[]:\n{strb.ToString()}");

			string toremove = "700.10.102";
			Dbc.Item.Remove(toremove);
			MessageBox.Show($"Dopo rimozione del {toremove}:\n{Dbc.Item.Codici()}");
			
			}

		private void btReset_Click(object sender, EventArgs e)
			{
			Dbc.Item.Reset();
			root[0] = root[1] = string.Empty;
			Invalidate();
			}

		private void Form1_Paint(object sender, PaintEventArgs e)
			{
			tbDb0.Text = root[0];
			tbDb1.Text = root[1];
			}

		private void btConfrontaDistinte_Click(object sender, EventArgs e)
			{
			LeggiDistinte();
			CalcolaSomiglianze();
			ReportSuFile();
			CalcolaSomiglianze(true);
			ReportSuFile(true);
			}

		private void btCfr_Click(object sender, EventArgs e)
			{

			#if false
			List<string> l = new List<string>
				{
				"712.04.011", "712.05.005b",
				"700.10.100a", "700.10.100b",
				"875.15.901", "700.10.100",
				"700.10.100", "700.10.100a",
				"700.10.100b", "700.10.100",
				"700.10.100a", "700.10.100b",
				"700.10.100a", "700.10.1004",
				"700.10.1004", "700.10.100",
				"700.10.100", "700.10.100"
				

				};
			StringBuilder strb = new StringBuilder();
			for(int i=0; i<l.Count-1; i+=2)
				{
				strb.AppendLine($"{l[i]} <-> {l[i+1]} : {Dbc.SomiglianzaCodici(l[i],l[i+1])}");
				}
			MessageBox.Show(strb.ToString());
			
			List<string> ld = new List<string>
				{
				"STAFFA DI SUPPORTO", "STAFFA DI SUPPORTO CARRO",
				"BRACCIO", "Braccio",
				"staffa", "staffetta",
				"staffa di arresto", "staffa d'arresto"

				};
			strb.Clear();
			for(int i=0; i<ld.Count-1; i+=2)
				{
				strb.AppendLine($"{ld[i]} <-> {ld[i+1]} : {Dbc.SomiglianzaDescrizioni(ld[i],ld[i+1])}");
				}
			MessageBox.Show(strb.ToString());
			#endif

			CalcolaSomiglianze();
			}

		private void btReport_Click(object sender, EventArgs e)
			{
			
			List<string> msg;

			//Queue<Tuple<Dbc.Relazione, Dbc.Relazione>> tmp = new Queue<Tuple<Dbc.Relazione,Dbc.Relazione>>();
			//msg = dbu.ReportDistintaSingola(Dbc.Item.GetItem(root[0]),Dbc.Item.GetItem(root[1]),tmp);
			//MessageBox.Show(msg);

			msg = dbu.ReportDistintaCompleta(Dbc.Item.GetItem(root[0]),Dbc.Item.GetItem(root[1]));

			StringBuilder strb = new StringBuilder();
			foreach(string line in msg)	strb.AppendLine(line);
			MessageBox.Show(strb.ToString());
			}
		}
	}
