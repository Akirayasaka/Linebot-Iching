namespace Linebot_Iching.Models
{

    public class Iching64
    {
        public Trigram[] trigrams { get; set; }
        public Hexagram[] hexagrams { get; set; }
    }

    public class Trigram
    {
        public int number { get; set; }
        public string name { get; set; }
        public string binary { get; set; }
        public string character { get; set; }
        public string family { get; set; }
        public string position { get; set; }
        public string animal { get; set; }
        public string body { get; set; }
        public string unicode { get; set; }
        public string element { get; set; }
        public string season { get; set; }
        public string[] society { get; set; }
        public string temperament { get; set; }
        public string nature { get; set; }
    }

    public class Hexagram
    {
        public int number { get; set; }
        public string name { get; set; }
        public string binary { get; set; }
        public string character { get; set; }
        public int topTrigram { get; set; }
        public int bottomTrigram { get; set; }
        public string text { get; set; }
        public Description description { get; set; }
        public Yao[] yao { get; set; }
    }

    public class Description
    {
        public string[] text { get; set; }
        public string[] wenyan { get; set; }
    }

    public class Yao
    {
        public int number { get; set; }
        public string name { get; set; }
        public string text { get; set; }
        public Description1 description { get; set; }
    }

    public class Description1
    {
        public string[] wenyan { get; set; }
        public string[] text { get; set; }
    }

}
