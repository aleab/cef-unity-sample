using System;
using UnityEngine;


namespace Aleab.CefUnity.Structs
{
    [Serializable]
    public struct Size
    {
        [SerializeField]
        private int _width;

        [SerializeField]
        private int _height;

        public int Width
        {
            get { return this._width; }
            private set { this._width = value; }
        }

        public int Height
        {
            get { return this._height; }
            private set { this._height = value; }
        }

        public float Ratio { get { return (float)this.Width / this.Height; } }

        public Size(int width, int heigth)
        {
            this._width = width;
            this._height = heigth;
        }

        public static Size operator *(int k, Size s)
        {
            return new Size(k * s.Width, k * s.Height);
        }

        public static Size operator *(Size s, int k)
        {
            return k * s;
        }

        public static bool operator ==(Size lhs, Size rhs)
        {
            return lhs.Width == rhs.Width && lhs.Height == rhs.Height;
        }

        public static bool operator !=(Size lhs, Size rhs)
        {
            return !(lhs == rhs);
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() != typeof(Size))
                return false;
            return this == (Size)obj;
        }

        public override int GetHashCode()
        {
            return this.Width ^ this.Height;
        }
    }
}