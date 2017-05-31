using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model.XML;

namespace GQTests.Model.XML
{

	public class GreaterConditionTest : XMLTest
	{

		protected override string XmlRoot {
			get {
				return "gt";
			}
		}

		[Test]
		public void Empty ()
		{
			// Act:
			GreaterCondition condition = parseXML<GreaterCondition> (@"<gt></gt>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Bool_True ()
		{
			// Act:
			GreaterCondition condition = parseXML<GreaterCondition> 
				(@"	<gt>
						<bool>
							true
						</bool>
					</gt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Bool_True_False ()
		{
			// Act:
			GreaterCondition condition = parseXML<GreaterCondition> 
				(@"	<gt>
						<bool>
							true
						</bool>
						<bool>
							false
						</bool>
					</gt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Bool_False_True ()
		{
			// Act:
			GreaterCondition condition = parseXML<GreaterCondition> 
				(@"	<gt>
						<bool>
							false
						</bool>
						<bool>
							true
						</bool>
					</gt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Num_13 ()
		{
			// Act:
			GreaterCondition condition = parseXML<GreaterCondition> 
				(@"	<gt>
						<num>
							13
						</num>
					</gt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Num_2_1 ()
		{
			// Act:
			GreaterCondition condition = parseXML<GreaterCondition> 
				(@"	<gt>
						<num>2</num>
						<num>1</num>
					</gt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Num_1_2 ()
		{
			// Act:
			GreaterCondition condition = parseXML<GreaterCondition> 
				(@"	<gt>
						<num>1</num>
						<num>2</num>
					</gt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Num_4_3_2_1 ()
		{
			// Act:
			GreaterCondition condition = parseXML<GreaterCondition> 
				(@"	<gt>
						<num>4</num>
						<num>3</num>
						<num>2</num>
						<num>1</num>
					</gt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Num_4_3_3_2_1 ()
		{
			// Act:
			GreaterCondition condition = parseXML<GreaterCondition> 
				(@"	<gt>
						<num>4</num>
						<num>3</num>
						<num>3</num>
						<num>2</num>
						<num>1</num>
					</gt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Num_Floats_True ()
		{
			// Act:
			GreaterCondition condition = parseXML<GreaterCondition> 
				(@"	<gt>
						<num>2.01</num>
						<num>2.003</num>
					</gt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Num_Floats_False ()
		{
			// Act:
			GreaterCondition condition = parseXML<GreaterCondition> 
				(@"	<gt>
						<num>2.01</num>
						<num>2.013</num>
					</gt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Num_Floats_Equal ()
		{
			// Act:
			GreaterCondition condition = parseXML<GreaterCondition> 
				(@"	<gt>
						<num>2.013</num>
						<num>2.013</num>
					</gt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Num_Floats_5_True ()
		{
			// Act:
			GreaterCondition condition = parseXML<GreaterCondition> 
				(@"	<gt>
						<num>17.07</num>
						<num>16.01</num>
						<num>12.01</num>
						<num>-10.01</num>
						<num>-20.01</num>
					</gt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Text_Single ()
		{
			// Act:
			GreaterCondition condition = parseXML<GreaterCondition> 
				(@"	<gt>
						<string>
							abc
						</string>
					</gt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Text_4_True ()
		{
			// Act:
			GreaterCondition condition = parseXML<GreaterCondition> 
				(@"	<gt>
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
					</gt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Text_3_False ()
		{
			// Act:
			GreaterCondition condition = parseXML<GreaterCondition> 
				(@"	<gt>
						<string>
							die
						</string>
						<string>
							rote
						</string>
						<string>
							zora
						</string>
					</gt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void Var_X ()
		{
			// Act:
			GreaterCondition condition = parseXML<GreaterCondition> 
				(@"	<gt>
						<var>
							x
						</var>
					</gt>"); 

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}
	}
}
