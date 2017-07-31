using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using LagerApp.Model;
using LagerApp.DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace LagerApp.Controllers
{
    [Route("api/artikel")]
    public class ArtikelPlatzControllerAPI : Controller
    {
        // POST api/box
        //Create or Update if exists
        [HttpPost]
        public IActionResult Post([FromBody]ArtikelLagerPlatzDTO dto)
        {
            LagerContext con = HttpContext.RequestServices.GetService(typeof(LagerApp.Model.LagerContext)) as LagerContext;
            int rowsAffected = 0;
            if (dto == null)
            {
                return NotFound();
            }
            rowsAffected = con.saveOrUpdateArtikelToPlatz(dto);

            return Ok(dto);
        }
    }
}
