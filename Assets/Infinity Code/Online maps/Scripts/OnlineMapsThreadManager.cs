/*     INFINITY CODE 2013-2014      */
/*   http://www.infinity-code.com   */

using System;
using System.Collections.Generic;
using System.Threading;

/// <summary>
/// This class manages the background threads.
/// <strong>Please do not use it.</strong>\n
/// </summary>
public static class OnlineMapsThreadManager
{
    private static Thread thread;
    private static List<Action> threadActions;

    public static void AddThreadAction(Action action)
    {
        if (threadActions == null) threadActions = new List<Action>();

        lock (threadActions)
        {
            threadActions.Add(action);
        }

        if (thread == null)
        {
            thread = new Thread(StartNextAction);
            thread.Start();
        }
    }

    private static void StartNextAction()
    {
        while (true)
        {
            bool actionInvoked = false;
            lock (threadActions)
            {
                if (threadActions.Count > 0)
                {
                    Action currentLoading = threadActions[0];
                    threadActions.RemoveAt(0);
                    currentLoading();
                    actionInvoked = true;
                }
            }
            if (!actionInvoked) Thread.Sleep(1);
        }
    }
}