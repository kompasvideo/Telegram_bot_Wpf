using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telegram_bot_Wpf
{
    enum Type
    {
        Document,
        Photo,
        Audio,
        Voice,
        Video
    }

    class TypeMessage
    {
        public Type type { get; set; }
        public string fileName { get; set; }        
    }
}
