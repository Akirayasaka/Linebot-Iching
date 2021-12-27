namespace Linebot_Iching.Models
{

    public class FlexSample
    {
        public string type { get; set; }
        public Hero hero { get; set; }
        public Body body { get; set; }
        public Footer footer { get; set; }
    }

    public class Hero
    {
        public string type { get; set; }
        public string url { get; set; }
        public string size { get; set; }
        public string aspectRatio { get; set; }
        public string aspectMode { get; set; }
        public Action action { get; set; }
    }

    public class Action
    {
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Body
    {
        public string type { get; set; }
        public string layout { get; set; }
        public Content[] contents { get; set; }
    }

    public class Content
    {
        public string type { get; set; }
        public string text { get; set; }
        public string weight { get; set; }
        public string size { get; set; }
        public string layout { get; set; }
        public string margin { get; set; }
        public Content1[] contents { get; set; }
        public string spacing { get; set; }
    }

    public class Content1
    {
        public string type { get; set; }
        public string size { get; set; }
        public string url { get; set; }
        public string text { get; set; }
        public string color { get; set; }
        public string margin { get; set; }
        public int flex { get; set; }
        public string layout { get; set; }
        public string spacing { get; set; }
        public Content2[] contents { get; set; }
    }

    public class Content2
    {
        public string type { get; set; }
        public string text { get; set; }
        public string color { get; set; }
        public string size { get; set; }
        public int flex { get; set; }
        public bool wrap { get; set; }
    }

    public class Footer
    {
        public string type { get; set; }
        public string layout { get; set; }
        public string spacing { get; set; }
        public Content3[] contents { get; set; }
        public int flex { get; set; }
    }

    public class Content3
    {
        public string type { get; set; }
        public string style { get; set; }
        public string height { get; set; }
        public Action1 action { get; set; }
        public string size { get; set; }
    }

    public class Action1
    {
        public string type { get; set; }
        public string label { get; set; }
        public string uri { get; set; }
    }

}
