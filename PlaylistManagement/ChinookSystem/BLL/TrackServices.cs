using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using ChinookSystem.DAL;
using ChinookSystem.Models;
#endregion

namespace ChinookSystem.BLL
{
    public class TrackServices
    {
        #region Constructor for Context Dependency
        private readonly ChinookContext _context;
        internal TrackServices(ChinookContext context)
        {
            _context = context;
        }
        #endregion

        #region Queries
        List<TrackSelection> Track_FetchBy(string searcharg, string searchtype)
        {
            IEnumerable<TrackSelection> info = null;
            info = _context.Tracks
                    .Where(x => (searchtype.Equals("Album") && x.Album.Title.Contains(searcharg))
                             ||
                                (searchtype.Equals("Artist") && x.Album.Artist.Name.Contains(searcharg)))
                    .Select(x => new TrackSelection
                    {
                        TrackId = x.TrackId,
                        SongName = x.Name,
                        AlbumTitle = x.Album.Title,
                        ArtistName = x.Album.Artist.Name,
                        Milliseconds = x.Milliseconds,
                        Price = x.UnitPrice
                    })
                    .OrderBy(x => x.SongName);
            return info.ToList();
        }

        #endregion
    }
}
