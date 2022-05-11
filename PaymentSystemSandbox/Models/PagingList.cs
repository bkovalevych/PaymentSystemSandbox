namespace PaymentSystemSandbox.Models
{
    public class PagingList<T> : List<T>
    {
        public int TotalCount { get; set; }
        
        public int Offset { get; set; }

        public int Top { get; set; }
    }
}
