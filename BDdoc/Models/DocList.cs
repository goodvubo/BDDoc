using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BDdoc.Models
{
    public class DocList
    {
        public Doctor doctor;
        public List<Feedback> feeds;
        public int likes;
    }
}