﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMovieList.Core.Models
{
    public class LikedShowsViewModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public double OverallRating { get; set; }

        public double YourRating { get; set; }

        public int Season { get; set; }

        public string Image { get; set; }
    }
}
