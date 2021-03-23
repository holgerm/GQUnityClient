using Code.GQClient.Model.conditions;
using NUnit.Framework;

namespace GQTests.Model
{

	public class ConditionGreaterTest : GQMLTest
	{

		[SetUp]
		public void Init ()
		{
			XmlRoot = "gt";
		}

		[Test]
		public void Empty ()
		{
			// Act:
			ConditionGt condition = parseXML<ConditionGt> (@"<gt></gt>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.That (condition.IsFulfilled ());
		}

		[Test]
		public void Bool_True ()
		{
			// Act:
			ConditionGt condition = parseXML<ConditionGt> 
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
			ConditionGt condition = parseXML<ConditionGt> 
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
			ConditionGt condition = parseXML<ConditionGt> 
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
			ConditionGt condition = parseXML<ConditionGt> 
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
			ConditionGt condition = parseXML<ConditionGt> 
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
			ConditionGt condition = parseXML<ConditionGt> 
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
			ConditionGt condition = parseXML<ConditionGt> 
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
			ConditionGt condition = parseXML<ConditionGt> 
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
			ConditionGt condition = parseXML<ConditionGt> 
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
			ConditionGt condition = parseXML<ConditionGt> 
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
			ConditionGt condition = parseXML<ConditionGt> 
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
			ConditionGt condition = parseXML<ConditionGt> 
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
			ConditionGt condition = parseXML<ConditionGt> 
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
			ConditionGt condition = parseXML<ConditionGt> 
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
			ConditionGt condition = parseXML<ConditionGt> 
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
			ConditionGt condition = parseXML<ConditionGt> 
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
