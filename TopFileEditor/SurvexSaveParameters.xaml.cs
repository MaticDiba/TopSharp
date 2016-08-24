using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using TopSharp;

namespace TopFileEditor
{
    /// <summary>
    /// Interaction logic for SurvexSaveParameters.xaml
    /// </summary>
    public partial class SurvexSaveParameters : Window
    {
        private List<TopFile> ListOfSurveys;

        public ObservableCollection<Test> TestItems { get; set; }
        public List<TmpStation> Stations { get; private set; }

        public SurvexSaveParameters(List<TopFile> surveyFilesList)
        {
            TestItems = new ObservableCollection<Test>();
            InitializeComponent();
            /*this.lbFileName.Items.Clear();
            this.lbFileName.ItemsSource = new System.Collections.ObjectModel.ObservableCollection<string>(surveyFilesList.Select(s => s.FileName));*/
            this.ListOfSurveys = surveyFilesList;
            foreach(var fajl in surveyFilesList)
            {
                Test test = new Test(fajl);
                TestItems.Add(test);
            }
            string[] oldValues = ImportExportHelper.OpenSurvexSaveConfig(Properties.Settings.Default.LastUsedMultipleFolder);
            if(oldValues != null && oldValues.Length == 6)
            {
                this.tbName.Text = oldValues[0];
                this.tbXCoor.Text = oldValues[1];
                this.tbYCoor.Text = oldValues[2];
                this.tbEntrancElev.Text = oldValues[3];
            }
            Stations = new List<TmpStation>();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void lbFileName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //this.lbStationName.IsEnabled = true;
            var sFileName = this.lbFileName.SelectedItem.ToString();
            var selectedFile = this.ListOfSurveys.First(fl => fl.FileName == sFileName);
            List<string> stationNames = selectedFile.StationMappings.Keys.ToList<string>();
            this.Stations = selectedFile.StationMappings.Select(stat => new TmpStation() { OldName = stat.Key, NewName = stat.Value }).ToList();
            //this.lbStationName.Items.Refresh();
        }

        private string _selectedStationIndex;
        public string SelectedStationIndex
        {
            get { return _selectedStationIndex; }
            set
            {
                _selectedStationIndex = value;
            }
        }

        public string SelectedFile { get; private set; }
        public string SelectedStation { get; private set; }

        private void lbFileName_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedFile = lbFileName.SelectedItem.ToString();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.SelectedStation = this.TestItems.First(ss => ss.Name == this.SelectedFile).Test2Items.First(ss => ss.Label == this.lbStationName.SelectedItem.ToString()).OldLabel;
        }
    }

    public class TmpStation
    {
        public string OldName { get; set; }
        public string NewName { get; set; }
    }

    public class Test
    {

        public Test(TopFile fajl)
        {
            this.Name = fajl.FileName;
            this.Test2Items = new ObservableCollection<Test2>();
            foreach (var stationPair in fajl.StationMappings)
            {
                this.Test2Items.Add(new Test2(stationPair));
            }
        }

        public string Name { get; set; }
        public ObservableCollection<Test2> Test2Items { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class Test2
    {

        public Test2(KeyValuePair<string, string> stationPair)
        {
            this.Label = stationPair.Key;
            this.OldLabel = stationPair.Value;
        }

        public string Label { get; set; }
        public string OldLabel { get; set; }
        public override string ToString()
        {
            return Label;
        }
    }
}
