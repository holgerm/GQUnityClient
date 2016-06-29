using System.IO;
using System;
using System.Collections.Generic;
using GQ.Util;

namespace GQ.Build {

	public class Product {

		protected const string APP_ICON_FILE_NAME = "AppIcon.png";

		protected string _id;

		public string Id {
			get {
				return _id;
			}
			set {
				_id = value;
			}
		}

		public string AppIconPath {
			get {
				return (Dir + Files.PATH_ELEMENT_SEPARATOR + APP_ICON_FILE_NAME);
			}
		}

		public string Dir {
			get {
				return (ProductManager.ProductsDirPath + Id);
			}
		}

		/// <summary>
		/// Initializes a new Product instance with the given id and a complete set of default values. 
		/// The new product will reside at the project managers default location.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public Product (string id) {
			this._id = id;
		}

		public override string ToString () {
			return string.Format("product {0}", Id);
		}
	}
}