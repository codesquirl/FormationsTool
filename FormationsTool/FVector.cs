using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FormationsTool
{
    public class FVector : INotifyPropertyChanged
    {
        private float x = 0.0f;
        private float y = 0.0f;
        private float z = 0.0f;

        public event PropertyChangedEventHandler PropertyChanged;

        public float X { get => x; set { x = (float)Math.Round(value, 2); if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("X")); } }
        public float Y { get => y; set { y = (float)Math.Round(value, 2); if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Y")); } }
        public float Z { get => z; set { z = (float)Math.Round(value, 2); if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Z")); } }
        public bool Editable { get; set; } = true;
        public string Name { get; set; } = string.Empty;

        public FVector() { }

        public void Set(float x, float y, float z, bool editable = true)
        {
            X = x;
            Y = y;
            Z = z;
            Editable = editable;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }

        public void Set(float x, float y)
        {
            X = x;
            Y = y;
        }
    }
}
