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
            
            foreach (Paper paper in papers)
            {
                IEnumerable<Variant> match = from item in vrts.Keys where item.VariantCode == paper.Variant select item;
                if (match.Count()==0)
                {
                    Variant v = new Variant { VariantCode = paper.Variant };
                    vrts.Add(v, new List<Paper> { paper });
                }
                else
                {
                    vrts[match.First()].Add(paper);
                }
            }
            List<KeyValuePair<Variant, List<Paper>>> lst = (from item in vrts orderby item.Key.VariantCode select item).ToList();
            Variants = new Variant[lst.Count()];
            for (int i = 0; i < lst.Count(); i++)
            {
                Variant vrt = lst[i].Key;
                Paper[] plst = lst[i].Value.OrderBy(item => item.Type).ToArray();
                vrt.Papers = plst;
                Variants[i] = vrt;
            }
        }
    }
}
