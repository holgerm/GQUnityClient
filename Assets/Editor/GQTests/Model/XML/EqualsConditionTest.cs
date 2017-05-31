using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model.XML;

namespace GQTests.Model.XML
{

	public class EqualsConditionTest : XMLTest
	{

		protected override string XmlRoot {
			get {
				return "eq";
			}
		}

		[Test]
		public void Empty ()
		{
			// Act:
			EqualCondition condition = parseXML<EqualCondition> (@"<eq></eq>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void SingleBool ()
		{
			// Act:
			EqualCondition condition = parseXML<EqualCondition> 
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
			EqualCondition condition = parseXML<EqualCondition> 
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
			EqualCondition condition = parseXML<EqualCondition> 
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
			EqualCondition condition = parseXML<EqualCondition> 
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
			EqualCondition condition = parseXML<EqualCondition> 
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
			EqualCondition condition = parseXML<EqualCondition> 
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
			EqualCondition condition = parseXML<EqualCondition> 
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
			EqualCondition condition = parseXML<EqualCondition> 
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
			EqualCondition condition = parseXML<EqualCondition> 
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
			EqualCondition condition = parseXML<EqualCondition> 
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
			EqualCondition condition = parseXML<EqualCondition> 
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
			EqualCondition condition = parseXML<EqualCondition> 
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
			EqualCondition condition = parseXML<EqualCondition> 
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
			EqualCondition condition = parseXML<EqualCondition> 
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
