using System;
using System.Windows.Shapes;

namespace EnemyEditor
{
    
    public class CControllerEventArgs : EventArgs
    {
        public Ellipse Sprite { get; set; }

        public string Message { get; set; }

        public BigNumber Points { get; set; }

        public string EventType { get; set; }

        public CControllerEventArgs(Ellipse sprite)
        {
            Sprite = sprite;
            Message = "";
            Points = new BigNumber(0);
            EventType = "ObjectEvent";
        }

        public CControllerEventArgs(string message)
        {
            Sprite = null;
            Message = message;
            Points = new BigNumber(0);
            EventType = "GameEvent";
        }

        public CControllerEventArgs(BigNumber points)
        {
            Sprite = null;
            Message = "";
            Points = points;
            EventType = "PointsEvent";
        }

        public CControllerEventArgs(Ellipse sprite, string message)
        {
            Sprite = sprite;
            Message = message;
            Points = new BigNumber(0);
            EventType = "ObjectEvent";
        }

        public CControllerEventArgs(string message, BigNumber points)
        {
            Sprite = null;
            Message = message;
            Points = points;
            EventType = "PointsEvent";
        }
    }
}