namespace WageringRoulette.ApplicationServices.Configuration
{
    public class AppConfiguration
    {
        public decimal MaxMoney { get; set; }
        public decimal MinMoney { get; set; }
        public int Red { get; set; }
        public int Black { get; set; }
        public int MultiplierByNumber { get; set; }
        public decimal MultiplierByColor { get; set; }
    }
}
