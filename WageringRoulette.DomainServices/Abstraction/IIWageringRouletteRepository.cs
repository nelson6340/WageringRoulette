using System.Collections.Generic;
using WageringRoulette.DomainServices.Model;

namespace WageringRoulette.DomainServices.Abstraction
{
    public interface IWageringRouletteRepository
    {
        public RouletteModel GetById(string Id);

        public List<RouletteModel> GetAll();

        public RouletteModel Update(RouletteModel roulette);

        public RouletteModel Save(RouletteModel roulette);
    }
}
