using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FormationsTool
{
    /// <summary>
    /// Interaction logic for FVectorEditorControl.xaml
    /// </summary>
    public partial class FVectorEditorControl : UserControl, INotifyPropertyChanged
    {
        private bool bound = false;
        private FVector vector;

        public Color BackgroundColor { get; set; } = Colors.Thistle;

        public event PropertyChangedEventHandler PropertyChanged;

        public FVector Vector
        {
            get => vector;
            set
            {
                UnBind();
                vector = value;
                XTextBox.IsEnabled = vector.Editable;
                YTextBox.IsEnabled = vector.Editable;
                ZTextBox.IsEnabled = vector.Editable;
                DataContext = vector;
                ReBind();
            }
        }

        public FVectorEditorControl()
        {
            InitializeComponent();            
        }
        
        public void Init(int index, Color color, FVector fVector)
        {
            IndexTextBox.Text = $"{index}";
            IndexTextBox.Background = new SolidColorBrush(color);
            Vector = fVector;
        }

        private void ReBind()
        {
            if (bound) UnBind();
            vector.PropertyChanged += Vector_PropertyChanged;
        }

        private void UnBind()
        {
            if (!bound) return;
            vector.PropertyChanged -= Vector_PropertyChanged;
        }

        private void Vector_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null) PropertyChanged(sender, e);
        }
        
    }
}
