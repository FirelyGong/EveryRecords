using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Views.Animations;

namespace EveryRecords
{
    [Activity(Label = "IntroductionActivity", Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen")]
    public class IntroductionActivity : Activity, GestureDetector.IOnGestureListener  
    {
        private ViewFlipper _imagesContainer;
        private GestureDetector _gestureDetector;
        private int[] _imageIDs = { Resource.Drawable.a, Resource.Drawable.b, Resource.Drawable.c,  
            Resource.Drawable.d, Resource.Drawable.e };

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Create your application here
            SetContentView(Resource.Layout.IntroductionLayout);

            _imagesContainer = FindViewById<ViewFlipper>(Resource.Id.ImagesView);
            _gestureDetector = new GestureDetector(this);

            for (int i = 0; i < _imageIDs.Length; i++)
            {
                // 定义一个ImageView对象  
                ImageView image = new ImageView(this);
                image.Id = i;
                image.SetImageResource(_imageIDs[i]);
                // 充满父控件  
                image.SetScaleType(ImageView.ScaleType.FitXy);
                // 添加到viewFlipper中  
                _imagesContainer.AddView(image, new ViewGroup.LayoutParams(
                        ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));
            } 
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            return _gestureDetector.OnTouchEvent(e);
        }

        public bool OnDown(MotionEvent e)
        {
            return false;
        }

        public bool OnFling(MotionEvent arg0, MotionEvent arg1, float velocityX, float velocityY)
        {
            //对手指滑动的距离进行了计算，如果滑动距离大于120像素，就做切换动作，否则不做任何切换动作。  
            // 从右向左滑动  
            if (arg0.GetX() - arg1.GetX() > 120)
            {
                if(_imagesContainer.CurrentView.Id==4)
                {
                    Finish();
                    return false;
                }

                // 添加动画  
                _imagesContainer.SetInAnimation(this,
                        Resource.Animation.PushLeftIn);
                _imagesContainer.SetOutAnimation(this,
                        Resource.Animation.PushLeftOut);
                _imagesContainer.ShowNext();
                return true;
            }// 从左向右滑动  
            else if (arg0.GetX() - arg1.GetX() < -120)
            {
                if (_imagesContainer.CurrentView.Id == 0)
                {
                    return false;
                }

                _imagesContainer.SetInAnimation(this,
                        Resource.Animation.PushRightIn);
                _imagesContainer.SetOutAnimation(this,
                        Resource.Animation.PushRightOut);
                _imagesContainer.ShowPrevious();
                return true;
            }
            return true;
        }

        public void OnLongPress(MotionEvent e)
        {
        }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            return false;
        }

        public void OnShowPress(MotionEvent e)
        {
        }

        public bool OnSingleTapUp(MotionEvent e)
        {
            return false;
        }
    }
}