namespace FileServer
{
    public class RetrieveFileRequest : IRequest
    {
        public string RequestType { get; set; }
        public string FileName { get; set; }

        public RetrieveFileRequest(string fileName)
        {
            RequestType = "RetrieveFile";
            FileName = fileName;
        }
    }
}
