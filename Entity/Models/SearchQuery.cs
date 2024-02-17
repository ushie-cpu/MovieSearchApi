namespace Entity.Models
{
    public class SearchQuery
    {
        public string Title { get; set; }
        public DateTime SearchTime { get; set; } = DateTime.Now;    
    }
}
