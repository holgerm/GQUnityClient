using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace GQ.Client.Err
{

	public class Log
	{

		#region Public API

		static Stack<Problem> stack = new Stack<Problem> ();

		public static void File (string message, Level level, Recipient recipient)
		{
			stack.Push (new Problem (message, level, recipient));
		}


		public static Problem GetLastProblem ()
		{
			return stack.Peek ();
		}


		public static void WarnAuthor (string message)
		{
			File (message, Level.Warning, Recipient.Author);
		}


		public static void WarnAuthor (string formatString, params object[] values)
		{
			WarnAuthor (String.Format (formatString, values));
		}


		public static void WarnDeveloper (string message)
		{
			File (message, Level.Warning, Recipient.Developer);
		}


		public static void WarnDeveloper (string formatString, params object[] values)
		{
			WarnDeveloper (String.Format (formatString, values));
		}

		#endregion



	}


	public class Problem : MonoBehaviour
	{
		string message;

		public string Message {
			get {
				return message;
			}
		}

		Recipient recipient;

		public Recipient Recipient {
			get {
				return recipient;
			}
		}

		Level level;

		public Level Level {
			get {
				return level;
			}
		}

		DateTime timestamp;

		public DateTime Timestamp {
			get {
				return timestamp;
			}
		}

		public Problem (string message, Level level, Recipient recipient)
		{
			this.message = message;
			this.level = level;
			this.recipient = recipient;
			this.timestamp = DateTime.Now;
		}
	}


	public enum Recipient
	{
		User,
		Author,
		Provider,
		Developer
	}


	public enum Level
	{
		Info,
		Warning,
		Error,
		FatalError
	}

}
