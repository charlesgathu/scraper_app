using System.Collections.Generic;

namespace Scrapper.Service
{
    public class ShowResponseDto
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public IEnumerable<CastResposeDto> Cast { get; set; }
    }
}