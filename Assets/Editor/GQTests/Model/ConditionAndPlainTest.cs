using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model;

namespace GQTests.Model
{

	public class ConditionAndPlainTest : GQMLTest
	{

		[SetUp]
		public void Init ()
		{
			XmlRoot = "and";
		}

		[Test]
		public void Empty ()
		{
			// Act:
			ConditionAnd condition = parseXML<ConditionAnd> 
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
			ConditionAnd condition = parseXML<ConditionAnd> 
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
			ConditionAnd condition = parseXML<ConditionAnd> 
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
			ConditionAnd condition = parseXML<ConditionAnd> 
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
			ConditionAnd condition = parseXML<ConditionAnd> 
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
			ConditionAnd condition = parseXML<ConditionAnd> 
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
			ConditionAnd condition = parseXML<ConditionAnd> 
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
