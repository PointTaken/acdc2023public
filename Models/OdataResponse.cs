namespace PirateApi.Functions.Models
{
    public class OdataResponse<T>
    {
        public List<T> value { get; set; } = new List<T>();
    }
}
