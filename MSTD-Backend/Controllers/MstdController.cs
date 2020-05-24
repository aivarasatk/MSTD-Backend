using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MSTD_Backend.Interfaces;
using MSTD_Backend.Models.Sources;

namespace MSTD_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MstdController : ControllerBase
    {
        private readonly IThePirateBaySource _thePirateBaySource;
        private readonly ILogService _logger;
        private readonly ILeetxSource _leetxSource;
        private readonly IKickassSource _kickassSource;

        public MstdController(IThePirateBaySource thePirateBaySource, ILogService logger, 
            ILeetxSource leetxSource, IKickassSource kickassSource)
        {
            _thePirateBaySource = thePirateBaySource;
            _logger = logger;
            _leetxSource = leetxSource;
            _kickassSource = kickassSource;
        }
        /// <summary>
        /// Returns a list of torrrent sources for MSTD client app to use for customization
        /// </summary>
        /// <returns></returns>
        [HttpGet("sources")]
        [ProducesResponseType(typeof(IEnumerable<SourceDto>), (int)HttpStatusCode.OK)]
        public IActionResult GetSources()
        {
            throw new NotImplementedException();
        }
    }
}