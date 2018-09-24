using System;
using System.Collections;
using System.Collections.Generic;
using GQ.Client.Util;
using UnityEngine;
using UnityEngine.UI;

namespace GQ.Client.UI
{
    public class FullScreenImageScrollRect : ScrollRect
    {
        [SerializeField] float _minZoom = 0.5f;
        [SerializeField] float _maxZoom = 10f;
        [SerializeField] float _zoomLerpSpeed = 10f;

        float _currentZoom = 1;
        bool _isPinching = false;
        float _startPinchDist;
        float _startPinchZoom;
        Vector2 _startPinchCenterPosition;
        Vector2 _startPinchScreenPosition;
        float _mouseWheelSensitivity = 1;
        bool blockPan = false;

        protected override void Awake()
        {
            Input.multiTouchEnabled = true;
        }

        protected override void Start()
        {
            base.Start();
            _minZoom = 0.5f;
        }

        public void Open(RawImage image)
        {
            Texture = image.texture;
            // set the enclosing canvas active:
            transform.parent.gameObject.SetActive(true);
            ApplyARF();
        }

        DeviceOrientation orientation;


        private void Update()
        {
            if (Input.deviceOrientation != orientation)
            {
                OnDeviceOrientationChanged();
            }


            if (isDoubleTapped() || isDoubleClicked())
            {
                OnDoubleClick();
            }

            if (Input.touchCount == 2)
            {
                if (!_isPinching)
                {
                    _isPinching = true;
                    OnPinchStart();
                }
                OnPinch();
            }
            else
            {
                _isPinching = false;
                if (Input.touchCount == 0)
                {
                    blockPan = false;
                }
            }

            float scrollWheelInput = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollWheelInput) > float.Epsilon)
            {
                _currentZoom *= 1 + scrollWheelInput * _mouseWheelSensitivity;
                _currentZoom = Mathf.Clamp(_currentZoom, _minZoom, _maxZoom);
                _startPinchScreenPosition = (Vector2)Input.mousePosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(content, _startPinchScreenPosition, null, out _startPinchCenterPosition);
                Vector2 pivotPosition = new Vector3(content.pivot.x * content.rect.size.x, content.pivot.y * content.rect.size.y);
                //Debug.Log("pivotPosition POS: x: " + pivotPosition.x + ", y: " + pivotPosition.y);
                Vector2 posFromBottomLeft = pivotPosition + _startPinchCenterPosition;
                //Debug.Log("PIVOT posFromBottomLeft: x: " + posFromBottomLeft.x + ", y: " + posFromBottomLeft.y);
                SetPivot(content, new Vector2(posFromBottomLeft.x / content.rect.width, posFromBottomLeft.y / content.rect.height));
            }
            //pc input end

            if (Mathf.Abs(content.localScale.x - _currentZoom) > 0.001f)
                content.localScale = Vector3.Lerp(content.localScale, Vector3.one * _currentZoom, _zoomLerpSpeed * Time.deltaTime);

            if (_currentZoom < 0.6)
            {
                _currentZoom = 1f;
                Close();
            }
        }

        private void OnDeviceOrientationChanged()
        {
            orientation = Input.deviceOrientation;
            switch (orientation)
            {
                case DeviceOrientation.Portrait:
                case DeviceOrientation.Unknown:
                    Debug.Log("DO CHANGED to PORTRAIT");
                    content.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    break;
                case DeviceOrientation.LandscapeLeft:
                    Debug.Log("DO CHANGED to LandscapeLeft");
                    content.localRotation = Quaternion.Euler(0f, 0f, -90f);
                    break;
                case DeviceOrientation.LandscapeRight:
                    Debug.Log("DO CHANGED to LandscapeRight");
                    content.localRotation = Quaternion.Euler(0f, 0f, 90f);
                    break;
                case DeviceOrientation.PortraitUpsideDown:
                    content.localRotation = Quaternion.Euler(0f, 0f, 180f);
                    Debug.Log("DO CHANGED to PortraitUpsideDown");
                    break;
                default:
                    break;
            }

            Debug.Log("rotation: x: " + content.localRotation.x + ", y:" + content.localRotation.y + ", z:" + content.localRotation.z + ", w:" + content.localRotation.w);
        }

        protected override void SetContentAnchoredPosition(Vector2 position)
        {
            if (_isPinching || blockPan) return;
            base.SetContentAnchoredPosition(position);
        }

        void OnPinchStart()
        {
            Vector2 pos1 = Input.touches[0].position;
            Vector2 pos2 = Input.touches[1].position;

            _startPinchDist = Distance(pos1, pos2) * content.localScale.x;
            _startPinchZoom = _currentZoom;
            _startPinchScreenPosition = (pos1 + pos2) / 2;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(content, _startPinchScreenPosition, null, out _startPinchCenterPosition);

            Vector2 pivotPosition = new Vector3(content.pivot.x * content.rect.size.x, content.pivot.y * content.rect.size.y);
            Vector2 posFromBottomLeft = pivotPosition + _startPinchCenterPosition;

            SetPivot(content, new Vector2(posFromBottomLeft.x / content.rect.width, posFromBottomLeft.y / content.rect.height));
            blockPan = true;
        }

        void OnPinch()
        {
            float currentPinchDist = Distance(Input.touches[0].position, Input.touches[1].position) * content.localScale.x;
            _currentZoom = (currentPinchDist / _startPinchDist) * _startPinchZoom;
            _currentZoom = Mathf.Clamp(_currentZoom, _minZoom, _maxZoom);
        }

        float Distance(Vector2 pos1, Vector2 pos2)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(content, pos1, null, out pos1);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(content, pos2, null, out pos2);
            return Vector2.Distance(pos1, pos2);
        }

        static void SetPivot(RectTransform rectTransform, Vector2 pivot)
        {
            //Debug.Log("PIVOT: x: " + pivot.x + ", y: " + pivot.y);

            if (rectTransform == null) return;

            Vector2 size = rectTransform.rect.size;
            Vector2 deltaPivot = rectTransform.pivot - pivot;
            Vector3 deltaPosition = new Vector3(deltaPivot.x * size.x, deltaPivot.y * size.y) * rectTransform.localScale.x;
            //rectTransform.pivot = pivot;
            rectTransform.localPosition -= deltaPosition;
            //Debug.Log("SETPIVOT localscale: x: " + rectTransform.localScale.x + ", y: " + rectTransform.localScale.y);


        }

        private RawImage _image;
        public RawImage Image
        {
            get
            {
                if (_image == null)
                {
                    _image = content.GetComponent<RawImage>();
                }
                return _image;
            }
            set
            {
                _image = value;
            }
        }

        public Texture Texture
        {
            get
            {
                return Image.texture;
            }
            set
            {
                Image.rectTransform.sizeDelta = new Vector2(
                     (float)value.width,
                     (float)value.height
                 );

                Image.texture = value;
            }
        }

        public void Close()
        {
            RectTransform rt = Image.GetComponent<RectTransform>();

            aspectRatioFitter.enabled = false;
            transform.parent.gameObject.SetActive(false);
            rt.localScale = new Vector3(1f, 1f, 1f);
        }

        #region adapt image scale

        AspectRatioFitter _aspectRatioFitter;
        AspectRatioFitter aspectRatioFitter
        {
            get
            {
                if (_aspectRatioFitter == null)
                {
                    _aspectRatioFitter = content.GetComponent<AspectRatioFitter>();
                }
                return _aspectRatioFitter;
            }
            set
            {
                _aspectRatioFitter = value;
            }
        }


        public void ApplyARF()
        {
            RectTransform rt = Image.GetComponent<RectTransform>();
            Debug.Log("BEFORE ARF: w: " + rt.rect.width + ", h: " + rt.rect.height);
            aspectRatioFitter.enabled = true;
            aspectRatioFitter.aspectMode = AspectRatioFitter.AspectMode.FitInParent;
            aspectRatioFitter.aspectRatio =
                (float)Image.texture.width / (float)Image.texture.height;
            Debug.Log("AFTER ARF: w: " + rt.rect.width + ", h: " + rt.rect.height);
            rt.localScale = new Vector3(1f, 1f, 1f);
        }

        #endregion



        #region double tap & click

        private bool isDoubleTapped()
        {
            // detect double tap on touchscreens:
            for (int i = 0; i < Input.touchCount; ++i)
            {
                if (Input.touches[0].tapCount == 2)
                {
                    return true;
                }
            }
            return false;
        }


        private bool isSecondClick = false;
        private float timeFirstClick = 0f;
        private float doubleClickTimeSpan = 0.3f;

        private bool isDoubleClicked()
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (isSecondClick)
                {
                    float now = Time.time;
                    if (now < timeFirstClick + doubleClickTimeSpan)
                    {
                        isSecondClick = false;
                        return true;
                    }
                    else
                    {
                        // span was too long, we start a new span with this click
                        timeFirstClick = now;
                        return false;
                    }
                }
                else
                {
                    // this is the first click, next will be second:
                    isSecondClick = true;
                    timeFirstClick = Time.time;
                    return false;
                }
            }
            return false;
        }

        void OnDoubleClick()
        {
            // normalize zoom by double tap or click:
            if (Input.deviceOrientation == DeviceOrientation.LandscapeLeft || Input.deviceOrientation == DeviceOrientation.LandscapeRight)
                _currentZoom = (float) Device.height / (float) Device.width;
            else
                _currentZoom = 1f;
        }

        #endregion
    }
}