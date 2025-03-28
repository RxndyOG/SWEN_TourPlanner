﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SWEN_TourPlanner.Models
{
    public class BlockModel
    {
        public int TourID { get; set; }
        public string Text { get; set; }
        public string Description2 { get; set; }
        public ImageSource ImageTextBlock { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand NavigateCommandRight { get; set; }
    }
}
