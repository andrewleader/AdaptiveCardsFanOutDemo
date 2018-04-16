using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApp.Model;

namespace WebApp.Controllers
{
    [Produces("application/json")]
    [Route("api/Motherships")]
    public class MothershipsController : Controller
    {
        // GET: api/Motherships
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return AllMothershipsModel.GetAllMothershipNames();
        }
    }
}
