using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model;

namespace GQTests.Model
{

	public class ConditionLessTest : GQMLTest
	{

		[SetUp]
		public void Init ()
		{
			XmlRoot = "lt";
		}

		[Test]
		public void Empty ()
		{
			// Act:
			LessCondition condition = parseXML<LessCondition> (@"<lt></lt>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Bool_True ()
		{
			// Act:
			LessCondition condition = parseXML<LessCondition> 
				(@"	<lt>
						<bool>
							true
						</bool>
					</lt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Bool_True_False ()
		{
			// Act:
			LessCondition condition = parseXML<LessCondition> 
				(@"	<lt>
						<bool>
							true
						</bool>
						<bool>
							false
						</bool>
					</lt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Bool_False_True ()
		{
			// Act:
			LessCondition condition = parseXML<LessCondition> 
				(@"	<lt>
						<bool>
							false
						</bool>
						<bool>
							true
						</bool>
					</lt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void Num_13 ()
		{
			// Act:
			LessCondition condition = parseXML<LessCondition> 
				(@"	<lt>
						<num>
							13
						</num>
					</lt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Num_2_1 ()
		{
			// Act:
			LessCondition condition = parseXML<LessCondition> 
				(@"	<lt>
						<num>2</num>
						<num>1</num>
					</lt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Num_1_2 ()
		{
			// Act:
			LessCondition condition = parseXML<LessCondition> 
				(@"	<lt>
						<num>1</num>
						<num>2</num>
					</lt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void Num_4_3_2_1 ()
		{
			// Act:
			LessCondition condition = parseXML<LessCondition> 
				(@"	<lt>
						<num>4</num>
						<num>3</num>
						<num>2</num>
						<num>1</num>
					</lt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Num_4_3_3_2_1 ()
		{
			// Act:
			LessCondition condition = parseXML<LessCondition> 
				(@"	<lt>
						<num>4</num>
						<num>3</num>
						<num>3</num>
						<num>2</num>
						<num>1</num>
					</lt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Num_Floats_True ()
		{
			// Act:
			LessCondition condition = parseXML<LessCondition> 
				(@"	<lt>
						<num>2.01</num>
						<num>2.003</num>
					</lt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Num_Floats_False ()
		{
			// Act:
			LessCondition condition = parseXML<LessCondition> 
				(@"	<lt>
						<num>2.01</num>
						<num>2.013</num>
					</lt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void Num_Floats_Equal ()
		{
			// Act:
			LessCondition condition = parseXML<LessCondition> 
				(@"	<lt>
						<num>2.013</num>
						<num>2.013</num>
					</lt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Num_Floats_5_False ()
		{
			// Act:
			LessCondition condition = parseXML<LessCondition> 
				(@"	<lt>
						<num>17.07</num>
						<num>16.01</num>
						<num>12.01</num>
						<num>-10.01</num>
						<num>-20.01</num>
					</lt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Text_Single ()
		{
			// Act:
			LessCondition condition = parseXML<LessCondition> 
				(@"	<lt>
						<string>
							abc
						</string>
					</lt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Text_4_False ()
		{
			// Act:
			LessCondition condition = parseXML<LessCondition> 
				(@"	<lt>
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
					</lt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Text_3_True ()
		{
			// Act:
			LessCondition condition = parseXML<LessCondition> 
				(@"	<lt>
						<string>
							die
						</string>
						<string>
							rote
						</string>
						<string>
							zora
						</string>
					</lt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void Var_X ()
		{
			// Act:
			LessCondition condition = parseXML<LessCondition> 
				(@"	<lt>
						<var>
							x
						</var>
					</lt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}
	}
}
