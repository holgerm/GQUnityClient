using NUnit.Framework;
using Code.GQClient.Conf;
using Code.GQClient.UI.layout;

namespace GQTests.Layout
{

	public class HeightTests
	{

		[Test]
		public void FooterHeightAdaptToMin ()
		{

			// prepare product json:
			Config.RetrieveProductJSONText = () => {
				return 
					@"{
							""id"": ""test"",
							""headerHeightUnits"": 60.0,
			 				""contentHeightUnits"": 750.0,
			 	 			""footerHeightUnits"": 75.0,
			  				""footerHeightMinMM"": 40.0,
			  				""footerHeightMaxMM"": 0.0
					}";
			};

			DeviceDefinitions.Use (DeviceDefinitions.DeviceType.iPad4);

			// we have a totoal of 60 + 750 +75 = 885 units defined.
			Assert.That (LayoutConfig.ScreenHeightUnits, Is.EqualTo (885f));
			// the device has a tital screen height of (2048p / 264p/inch) * 25,4inch/mm = 197mm.
			// we defined for the footer height a minimum of 40mm, i.e. 20.3% of the total screen height.
			// we expect the footer height to use 20.3% ot the tital units, i.e. 179.7 units.

			// we expect the resulting footer height to be between 39 and 41mm, as we rectricted it to be at least 40mm:
			Assert.That (LayoutConfig.Units2MM (LayoutConfig.FooterHeightUnits), Is.InRange (39f, 41f));

			// the apadated heights should still sum up to the same total height:
			Assert.That (LayoutConfig.FooterHeightUnits + LayoutConfig.ContentHeightUnits + LayoutConfig.HeaderHeightUnits, Is.EqualTo (LayoutConfig.ScreenHeightUnits));
		}


		[Test]
		public void FooterHeightAdaptToMax ()
		{

			// prepare product json:
			Config.RetrieveProductJSONText = () => {
				return 
					@"{
							""id"": ""test"",
							""headerHeightUnits"": 60.0,
			 				""contentHeightUnits"": 750.0,
			 	 			""footerHeightUnits"": 75.0,
			  				""footerHeightMaxMM"": 5.0
					}";
			};

			DeviceDefinitions.Use (DeviceDefinitions.DeviceType.iPad4);

			// we expect the resulting footer height to be between 4.5 and 5.5mm, as we rectricted it to be at most 5mm:
			Assert.That (LayoutConfig.Units2MM (LayoutConfig.FooterHeightUnits), Is.InRange (4.5f, 5.5f));

			// the apadated heights should still sum up to the same total height:
			Assert.That (LayoutConfig.FooterHeightUnits + LayoutConfig.ContentHeightUnits + LayoutConfig.HeaderHeightUnits, Is.EqualTo (LayoutConfig.ScreenHeightUnits));
		}

		[Test]
		public void HeaderHeightAdaptToMin ()
		{

			// prepare product json:
			Config.RetrieveProductJSONText = () => {
				return 
					@"{
							""id"": ""test"",
							""headerHeightUnits"": 60.0,
			 				""contentHeightUnits"": 750.0,
			 	 			""footerHeightUnits"": 75.0,
			  				""headerHeightMinMM"": 40.0,
			  				""headerHeightMaxMM"": 0.0
					}";
			};

			DeviceDefinitions.Use (DeviceDefinitions.DeviceType.iPad4);

			// we have a totoal of 60 + 750 +75 = 885 units defined.
			Assert.That (LayoutConfig.ScreenHeightUnits, Is.EqualTo (885f));
			// the device has a total screen height of (2048p / 264p/inch) * 25,4inch/mm = 197mm.
			// we defined for the header height a minimum of 40mm, i.e. 20.3% of the total screen height.
			// we expect the header height to use 20.3% ot the tital units, i.e. 179.7 units.

			// we expect the resulting footer height to be between 39 and 41mm, as we rectricted it to be at least 40mm:
			Assert.That (LayoutConfig.Units2MM (LayoutConfig.HeaderHeightUnits), Is.InRange (39f, 41f));

			// the apadated heights should still sum up to the same total height:
			Assert.That (LayoutConfig.FooterHeightUnits + LayoutConfig.ContentHeightUnits + LayoutConfig.HeaderHeightUnits, Is.EqualTo (LayoutConfig.ScreenHeightUnits));
		}

		[Test]
		public void HeaderHeightAdaptToMax ()
		{

			// prepare product json:
			Config.RetrieveProductJSONText = () => {
				return 
					@"{
							""id"": ""test"",
							""headerHeightUnits"": 60.0,
			 				""contentHeightUnits"": 750.0,
			 	 			""footerHeightUnits"": 75.0,
			  				""headerHeightMaxMM"": 5.0
					}";
			};

			DeviceDefinitions.Use (DeviceDefinitions.DeviceType.iPad4);

			// we expect the resulting footer height to be between 4.5 and 5.5mm, as we rectricted it to be at most 5mm:
			Assert.That (LayoutConfig.Units2MM (LayoutConfig.HeaderHeightUnits), Is.InRange (4.5f, 5.5f));

			// the apadated heights should still sum up to the same total height:
			Assert.That (LayoutConfig.FooterHeightUnits + LayoutConfig.ContentHeightUnits + LayoutConfig.HeaderHeightUnits, Is.EqualTo (LayoutConfig.ScreenHeightUnits));
		}

		[Test]
		public void HeaderAndFooterHeightAdaptToMin ()
		{

			// prepare product json:
			Config.RetrieveProductJSONText = () => {
				return 
					@"{
							""id"": ""test"",
							""headerHeightUnits"": 60.0,
			 				""contentHeightUnits"": 750.0,
			 	 			""footerHeightUnits"": 75.0,
			  				""headerHeightMinMM"": 40.0,
			  				""footerHeightMinMM"": 40.0
					}";
			};

			DeviceDefinitions.Use (DeviceDefinitions.DeviceType.iPad4);

			// we have a totoal of 60 + 750 +75 = 885 units defined.
			Assert.That (LayoutConfig.ScreenHeightUnits, Is.EqualTo (885f));
			// the device has a total screen height of (2048p / 264p/inch) * 25,4inch/mm = 197mm.
			// we defined for the header height a minimum of 40mm, i.e. 20.3% of the total screen height.
			// we expect the header height to use 20.3% ot the tital units, i.e. 179.7 units.

			// we expect the resulting footer and header heights to be between 39 and 41mm, as we rectricted it to be at least 40mm:
			Assert.That (LayoutConfig.Units2MM (LayoutConfig.HeaderHeightUnits), Is.InRange (39f, 41f));
			Assert.That (LayoutConfig.Units2MM (LayoutConfig.FooterHeightUnits), Is.InRange (39f, 41f));
			// while the content can only take the reminaing height of ca. 197mm - 2 * 40mm = 117mm:
			Assert.That (LayoutConfig.Units2MM (LayoutConfig.ContentHeightUnits), Is.InRange (116f, 118f));

			// the apadated heights should still sum up to the same total height:
			Assert.That (LayoutConfig.FooterHeightUnits + LayoutConfig.ContentHeightUnits + LayoutConfig.HeaderHeightUnits, Is.EqualTo (LayoutConfig.ScreenHeightUnits));
		}

		[Test]
		public void HeaderAndFooterHeightAdaptToMax ()
		{

			// prepare product json:
			Config.RetrieveProductJSONText = () => {
				return 
					@"{
							""id"": ""test"",
							""headerHeightUnits"": 60.0,
			 				""contentHeightUnits"": 750.0,
			 	 			""footerHeightUnits"": 75.0,
			  				""headerHeightMaxMM"": 5.0,
			  				""footerHeightMaxMM"": 5.0
					}";
			};

			DeviceDefinitions.Use (DeviceDefinitions.DeviceType.iPad4);

			// we expect the resulting footer and header height to be between 4.5 and 5.5mm, as we rectricted it to be at most 5mm:
			Assert.That (LayoutConfig.Units2MM (LayoutConfig.HeaderHeightUnits), Is.InRange (4.5f, 5.5f));
			Assert.That (LayoutConfig.Units2MM (LayoutConfig.FooterHeightUnits), Is.InRange (4.5f, 5.5f));
			// and the content to take the rest of 197mm - 2 * 5mm = 187mm:
			Assert.That (LayoutConfig.Units2MM (LayoutConfig.ContentHeightUnits), Is.InRange (186f, 188f));

			// the apadated heights should still sum up to the same total height:
			Assert.That (LayoutConfig.FooterHeightUnits + LayoutConfig.ContentHeightUnits + LayoutConfig.HeaderHeightUnits, Is.EqualTo (LayoutConfig.ScreenHeightUnits));
		}

		[Test]
		public void AdaptHeightsOfHeaderToMaxAndFooterToMin ()
		{

			// prepare product json:
			Config.RetrieveProductJSONText = () => {
				return 
					@"{
							""id"": ""test"",
							""headerHeightUnits"": 60.0,
			 				""contentHeightUnits"": 750.0,
			 	 			""footerHeightUnits"": 75.0,
			  				""headerHeightMaxMM"": 5.0,
			  				""footerHeightMinMM"": 40.0
					}";
			};

			DeviceDefinitions.Use (DeviceDefinitions.DeviceType.iPad4);

			// we expect the resulting footer and header height to be between 4.5 and 5.5mm, as we rectricted it to be at most 5mm:
			Assert.That (LayoutConfig.Units2MM (LayoutConfig.HeaderHeightUnits), Is.InRange (4.5f, 5.5f));
			Assert.That (LayoutConfig.Units2MM (LayoutConfig.FooterHeightUnits), Is.InRange (39.5f, 40.5f));
			// and the content to take the rest of 197mm - (5mm + 40mm) = 152mm:
			Assert.That (LayoutConfig.Units2MM (LayoutConfig.ContentHeightUnits), Is.InRange (151.5f, 152.5f));

			// the apadated heights should still sum up to the same total height:
			Assert.That (LayoutConfig.FooterHeightUnits + LayoutConfig.ContentHeightUnits + LayoutConfig.HeaderHeightUnits, Is.EqualTo (LayoutConfig.ScreenHeightUnits));
		}


		[Test]
		public void HeightOfMapButton ()
		{

			// prepare product json:
			Config.RetrieveProductJSONText = () => {
				return 
					@"{
							""id"": ""test"",
							""headerHeightUnits"": 60.0,
			 				""contentHeightUnits"": 750.0,
			 	 			""footerHeightUnits"": 75.0,
			  				""mapButtonHeightUnits"": 100.0
					}";
			};

			DeviceDefinitions.Use (DeviceDefinitions.DeviceType.iPad4);

			// we expect the resulting map button to be 100 units high, i.e. a share of 100 / 885 = 11,3% of 197mm is ca. 22.26mm:
			Assert.That (LayoutConfig.Units2MM (FoyerMapScreenLayout.MapButtonHeightUnits), Is.InRange (22.24f, 22.28f));
		}

	}
}