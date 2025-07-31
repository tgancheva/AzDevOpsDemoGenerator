namespace RestAPI.Viewmodel.WorkItem
{
    public class CreateUpdateNodeViewModel
    {
        public class Node : BaseViewModel
        {
            public int id { get; set; }
            public string name { get; set; }
            public Attributes attributes { get; set; }
        }

        public class Attributes
        {
            public string startDate { get; set; }
            public string finishDate { get; set; }
        }
    }
}
