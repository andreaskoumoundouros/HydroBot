﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace Hydrobot.Resources.Database {
    public class Ounce {

        [Key]
        public ulong UserId { get; set; }
        public int Amount { get; set; }
    }
}
