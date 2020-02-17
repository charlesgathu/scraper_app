using Microsoft.EntityFrameworkCore;
using Scraper.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Scraper.Infrastructure
{
    public class ShowRepository : IShowRepository
    {

        private readonly ScraperContext context;
        private const int pageCapacity = 250;

        public ShowRepository(ScraperContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(ScraperContext));
        }

        public IUnitOfWork UnitOfWork
        {
            get
            {
                return context;
            }
        }

        public Show Add(Show show)
        {
            return context.Shows.Add(show).Entity;
        }

        public async Task<Show> GetAsync(int showId)
        {
            var show = await context.Shows.Include("ShowCast.Cast").FirstOrDefaultAsync(o => o.Id == showId);

            if (show == null)
            {
                show = context.Shows.Local.FirstOrDefault(o => o.Id == showId);
            }

            return show;
        }

        public async Task<int> GetLastIndexAsync()
        {
            var count = await context.Shows.CountAsync();
            return (int)Math.Floor((decimal)(count / pageCapacity));
        }

        public async Task<IEnumerable<Show>> GetShowsAsync(int index)
        {
            var shows = await context.Shows.Include("ShowCast.Cast").Skip(index * pageCapacity).Take(pageCapacity).ToArrayAsync();
            return shows;
        }

        public void Update(Show show)
        {
            context.Entry(show).State = EntityState.Modified;
        }

        public async Task<Cast> GetCastAsync(int castId)
        {
            var cast = await context.Cast.FindAsync(castId);
            return cast;
        }

        public async Task<ShowCast> GetShowCastAsync(int showId, int castId)
        {
            var showCast = await context.ShowCast.FirstOrDefaultAsync(sel => sel.ShowId == showId && sel.CastId == castId);
            return showCast;
        }
    }
}
