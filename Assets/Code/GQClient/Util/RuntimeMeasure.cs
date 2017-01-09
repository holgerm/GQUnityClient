using UnityEngine;
using System.Collections;
using System.Diagnostics;

public class RuntimeMeasure {

	private string message;
	private Stopwatch stopwatch;

	public RuntimeMeasure (string message) {
		this.message = message;
		stopwatch = new Stopwatch();
		stopwatch.Start();
	}

	public void Stop () {
		this.stopwatch.Stop();
		UnityEngine.Debug.Log("STOPWATCH:   " + message + " took: " + stopwatch.Elapsed);
	}

}
