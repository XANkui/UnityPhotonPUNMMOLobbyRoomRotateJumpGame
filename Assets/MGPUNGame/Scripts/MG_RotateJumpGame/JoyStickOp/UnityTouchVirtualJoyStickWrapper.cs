using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XANUtil
{

    public class UnityTouchVirtualJoyStickWrapper
    {
        private Vector2 mOldPos;
        private Vector2 mLstPos;
        private IUnityTouchVirtualJoyStickCallback mCallback;

        private float mCountTime = 0;
        private float mCountTargetTime = 0.2f;
        private bool mIsFirst = false;
        private bool mIsLongPress = false;

        public void Init(IUnityTouchVirtualJoyStickCallback callback)
        {
            mCallback = callback;
        }

        public void Update()
        {
            // OldTouchListener();

            OldTouchListenerAndClick();
        }

        void OldTouchListener() {
            if (Input.GetMouseButtonDown(0))
            {
                mOldPos = Input.mousePosition;
                mCallback?.TouchDownPos(mOldPos);
            }
            else if (Input.GetMouseButton(0))
            {

                mLstPos = Input.mousePosition;
                Vector2 deltaPosition = (mLstPos - mOldPos);

                mCallback?.TouchDownMovePos(mLstPos);
                mCallback?.TouchDownMoveDeltraPos(deltaPosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                mCallback?.TouchUp();
            }
        }

        void OldTouchListenerAndClick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                mOldPos = Input.mousePosition;
                mCallback?.TouchDownPos(mOldPos);

                mIsFirst = true;
            }
            else if (Input.GetMouseButton(0))
            {

                mLstPos = Input.mousePosition;
                Vector2 deltaPosition = (mLstPos - mOldPos);

                mCallback?.TouchDownMovePos(mLstPos);
                mCallback?.TouchDownMoveDeltraPos(deltaPosition);
            }
            else if (Input.GetMouseButtonUp(0))
            {
                mCallback?.TouchUp();

                if (mCountTime<mCountTargetTime)
                {
                    mCallback?.TouchClick();
                }

                mIsFirst = false;
                mCountTime = 0;
            }

            if (mIsFirst)
            {
                mCountTime += Time.deltaTime;
            }
        }

        void NewTest() {
            if (Input.GetMouseButtonDown(0))
            {
                mIsFirst = true;
                mOldPos = Input.mousePosition;
                mCallback?.TouchDownPos(mOldPos);
            }
            if (Input.GetMouseButtonUp(0))
            {
                mCallback?.TouchUp();

                mIsFirst = false;
                mCountTime = 0;
                mIsLongPress = false;
            }

            if (Input.GetMouseButton(0))
            {

                mLstPos = Input.mousePosition;
                Vector2 deltaPosition = (mLstPos - mOldPos);

                mCallback?.TouchDownMovePos(mLstPos);
                mCallback?.TouchDownMoveDeltraPos(deltaPosition);
            }

            if (mIsFirst)
            {
                mCountTime += Time.deltaTime;
            }

            if (mIsFirst == true)
            {
                if (mCountTime >= mCountTargetTime)
                {
                    mIsLongPress = false;
                }
                else
                {
                    mIsLongPress = true;
                }
            }
        }
    }
}