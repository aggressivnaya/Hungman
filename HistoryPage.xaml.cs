﻿using System;
using System.Collections.Generic;
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
using System.Data;
using System.Data.OleDb;

namespace IshTaluy
{
    
    public partial class HistoryPage : Page
    {
        DataView historyTbl = new DataView();
        public HistoryPage()
        {
            InitializeComponent();
            historyTbl = DAL.GetDataView("select * from historyTbl");
            this.lstView1.ItemsSource = historyTbl;
        }
    }
}
