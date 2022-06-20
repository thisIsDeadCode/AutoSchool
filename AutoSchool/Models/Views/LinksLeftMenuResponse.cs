namespace AutoSchool.Models.Views
{
    public class LinksLeftMenuResponse : Response
    {
        public IEnumerable<LinkResponse> HrefCourses { get; set; }
        public IEnumerable<LinkResponse> HrefThemes { get; set; }
        public IEnumerable<LinkResponse> HrefLectures { get; set; }
        public IEnumerable<LinkResponse> HrefTests { get; set; }
    }
}
