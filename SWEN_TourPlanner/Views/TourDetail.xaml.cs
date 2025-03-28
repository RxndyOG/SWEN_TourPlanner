﻿using SWEN_TourPlanner.Commands;
using SWEN_TourPlanner.Database;
using SWEN_TourPlanner.GUI;
using SWEN_TourPlanner.ViewModels;
using System;
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

namespace SWEN_TourPlanner.Views
{
    /// <summary>
    /// Interaction logic for TourDetail.xaml
    /// </summary>
    public partial class TourDetail : Page
    {
        public MainViewModel MainVM { get; set; }
        public TourDetail(AddTourModel Tour, MainViewModel mainVM)
        {
            InitializeComponent();
            DataContext = Tour;
            MainVM = mainVM;


        }

        
    }
}
