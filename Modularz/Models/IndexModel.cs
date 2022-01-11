using Modularz.Data.EF;

namespace Modularz.Models;

public class IndexModel
{
    public BlogPost Post { get; set; }
    public List<MiniPostModel> MiniPostModels { get; set; }

    public class MiniPostModel
    {
        public DateTime  DatePublished { get; set; }
        public string Title { get; set; }
    }
}