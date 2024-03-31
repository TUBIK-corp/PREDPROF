using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modest.Models
{
    internal class JSON
    {
    }

    public class Root
    {
        public Message message { get; set; }
    }

    public class Message
    {
        public DateInfo date { get; set; }
        public RoomsCountInfo rooms_count { get; set; }
        public WindowsForRoomInfo windows_for_room { get; set; }
        public WindowsInfo windows { get; set; }
    }

    public class DateInfo
    {
        public int data { get; set; } // Используйте тип DateTime, если вам нужно хранить даты/время
        public string description { get; set; }
    }

    public class RoomsCountInfo
    {
        public int data { get; set; }
        public string description { get; set; }
    }

    public class WindowsForRoomInfo
    {
        public List<int> data { get; set; }
        public string description { get; set; }
    }

    public class WindowsInfo
    {
        public Dictionary<string, List<bool>> data { get; set; }
        public string description { get; set; }
    }
}
