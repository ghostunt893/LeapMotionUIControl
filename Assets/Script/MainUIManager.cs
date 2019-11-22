using DG.Tweening;
using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Granden.Health
{
    public enum PageStatus
    {
        PageOne = 0,
        PageTwo = 1,
        PageThree = 2
    }

    public class MainUIManager : MonoBehaviour
    {
        [SerializeField]
        private Image[] _outlines;
        [SerializeField]
        private Image[] _xRay;
        [SerializeField]
        private ScrollRect _scrollRect;
        [SerializeField]
        private GameObject _panel2;
        [SerializeField]
        private GameObject _panel3;
        [SerializeField]
        private GameObject _bodyImage;
        [SerializeField]
        private PinchDetector _pinchDetector_right;

        private int _currentOutline;
        private int _currentXRay;
        private PageStatus _pageStatus;
        private Vector3 _pinchPositionScroll;
        private Vector3 _pinchPositionScale;

        // Start is called before the first frame update
        void Start()
        {
            SceneInit();
        }

        // Update is called once per frame
        void Update()
        {
            //if (Input.GetKeyDown(KeyCode.LeftArrow))
            //{
            //    LeftSelect();
            //}
            //else if (Input.GetKeyDown(KeyCode.RightArrow))
            //{
            //    RightSelect();
            //}

            if (_pinchDetector_right.DidStartPinch)
            {
                PinchCheck(0,_pinchDetector_right.Position);
            }

            if (_pinchDetector_right.DidEndPinch)
            {
                PinchCheck(1, _pinchDetector_right.Position);
            }

            if (_pinchDetector_right.IsPinching)
            {
                PinchCheck(2, _pinchDetector_right.Position);
            }
        }

        private void SceneInit()
        {
            _currentOutline = 0;
            _currentXRay = 0;
            _pageStatus = PageStatus.PageOne;

            foreach (Image image in _outlines)
            {
                image.color = Color.white;
                image.DOColor(Color.yellow, .7f).SetLoops(-1);
            }
        }

        public void SwipeGestureDetect(string direct)
        {
            switch (direct)
            {
                case "eRight":
                    RightSelect();
                    break;
                case "eLeft":
                    LeftSelect();
                    break;
                case "eOutwards":
                    if(_pageStatus == PageStatus.PageOne && _currentOutline == 2)
                    {
                        _panel2.SetActive(true);
                        _pageStatus = PageStatus.PageTwo;
                    }
                    else if (_pageStatus == PageStatus.PageOne && _currentOutline == 0)
                    {
                        _panel3.SetActive(true);
                        _pageStatus = PageStatus.PageThree;
                    }
                    break;
                case "eInWards":
                    if(_pageStatus == PageStatus.PageTwo)
                    {
                        _panel2.SetActive(false);
                        _pageStatus = PageStatus.PageOne;
                    }
                    else if(_pageStatus == PageStatus.PageThree)
                    {
                        _panel3.SetActive(false);
                        _pageStatus = PageStatus.PageOne;
                    }
                    break;
                default:
                    break;
            }
        }

        private void RightSelect()
        {
            if (_pageStatus == PageStatus.PageOne)
            {
                if (_currentOutline + 1 <= 5)
                {
                    _outlines[_currentOutline].gameObject.SetActive(false);
                    _outlines[_currentOutline + 1].gameObject.SetActive(true);
                    _currentOutline = _currentOutline + 1;
                }
            }
            else if(_pageStatus == PageStatus.PageTwo)
            {
                if (_currentXRay + 1 <= 1)
                {
                    _xRay[_currentXRay].gameObject.SetActive(false);
                    _xRay[_currentXRay + 1].gameObject.SetActive(true);
                    _currentXRay = _currentXRay + 1;
                }
            }
        }

        private void LeftSelect()
        {
            if (_pageStatus == PageStatus.PageOne)
            {
                if (_currentOutline - 1 >= 0)
                {
                    _outlines[_currentOutline].gameObject.SetActive(false);
                    _outlines[_currentOutline - 1].gameObject.SetActive(true);
                    _currentOutline = _currentOutline - 1;
                }
            }
            else if(_pageStatus == PageStatus.PageTwo)
            {
                if (_currentXRay - 1 >= 0)
                {
                    _xRay[_currentXRay].gameObject.SetActive(false);
                    _xRay[_currentXRay - 1].gameObject.SetActive(true);
                    _currentXRay = _currentXRay - 1;
                }
            }
        }

        private void PinchCheck(int status, Vector3 position)
        {
            if (_currentOutline == 4)
            {
                if (status == 0)
                {
                    _pinchPositionScroll = position;
                }

                if (status == 2)
                {
                    if (position.y > _pinchPositionScroll.y)
                    {
                        _scrollRect.normalizedPosition = new Vector2(_scrollRect.normalizedPosition.x, _scrollRect.normalizedPosition.y + (position.y - _pinchPositionScroll.y) * 3);
                    }
                    else if (position.y < _pinchPositionScroll.y)
                    {
                        _scrollRect.normalizedPosition = new Vector2(_scrollRect.normalizedPosition.x, _scrollRect.normalizedPosition.y - (_pinchPositionScroll.y - position.y) * 3);
                    }
                    _pinchPositionScroll = position;
                }
            }
            else if(_currentOutline == 0)
            {
                if (status == 0)
                {
                    _pinchPositionScale = position;
                }

                if (status == 2)
                {
                    if (position.x > _pinchPositionScale.x)
                    {
                        float offset = position.x - _pinchPositionScale.x;
                        //_scrollRect.normalizedPosition = new Vector2(_scrollRect.normalizedPosition.x, _scrollRect.normalizedPosition.y + (position.y - _pinchPositionScale.y) * 3);
                        if (_bodyImage.transform.localScale.x < 0.8f)
                        {
                            _bodyImage.transform.localScale = new Vector3(_bodyImage.transform.localScale.x + offset, _bodyImage.transform.localScale.y + offset, _bodyImage.transform.localScale.z + offset);
                        }
                    }
                    else if (position.x < _pinchPositionScale.x)
                    {
                        float offset = _pinchPositionScale.x - position.x;
                        if(_bodyImage.transform.localScale.x > 0.3f)
                        {
                            _bodyImage.transform.localScale = new Vector3(_bodyImage.transform.localScale.x - offset, _bodyImage.transform.localScale.y - offset, _bodyImage.transform.localScale.z - offset);
                        }
                        //_scrollRect.normalizedPosition = new Vector2(_scrollRect.normalizedPosition.x, _scrollRect.normalizedPosition.y - (_pinchPositionScale.y - position.y) * 3);
                    }
                    _pinchPositionScale = position;
                }
            }
        }
    }
}