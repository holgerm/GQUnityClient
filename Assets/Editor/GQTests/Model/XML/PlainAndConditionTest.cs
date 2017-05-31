using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model.XML;

namespace GQTests.Model.XML
{

	public class PlainAndConditionTest : XMLTest
	{

		protected override string XmlRoot {
			get {
				return "and";
			}
		}

		[Test]
		public void Empty ()
		{
			// Act:
			AndCondition condition = parseXML<AndCondition> 
				(@"	<and>
					</and>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Single_EQ_True ()
		{
			// Act:
			AndCondition condition = parseXML<AndCondition> 
				(@"	<and>
						<eq>
							<bool>true</bool>
							<bool>true</bool>
						</eq>
					</and>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Single_LT_True ()
		{
			// Act:
			AndCondition condition = parseXML<AndCondition> 
				(@"	<and>
						<lt>
							<num>10</num>
							<num>10.8</num>
						</lt>
					</and>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Single_GT_False ()
		{
			// Act:
			AndCondition condition = parseXML<AndCondition> 
				(@"	<and>
						<gt>
							<num>10</num>
							<num>10.8</num>
						</gt>
					</and>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Mixed_4_True ()
		{
			// Act:
			AndCondition condition = parseXML<AndCondition> 
				(@"	<and>
						<leq>
							<num>10.8</num>
							<num>10.9</num>
						</leq>
						<eq>
							<bool>true</bool>
							<bool>true</bool>
						</eq>
						<gt>
							<string>zora</string>
							<string>rote</string>
						</gt>
						<eq>
							<num>10</num>
							<num>10</num>
						</eq>
					</and>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Mixed_T_F_T_F_False ()
		{
			// Act:
			AndCondition condition = parseXML<AndCondition> 
				(@"	<and>
						<eq>
							<num>10</num>
							<num>10</num>
						</eq>
						<leq>
							<num>12</num>
							<num>10.9</num>
						</leq>
						<eq>
							<bool>true</bool>
							<bool>true</bool>
						</eq>
						<gt>
							<string>rote</string>
							<string>zora</string>
						</gt>
					</and>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Mixed_T_T_F_False ()
		{
			// Act:
			AndCondition condition = parseXML<AndCondition> 
				(@"	<and>
						<eq>
							<num>10</num>
							<num>10</num>
						</eq>
						<eq>
							<bool>true</bool>
							<bool>true</bool>
						</eq>
						<gt>
							<string>rote</string>
							<string>zora</string>
						</gt>
					</and>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}
	}
}
