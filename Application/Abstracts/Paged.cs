namespace Applications.Abstracts
{
    public sealed class Paged<TModel> where TModel : class
    {
        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int RecordsInPage { get; set; }

        public int TotalRecords { get; set; }

        public List<TModel>? Records { get; set; }
    }
}
