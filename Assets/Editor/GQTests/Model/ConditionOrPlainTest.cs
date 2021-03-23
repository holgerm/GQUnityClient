using Code.GQClient.Model.conditions;
using NUnit.Framework;

namespace GQTests.Model
{

	public class ConditionOrPlainTest : GQMLTest
	{

		[SetUp]
		public void Init ()
		{
			XmlRoot = "or";
		}

		[Test]
		public void Empty ()
		{
			// Act:
			ConditionOr condition = parseXML<ConditionOr> 
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
			ConditionOr condition = parseXML<ConditionOr> 
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
			ConditionOr condition = parseXML<ConditionOr> 
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
			ConditionOr condition = parseXML<ConditionOr> 
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
			ConditionOr condition = parseXML<ConditionOr> 
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
			ConditionOr condition = parseXML<ConditionOr> 
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
			ConditionOr condition = parseXML<ConditionOr> 
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
