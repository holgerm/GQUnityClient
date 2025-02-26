﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Code.QM.Util
{
    public class WATCH
    {
        static Dictionary<string, WATCH> watches = new Dictionary<string, WATCH>();

        private Stopwatch stopwatch;
        public string Name { get; private set; }
        private long lastTimeStamp;

        public WATCH() : this(new StackFrame(1).GetMethod().DeclaringType.Name + "." +
                              new StackFrame(1).GetMethod().Name)
        {
        }

        public WATCH(string name, bool log = false)
        {
            stopwatch = new Stopwatch();
            this.Name = name;
            nameOfLastStarted = name;
            this.lastTimeStamp = 0L;
            watches[name] = this;
            if (log)
            {
                UnityEngine.Debug.Log("WATCH started: " + name + " (frame# " + Time.frameCount + ")");
            }
        }

        public static WATCH Get(string name)
        {
            WATCH watch;
            if (!watches.TryGetValue(name, out watch))
            {
                return null;
            }

            return watch;
        }

        public void Start()
        {
            lastTimeStamp = 0L;
            nameOfLastStarted =
                this.Name == null
                    ? new StackFrame(1).GetType().Name + "." + new StackFrame(1).GetMethod().Name
                    : this.Name;
            stopwatch.Start();
        }

        public void StopAndShow()
        {
            stopwatch.Stop();
            UnityEngine.Debug.Log(
                string.Format("WATCH {0} stopped after {1} ms (<color=Red>{2}</color> delta in frame# {3})",
                    Name,
                    stopwatch.ElapsedMilliseconds,
                    stopwatch.ElapsedMilliseconds - lastTimeStamp,
                    Time.frameCount
                )
            );
        }

        public static void Lap(string name, bool log = false)
        {
            WATCH w = Get(name);
            if (w == null)
            {
                UnityEngine.Debug.Log(string.Format("WATCH {0} not available.", name));
                return;
            }

            w.Lap();
            if (log)
            {
                UnityEngine.Debug.Log("WATCH lap: " + name + " (frame# " + Time.frameCount + ")");
            }
        }

        public void Lap()
        {
            lastTimeStamp = 0L;
        }

        public static void Show(string name, string pointName)
        {
            WATCH w = Get(name);
            if (w == null)
            {
                UnityEngine.Debug.Log(string.Format("WATCH {0} not available at {1}.", name, pointName));
                return;
            }

            w.Show(pointName);
        }

        public void Show(string pointName)
        {
            stopwatch.Stop();
            Debug.Log($"WATCH {Name} at {pointName} took {stopwatch.ElapsedMilliseconds} ms (<color=Red>{stopwatch.ElapsedMilliseconds - lastTimeStamp}</color>) delta in frame# {Time.frameCount}");
            lastTimeStamp = stopwatch.ElapsedMilliseconds;
            stopwatch.Start();
        }

        public static long Milliseconds(string name)
        {
            WATCH w = Get(name);
            if (w == null)
                return 0L;
            return w.Milliseconds();
        }

        public long Milliseconds()
        {
            return stopwatch.ElapsedMilliseconds;
        }

        private static string nameOfLastStarted;
        private static int nrOfShows = 0;

        public static void _Start()
        {
            _Start(new StackFrame(1).GetMethod().DeclaringType.Name + "." + new StackFrame(1).GetMethod().Name);
        }

        public static void _Start(string name)
        {
            nameOfLastStarted = name;
            WATCH w = new WATCH(name);
            w.Start();
        }

        public static void _Show()
        {
            WATCH w = Get(nameOfLastStarted);
            if (w == null)
                return;

            w.Show("" + nrOfShows++);
        }

        public static void _StopAndShow()
        {
            _StopAndShow(nameOfLastStarted);
        }

        public static void _StopAndShow(string name)
        {
            WATCH w = Get(name);
            if (w == null)
            {
                UnityEngine.Debug.Log(string.Format("WATCH {0} not available.", name));
                return;
            }

            w.StopAndShow();
        }

        private static Dictionary<string, long> measurements = new Dictionary<string, long>();

        public static void StartMeasure(string name = "_")
        {
            if (measurements.ContainsKey(name))
            {
                measurements.Remove(name);
            }

            measurements.Add(name, DateTime.Now.Ticks);
        }

        public static long TakeMeasure(string name = "_")
        {
            long now = DateTime.Now.Ticks;

            if (!measurements.ContainsKey(name))
                return -1L;

            return (now - measurements[name]);
        }

        public static void ShowMeasure(string name = "_")
        {
            Debug.Log($"MEASURE {name}: {TakeMeasure(name)}");
        }
    }
}