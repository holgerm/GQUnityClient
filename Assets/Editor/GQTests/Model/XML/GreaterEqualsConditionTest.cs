using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model.XML;

namespace GQTests.Model.XML
{

	public class GreaterEqualConditionTest : XMLTest
	{

		protected override string XmlRoot {
			get {
				return "geq";
			}
		}

		[Test]
		public void Empty ()
		{
			// Act:
			GreaterEqualCondition condition = parseXML<GreaterEqualCondition> (@"<geq></geq>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Bool_True ()
		{
			// Act:
			GreaterEqualCondition condition = parseXML<GreaterEqualCondition> 
				(@"	<geq>
						<bool>
							true
						</bool>
					</geq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Bool_True_False ()
		{
			// Act:
			GreaterEqualCondition condition = parseXML<GreaterEqualCondition> 
				(@"	<geq>
						<bool>
							true
						</bool>
						<bool>
							false
						</bool>
					</geq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Bool_False_True ()
		{
			// Act:
			GreaterEqualCondition condition = parseXML<GreaterEqualCondition> 
				(@"	<geq>
						<bool>
							false
						</bool>
						<bool>
							true
						</bool>
					</geq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Bool_3_True ()
		{
			// Act:
			GreaterEqualCondition condition = parseXML<GreaterEqualCondition> 
				(@"	<geq>
						<bool>
							true
						</bool>
						<bool>
							true
						</bool>
						<bool>
							true
						</bool>
					</geq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void Num_13 ()
		{
			// Act:
			GreaterEqualCondition condition = parseXML<GreaterEqualCondition> 
				(@"	<geq>
						<num>
							13
						</num>
					</geq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Num_2_1 ()
		{
			// Act:
			GreaterEqualCondition condition = parseXML<GreaterEqualCondition> 
				(@"	<geq>
						<num>2</num>
						<num>1</num>
					</geq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Num_1_2 ()
		{
			// Act:
			GreaterEqualCondition condition = parseXML<GreaterEqualCondition> 
				(@"	<geq>
						<num>1</num>
						<num>2</num>
					</geq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Num_2_2_2 ()
		{
			// Act:
			GreaterEqualCondition condition = parseXML<GreaterEqualCondition> 
				(@"	<geq>
						<num>2</num>
						<num>2</num>
						<num>2</num>
					</geq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void Num_4_3_2_1 ()
		{
			// Act:
			GreaterEqualCondition condition = parseXML<GreaterEqualCondition> 
				(@"	<geq>
						<num>4</num>
						<num>3</num>
						<num>2</num>
						<num>1</num>
					</geq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Num_4_3_3_2_1 ()
		{
			// Act:
			GreaterEqualCondition condition = parseXML<GreaterEqualCondition> 
				(@"	<geq>
						<num>4</num>
						<num>3</num>
						<num>3</num>
						<num>2</num>
						<num>1</num>
					</geq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void Num_Floats_True ()
		{
			// Act:
			GreaterEqualCondition condition = parseXML<GreaterEqualCondition> 
				(@"	<geq>
						<num>2.01</num>
						<num>2.003</num>
					</geq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Num_Floats_False ()
		{
			// Act:
			GreaterEqualCondition condition = parseXML<GreaterEqualCondition> 
				(@"	<geq>
						<num>2.01</num>
						<num>2.013</num>
					</geq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Num_Floats_Equal ()
		{
			// Act:
			GreaterEqualCondition condition = parseXML<GreaterEqualCondition> 
				(@"	<geq>
						<num>2.013</num>
						<num>2.013</num>
					</geq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void Num_Floats_5_True ()
		{
			// Act:
			GreaterEqualCondition condition = parseXML<GreaterEqualCondition> 
				(@"	<geq>
						<num>17.07</num>
						<num>16.01</num>
						<num>12.01</num>
						<num>-10.01</num>
						<num>-20.01</num>
					</geq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Text_Single ()
		{
			// Act:
			GreaterEqualCondition condition = parseXML<GreaterEqualCondition> 
				(@"	<geq>
						<string>
							abc
						</string>
					</geq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Text_3_WithWhitespace_Equal ()
		{
			// Act:
			GreaterEqualCondition condition = parseXML<GreaterEqualCondition> 
				(@"	<geq>
						<string>
							abc
						</string>
						<string>
							abc
						</string>
						<string>
							abc
						</string>
					</geq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Text_3_NoWhitespace_Equal ()
		{
			// Act:
			GreaterEqualCondition condition = parseXML<GreaterEqualCondition> 
				(@"	<geq>
						<string>abc</string>
						<string>abc</string>
						<string>abc</string>
					</geq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Text_4_True ()
		{
			// Act:
			GreaterEqualCondition condition = parseXML<GreaterEqualCondition> 
				(@"	<geq>
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
					</geq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Text_3_False ()
		{
			// Act:
			GreaterEqualCondition condition = parseXML<GreaterEqualCondition> 
				(@"	<geq>
						<string>
							die
						</string>
						<string>
							rote
						</string>
						<string>
							zora
						</string>
					</geq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Var_X ()
		{
			// Act:
			GreaterEqualCondition condition = parseXML<GreaterEqualCondition> 
				(@"	<geq>
						<var>
							x
						</var>
					</geq>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}
	}
}
