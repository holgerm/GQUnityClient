using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GQ.Client.Model;

namespace GQ.Client.Err
{

	public class Log
	{

		#region Public API

		static Stack<Problem> stack = new Stack<Problem> ();

		public static void File (string message, Level level, Recipient recipient)
		{
			Problem problem = new Problem (message, level, recipient);
			stack.Push (problem);

			#if UNITY_EDITOR
			string logtext = 
				string.Format (
					"{0}. {1} for {2} in quest {3} ({4})", 
					stack.Count, 
					message, 
					recipient,
					problem.QuestName,
					problem.QuestID
				);
			switch (level) {
			case Level.Info:
				Debug.Log (logtext);
				break;
			case Level.Warning:
				Debug.LogWarning (logtext);
				break;
			case Level.Error:
			case Level.FatalError:
				Debug.LogError (logtext);
				break;
			default:
				break;
			}
			#endif		
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



		public static void SignalErrorToAuthor (string message)
		{
			File (message, Level.Error, Recipient.Author);
		}


		public static void SignalErrorToAuthor (string formatString, params object[] values)
		{
			SignalErrorToAuthor (String.Format (formatString, values));
		}


		public static void WarnDeveloper (string message)
		{
			File (message, Level.Warning, Recipient.Developer);
		}


		public static void WarnDeveloper (string formatString, params object[] values)
		{
			WarnDeveloper (String.Format (formatString, values));
		}


		public static void SignalErrorToDeveloper (string message)
		{
			File (message, Level.Error, Recipient.Developer);
		}


		public static void SignalErrorToDeveloper (string formatString, params object[] values)
		{
			SignalErrorToDeveloper (String.Format (formatString, values));
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

		int questID;

		public int QuestID {
			get {
				return questID;
			}
		}

		string questName;

		public string QuestName {
			get {
				return questName;
			}
		}

		public Problem (string message, Level level, Recipient recipient)
		{
			this.message = message;
			this.level = level;
			this.recipient = recipient;
			this.timestamp = DateTime.Now;
			this.questID = Quest.CurrentlyParsingQuest.Id;
			this.questName = Quest.CurrentlyParsingQuest.Name;
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
