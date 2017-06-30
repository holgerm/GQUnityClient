using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model;

namespace GQTests.Model
{

	public class ConditionLessEqualTest : GQMLTest
	{

		[SetUp]
		public void Init ()
		{
			XmlRoot = "leq";
		}

		[Test]
		public void Empty ()
		{
			// Act:
			ConditionLeq condition = parseXML<ConditionLeq> (@"<leq></leq>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Bool_True ()
		{
			// Act:
			ConditionLeq condition = parseXML<ConditionLeq> 
				(@"	<leq>
						<bool>
							true
						</bool>
					</leq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Bool_True_False ()
		{
			// Act:
			ConditionLeq condition = parseXML<ConditionLeq> 
				(@"	<leq>
						<bool>
							true
						</bool>
						<bool>
							false
						</bool>
					</leq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Bool_False_True ()
		{
			// Act:
			ConditionLeq condition = parseXML<ConditionLeq> 
				(@"	<leq>
						<bool>
							false
						</bool>
						<bool>
							true
						</bool>
					</leq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void Num_13 ()
		{
			// Act:
			ConditionLeq condition = parseXML<ConditionLeq> 
				(@"	<leq>
						<num>
							13
						</num>
					</leq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Num_2_1 ()
		{
			// Act:
			ConditionLeq condition = parseXML<ConditionLeq> 
				(@"	<leq>
						<num>2</num>
						<num>1</num>
					</leq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Num_1_2 ()
		{
			// Act:
			ConditionLeq condition = parseXML<ConditionLeq> 
				(@"	<leq>
						<num>1</num>
						<num>2</num>
					</leq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void Num_4_3_2_1 ()
		{
			// Act:
			ConditionLeq condition = parseXML<ConditionLeq> 
				(@"	<leq>
						<num>4</num>
						<num>3</num>
						<num>2</num>
						<num>1</num>
					</leq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Num_4_3_3_2_1 ()
		{
			// Act:
			ConditionLeq condition = parseXML<ConditionLeq> 
				(@"	<leq>
						<num>4</num>
						<num>3</num>
						<num>3</num>
						<num>2</num>
						<num>1</num>
					</leq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Num_Floats_True ()
		{
			// Act:
			ConditionLeq condition = parseXML<ConditionLeq> 
				(@"	<leq>
						<num>2.01</num>
						<num>2.003</num>
					</leq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Num_Floats_False ()
		{
			// Act:
			ConditionLeq condition = parseXML<ConditionLeq> 
				(@"	<leq>
						<num>2.01</num>
						<num>2.013</num>
					</leq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void Num_Floats_Equal ()
		{
			// Act:
			ConditionLeq condition = parseXML<ConditionLeq> 
				(@"	<leq>
						<num>2.013</num>
						<num>2.013</num>
					</leq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void Num_Floats_5_False ()
		{
			// Act:
			ConditionLeq condition = parseXML<ConditionLeq> 
				(@"	<leq>
						<num>17.07</num>
						<num>16.01</num>
						<num>12.01</num>
						<num>-10.01</num>
						<num>-20.01</num>
					</leq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Text_Single ()
		{
			// Act:
			ConditionLeq condition = parseXML<ConditionLeq> 
				(@"	<leq>
						<string>
							abc
						</string>
					</leq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Text_4_False ()
		{
			// Act:
			ConditionLeq condition = parseXML<ConditionLeq> 
				(@"	<leq>
						<string>
							zora
						</string>
						<string>
							rote
						</string>
						<string>
							geldbe
						</string>
						<string>
							blaue
						</string>
					</leq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Text_5_True ()
		{
			// Act:
			ConditionLeq condition = parseXML<ConditionLeq> 
				(@"	<leq>
						<string>
							die
						</string>
						<string>
							rote
						</string>
						<string>
							zora
						</string>
						<string>
							zora
						</string>
						<string>
							zora
						</string>
					</leq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void Var_X ()
		{
			// Act:
			ConditionLeq condition = parseXML<ConditionLeq> 
				(@"	<leq>
						<var>
							x
						</var>
					</leq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}
	}
}
