namespace DACS2.ViewModels
{
    public class TestCaseViewModel
    {
        public int BaiHocId { get; set; }
        public string CssSelector { get; set; }
        public string Description { get; set; }
        public string TestType { get; set; }         
        public string Property { get; set; }          
        public string ExpectedValue { get; set; }      

        public bool UseJudge0 { get; set; } = false; 
    }
}
