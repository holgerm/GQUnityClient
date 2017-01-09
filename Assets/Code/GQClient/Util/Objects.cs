
namespace GQ.Client.Util {

	public class Objects {

		/// <summary>
		/// GeoQuest Utility class for extensions to all objects.
		/// </summary>
		/// <returns>The string.</returns>
		/// <param name="obj">Object.</param>
		public static string ToString (object obj) {
			if ( obj != null )
				return obj.ToString();
			else
				return "[null]";
			
		}

	}
}
