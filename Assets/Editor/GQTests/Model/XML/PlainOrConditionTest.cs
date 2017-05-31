using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model.XML;

namespace GQTests.Model.XML
{

	public class PlainOrConditionTest : XMLTest
	{

		protected override string XmlRoot {
			get {
				return "or";
			}
		}

		[Test]
		public void Empty ()
		{
			// Act:
			OrCondition condition = parseXML<OrCondition> 
				(@"	<or>
					</or>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Single_EQ_True ()
		{
			// Act:
			OrCondition condition = parseXML<OrCondition> 
				(@"	<or>
						<eq>
							<bool>true</bool>
							<bool>true</bool>
						</eq>
					</or>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Single_LT_True ()
		{
			// Act:
			OrCondition condition = parseXML<OrCondition> 
				(@"	<or>
						<lt>
							<num>10</num>
							<num>10.8</num>
						</lt>
					</or>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Single_GT_False ()
		{
			// Act:
			OrCondition condition = parseXML<OrCondition> 
				(@"	<or>
						<gt>
							<num>10</num>
							<num>10.8</num>
						</gt>
					</or>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Mixed_T_F_F_F_True ()
		{
			// Act:
			OrCondition condition = parseXML<OrCondition> 
				(@"	<or>
						<leq>
							<num>10.8</num>
							<num>10.9</num>
						</leq>
						<eq>
							<bool>true</bool>
							<bool>false</bool>
						</eq>
						<lt>
							<string>zora</string>
							<string>rote</string>
						</lt>
						<eq>
							<num>10</num>
							<num>100</num>
						</eq>
					</or>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void Mixed_F_F_T_False ()
		{
			// Act:
			OrCondition condition = parseXML<OrCondition> 
				(@"	<or>
						<eq>
							<num>10</num>
							<num>100</num>
						</eq>
						<eq>
							<bool>true</bool>
							<bool>false</bool>
						</eq>
						<lt>
							<string>rote</string>
							<string>zora</string>
						</lt>
					</or>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void Mixed_F_F_F_F_False ()
		{
			// Act:
			OrCondition condition = parseXML<OrCondition> 
				(@"	<or>
						<eq>
							<num>10</num>
							<num>100</num>
						</eq>
						<eq>
							<bool>true</bool>
							<bool>false</bool>
						</eq>
						<gt>
							<string>rote</string>
							<string>zora</string>
						</gt>
						<geq>
							<num>10.8</num>
							<num>10.9</num>
						</geq>
					</or>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

	}
}
