using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpAutokeyboard
{
    [Serializable]
    public class KeyDataEntry
    {
        public bool enabled;
        public string keys;
        public int flood;
        public int delay;
        public int repeat;
    }
}
