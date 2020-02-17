using System;
using System.Collections.Generic;

namespace Scraper.Domain
{
    public class Cast
    {

        public int Id { get; set; }

        public IEnumerable<ShowCast> ShowCast { get; set; }

        public string Name { get; set; }

        public DateTime? Birthday { get; set; }

    }
}