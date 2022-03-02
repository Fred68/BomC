using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

#if DEBUG
using System.Windows.Forms;     // Solo per debug
#endif


namespace BomC
	{
	 public partial class Dbc
		{

		/// <summary>
		/// Oggetto
		/// Il codice è identificatore univoco
		/// </summary>
		public class Item : IComparable<Item>
			{

			/// <summary>
			/// Lista statica degli item (i codici devono essere univoci, anche tra distinte diverse)
			/// </summary>
			static List<Item> _items;

			/// <summary>
			/// Identificatore univoco
			/// </summary>
			string _cod;
			
			/// <summary>
			/// Descrizione
			/// </summary>
			string _des;

			/// <summary>
			/// Tipo di unità
			/// </summary>
			Unità _unit;

			/// <summary>
			/// Distinta (se è un assieme)
			/// </summary>
			List<Relazione> _dist;
			bool ordinata;

			/**********************************************************************************************/

			#region STATIC

			/// <summary>
			/// Costruttore statico
			/// Crea la lista univoca degli item (codici)
			/// </summary>
			static Item()
				{
				_items = new List<Item>();
				}

			/// <summary>
			/// Azzera tutto
			/// </summary>
			public static void Reset()
				{
				while(_items.Count > 0)
					{
					Item it = _items[0];
					Item.Remove(it);
					}
				_items.Clear();		// Superfluo
				}

			/// <summary>
			/// Elimina un item
			/// </summary>
			/// <param name="item"></param>
			public static void Remove(Item item)
				{
				foreach(Item it in _items)					// Percorre tutti gli item
					{
					if(it._dist != null)
						{
						Stack<Relazione> stack = new Stack<Relazione>();
						foreach(Relazione r in it._dist)	// Elimina tutte le relazioni contenenti il codice da eliminare 
							{
							if(r.codice == item.codice)
								{
								
								stack.Push(r);				// Non esegue subito it._dist.Remove(r), durante il ciclo forech
								it.ordinata = false;
								}
							}
						while(stack.Count > 0)
							{
							Relazione r = stack.Pop();
							it._dist.Remove(r);
							}
						}
					}
				if(_items.Contains(item))					// rimuove l'item dalla lista globale
					{
					_items.Remove(item);
					}
				}

			/// <summary>
			/// Elimina un Item (in base al codice, univoco).
			/// </summary>
			/// <param name="cod_item"></param>
			public static void Remove(string cod_item)
				{
				Item foundItem = _items.Find( x => x._cod == cod_item );
				if(foundItem != null)
					{
					_items.Remove(foundItem);
					}
				}
			
			/// <summary>
			/// Dump dei codici
			/// </summary>
			/// <returns></returns>
			public static string Codici()
				{
				StringBuilder strb = new StringBuilder();
				strb.AppendLine($"Codici[{_items.Count}]");
				foreach(Item item in _items)
					{
					strb.AppendLine(item.codice + (item.haDistinta ? "\t\t[DB]" : ""));
					}
				return strb.ToString();
				}
			
			/// <summary>
			/// Trova l'item con il codice (oppure null).
			/// </summary>
			/// <param name="codice"></param>
			/// <returns></returns>
			public static Item GetItem(string codice)
				{
				return _items.Find( x => x._cod == codice );	// Può restituire null
				}

			/// <summary>
			/// Aggiunge una linea di distina (relazione)
			/// </summary>
			/// <param name="codice"></param>
			/// <param name="codice_padre"></param>
			/// <param name="quantità"></param>
			/// <exception cref="Exception"></exception>
			public static void AggiungeLineaDb(string codice, string codice_padre, int quantità = 1)
				{
				Item padre = GetItem(codice_padre);
				if(padre != null)
					{
					padre.AggiungeLineaDb(codice, quantità);
					}
				else
					{
					throw new DbException($"Il codice {codice_padre} non esiste", DbException.TipoErr.Codice_Inesistente);
					}
				}

			/// <summary>
			/// Confronta un item con i dati di un altro item.
			/// La descrizione non viene utilizzata (potrebbe esser diversa se troncata da file simili)
			/// </summary>
			/// <param name="it1">Primo item</param>
			/// <param name="codice">codice del secondo item</param>
			/// /// <param name="unità">unità del secondo item</param>
			/// <param name="descrizione"> del secondo item</param>
			/// <returns></returns>
			public static bool ItemUguale(Item it1, string codice, Unità unità, string descrizione = "")
				{
				return ( (it1.codice == codice) && (it1.unita == unità) /*&& (it1.descrizione == descrizione)*/ );
				}

			/// <summary>
			/// Conffronta due item con distinta
			/// </summary>
			/// <param name="it1">Primo item</param>
			/// <param name="it2">Secondo item</param>
			/// <param name="conDistinta">Include la distinta (solo primo livello)</param>
			/// <param name="soloDistinta">Trascura il codice dei due item</param>
			/// <returns></returns>
			public static bool ItemUguale(Item it1, Item it2, bool conDistinta = true, bool soloDistinta = false)
				{
				bool uguali = true;
				if( !Item.ItemUguale(it1, it2.codice, it2.unita /*, it2.descrizione*/) && (soloDistinta == false) )		// Verifica i codici (trascura la descrizione)
					{
					uguali = false;
					}
				else if(conDistinta && it1.haDistinta && it2.haDistinta)	// Se richiesto il confronto delle distinte
					{
					if(it1._dist.Count != it2._dist.Count)					// Le distinte devono avere la stessa lunghezza
						{
						uguali = false;
						}
					else
						{
						it1.OrdinaDistinta();								// Ordina le due distinte (anche se dovrebbero già esserlo)
						it2.OrdinaDistinta();
						
						// Devono avere gli stessi codici (nello stesso ordine, sono già ordinate)
						// Ciclo su una delle due distinte (stessa lunghezza), finché risultano uguali
						for(int i=0; (i < it1._dist.Count) && (uguali == true); i++)				
							{
							Relazione r1 = it1[i];
							Relazione r2 = it2[i];
							if( (r1.codice != r2.codice) || (r1.quantità != r2.quantità) )
								{
								uguali = false;
								}
							}
						}
					}
				return uguali;
				}

			#endregion
			
			/**********************************************************************************************/

			#region CTOR
			public Item(string codice, string descrizione = "", Unità unità = Unità.n)
				{
				if(_items.Exists( x => x.codice == codice))
					{
					throw new DbException($"Codice {codice} esistente", DbException.TipoErr.Codice_Duplicato);
					}
				_cod = codice;
				_des = descrizione;
				_dist = null;
				ordinata = false;
				_unit = unità;
				_items.Add(this);
				}
			#endregion

			/**********************************************************************************************/

			#region PROPRIETÀ
			public string codice
				{
				get { return _cod; }
				}
			public string descrizione
				{
				get { return _des; }
				}
			public List<Relazione> distinta
				{
				get { return _dist; }
				}
			public bool haDistinta
				{
				get
					{
					bool ret = false;
					if(_dist != null)
						{
						if(_dist.Count > 0)
							{
							ret = true;
							}
						}
					return ret;
					}
				}
			public Unità unita
				{
				get {return _unit; }
				}
			public int contaDistinta
				{
				get
					{
					if(_dist != null)
						return _dist.Count;
					else
						return 0;
					}
				}
			#endregion

			/**********************************************************************************************/

			/// <summary>
			/// Implementa funzione di IComparable<>
			/// Confronta i codici (non le descrizioni)
			/// </summary>
			/// <param name="that"></param>
			/// <returns></returns>
			public int CompareTo(Item that)
				{
				if (that == null)		return 1;
				return(this._cod.CompareTo(that._cod));
				}

			/// <summary>
			/// Enumeratore degli item in distinta
			/// </summary>
			/// <returns></returns>
			public IEnumerable<Item> Items()
			{
			if(_dist != null)
				{
				foreach (Relazione r in _dist)
					yield return r.item;
				}
			yield break;
			}

			/// <summary>
			/// Enumeratore delle Relazioni in distinta
			/// </summary>
			/// <returns></returns>
			public IEnumerable<Relazione> Relazioni()
			{
			if(_dist != null)
				{
				foreach (Relazione r in _dist)
					yield return r;
				}
			yield break;
			}
			
			/// <summary>
			/// ToString() con argomento
			/// </summary>
			/// <param name="conDistinta">Con distinta primo livello</param>
			/// <returns></returns>
			public string ToString(bool conDistinta)
				{
				StringBuilder strb = new StringBuilder();
				strb.Append(codice + SEP + descrizione + SEP + unita.ToString() + (haDistinta ? SEP+"[DB]" : ""));
				if(conDistinta)
					{
					strb.AppendLine();
					foreach(Relazione r in Relazioni())
						{
						strb.AppendLine(SEP + r.ToString());
						}
					}
				return strb.ToString();
				}

			/// <summary>
			/// ToString() override
			/// </summary>
			/// <returns></returns>
			public override string ToString()
				{
				return ToString(false);
				}

			/// <summary>
			/// Accesso alle relazioni della distinta per indice
			/// </summary>
			/// <param name="i">Indice</param>
			/// <returns></returns>
			/// <exception cref="DbException">Indice errato o distinta vuota</exception>
			public Relazione this[int i]
				{
				get
					{
					Relazione r = null;
					if(_dist != null)
						{
						if( (i >= 0) && (i < this._dist.Count))
							{
							r = _dist[i];
							}
						else
							{
							if(i < 0)
								throw new DbException($"Ricerca nella distinta {_cod} con indice {i} negativo",DbException.TipoErr.Indice_errato);
							else
								{
								throw new DbException($"Ricerca nella distinta {_cod} con indice {i} oltre la lunghezza {_dist.Count} della distinta",DbException.TipoErr.Indice_errato);
								}
							}
						}
					else
						{
						throw new DbException($"Il codice {_cod} non ha distinta",DbException.TipoErr.Distinta_nulla);
						}

					return r;
					}
				}

			/// <summary>
			/// Restituisce un item della distinta, per indice
			/// </summary>
			/// <param name="i"></param>
			/// <returns></returns>
			public Item DbItem(int i)
				{
				Item it = null;
				Relazione r = this[i];
				if(r != null)
					{
					it = r.item;
					}
				return it;
				}

			/// <summary>
			/// Restituisce la quantità di un item della distinta, per indice
			/// </summary>
			/// <param name="i"></param>
			/// <returns></returns>
			/// <exception cref="DbException"></exception>
			public int DbQta(int i)
				{
				int qta = 0;
				Relazione r = this[i];
				if(r != null)
					{
					qta = r.quantità;
					}
				return qta;
				}		

			/// <summary>
			/// Ordina la distinta delle relazioni
			/// </summary>
			public void OrdinaDistinta()
				{
				if(_dist != null)
					{
					if(!ordinata)
						{
						_dist.Sort();	// Ordina la List<Relazione> (Relazione è IComparable<>)
						ordinata = true;
						}
					}
				}
	
			/// <summary>
			/// Aggiunge una linea di distinta
			/// </summary>
			/// <param name="codice"></param>
			/// <param name="quantità"></param>
			public void AggiungeLineaDb(string codice, int quantità = 1)
				{
				Item item =_items.Find( x => x.codice == codice);	// Cerca l'item con il codice figlio, da inserire nella distinta di this
				if(item != null)
					{
					if(item.ListaCodici(true).Contains(_cod))		// Se la distinta del codice figlio contiene il codice di this
						{
						throw new DbException($"Il codice {codice} non può essere figlio del codice {_cod} perché lo contiene già", DbException.TipoErr.Dipendenza_Ciclica);
						}
					if (_dist == null)							// Se la distinta non esiste ancora, la crea
						{
						_dist = new List<Relazione>();
						ordinata = true;
						}
					
					Relazione r = _dist.Find(x => x.codice == codice);		// Cerca il codice della distinta
					if(r != null)		
						{
						r.quantità += quantità;								// Se c'é già, somma le quantità
						}
					else
						{
						_dist.Add(new Relazione(codice, quantità));			// Altrimenti aggiunge
						ordinata = false;
						}
					}
				else
					{
					throw new DbException($"Il codice {codice} non esiste", DbException.TipoErr.Codice_Inesistente);
					}
				}

			/// <summary>
			/// Cancella la distinta base
			/// </summary>
			public void EliminaDb()
				{
				_dist.Clear();
				_dist = null;
				ordinata = false;
				}

			/// <summary>
			/// Per la scelta del tipo item
			/// </summary>
			public enum TipoCodice		{ SenzaDb, ConDb, Tutti }

			/// <summary>
			/// Lista di tutti i codici (senza ripetizioni) contenuti in distinta
			/// </summary>
			/// <param name="conDescrizione">Aggiunge la descrizione</param>
			/// <param name="tp">Sceglie i codici da elencare</param>
			/// <returns></returns>
			public List<string> ListaCodici(bool conDescrizione = false, TipoCodice tp = TipoCodice.Tutti)
				{
				List<string> codici = new List<string>();		// Lista dei codici trovati
				Queue<Item> items = new Queue<Item>();			// Per la ricerca dei codici (Queue o Stack)
				items.Enqueue(this);
				while(items.Count > 0)
					{
					Item top = items.Dequeue();
					switch(tp)
						{
						case TipoCodice.SenzaDb:
							if(!top.haDistinta)
								codici.Add(top.codice + (conDescrizione ? '\t' + top.descrizione : string.Empty));
							break;
						case TipoCodice.ConDb:
							if(top.haDistinta)
								codici.Add(top.codice + (conDescrizione ? '\t' + top.descrizione : string.Empty));
							break;
						default:
							codici.Add(top.codice + (conDescrizione ? '\t' + top.descrizione : string.Empty));
							break;
						}
					
					if(top.haDistinta)
						{
						foreach(Relazione r in top._dist)
							{
							Item itm = r.item;
							if(itm != null)
								{
								items.Enqueue(itm);
								}
							}
						}
					}
				List<string> tmp = codici.Distinct().ToList();
				return tmp;
				}

			/// <summary>
			/// Distinta esplosa di tutti i codici, con la profondità
			/// </summary>
			/// <returns></returns>
			public List<string> DistintaEsplosa(bool conDescrizione)
				{
				List<string> righe = new List<string>();
				Stack<Tuple<Relazione, int>> items = new Stack<Tuple<Relazione, int>>(); // Coda con profondità
				items.Push(new Tuple<Relazione, int>(new Relazione(this.codice), 0));

				while(items.Count > 0)
					{
					Tuple<Relazione, int> top = items.Pop();

					righe.Add(new string('\t',top.Item2) + (top.Item1.codice) + '\t' + top.Item1.item.unita + ' ' + top.Item1.quantità + (conDescrizione ? '\t' + (top.Item1.item.descrizione): "" ));
					if(top.Item1.item.haDistinta)
						{
						foreach (Relazione r in top.Item1.item._dist)
							{
							Item itm = r.item;
							if (itm != null)
								{
								items.Push(new Tuple<Relazione, int>(new Relazione(itm.codice, r.quantità),top.Item2+1));
								}
							}
						}
					}
				return righe;
				}

			/// <summary>
			/// Distinta raggruppata con i livelli
			/// </summary>
			/// <returns></returns>
			public List<string> DistintaRaggruppata(bool conDescrizione)
				{
				List<string> righe = new List<string>();
				Queue<Tuple<Relazione, int>> items = new Queue<Tuple<Relazione, int>>(); // Coda con profondità
				//items.Enqueue(new Tuple<Item, int>(this, 0));
				items.Enqueue(new Tuple<Relazione, int>(new Relazione(this.codice), 0));

				while (items.Count > 0)
					{
					Tuple<Relazione, int> top = items.Dequeue();
					if (top.Item1.item.haDistinta)
						{
						righe.Add($"[{top.Item2.ToString()}] --- {top.Item1.item.unita} {top.Item1.quantità}\t{top.Item1.codice}{ (conDescrizione ? '\t' + top.Item1.item.descrizione : string.Empty)}:");
						foreach (Relazione r in top.Item1.item._dist)
							{
							Item itm = Item.GetItem(r.codice);
							if (itm != null)
								{
								righe.Add(itm.unita + " " + r.quantità + '\t' + itm.codice + (conDescrizione ? '\t' + itm.descrizione : string.Empty));
								if(itm.haDistinta)
									{
									//items.Enqueue(new Tuple<Relazione, int>(itm, top.Item2 + 1));
									items.Enqueue(new Tuple<Relazione, int>(new Relazione(itm.codice, r.quantità),top.Item2+1));
									}
								}
							}
						}
					}
				return righe;
				}
			
			/// <summary>
			/// Distinta monolivello
			/// </summary>
			/// <param name="conDescrizione">Aggiunge la descrizione</param>
			/// <returns></returns>
			public List<string> DistintaMonolivello(bool conDescrizione = false)
				{
				List<string> righe = new List<string>();

				List<Relazione> rels = ListaCodiciMonolivello();	// Lista codici monolivello

				foreach(Relazione r in rels)
					{
					righe.Add((r.codice) + '\t' + r.item.unita + ' ' + r.quantità + (conDescrizione ? '\t' + (r.item.descrizione): "" ));
					}
				return righe;
				}

			/// <summary>
			/// Crea la distinta monolivello (codice di this, ma con estensione per l'esploso)
			/// </summary>
			/// <returns></returns>
			public Item CreaDistintaMonolivello()
				{
				List<Relazione> rels = ListaCodiciMonolivello();	// Lista codici monolivello
				string codExp = _cod + EXPM;						// Ricava il codice della distinta esplosa
				Item.Remove(codExp);								// Elimina un eventuale codice preesistente
				Item itExp =new Item(codExp, _des);					// Crea il nuovo item
				foreach(Relazione r in rels)
					{
					itExp.AggiungeLineaDb(r.codice,r.quantità);
					}
				return Item.GetItem(codExp);
				}

			/// <summary>
			/// Crea una lista Relazioni corrispondent alla distinta monolivello
			/// </summary>
			/// <param name="conAssiemi"></param>
			/// <returns>List<Relazione></returns>
			private List<Relazione> ListaCodiciMonolivello()
				{

				List<Relazione> dbex = new List<Relazione>();		// Distinta esplosa
				Queue<Tuple<string,Relazione>> rels = new Queue<Tuple<string,Relazione>>();		// Coda con codice padre e Relazioni 

				foreach(Relazione r in _dist)						// Percorre le relazioni della radice
					{
					rels.Enqueue(new Tuple<string,Relazione>(this.codice,r));		// Aggiunge relazioni e codice della radice
					}

				// Ciclo di ricerca
				while(rels.Count > 0)								// Ripete finché la coda non è vuota
					{
					Tuple<string,Relazione> top = rels.Dequeue();	// Toglie dalla coda la prima Tupla, contenente:...
					string padre = top.Item1;						// ...codice padre
					Relazione rtop = top.Item2;						// ...Relazione (linea di distinta)

					if(rtop.item.haDistinta)					// Se la Relazione fa riferimento ad un Item con distinta...
						{												
						foreach(Relazione r in rtop.item._dist)		// ...trasferisce le relazioni dalla distinta reale a quella padre...
							{					// ...moltiplicando la quantità della sottodistinta per quella dell'oggetto esaminato
							rels.Enqueue(new Tuple<string,Relazione>(padre, new Relazione(r.codice,r.quantità * rtop.quantità)));
							}										// Poi le rimette nella coda.								
						}											
					else
						{										// Altrimenti lo aggiunge alla distinta padre esplosa
						AggiungeRelazione(rtop,dbex);
						}
					}
				return dbex;
				}

			/// <summary>
			/// Aggiunge una relazione ad una lista di relazioni, conteggiando le quantità
			/// </summary>
			/// <param name="r">Relazione da aggiungere</param>
			/// <param name="l">Lista di relazioni (distinta)</param>
			private void AggiungeRelazione(Relazione r, List<Relazione> l)
				{
				Relazione f = l.Find(x => x.codice == r.codice);	// Cerca se il codice esiste già nella lista (default(T) se non trovato)
				if(f == null)										// default(T) di una classe è null, se non è definito
					{												// Se non l'ha trovato...
					l.Add(r);										// ...la aggiunge 
					}
				else
					{												// Altrimenti...
					f.quantità += r.quantità;						// ...somma la quantità a quello che c'é già
					}
				}
			}

		}

	}
