namespace AutoSchool.Models.Views
{
    public class LinksLeftMenuView
    {
        public IEnumerable<Link> HrefCourses { get; set; }
        public IEnumerable<Link> HrefThemes { get; set; }
        public IEnumerable<Link> HrefLectures { get; set; }
        public IEnumerable<Link> HrefTests { get; set; }
        public List<string> Errors { get; set; }


        public class Link
        {
            public string Title { get; set; }
            public string Href { get; set; }
        }
    }
}
