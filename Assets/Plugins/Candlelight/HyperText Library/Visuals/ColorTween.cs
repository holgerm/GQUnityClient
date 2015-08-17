// 
// ColorTween.cs
// 
// Copyright (c) 2014, Candlelight Interactive, LLC
// All rights reserved.
// 
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://download.unity3d.com/assetstore/customer-eula.pdf
// 
// This file contains some basic classes for performing a color tween. It
// mirrors those found in UnityEngine.UI.CoroutineTween.

using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Candlelight.ColorTween
{
	/// <summary>
	/// Possible color tween modes.
	/// </summary>
	public enum Mode
	{
		All,
		RGB,
		Alpha
	}

	/// <summary>
	/// Info for the color tween.
	/// </summary>
	public struct Info
	{
		/// <summary>
		/// A color change callback.
		/// </summary>
		internal class Callback : UnityEvent<Color> {}

		/// <summary>
		/// The on color change callbacks.
		/// </summary>
		private Callback onColorChange;

		/// <summary>
		/// Gets the duration.
		/// </summary>
		/// <value>The duration.</value>
		public float Duration { get; private set; }

		/// <summary>
		/// Gets a value indicating whether this <see cref="Candlelight.ColorTween+Info"/> ignore time scale.
		/// </summary>
		/// <value><c>true</c> if ignore time scale; otherwise, <c>false</c>.</value>
		public bool IgnoreTimeScale { get; private set; }

		/// <summary>
		/// Gets the start color.
		/// </summary>
		/// <value>The start color.</value>
		public Color StartColor { get; private set; }

		/// <summary>
		/// Gets the color of the target.
		/// </summary>
		/// <value>The color of the target.</value>
		public Color TargetColor { get; private set; }

		/// <summary>
		/// Gets the tween mode.
		/// </summary>
		/// <value>The tween mode.</value>
		public Mode TweenMode { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Candlelight.ColorTween+Info"/> struct.
		/// </summary>
		/// <param name="duration">Duration.</param>
		/// <param name="ignoreTimeScale">If set to <c>true</c> ignore time scale.</param>
		/// <param name="startColor">Start color.</param>
		/// <param name="targetColor">Target color.</param>
		/// <param name="tweenMode">Tween mode.</param>
		public Info(
			float duration, bool ignoreTimeScale, Color startColor, Color targetColor, Mode tweenMode
		): this()
		{
			Duration = duration;
			IgnoreTimeScale = ignoreTimeScale;
			StartColor = startColor;
			TargetColor = targetColor;
			TweenMode = tweenMode;
			onColorChange = new Callback();
		}

		/// <summary>
		/// Adds the on changed callback.
		/// </summary>
		/// <param name="callback">Callback.</param>
		public void AddOnChangedCallback(UnityAction<Color> callback)
		{
			onColorChange.AddListener(callback);
		}

		/// <summary>
		/// Tweens the value.
		/// </summary>
		/// <param name="percentage">Percentage.</param>
		public void TweenValue(float percentage)
		{
			Color result = Color.Lerp(StartColor, TargetColor, percentage);
			switch (TweenMode)
			{
			case Mode.Alpha:
				result.r = this.StartColor.r;
				result.g = this.StartColor.g;
				result.b = this.StartColor.b;
				break;
			case Mode.RGB:
				result.a = this.StartColor.a;
				break;
			}
			this.onColorChange.Invoke(result);
		}
	}

	/// <summary>
	/// An iterator to invoke as a coroutine.
	/// </summary>
	internal class Iterator : IEnumerator
	{
		/// <summary>
		/// The elapsed time.
		/// </summary>
		private float elapsedTime = 0f;
		/// <summary>
		/// The percentage completion.
		/// </summary>
		private float percentage = 0f;
		/// <summary>
		/// The tween info.
		/// </summary>
		private readonly Info tweenInfo;

		/// <summary>
		/// Initializes a new instance of the <see cref="Candlelight.ColorTween+Iterator"/> class.
		/// </summary>
		/// <param name="tweenInfo">Tween info.</param>
		public Iterator(Info tweenInfo)
		{
			this.tweenInfo = tweenInfo;
		}

		/// <summary>
		/// Gets the current value.
		/// </summary>
		/// <value>The current value.</value>
		public object Current { get { return null; } }

		/// <summary>
		/// Moves to the next item in the iterator.
		/// </summary>
		/// <returns><c>true</c>, if he iterator advanced, <c>false</c> otherwise.</returns>
		public bool MoveNext()
		{
			if (elapsedTime < tweenInfo.Duration)
			{
				elapsedTime += !tweenInfo.IgnoreTimeScale ? Time.deltaTime : Time.unscaledDeltaTime;
				percentage = Mathf.Clamp01(elapsedTime / tweenInfo.Duration);
				tweenInfo.TweenValue(percentage);
				return true;
			}
			tweenInfo.TweenValue(1f);
			return false;
		}

		/// <summary>
		/// Reset this instance.
		/// </summary>
		public void Reset()
		{
			throw new System.NotSupportedException();
		}
	}

	/// <summary>
	/// A class to run a color tween.
	/// </summary>
	public class Runner
	{
		/// <summary>
		/// The MonoBehaviour that will run the coroutine.
		/// </summary>
		private readonly MonoBehaviour coroutineContainer = null;
		/// <summary>
		/// The iterator.
		/// </summary>
		private IEnumerator iterator;

		/// <summary>
		/// Initializes a new instance of the <see cref="Candlelight.ColorTween.Runner"/> class.
		/// </summary>
		/// <param name="container">Container.</param>
		public Runner(MonoBehaviour container)
		{
			coroutineContainer = container;
		}

		/// <summary>
		/// Starts the tween.
		/// </summary>
		/// <param name="colorTweenInfo">Color tween info.</param>
		public void StartTween(Info colorTweenInfo)
		{
			if (iterator != null)
			{
				coroutineContainer.StopCoroutine(iterator);
				iterator = null;
			}
			if (!coroutineContainer.gameObject.activeInHierarchy)
			{
				colorTweenInfo.TweenValue(1f);
				return;
			}
			iterator = new Iterator(colorTweenInfo);
			coroutineContainer.StartCoroutine(iterator);
		}
	}
}