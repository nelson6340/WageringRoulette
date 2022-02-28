using System;
using System.ComponentModel.DataAnnotations;

namespace WageringRoulette.ApplicationServices.Requests
{
    public class WagerRequest
    {
        public string UserId { get; set; }

        public decimal Money { get; set; }

        [Range(0, 39)]
        public int Position { get; set; }
    }
}
