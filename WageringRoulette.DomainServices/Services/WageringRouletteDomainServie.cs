using System.Collections.Generic;
using WageringRoulette.DomainServices.Abstraction;
using WageringRoulette.DomainServices.Model;

namespace WageringRoulette.DomainServices
{
    public class WageringRouletteDomainServie : IWageringRouletteDomainServie
    {
        private readonly IWageringRouletteRepository wageringRouletteRepository;

        public WageringRouletteDomainServie(IWageringRouletteRepository wageringRouletteRepository)
        {
            this.wageringRouletteRepository = wageringRouletteRepository;
        }

        public List<RouletteModel> GetAll()
        {
            return wageringRouletteRepository.GetAll();
        }

        public RouletteModel GetById(string Id)
        {
            return wageringRouletteRepository.GetById(Id : Id);
        }

        public RouletteModel Save(RouletteModel roulette)
        {
            return wageringRouletteRepository.Save(roulette : roulette);
        }

        public RouletteModel Update(RouletteModel roulette)
        {
            return wageringRouletteRepository.Update(roulette: roulette);
        }
    }
}
