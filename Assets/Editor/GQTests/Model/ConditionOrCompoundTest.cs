using Code.GQClient.Model.conditions;
using NUnit.Framework;

namespace GQTests.Model
{

	public class ConditionOrCompoundTest : GQMLTest
	{

		[SetUp]
		public void Init ()
		{
			XmlRoot = "or";
		}

		[Test]
		public void O_Oft_True ()
		{
			// Act:
			ConditionOr condition = parseXML<ConditionOr> 
				(@"	<or>
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
					</or>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void O_Off_Off_Off_Off_False ()
		{
			// Act:
			ConditionOr condition = parseXML<ConditionOr> 
				(@"	<or>
						<or>
							<eq>
								<bool>true</bool>
								<bool>false</bool>
							</eq>
							<lt>
								<num>10</num>
								<num>9.8</num>
							</lt>
						</or>
						<or>
							<eq>
								<bool>true</bool>
								<bool>false</bool>
							</eq>
							<lt>
								<num>10</num>
								<num>9.8</num>
							</lt>
						</or>
						<or>
							<eq>
								<bool>true</bool>
								<bool>false</bool>
							</eq>
							<lt>
								<num>10</num>
								<num>9.8</num>
							</lt>
						</or>
						<or>
							<eq>
								<bool>true</bool>
								<bool>false</bool>
							</eq>
							<lt>
								<num>10</num>
								<num>9.8</num>
							</lt>
						</or>
					</or>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void O__A_Otf_Oft__A_Oft_Oft__A_Otf_Oft__True ()
		{
			// Act:
			ConditionOr condition = parseXML<ConditionOr> 
				(@"	<or>
						<and>
							<or>
								<eq>
									<bool>true</bool>
									<bool>true</bool>
								</eq>
								<lt>
									<num>10</num>
									<num>19.8</num>
								</lt>
							</or>
							<or>
								<eq>
									<bool>true</bool>
									<bool>false</bool>
								</eq>
								<gt>
									<num>10</num>
									<num>9.8</num>
								</gt>
							</or>
						</and>
						<and>
							<or>
								<eq>
									<bool>true</bool>
									<bool>false</bool>
								</eq>
								<lt>
									<num>10</num>
									<num>19.8</num>
								</lt>
							</or>
							<or>
								<eq>
									<bool>true</bool>
									<bool>false</bool>
								</eq>
								<gt>
									<num>10</num>
									<num>9.8</num>
								</gt>
							</or>
						</and>
						<and>
							<or>
								<eq>
									<bool>false</bool>
									<bool>false</bool>
								</eq>
								<lt>
									<num>10</num>
									<num>9.8</num>
								</lt>
							</or>
							<or>
								<eq>
									<bool>true</bool>
									<bool>true</bool>
								</eq>
								<gt>
									<num>10</num>
									<num>19.8</num>
								</gt>
							</or>
						</and>
					</or>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void O__A_Off_Oft__A_Off_Oft__A_Off_Off__True ()
		{
			// Act:
			ConditionOr condition = parseXML<ConditionOr> 
				(@"	<or>
						<and>
							<or>
								<eq>
									<bool>true</bool>
									<bool>false</bool>
								</eq>
								<lt>
									<num>10</num>
									<num>9.8</num>
								</lt>
							</or>
							<or>
								<eq>
									<bool>true</bool>
									<bool>false</bool>
								</eq>
								<gt>
									<num>10</num>
									<num>9.8</num>
								</gt>
							</or>
						</and>
						<and>
							<or>
								<eq>
									<bool>true</bool>
									<bool>false</bool>
								</eq>
								<lt>
									<num>10</num>
									<num>9.8</num>
								</lt>
							</or>
							<or>
								<eq>
									<bool>true</bool>
									<bool>false</bool>
								</eq>
								<gt>
									<num>10</num>
									<num>9.8</num>
								</gt>
							</or>
						</and>
						<and>
							<or>
								<eq>
									<bool>true</bool>
									<bool>false</bool>
								</eq>
								<lt>
									<num>10</num>
									<num>9.8</num>
								</lt>
							</or>
							<or>
								<eq>
									<bool>true</bool>
									<bool>false</bool>
								</eq>
								<gt>
									<num>10</num>
									<num>19.8</num>
								</gt>
							</or>
						</and>
					</or>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

	}
}
