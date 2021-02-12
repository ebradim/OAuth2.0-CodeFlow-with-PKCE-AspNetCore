using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthorizationServer.Persistence
{
    public class StaticData
    {
        public static string CurrentUserName { get; set; }
        public static string Scope { get; set; }
    }
}
