using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace simpleAPI.Controllers.V1._0
{
    [ApiVersion("1.0")]
    [Route("v{v:apiVersion}/[controller]")]
    public class HomeController : Controller
    {
       
        public ActionResult Index()
        {
            return View();
        }

        

        
           
    }
}