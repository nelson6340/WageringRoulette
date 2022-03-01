using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using WageringRoulette.ApplicationServices;
using WageringRoulette.ApplicationServices.Configuration;
using WageringRoulette.ApplicationServices.Exceptions;
using WageringRoulette.ApplicationServices.Requests;
using WageringRoulette.DomainServices.Abstraction;
using WageringRoulette.DomainServices.Model;
using Xunit;
using static WageringRoulette.ApplicationServices.ErrorHandling.MessageHandler;

namespace WageringRoulette.UnitTest
{
    public class WageringRouletteApplicationServiceTest
    {
        private readonly Mock<IWageringRouletteDomainServie> mockDomainServices = new Mock<IWageringRouletteDomainServie>();
        readonly IOptions<AppConfiguration> appConfiguration = Options.Create(new AppConfiguration()
        {
            Black = 38,
            Red = 39,
            MinMoney = 0.1m,
            MaxMoney = 10000,
            MultiplierByNumber = 5,
            MultiplierByColor = 1.8m
        });

        [Fact]
        public void Create_Should_IdCreated()
        {
            mockDomainServices.Setup(x => x.Save(It.IsAny<RouletteModel>()));
            var instance = new WageringRouletteApplicationServie(mockDomainServices.Object, null);
            var result = instance.Create();

            Assert.NotNull(result.Id);
        }

        [Fact]
        public void OpenWager_Should_WagerOpened()
        {
            string id = Guid.NewGuid().ToString();

            RouletteModel rouletteModel = new RouletteModel()
            {
                Id = id,
                ClosingDate = null,
                IsOpen = true,
                OpeningDate = null,
                Pocket = new IDictionary<string, decimal>[40]
            };
            mockDomainServices.Setup(x => x.Update(rouletteModel)).Returns(rouletteModel);
            mockDomainServices.Setup(x => x.GetById(id)).Returns(rouletteModel);

            var instance = new WageringRouletteApplicationServie(mockDomainServices.Object, null);
            var result = instance.OpenWager(id);

            Assert.True(result.IsOpen);
        }

        [Fact]
        public void OpenWager_Should_ErrorNotAllowedOpen()
        {
            string id = Guid.NewGuid().ToString();
            RouletteModel rouletteModel = new RouletteModel()
            {
                Id = id,
                ClosingDate = null,
                IsOpen = true,
                OpeningDate = "2022-02-28T10:01:54.9571247Z",
                Pocket = new IDictionary<string, decimal>[40]
            };
            mockDomainServices.Setup(x => x.Update(It.IsAny<RouletteModel>()));
            mockDomainServices.Setup(x => x.GetById(id)).Returns(rouletteModel);
            var instance = new WageringRouletteApplicationServie(mockDomainServices.Object, null);
            BusinessException ex = Assert.Throws<BusinessException>(() => instance.OpenWager(id));

            Assert.Equal(GetErrorDescription(MessageCodes.NotAllowedOpen), ex.Message);
        }

        [Fact]
        public void OpenWager_Should_ErrorNotFound()
        {
            string id = Guid.NewGuid().ToString();
            mockDomainServices.Setup(x => x.GetById(id));
            var instance = new WageringRouletteApplicationServie(mockDomainServices.Object, null);
            BusinessException ex = Assert.Throws<BusinessException>(() => instance.OpenWager(id));

            Assert.Equal(GetErrorDescription(MessageCodes.NotFound), ex.Message);
        }

        [Fact]
        public void Find_Should_Found()
        {
            string id = Guid.NewGuid().ToString();
            RouletteModel rouletteModel = new RouletteModel()
            {
                Id = id,
                ClosingDate = null,
                IsOpen = true,
                OpeningDate = null,
                Pocket = new IDictionary<string, decimal>[40]
            };
            mockDomainServices.Setup(x => x.GetById(id)).Returns(rouletteModel);
            var instance = new WageringRouletteApplicationServie(mockDomainServices.Object, null);
            var result = instance.Find(id);

            Assert.NotNull(result);
        }

        [Fact]
        public void CloseWager_Should_ErrorNotFound()
        {
            string id = Guid.NewGuid().ToString();
            mockDomainServices.Setup(x => x.GetById(id));
            var instance = new WageringRouletteApplicationServie(mockDomainServices.Object, null);
            BusinessException ex = Assert.Throws<BusinessException>(() => instance.CloseWager(id));

            Assert.Equal(GetErrorDescription(MessageCodes.NotFound), ex.Message);
        }

        [Fact]
        public void CloseWager_Should_ErrorNotAllowedOpen()
        {
            string id = Guid.NewGuid().ToString();

            RouletteModel rouletteModel = new RouletteModel()
            {
                Id = id,
                ClosingDate = null,
                IsOpen = true,
                OpeningDate = null,
                Pocket = new IDictionary<string, decimal>[40]
            };

            mockDomainServices.Setup(x => x.Update(It.IsAny<RouletteModel>()));
            mockDomainServices.Setup(x => x.GetById(id)).Returns(rouletteModel);
            var instance = new WageringRouletteApplicationServie(mockDomainServices.Object, null);
            BusinessException ex = Assert.Throws<BusinessException>(() => instance.CloseWager(id));

            Assert.Equal(GetErrorDescription(MessageCodes.NotAllowedCloseByOpen), ex.Message);
        }

        [Fact]
        public void CloseWager_Should_ErrorNotAllowedClose()
        {
            string id = Guid.NewGuid().ToString();

            RouletteModel rouletteModel = new RouletteModel()
            {
                Id = id,
                ClosingDate = "2022-02-28T10:01:54.9571247Z",
                IsOpen = true,
                OpeningDate = "2022-02-28T10:01:54.9571247Z",
                Pocket = new IDictionary<string, decimal>[40]
            };

            mockDomainServices.Setup(x => x.Update(It.IsAny<RouletteModel>()));
            mockDomainServices.Setup(x => x.GetById(id)).Returns(rouletteModel);
            var instance = new WageringRouletteApplicationServie(mockDomainServices.Object, null);
            BusinessException ex = Assert.Throws<BusinessException>(() => instance.CloseWager(id));

            Assert.Equal(GetErrorDescription(MessageCodes.NotAllowedClose), ex.Message);
        }

        [Fact]
        public void Wager_Should_ErrorMoneyOutRange()
        {
            string id = Guid.NewGuid().ToString();

            WagerRequest wagerRequest = new WagerRequest()
            {
                Money = 0
            };

            RouletteModel rouletteModel = new RouletteModel()
            {
                Id = id,
                ClosingDate = "2022-02-28T10:01:54.9571247Z",
                IsOpen = true,
                OpeningDate = "2022-02-28T10:01:54.9571247Z",
                Pocket = new IDictionary<string, decimal>[40]
            };



            mockDomainServices.Setup(x => x.Update(It.IsAny<RouletteModel>()));
            mockDomainServices.Setup(x => x.GetById(id)).Returns(rouletteModel);
            var instance = new WageringRouletteApplicationServie(mockDomainServices.Object, appConfiguration);
            BusinessException ex = Assert.Throws<BusinessException>(() => instance.Wager(id, wagerRequest));

            Assert.Equal(GetErrorDescription(MessageCodes.MoneyOutRange), ex.Message);
        }

        [Fact]
        public void Wager_Should_ErrorNotFound()
        {
            string id = Guid.NewGuid().ToString();

            WagerRequest wagerRequest = new WagerRequest()
            {
                Money = 10,
                Position = 1,
                UserId = "Usuario"
            };
            mockDomainServices.Setup(x => x.Update(It.IsAny<RouletteModel>()));
            var instance = new WageringRouletteApplicationServie(mockDomainServices.Object, appConfiguration);
            BusinessException ex = Assert.Throws<BusinessException>(() => instance.Wager(id, wagerRequest));

            Assert.Equal(GetErrorDescription(MessageCodes.NotFound), ex.Message);
        }

        [Fact]
        public void Wager_Should_ErrorRouletteClosed()
        {
            string id = Guid.NewGuid().ToString();

            WagerRequest wagerRequest = new WagerRequest()
            {
                Money = 10,
                Position = 1,
                UserId = "Usuario"
            };

            RouletteModel rouletteModel = new RouletteModel()
            {
                Id = id,
                ClosingDate = "2022-02-28T10:01:54.9571247Z",
                IsOpen = false,
                OpeningDate = "2022-02-28T10:01:54.9571247Z",
                Pocket = new IDictionary<string, decimal>[40]
            };

            mockDomainServices.Setup(x => x.Update(It.IsAny<RouletteModel>()));
            mockDomainServices.Setup(x => x.GetById(id)).Returns(rouletteModel);
            var instance = new WageringRouletteApplicationServie(mockDomainServices.Object, appConfiguration);
            BusinessException ex = Assert.Throws<BusinessException>(() => instance.Wager(id, wagerRequest));

            Assert.Equal(GetErrorDescription(MessageCodes.RouletteClosed), ex.Message);
        }

        [Fact]
        public void Wager_Should_Wagered()
        {
            string id = Guid.NewGuid().ToString();

            WagerRequest wagerRequest = new WagerRequest()
            {
                Money = 10,
                Position = 1,
                UserId = "Usuario"
            };
            RouletteModel rouletteModel = new RouletteModel()
            {
                Id = id,
                ClosingDate = null,
                IsOpen = true,
                OpeningDate = "2022-02-28T10:01:54.9571247Z",
                Pocket = new IDictionary<string, decimal>[40]
            };
            for (int i = 0; i < rouletteModel.Pocket.Length; i++)
            {
                rouletteModel.Pocket[i] = new Dictionary<string, decimal>();
            }

            mockDomainServices.Setup(x => x.Update(It.IsAny<RouletteModel>())).Returns(rouletteModel);
            mockDomainServices.Setup(x => x.GetById(id)).Returns(rouletteModel);
            var instance = new WageringRouletteApplicationServie(mockDomainServices.Object, appConfiguration);
            var result = instance.Wager(id, wagerRequest);

            Assert.True(result.Pocket[1].Any());
        }
    }
}
