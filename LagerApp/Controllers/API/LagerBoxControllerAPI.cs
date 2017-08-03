using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LagerApp.DTOs;
using LagerApp.Model;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LagerApp.Controllers
{
    [Route("api/box")]
    public class LagerBoxControllerAPI : Controller
    {

        // POST api/box
        //Create or Update if exists
        /// <summary>
        /// Artikel A15000 zu Lagerbox BOX2000
        /// </summary>
        [HttpPost]
        public IActionResult Post([FromBody]LagerBoxDTO dto)
        {
            LagerContext con = HttpContext.RequestServices.GetService(typeof(LagerApp.Model.LagerContext)) as LagerContext;
            int rowsAffected=0;
            if (dto == null)
            {
                return NotFound();
            }
            rowsAffected = con.saveOrUpdateToBox(dto);

            return Ok(dto);
        }
        
    }
}
