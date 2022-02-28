using System;
using System.Collections.Generic;
using WageringRoulette.ApplicationServices.Abstraction;
using WageringRoulette.ApplicationServices.Exceptions;
using WageringRoulette.ApplicationServices.Requests;
using WageringRoulette.DomainServices.Abstraction;
using WageringRoulette.DomainServices.Model;
using static WageringRoulette.ApplicationServices.ErrorHandling.MessageHandler;

namespace WageringRoulette.ApplicationServices
{
    public class WageringRouletteApplicationServie : IWageringRouletteApplicationServie
    {
        private readonly IWageringRouletteDomainServie wageringRouletteDomainServie;
        private const decimal MAX_MONEY = 10000;
        private const decimal MIN_MONEY = 0.1M;
        private const int RED = 39;
        private const int BLACK = 38;
        private const int MULTIPLIER_BY_NUMBER = 5;
        private const decimal MULTIPLIER_BY_COLOR = 1.8M;

        public WageringRouletteApplicationServie(IWageringRouletteDomainServie wageringRouletteDomainServie)
        {
            this.wageringRouletteDomainServie = wageringRouletteDomainServie;
        }

        public RouletteModel Create()
        {
            RouletteModel roulette = new RouletteModel()
            {
                Id = Guid.NewGuid().ToString(),
                OpeningDate = null,
                ClosingDate = null
            };

            wageringRouletteDomainServie.Save(roulette: roulette);

            return roulette;
        }

        public RouletteModel Find(string Id)
        {
            return wageringRouletteDomainServie.GetById(Id: Id);
        }

        public RouletteModel OpenWager(string Id)
        {
            RouletteModel roulette = wageringRouletteDomainServie.GetById(Id);
            if (roulette == null)
            {
                throw new BusinessException(MessageCodes.NotFound, GetErrorDescription(MessageCodes.NotFound));
            }
            if (roulette.OpeningDate != null)
            {
                throw new BusinessException(MessageCodes.NotAllowedOpen, GetErrorDescription(MessageCodes.NotAllowedOpen));
            }
            roulette.IsOpen = true;
            roulette.OpeningDate = DateTime.UtcNow.ToString("o");

            return wageringRouletteDomainServie.Update(roulette: roulette);
        }

        public RouletteModel CloseWager(string Id)
        {
            RouletteModel roulette = wageringRouletteDomainServie.GetById(Id);
            if (roulette == null)
            {
                throw new BusinessException(MessageCodes.NotFound, GetErrorDescription(MessageCodes.NotFound));
            }
            if (roulette.OpeningDate == null)
            {
                throw new BusinessException(MessageCodes.NotAllowedCloseByOpen, GetErrorDescription(MessageCodes.NotAllowedCloseByOpen));
            }
            if (roulette.ClosingDate != null)
            {
                throw new BusinessException(MessageCodes.NotAllowedClose, GetErrorDescription(MessageCodes.NotAllowedClose));
            }
            roulette.IsOpen = false;
            roulette.ClosingDate = DateTime.UtcNow.ToString("o");
            GameValidator(roulette: roulette);

            return wageringRouletteDomainServie.Update(roulette);
        }

        private static void GameValidator(RouletteModel roulette)
        {
            Random random = new Random();
            int numberWon = random.Next(0, 36);

            if (roulette.Pocket[numberWon].Count > 0)
            {
                SetWinnersByNumber(roulette: roulette, numberWon: numberWon);
                SetLosersByNumber(roulette, numberWon: numberWon, isAll: false);
            }
            else
            {
                SetLosersByNumber(roulette, numberWon: numberWon, isAll: true);
            }
            if ((numberWon % 2) == 0)
            {
                SetRedWinners(roulette: roulette);
                SetBlackLoser(roulette: roulette);
            }
            else
            {
                SetBlackWinners(roulette: roulette);
                SetRedLosers(roulette: roulette);
            }
        }

        private static void SetLosersByNumber(RouletteModel roulette, int numberWon, bool isAll)
        {
            if (isAll)
            {
                for (int i = 0; i < 37; i++)
                {
                    SetZeroValue(roulette, i);
                }
            }
            else
            {
                for (int i = 0; i < 37; i++)
                {
                    if (i != numberWon)
                    {
                        SetZeroValue(roulette, i);
                    }
                }
            }
        }

        private static void SetZeroValue(RouletteModel roulette, int i)
        {
            List<string> losers = new List<string>(roulette.Pocket[i].Keys);
            foreach (var loser in losers)
            {
                roulette.Pocket[i].Remove(loser);
                roulette.Pocket[i].TryAdd(loser, 0);
            }
        }

        private static void SetWinnersByNumber(RouletteModel roulette, int numberWon)
        {
            List<string> winners = new List<string>(roulette.Pocket[numberWon].Keys);
            foreach (var winner in winners)
            {
                roulette.Pocket[numberWon].TryGetValue(winner, out decimal value);
                roulette.Pocket[numberWon].Remove(winner);
                roulette.Pocket[numberWon].TryAdd(winner, (value * MULTIPLIER_BY_NUMBER));
            }
        }

        private static void SetRedLosers(RouletteModel roulette)
        {
            if (roulette.Pocket[RED].Count > 0)
            {
                SetZeroValue(roulette, RED);
            }
        }

        private static void SetBlackWinners(RouletteModel roulette)
        {
            if (roulette.Pocket[BLACK].Count > 0)
            {
                SetWonValueByColor(roulette, BLACK);
            }
        }

        private static void SetWonValueByColor(RouletteModel roulette, int colour)
        {
            List<string> winners = new List<string>(roulette.Pocket[colour].Keys);
            foreach (var winner in winners)
            {
                roulette.Pocket[colour].TryGetValue(winner, out decimal value);
                roulette.Pocket[colour].Remove(winner);
                roulette.Pocket[colour].TryAdd(winner, (value * MULTIPLIER_BY_COLOR));
            }
        }

        private static void SetBlackLoser(RouletteModel roulette)
        {
            if (roulette.Pocket[BLACK].Count > 0)
            {
                SetZeroValue(roulette, BLACK);
            }
        }

        private static void SetRedWinners(RouletteModel roulette)
        {
            if (roulette.Pocket[RED].Count > 0)
            {
                SetWonValueByColor(roulette, RED);
            }
        }

        public RouletteModel Wager(string rouletteId, WagerRequest wagerRequest)
        {
            if (wagerRequest.Money > MAX_MONEY || wagerRequest.Money < MIN_MONEY)
            {
                throw new BusinessException(MessageCodes.MoneyOutRange, GetErrorDescription(MessageCodes.MoneyOutRange));
            }
            RouletteModel roulette = wageringRouletteDomainServie.GetById(rouletteId);
            if (roulette == null)
            {
                throw new BusinessException(MessageCodes.NotFound, GetErrorDescription(MessageCodes.NotFound));
            }
            if (!roulette.IsOpen)
            {
                throw new BusinessException(MessageCodes.RouletteClosed, GetErrorDescription(MessageCodes.RouletteClosed));
            }
            SetWager(wagerRequest : wagerRequest, roulette: roulette);

            return wageringRouletteDomainServie.Update(roulette: roulette);
        }

        private static void SetWager(WagerRequest wagerRequest, RouletteModel roulette)
        {
            roulette.Pocket[wagerRequest.Position].TryGetValue(wagerRequest.UserId, out decimal value);
            roulette.Pocket[wagerRequest.Position].Remove(wagerRequest.UserId);
            roulette.Pocket[wagerRequest.Position].TryAdd(wagerRequest.UserId, value + wagerRequest.Money);
        }

        public List<RouletteModel> GetAll()
        {
            return wageringRouletteDomainServie.GetAll();
        }
    }
}
