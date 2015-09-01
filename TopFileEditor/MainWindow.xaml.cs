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
        private List<Station> Stations;
        private List<Station> CopySelectedStations = new List<Station>();
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
            foreach (Shot shat in this.topFile.Shots)
            {
                Stations.Add(new Station(shat));
            }
            this.SurveyView.ItemsSource = Stations;
        }

        private void SetBack_Click(object sender, RoutedEventArgs e)
        {
            if (this.topFile == null) return;
            int cnt = 0;
            foreach (Station station in this.SurveyView.Items)
            {
                this.topFile.Shots[cnt].From = station.From;
                this.topFile.Shots[cnt].To = station.To;
                this.topFile.Shots[cnt].Distance = station.Distance;
                this.topFile.Shots[cnt].Azimuth = station.Azimuth;
                this.topFile.Shots[cnt].Inclination = station.Inclination;
                this.topFile.Shots[cnt].Comment = station.Comment;
                cnt++;
            }
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
        }

        private void CanCopy(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = numbers.Count > 0;
        }
        private void CanCut(object sender, CanExecuteRoutedEventArgs e)
        {
            //e.CanExecute = numbers.Count > 0;
        }

        private void Cut(object sender, ExecutedRoutedEventArgs e)
        {
            //e.CanExecute = numbers.Count > 0;
        }

        private void CanPaste(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Clipboard.ContainsData(DataFormats.Text);
            
            e.Handled = true;
        }

        private void Paste(object sender, ExecutedRoutedEventArgs e)
        {
            int iCrrentSelectedRow = this.SurveyView.SelectedIndex;
            var ClipBoardText = Clipboard.GetData(DataFormats.Text);
            var selectedCells = this.SurveyView.SelectedCells.Count;

            switch (TypeOfClipboardData(ClipBoardText.ToString()))
            {
                case 1:
                    InsertNewColumn2Grid(ClipBoardText.ToString(), iCrrentSelectedRow);
                    break;
                case 2:
                    InsertNewRow2Grid(ClipBoardText.ToString(), iCrrentSelectedRow);
                    break;
                case 3:
                    InsertNewRows2Grid(ClipBoardText.ToString(), iCrrentSelectedRow);
                    break;
            }
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
        private int TypeOfClipboardData(string CopiedData)
        {
            if (this.CopySelectedStations.Count > 1)
            {

            }
            else { return 1; }

            return -1;
        }

        private void InsertNewRow2Grid(string CopiedData, int position)
        {
            //DeleteSelectedRows();
            foreach (Station station in this.CopySelectedStations)
            {
                this.Stations.Insert(position, station);
                position++;
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
            this.SurveyView.Items.Refresh();
        }

        private void InsertNewColumn2Grid(string CopiedData, int position)
        {
            if (this.SurveyView.SelectedCells.Count > 0)
            {
                foreach (string line in CopiedData.Trim().Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                {
                    string[] items = line.Trim().Split('\t');
                    int i = 0;
                    foreach (DataGridCellInfo cell in this.SurveyView.SelectedCells)
                    {
                        if (i >= items.Length)
                            break;
                        var nek = cell.Column.Header;
                        switch (cell.Column.Header.ToString())
                        {
                            case "From":
                                ((Station)cell.Item).From = items[i];
                                break;
                            case "To":
                                ((Station)cell.Item).To = items[i];
                                break;
                            case "Distance":
                                double val1;
                                if (double.TryParse(items[i], out val1))
                                {
                                    ((Station)cell.Item).Distance = val1;
                                }
                                break;
                            case "Azimuth":
                                double val2;
                                if (double.TryParse(items[i], out val2))
                                {
                                    ((Station)cell.Item).Azimuth = val2;
                                }
                                break;
                            case "Inclination":
                                double val3;
                                if (double.TryParse(items[i], out val3))
                                {
                                    ((Station)cell.Item).Inclination = val3;
                                }
                                break;
                            case "Comment":
                                ((Station)cell.Item).Comment = items[i];
                                break;
                        }
                        i++;
                    }
                }
                this.SurveyView.Items.Refresh();
            }
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
                foreach (Station station in this.SurveyView.Items)
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
    }
    public class Station
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

        public Station(Shot shot)
        {
            this.From = shot.From;
            this.To = shot.To;
            this.Distance = shot.Distance;
            this.Azimuth = shot.Azimuth;
            this.Inclination = shot.Inclination;
            this.Comment = shot.Comment;
        }

    }

}