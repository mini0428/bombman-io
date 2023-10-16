using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Util
{
    public static class UIUtil
    {
        public static string CoinFormat(long val)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0:#,###,###,##0}", val);
            return sb.ToString();
        }

        public static string CoinFormat(int val)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0:#,###,###,##0}", val);
            return sb.ToString();
        }

        public static string DateTimeFormat(int totalSec)
        {
            int remainSec = totalSec % 60;
            totalSec /= 60;
            int remainMin = totalSec % 60;
            totalSec /= 60;
            int remainHour = totalSec % 24;

            return $"{remainHour:00}:{remainMin:00}:{remainSec:00}";
        }
    
        public static string DateTimeDayFormat(long totalSec)
        {
            long remainSec = totalSec % 60;
            totalSec /= 60;
            long remainMin = totalSec % 60;
            totalSec /= 60;
            long remainHour = totalSec % 24;
            totalSec /= 24;
        
            if( totalSec > 0 )
                return $"{totalSec}D-{remainHour:00}:{remainMin:00}:{remainSec:00}";
            else
                return $"{remainHour:00}:{remainMin:00}:{remainSec:00}";
        }

        public static string DateTimeFormat(long totalSec)
        {
            long remainSec = totalSec % 60;
            totalSec /= 60;
            long remainMin = totalSec % 60;
            totalSec /= 60;
            long remainHour = totalSec % 24;

            return $"{remainHour:00}:{remainMin:00}:{remainSec:00}";
        }
    
        public static string DateTimeMinFormat(long totalSec)
        {
            long remainSec = totalSec % 60;
            totalSec /= 60;
            long remainMin = totalSec % 60;

            return $"{remainMin:00}:{remainSec:00}";
        }
    
        public static string DynamicDateTimeFormat(long totalSec)
        {
            long remainSec = totalSec % 60;
            totalSec /= 60;
            long remainMin = totalSec % 60;
            totalSec /= 60;
            long remainHour = totalSec;

            if( remainHour <= 0 )
                return $"{remainMin:00}:{remainSec:00}";
            else
                return $"{remainHour:00}:{remainMin:00}:{remainSec:00}";
        }


        public static void ScrollCenter(ScrollRect scrollRect, GameObject content, GameObject selectItem, bool isVertical = true)
        {
            if (selectItem == null)
                return;

            var target = selectItem.GetComponent<RectTransform>();
            var contentRc = content.GetComponent<RectTransform>();
            RectTransform scrollRc = scrollRect.GetComponent<RectTransform>();

            //this is the center point of the visible area
            var maskHalfSize = scrollRc.rect.size * 0.5f;
            var contentSize = contentRc.rect.size;
            //get object position inside content
            var targetRelativePosition =
                contentRc.InverseTransformPoint(target.position);
            //adjust for item size
            targetRelativePosition += new Vector3(target.rect.size.x, target.rect.size.y, 0f) * 0.25f;
            //get the normalized position inside content
            var normalizedPosition = new Vector2(
                Mathf.Clamp01(targetRelativePosition.x / (contentSize.x - maskHalfSize.x)),
                1f - Mathf.Clamp01(targetRelativePosition.y / -(contentSize.y - maskHalfSize.y))
            );
            //we want the position to be at the middle of the visible area
            //so get the normalized center offset based on the visible area width and height
            var normalizedOffsetPosition = new Vector2(maskHalfSize.x / contentSize.x, maskHalfSize.y / contentSize.y);
            //and apply it
            normalizedPosition.x -= (1f - normalizedPosition.x) * normalizedOffsetPosition.x;
            normalizedPosition.y += normalizedPosition.y * normalizedOffsetPosition.y;

            normalizedPosition.x = Mathf.Clamp01(normalizedPosition.x);
            normalizedPosition.y = Mathf.Clamp01(normalizedPosition.y);
        
            if( isVertical )
                scrollRect.verticalNormalizedPosition = normalizedPosition.y;
            else
                scrollRect.horizontalNormalizedPosition = normalizedPosition.x;
        }
    
        public static Vector3 WorldToUISpace(Camera mainCam, Camera uiCam, Canvas canvas, Vector3 worldPos)
        {
            Vector3 screenPos = mainCam.WorldToScreenPoint(worldPos);
            Vector2 movePos;
        
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, 
                screenPos, 
                uiCam, out movePos);
            return canvas.transform.TransformPoint(movePos);
        }
    
        public static Vector2 GetAnchoredPositionFromWorldPosition(Camera _camera, Canvas _canvas, Vector3 _worldPostion)
        {
            Vector2 myPositionOnScreen = _camera.WorldToViewportPoint(_worldPostion); //for RectTransform.AnchoredPosition?
            float scaleFactor = _canvas.scaleFactor;
            return new Vector2(myPositionOnScreen.x / scaleFactor, myPositionOnScreen.y / scaleFactor);
        }
    }
}