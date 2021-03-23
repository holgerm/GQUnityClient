using Code.GQClient.Model.conditions;
using NUnit.Framework;

namespace GQTests.Model
{

	public class ConditionTest : GQMLTest
	{

		[SetUp]
		public void Init ()
		{
			XmlRoot = "condition";
		}

		[Test]
		public void AndCondition_True ()
		{
			// Act:
			CompoundCondition condition = parseXML<CompoundCondition> 
				(@"	<condition>
						<and>
							<eq>
								<bool>true</bool>
								<bool>true</bool>
							</eq>
							<lt>
								<num>10</num>
								<num>10.8</num>
							</lt>
						</and>
					</condition>");
			
			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void AndCondition_False ()
		{
			// Act:
			CompoundCondition condition = parseXML<CompoundCondition> 
				(@"	<condition>
						<and>
							<eq>
								<bool>true</bool>
								<bool>true</bool>
							</eq>
							<lt>
								<num>10.9</num>
								<num>10.8</num>
							</lt>
						</and>
					</condition>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void OrCondition_True ()
		{
			// Act:
			CompoundCondition condition = parseXML<CompoundCondition> 
				(@"	<condition>
						<or>
							<eq>
								<bool>true</bool>
								<bool>false</bool>
							</eq>
							<lt>
								<num>10</num>
								<num>10.8</num>
							</lt>
						</or>
					</condition>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void OrCondition_False ()
		{
			// Act:
			CompoundCondition condition = parseXML<CompoundCondition> 
				(@"	<condition>
						<or>
							<eq>
								<bool>true</bool>
								<bool>false</bool>
							</eq>
							<leq>
								<num>12</num>
								<num>10.8</num>
							</leq>
						</or>
					</condition>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void SingleComparison_True ()
		{
			// Act:
			CompoundCondition condition = parseXML<CompoundCondition> 
				(@"	<condition>
						<leq>
							<num>10</num>
							<num>10.8</num>
						</leq>
					</condition>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void SingleComparison_False ()
		{
			// Act:
			CompoundCondition condition = parseXML<CompoundCondition> 
				(@"	<condition>
						<leq>
							<num>12</num>
							<num>10.8</num>
						</leq>
					</condition>"); 
			
			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void MultipleComparison_True ()
		{
			// Act:
			CompoundCondition condition = parseXML<CompoundCondition> 
				(@"	<condition>
						<leq>
							<num>10</num>
							<num>10.8</num>
						</leq>
						<eq>
							<num>
								13.02
							</num>
							<num>
								13.02
							</num>
							<num>
								13.02
							</num>
							<num>
								13.02
							</num>
						</eq>
						<gt>
							<bool>
								true
							</bool>
							<bool>
								false
							</bool>
						</gt>
					</condition>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void MultipleComparison_False ()
		{
			// Act:
			CompoundCondition condition = parseXML<CompoundCondition> 
				(@"	<condition>
						<eq>
							<num>
								13.02
							</num>
							<num>
								13.02
							</num>
							<num>
								13.02
							</num>
							<num>
								13.02
							</num>
						</eq>
						<gt>
							<bool>
								true
							</bool>
							<bool>
								false
							</bool>
						</gt>
						<leq>
							<num>12</num>
							<num>10.8</num>
						</leq>
					</condition>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}
	}
}
