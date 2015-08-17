// 
// HyperText.cs
// 
// Copyright (c) 2014-2015, Candlelight Interactive, LLC
// All rights reserved.
// 
// This file is licensed according to the terms of the Unity Asset Store EULA:
// http://download.unity3d.com/assetstore/customer-eula.pdf
// 
// This file contains a subclass of the UnityEngine.UI.Text component with
// some additional features. It internally uses a HyperTextProcessor to
// support <a> tags and custom styling with user-defined tags and <quad>
// classes. See HyperTextProcessor for information on syntax, as well as
// automatic detection and tagging of keywords.
//
// Links extracted by the HyperTextProcessor are then colorized and emit
// callbacks when clicked. The text "Here is a <a name="some_link">link</a>"
// will render as "Here is a link", but with coloration and mouseover events for
// events for the word "link". When the word "link" is clicked, entered, or
// exited, the component will emit callbacks of type HyperlinkEvent specifying
// that a link with the id "some_link" was involved, along with its hit boxes.

#if (UNITY_4_6 && !UNITY_4_6_5) || UNITY_5_0
#define IS_TEXTGEN_SCALE_FACTOR_ABSENT
#endif

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Candlelight.UI
{
	/// <summary>
	/// Different color tint modes.
	/// </summary>
	public enum ColorTintMode
	{
		Multiplicative,
		Constant,
		Additive
	}

	/// <summary>
	/// A text component which can contain links and apply custom styles.
	/// </summary>
	/// <remarks>
	/// To use, wrap link text in <a> tags with a name attribute in the text property's value. This attribute becomes
	/// the id for the hitbox when events are handled. E.g., 'Some <a name="example">example</a> text.'
	/// </remarks>
	[AddComponentMenu("UI/Candlelight/HyperText"), ExecuteInEditMode]
	public class HyperText : UnityEngine.UI.Text,
		UnityEngine.UI.ILayoutElement,
		UnityEngine.EventSystems.IEventSystemHandler,
		UnityEngine.EventSystems.IPointerClickHandler,
		UnityEngine.EventSystems.IPointerDownHandler,
		UnityEngine.EventSystems.IPointerEnterHandler,
		UnityEngine.EventSystems.IPointerExitHandler,
		UnityEngine.EventSystems.IPointerUpHandler
	{
		/// <summary>
		/// Possible link selection states.
		/// </summary>
		internal enum LinkSelectionState
		{
			Normal,
			Highlighted,
			Pressed,
			Disabled
		}

		/// <summary>
		/// A class for storing information about a link indicated in the text.
		/// </summary>
		private class Link : TagGeometryData
		{
			/// <summary>
			/// Gets the name of the class.
			/// </summary>
			/// <value>The name of the class.</value>
			public string ClassName { get; private set; }
			/// <summary>
			/// Gets or sets the color.
			/// </summary>
			/// <value>The color.</value>
			public Color32 Color { get; set; }
			/// <summary>
			/// Gets or sets the color tween runner.
			/// </summary>
			/// <value>The color tween runner.</value>
			public ColorTween.Runner ColorTweenRunner { get; private set; }
			/// <summary>
			/// Gets the identifier.
			/// </summary>
			/// <value>The identifier.</value>
			public string Id { get; private set; }
			/// <summary>
			/// Gets the info.
			/// </summary>
			/// <value>The info.</value>
			public LinkInfo Info { get { return new LinkInfo(this.Id, this.ClassName, this.Hitboxes.ToArray()); } }
			/// <summary>
			/// Gets the hitboxes.
			/// </summary>
			/// <value>The hitboxes.</value>
			public List<Rect> Hitboxes { get; set; }
			/// <summary>
			/// Gets or sets the style.
			/// </summary>
			/// <value>The style.</value>
			public HyperTextStyles.Link Style { get; set; }
			/// <summary>
			/// Gets the vertical offset as a percentage of the surrounding line height.
			/// </summary>
			/// <value>The vertical offset as a percentage of the surrounding line height.</value>
			protected override float VerticalOffset { get { return Style.VerticalOffset; } }

			/// <summary>
			/// Initializes a new instance of the <see cref="Candlelight.UI.HyperText+Link"/> class.
			/// </summary>
			/// <param name="data">Data from a <see cref="Candlelight.UI.HyperTextProcessor"/>.</param>
			/// <param name="hyperText">Hyper text.</param>
			public Link(
				HyperTextProcessor.Link data, HyperText hyperText
			) : base((IndexRange)data.CharacterIndices.Clone())
			{
				this.Id = data.Id;
				this.ClassName = data.ClassName;
				this.ColorTweenRunner = new Candlelight.ColorTween.Runner(hyperText);
				this.Hitboxes = new List<Rect>();
				this.Style = data.Style;
				this.Color = hyperText.GetTargetLinkColorForState(
					hyperText.IsInteractable() ? LinkSelectionState.Normal : LinkSelectionState.Disabled, Style
				);
			}

			/// <summary>
			/// Tests whether this instance contains the specified ui position.
			/// </summary>
			/// <param name="uiPosition">User interface position in the space of this instance's RectTransform.</param>
			public bool Contains(Vector2 uiPosition)
			{
				foreach (Rect hitbox in Hitboxes)
				{
					if (hitbox.Contains(uiPosition))
					{
						return true;
					}
				}
				return false;
			}
		}

		/// <summary>
		/// A public structure with minimal information about a link.
		/// </summary>
		public struct LinkInfo
		{
			/// <summary>
			/// Gets the name of the class.
			/// </summary>
			/// <value>The name of the class.</value>
			public string ClassName { get; private set; }
			/// <summary>
			/// Gets the identifier.
			/// </summary>
			/// <value>The identifier.</value>
			public string Id { get; private set; }
			/// <summary>
			/// Gets the hitboxes.
			/// </summary>
			/// <value>The hitboxes.</value>
			public Rect[] Hitboxes { get; private set; }

			/// <summary>
			/// Initializes a new instance of the <see cref="Candlelight.UI.HyperText+LinkInfo"/> class.
			/// </summary>
			/// <param name="id">Identifier.</param>
			/// <param name="hitboxes">Hitboxes.</param>
			public LinkInfo(string id, string className, Rect[] hitboxes) : this()
			{
				this.Id = id;
				this.ClassName = className;
				this.Hitboxes = (Rect[])hitboxes.Clone();
			}
		}

		/// <summary>
		/// A class for storing information about a custom tag indicated in the text.
		/// </summary>
		private class CustomTag : TagGeometryData
		{
			/// <summary>
			/// Gets the style.
			/// </summary>
			/// <value>The style.</value>
			public HyperTextStyles.Text Style { get; private set; }
			/// <summary>
			/// Gets the vertical offset as a percentage of the surrounding line height.
			/// </summary>
			/// <value>The vertical offset as a percentage of the surrounding line height.</value>
			protected override float VerticalOffset { get { return Style.VerticalOffset; } }

			/// <summary>
			/// Initializes a new instance of the <see cref="Candlelight.UI.HyperText+CustomTag"/> class.
			/// </summary>
			/// <param name="data">Data from a <see cref="Candlelight.UI.HyperTextProcessor"/>.</param>
			public CustomTag(HyperTextProcessor.CustomTag data) : base((IndexRange)data.CharacterIndices.Clone())
			{
				Style = data.Style;
			}
		}

		/// <summary>
		/// A class for storing information about a quad indicated in the text.
		/// </summary>
		private class Quad : TagGeometryData
		{
			/// <summary>
			/// Gets or sets the renderer.
			/// </summary>
			/// <value>The renderer.</value>
			public CanvasRenderer Renderer { get; set; }
			/// <summary>
			/// Gets the rect transform.
			/// </summary>
			/// <value>The rect transform.</value>
			public RectTransform RectTransform
			{
				get { return Renderer == null ? null : Renderer.transform as RectTransform; }
			}
			/// <summary>
			/// Gets the style.
			/// </summary>
			/// <value>The style.</value>
			public HyperTextStyles.Quad Style { get; private set; }
			/// <summary>
			/// Gets the texture.
			/// </summary>
			/// <value>The texture.</value>
			public Texture2D Texture { get { return Style.Sprite == null ? null : Style.Sprite.texture; } }
			/// <summary>
			/// Gets the UV rect for the sprite.
			/// </summary>
			/// <value>The UV rect for the sprite.</value>
			public Rect UVRect
			{
				get
				{
					if (Style.Sprite == null)
					{
						return new Rect(0f, 0f, 1f, 1f);
					}
					Vector4 v = UnityEngine.Sprites.DataUtility.GetOuterUV(Style.Sprite);
					return new Rect(v.x, v.y, v.z - v.x, v.w - v.y);
				}
			}
			/// <summary>
			/// Gets the vertical offset as a percentage of the surrounding line height.
			/// </summary>
			/// <value>The vertical offset as a percentage of the surrounding line height.</value>
			protected override float VerticalOffset { get { return Style.VerticalOffset; } }
			/// <summary>
			/// Gets a list of UIVertex objects to be sent to this instance's CanvasRenderer.
			/// </summary>
			/// <value>A list of UIVertex objects to be sent to this instance's CanvasRenderer.</value>
			public List<UIVertex> Vertices { get; private set; }

			/// <summary>
			/// Initializes a new instance of the <see cref="Candlelight.UI.HyperText+Quad"/> class.
			/// </summary>
			/// <param name="data">Data from a <see cref="Candlelight.UI.HyperTextProcessor"/>.</param>
			public Quad(HyperTextProcessor.Quad data) : base((IndexRange)data.CharacterIndices.Clone())
			{
				Style = data.Style;
				Vertices = new List<UIVertex>();
			}
		}

		/// <summary>
		/// A base class for storing data about the geometry for a tag appearing in the text.
		/// </summary>
		private abstract class TagGeometryData
		{
			/// <summary>
			/// Gets or sets the character indices.
			/// </summary>
			/// <value>The character indices.</value>
			public IndexRange CharacterIndices { get; set; }
			/// <summary>
			/// Gets the list of indices for characters that are redrawn as a consequence of UI.IVertexModifier effects.
			/// </summary>
			/// <value>The list of redraw indices.</value>
			public List<IndexRange> RedrawIndices { get; private set; }
			/// <summary>
			/// Gets the vertical offset as a percentage of the surrounding line height.
			/// </summary>
			/// <value>The vertical offset as a percentage of the surrounding line height.</value>
			protected abstract float VerticalOffset { get; }

			/// <summary>
			/// Gets the vertical offset.
			/// </summary>
			/// <returns>The vertical offset.</returns>
			/// <param name="fontSize">Font size.</param>
			public float GetVerticalOffset(int fontSize)
			{
				return VerticalOffset * fontSize;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="Candlelight.UI.HyperText+TagGeometryData"/> class.
			/// </summary>
			/// <param name="indexRange">Index range.</param>
			public TagGeometryData(IndexRange indexRange)
			{
				CharacterIndices = indexRange;
				RedrawIndices = new List<IndexRange>();
			}
		}

		/// <summary>
		/// An event class for handling hyperlinks.
		/// </summary>
		[System.Serializable]
		public class HyperlinkEvent : UnityEngine.Events.UnityEvent<HyperText, LinkInfo> {}

		/// <summary>
		/// The color of untinted vertices.
		/// </summary>
		private static readonly Color32 s_UntintedVertexColor = Color.white;
		#region Shared Allocations
#pragma warning disable 414
		private static readonly List<CanvasGroup> m_CanvasGroups = new List<CanvasGroup>();
		private static List<HyperTextProcessor.Link> s_LinkCharacterData =
			new List<HyperTextProcessor.Link>(64);
		private static List<HyperTextProcessor.Quad> s_QuadCharacterData =
			new List<HyperTextProcessor.Quad>(64);
		private static Material s_StencilTrigger;
		private static List<HyperTextProcessor.CustomTag> s_TagCharacterData =
			new List<HyperTextProcessor.CustomTag>(64);
		private static readonly List<UIVertex> s_Vbo = new List<UIVertex>();
#pragma warning restore 414
		#endregion

		/// <summary>
		/// The custom tag geometry data extracted from the text.
		/// </summary>
		private List<CustomTag> m_CustomTagGeometryData = new List<CustomTag>();
		/// <summary>
		/// The default styles to use when a new component is added.
		/// </summary>
		[SerializeField, HideInInspector]
		private HyperTextStyles m_DefaultStyles = null;
		/// <summary>
		/// A flag to indicate whether the font texture changed callback should be invoked.
		/// </summary>
		private bool m_DisableFontTextureChangedCallback = false;
		/// <summary>
		/// A flag to keep track of whether interactability is permitted by any canvas groups.
		/// </summary>
		private bool m_DoGroupsAllowInteraction = true;
		/// <summary>
		/// The effects components on this instance.
		/// </summary>
		private List<Component> m_Effects = new List<Component>();
		/// <summary>
		/// An allocation;
		/// </summary>
		private Link m_HitboxCandidate = null;
		/// <summary>
		/// The hitbox under the cursor when the pointer down event is raised.
		/// </summary>
		private Link m_HitboxOnPointerDown = null;
		/// <summary>
		/// The hitbox under the cursor.
		/// </summary>
		private Link m_HitboxUnderCursor = null;
		/// <summary>
		/// The link geometry data extracted from the text.
		/// </summary>
		private List<Link> m_LinkGeometryData = new List<Link>();
		/// <summary>
		/// The most recent enter event camera.
		/// </summary>
		private Camera m_MostRecentEnterEventCamera;
		/// <summary>
		/// The quad material after the application of masking.
		/// </summary>
		private Material m_QuadMaskMaterial = null;
		/// <summary>
		/// The renderers for the quads.
		/// </summary>
		[SerializeField]
		private List<CanvasRenderer> m_QuadRenderersPool = new List<CanvasRenderer>();
		/// <summary>
		/// The quad geometry data extracted from the text.
		/// </summary>
		private List<Quad> m_QuadGeometryData = new List<Quad>();
		/// <summary>
		/// The quad tracker.
		/// </summary>
		DrivenRectTransformTracker m_QuadTracker = new DrivenRectTransformTracker();
		/// <summary>
		/// A flag indicating whether or not the external depenency callback should be invoked. Used to prevent dirtying
		/// during rebuild phase.
		/// </summary>
		private bool m_ShouldInvokeExternalDependencyCallback = true;
		/// <summary>
		/// The postprocessed string most recently sent to the TextGenerator.
		/// </summary>
		private string m_TextGeneratorInput = null;
		/// <summary>
		/// The UIVertices.
		/// </summary>
		private List<UIVertex> m_UIVertices = new List<UIVertex>();

		#region Backing Fields
		[SerializeField, UnityEngine.Serialization.FormerlySerializedAs("m_AreLinksEnabled"), PropertyBackingField]
		private bool m_Interactable = true;
		[SerializeField]
		private HyperlinkEvent m_OnClick = new HyperlinkEvent();
		[SerializeField]
		private HyperlinkEvent m_OnEnter = new HyperlinkEvent();
		[SerializeField]
		private HyperlinkEvent m_OnExit = new HyperlinkEvent();
		[SerializeField]
		private HyperlinkEvent m_OnPress = new HyperlinkEvent();
		[SerializeField]
		private HyperlinkEvent m_OnRelease = new HyperlinkEvent();
		[SerializeField]
		private bool m_ShouldOverrideStylesFontStyle = false;
		[SerializeField]
		private bool m_ShouldOverrideStylesFontColor = false;
		[SerializeField]
		private bool m_ShouldOverrideStylesLineSpacing = false;
		[SerializeField]
		private HyperTextProcessor m_TextProcessor = null;
		private Material m_QuadMaterial = null;
		#endregion

		[System.Obsolete("Use Interactable Property instead.")]
		public bool AreLinksEnabled
		{
			get { return this.Interactable; }
			set { this.Interactable = value; }
		}
		/// <summary>
		/// Gets the default link style.
		/// </summary>
		/// <value>The default link style.</value>
		public HyperTextStyles.Link DefaultLinkStyle
		{
			get { return Styles == null ? HyperTextStyles.Link.DefaultStyle : Styles.CascadedDefaultLinkStyle; }
		}
		/// <summary>
		/// Gets the color of the text.
		/// </summary>
		/// <value>The color of the text.</value>
		public Color DefaultTextColor
		{
			get
			{
				return Styles == null || m_ShouldOverrideStylesFontColor ? this.color : Styles.CascadedDefaultTextColor;
			}
		}
		/// <summary>
		/// Gets the style of the text.
		/// </summary>
		/// <value>The style of the text.</value>
		public FontStyle DefaultTextStyle
		{
			get
			{
				return Styles == null || m_ShouldOverrideStylesFontStyle ?
					this.fontStyle : Styles.CascadedDefaultFontStyle;
			}
		}
		/// <summary>
		/// Gets the font size to use.
		/// </summary>
		/// <value>The font size to use.</value>
		public int FontSizeToUse
		{
			get
			{
				return TextProcessor.ShouldOverrideStylesFontSize || Styles == null ?
					this.fontSize : Styles.CascadedFontSize;
			}
		}
		/// <summary>
		/// Gets the font to use.
		/// </summary>
		/// <value>The font to use.</value>
		public Font FontToUse
		{
			get { return this.font != null ? this.font : (Styles == null ? null : Styles.CascadedFont); }
		}
		/// <summary>
		/// Gets or sets the input text source. If a value is assigned, its OutputText will be used in place of the
		/// value in the text property of this <see cref="Candlelight.UI.HyperText"/>.
		/// </summary>
		/// <value>The input text source.</value>
		public ITextSource InputTextSource
		{
			get { return TextProcessor.InputTextSource; }
			set { TextProcessor.InputTextSource = value; }
		}
		/// <summary>
		/// Sets a value indicating whether links are interactable on this <see cref="Candlelight.HyperText"/>.
		/// </summary>
		/// <value><c>true</c> if links are interactable; otherwise, <c>false</c>.</value>
		public bool Interactable
		{
			get { return m_Interactable; }
			set
			{
				if (value == m_Interactable)
				{
					return;
				}
				m_Interactable = value;
				OnInterableChanged();
			}
		}
		/// <summary>
		/// Gets a value indicating whether this instance is a prefab.
		/// </summary>
		/// <value><c>true</c> if this instance is a prefab; otherwise, <c>false</c>.</value>
		private bool IsPrefab
		{
			get
			{
#if UNITY_EDITOR
				return UnityEditor.PrefabUtility.GetPrefabType(this) == UnityEditor.PrefabType.Prefab;
#else
				return false;
#endif
			}
		}
		/// <summary>
		/// Gets the main texture.
		/// </summary>
		/// <value>The main texture.</value>
		public override Texture mainTexture
		{
			get
			{
				if (FontToUse != null && FontToUse.material != null && FontToUse.material.mainTexture != null)
				{
					return FontToUse.material.mainTexture;
				}
				if (this.m_Material != null)
				{
					return this.m_Material.mainTexture;
				}
				return base.mainTexture;
			}
		}
		/// <summary>
		/// Gets the on click callback.
		/// </summary>
		/// <value>The on click callback.</value>
		public HyperlinkEvent OnClick { get { return m_OnClick; } }
		/// <summary>
		/// Gets the on enter callback.
		/// </summary>
		/// <value>The on enter callback.</value>
		public HyperlinkEvent OnEnter { get { return m_OnEnter; } }
		/// <summary>
		/// Gets the on exit callback.
		/// </summary>
		/// <value>The on exit callback.</value>
		public HyperlinkEvent OnExit { get { return m_OnExit; } }
		/// <summary>
		/// Gets the on press callback.
		/// </summary>
		/// <value>The on press callback.</value>
		public HyperlinkEvent OnPress { get { return m_OnPress; } }
		/// <summary>
		/// Gets the on release callback.
		/// </summary>
		/// <value>The on release callback.</value>
		public HyperlinkEvent OnRelease { get { return m_OnRelease; } }
		/// <summary>
		/// Gets the pixels per unit.
		/// </summary>
		/// <value>The pixels per unit.</value>
		new public float pixelsPerUnit
		{
			get
			{
				Canvas localCanvas = canvas;
				if (localCanvas == null)
				{
					return 1;
				}
				// For dynamic fonts, ensure we use one pixel per pixel on the screen.
				if (FontToUse == null || FontToUse.dynamic)
				{
					return localCanvas.scaleFactor;
				}
				// For non-dynamic fonts, calculate pixels per unit based on specified font size relative to font object's own font size.
				if (FontSizeToUse <= 0)
				{
					return 1;
				}
				return FontToUse.fontSize / (float)FontSizeToUse;
			}
		}
		/// <summary>
		/// Gets the preferred height for layout.
		/// </summary>
		/// <value>The preferred height for layout.</value>
		public override float preferredHeight
		{
			get
			{
				UpdateTextProcessor();
				return cachedTextGeneratorForLayout.GetPreferredHeight(
					TextProcessor.OutputText,
					GetGenerationSettings(new Vector2(rectTransform.rect.size.x, 0f))
				) / pixelsPerUnit;
			}
		}
		/// <summary>
		/// Gets the preferred width for layout.
		/// </summary>
		/// <value>The preferred width for layout.</value>
		public override float preferredWidth
		{
			get
			{
				UpdateTextProcessor();
				return cachedTextGeneratorForLayout.GetPreferredWidth(
					TextProcessor.OutputText,
					GetGenerationSettings(Vector2.zero)
				) / pixelsPerUnit;
			}
		}
		/// <summary>
		/// Gets the quad base material.
		/// </summary>
		/// <value>The quad base material.</value>
		private Material QuadBaseMaterial
		{
			get { return m_QuadMaterial == null ? defaultGraphicMaterial : m_QuadMaterial; }
		}
		/// <summary>
		/// Gets or sets the material to apply to quads.
		/// </summary>
		/// <value>The material to apply to quads.</value>
		public Material QuadMaterial
		{
			get
			{
				// trigger stencil update
				s_StencilTrigger = base.material;
				// return masked version if quads should be masked
				if (m_IncludeForMasking)
				{
					if (m_QuadMaskMaterial == null)
					{
						m_QuadMaskMaterial =
							UnityEngine.UI.StencilMaterial.Add(QuadBaseMaterial, (1 << m_StencilValue) - 1);
					}
					return m_QuadMaskMaterial ?? QuadBaseMaterial;
				}
				// otherwise return the result of the base material
				return QuadBaseMaterial;
			}
			set
			{
				if (m_QuadMaterial != value)
				{
					m_QuadMaterial = value;
					SetMaterialDirty();
				}
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Candlelight.UI.HyperText"/> should override the font
		/// color specified in styles, if one is assigned.
		/// </summary>
		/// <value><c>true</c> if should override the font color specified in styles; otherwise, <c>false</c>.</value>
		public bool ShouldOverrideStylesFontColor
		{
			get { return m_ShouldOverrideStylesFontColor; }
			set
			{
				if (m_ShouldOverrideStylesFontColor != value)
				{
					m_ShouldOverrideStylesFontColor = value;
					SetAllDirty();
				}
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Candlelight.UI.HyperText"/> should override the font
		/// size specified in styles, if one is assigned.
		/// </summary>
		/// <value><c>true</c> if should override the font size specified in styles; otherwise, <c>false</c>.</value>
		public bool ShouldOverrideStylesFontSize
		{
			get { return TextProcessor.ShouldOverrideStylesFontSize; }
			set { TextProcessor.ShouldOverrideStylesFontSize = value; }
		}
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Candlelight.UI.HyperText"/> should override the font
		/// face specified in styles, if one is assigned.
		/// </summary>
		/// <value><c>true</c> if should override the font face specified in styles; otherwise, <c>false</c>.</value>
		public bool ShouldOverrideStylesFontStyle
		{
			get { return m_ShouldOverrideStylesFontStyle; }
			set
			{
				if (m_ShouldOverrideStylesFontStyle != value)
				{
					m_ShouldOverrideStylesFontStyle = value;
					SetAllDirty();
				}
			}
		}
		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="Candlelight.UI.HyperText"/> should override the line
		/// spacing specified in styles, if one is assigned.
		/// </summary>
		/// <value><c>true</c> if should override the line spacing specified in styles; otherwise, <c>false</c>.</value>
		public bool ShouldOverrideStylesLineSpacing
		{
			get { return m_ShouldOverrideStylesLineSpacing; }
			set
			{
				if (m_ShouldOverrideStylesLineSpacing != value)
				{
					m_ShouldOverrideStylesLineSpacing = value;
					SetAllDirty();
				}
			}
		}
		/// <summary>
		/// Gets or sets the styles.
		/// </summary>
		/// <value>The styles.</value>
		public HyperTextStyles Styles
		{
			get { return TextProcessor.Styles; }
			set { TextProcessor.Styles = value; }
		}
		/// <summary>
		/// Gets the text processor.
		/// </summary>
		/// <value>The text processor.</value>
		private HyperTextProcessor TextProcessor
		{
			get
			{
				if (m_TextProcessor == null)
				{
					m_TextProcessor = new HyperTextProcessor();
					m_TextProcessor.OnBecameDirty.AddListener(OnExternalDependencyChanged);
					UpdateTextProcessor();
				}
				return m_TextProcessor;
			}
		}

		/// <summary>
		/// Clears the quad mask material.
		/// </summary>
		private void ClearQuadMaskMaterial()
		{
			if (m_QuadMaskMaterial != null)
			{
				UnityEngine.UI.StencilMaterial.Remove(m_QuadMaskMaterial);
			}
		}
		
		/// <summary>
		/// Does a state transition for the specified link.
		/// </summary>
		/// <param name="link">Link.</param>
		/// <param name="newState">New state.</param>
		private void DoLinkStateTransition(Link link, LinkSelectionState newState)
		{
			if (!IsActive() || link == null)
			{
				return;
			}
			Color targetColor = GetTargetLinkColorForState(newState, link.Style);
			if (link.Color == targetColor)
			{
				return;
			}
			ColorTween.Info colorTweenInfo = new ColorTween.Info(
				link.Style.Colors.fadeDuration, true, link.Color, targetColor, link.Style.ColorTweenMode
			);
			colorTweenInfo.AddOnChangedCallback(
				new UnityEngine.Events.UnityAction<Color>(value => link.Color = value)
			);
			link.ColorTweenRunner.StartTween(colorTweenInfo);
		}

		/// <summary>
		/// A callback to indicate the font texture has changed (mirrors that from base class).
		/// </summary>
		new public void FontTextureChanged()
		{
			if (this.Equals(null))
			{
				FontUpdateTracker.UntrackHyperText(this);
				return;
			}
			if (m_DisableFontTextureChangedCallback)
			{
				return;
			}
			cachedTextGenerator.Invalidate();
			if (!IsActive())
			{
				return;
			}
			if (
				UnityEngine.UI.CanvasUpdateRegistry.IsRebuildingGraphics() ||
				UnityEngine.UI.CanvasUpdateRegistry.IsRebuildingLayout()
			)
			{
				UpdateGeometry();
			}
			else
			{
				SetAllDirty();
			}
		}

		/// <summary>
		/// Gets the generation settings.
		/// </summary>
		/// <returns>The generation settings.</returns>
		/// <param name="extents">Extents.</param>
		new public TextGenerationSettings GetGenerationSettings(Vector2 extents)
		{
			TextGenerationSettings result = new TextGenerationSettings();
#if IS_TEXTGEN_SCALE_FACTOR_ABSENT
			result.generationExtents = extents * this.pixelsPerUnit + Vector2.one * 0.0001f; // Text.kEpsilon
			if (FontToUse != null && FontToUse.dynamic)
			{
				result.fontSize = Mathf.FloorToInt((float)FontSizeToUse * this.pixelsPerUnit);
				result.resizeTextMinSize = Mathf.FloorToInt((float)resizeTextMinSize * this.pixelsPerUnit);
				result.resizeTextMaxSize = Mathf.FloorToInt((float)resizeTextMaxSize * this.pixelsPerUnit);
			}
#else
			result.generationExtents = extents;
			if (FontToUse != null && FontToUse.dynamic)
			{
				result.fontSize = FontSizeToUse;
				result.resizeTextMinSize = resizeTextMinSize;
				result.resizeTextMaxSize = resizeTextMaxSize;
			}
			result.scaleFactor = this.pixelsPerUnit;
#endif
			result.textAnchor = this.alignment;
			result.color = DefaultTextColor;
			result.font = FontToUse;
			result.pivot = this.rectTransform.pivot;
			result.richText = this.supportRichText;
			result.lineSpacing = Styles == null ? this.lineSpacing : Styles.CascadedLineSpacing;
			result.fontStyle = DefaultTextStyle;
			result.resizeTextForBestFit = this.resizeTextForBestFit;
			result.updateBounds = false;
			result.horizontalOverflow = this.horizontalOverflow;
			result.verticalOverflow = this.verticalOverflow;
			return result;
		}

		/// <summary>
		/// Gets the link at the specified world position.
		/// </summary>
		/// <returns>The link at the specified world position.</returns>
		/// <param name="pointerPosition">Pointer position.</param>
		/// <param name="eventCamera">Event camera.</param>
		private Link GetLinkAtPointerPosition(Vector3 pointerPosition, Camera eventCamera)
		{
			if (eventCamera != null)
			{
				float distance;
				Ray ray = eventCamera.ScreenPointToRay(pointerPosition);
				if (!new Plane(-transform.forward, transform.position).Raycast(ray, out distance))
				{
					return null;
				}
				pointerPosition = ray.GetPoint(distance);
			}
			Vector3 uiPosition = transform.InverseTransformPoint(pointerPosition);
			foreach (Link link in m_LinkGeometryData)
			{
				if (link.Contains(uiPosition))
				{
					return link;
				}
			}
			return null;
		}

		/// <summary>
		/// Gets the link hitboxes.
		/// </summary>
		/// <param name="hitboxes">Hitboxes.</param>
		public void GetLinkHitboxes(ref List<LinkInfo> hitboxes)
		{
			hitboxes = hitboxes ?? new List<LinkInfo>(m_LinkGeometryData.Count);
			hitboxes.Clear();
			hitboxes.AddRange(from link in m_LinkGeometryData select link.Info);
		}

		/// <summary>
		/// Gets the link keyword collections.
		/// </summary>
		/// <param name="collections">Collections.</param>
		public void GetLinkKeywordCollections(ref List<HyperTextProcessor.KeywordCollectionClass> collections)
		{
			TextProcessor.GetLinkKeywordCollections(ref collections);
		}

		/// <summary>
		/// Gets the quad keyword collections.
		/// </summary>
		/// <param name="collections">Collections.</param>
		public void GetQuadKeywordCollections(ref List<HyperTextProcessor.KeywordCollectionClass> collections)
		{
			TextProcessor.GetQuadKeywordCollections(ref collections);
		}

		/// <summary>
		/// Gets the tag keyword collections.
		/// </summary>
		/// <param name="collections">Collections.</param>
		public void GetTagKeywordCollections(ref List<HyperTextProcessor.KeywordCollectionClass> collections)
		{
			TextProcessor.GetTagKeywordCollections(ref collections);
		}

		/// <summary>
		/// Gets the target link color for the specified state.
		/// </summary>
		/// <returns>The target link color for the specified state.</returns>
		/// <param name="state">A link select state.</param>
		private Color GetTargetLinkColorForState(LinkSelectionState state, HyperTextStyles.Link style)
		{
			Color baseColor = style.TextStyle.ShouldReplaceColor ? style.TextStyle.ReplacementColor : DefaultTextColor;
			Color stateColor = style.Colors.normalColor;
			switch (state)
			{
			case LinkSelectionState.Disabled:
				stateColor = style.Colors.disabledColor;
				break;
			case LinkSelectionState.Highlighted:
				stateColor = style.Colors.highlightedColor;
				break;
			case LinkSelectionState.Normal:
				stateColor = style.Colors.normalColor;
				break;
			case LinkSelectionState.Pressed:
				stateColor = style.Colors.pressedColor;
				break;
			}
			stateColor *= style.Colors.colorMultiplier;
			Color result = stateColor;
			switch (style.ColorTintMode)
			{
			case ColorTintMode.Additive:
				result = stateColor + baseColor;
				break;
			case ColorTintMode.Constant:
				result = stateColor;
				break;
			case ColorTintMode.Multiplicative:
				result = stateColor * baseColor;
				break;
			}
			switch (style.ColorTweenMode)
			{
			case Candlelight.ColorTween.Mode.RGB:
				result.a = baseColor.a;
				break;
			case Candlelight.ColorTween.Mode.Alpha:
				result.r = baseColor.r;
				result.g = baseColor.g;
				result.b = baseColor.b;
				break;
			}
			return result;
		}
		
		/// <summary>
		/// Determines whether this instance is interactable.
		/// </summary>
		/// <returns><c>true</c> if this instance is interactable; otherwise, <c>false</c>.</returns>
		protected bool IsInteractable()
		{
			return m_DoGroupsAllowInteraction && m_Interactable;
		}

		/// <summary>
		/// Raises the canvas group changed event. Copied from UnityEngine.UI.Selectable.
		/// </summary>
		protected override void OnCanvasGroupChanged()
		{
			// figure out if parent groups allow interaction
			bool doGroupsAllowInteraction = true;
			Transform t = transform;
			while (t != null)
			{
				t.GetComponents(m_CanvasGroups);
				for (var i = 0; i < m_CanvasGroups.Count; ++i)
				{
					if (!m_CanvasGroups[i].interactable)
					{
						doGroupsAllowInteraction = false;
						break;
					}
					if (m_CanvasGroups[i].ignoreParentGroups)
					{
						break;
					}
				}
				t = t.parent;
			}
			m_CanvasGroups.Clear();
			// trigger a state change if needed
			if (doGroupsAllowInteraction != m_DoGroupsAllowInteraction)
			{
				m_DoGroupsAllowInteraction = doGroupsAllowInteraction;
				OnInterableChanged();
			}
		}

		/// <summary>
		/// Raises the click link event.
		/// </summary>
		/// <param name="link">Link.</param>
		private void OnClickLink(Link link)
		{
			if (link == m_HitboxOnPointerDown && link != null)
			{
				m_OnClick.Invoke(this, link.Info);
			}
			m_HitboxOnPointerDown = null; // NOTE: done here because this event is raised after OnPointerUp()
		}
		
		/// <summary>
		/// Raises the destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			base.OnDestroy();
			TextProcessor.Dispose();
		}
		
		/// <summary>
		/// Raises the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();
			FontUpdateTracker.UntrackHyperText(this);
			ClearQuadMaskMaterial();
			foreach (CanvasRenderer quadRenderer in m_QuadRenderersPool)
			{
				if (quadRenderer != null)
				{
					quadRenderer.Clear();
				}
			}
		}

		/// <summary>
		/// Raises the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			FontUpdateTracker.TrackHyperText(this);
			TextProcessor.OnEnable();
			TextProcessor.OnBecameDirty.RemoveListener(OnExternalDependencyChanged);
			TextProcessor.OnBecameDirty.AddListener(OnExternalDependencyChanged);
			OnExternalDependencyChanged();
		}
		
		/// <summary>
		/// Raises the enter link event.
		/// </summary>
		/// <param name="link">Link.</param>
		private void OnEnterLink(Link link)
		{
			// do nothing if the link is already under the cursor
			if (m_HitboxUnderCursor == link)
			{
				return;
			}
			// process as a press if e.g., click started, moved off, and moved back on
			if (m_HitboxUnderCursor == null && m_HitboxOnPointerDown != null && link == m_HitboxOnPointerDown)
			{
				m_HitboxUnderCursor = link;
				m_HitboxCandidate = m_HitboxOnPointerDown;
				m_HitboxOnPointerDown = null;
				OnPressLink(m_HitboxCandidate);
			}
			else
			{
				// otherwise exit the link previously under the cursor
				OnExitLink(m_HitboxUnderCursor);
				// store the link under the cursor and highlight it
				m_HitboxUnderCursor = link;
				if (m_HitboxUnderCursor != null)
				{
					DoLinkStateTransition(m_HitboxUnderCursor, LinkSelectionState.Highlighted);
					m_OnEnter.Invoke(this, m_HitboxUnderCursor.Info);
				}
			}
		}
		
		/// <summary>
		/// Raises the exit link event.
		/// </summary>
		/// <param name="link">Link.</param>
		private void OnExitLink(Link link)
		{
			// do nothing if null
			if (link == null)
			{
				return;
			}
			// clear link under cursor if it was exited
			if (link == m_HitboxUnderCursor)
			{
				m_HitboxUnderCursor = null;
			}
			// transition supplied link back to normal
			DoLinkStateTransition(link, LinkSelectionState.Normal);
			// fire off events
			m_OnExit.Invoke(this, link.Info);
		}

		/// <summary>
		/// Raises the external dependency changed event.
		/// </summary>
		private void OnExternalDependencyChanged()
		{
			if (m_ShouldInvokeExternalDependencyCallback)
			{
				FontUpdateTracker.UntrackHyperText(this);
				FontUpdateTracker.TrackHyperText(this);
				cachedTextGenerator.Invalidate();
				SetAllDirty();
			}
		}

		/// <summary>
		/// Raises the fill VBO event.
		/// </summary>
		/// <param name="vertexBufferObject">Vertex buffer object.</param>
		protected override void OnFillVBO(List<UIVertex> vertexBufferObject)
		{
			if (FontToUse == null)
			{
				return;
			}
			m_DisableFontTextureChangedCallback = true;
			Rect inputRect = rectTransform.rect;
			cachedTextGenerator.Populate(PostprocessText(), GetGenerationSettings(inputRect.size));
			Vector2 textAnchorPivot = GetTextAnchorPivot(alignment);
			Vector2 refPoint = Vector2.zero;
			refPoint.x = (textAnchorPivot.x == 1f) ? inputRect.xMax : inputRect.xMin;
			refPoint.y = (textAnchorPivot.y == 0f) ? inputRect.yMin : inputRect.yMax;
			Vector2 roundingOffset = PixelAdjustPoint(refPoint) - refPoint;
			IList<UIVertex> vertices = cachedTextGenerator.verts;
			UIVertex vertex;
			float unitsPerPixel = 1f / pixelsPerUnit;
			if (roundingOffset != Vector2.zero)
			{
				for (int i = 0; i < vertices.Count; ++i)
				{
					vertex = vertices[i];
					vertex.position *= unitsPerPixel;
					vertex.position.x = vertex.position.x + roundingOffset.x;
					vertex.position.y = vertex.position.y + roundingOffset.y;
					vertexBufferObject.Add(vertex);
				}
			}
			else
			{
				for (int i = 0; i < vertices.Count; ++i)
				{
					vertex = vertices[i];
					vertex.position *= unitsPerPixel;
					vertexBufferObject.Add(vertex);
				}
			}
			m_UIVertices.Clear();
			m_UIVertices.AddRange(vertexBufferObject);
			m_DisableFontTextureChangedCallback = false;
		}

		/// <summary>
		/// Raises the interable changed event, which initiates link state transitions.
		/// </summary>
		private void OnInterableChanged()
		{
			// if application is not playing, do immediate color change
			if (!Application.isPlaying)
			{
				UpdateGeometry();
			}
			// otherwise initiate state transition
			else
			{
				// NOTE: Unity always triggers UpdateGeometry() from inspector, so manual transition is immediate
				LinkSelectionState newState =
					IsInteractable() ? LinkSelectionState.Normal : LinkSelectionState.Disabled;
				foreach (Link link in m_LinkGeometryData)
				{
					DoLinkStateTransition(link, newState);
				}
			}
		}
		
		/// <summary>
		/// Raises the pointer click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
		{
			if (!IsActive())
			{
				return;
			}
			OnClickLink(GetLinkAtPointerPosition(eventData.position, eventData.pressEventCamera));
		}

		/// <summary>
		/// Raises the pointer down event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
		{
			if (!IsActive())
			{
				return;
			}
			OnPressLink(GetLinkAtPointerPosition(eventData.pressPosition, eventData.pressEventCamera));
		}
		
		/// <summary>
		/// Updates the mouseover state.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
		{
			if (!IsActive())
			{
				return;
			}
			m_MostRecentEnterEventCamera = eventData.enterEventCamera;
			OnEnterLink(GetLinkAtPointerPosition(eventData.position, eventData.enterEventCamera));
		}

		
		/// <summary>
		/// Updates the mouseover state.
		/// </summary>
		/// <param name="pointerData">Pointer data.</param>
		public void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
		{
			if (!IsActive() || m_HitboxUnderCursor == null)
			{
				return;
			}
			OnExitLink(m_HitboxUnderCursor);
		}
		
		/// <summary>
		/// Raises the pointer up event.
		/// </summary>
		/// <param name="pointerData">Pointer data.</param>
		public void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
		{
			if (!IsActive())
			{
				return;
			}
			OnReleaseLink(GetLinkAtPointerPosition(eventData.position, eventData.pressEventCamera));
		}
		
		/// <summary>
		/// Raises the press link event.
		/// </summary>
		/// <param name="link">Link.</param>
		private void OnPressLink(Link link)
		{
			if (m_HitboxOnPointerDown == link || link == null)
			{
				return;
			}
			m_HitboxOnPointerDown = link;
			DoLinkStateTransition(m_HitboxOnPointerDown, LinkSelectionState.Pressed);
			m_OnPress.Invoke(this, m_HitboxOnPointerDown.Info);
		}

#if UNITY_EDITOR
		/// <summary>
		/// Raises the rebuild requested event.
		/// </summary>
		public override void OnRebuildRequested()
		{
			FontUpdateTracker.UntrackHyperText(this);
			FontUpdateTracker.TrackHyperText(this);
			base.OnRebuildRequested();
		}
#endif

		/// <summary>
		/// Raises the release link event.
		/// </summary>
		/// <param name="link">Link.</param>
		private void OnReleaseLink(Link link)
		{
			if (link != m_HitboxOnPointerDown || m_HitboxOnPointerDown == null)
			{
				return;
			}
			DoLinkStateTransition(m_HitboxOnPointerDown, LinkSelectionState.Highlighted);
			m_OnRelease.Invoke(this, m_HitboxOnPointerDown.Info);
		}

#if UNITY_EDITOR
		/// <summary>
		/// Raises the validate event.
		/// </summary>
		protected override void OnValidate()
		{
			base.OnValidate();
			ClearQuadMaskMaterial();
		}
#endif

		/// <summary>
		/// Using the supplied list of vertex modifiers and the text generator input string, apply offsets to character
		/// index ranges in the supplied tables to reflect how the vertex modifiers shift the list of UI vertices.
		/// Override this method if you need to account for custom UI.IVertexModifiers that use more discriminating
		/// methods.
		/// </summary>
		/// <param name="vertexModifiers">All vertex modifiers on the object.</param>
		/// <param name="textGeneratorInputValue">The string submitted to cachedTextGenerator.</param>
		/// <param name="customTagCharacterIndices">
		/// Range of character indices for all link tags, custom text style tags, and <quad> tags (plus subsequent quad
		/// character in TextGenerator output).
		/// </param>
		/// <param name="customTagRedrawCharacterIndices">
		/// Ranges of character indices for any redrawn links, custom text, and quad geometry in TextGenerator output.
		/// </param>
		protected virtual void PostprocessCharacterIndexRanges(
			System.Collections.ObjectModel.ReadOnlyCollection<Behaviour> vertexModifiers,
			string textGeneratorInputValue,
			IndexRange[] customTagCharacterIndices,
			List<IndexRange>[] customTagRedrawCharacterIndices
		)
		{
			// determine number of draws
			int numDraws = 1;
			foreach (Behaviour vertexModifier in vertexModifiers)
			{
				if (!vertexModifier.enabled)
				{
					continue;
				}
				if (vertexModifier is UnityEngine.UI.Outline) // inherits from Shadow, so test first
				{
					numDraws *= 5;
				}
				else if (vertexModifier is UnityEngine.UI.Shadow)
				{
					numDraws *= 2;
				}
			}
			// determine offset amount (NOTE: use actual vertices generated to account for clipping)
			cachedTextGenerator.Populate(textGeneratorInputValue, GetGenerationSettings(rectTransform.rect.size));
			int charWrapPerDraw = cachedTextGenerator.vertexCount / 4;
			int tagWrap = charWrapPerDraw * (numDraws - 1);
			// offset index ranges for custom tags and populate lists of index ranges for redrawn characters
			for (int tagIndex = 0; tagIndex < customTagRedrawCharacterIndices.Length; ++tagIndex)
			{
				IndexRange tagRange = customTagCharacterIndices[tagIndex];
				for (int drawPass = 0; drawPass < numDraws - 1; ++drawPass)
				{
					int scroll = drawPass * charWrapPerDraw;
					customTagRedrawCharacterIndices[tagIndex].Add(
						new IndexRange(tagRange.StartIndex + scroll, tagRange.EndIndex + scroll)
					);
				}
				tagRange.StartIndex += tagWrap;
				tagRange.EndIndex += tagWrap;
			}
		}

		/// <summary>
		/// Postprocess the text data before submitting it to cachedTextGenerator.
		/// </summary>
		private string PostprocessText()
		{
			UpdateTextProcessor();
			// clear existing data
			m_LinkGeometryData.Clear();
			m_CustomTagGeometryData.Clear();
			m_QuadGeometryData.Clear();
			m_QuadRenderersPool.RemoveAll(quadRenderer => quadRenderer == null);
			foreach (CanvasRenderer quadRenderer in m_QuadRenderersPool)
			{
				quadRenderer.Clear();
			}
			// copy link data
			TextProcessor.GetLinks(ref s_LinkCharacterData);
			for (int i = 0; i < s_LinkCharacterData.Count; ++i)
			{
				m_LinkGeometryData.Add(new Link(s_LinkCharacterData[i], this));
			}
			s_LinkCharacterData.Clear();
			// set up other rich tags if enabled
			if (TextProcessor.IsRichTextEnabled)
			{
				// add custom text style tag geometry data
				TextProcessor.GetCustomTags(ref s_TagCharacterData);
				for (int i = 0; i < s_TagCharacterData.Count; ++i)
				{
					m_CustomTagGeometryData.Add(new CustomTag(s_TagCharacterData[i]));
				}
				// set up quads if the current object is not a prefab
				if (!IsPrefab)
				{
					m_QuadTracker.Clear();
					RectTransform quadTransform = null;
					TextProcessor.GetQuads(ref s_QuadCharacterData);
					for (int matchIndex = 0; matchIndex < s_QuadCharacterData.Count; ++matchIndex)
					{
						// TODO: switch over to ObjectX.GetFromPool()
						// add new quad data to list
						m_QuadGeometryData.Add(new Quad(s_QuadCharacterData[matchIndex]));
						// grow pool if needed
						if (matchIndex >= m_QuadRenderersPool.Count)
						{
							GameObject newQuadObject =
								new GameObject("<quad>", typeof(RectTransform), typeof(CanvasRenderer));
							m_QuadRenderersPool.Add(newQuadObject.GetComponent<CanvasRenderer>());
#if UNITY_EDITOR
							// ensure changes to prefab instances' pools get serialized when not selected
							if (!Application.isPlaying && !IsPrefab)
							{
								UnityEditor.EditorUtility.SetDirty(this);
							}
#endif
						}
						// make sure layer is the same
						m_QuadRenderersPool[matchIndex].gameObject.layer = this.gameObject.layer;
						// lock transform
						quadTransform = m_QuadRenderersPool[matchIndex].transform as RectTransform;
						quadTransform.SetParent(this.rectTransform);
						m_QuadTracker.Add(this, quadTransform, DrivenTransformProperties.All);
						quadTransform.anchorMax = Vector2.one;
						quadTransform.anchorMin = Vector2.zero;
						quadTransform.sizeDelta = Vector2.zero;
						quadTransform.pivot = rectTransform.pivot;
						quadTransform.localPosition = Vector3.zero;
						quadTransform.localRotation = Quaternion.identity;
						quadTransform.localScale = Vector3.one;
						// configure quad
						m_QuadGeometryData[matchIndex].Renderer = m_QuadRenderersPool[matchIndex];
						m_QuadGeometryData[matchIndex].Renderer.Clear();
						m_QuadGeometryData[matchIndex].Renderer.SetMaterial(QuadMaterial, m_QuadGeometryData[matchIndex].Texture);
					}
				}
			}
			m_TextGeneratorInput = TextProcessor.OutputText;
			return m_TextGeneratorInput;
		}

		/// <summary>
		/// A custom raycast callback to determine if there is a link hitbox under the pointer position.
		/// </summary>
		/// <returns><c>true</c>, if pointer position is over a link hitbox, <c>false</c> otherwise.</returns>
		/// <param name="pointerPosition">Pointer position.</param>
		/// <param name="eventCamera">Event camera.</param>
		public override bool Raycast(Vector2 pointerPosition, Camera eventCamera)
		{
			// early out if links are disabled or base raycast fails
			if (!IsInteractable() || !base.Raycast(pointerPosition, eventCamera))
			{
				return false;
			}
			m_HitboxCandidate = GetLinkAtPointerPosition(pointerPosition, eventCamera);
			if (m_HitboxCandidate == null)
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Sets the link keyword collections.
		/// </summary>
		/// <param name="value">Value.</param>
		private void SetLinkKeywordCollections(HyperTextProcessor.KeywordCollectionClass[] value)
		{
			SetLinkKeywordCollections(value as IEnumerable<HyperTextProcessor.KeywordCollectionClass>);
		}

		/// <summary>
		/// Sets the link keyword collections.
		/// </summary>
		/// <param name="value">Value.</param>
		public void SetLinkKeywordCollections(IEnumerable<HyperTextProcessor.KeywordCollectionClass> value)
		{
			TextProcessor.SetLinkKeywordCollections(value);
		}

		/// <summary>
		/// Sets the material dirty.
		/// </summary>
		public override void SetMaterialDirty()
		{
			base.SetMaterialDirty();
			ClearQuadMaskMaterial();
		}
		
		/// <summary>
		/// Sets the quad keyword collections.
		/// </summary>
		/// <param name="value">Value.</param>
		private void SetQuadKeywordCollections(HyperTextProcessor.KeywordCollectionClass[] value)
		{
			SetQuadKeywordCollections(value as IEnumerable<HyperTextProcessor.KeywordCollectionClass>);
		}

		/// <summary>
		/// Sets the quad keyword collections.
		/// </summary>
		/// <param name="value">Value.</param>
		public void SetQuadKeywordCollections(IEnumerable<HyperTextProcessor.KeywordCollectionClass> value)
		{
			TextProcessor.SetQuadKeywordCollections(value);
		}
		
		/// <summary>
		/// Sets the tag keyword collections.
		/// </summary>
		/// <param name="value">Value.</param>
		private void SetTagKeywordCollections(HyperTextProcessor.KeywordCollectionClass[] value)
		{
			SetTagKeywordCollections(value as IEnumerable<HyperTextProcessor.KeywordCollectionClass>);
		}

		/// <summary>
		/// Sets the tag keyword collections.
		/// </summary>
		/// <param name="value">Value.</param>
		private void SetTagKeywordCollections(IEnumerable<HyperTextProcessor.KeywordCollectionClass> value)
		{
			TextProcessor.SetTagKeywordCollections(value);
		}
		
		/// <summary>
		/// Initialize this <see cref="Candlelight.UI.HyperText"/>.
		/// </summary>
		protected override void Start()
		{
			base.Start();
			Styles = Styles == null ? m_DefaultStyles : Styles;
		}

		/// <summary>
		/// Update vertex colors on this instance.
		/// </summary>
		private void Update()
		{
			if (Application.isPlaying)
			{
				// force state transitions if link under cursor changed; prevents false misses from fast mouse movements
				if (Input.mousePresent && m_HitboxUnderCursor != null)
				{
					m_HitboxCandidate = GetLinkAtPointerPosition(Input.mousePosition, m_MostRecentEnterEventCamera);
					if (m_HitboxCandidate != m_HitboxUnderCursor)
					{
						OnEnterLink(m_HitboxCandidate);
					}
				}
				UpdateVertexColors(); // TODO: move vertex color animations to coroutines
			}
		}
		
		/// <summary>
		/// Updates the geometry.
		/// </summary>
		protected override void UpdateGeometry()
		{
			if (FontToUse == null)
			{
				return;
			}
			m_ShouldInvokeExternalDependencyCallback = false;
			// populate cachedTextGenerator, links, quads, and uiVertices
			// do not call base implementation of UpdateGeometry(), as it requires this.font to be set
			if (rectTransform != null && rectTransform.rect.width >= 0f && rectTransform.rect.height >= 0f)
			{
				OnFillVBO(s_Vbo);
			}
			s_Vbo.Clear();
			// set final positions for quads
			UIVertex vertex;
			for (int quadIndex = 0; quadIndex < m_QuadGeometryData.Count; ++quadIndex)
			{
				int uiVertexIndex = m_QuadGeometryData[quadIndex].CharacterIndices.EndIndex * 4;
				// shift vertices by vertical offset if they are not clipped
				if (uiVertexIndex + 3 <= m_UIVertices.Count)
				{
					for (int idxScroll = 0; idxScroll < 4; ++idxScroll)
					{
						vertex = m_UIVertices[uiVertexIndex + idxScroll];
						vertex.position += Vector3.up * m_QuadGeometryData[quadIndex].GetVerticalOffset(FontSizeToUse);
						m_UIVertices[uiVertexIndex + idxScroll] = vertex;
					}
				}
			}
			// populate hitboxes of links
			UpdateLinkHitboxRects();
			// get all the effects on this object
			m_Effects.Clear();
			GetComponents(typeof(UnityEngine.UI.IVertexModifier), m_Effects);
			// offset values in character index tables to account for vertex modifier effects
			List<IndexRange> customTagRanges = new List<IndexRange>();
			customTagRanges.AddRange(from l in m_LinkGeometryData select l.CharacterIndices);
			customTagRanges.AddRange(from q in m_QuadGeometryData select q.CharacterIndices);
			customTagRanges.AddRange(from t in m_CustomTagGeometryData select t.CharacterIndices);
			List<List<IndexRange>> customTagRedrawRanges = new List<List<IndexRange>>();
			customTagRedrawRanges.AddRange(from l in m_LinkGeometryData select l.RedrawIndices);
			customTagRedrawRanges.AddRange(from q in m_QuadGeometryData select q.RedrawIndices);
			customTagRedrawRanges.AddRange(from t in m_CustomTagGeometryData select t.RedrawIndices);
			PostprocessCharacterIndexRanges(
				new System.Collections.ObjectModel.ReadOnlyCollection<Behaviour>(
					m_Effects.ToArray().Cast<Behaviour>().ToArray()
				), m_TextGeneratorInput,
				customTagRanges.ToArray(),
				customTagRedrawRanges.ToArray()
			);
			// apply any vertex modification effects to cached vertices
			foreach (Behaviour effect in m_Effects)
			{
				if (!effect.enabled)
				{
					continue;
				}
				(effect as UnityEngine.UI.IVertexModifier).ModifyVertices(m_UIVertices);
			}
			// apply vertical offsets to all link and custom text styles
			Vector3 offset;
			List<IndexRange> ranges = new List<IndexRange>();
			foreach (
				TagGeometryData tagData in
				Enumerable.Concat(m_LinkGeometryData.Cast<TagGeometryData>(), m_CustomTagGeometryData.Cast<TagGeometryData>())
			)
			{
				offset = tagData.GetVerticalOffset(FontSizeToUse) * Vector3.up;
				ranges.Clear();
				ranges.Add(tagData.CharacterIndices);
				ranges.AddRange(tagData.RedrawIndices);
				foreach (IndexRange range in ranges)
				{
					foreach (int charIndex in range)
					{
						int uiVertexIndex = charIndex * 4;
						for (int scrollIndex = 0; scrollIndex < 4; ++scrollIndex)
						{
							if (uiVertexIndex + scrollIndex >= m_UIVertices.Count)
							{
								continue;
							}
							vertex = m_UIVertices[uiVertexIndex + scrollIndex];
							vertex.position += offset;
							m_UIVertices[uiVertexIndex + scrollIndex] = vertex;
						}
					}
				}
			}
			// initialize quad vertex lists and degenerate vertices in source lise
			Vector2[] uvTransform = new Vector2[4];
			for (int quadIndex = 0; quadIndex < m_QuadGeometryData.Count; ++quadIndex)
			{
				Rect uv = m_QuadGeometryData[quadIndex].UVRect;
				uvTransform[0] = new Vector2(uv.min.x, uv.max.y); // (0, 1)
				uvTransform[1] = new Vector2(uv.max.x, uv.max.y); // (1, 1)
				uvTransform[2] = new Vector2(uv.max.x, uv.min.y); // (1, 0)
				uvTransform[3] = new Vector2(uv.min.x, uv.min.y); // (0, 0)
				// add redrawn vertices first
				foreach (IndexRange range in m_QuadGeometryData[quadIndex].RedrawIndices)
				{
					foreach (int charIndex in range)
					{
						int uiVertexIndex = charIndex * 4;
						for (int scrollIndex = 0; scrollIndex < 4; ++scrollIndex)
						{
							if (uiVertexIndex + scrollIndex >= m_UIVertices.Count)
							{
								continue;
							}
							vertex = m_UIVertices[uiVertexIndex + scrollIndex];
							vertex.uv0 = uvTransform[scrollIndex];
							m_QuadGeometryData[quadIndex].Vertices.Add(vertex);
							// degenerate
							m_UIVertices[uiVertexIndex + scrollIndex] = m_UIVertices[uiVertexIndex];
						}
					}
				}
				foreach (int charIndex in m_QuadGeometryData[quadIndex].CharacterIndices)
				{
					int uiVertexIndex = charIndex * 4;
					for (int scrollIndex = 0; scrollIndex < 4; ++scrollIndex)
					{
						if (uiVertexIndex + scrollIndex >= m_UIVertices.Count)
						{
							continue;
						}
						vertex = m_UIVertices[uiVertexIndex + scrollIndex];
						vertex.uv0 = uvTransform[scrollIndex];
						if (!m_QuadGeometryData[quadIndex].Style.ShouldRespectColorization)
						{
							vertex.color = Color.white;
						}
						m_QuadGeometryData[quadIndex].Vertices.Add(vertex);
						// degenerate
						m_UIVertices[uiVertexIndex + scrollIndex] = m_UIVertices[uiVertexIndex];
					}
				}
			}
			// update the renderer to set link colors
			UpdateVertexColors();
			m_ShouldInvokeExternalDependencyCallback = true;
		}
		
		/// <summary>
		/// Updates the link hitbox rects.
		/// </summary>
		private void UpdateLinkHitboxRects()
		{
			IList<UILineInfo> lines = cachedTextGenerator.lines;
			List<int[]> lineBreaks = new List<int[]>();
			Bounds bounds;
			foreach (Link link in m_LinkGeometryData)
			{
				link.Hitboxes.Clear();
				if (m_UIVertices.Count == 0)
				{
					continue;
				}
				lineBreaks.Clear();
				lineBreaks.Add(new int[] { link.CharacterIndices.StartIndex, link.CharacterIndices.EndIndex });
				foreach (UILineInfo line in lines)
				{
					if (
						line.startCharIdx > link.CharacterIndices.StartIndex &&
						line.startCharIdx <= link.CharacterIndices.EndIndex
					)
					{
						lineBreaks[lineBreaks.Count - 1][1] = line.startCharIdx - 1;
						lineBreaks.Add(new int[] { line.startCharIdx, link.CharacterIndices.EndIndex });
					}
				}
				Vector3 linkVerticalOffset = Vector3.up * link.GetVerticalOffset(FontSizeToUse);
				foreach (int[] lineBreak in lineBreaks)
				{
					int startVertexIndex = Mathf.Min(lineBreak[0] * 4, m_UIVertices.Count - 1);
					bounds = new Bounds(m_UIVertices[startVertexIndex].position + linkVerticalOffset, Vector3.zero);
					for (
						int uiVertexIndex = startVertexIndex;
						uiVertexIndex < Mathf.Min(lineBreak[1] * 4 + 4, m_UIVertices.Count);
						++uiVertexIndex
					)
					{
						// determine if point is in a custom tag or quad
						int uiCharInfoIndex = uiVertexIndex / 4;
						int tagIndex = m_CustomTagGeometryData.FindIndex(
							tag =>
							uiCharInfoIndex >= tag.CharacterIndices.StartIndex &&
							uiCharInfoIndex < tag.CharacterIndices.EndIndex
						);
						int quadIndex = m_QuadGeometryData.FindIndex(
							quad =>
								uiCharInfoIndex >= quad.CharacterIndices.StartIndex &&
								uiCharInfoIndex < quad.CharacterIndices.EndIndex
						);
						// if the UIVertex is part of a custom tag or quad, then offset and skip ahead
						if (quadIndex >= 0 || tagIndex >= 0)
						{
							TagGeometryData data = quadIndex >= 0 ?
								(TagGeometryData)m_QuadGeometryData[quadIndex] : (TagGeometryData)m_CustomTagGeometryData[tagIndex];
							Vector3 offset = linkVerticalOffset + Vector3.up * data.GetVerticalOffset(FontSizeToUse);
							int tagStartVertexIndex = 4 * m_CustomTagGeometryData[tagIndex].CharacterIndices.StartIndex;
							int tagEndVertexIndex = 4 * m_CustomTagGeometryData[tagIndex].CharacterIndices.EndIndex + 3;
							if (tagStartVertexIndex < m_UIVertices.Count)
							{
								// re-initialize bounds if the tag geometry is at the start of the link
								if (uiVertexIndex == startVertexIndex)
								{
									bounds =
										new Bounds(m_UIVertices[tagStartVertexIndex].position + offset, Vector3.zero);
								}
								// encapsulate all of the proper text
								for (int idx = tagStartVertexIndex; idx <= tagEndVertexIndex; ++idx)
								{
									if (idx >= m_UIVertices.Count)
									{
										continue;
									}
									bounds.Encapsulate(m_UIVertices[idx].position + offset);
								}
								// advance counter to outside of tag
								uiVertexIndex += 4 * data.CharacterIndices.Count;
							}
						}
						else
						{
							bounds.Encapsulate(m_UIVertices[uiVertexIndex].position + linkVerticalOffset);
						}
					}
					link.Hitboxes.Add(new Rect(bounds.min.x, bounds.min.y, bounds.size.x, bounds.size.y));
				}
			}
		}

		/// <summary>
		/// Updates the text processor.
		/// </summary>
		private void UpdateTextProcessor()
		{
			TextProcessor.ReferenceFontSize = FontSizeToUse;
			TextProcessor.InputText = text;
			TextProcessor.IsDynamicFontDesired = FontToUse != null && FontToUse.dynamic;
			TextProcessor.IsRichTextDesired = supportRichText;
#if IS_TEXTGEN_SCALE_FACTOR_ABSENT
			TextProcessor.ScaleFactor = pixelsPerUnit;
#else
			TextProcessor.ScaleFactor = 1f;
#endif
		}
		
		/// <summary>
		/// Updates the canvas renderers, including link vertex colors.
		/// </summary>
		private void UpdateVertexColors()
		{
			UIVertex vertex;
			// colorize links
			foreach (Link link in m_LinkGeometryData)
			{
				for (
					int uiVertexIndex = link.CharacterIndices.StartIndex * 4;
					uiVertexIndex < Mathf.Min(link.CharacterIndices.EndIndex * 4 + 4, m_UIVertices.Count);
					++uiVertexIndex
				)
				{
					vertex = m_UIVertices[uiVertexIndex];
					vertex.color = link.Color;
					m_UIVertices[uiVertexIndex] = vertex;
				}
			}
			// colorize quads and set the vertices on managed CanvasRenderers
			bool swizzleQuadRB = // TextGenerator swaps R and B of quad vertex colors on DX9 and lower
				SystemInfo.graphicsDeviceVersion.StartsWith("Direct3D") &&
				SystemInfo.graphicsShaderLevel <= 30 &&
				SystemInfo.graphicsDeviceVersion != "Direct3D 9.0c [emulated]"; // bug won't appear with emulation
			for (int quadIndex = 0; quadIndex < m_QuadGeometryData.Count; ++quadIndex)
			{
				int uiVertexIndex = m_QuadGeometryData[quadIndex].CharacterIndices.EndIndex * 4;
				// empty out renderers for quads that are clipped
				if (uiVertexIndex + 3 > m_UIVertices.Count)
				{
					m_QuadGeometryData[quadIndex].Renderer.Clear();
				}
				else
				{
					// copy colors from vertex list and apply to quad renderer
					int end = m_QuadGeometryData[quadIndex].Vertices.Count;
					for (int scrollIndex = 0; scrollIndex < 4; ++scrollIndex)
					{
						if (uiVertexIndex + scrollIndex >= m_UIVertices.Count)
						{
							continue;
						}
						vertex = m_QuadGeometryData[quadIndex].Vertices[end - 1 - scrollIndex];
						vertex.color = m_QuadGeometryData[quadIndex].Style.ShouldRespectColorization ?
							m_UIVertices[uiVertexIndex + scrollIndex].color : s_UntintedVertexColor;
						if (swizzleQuadRB)
						{
							vertex.color = new Color32(vertex.color.b, vertex.color.g, vertex.color.r, vertex.color.a);
						}
						m_QuadGeometryData[quadIndex].Vertices[end - 1 - scrollIndex] = vertex;
					}
					m_QuadGeometryData[quadIndex].Renderer.SetVertices(m_QuadGeometryData[quadIndex].Vertices);
				}
			}
			canvasRenderer.SetVertices(m_UIVertices);
		}

		#region Obsolete
		[System.Obsolete("Use void HyperText.GetLinkHitboxes(ref List<LinkInfo> hitboxes)")]
		public LinkInfo[] GetLinkHitboxes()
		{
			return (from link in m_LinkGeometryData select link.Info).ToArray();
		}
		[System.Obsolete("Use void HyperText.GetLinkKeywordCollections(ref List<HyperTextProcessor.KeywordCollectionClass> collections)")]
		public HyperTextProcessor.KeywordCollectionClass[] GetLinkKeywordCollections()
		{
			return TextProcessor.GetLinkKeywordCollections();
		}
		[System.Obsolete("Use void HyperText.GetTagKeywordCollections(ref List<HyperTextProcessor.KeywordCollectionClass> collections)")]
		public HyperTextProcessor.KeywordCollectionClass[] GetTagKeywordCollections()
		{
			return TextProcessor.GetTagKeywordCollections();
		}
		#endregion
	}
}