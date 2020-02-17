using System;
using System.Collections.Generic;
using System.Text;

namespace Scraper.Domain
{
    public class Show
    {

        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<ShowCast> ShowCast { get; set; }

    }
}
