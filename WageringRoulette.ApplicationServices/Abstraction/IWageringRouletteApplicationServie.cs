using System.Collections.Generic;
using WageringRoulette.ApplicationServices.Requests;
using WageringRoulette.DomainServices.Model;

namespace WageringRoulette.ApplicationServices.Abstraction
{
    public interface IWageringRouletteApplicationServie
    {
        public RouletteModel Create();
        public RouletteModel Find(string Id);
        public RouletteModel OpenWager(string Id);
        public RouletteModel CloseWager(string Id);
        public RouletteModel Wager(string rouletteId, WagerRequest wagerRequest);
        public List<RouletteModel> GetAll();
    }
}
