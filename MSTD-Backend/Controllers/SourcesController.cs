using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MSTD_Backend.Models.Sources;

namespace MSTD_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SourcesController : ControllerBase
    {
        /// <summary>
        /// Returns a list of torrrent sources for MSTD client app to use for customization
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SourceDto>), (int)HttpStatusCode.OK)]
        public IActionResult GetSources()
        {
            throw new NotImplementedException();
        }
    }
}