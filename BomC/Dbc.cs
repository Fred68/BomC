using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

using TwoKeyDictionary;			// Dizionario a due chiavi


namespace BomC
	{
	public partial class Dbc
		{
		
		/// <summary>
		/// Unità di misura
		/// </summary>
		public enum Unità	{n = 0, mt, cm};
		
		// Lettura distinta base in formato testo
		/// <summary>
		/// Numero parti per riga di file (assieme)
		/// </summary>
		public const int N_ASM = 2;
		/// <summary>
		/// Numero parti per riga di file (componente)
		/// </summary>
		public const int N_PAR = 4;
		/// <summary>
		/// Indice Codice
		/// </summary>
		public const int I_COD = 0;
		/// <summary>
		/// Indice Descrizione
		/// </summary>
		public const int I_DES = 1;
		/// <summary>
		/// Indice Unità
		/// </summary>
		public const int I_UNI = 2;
		/// <summary>
		/// Indice Quantità
		/// </summary>
		public const int I_QTA = 3;

		// ToString()
		/// <summary>
		/// Separatore per ToString()
		/// </summary>
		public const string SEP = "\t";		
		
		// Assiemi duplicati
		/// <summary>
		/// Lunghezza del contatore di assiemi duplicati 
		/// </summary>
		public const int DUPCH = 3;
		/// <summary>
		/// Separatore del contatore di assiemi duplicati
		/// </summary>
		public const char DUPSP = '_';
		/// <summary>
		/// Espensione codice per distinta esplosa monolivello
		/// </summary>
		public const string EXPM = "_EXPM";

		// Valutazione somiglianza
		/// <summary>
		/// Valore restituito per somiglianza codici stringa se sono revisioni dello stesso codice
		/// </summary>
		public const float SOM_REVCOD = -10f;
		/// <summary>
		/// Valore restituito per somiglianza se errore (somiglianza nulla)
		/// </summary>
		public const float SOM_ERR = 0f;
		/// <summary>
		/// Valore restituito in caso si codici identici (somiglianza massima)
		/// </summary>
		public const float SOM_EQU = 1f;
		/// <summary>
		/// Valore restituito per somiglianza item se sono revisioni dello stesso codice
		/// </summary>
		public const float SOM_REV = 0.99f;
		/// <summary>
		/// Separatori di parole per somiglianza descrizioni. Il punto non è un separatore (può essere una abbreviazione)
		/// </summary>
		public static readonly char[] SOM_SEP_DES = new char[] { ' ', ',', ';', '(', ')', '\'' };
		/// <summary>
		/// Lunghezza minima di una parola per somiglianza descrizioni
		/// </summary>
		public const int SOM_TOK_LMIN = 3;
		/// <summary>
		/// Correttore del prodotto tra somiglianza di codice e somiglianza di descrizione, per codici differenti (non revisioni)
		/// </summary>
		public const float SOM_COD_DIFF = 1f;
		/// <summary>
		/// Valore restituito per somiglianza distinte se non calcolabile
		/// </summary>
		public const float SOM_NOCALC = -100f;
		/// <summary>
		/// Moltiplicatore di somiglianza se diversa quantità
		/// </summary>
		public const float SOM_QTADIF = 0.9f;
		/// <summary>
		/// Massimo numero di cicli completi (profondità) per calcolare le somiglianze degli assimi con distinta
		/// </summary>
		public const uint MAX_CICLI = 50;
		/// <summary>
		/// Similarità minima tra due item per considerarli come una sostituzione
		/// </summary>
		public const float SOM_MIN_SOST = 0.3f;

		/// <summary>
		/// Dizionario doppio delle somiglianze tra coppie di codici.
		/// </summary>
		private TwoKeyDictionary<string, string, Somiglianza> dict;
		
		/// <summary>
		/// Classe per memorizzare le somiglianze tra i codici
		/// </summary>
		public class Somiglianza
			{
			float _som;			// Indice di somiglianza
			bool _set;			// Flag se è già stato impostato
			bool _equ;			// Se uguali
			bool _rev;			// Se revisioni
			bool _nocalc;		// Non calcolabile

			public float Som
				{
				get
					{
					return _som;
					}
				set
					{
					_som = value;
					_set = true;		// Imposta anche il flag
					_nocalc = false;
					}
				}

			public bool Equ
				{
				get
					{
					return _equ;
					}
				set
					{
					_equ = value;
					if(_equ)				// Se imposta a true...
						{
						_som = SOM_EQU;		// Allora imposta anche valore e flag
						_set = true;
						_rev = false;
						}
					}
				}


			public bool NoCalc
				{
				get
					{
					return _nocalc;
					}
				set
					{
					_nocalc = value;
					_set = true;
					if(_nocalc)
						{
						_som = SOM_NOCALC;
						}
					}
				}


			public bool Rev
				{
				get
					{
					return _rev;
					}
				set
					{
					_rev = value;
					if(value)				// Se imposta a true...
						{
						_som = SOM_REV;		// Allora imposta anche valore e flag
						_set = true;
						_equ = false;
						}
					}
				}
					
			public Somiglianza()
				{
				Clear();
				}

			public void Clear()
				{
				_som = 0.0f;
				_set = false;
				_equ = _rev = false;
				_nocalc = false;
				}

			public override string ToString()
				{
				return _set ? _som.ToString() : "?";
				}

			public bool IsSet
				{
				get { return _set; }
				}

			}

		/// <summary>
		/// Ctor
		/// </summary>
		public Dbc()
			{
			dict = new TwoKeyDictionary.TwoKeyDictionary<string, string, Somiglianza>();	// Crea il dizionario
			}

		/// <summary>
		/// Azzera tutto
		/// </summary>
		public void Clear()
			{
			dict.Clear();
			Item.Reset();
			}

		/// <summary>
		/// Ricava codice duplicato
		/// </summary>
		/// <param name="cod">codice</param>
		/// <param name="i">indice</param>
		/// <returns></returns>
		string Cod_dupl(string cod, int i)
			{
			return cod + DUPSP + i.ToString().PadLeft(DUPCH, '0');
			}

		/// <summary>
		/// Legge l'unità di misura
		/// </summary>
		/// <param name="txt"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		Unità UnitaDaStringa(string txt)
			{
			Unità u = Unità.n;
			if(txt.Contains("n"))
				u = Unità.n;
			else if(txt.Contains("mt"))
				u = Unità.mt;
			else if(txt.Contains("cm"))
				u = Unità.cm;
			else
				throw new Exception("Unità di misura non riconosciuto nel testo dela distinta");
			return u;
			}

		/// <summary>
		/// Estrae la quantità (solo > 0)
		/// </summary>
		/// <param name="txt"></param>
		/// <returns></returns>
		int QtaDaStringa(string txt)
			{
			int qta = 0;
			int x;
			if(int.TryParse(txt, out x))
				{
				if(x > 0)
					qta = x;
				}
			return qta;
			}

		/// <summary>
		/// Legge un file di testo e ne importa la distinta
		/// </summary>
		/// <param name="nomefile">nomefile completo</param>
		/// <returns>codice radice della distinta</returns>
		public string ImportaDistintaDaFile(string nomefile, out string msg)
			{
			string root = string.Empty;
			string line;
			
			List<string> asm_doppi = new List<string>();		// Lista dei codici doppi, per verifica finale

			StringBuilder _msg = new StringBuilder();			// Messaggi

			try
				{
				using (StreamReader reader = new StreamReader(nomefile))
					{
					
					string current_asm = String.Empty;		// Assieme corrente in cui importare i codici

					string cod, des, uni, qta;				// Temporanei
					Item x;	
					Unità u;
					int qt;
					Item last;

					while ((line = reader.ReadLine()) != null)
						{
						string[] parts = line.Split(';');

						switch(parts.Length)
							{
							case N_ASM:
								// Estrae e verifica i codici
								cod = parts[I_COD].Trim();
								des = parts[I_DES].Trim();
								if(cod.Length < 1)		break;		// Interrompe se codice nullo
								
								x = Item.GetItem(cod);
								if(x != null)						// Se il codice cod esiste già
									{
									if(x.haDistinta)				// e ha già una distinta
										{							// Cerca il primo codice libero con un suffisso progressivo
										int i = 0;
										string cod_d = Cod_dupl(cod, i);
										while(Item.GetItem(Cod_dupl(cod, i)) != null)
											{
											cod_d = Cod_dupl(cod, ++i);
											}
										if(i > 900)
											{
											throw new DbException($"Codice progressivo {cod} libero non trovato", DbException.TipoErr.Codice_Libero_Non_Trovato);
											}
										cod = cod_d;					// Usa il codice duplicato
										asm_doppi.Add(cod_d);			// Memorizza il codice duplicato, per futura verifica
										}
									}
								try
									{
									new Item(cod, des, Unità.n);		// Crea il codice cod (semplice o duplicato)
									}
								catch (DbException ex)					// Non considera l'eccezione del codice duplicato
									{
									if(ex.Errore != DbException.TipoErr.Codice_Duplicato)
										{
										throw ex;
										}
									}

								if(current_asm == String.Empty)			// E 'il primo codice di assieme: è l'assieme root
									{
									root = cod;
									}
								else
									{									// Ordina la distinta dell'utlimo assieme completato
									last = Item.GetItem(current_asm);
									if(last != null)
										{
										last.OrdinaDistinta();
										}
									}
								
								current_asm = cod;					// Lo imposta come l'assieme attuale
								_msg.AppendLine($"->Assieme {current_asm}");
								break;

							case N_PAR:
								// Estrae e verifica i codici
								cod = parts[I_COD].Trim();
								des = parts[I_DES].Trim();
								uni = parts[I_UNI].Trim();
								qta = parts[I_QTA].Trim();
								if(cod.Length < 1)		break;      // Interrompe se codice nullo

								// Valutare l'unità di misura (se errore, eccezione)
								u = UnitaDaStringa(uni);

								// Valutare la quantità (se errore, qtà = 0)
								qt = QtaDaStringa(qta);
								
								x = Item.GetItem(cod);
								if (x == null)		// Se il codice non esiste ancora
									{
									new Item(cod, des, u);	// ...lo aggiunge
									}
								else
									{                               // Se esiste, verifica che i due item siano identici
									if(!Item.ItemUguale(x, cod, u, des))		// Se non lo sono: errore (distinte txt esportate in momenti diversi...)
										{
										throw new DbException($"Codice {cod} non univoco", DbException.TipoErr.Codici_non_corrispondenti);
										}
									}
								if (current_asm.Length > 0)          // Aggiunge l'item all'assieme current_asm (se non è nullo)
									{
									Item.AggiungeLineaDb(cod, current_asm, qt);
									#if false
									_msg.AppendLine($"->Aggiunto {cod} all'assieme {current_asm}");
									#endif
									}
								break;
								
							default:        // Non fa nulla: linea scartata
								break;
							}	// switch

						#if false
						// Scrive le parti estratte
						for(int i=0; i<parts.Length; i++)
							{
							_msg.Append(parts[i].Trim().ToString() + " <-> ");
							}
						_msg.Append(System.Environment.NewLine);
						#endif
						}	// while

					// Ordina la distinta dell'utlimo assieme completato
					last = Item.GetItem(current_asm);
					if(last != null)
						{
						last.OrdinaDistinta();
						}

					}	// using streamreader
				
				// Controlla che i codici doppi di assieme importati abbiano distinte identiche ai primi assiemi importati
				foreach(string a2 in asm_doppi)		// Percorre tutti gli assiemi duplicati
					{
					string a1 = a2.Remove(a2.Length - DUPCH - 1);	// Ottiene il codice dell'assieme base (taglia l'ultima parte)	
					Item it1 = Item.GetItem(a1);					// Ottiene i due item (copia ed originario)
					Item it2 = Item.GetItem(a2);
					if(!Item.ItemUguale(it1, it2, true, true))		// Li confronta
						{
						throw new DbException($"I codici duplicati {a1} e {a2} hanno distinte differenti",DbException.TipoErr.Distinte_non_corrispondenti);
						}
					else
						{
						_msg.AppendLine($"Ok: {a2} == {a1}");
						}
					}

				// Elimina gli assiemi doppi
				foreach (string a2 in asm_doppi) {
					Item.Remove(a2);
					}

				}	// try
			catch(Exception ex)
				{
				_msg.AppendLine("Errore: "+ex.Message);
				}
			
			msg = _msg.ToString();
			return root;
			}
		
		/// <summary>
		/// Delegate per la funzione che verifica un carattere (se testo o se numero)
		/// </summary>
		/// <param name="ch"></param>
		/// <returns></returns>
		private delegate bool TestChar(char ch);

		/// <summary>
		/// Dimensioni della matrice delle somiglianze
		/// </summary>
		/// <returns>Tuple<int,int></returns>
		public Tuple<int,int> SizeSomiglianze()
			{
			return dict.Size();
			}

		/// <summary>
		/// Scelta
		/// </summary>
		public enum IsCharOrDigit { isChar, isDigit };
	
		/*** Somiglianza tra stringhe ***/
		/// <summary>
		/// Indice di somiglianza di due codici, verificando se sono revisioni
		///	Trascura maiuscole e minuscole (non 'case-sensitive')
		/// Se -1, i due codici sono revisioni dello stesso codice
		/// </summary>
		/// <param name="cod1">primo codice</param>
		/// <param name="cod2">secondo codice</param>
		/// <param name="lRev">lunghezza max dell'indice di revisione</param>
		/// <param name="isod">la revisione è di lettere o cifre</param>
		/// <returns>float [0...1], -1 se revisioni, 0 se codice nullo</returns>
		static Somiglianza SomiglianzaCodici(string cod1, string cod2, int lRev = 1, IsCharOrDigit isod = IsCharOrDigit.isChar)
			{

			Somiglianza s = new Somiglianza();
			
			int count, lastch, i;
			bool foundLast = false;
			
			int lmin = Math.Min(cod1.Length, cod2.Length);
			int lmax = Math.Max(cod1.Length, cod2.Length);
			
			// Esce subito se uno dei due codici è nullo
			if( (lmin==0) || (lmax==0) )
				{
				return s;		// Ctor Somiglianza() imposta tutto a false
				}

			// Se codice identico, imposta ed esce subito
			if(cod1 == cod2)
				{
				s.Equ = true;	
				return s;
				}

			// Conta i caratteri uguali nella identica posizione nei due codici
			// Memorizza in lastch la pos. dell'ultimo carattere uguale, partendo dall'inizio
			// Il separatore di codice è considerato un carattere come gli altri, per semplicità.
			for(i = count = 0, lastch = -1; i < lmin; i++)		
				{
				if(Char.ToUpper(cod1[i]) == Char.ToUpper(cod2[i]))
					{
					count++;
					if(!foundLast)
						lastch = i + 1;
					}
				else
					{
					foundLast = true;
					}
				}
			
			if((lmin != lmax) && !foundLast)
				{
				foundLast = true;		// Se sono di lunghezza diversa, l'ultimo carattere uguale è il finale del codice più corto.
				lastch = lmin;
				}

			// Calcola la somiglianza (numero di caratteri uguali / lunghezza del codice maggiore) * SOM_EQU (costante)
			if(lmax > 0)
				{
				s.Som = SOM_EQU * ((float)count) / ((float)lmax);
				}
			// Verifica se la parte finale, differente tra nei due codici, è un indice di revisione
			if(foundLast && (lastch >= 0))
				{
				TestChar tc;
				if(isod == IsCharOrDigit.isChar)	// Sceglie la funzione di controllo: carattere o cifra
					{
					tc = Char.IsLetter;
					}
				else
					{
					tc = Char.IsDigit;
					}
				string[] fin = new string[2];
				int[] isrev = new int[2];			// 1 è un codice di revisione, 0 può esserlo (se lo è l'altro), -1 non lo è di sicuro
				fin[0] = cod1.Remove(0,lastch);		// Estrae le parti finali dei codici (codici o lunghezze differenti)
				fin[1] = cod2.Remove(0,lastch);
				isrev[0] = isrev[1] = 0;
				for(i = 0; i<2 ; i++)				// Ripete per entrambi i codici
					{
					if((fin[i].Length > 0) && (fin[i].Length <= lRev))	// Se c'é una parte finale e non è più lunga di lRev...
						{
						isrev[i] = 1;				// ...è probabilmente una revisione
						}
					foreach(char ch in fin[i])		// Controlla che tutti i caratteri siano di una revisione
						{
						if(!tc(ch))
							{
							isrev[i] = -1;			// Se uno solo non lo è...allora non è di certo una revisione
							break;
							}
						}
					}
				// Risultati:
				//				-1		0		1
				//		-1		no		no		no
				//		0		no		no		si
				//		1		no		si		si
				if(isrev[0] + isrev[1] > 0)
					{
					// s = SOM_REVCOD;		// Se la somma è >0: è una revisione tra codici
					s.Rev = true;
					}
				}

			return s;
			}
		
		/// <summary>
		/// Indice di somiglianza di due descrizioni, basata sulle parole comuni
		///	Trascura maiuscole e minuscole (non 'case-sensitive')
		/// </summary>
		/// <param name="des1"></param>
		/// <param name="des2"></param>
		/// <returns></returns>
		static Somiglianza SomiglianzaDescrizioni(string des1, string des2)
			{
			Somiglianza s = new Somiglianza();
			
			try
				{
				int count = 0;
				List<string>[] tok = new List<string>[]				// Divide le parole delle descrizioni in 'token'
					{
					des1.Split(SOM_SEP_DES).Distinct().ToList(),
					des2.Split(SOM_SEP_DES).Distinct().ToList()
					};

				for(int j=0; j< tok.Length; j++)
					{
					for(int i=0; i<tok[j].Count; i++)
						{
						if(tok[j][i].Length < SOM_TOK_LMIN)			// Elimina i token di lunghezza insufficiente:...
							{
							tok[j].RemoveAt(i);						// ...scarta articoli, congiunzioni...
							}
						}
					}

				int toks = Math.Max(tok[0].Count,tok[1].Count);		// Ottiene il numero di token maggiori tra le due descrizioni
				if(toks > 0)
					{
					for(int i1 = 0; i1 < tok[0].Count; i1++)
						{
						for(int i2 = 0; i2< tok[1].Count; i2++)
							{
							if(tok[0][i1].Equals(tok[1][i2], StringComparison.CurrentCultureIgnoreCase))	// Confronta i token
								{
								count++;							// Se un token della seconmda descrizione corrisponde, lo conteggia...
								tok[1].RemoveAt(i2);				// ...ed elimina il token dalla lista (per evitare doppi conteggi)
								}
							}
						}
					s.Som = SOM_EQU * (float)count / ((float)toks);		// Calcola la somiglianza
					}
				else
					{
					s.Som = 0.0f;									// Somiglianza nulla se non ci sono token (corti o insignificanti)
					}
				}
			catch
				{
				s.Som = SOM_ERR;									// Se errore
				}
			return s;
			}

		/*** Somiglianza tra Item ***/
		/// <summary>
		/// Somiglianza di due Item basata su codice e descrizione.
		/// Utilizza codice, eventuale revisione e descrizione.
		/// </summary>
		/// <param name="it1">Primo item</param>
		/// <param name="it2">Secondo item</param>
		/// <param name="lRev">lunghezza max dell'indice di revisione</param>
		/// <param name="isod">la revisione è di lettere o cifre</param>
		/// <returns>Indice di somiglianza, oppure SOM_REV se sono revisioni o SOM_ERR se errore</returns>
		public Somiglianza SomiglianzaItemSuCodici(Item it1, Item it2, int lRev = 1, IsCharOrDigit isod = IsCharOrDigit.isChar)
			{
			Somiglianza s = new Somiglianza();
			Somiglianza scod;		// Somiglianza codici
			Somiglianza sdes;		// Somiglianza descrizioni

			if((it1 == null) || (it2 == null))		// Esce se un item è null
				{
				s.Som = SOM_ERR;
				return s;
				}

			scod = SomiglianzaCodici(it1.codice, it2.codice, lRev, isod);	// Calcola somiglianza dei codici
			if(scod.Equ)
				{
				s.Equ = true;									// Se codice identico		
				}
			else if(scod.Rev)
				{
				s.Rev = true;									// Se sono revisioni dello stesso codice
				}
			else 
				{											
				sdes = SomiglianzaDescrizioni(it1.descrizione, it2.descrizione);	// Tutti gli altri casi: usa codice e descrizione
				s.Som = scod.Som * sdes.Som * SOM_COD_DIFF;
				}
			return s;
			}

		/// <summary>
		/// Somiglianza di due Item basata su distinta.
		/// Utilizza il TwoKeyDictionary<string, string, Somiglianza> delle somiglianze
		/// </summary>
		/// <param name="it1"></param>
		/// <param name="it2"></param>
		/// <param name="dict">}</param>
		/// <returns>Indice di somiglianza oppure SOM_NOCALC se non calcolabile</returns>
		public Somiglianza SomiglianzaSuDistinte(Item it1, Item it2)
			{
			Somiglianza s = new Somiglianza();	// Somiglianza
			int NitemMax;						// Lunghezza distinta maggiore
			bool ok = false;					// Flag errore

			if( (it1 != null) && (it2 != null))					// Verifica che gli item non siano nulli ed abbiano entrambi una distinta
				if(it1.haDistinta && it2.haDistinta)
					ok = true;
			if(!ok)												// In caso contrario, esce anticipatamente	
				return s;

			NitemMax = Math.Max(it1.contaDistinta, it2.contaDistinta);	// Lunghezza della distinta maggiore

			if(NitemMax == 0)
				throw new Exception("ERRORE LMAX = 0");

			foreach(Relazione r1 in it1.Relazioni())			// Percorre tutti i codici della prima lista...
				{
				foreach(Relazione r2 in it2.Relazioni())		// Percorre tutti i codici della prima lista...
					{
					if(!dict[r1.codice, r2.codice].IsSet)		// Se manca una somiglianza, imposta errore
						{
						ok = false;
						break;
						}
					}
				}
			if(!ok)									// Se non ok (non calcolabile), esce restituendo SOM_NOCALC
				{
				s.NoCalc = true;
				return s;
				}

			float cont = 0f;						// Contatore di similitudine (azzerato)

			Relazione rel2max;						// Temporanei, per ricerca di codice e similarità del codice della prima distinta...
			float sim2max;							// ...con quelli della seconda
			
			foreach(Relazione r1 in it1.Relazioni())		// Percorre tutti i codici della prima lista...
				{
				rel2max = null;								// Azzera contatori
				sim2max = float.MinValue;

				foreach(Relazione r2 in it2.Relazioni())		// Percorre tutti i codici della seconda lista...
					{
					Somiglianza _s = dict[r1.codice, r2.codice];
					if( _s.Som > sim2max)
						{
						rel2max = r2;							// Memorizza la relazione (della seconda distinta) più simile
						sim2max = _s.Som;
						}
					}
				
				if(rel2max != null)								// Se l'ha trovata, somma la quantità al contatore
					{
					if(r1.quantità == rel2max.quantità)			// Se la quantità è la stessa, la somma normalmente
						cont += sim2max;					
					else
						cont += sim2max * SOM_QTADIF;			// Se è diversa, la moltiplica per un fattore 
					}
				}
			
			s.Som = SOM_EQU * cont / ((float) NitemMax);			// Divide il contatore per la lunghezza della distinta più lunga

			return s;
			}

		/// <summary>
		/// Calcola gli indici di somiglianza di tutte le coppie di codici delle due distinte
		/// Utilizza il TwoKeyDictionary<string, string, Somiglianza> delle somiglianze, già parzialmente compilato
		/// </summary>
		/// <param name="it1">Primo item</param>
		/// <param name="it2">Secondo item</param>
		/// <param name="lRev">lunghezza max dell'indice di revisione</param>
		/// <param name="isod">la revisione è di lettere o cifre</param>
		/// <returns>false se il calcolo non è completo</returns>
		public bool CalcolaSomiglianze(Item it1, Item it2, int lRev = 1, IsCharOrDigit isod = IsCharOrDigit.isChar)
			{
			bool fatto = false;											// Valore restituito, se ha compilato tutte le somiglianze
			if((it1 == null) || (it2 == null))	return fatto;			// Verifica che gli item non siano nulli
			List<string>[] lstCod = new List<string>[2];				// Crea la lista dei codici delle distinte dei due item
			lstCod[0] = it1.ListaCodici();
			lstCod[1] = it2.ListaCodici();
			if((lstCod[0] == null) || (lstCod[1] == null))				// Se un item è nullo, esce.
				{ return fatto; }
			
			if((lstCod[0].Count == 0) || (lstCod[1].Count == 0))		// Se una sola delle distinte è nulla, esce
				{ return fatto; }

			dict.Clear();												// Azzera il dizionario (matrice doppia) di somiglianza...
			foreach(string s0 in lstCod[0])								// ...e lo inizializza
				{
				foreach(string s1 in lstCod[1])
					{
					dict[s0, s1] = new Somiglianza();
					}
				}

			Queue<Tuple<string,string>> ccnc = new Queue<Tuple<string,string>>();		// Coda delle coppie di codici non calcolabili immediatamente

			// Calcola somiglianza di tutte le coppie di codici delle due distinte,
			// tranne quelle in cui entrambi i codici hanno una distinta: il confronto è più complesso.
			// Memorizza le coppie di codici, entrambi con distinta, non calcolabili immediatamente, in una coda

			foreach(string s0 in lstCod[0])				// Percorre i codici della prima...
				{
				foreach(string s1 in lstCod[1])			// ...e della seconda distinta
					{
					Item _i0 = Item.GetItem(s0);		// Ottiene gli Item
					Item _i1 = Item.GetItem(s1);
					if( (_i0 != null) && (_i1 != null))		// Se non sono nulli...
						{
						if( (!_i0.haDistinta) || (! _i1.haDistinta))	// ...e se almeno uno non ha distinta
							{
							dict[s0, s1] = SomiglianzaItemSuCodici(_i0, _i1, lRev, isod);		// Calcola la somiglianza degli item su codice e descrizione
							}
						else
							{													// Se hanno entrambi una distinta
							ccnc.Enqueue(new Tuple<string,string>(s0, s1));		// Aggiunge la coppia di codici alla lista dei non calcolabili
							}
						}
					}
				}		// Al termine dell'operazione, mancano tutte le coppie di item aventi entrambi una distinta
			
			uint ripetMax = MAX_CICLI * (uint)ccnc.Count;			// Calcola il numero di ripetizioni massime
			
			for(int i=0; (i < ripetMax) && (!fatto); i++)			// Ciclo (limitato ad un numero massimo di iterazioni)
				{
				if(ccnc.Count == 0)							// Se non ci sono più elementi: finisce ed esce da ciclo
					{
					fatto = true;
					continue;
					}
				Tuple<string,string> cc = ccnc.Dequeue();	// Estrae una coppia di codici
				Item _it1 = Item.GetItem(cc.Item1);			// Ottiene i due oggetti della 'tupla'
				Item _it2 = Item.GetItem(cc.Item2);
				Somiglianza somD = SomiglianzaSuDistinte(_it1, _it2);		// Calcola la somiglianza delle distinte
				if(somD.NoCalc)								// Se non è calcolabile...
					{
					ccnc.Enqueue(cc);						// ...la rimette in coda.
					}
				else
					{										
					Somiglianza s = new Somiglianza();		// Altrimenti...
					Somiglianza somC = SomiglianzaItemSuCodici(_it1, _it2, lRev, isod);		// Calcola anche la somiglianza sui codici.

					if(_it1.codice == _it2.codice)
						{
						s.Equ = true;						// Se hanno lo stesso codice: la somiglianza è 1.
						}
					else if(somC.Rev)					
						{
						s.Rev = true;						// Se sono revisioni di codice: la somiglianza è SOM_REV (elevata)
						}
					else
						{
						s = somD;							// Se sono codici differenti: usa la somiglianza delle distinte, non dei codici					
						}
					dict[_it1.codice, _it2.codice] = s;		// Memorizza il valore nel dizionario delle somiglianze
					}
				}
			return fatto;
			}

		public List<string> ReportDistintaCompleta(Item it1, Item it2, int lRev = 1, IsCharOrDigit isod = IsCharOrDigit.isChar)
			{
			List<string> msg= new List<string>();
			Queue<Tuple<Relazione,Relazione>> db;

			if((it1 == null) || (it2 == null))
				{
				msg.Add("Uno dei codici è nullo");
				return msg;
				}

			if( (!it1.haDistinta) || (!it2.haDistinta) )
				{
				msg.Add("Uno dei codici non ha distinta base");
				return msg;
				}

			db = new Queue<Tuple<Relazione, Relazione>>();

			msg.Add($"\nConfronto completo distinte {it1.codice} e {it2.codice}");
			
			db.Enqueue(new Tuple<Relazione, Relazione>(new Relazione(it1.codice), new Relazione(it2.codice)));	// Accoda la prima coppia
			while(db.Count > 0)
				{
				Tuple<Relazione,Relazione> r = db.Dequeue();									// Estrae la testa della coda
				msg.AddRange(ReportDistintaSingola(r.Item1.item, r.Item2.item, db));			// Elabora la coppia
				}

			return msg;
			}

		public List<string> ReportDistintaSingola(Item it1, Item it2, Queue<Tuple<Relazione, Relazione>> codConDb, int lRev = 1, IsCharOrDigit isod = IsCharOrDigit.isChar)
			{
			List<string> msg= new List<string>();

			if((it1 == null) || (it2 == null))
				{
				msg.Add("Uno dei codici è nullo");
				return msg;
				}

			if( (!it1.haDistinta) || (!it2.haDistinta) )
				{
				//msg.Add("Uno dei codici non ha distinta base");
				return msg;
				}

			msg.Add($"\nConfronto distinte {it1.codice} e {it2.codice}");

			List<Relazione>[] dist = new List<Relazione>[2];				// Liste delle relazioni delle due distinte
			dist[0] = new List<Relazione>();
			dist[1] = new List<Relazione>();
			List<Tuple<Relazione, Relazione>> uguali = new List<Tuple<Relazione,Relazione>>();		// Lista relazioni con codici identici
			List<Tuple<Relazione, Relazione>> revisioni = new List<Tuple<Relazione,Relazione>>();	// Lista relazioni con codici revisionati
			
			foreach(Relazione r1 in it1.Relazioni())		dist[0].Add(r1);
			foreach(Relazione r2 in it2.Relazioni())		dist[1].Add(r2);

			foreach(Relazione r1 in dist[0])								// Primo ciclo: trova tutti i codici uguali o revisioni
				{
				foreach(Relazione r2 in dist[1])
					{
					Somiglianza s = dict[r1.codice,r2.codice];
					if(s.Equ)
						{
						uguali.Add(new Tuple<Relazione, Relazione>(r1,r2));			// I codici identici non vanno confrontati
						}
					else if(s.Rev)
						{
						revisioni.Add(new Tuple<Relazione, Relazione>(r1,r2));
						if (r1.item.haDistinta && r2.item.haDistinta)               // Se sono revisioni ed hanno entrambi distinta...
							{
							codConDb.Enqueue(new Tuple<Relazione, Relazione>(r1, r2));  // ...li accoda alle distinte da confrontare
							}
						}
					}
				}
			
			bool addedMsg;

			msg.Add("Codici identici");
			addedMsg = false;
			foreach(Tuple<Relazione,Relazione> t in uguali)					// Percorre le relazioni trovate e le elimina dalle distinte temporanee
				{
				dist[0].Remove(t.Item1);
				dist[1].Remove(t.Item2);
				if(t.Item1.quantità == t.Item2.quantità)
					{
					msg.Add($"{t.Item1.codice} : {t.Item1.item.unita}{t.Item1.quantità} === {t.Item2.codice} : {t.Item2.item.unita}{t.Item2.quantità}");
					addedMsg = true;
					}
				else
					{
					msg.Add($"{t.Item1.codice} : {t.Item1.item.unita}{t.Item1.quantità} .=. {t.Item2.codice} : {t.Item2.item.unita}{t.Item2.quantità}");
					addedMsg = true;
					}
				}
			if(!addedMsg)	msg.RemoveAt(msg.Count - 1);

			msg.Add("Codici revisionati");
			addedMsg = false;
			foreach(Tuple<Relazione,Relazione> t in revisioni)
				{
				dist[0].Remove(t.Item1);
				dist[1].Remove(t.Item2);
				msg.Add($"{t.Item1.codice} : {t.Item1.item.unita}{t.Item1.quantità} <-- {t.Item2.codice} : {t.Item2.item.unita}{t.Item2.quantità}");
				addedMsg = true;
				}
			if(!addedMsg)	msg.RemoveAt(msg.Count - 1);


			msg.Add("Codici sostituiti");
			addedMsg = false;
			for(int i=0; i<dist[0].Count; i++)					// Percorre gli elementi della prima lista e cerca, per ognuno,
				{												// quello con la massima similarità
				Somiglianza sim = new Somiglianza();
				int indx = -1;
				for(int j=0; j<dist[1].Count; j++)
					{
					Somiglianza s = dict[dist[0][i].codice,dist[1][j].codice];
					if( (s.Som > SOM_MIN_SOST) && (s.Som > sim.Som))		// Solo se supera la soglia minima di similarità
						{
						indx = j;
						sim = s;
						}
					}
				if(indx != -1)									// Se l'ha trovato...
					{
					codConDb.Enqueue(new Tuple<Relazione,Relazione>(dist[0][i],dist[1][indx]));	// ...accoda la coppia alle distinte da confrontare	
					msg.Add(															// ...aggiunge al report...
						$"{dist[0][i].codice} {dist[0][i].item.unita}{dist[0][i].quantità} <[{sim.Som.ToString("#.##")}]> {dist[1][indx].codice} {dist[1][indx].item.unita}{dist[1][indx].quantità}"
						);
					dist[0].RemoveAt(i);						// ...ed elimina gli elementi dalle liste
					dist[1].RemoveAt(indx);
					addedMsg = true;
					}
				}
			if(!addedMsg)	msg.RemoveAt(msg.Count - 1);


			msg.Add($"Codici aggiunti in {it1.codice}");
			addedMsg = false;
			foreach(Relazione r in dist[0])
				{
				msg.Add($"{r.codice} : {r.item.unita}{r.quantità}");
				addedMsg = true;
				}
			if(!addedMsg)	msg.RemoveAt(msg.Count - 1);


			msg.Add($"Codici eliminati in {it2.codice}");
			addedMsg = false;
			foreach(Relazione r in dist[1])
				{
				msg.Add($"{r.codice} : {r.item.unita}{r.quantità}");
				addedMsg = true;
				}
			if(!addedMsg)	msg.RemoveAt(msg.Count - 1);

			return msg;
			}
		
		}		// Fine partial class Dbc
	}		// Fine namespace
