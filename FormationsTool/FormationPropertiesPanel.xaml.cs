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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FormationsTool
{
    /// <summary>
    /// Interaction logic for FormationPropertiesPanel.xaml
    /// </summary>
    public partial class FormationPropertiesPanel : UserControl
    {
        private Formation formation;

        public Formation Formation { 
            get => formation;
            set {
                formation = value;
                DataContext = formation;                
            } 
        }

        public FormationPropertiesPanel()
        {
            InitializeComponent();
            DataContextChanged += FormationPropertiesPanel_DataContextChanged;
        }

        private void FormationPropertiesPanel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            PositionStack.Children.Clear();
            if (Formation != null && Formation.Positions.Count > 0)
            {
                AddVectorEditor(0, App.PlayerColor, Formation.Positions[0]);
                for (int i = 1; i < Formation.Positions.Count; i++)
                {
                    AddVectorEditor(i, App.PositionColors[(i-1)%App.PositionColors.Count], Formation.Positions[i]);
                }
            }
        }

        private void AddVectorEditor(int index, Color color, FVector vector)
        {
            FVectorEditorControl vectorEditorControl = new FVectorEditorControl();
            vectorEditorControl.Init(index, color, vector);
            PositionStack.Children.Add(vectorEditorControl);
        }
    }
}
