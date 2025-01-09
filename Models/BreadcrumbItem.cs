namespace KASPDispetcher.Models
{
    public class BreadcrumbItem
    {
        public string Title { get; set; } // Название
        public string Url { get; set; } // Ссылка
        public bool IsActive { get; set; } // Текущая страница
    }
}
