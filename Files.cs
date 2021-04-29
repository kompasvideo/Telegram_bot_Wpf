using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telegram_bot_Wpf
{
    class Files
    {
        public string name { get; set; }
        public string type { get; set; }
        public string size { get; set; }
        public string date { get; set; }
        public Files(string p_name, string p_type, long p_size, DateTime p_date)
        {
            name = p_name;
            type = p_type;
            size = p_size.ToString();
            date = p_date.ToString();
        }
    }
}
