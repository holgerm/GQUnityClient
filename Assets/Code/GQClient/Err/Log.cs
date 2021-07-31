using System;
using System.Collections.Generic;
using System.Text;
using Code.GQClient.Model.mgmt.quests;
using UnityEngine;

namespace Code.GQClient.Err
{

    public class Log
	{

		private static Stack<Problem> stack;

		#region File Messages

		/// <summary>
		/// Log only logs problems with a level equal or higher than the current ReportLevel.
		/// </summary>
		/// <value>The report level.</value>
		public static Level ReportLevel { get; set; }

		static Log ()
		{
			ReportLevel = Level.Info;
			stack = new Stack<Problem> ();
		}

		public static void File (string message, Level level, Recipient recipient)
		{
			if (level < ReportLevel)
				return;
			
			var problem = new Problem (message, level, recipient);
			stack.Push (problem);

			var logtext = 
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
				Debug.LogWarning ("WARNING: " + logtext);
				break;
			case Level.Error:
			case Level.FatalError:
				Debug.LogWarning ("ERROR: " + logtext);
				break;
			}
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
			InformUser (string.Format (formatString, values));
		}

		public static void InformDeveloper (string message)
		{
			File (message, Level.Info, Recipient.Developer);
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


		#region Stats

		public static void TexturesLoaded (string prefix = "")
		{
			UnityEngine.Object[] textures = Resources.FindObjectsOfTypeAll (typeof(Texture));
			StringBuilder details = new StringBuilder ();
			int bigTexturesLoaded = 0;
			for (int i = 0; i < textures.Length; i++) {
				Texture tex = (Texture)textures [i];
				if (tex.width > 100 || tex.height > 100) {
					bigTexturesLoaded++;
					details.AppendLine (tex.name + ": " + tex.width + " x " + tex.height);
				}
			}
			File (
				(prefix + "Big Textures " + bigTexturesLoaded).Yellow () + "\n" + details.ToString (), 
				Level.Info, 
				Recipient.Developer
			);
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
            if (QuestManager.Instance.CurrentQuest == Quest.Null)
            {
                QuestID = QuestManager.CurrentlyParsingQuest.Id;
                QuestName = QuestManager.CurrentlyParsingQuest.Name;
            }
            else
            {
                QuestID = QuestManager.Instance.CurrentQuest.Id;
                QuestName = QuestManager.Instance.CurrentQuest.Name;
            }
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
