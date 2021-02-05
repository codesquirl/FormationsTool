using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FormationsTool
{
    public class Formation : INotifyPropertyChanged
    {
        private bool isDefault = false;
        private string name = "New Formation";

        public event PropertyChangedEventHandler PropertyChanged;

        public delegate void PositionChangedEventHandler(object sender, PositionChangedEventArgs e);
        public event PositionChangedEventHandler PositionChanged;

        public string Name { get => name; set { name = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Name")); } }

        public bool IsDefault { get => isDefault; set { isDefault = value; if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("IsDefault")); } }
        public List<FVector> Positions { get; set; } = new List<FVector>();

        public Formation(){}

        // Accessor to avoid recursive issue setting default
        // from PropertyChanged handler
        public void SetDefault(bool isDefault)
        {
            this.isDefault = isDefault;
        }

        public static Formation Create(int numPositions)
        {
            if (numPositions < 1) numPositions = 1;
            float side = 1.0f;
            var r = 0;
            Formation f = new Formation();
            for (var c = 0; ((r * 8) + c) < numPositions; c++)
            {
                if (c == 8)
                {
                    r++;
                    c = 0;
                }
                side = (c % 2 == 0) ? 1.0f : -1.0f;
                f.AddPosition(
                    new FVector() {
                        X = side  * 150.0f * ((int)((c+1) / 2)),
                        Y = (150.0f * r),
                        Z = 0.0f,
                        Editable = true,
                        Name = $"{(r * 8) + c}"
                    });
            }
            f.Positions[0].Name = "PlayerPosition";
            f.Positions[0].Editable = false;
            return f;
        }

        public static Formation Create(int rows, int cols)
        {
            float side = 1.0f;
            Formation f = new Formation();
            for (var r = 0; r < rows; r++)
            {
                for (var c = 0; c < cols; c++)
                {
                    side = (c % 2 == 0) ? 1.0f : -1.0f;
                    f.AddPosition(
                        new FVector() {
                            X = side  * 150.0f * ((int)((c+1) / 2)),
                            Y = (150.0f * r),
                            Z = 0.0f,
                            Editable = true,
                            Name = $"{(r * 8) + c}"
                    });
                }
            }

            f.Positions[0].Name = "PlayerPosition";
            f.Positions[0].Editable = false;
            return f;
        }

        public void AddPosition(FVector position)
        {
            Positions.Add(position);
            position.PropertyChanged += VectorPropertyChanged;
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs("Positions"));
        }

        private void VectorPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            int index = Positions.IndexOf((FVector)sender);
            if (PositionChanged != null) PositionChanged(this, new PositionChangedEventArgs(index, (FVector)sender));
        }

        public void RebindChangeEvents()
        {
            foreach (FVector position in Positions)
            {
                position.PropertyChanged -= VectorPropertyChanged;
                position.PropertyChanged += VectorPropertyChanged;
            }
        }

        public string ToJson()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}
