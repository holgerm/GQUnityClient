using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model;

namespace GQTests.Model
{

	public class ConditionEqualsTest : GQMLTest
	{

		[SetUp]
		public void Init ()
		{
			XmlRoot = "eq";
		}

		[Test]
		public void Empty ()
		{
			// Act:
			ConditionEq condition = parseXML<ConditionEq> (@"<eq></eq>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void SingleBool ()
		{
			// Act:
			ConditionEq condition = parseXML<ConditionEq> 
				(@"	<eq>
						<bool>
							true
						</bool>
					</eq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Num_Float_True ()
		{
			// Act:
			ConditionEq condition = parseXML<ConditionEq> 
				(@"	<eq>
						<num>
							13.02
						</num>
					</eq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Num_Float_4_True ()
		{
			// Act:
			ConditionEq condition = parseXML<ConditionEq> 
				(@"	<eq>
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
					</eq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Num_Int_4_False ()
		{
			// Act:
			ConditionEq condition = parseXML<ConditionEq> 
				(@"	<eq>
						<num>
							13
						</num>
						<num>
							13
						</num>
						<num>
							13
						</num>
						<num>
							12
						</num>
					</eq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Text_Single ()
		{
			// Act:
			ConditionEq condition = parseXML<ConditionEq> 
				(@"	<eq>
						<string>
							hallo
						</string>
					</eq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Text_4_WithWhiteSpace_Equal ()
		{
			// Act:
			ConditionEq condition = parseXML<ConditionEq> 
				(@"	<eq>
						<string>
							hallo
						</string>
						<string>
							hallo
						</string>
						<string>
							hallo
						</string>
						<string>
							hallo
						</string>
					</eq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Text_4_NoWhiteSpace_Equal ()
		{
			// Act:
			ConditionEq condition = parseXML<ConditionEq> 
				(@"	<eq>
						<string>hallo</string>
						<string>hallo</string>
						<string>hallo</string>
						<string>hallo</string>
					</eq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Text_4_False ()
		{
			// Act:
			ConditionEq condition = parseXML<ConditionEq> 
				(@"	<eq>
						<string>
							hallo
						</string>
						<string>
							hallo
						</string>
						<string>
							hallo
						</string>
						<string>
							tschüß
						</string>
					</eq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Text_4_SpecialChars_True ()
		{
			// Act:
			ConditionEq condition = parseXML<ConditionEq> 
				(@"	<eq>
						<string>
							°!§$%&#038;/()=?,.-:;
						</string>
						<string>
							°!§$%&#038;/()=?,.-:;
						</string>
						<string>
							°!§$%&#038;/()=?,.-:;
						</string>
						<string>
							°!§$%&#038;/()=?,.-:;
						</string>
					</eq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void SingleVar ()
		{
			// Act:
			ConditionEq condition = parseXML<ConditionEq> 
				(@"	<eq>
						<var>
							x
						</var>
					</eq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Bool_True_True ()
		{
			// Act:
			ConditionEq condition = parseXML<ConditionEq> 
				(@"	<eq>
						<bool>true</bool>
						<bool>true</bool>
					</eq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Bool_False_False_False_False ()
		{
			// Act:
			ConditionEq condition = parseXML<ConditionEq> 
				(@"	<eq>
						<bool>false</bool>
						<bool>false</bool>
						<bool>false</bool>
						<bool>false</bool>
					</eq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void Bool_True_False ()
		{
			// Act:
			ConditionEq condition = parseXML<ConditionEq> 
				(@"	<eq>
						<bool>true</bool>
						<bool>false</bool>
					</eq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Bool_False_False_False_True ()
		{
			// Act:
			ConditionEq condition = parseXML<ConditionEq> 
				(@"	<eq>
						<bool>false</bool>
						<bool>false</bool>
						<bool>false</bool>
						<bool>true</bool>
					</eq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}
	}
}
