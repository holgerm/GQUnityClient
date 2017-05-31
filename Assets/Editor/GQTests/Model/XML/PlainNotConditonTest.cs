using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using GQ.Client.Model.XML;
using GQ.Client.Err;

namespace GQTests.Model.XML
{

	public class PlainNotConditonTest : XMLTest
	{

		protected override string XmlRoot {
			get {
				return "not";
			}
		}

		[Test]
		public void Empty ()
		{
			// Act:
			NotCondition condition = parseXML<NotCondition> 
				(@"	<not>
					</not>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
			Assert.AreEqual (NotCondition.NOT_CONDITION_PROBLEM_EMPTY, Log.GetLastProblem ().Message);
		}

		[Test]
		public void Single_True ()
		{
			// Act:
			NotCondition condition = parseXML<NotCondition> 
				(@"	<not>
						<eq>
							<bool>true</bool>
							<bool>false</bool>
						</eq>
					</not>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsTrue (condition.IsFulfilled ());
		}

		[Test]
		public void Single_False ()
		{
			// Act:
			NotCondition condition = parseXML<NotCondition> 
				(@"	<not>
						<lt>
							<num>10</num>
							<num>10.8</num>
						</lt>
					</not>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
		}

		[Test]
		public void TwoAtomicCond_NotAllowed ()
		{
			// Act:
			NotCondition condition = parseXML<NotCondition> 
				(@"	<not>
						<eq>
							<bool>true</bool>
							<bool>true</bool>
						</eq>
						<eq>
							<bool>true</bool>
							<bool>false</bool>
						</eq>
					</not>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
			Assert.AreEqual (NotCondition.NOT_CONDITION_PROBLEM_TOO_MANY_ATOMIC_CONIDITIONS, Log.GetLastProblem ().Message);
		}

		[Test]
		public void FourAtomicCond_NotAllowed ()
		{
			// Act:
			NotCondition condition = parseXML<NotCondition> 
				(@"	<not>
						<lt>
							<num>110</num>
							<num>10.8</num>
						</lt>
						<lt>
							<num>110</num>
							<num>10.8</num>
						</lt>
						<eq>
							<bool>false</bool>
							<bool>true</bool>
						</eq>
						<eq>
							<bool>true</bool>
							<bool>false</bool>
						</eq>
					</not>");

			// Assert:
			Assert.IsNotNull (condition);
			Assert.IsFalse (condition.IsFulfilled ());
			Assert.AreEqual (NotCondition.NOT_CONDITION_PROBLEM_TOO_MANY_ATOMIC_CONIDITIONS, Log.GetLastProblem ().Message);
		}

	}
}
