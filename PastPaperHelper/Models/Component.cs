using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PastPaperHelper.Models
{
    public class Component
    {
        public char Code { get; set; }
        public Paper[] Papers { get; set; }
    }
}
