using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Scraper.Domain;
using Scraper.Infrastructure;
using Scrapper.Service;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scraper.Service.Test
{
    public class ScraperQueryServiceTests
    {

        private IContainer container;

        [SetUp]
        public void Setup()
        {

            var builder = new ContainerBuilder();

            var loggingMock = new Mock<ILogger<ScraperQueryService>>();
            var showRepositoryMock = new Mock<IShowRepository>();

            var shows = new Show[] {
                new Show
                {
                    Id = 1,
                    Name = "Show 1",
                    ShowCast = new Domain.ShowCast[] {
                        new Domain.ShowCast{ Cast = new Domain.Cast{ Birthday = new System.DateTime(2001, 10, 2), Id = 1, Name = "Cast 1" } },
                        new Domain.ShowCast{ Cast = new Domain.Cast{ Birthday = new System.DateTime(1990, 11, 12), Id = 2, Name = "Cast 2" } },
                        new Domain.ShowCast{ Cast = new Domain.Cast{ Birthday = new System.DateTime(1943, 8, 15), Id = 3, Name = "Cast 3" } },
                        new Domain.ShowCast{ Cast = new Domain.Cast{ Birthday = new System.DateTime(1976, 1, 28), Id = 4, Name = "Cast 4" } },
                    }
                },
                new Show
                {
                    Id = 1,
                    Name = "Show 2",
                    ShowCast = new Domain.ShowCast[] {
                        new Domain.ShowCast{ Cast = new Domain.Cast{ Birthday = new System.DateTime(1988, 2, 20), Id = 5, Name = "Cast 5" } },
                        new Domain.ShowCast{ Cast = new Domain.Cast{ Birthday = new System.DateTime(1955, 7, 13), Id = 6, Name = "Cast 6" } },
                        new Domain.ShowCast{ Cast = new Domain.Cast{ Birthday = new System.DateTime(1967, 11, 9), Id = 7, Name = "Cast 7" } },
                        new Domain.ShowCast{ Cast = new Domain.Cast{ Birthday = new System.DateTime(1999, 11, 1), Id = 8, Name = "Cast " } },
                    }
                }
            };
            showRepositoryMock.Setup(s => s.GetShowsAsync(It.IsAny<int>())).Returns(Task.FromResult(shows.AsEnumerable()));


            builder.RegisterInstance(loggingMock.Object).As<ILogger<ScraperQueryService>>();
            builder.RegisterInstance(showRepositoryMock.Object).As<IShowRepository>();

            builder.RegisterType<ScraperQueryService>().As<IScraperQueryService>();

            container = builder.Build();

        }

        [Test]
        public async Task TestQuerySuccessful()
        {
            using var scope = container.BeginLifetimeScope();
            var service = scope.Resolve<IScraperQueryService>();
            var shows = await service.GetShowsAsync(0);

            Assert.IsNotNull(shows);
            Assert.AreEqual(shows.Count(), 2);
        }
    }
}