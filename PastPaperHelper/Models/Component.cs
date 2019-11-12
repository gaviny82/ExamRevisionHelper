using System.Collections.Generic;
using System.Linq;

namespace PastPaperHelper.Models
{
    public class Component
    {
        public char Code { get; set; }
        public Variant[] Variants { get; set; }

        public Component(char code, Paper[] papers)
        {
            Code = code;
            Dictionary<Variant, List<Paper>> vrts = new Dictionary<Variant, List<Paper>>();

            var variants = from paper in papers
                           group paper by paper.Variant into variant
                           orderby variant.Key ascending
                           select new Variant
                           {
                               VariantCode = variant.Key,
                               Papers = variant.ToArray()
                           };
            Variants = variants.ToArray();
        }
    }
}
