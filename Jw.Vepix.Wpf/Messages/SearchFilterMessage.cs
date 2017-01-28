using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jw.Vepix.Wpf.Messages
{
    public class SearchFilterMessage
    {
        public string Text { get;}

        public SearchFilterMessage(string text)
        {
            Text = text;
        }
    }
}
