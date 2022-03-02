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

	/// <summary>
	/// Eccezione specifica per la distinta base
	/// </summary>
	public class DbException : Exception
		{

		public enum TipoErr	{
								Codice_Duplicato,
								Codice_Inesistente,
								Dipendenza_Ciclica,
								Codice_Libero_Non_Trovato,
								Codici_non_corrispondenti,
								Distinte_non_corrispondenti,
								Distinta_nulla,
								Indice_errato
							};

		TipoErr _err;

		public TipoErr Errore
			{
			get { return _err; }
			}

		public DbException()
			{
			}

		public DbException(string message)
			: base(message)
			{
			}

		public DbException(string message, Exception inner)
			: base(message, inner)
			{
			}

		public DbException(string message, TipoErr tipo)
			: base(message)
			{
			_err = tipo;
			}
		}

	}
