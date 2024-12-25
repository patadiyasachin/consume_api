namespace loc_api_consume.Models
{
    public class CityModel
    {
        public int? CityID { get; set; }
        public int StateID { get; set; }
        public int CountryID { get; set; }
        public string CityName { get; set; }
        public string CityCode { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }

    public class CountryDropDownModel
    {
        public int CountryID { get; set; }
        public string CountryName { get; set; }
    }

    public class StateDropDownModel
    {
        public int StateID { get; set; }
        public string StateName { get; set; }
    }
}
