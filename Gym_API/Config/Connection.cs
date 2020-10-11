using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Gym_API.Config
{
    public class Connection
    {
        public string connectionString
        {
            get
            {
                return "workstation id=s1.srvrs.msanchez.dev,15701;user id=sa;pwd=fcuGopMyuqkGZlyxmXFkgD0smzWafXfP;data source=s1.srvrs.msanchez.dev,15701;persist security info=false;initial catalog=AgendaCDB";
            }
        }
    }
}
