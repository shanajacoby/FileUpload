﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUpload.Data
{
    public class Image
    {
        public int Id { get; set; }
        public string ImagePath { get; set; }
        public string Password { get; set; }
        public int NumberOfViews { get; set; }
    }
}
