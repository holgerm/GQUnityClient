using UnityEngine;
using System.Collections;

namespace GQ.Client.Model
{

	public interface IParentedXml {

		I_GQML Parent { get; set; }

	}
}
