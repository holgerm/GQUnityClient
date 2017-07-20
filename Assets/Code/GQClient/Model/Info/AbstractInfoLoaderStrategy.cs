using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GQ.Client.Model.Info {

	public abstract class AbstractInfoLoaderStrategy {

		public abstract IEnumerator<QuestInfo> GetEnumerator ();
	}

}
