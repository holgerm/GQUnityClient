using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using GQ.Client.Model;

namespace GQ.Client.Err
{

	public class Log
	{

		private static Stack<Problem> stack;

		#region Public API

		/// <summary>
		/// Log only logs problems with a level equal or higher than the current ReportLevel.
		/// </summary>
		/// <value>The report level.</value>
		public static Level ReportLevel { get; set; }

		static Log() {
			ReportLevel = Level.Warning;
			stack = new Stack<Problem> ();
		}

		public static void File (string message, Level level, Recipient recipient)
		{
			if (level < ReportLevel)
				return;
			
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


		public static void InformUser (string message)
		{
			File (message, Level.Info, Recipient.User);
		}

		public static void InformUser (string formatString, params object[] values)
		{
			InformUser (String.Format (formatString, values));
		}


		public static void SignalErrorToUser (string message)
		{
			File (message, Level.Error, Recipient.User);
		}

		public static void SignalErrorToUser (string formatString, params object[] values)
		{
			SignalErrorToUser (String.Format (formatString, values));
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


	public class Problem
	{
		public string Message { get; private set; }
		public Recipient Recipient { get; private set; }
		public Level Level { get; private set; } 
		public DateTime Timestamp { get; private set; }
		public int QuestID { get; private set; }
		public string QuestName { get; private set; }

		public Problem (string message, Level level, Recipient recipient)
		{
			Message = message;
			Level = level;
			Recipient = recipient;
			Timestamp = DateTime.Now;
			QuestID = Quest.CurrentlyParsingQuest.Id;
			QuestName = Quest.CurrentlyParsingQuest.Name;
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
