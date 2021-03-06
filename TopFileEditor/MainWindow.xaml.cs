﻿using System;
using System.Collections.Generic;
using System.IO;
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
using Microsoft.Win32;
using TopSharp;

namespace TopFileEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private TopFile topFile;
        private List<TopFile> topFilesFromFolder = new List<TopFile>();
        private List<Station> Stations;
        private List<Station> CopySelectedStations = new List<Station>();
        private List<DataGridCell> CopySelectedCells = new List<DataGridCell>();

        private bool CanCutAllowed = false;

        public string Path4MultipleFiles { get; private set; }
        public List<TopFile> MultipleTopFiles { get; private set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Filter = "top files (*.top)|*.top";
            dlg.Title = "Please select an top file to open.";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                using (BinaryReader b = new BinaryReader(File.Open(filename, FileMode.Open)))
                {
                    int pos = 0;
                    int length = (int)b.BaseStream.Length;
                    this.topFile = new TopFile(b);
                    LoadGrid();
                }
            }
        }

        private void LoadGrid()
        {
            Stations = new List<Station>();
            int i = 0;
            foreach (Shot shat in this.topFile.Shots)
            {
                Station station = new Station(shat);
                station.Idx = i;
                Stations.Add(station);
                i++;
            }
            this.SurveyView.ItemsSource = Stations;
        }

        private void SetBack_Click(object sender, RoutedEventArgs e)
        {
            // TODO zrihtat save
            if (this.topFile == null) return;
            int cnt = 0;
            foreach (Station station in this.Stations)
            {
                if (this.topFile.Shots.Count > cnt)
                {
                    this.topFile.Shots[cnt].From = station.From;
                    this.topFile.Shots[cnt].To = station.To;
                    this.topFile.Shots[cnt].Distance = station.Distance;
                    this.topFile.Shots[cnt].Azimuth = station.Azimuth;
                    this.topFile.Shots[cnt].Inclination = station.Inclination;
                    this.topFile.Shots[cnt].Comment = station.Comment;
                }
                else
                {
                    this.topFile.Shots.Add(new Shot(station));
                }
                cnt++;
            }
            this.topFile.SaveFile("test-trubar-pt1.top");
        }

        private void SurveyView_CopyingRowClipboardContent(object sender, DataGridRowClipboardEventArgs e)
        {
            //Clipboard.SetData(DataFormats.Text, this.SurveyView.SelectedIndex);
            var nek  = Clipboard.GetText();

            this.CopySelectedStations = new List<Station>(this.SurveyView.SelectedItems.Cast<Station>().ToList());
        }

        private void Copy(object sender, ExecutedRoutedEventArgs e)
        {
            //Clipboard.SetData(DataFormats.Text, string.Join(",", numbers));
            
            var grid = sender as DataGrid;
            var selectedItems = grid.SelectedItems;
            List<System.Data.DataRowView> selectedFile = (List<System.Data.DataRowView>)grid.SelectedItems;

        }

        private void CanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            var grid = sender as DataGrid;
            this.CanCutAllowed = false;
            /*
            var selectedItems = grid.SelectedItems;
            var selectedColumns = grid.SelectedCells;
            */
            //this.CopySelectedCells = grid.SelectedCells;
            List<DataGridCell> cellList = new List<DataGridCell>();
            this.CopySelectedCells = new List<DataGridCell>();
            foreach (DataGridCellInfo cellInfo in grid.SelectedCells)
            {
                this.CopySelectedCells.Add((DataGridCell)cellInfo.Column.GetCellContent(cellInfo.Item).Parent);
            }
            // For row copy
            if (grid.SelectedItems.Count > 0)
            {
                //IList<Station> nene = (IList<Station>)grid.SelectedItems;
                this.CopySelectedStations = new List<Station>();
                foreach(Station stejsn in grid.SelectedItems)
                    this.CopySelectedStations.Add(stejsn);
            }
            // TODO: for columns copy
        }
        private void CanCut(object sender, CanExecuteRoutedEventArgs e)
        {
            this.CanCutAllowed = true;
            //e.CanExecute = sender;
            
            var grid = sender as DataGrid;
            /*
            var selectedItems = grid.SelectedItems;
            var selectedColumns = grid.SelectedCells;
            IList<DataGridCellInfo> vaaar = grid.SelectedCells;
            var column = selectedColumns[0].Column.Header;*/
            //this.CopySelectedCells = grid.SelectedCells;

            if (grid.SelectedItems.Count > 0)
            {
                this.CopySelectedStations = new List<Station>();
                foreach (Station stejsn in grid.SelectedItems)
                    this.CopySelectedStations.Add(stejsn);
            }
        }

        private void Cut(object sender, ExecutedRoutedEventArgs e)
        {
            //e.CanExecute = numbers.Count > 0;
            var grid = sender as DataGrid;
            var selectedItems = grid.SelectedItems;
        }

        private void CanPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsData(DataFormats.Text);
            /*string sData = Clipboard.GetText();
            FillStations(sData);*/
            e.Handled = true;
        }

        private void FillStations(string sData)
        {
            foreach (string row in sData.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                if (row.Length == 0)
                    continue;
                this.Stations.Add(new Station(row));
            }
        }

        private void Paste(object sender, ExecutedRoutedEventArgs e)
        {
            int iCrrentSelectedRow = this.SurveyView.SelectedIndex;
            var ClipBoardText = Clipboard.GetData(DataFormats.Text);
            var selectedCells = this.SurveyView.SelectedCells.Count;
            if (iCrrentSelectedRow < 0 && this.SurveyView.SelectedCells.Count > 0)
            {
                Station selectedStation = (Station)this.SurveyView.SelectedCells.First().Item;
                iCrrentSelectedRow = this.SurveyView.Items.IndexOf(selectedStation);
                //iCrrentSelectedRow = this.SurveyView.SelectedCells.First().Item;
            }
            DataGrid fg = sender as DataGrid;

            switch (TypeOfClipboardData())
            {
                case 1:
                    InsertNewColumn2Grid(ClipBoardText.ToString(), iCrrentSelectedRow);
                    break;
                case 2:
                    InsertNewRow2Grid(iCrrentSelectedRow);
                    break;
                case 3:
                    InsertNewRows2Grid(ClipBoardText.ToString(), iCrrentSelectedRow);
                    break;
            }
            this.CanCutAllowed = false;
            //this.SurveyView.Items.Add(this.SurveyView.Items[int.Parse(Clipboard.GetText())]);
            //Close();
        }

        /// <summary>
        /// Returns the datatype for different types of data.
        /// 1 for inserting column
        /// 2 for inserting row
        /// 3 for inserting multiple rows
        /// </summary>
        /// <param name="CopiedData">Data that will be copied.</param>
        /// <returns></returns>
        private int TypeOfClipboardData()
        {
            if (this.CopySelectedStations.Count > 0)
            {
                return 2;
            }
            else if (this.CopySelectedStations.Count > 1) { return 3; }
            else if (this.CopySelectedStations.Count == 0 && CopySelectedCells.Count > 0)
            {
                //TODO: narest za columne
                return 1;
            }
            return -1;
        }

        private void InsertNewRow2Grid(int position)
        {
            //DeleteSelectedRows();
            foreach (Station station in this.CopySelectedStations)
            {
                Station newStation = station.Clone(this.Stations.Max(s => s.Idx) + 1);
                this.Stations.Insert(position, newStation);
                position++;
            }
            if (this.CanCutAllowed)
            {
                // Remove items from when cut is activated
                foreach (Station station in this.CopySelectedStations)
                {
                    this.Stations.Remove(station);
                }
            }
            this.SurveyView.Items.Refresh();
            
        }

        private void InsertNewRows2Grid(string CopiedData, int position)
        {
            DeleteSelectedRows();
            foreach (Station station in this.CopySelectedStations)
            {
                this.Stations.Insert(position, station);
                position++;
            }
            FillStations(CopiedData);
            this.SurveyView.Items.Refresh();
        }

        private void InsertNewColumn2Grid(string CopiedData, int position)
        {
            int idxStation = -1;
            foreach (DataGridCell cell in this.CopySelectedCells)
            {
                Station selectedStation = (Station)cell.DataContext;
                if (idxStation < 0) idxStation = selectedStation.Idx;
                else if (idxStation != selectedStation.Idx)
                {
                    position += 1;
                    idxStation = selectedStation.Idx;
                }
                switch (cell.Column.Header.ToString())
                {
                    case "From":
                        this.Stations[position].From = selectedStation.From;
                        break;
                    case "To":
                        this.Stations[position].To = selectedStation.To;
                        break;
                    case "Distance":
                        this.Stations[position].Distance = selectedStation.Distance;
                        break;
                    case "Azimuth":
                        this.Stations[position].Azimuth = selectedStation.Azimuth;
                        break;
                    case "Inclination":
                        this.Stations[position].Inclination = selectedStation.Inclination;
                        break;
                    case "Comment":
                        this.Stations[position].Comment = selectedStation.Comment;
                        break;
                }
            }
            this.SurveyView.Items.Refresh();
        }

        private void DeleteSelectedRows()
        {
            for (int i = this.SurveyView.SelectedItems.Count - 1; i >= 0; i--)
            {
                this.Stations.RemoveAt(this.SurveyView.SelectedIndex);
            }
            this.SurveyView.ItemsSource = Stations;
            this.SurveyView.Items.Refresh();
        }
        private void CanAddNew(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
            e.Handled = true;
        }

        private void AddNew(object sender, ExecutedRoutedEventArgs e)
        {
            //numbers.Add(numbers.Count);
        }

        private void ButtonChgPrefix_Click(object sender, RoutedEventArgs e)
        {
            int pref;
            if (TexboxPrefix.Text.Length > 0 && int.TryParse(TexboxPrefix.Text, out pref))
            {
                string prefix = TexboxPrefix.Text.Trim();
                foreach (Station station in this.Stations)
                {
                    string oldPref = station.From.Substring(0, station.From.IndexOf(","));
                    string oldAppendix = station.From.Substring(station.From.IndexOf(","));
                    station.From = prefix + oldAppendix;
                    if (station.To != null)
                    {
                        oldPref = station.To.Substring(0, station.To.IndexOf(","));
                        oldAppendix = station.To.Substring(station.To.IndexOf(","));
                        station.To = prefix + oldAppendix;
                    }
                }
                this.SurveyView.Items.Refresh();
            }
            else
            {
                MessageBox.Show("OI! Prefix is not in correct format!");
            }
        }

        private void SurveyView_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            
        }

        private void SurveyView_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {

        }

        private void SurveyView_TextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void DataGridTextColumn_PastingCellClipboardContent(object sender, DataGridCellClipboardEventArgs e)
        {

        }

        private void SurveyView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var grid = sender as DataGrid;
            var selected = grid.SelectedItems;

            foreach (var item in selected)
            {
                var dog = item;
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Filter = "top files (*.svx)|*.svx";
            dlg.Title = "Please select an svx file to open.";

            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string filename = dlg.FileName;
                using (StreamReader b = new StreamReader(filename))
                {
                    this.topFile = new TopFile(b);
                    
                    LoadGrid();
                }
            }
        }

        private void ImportFromFolder_Click(object sender, RoutedEventArgs e)
        {
            string sFolderName = @"D:\Documents\Jame\Platon\pocketopop\all";
            LoadAllTopFilesFromFolder(sFolderName);
        }

        private void LoadAllTopFilesFromFolder(string sFolderName)
        {
            string[] files = Directory.GetFiles(sFolderName);
            topFilesFromFolder = new List<TopFile>();
            foreach(string file in files)
            {
                if (file.Substring(file.LastIndexOf(".")).Contains("top"))
                {
                    TopFile TopFile = new TopFile(new BinaryReader((new StreamReader(file)).BaseStream));
                    topFilesFromFolder.Add(TopFile);
                }
            }
            MessageBox.Show("Done");
        }

        private void Export2tsv_Click(object sender, RoutedEventArgs e)
        { 
            using (StreamWriter sw = new StreamWriter("export_platon.tsv"))
            {
                foreach (TopFile tfile in topFilesFromFolder)
                {
                    foreach (string line in GetLinesFromPocketTopo(tfile))
                        sw.WriteLine(line);
                }
            }
        }

        private IEnumerable<string> GetLinesFromPocketTopo(TopFile tfile)
        {
            foreach(var groupedElements in tfile.Shots.GroupBy(x => new { x.From, x.To }))
            {
                if (groupedElements.Key.To == null)
                {
                    /*foreach (var pt in groupedElements)
                        yield return string.Format("{0}\t{1}\t{2}\t{3}\t{4}", pt.From, pt.To == null ? "*" : pt.To, pt.Distance, pt.Azimuth, pt.Inclination);*/
                }
                else
                {
                    
                    yield return string.Format("{0}\t{1}\t{2}\t{3}\t{4}", groupedElements.First().From, groupedElements.First().To == null ? "*" : groupedElements.First().To, groupedElements.Average(el => el.Distance), groupedElements.Average(el => el.Azimuth), groupedElements.Average(el => el.Inclination));
                }
            }
        }

        private void NewFile_Click(object sender, RoutedEventArgs e)
        {
            TopFile top = new TopFile();
            this.topFile = top;
            LoadGrid();
        }

        private void OpenMultiple_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            if (Properties.Settings.Default.LastUsedMultipleFolder.Length > 0)
            {
                dialog.SelectedPath = Properties.Settings.Default.LastUsedMultipleFolder;
            }
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();

            Properties.Settings.Default.LastUsedMultipleFolder = dialog.SelectedPath;
            Properties.Settings.Default.Save();

            Path4MultipleFiles = dialog.SelectedPath; //@"D:\My Documents\Visual Studio 2012\Projects\Private\TopOpener\TopFileEditor\bin\Debug\multipletest";// 
            try
            {
                this.MultipleTopFiles = ImportExportHelper.LoadMultipleFilesFromFolder(Path4MultipleFiles);

                this.MultipleTopFiles = ImportExportHelper.RenumberStationsForFiles(this.MultipleTopFiles);
            }catch
            {

            }
            if (MultipleTopFiles.Count > 0)
            {
                Export2Survex.IsEnabled = true;
                MessageBox.Show(string.Format("{0} files successfully imported.", this.MultipleTopFiles.Count));
            }
            else MessageBox.Show("Import failed!");
            
        }

        private void Export2survex_Click(object sender, RoutedEventArgs e)
        {
            SurvexSaveParameters dlgSurvex = new SurvexSaveParameters(this.MultipleTopFiles);
            dlgSurvex.Owner = this;
            dlgSurvex.ShowDialog();
            if (dlgSurvex.DialogResult == true)
            {
                string name = dlgSurvex.tbName.Text;
                string xcoor = dlgSurvex.tbXCoor.Text;
                string ycoor = dlgSurvex.tbYCoor.Text;
                string elev = dlgSurvex.tbEntrancElev.Text;
                string refPointFile = dlgSurvex.lbFileName.SelectedItem.ToString();
                string refPoint = dlgSurvex.lbStationName.SelectedItem.ToString();
                refPoint = this.MultipleTopFiles.First(fl => fl.FileName == refPointFile).StationMappings.First(stat => stat.Key == refPoint).Value;
                if(refPoint.IndexOf(".") > 0)
                {
                    refPoint.Replace(".", ",");
                }

                string sFile = ImportExportHelper.ExportMultipleFiles2SurvexFile(this.MultipleTopFiles, name, xcoor, ycoor, elev, refPoint);

                Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                dlg.FileName = "Document"; // Default file name
                dlg.DefaultExt = ".survex"; // Default file extension
                dlg.Filter = "Survex Documents (.svx)|*.svx";

                Nullable<bool> result = dlg.ShowDialog();

                // Process save file dialog box results
                if (result == true)
                {
                    // Save document
                    string filename = dlg.FileName;
                    File.WriteAllText(filename, sFile);
                    ImportExportHelper.WriteSurvexSaveConfig(new List<string>() {name, xcoor, ycoor, elev, refPointFile, refPoint },
                        Properties.Settings.Default.LastUsedMultipleFolder);
                    MessageBox.Show("File successfully saved!");
                }
            }
        }
    }

    /*public class Station
    {
        public string From { get; set; }
        public string To { get; set; }
        private double dist;
        public double Distance 
        { 
            get {
                return this.dist;
            }
            set {
                if (value.ToString().Contains(","))
                {

                }
                this.dist = value;
            }
        }
        public double Azimuth { get; set; }
        public double Inclination { get; set; }
        public string Comment { get; set; }
        public int Idx { get; set; }

        public Station(Shot shot)
        {
            this.From = shot.From;
            this.To = shot.To;
            this.Distance = shot.Distance;
            this.Azimuth = shot.Azimuth;
            this.Inclination = shot.Inclination;
            this.Comment = shot.Comment;
        }

        public Station(){}

        public Station Clone(int id = -1)
        {
            Station station = new Station() { Azimuth = this.Azimuth, Comment = this.Comment, Distance = this.Distance, From = this.From, Inclination = this.Inclination, To = this.To };
            if (id >= 0)
                station.Idx = id;
            return station;
        }

    }*/

}
