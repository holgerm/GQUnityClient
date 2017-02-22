using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GQ.Event {

	/// <summary>
	/// GQ event manager. You subscribe your call back methods for an event of type MyEvent by calling Subscribe() on the parameterized class GQEvent<MyEventArgs>. 
	/// Here MyEventArgs is the class defining the event data. 
	/// 
	/// You can use either our generic event data class GQEventArgs<MyEventArgs> or derive a custom event data class from EventArgs. 
	/// The generic event data class can only receive one single data parameter.
	/// 
	/// The call back methods you can subscribe must have the signature: void MyCallBackMethod(object sender, MyEventArgs eventData).
	/// 
	/// You raise the event by calling Fire() with the sender as first and the event data as second parameter.
	///
	/// C.f. the tests to see it in action.
	/// </summary>
	public static class GQEvent<EventArgsType> where EventArgsType : EventArgs {

		private static event EventHandler<EventArgsType> Listeners;

		public static void Subscribe (EventHandler<EventArgsType> eventHandler) {
			Listeners += eventHandler;
		}

		public static void Unsubscribe (EventHandler<EventArgsType> eventHandler) {
			Listeners -= eventHandler;
		}

		/// <summary>
		/// Raises an event and sends the sender and event data to all currently subscribed receiver methods.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="e">E.</param>
		public static void Fire (object sender, EventArgsType e) {
			if ( Listeners != null ) {
				Listeners(sender, e);
			}
		}
	}


	public class GQEventArgs<EventArgsType> : EventArgs {
		public EventArgsType args { 
			get; 
			private set; 
		}

		public GQEventArgs (EventArgsType args) {
			this.args = args;
		}
	}

}
