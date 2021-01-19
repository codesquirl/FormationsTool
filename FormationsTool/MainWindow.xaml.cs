using AdonisUI.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
    /// Interaction logic for MainWindow.xaml
    /// AdonisUI: https://github.com/benruehl/adonis-ui
    /// </summary>
    public partial class MainWindow : AdonisWindow
    {
        public static bool IsFileLoaded { get; set; } = false;
        public static bool IsSaved { get; set; } = true;

        public FormationFile CurrentFormationFile { get; set; }
        public string FormationFilePath { get; set; } = null;

        public MainWindow()
        {
            InitializeComponent();
            NewFile();
        }

        #region File Menu

        // CloseMenuItem
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSaved)
            {
                ConfirmationDialog c = new ConfirmationDialog("You have unsaved changes. Are you sure you want to exit?");
                if (!c.ShowDialog() ?? false)
                {
                    return;
                }
            }
            Close();
        }

        private void NewMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSaved)
            {
                ConfirmationDialog c = new ConfirmationDialog("You have unsaved changes. Discard changes?");
                if (!c.ShowDialog() ?? false)
                {
                    return;
                }
            }
            NewFile();
        }

        private void LoadMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!IsSaved)
            {
                ConfirmationDialog c = new ConfirmationDialog("You have unsaved changes. Discard changes?");
                if (!c.ShowDialog() ?? false)
                {
                    return;
                }
            }
            OpenFileDialog dlg = new OpenFileDialog();
            if (dlg.ShowDialog() ?? false)
            {
                FormationFilePath = dlg.FileName;
                LoadFile(FormationFilePath);
            }
        }

        private void SaveMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!IsFileLoaded)
            {
                SaveFileAs();
            }
            else
            {
                SaveFile();
            }
        }

        private void SaveAsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            SaveFileAs();
        }

        public void NewFile()
        {
            CurrentFormationFile = new FormationFile();
            FormationsListBox.Items.Clear();
            AddFormation(Formation.Create(8));
            FormationsListBox.SelectedItem = CurrentFormationFile.Formations[0];
            IsSaved = false;
        }

        public void SaveFile()
        {
            File.WriteAllText(FormationFilePath, CurrentFormationFile.ToJson());
            IsSaved = true;
        }

        public void SaveFileAs()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            if (dialog.ShowDialog() ?? false)
            {
                IsFileLoaded = true;
                FormationFilePath = dialog.FileName;
                SaveFile();
            }
        }

        public void LoadFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                CurrentFormationFile = new FormationFile();
                FormationsListBox.Items.Clear();
                List<Formation> formations = null;
                try
                {
                    formations = JsonSerializer.Deserialize<List<Formation>>(File.ReadAllText(filePath));
                }
                catch (Exception ex)
                {
                    // Show error
                    AdonisUI.Controls.MessageBox.Show("Error loading formation file: " + ex.Message);
                    NewFile();
                    return;
                }

                foreach (Formation formation in formations)
                {
                    AddFormation(formation);
                    FormationsListBox.SelectedItem = CurrentFormationFile.Formations[0];
                }
            }
        }

        #endregion

        public void NewFormation()
        {
            FormationDialog dlg = new FormationDialog();
            if (dlg.ShowDialog() ?? false)
            {
                Formation formation = Formation.Create(dlg.FormationSize);
                AddFormation(formation);
                FormationsListBox.SelectedItem = formation;
                IsSaved = false;
            }
        }
        public void AddFormation(Formation formation)
        {
            formation.PropertyChanged += Formation_PropertyChanged;
            CurrentFormationFile.Formations.Add(formation);
            FormationsListBox.Items.Add(formation);
        }

        public void RemoveFormation(Formation formation)
        {
            formation.PropertyChanged -= Formation_PropertyChanged;
            CurrentFormationFile.Formations.Remove(formation);
            FormationsListBox.Items.Remove(formation);
        }

        private void Formation_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
        }

        private void FormationsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Formation formation = FormationsListBox.SelectedItem as Formation;
            if (formation != null)
            {
                SetFormationDataContext(formation);
            }
        }

        private void SetFormationDataContext(Formation formation)
        {
            PropertiesPanel.Formation = formation;
            FormationCanvas.Formation = formation;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            // Add New Formation
            NewFormation();
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmationDialog dlg = new ConfirmationDialog("Are you sure you want to remove this formation?");
            if (dlg.ShowDialog() ?? false)
            {
                Formation formation = FormationsListBox.SelectedItem as Formation;
                if (formation != null)
                {
                    RemoveFormation(formation);
                    if (CurrentFormationFile.Formations.Count == 0)
                    {
                        AddFormation(Formation.Create(8));
                    }
                    FormationsListBox.SelectedItem = CurrentFormationFile.Formations[0];
                    IsSaved = false;
                }
            }
        }
    }
}
