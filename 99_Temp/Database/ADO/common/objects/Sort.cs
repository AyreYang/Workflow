using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataBase.common.objects
{
    public class Sort
    {
        public enum Orientation
        {
            asc, desc
        }

        private Dictionary<string, Orientation> parameters { get; set; }
        public int Count { get { return parameters.Count; } }

        public Sort()
        {
            parameters = new Dictionary<string, Orientation>();
        }

        public Sort Add(string name, Orientation orient)
        {
            if (string.IsNullOrWhiteSpace(name)) return this;
            var key = name.Trim().ToUpper();
            if (this.parameters.ContainsKey(key)) return this;
            this.parameters.Add(key, orient);
            return this;
        }

        public void Export(out string text)
        {
            var list = new List<string>();
            this.parameters.Keys.ToList().ForEach(key => list.Add(string.Format("{0} {1}", key, this.parameters[key])));
            text = string.Join(",", list);
        }
    }
}
