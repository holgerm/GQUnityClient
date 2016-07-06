
namespace GQ.Client.Util {

	public class Objects {

		public static string ToString (object obj) {
			if ( obj != null )
				return obj.ToString();
			else
				return "[null]";
			
		}

	}
}
