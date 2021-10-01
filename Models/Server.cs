using System;
using System.Collections.Generic;
using System.Text;

namespace GreeneryBOT.Models {
    public class Server {
        public ulong GuildId;
        public ulong ChannelId;
        public DateTime JoinedDate;

        public Server(ulong gid, ulong cid) {
            GuildId = gid;
            ChannelId = cid;
            JoinedDate = DateTime.UtcNow;
        }
    }
}
