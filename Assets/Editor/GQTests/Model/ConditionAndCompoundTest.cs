using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model;

namespace GQTests.Model
{

	public class ConditionAndCompoundTest : GQMLTest
	{

		[SetUp]
		public void Init ()
		{
			XmlRoot = "and";
		}

		[Test]
		public void A_A_True ()
		{
			// Act:
			AndCondition condition = parseXML<AndCondition> 
				(@"	<and>
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
					</and>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void A_A_False ()
		{
			// Act:
			AndCondition condition = parseXML<AndCondition> 
				(@"	<and>
						<and>
							<eq>
								<bool>true</bool>
								<bool>true</bool>
							</eq>
							<gt>
								<num>10</num>
								<num>10.8</num>
							</gt>
						</and>
					</and>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void A_A_A_A_False ()
		{
			// Act:
			AndCondition condition = parseXML<AndCondition> 
				(@"	<and>
						<and>
							<and>
								<and>
									<eq>
										<bool>true</bool>
										<bool>true</bool>
									</eq>
									<gt>
										<num>10</num>
										<num>10.8</num>
									</gt>
								</and>
							</and>
						</and>
					</and>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void A_AA_True ()
		{
			// Act:
			AndCondition condition = parseXML<AndCondition> 
				(@"	<and>
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
						<and>
							<eq>
								<bool>true</bool>
								<bool>true</bool>
							</eq>
							<leq>
								<num>10</num>
								<num>10</num>
							</leq>
						</and>
					</and>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void A_AAAA_True ()
		{
			// Act:
			AndCondition condition = parseXML<AndCondition> 
				(@"	<and>
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
						<and>
							<eq>
								<bool>true</bool>
								<bool>true</bool>
							</eq>
							<leq>
								<num>10</num>
								<num>10</num>
							</leq>
						</and>
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
						<and>
							<eq>
								<bool>true</bool>
								<bool>true</bool>
							</eq>
							<leq>
								<num>10</num>
								<num>10</num>
							</leq>
						</and>
					</and>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void A_AAAA_False ()
		{
			// Act:
			AndCondition condition = parseXML<AndCondition> 
				(@"	<and>
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
						<and>
							<eq>
								<bool>true</bool>
								<bool>true</bool>
							</eq>
							<leq>
								<num>10</num>
								<num>10</num>
							</leq>
						</and>
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
						<and>
							<eq>
								<bool>true</bool>
								<bool>true</bool>
							</eq>
							<leq>
								<num>10</num>
								<num>1</num>
							</leq>
						</and>
					</and>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void A_O_True ()
		{
			// Act:
			AndCondition condition = parseXML<AndCondition> 
				(@"	<and>
						<or>
							<eq>
								<bool>true</bool>
								<bool>false</bool>
							</eq>
							<gt>
								<num>11</num>
								<num>10.8</num>
							</gt>
						</or>
					</and>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void A_O_False ()
		{
			// Act:
			AndCondition condition = parseXML<AndCondition> 
				(@"	<and>
						<or>
							<eq>
								<bool>true</bool>
								<bool>false</bool>
							</eq>
							<gt>
								<num>10</num>
								<num>10.8</num>
							</gt>
						</or>
					</and>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void A_Ot_Of_False ()
		{
			// Act:
			AndCondition condition = parseXML<AndCondition> 
				(@"	<and>
						<or>
							<eq>
								<bool>true</bool>
								<bool>true</bool>
							</eq>
							<gt>
								<num>10</num>
								<num>10.8</num>
							</gt>
						</or>
						<or>
							<eq>
								<bool>true</bool>
								<bool>false</bool>
							</eq>
							<gt>
								<num>10</num>
								<num>10.8</num>
							</gt>
						</or>
					</and>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void A_Ot_Ot_True ()
		{
			// Act:
			AndCondition condition = parseXML<AndCondition> 
				(@"	<and>
						<or>
							<eq>
								<bool>true</bool>
								<bool>true</bool>
							</eq>
							<gt>
								<num>10</num>
								<num>10.8</num>
							</gt>
						</or>
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
					</and>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void A_Ot_A_Ot_A_True ()
		{
			// Act:
			AndCondition condition = parseXML<AndCondition> 
				(@"	<and>
						<or>
							<and>
								<eq>
									<bool>true</bool>
									<bool>true</bool>
								</eq>
								<gt>
									<num>10</num>
									<num>10.8</num>
								</gt>
							</and>
							<and>
								<eq>
									<bool>true</bool>
									<bool>true</bool>
								</eq>
							</and>
						</or>
						<or>
							<and>
								<eq>
									<bool>true</bool>
									<bool>true</bool>
								</eq>
								<gt>
									<num>10</num>
									<num>10.8</num>
								</gt>
							</and>
							<and>
								<eq>
									<bool>true</bool>
									<bool>true</bool>
								</eq>
							</and>
						</or>
					</and>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

	}
}
