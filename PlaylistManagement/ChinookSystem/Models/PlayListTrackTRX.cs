using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChinookSystem.Models
{
    public class PlayListTrackTRX
    {
        public bool SelectedTrack { get; set; }
        public int TrackId { get; set; }
        public int CurrentTrackNumber { get; set; }
        public int NewTrackNumber { get; set; }
    }
}
