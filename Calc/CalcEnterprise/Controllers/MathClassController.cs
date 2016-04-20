using CalcEnterprise.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Web.Http;

namespace CalcEnterprise.Controllers
{
    [Authorize]
    public class MathClassController : ApiController
    {
        // GET api/MathClass
        public IEnumerable<SessionTalk> Get()
        {
            return new SessionTalk[] 
            {
                new SessionTalk() { Title="Math Class A", Duration=60, Speakers= new string[] {"Andreas","Norbert"} },
                new SessionTalk() { Title="Math Class B", Duration=60, Speakers= new string[] {"Susi","Norbert"} },
                new SessionTalk() { Title="Math Class C", Duration=60, Speakers= new string[] {"Andreas","Maria"} },
                new SessionTalk() { Title="Math Class D", Duration=60, Speakers= new string[] {"Sonja","Michaela"} },
                new SessionTalk() { Title="Math Class E", Duration=60, Speakers= new string[] {"Tina","Andreas"} }
            };
        }

    }
}
