using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogLibrary
{
    public class Speed
    {
        public List<string> speed = new List<string>();
        public Speed(string data) => speed.Add(data);
    }
    public class EngineTah
    {
        public List<string> engineTah = new List<string>();
        public EngineTah(string data) => engineTah.Add(data);
    }
}
