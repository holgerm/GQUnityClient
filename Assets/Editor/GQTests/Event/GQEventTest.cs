
using NUnit.Framework;
using GQ.Event;
using UnityEngine;
using System;

namespace GQTests.Event {

	public class GQEventTest {


		[Test]
		public void GenericGQEventArgs () {
			// Arrange:
			MockObserver<GQEventArgs<string>> obsA = new MockObserver<GQEventArgs<string>>();
			GQEvent<GQEventArgs<string>>.Subscribe(obsA.notify);

			// Act:
			string hello = "Hello Test!";
			GQEvent<GQEventArgs<string>>.Fire(this, new GQEventArgs<string>(hello));

			// Assert:
			Assert.AreEqual(1, obsA.notified);
		}

		[Test]
		public void CustomGQEventArgs () {
			// Arrange:
			MockObserver<CustomEventArgs> obsA = new MockObserver<CustomEventArgs>();
			GQEvent<CustomEventArgs>.Subscribe(obsA.notify);

			// Act:
			string hello = "Hello Test!";
			int count = 1001;
			GQEvent<CustomEventArgs>.Fire(this, new CustomEventArgs(hello, count));

			// Assert:
			Assert.AreEqual(1, obsA.notified);
		}

		[Test]
		public void Unsubscribe () {
			// Arrange:
			MockObserver<CustomEventArgs> obsA = new MockObserver<CustomEventArgs>();
			GQEvent<CustomEventArgs>.Subscribe(obsA.notify);

			// Act:
			string hello = "Hello Test!";
			int count = 1001;
			GQEvent<CustomEventArgs>.Fire(this, new CustomEventArgs(hello, count));

			// Assert:
			Assert.AreEqual(1, obsA.notified);

			// Arrange again:
			GQEvent<CustomEventArgs>.Unsubscribe(obsA.notify);

			//Act again
			GQEvent<CustomEventArgs>.Fire(this, new CustomEventArgs(hello, count));

			//Assert again
			Assert.AreEqual(1, obsA.notified);
		}

		[Test]
		public void MultipleSubscriptions () {
			// Arrange:
			MockObserver<CustomEventArgs> obsA = new MockObserver<CustomEventArgs>();
			GQEvent<CustomEventArgs>.Subscribe(obsA.notify);
			MockObserver<CustomEventArgs> obsB = new MockObserver<CustomEventArgs>();
			GQEvent<CustomEventArgs>.Subscribe(obsB.notify);

			//Act
			string hello = "Hello Test!";
			int count = 1001;
			GQEvent<CustomEventArgs>.Fire(this, new CustomEventArgs(hello, count));

			//Assert
			Assert.AreEqual(1, obsA.notified);
			Assert.AreEqual(1, obsB.notified);

			//Act
			hello = "Hello Again!";
			count = 1002;
			GQEvent<CustomEventArgs>.Fire(this, new CustomEventArgs(hello, count));

			//Assert
			Assert.AreEqual(2, obsA.notified);
			Assert.AreEqual(2, obsB.notified);
		}

		[Test]
		public void MultipleEventTypes () {
			// Arrange:
			MockObserver<GQEventArgs<string>> obsGenericEvent = new MockObserver<GQEventArgs<string>>();
			GQEvent<GQEventArgs<string>>.Subscribe(obsGenericEvent.notify);

			MockObserver<CustomEventArgs> obsCustomEvent = new MockObserver<CustomEventArgs>();
			GQEvent<CustomEventArgs>.Subscribe(obsCustomEvent.notify);

			// Act on Generic Event:
			string hello = "Hello receiver of a GENERIC event!";
			GQEvent<GQEventArgs<string>>.Fire(this, new GQEventArgs<string>(hello));

			// Assert:
			Assert.AreEqual(1, obsGenericEvent.notified);
			Assert.AreEqual(0, obsCustomEvent.notified);

			// Act on TestEvent:
			hello = "Hello receiver of a CUSTOM event!";
			int count = 1001;
			GQEvent<CustomEventArgs>.Fire(this, new CustomEventArgs(hello, count));

			// Assert:
			Assert.AreEqual(1, obsGenericEvent.notified);
			Assert.AreEqual(1, obsCustomEvent.notified);
		}
	}



	// ######################## HELPERS: #######################
	// _________________________________________________________

	public class MockObserver<EventArgsType> {

		public object Sender {
			get; 
			private set;
		}

		public EventArgsType eventArgs {
			get; 
			private set;
		}

		public void notify (object sender, EventArgsType e) {
			notified++;
			Sender = sender;
			eventArgs = e;
		}

		public int notified = 0;
	}


	public class CustomEventArgs : EventArgs {
		string testData { get; set; }

		public CustomEventArgs (string whatHappened, int howOftenItHappended) {
			testData = whatHappened;
		}
	}

}
