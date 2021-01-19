using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using AdonisUI.Controls;
using MessageBox = System.Windows.MessageBox;

namespace FormationsTool
{
    public partial class FormationDialog : AdonisWindow
    {
        public int FormationSize { get; set; } = 4;

        public FormationDialog()
        {
            InitializeComponent();
        }

        private void OKButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(FormationSizeTextBox.Text, out int formationSize))
            {
                FormationSize = formationSize;
                DialogResult = true;
                Close();
            }
            else
            {
                MessageBox.Show("Formation Size must be an integer.");
            }
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void FormationSizeTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            string test = FormationSizeTextBox.Text;
            if (!int.TryParse(test, out int val))
            {
                FormationSizeTextBox.Text = "0";
                FormationSizeTextBox.SelectionStart = 0;
                FormationSizeTextBox.SelectionLength = 1;
            }
        }
    }
}