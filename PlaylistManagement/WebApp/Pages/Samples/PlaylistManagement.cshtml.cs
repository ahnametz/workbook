using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

#region Additional 
using ChinookSystem.Models;
using WebApp.Helpers;
using ChinookSystem.BLL;
#endregion


namespace WebApp.Pages.SamplePages
{

    public class PlaylistManagementModel : PageModel
    {
        #region Private variables and DI constructor
        private readonly TrackServices _trackServices;
        private readonly PlaylistTrackServices _playlistServices;

        public PlaylistManagementModel(TrackServices trackServices, 
            PlaylistTrackServices playlistServices)
        {
            _trackServices=trackServices;
            _playlistServices=playlistServices;
        }
        #endregion

        #region Messaging and Error Handling
        [TempData]
        public string FeedBackMessage { get; set; }
        
        public string ErrorMessage { get; set; }

        //a get property that returns the result of the lamda action
        public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);
        public bool HasFeedBack => !string.IsNullOrWhiteSpace(FeedBackMessage);

        //used to display any collection of errors on web page
        //whether the errors are generated locally OR come form the class library
        //      service methods
        public List<string> ErrorDetails { get; set; } = new();

        //PageModel local error list for collection 
        public List<Exception> Errors { get; set; } = new();

        #endregion

        #region Paginator
        private const int PAGE_SIZE = 5;
        public Paginator Pager { get; set; }
        [BindProperty(SupportsGet = true)]
        public int? currentpage { get; set; }   
        #endregion

        [BindProperty(SupportsGet = true)]
        public string searchBy { get; set; }

        [BindProperty(SupportsGet = true)]
        public string searchArg { get; set; }

        [BindProperty(SupportsGet = true)]
        public string playlistname { get; set; }

        [BindProperty]
        public List<TrackSelection> trackInfo { get; set; } = new();

        public List<PlaylistInfo> qplaylistInfo { get; set; }

        //this property will be tied to the INPUT fields of the web page
        //this list is tied to the table data elements for the playlist
        //the =new() is REQUIRED to assist in retaining values during error handling
        [BindProperty]
        public List<PlayListTrackTRX> cplaylistInfo { get; set; } = new();

        //this property is tied to the form input element located on each
        //  of the rows of the track table
        //it will hold the trackid one wish to attempt to add to the playlist
        [BindProperty]
        public int addtrackid { get; set; }

        public const string USERNAME = "HansenB";
        public void OnGet()
        {
            //this method is executed everytime the page is call for the first time
            //   OR
            //whenever a Get request is made to the page SUCH AS RedirectToPage()
           
        }

        public void GetTrackInfo()
        {
           
        }

        public void GetPlaylist()
        {
           
        }
        public IActionResult OnPostTrackSearch()
        {
            try
            {
                //did they press a radio button?
                if (string.IsNullOrWhiteSpace(searchBy))
                {
                    Errors.Add(new Exception("Track search type not selected"));
                }
                //did they enter a search argument?
                if (string.IsNullOrWhiteSpace(searchArg))
                {
                    Errors.Add(new Exception("Track search string not entered"));
                }
                if (Errors.Any())
                {
                    throw new AggregateException(Errors);
                }
                trackInfo = _trackServices.Track_FetchBy(searchArg, searchBy);
                return Page();
            }
            catch (AggregateException ex)
            {
                ErrorMessage = "Unable to process search";
                foreach(var error in ex.InnerExceptions) 
                {
                    ErrorDetails.Add(error.Message);
                }
                return Page();
            }
            catch (Exception ex) 
            { 
                ErrorMessage = GetInnerException(ex).Message;
                return Page();
            }
            //return Page();
           
        }

        public IActionResult OnPostFetch()
        {
            try
            {
                //did they enter a search argument?
                if (string.IsNullOrWhiteSpace(playlistname))
                {
                    Errors.Add(new Exception("Enter a playlist name not entered"));
                }
                if (Errors.Any())
                {
                    throw new AggregateException(Errors);
                }
                //the username would be the login user name of the person using this form
                //this username SHOULD come from the system via security
                //HOWEVER we do not have security setup at this time SO a constant has
                //      been created with a known user name.
                qplaylistInfo = _playlistServices.PlaylistTrack_FetchPlaylist(playlistname, USERNAME);
                return Page();
            }
            catch (AggregateException ex)
            {
                ErrorMessage = "Unable to process search";
                foreach (var error in ex.InnerExceptions)
                {
                    ErrorDetails.Add(error.Message);
                }
                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = GetInnerException(ex).Message;
                return Page();
            }

            //return Page();
        }

        public IActionResult OnPostAddTrack()
        {

            try
            {
                //did they enter a search argument?
                if (string.IsNullOrWhiteSpace(playlistname))
                {
                    Errors.Add(new Exception("Enter a playlist name not entered"));
                }
                if (Errors.Any())
                {
                    throw new AggregateException(Errors);
                }

                _playlistServices.PlaylistTrack_AddTrack(playlistname, USERNAME, addtrackid);
                //Refresh the playlist AFTER adding successfully to the playlist
                qplaylistInfo = _playlistServices.PlaylistTrack_FetchPlaylist(playlistname, USERNAME);
                return Page();
            }
            catch (AggregateException ex)
            {
                ErrorMessage = "Unable to add a track";
                foreach (var error in ex.InnerExceptions)
                {
                    ErrorDetails.Add(error.Message);
                }
                qplaylistInfo = _playlistServices.PlaylistTrack_FetchPlaylist(playlistname, USERNAME);

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = GetInnerException(ex).Message;
                qplaylistInfo = _playlistServices.PlaylistTrack_FetchPlaylist(playlistname, USERNAME);

                return Page();
            }

            return Page();

        }

        public IActionResult OnPostRemove()
        {
            return Page();

        }

        public IActionResult OnPostReOrg()
        {
            return Page();

        }
        private Exception GetInnerException(Exception ex)
        {
            while(ex.InnerException != null)
                ex = ex.InnerException;
            return ex;
        }
    }
}
