using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model.XML;

namespace GQTests.Model.XML
{

	public class CompoundOrConditionTest : XMLTest
	{

		protected override string XmlRoot {
			get {
				return "or";
			}
		}

		[Test]
		public void O_Oft_True ()
		{
			// Act:
			OrCondition condition = parseXML<OrCondition> 
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
			OrCondition condition = parseXML<OrCondition> 
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
			OrCondition condition = parseXML<OrCondition> 
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
			OrCondition condition = parseXML<OrCondition> 
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
