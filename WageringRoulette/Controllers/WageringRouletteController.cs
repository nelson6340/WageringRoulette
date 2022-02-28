using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using WageringRoulette.ApplicationServices.Abstraction;
using WageringRoulette.ApplicationServices.Requests;
using WageringRoulette.DomainServices.Model;

namespace WageringRoulette.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WageringRouletteController : Controller
    {
        private readonly IWageringRouletteApplicationServie wageringRouletteApplicationServie;

        public WageringRouletteController(IWageringRouletteApplicationServie wageringRouletteApplicationServie)
        {
            this.wageringRouletteApplicationServie = wageringRouletteApplicationServie;
        }

        /// <summary>
        /// Crea una rueleta nueva
        /// </summary>
        /// <returns>Identificador de la ruleta creada</returns>
        [HttpPost("[action]")]
        public IActionResult CreateRuolette()
        {
            try
            {
                RouletteModel roulette = wageringRouletteApplicationServie.Create();
                return Ok(roulette.Id);
            }
            catch (Exception ex)
            {
                return StatusCode(409, ex.Message);
            }
        }

        /// <summary>
        /// Abre las apuestas de una ruleta
        /// </summary>
        /// <param name="id">Identificador de la ruleta</param>
        /// <returns>Confirmación</returns>
        [HttpPut("{id}/open")]
        public IActionResult OpenWager([Required] string id)
        {
            try
            {
                wageringRouletteApplicationServie.OpenWager(Id : id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(409, ex.Message);
            }
        }

        /// <summary>
        /// Realiza la apuesta en la ruleta
        /// </summary>
        /// <param name="userId">Identificador de usuario</param>
        /// <param name="rouletteId">Identificador de la ruleta</param>
        /// <param name="wagerRequest">Número o color elegido, 38-Negro 39-Rojo y números de 0 a 36</param>
        /// <returns>Resumen de apuestas</returns>
        [HttpPost("[action]")]
        public IActionResult Wager([Required] string rouletteId, [Required] WagerRequest wagerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { error = true, msg = "Bad Request" });
            }

            try
            {
                RouletteModel roulette = 
                    wageringRouletteApplicationServie.Wager(rouletteId : rouletteId, wagerRequest : wagerRequest);
                return Ok(roulette);
            }
            catch (Exception ex)
            {
                return StatusCode(409, ex.Message);
            }
        }

        /// <summary>
        /// cierra las apuestas de la ruleta
        /// </summary>
        /// <param name="id">Identificador de la ruleta</param>
        /// <returns>Retorna el ganador de las apuestas</returns>
        [HttpPost("{id}/close")]
        public IActionResult CloseWager([Required] string id)
        {
            try
            {
                RouletteModel roulette = wageringRouletteApplicationServie.CloseWager(id);
                return Ok(roulette);
            }
            catch (Exception ex)
            {
                return StatusCode(409, ex.Message);
            }
        }

        /// <summary>
        /// Obtiene todas las ruletas
        /// </summary>
        /// <returns>Ruletas creadas</returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                return Ok(wageringRouletteApplicationServie.GetAll());
            }
            catch (Exception ex)
            {
                return StatusCode(409, ex.Message);
            }            
        }
    }
}

