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
		/// Relazione
		/// Il riferimento è all'oggetto item, più efficiente del riferimento al codice
		/// </summary>
		public class Relazione : IComparable<Relazione>
			{
			Item _item;
			int _qta;

			/**********************************************************************************************/

			#region CTOR
			public Relazione()
				{
				_item = null;
				//_cod = string.Empty;
				_qta = 1;
				}

			public Relazione(string codice, int quantità = 1)
				{
				_qta = quantità;
				_item = Item.GetItem(codice);
				}
			#endregion

			/**********************************************************************************************/

			#region PROPRIETÀ
			public string codice
				{
				get { return (_item !=null) ? _item.codice : String.Empty; }
				}
			public Item item
				{ get { return _item; } }
			public int quantità
				{
				get { return _qta; }
				set { _qta = value; }
				}
			#endregion

			/**********************************************************************************************/

			/// <summary>
			/// Implementa l'interfaccia IComparable<>
			/// Confronta gli item (che confrontano solo i codici), non le quantità
			/// </summary>
			/// <param name="that"></param>
			/// <returns></returns>
			public int CompareTo( Relazione that )
				{
				if ( that == null )		return 1;
				if ((this._item == null) || (that._item == null)) return 0;
				return(this._item.CompareTo(that._item));
				}

			/// <summary>
			/// ToString()
			/// </summary>
			/// <returns></returns>
			public override string ToString()
				{
				if(_item == null)
					{
					return String.Empty;
					}
				else
					{
					return _item.ToString() + " " + _qta.ToString();
					}
				}

			}

		}
	}
