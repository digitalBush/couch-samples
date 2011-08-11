using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NoteTaker.Models
{
    public class NoteList
    {
        public IEnumerable<Note> Notes { get; set; }
        public Dictionary<string,int> Tags { get; set; }
    }
}