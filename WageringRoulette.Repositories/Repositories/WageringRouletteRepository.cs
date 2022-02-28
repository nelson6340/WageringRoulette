using EasyCaching.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using WageringRoulette.DomainServices.Abstraction;
using WageringRoulette.DomainServices.Model;

namespace WageringRoulette.Repositories
{
    public class WageringRouletteRepository : IWageringRouletteRepository
    {
        private readonly IEasyCachingProviderFactory cachingProviderFactory;
        private readonly IEasyCachingProvider cachingProvider;
        private const string KEY = "TABLE";

        public WageringRouletteRepository(IEasyCachingProviderFactory cachingProviderFactory)
        {
            this.cachingProviderFactory = cachingProviderFactory;
            this.cachingProvider = this.cachingProviderFactory.GetCachingProvider("wageringRoulette");
        }

        public List<RouletteModel> GetAll()
        {
            var rouletes = this.cachingProvider.GetByPrefix<RouletteModel>(KEY);
            if (rouletes.Values.Count == 0)
            {
                return new List<RouletteModel>();
            }
            return new List<RouletteModel>(rouletes.Select(x => x.Value.Value));
        }

        public RouletteModel GetById(string Id)
        {
            var item = this.cachingProvider.Get<RouletteModel>(KEY + Id);
            if (!item.HasValue)
            {
                return null;
            }
            return item.Value;
        }

        public RouletteModel Save(RouletteModel roulette)
        {
            cachingProvider.Set(KEY + roulette.Id, roulette, TimeSpan.FromDays(365));
            return roulette;
        }

        public RouletteModel Update(RouletteModel roulette)
        {
            return Save(roulette);
        }
    }
}
