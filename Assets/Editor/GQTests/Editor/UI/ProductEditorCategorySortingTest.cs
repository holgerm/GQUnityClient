using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.IO;
using GQ.Editor.UI;
using System.Collections.Generic;
using Code.GQClient.Conf;

namespace GQTests.Editor.UI {

	public class ProductEditorCategorySortingTest {

		[Test]
		public void WithoutFolders() {
			ProductEditorPart4ListOfCategory pe = new ProductEditorPart4ListOfCategory ();

			List<Category> catList = new List<Category> ();
			Category c1 = new Category ("1", "One", "", "");
			catList.Add (c1);
			Category c2 = new Category ("2", "Two", "", "");
			catList.Add (c2);
			Category c3 = new Category ("3", "Three", "", "");
			catList.Add (c3);

			Assert.AreEqual (0, catList.IndexOf (c1));
			Assert.AreEqual (1, catList.IndexOf (c2));
			Assert.AreEqual (2, catList.IndexOf (c3));

			catList = pe.getSortedAccordingToFolders (catList);

			Assert.AreEqual (0, catList.IndexOf (c1));
			Assert.AreEqual (1, catList.IndexOf (c2));
			Assert.AreEqual (2, catList.IndexOf (c3));
		}


		[Test]
		public void WithFoldersAlreadySorted() {
			ProductEditorPart4ListOfCategory pe = new ProductEditorPart4ListOfCategory ();

			List<Category> catList = new List<Category> ();
			Category a1 = new Category ("A1", "One", "A", "");
			catList.Add (a1);
			Category a2 = new Category ("A2", "Two", "A", "");
			catList.Add (a2);
			Category a3 = new Category ("A3", "Three", "A", "");
			catList.Add (a3);
			Category b1 = new Category ("B1", "Four", "B", "");
			catList.Add (b1);
			Category b2 = new Category ("B2", "Five", "B", "");
			catList.Add (b2);
			Category b3 = new Category ("B3", "Six", "B", "");
			catList.Add (b3);

			Assert.AreEqual (0, catList.IndexOf (a1));
			Assert.AreEqual (1, catList.IndexOf (a2));
			Assert.AreEqual (2, catList.IndexOf (a3));
			Assert.AreEqual (3, catList.IndexOf (b1));
			Assert.AreEqual (4, catList.IndexOf (b2));
			Assert.AreEqual (5, catList.IndexOf (b3));

			catList = pe.getSortedAccordingToFolders (catList);

			Assert.AreEqual (0, catList.IndexOf (a1));
			Assert.AreEqual (1, catList.IndexOf (a2));
			Assert.AreEqual (2, catList.IndexOf (a3));
			Assert.AreEqual (3, catList.IndexOf (b1));
			Assert.AreEqual (4, catList.IndexOf (b2));
			Assert.AreEqual (5, catList.IndexOf (b3));
		}


		[Test]
		public void WithFoldersNotYetSorted() {
			ProductEditorPart4ListOfCategory pe = new ProductEditorPart4ListOfCategory ();

			List<Category> catList = new List<Category> ();
			Category a1 = new Category ("A1", "One", "A", "");
			catList.Add (a1);
			Category b1 = new Category ("B1", "Four", "B", "");
			catList.Add (b1);
			Category a2 = new Category ("A2", "Two", "A", "");
			catList.Add (a2);
			Category b2 = new Category ("B2", "Five", "B", "");
			catList.Add (b2);
			Category b3 = new Category ("B3", "Six", "B", "");
			catList.Add (b3);
			Category a3 = new Category ("A3", "Three", "A", "");
			catList.Add (a3);

			Assert.AreEqual (0, catList.IndexOf (a1));
			Assert.AreEqual (1, catList.IndexOf (b1));
			Assert.AreEqual (2, catList.IndexOf (a2));
			Assert.AreEqual (3, catList.IndexOf (b2));
			Assert.AreEqual (4, catList.IndexOf (b3));
			Assert.AreEqual (5, catList.IndexOf (a3));

			catList = pe.getSortedAccordingToFolders (catList);

			Assert.AreEqual (0, catList.IndexOf (a1));
			Assert.AreEqual (1, catList.IndexOf (a2));
			Assert.AreEqual (2, catList.IndexOf (a3));
			Assert.AreEqual (3, catList.IndexOf (b1));
			Assert.AreEqual (4, catList.IndexOf (b2));
			Assert.AreEqual (5, catList.IndexOf (b3));
		}
	}
}