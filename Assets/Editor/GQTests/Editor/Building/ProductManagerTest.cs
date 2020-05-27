using NUnit.Framework;
using GQ.Editor.Building;
using System.IO;
using UnityEngine;
using System;
using GQ.Editor.Util;

namespace GQTests.Editor.Building {

	public class ProductManagerTest {

		protected string PRODUCTS_TEST_DIR = GQAssert.TEST_DATA_BASE_DIR + "TestProducts/";

		ProductManager testPM, prodPM;

		[SetUp]
		public void initPM () {
			string newProductsDir = Files.CombinePath(PRODUCTS_TEST_DIR, "NewProducts");
			if ( !Files.ExistsDir(newProductsDir) )
				Files.CreateDir(newProductsDir);

			string dirWithEmptyProductList = Files.CombinePath(PRODUCTS_TEST_DIR, "ProductListEmpty");
			if ( !Files.ExistsDir(dirWithEmptyProductList) )
				Files.CreateDir(dirWithEmptyProductList);

			string configResourcesDir = Files.CombinePath(PRODUCTS_TEST_DIR, "Output/ConfigAssets/Resources");
			if ( !Files.ExistsDir(configResourcesDir) )
				Files.CreateDir(configResourcesDir);
			
			ProductManager._dispose();
			testPM = ProductManager.TestInstance;
			prodPM = ProductManager.Instance;
		}

		[TearDown]
		public void clean () {
			GQ.Editor.Util.Assets.ClearAssetFolder(testPM.BuildExportPath);
			ProductManager._dispose();
		}

		[Test]
		public void InitStandardPM () {
			// Arrange:

			// Act:
			var pm = ProductManager.Instance;

			// Assert:
			Assert.AreEqual(ProductManager.ProductsDirPath, ProductManager.ProductsDirPath);
			Assert.AreEqual(0, pm.Errors.Count, pm.Errors.Count > 0 ? "Unexpected errors. The first is: " + pm.Errors[0].ToString() : "No errors as expected.");
		}

		[Test]
		public void InitTestPM () {
			// Arrange:
			ProductManager pm = null;

			// Act:
			ProductManager.ProductsDirPath = PRODUCTS_TEST_DIR;
			pm = ProductManager.Instance;

			// Assert:
			Assert.IsNotNull(pm);
			Assert.AreEqual(PRODUCTS_TEST_DIR, ProductManager.ProductsDirPath);
			Assert.AreEqual(0, pm.AllProducts.Count);
		}

		[Test]
		public void CreateNewProduct () {
			// Arrange:
			string testDir = Files.CombinePath(PRODUCTS_TEST_DIR, "NewProducts");
			if ( !Files.ExistsDir(testDir) )
				Files.CreateDir(testDir);
			Files.ClearDir(testDir);


			ProductManager.ProductsDirPath = testDir;
			ProductManager prodPM = ProductManager.Instance;
			string testProductID = "testProduct";

			// Act:
			prodPM.createNewProduct(testProductID);

			///////////////////////////////////
			// Assert:
			Assert.AreEqual(1, prodPM.AllProducts.Count);
			ProductSpec product = prodPM.GetProduct(testProductID);
			Assert.AreEqual(testProductID, product.Id);
			Assert.That(Directory.Exists(product.Dir), "Product dir should be ok for product " + product);

			// Branding files:
			Assert.That(File.Exists(product.AppIconPath), "App icon file should exist at " + product.AppIconPath);
			Assert.That(File.Exists(product.SplashScreenPath), "Splashscreen file should exist at " + product.SplashScreenPath);
			Assert.That(File.Exists(product.TopLogoPath), "Top logo file should exist at " + product.TopLogoPath);
			Assert.That(
				product.IsValid(), 
				"Newly created product " + product.Id + " is not valid (" +
				product.Errors.Count + " errors):\n" + product.AllErrorsAsString()
			);

			// TODO Loading Canvas (copy default when creating new product)

			// Config file:
			Assert.That(File.Exists(product.ConfigPath), "Config file should exist at " + product.ConfigPath);
			Assert.AreEqual(testProductID, product.Id);
			Assert.AreEqual(testProductID, product.Config.id);

			// Clean:
			Files.ClearDir(testDir);
		}

		[Test]
		public void EmptyProductList () {
			// Act:
			ProductManager.ProductsDirPath = PRODUCTS_TEST_DIR + "ProductListEmpty";
			ProductManager pm = ProductManager.Instance;

			// Assert:
			Assert.AreEqual(0, pm.AllProducts.Count, "Product List should be empty.");
		}

		[Test]
		public void PopulatedProductList () {
			// Act:
			ProductManager.ProductsDirPath = PRODUCTS_TEST_DIR + "ProductListPopulated";
			ProductManager pm = ProductManager.Instance;

			// Assert:
			Assert.AreEqual(4, pm.AllProducts.Count, "Product List should contain the 4 valid products.");
			Assert.That(pm.AllProductIds.Contains("product1"), "product1 missing");
			Assert.That(pm.AllProductIds.Contains("product2"), "product2 missing");
			Assert.That(pm.AllProductIds.Contains("product3"), "product3 missing");
			Assert.That(pm.AllProductIds.Contains("product4"), "product4 missing");
			Assert.AreEqual(0, pm.Errors.Count);
		}

		[Test]
		public void Paths4RealProductManager () {
			// Act:
			// Already done in initPM() on SetUp.

			// Assert:
			Assert.That(prodPM.BuildExportPath, Is.EqualTo("Assets/ConfigAssets/Resources"));
			Assert.That(prodPM.ANDROID_MANIFEST_FILE, Is.EqualTo("Assets/Plugins/Android/AndroidManifest.xml"));
			Assert.That(prodPM.STREAMING_ASSET_PATH, Is.EqualTo("Assets/StreamingAssets"));
		}

		[Test]
		public void Paths4TestProductManager () {
			// Act:
			// Already done in initPM() on SetUp.

			// Assert:
			Assert.That(testPM.BuildExportPath, Is.EqualTo("Assets/Editor/GQTestsData/Output/ConfigAssets/Resources"));
			Assert.That(testPM.ANDROID_MANIFEST_FILE, Is.EqualTo("Assets/Editor/GQTestsData/Output/Plugins/Android/AndroidManifest.xml"));
			Assert.That(testPM.STREAMING_ASSET_PATH, Is.EqualTo("Assets/Editor/GQTestsData/Output/StreamingAssets"));
		}

		private void AssertBuildIsValid (ProductManager pm, string productName) {
			bool isValid = true;
			bool productJSONFound = false;
			bool appIconFound = false;
			bool splashScreenFound = false;
			bool topLogoFound = false;

			// Directory must exist:
			DirectoryInfo productDir = new DirectoryInfo(pm.BuildExportPath);
			isValid &= productDir.Exists;

			// Checking the files in configassets folder:
			FileInfo[] files = productDir.GetFiles();
			foreach ( FileInfo file in files ) {
				// Product.json
				if ( "Product.json".Equals(file.Name) ) {
					productJSONFound = true;
					// TODO do more detailed checks here (e.g. marker images)
					continue;
				}

				// AppIcon.png
				if ( "AppIcon.png".Equals(file.Name) ) {
					appIconFound = true;// TODO do more detailed checks here
					continue;
				}

				// SplashScreen.jpg
				if ( "SplashScreen.jpg".Equals(file.Name) ) {
					splashScreenFound = true;// TODO do more detailed checks here
					continue;
				}

				// TopLogo.jpg
				if ( "TopLogo.jpg".Equals(file.Name) ) {
					topLogoFound = true;// TODO do more detailed checks here
					continue;
				}
			} 

			if ( !productJSONFound ) {
				Assert.Fail("No Product.json file found. (in build of product " + pm.GetProduct(productName).Id + ")");
			}

			if ( !appIconFound ) {
				Assert.Fail("No AppIcon.png file found. (in build of product " + pm.GetProduct(productName).Id + ")");
			}

			if ( !splashScreenFound ) {
				Assert.Fail("No SplashScreen.jpg file found. (in build of product " + pm.GetProduct(productName).Id + ")");
			}

			if ( !topLogoFound ) {
				Assert.Fail("No TopLogo.jpg file found. (in build of product " + pm.GetProduct(productName).Id + ")");
			}


			// check watermark of android manifest (in plugins/android folder):
			Assert.That(File.Exists(pm.ANDROID_MANIFEST_FILE), "No Android Manifest found in build directory.");
			string idFoundInManifest = ProductManager.Extract_ID_FromXML_Watermark(pm.ANDROID_MANIFEST_FILE);
			Assert.AreEqual(productName, idFoundInManifest);
		}

		[Test]
		public void PrepareProductForBuild () {
			// Arrange:
			ProductManager.ProductsDirPath = PRODUCTS_TEST_DIR + "ProductListPopulated";
			testPM = ProductManager.TestInstance;

			if ( Directory.Exists(testPM.BuildExportPath) )
				GQ.Editor.Util.Assets.ClearAssetFolder(testPM.BuildExportPath);
			

			// Act:
			testPM.PrepareProductForBuild("product1");

			// Assert:
			AssertBuildIsValid(testPM, "product1");



			// Act:
			testPM.PrepareProductForBuild("product3");

			// Assert:
			AssertBuildIsValid(testPM, "product3");
		}

		[Test]
		public void PrepareProductWithMarkers () {
			// Arrange:
			ProductManager.ProductsDirPath = PRODUCTS_TEST_DIR + "ProductsWithSubdirsTest";
			testPM = ProductManager.TestInstance;
			GQ.Editor.Util.Assets.ClearAssetFolder(testPM.BuildExportPath);

			// Act:
			testPM.PrepareProductForBuild("productWithMarkers");
			ProductSpec buildProduct = new ProductSpec(testPM.BuildExportPath);

			// Assert:
			Assert.AreEqual("productWithMarkers", buildProduct.Id);
			Assert.That(Directory.Exists(Files.CombinePath(buildProduct.Dir, "markers")), "markers directory missing in product dir: " + buildProduct.Dir);
			Assert.That(File.Exists(Files.CombinePath(buildProduct.Dir, "markers", "marker1.png")), "marker1.png missing in product");
			Assert.That(File.Exists(Files.CombinePath(buildProduct.Dir, "markers", "marker2.png")), "marker2.png missing in product");

			// Act:
			testPM.PrepareProductForBuild("productWithoutMarkers");

			// Assert:
			buildProduct = new ProductSpec(testPM.BuildExportPath);
			Assert.AreEqual("productWithoutMarkers", buildProduct.Id);
			Assert.That(!Directory.Exists(Files.CombinePath(buildProduct.Dir, "markers")), "marker directory should not exist with this product set for build");
		}

		[Test]
		public void PrepareProductWithIgnoredSubdirs () {
			// Arrange:
			ProductManager.ProductsDirPath = PRODUCTS_TEST_DIR + "ProductsWithSubdirsTest";
			testPM = ProductManager.TestInstance;
			GQ.Editor.Util.Assets.ClearAssetFolder(testPM.BuildExportPath);

			// Act:
			testPM.PrepareProductForBuild("productWithIgnoredSubdirs");

			// Assert:
			ProductSpec product = testPM.CurrentProduct;
			Assert.That(product != null);
			Assert.AreEqual("productWithIgnoredSubdirs", product.Id);
			Assert.That(
				!Directory.Exists(Files.CombinePath(testPM.BuildExportPath, "_images")), 
				"Directory _images should not be included in build.");
			Assert.That(
				!Directory.Exists(Files.CombinePath(testPM.BuildExportPath, "_texts")), 
				"Directory _texts should not be included in build.");
			Assert.That(
				Directory.Exists(Files.CombinePath(testPM.BuildExportPath, "images")), 
				"Directory images should be included in build.");
			Assert.That(
				File.Exists(Files.CombinePath(testPM.BuildExportPath, "images", "IncludedImage.png")), 
				"File images/IncludedImage.png should be included in build.");
			Assert.That(
				Directory.Exists(Files.CombinePath(testPM.BuildExportPath, "texts")), 
				"Directory texts should be included in build.");
			Assert.That(
				File.Exists(Files.CombinePath(testPM.BuildExportPath, "texts", "IncludedTextDoc.txt")), 
				"File texts/IncludedTextDoc.txt should be included in build.");
		}


		[Test]
		public void PrepareProductWithStreamingAssets () {
			// Arrange:
			ProductManager.ProductsDirPath = PRODUCTS_TEST_DIR + "ProductsWithStreamingAssets";
			testPM = ProductManager.TestInstance;
			GQ.Editor.Util.Assets.ClearAssetFolder(testPM.BuildExportPath);

			// Act:
			testPM.PrepareProductForBuild("productWithMapTilesAndPredeployedQuests");

			// Assert:
			ProductSpec product = testPM.CurrentProduct;
			Assert.That(product != null);
			Assert.AreEqual("productWithMapTilesAndPredeployedQuests", product.Id);
			Assert.That(
				!Directory.Exists(Files.CombinePath(testPM.BuildExportPath, "StreamingAssets")), 
				"Directory 'StreamingAssets' should NOT be created in build folder.");
			Assert.That(
				File.Exists(Files.CombinePath(testPM.STREAMING_ASSET_PATH, "mapTiles", "14", "8525", "5447.jpg")), 
				"Directory 'StreamingAssets' should contain map files, e.g. at mapTiles/14/8525/5447.jpg");
			Assert.That(
				File.Exists(Files.CombinePath(testPM.STREAMING_ASSET_PATH, "predeployed", "quests", "6088", "game.xml")), 
				"Directory 'StreamingAssets' should contain predeployed quest files, e.g. at predeployed/quests/6088/game.xml");
		}
	}

}
