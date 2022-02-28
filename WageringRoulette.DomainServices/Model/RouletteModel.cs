using System;
using System.Collections.Generic;

namespace WageringRoulette.DomainServices.Model
{
    [Serializable]
    public class RouletteModel
    {
        public string Id { get; set; }                
        public string ClosingDate { get; set; }
        public string OpeningDate { get; set; }
        public bool IsOpen { get; set; } = false;
        public IDictionary<string, decimal>[] Pocket { get; set; } = new IDictionary<string, decimal>[40];

        public RouletteModel()
        {
            for (int i = 0; i < Pocket.Length; i++)
            {
                Pocket[i] = new Dictionary<string, decimal>();
            }
        }
    }
}
