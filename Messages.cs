using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace Telegram_bot_Wpf
{
    [Serializable]
    // Класс хранит сообщения 
    public class Messages
    {
        public string dt { get; set; }
        public string firstName { get; set; }
        public long id { get; set; }
        public string text { get; set; }
        public string type { get; set; }
        public Messages(DateTime p_dt, string p_fName, long p_id, string p_text, Telegram.Bot.Types.Enums.MessageType p_type)
        {
            dt = p_dt.ToString() + "  ";
            firstName = p_fName + "  ";
            id = p_id;
            text = "  " + p_text + "  "; 
            type = p_type.ToString();
        }
    }
}
