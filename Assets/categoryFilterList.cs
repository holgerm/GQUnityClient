using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GQ.Client.Conf;

public class categoryFilterList : MonoBehaviour {

	public List<categoryFilter> filter;

	public categoryFilter prefab;



	public void reInstantiateFilter () {


		if ( filter != null && filter.Count > 0 ) {


			foreach ( categoryFilter cF in filter ) {


				Destroy(cF.gameObject);

			}



		} 

		filter = new List<categoryFilter>();



		foreach ( QuestMetaCategory qmc in Configuration.instance.metaCategoryUsage ) {

			if ( qmc.filterButton ) {


				categoryFilter cF =	Instantiate(prefab);
				cF.transform.SetParent(transform, false);
				cF.transform.localScale = Vector3.one;
				cF.category = qmc;

			}


		}




	}




}
