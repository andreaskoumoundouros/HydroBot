using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Hydrobot.Resources.Database {
    public class Server {

        [Key]
        public string ProcessName { get; set; }
        public string ExecutableLocation { get; set; }
        public string ExecutableName { get; set; }
        public ulong Port { get; set; }
    }
}