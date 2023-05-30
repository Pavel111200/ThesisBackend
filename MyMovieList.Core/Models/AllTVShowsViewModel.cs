﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieList.Core.Models
{
    public class AllTVShowsViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public double Rating { get; set; }

        public string Image { get; set; }

        public string Season { get; set; }
    }
}
