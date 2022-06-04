namespace AutoSchool.Models.Views
{
    public class LinksLeftMenuResponse : Response
    {
        public IEnumerable<Link> HrefCourses { get; set; }
        public IEnumerable<Link> HrefThemes { get; set; }
        public IEnumerable<Link> HrefLectures { get; set; }
        public IEnumerable<Link> HrefTests { get; set; }

        public class Link
        {
            public string Title { get; set; }
            public string Href { get; set; }
        }
    }
}
