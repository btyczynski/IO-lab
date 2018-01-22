namespace ConsoleApplication14
{
    class Program
    {
        public async Task<XmlDocument> RunMakeXML(string address)
        {
            WebClient webClient = new WebClient(); ;
            var result = await webClient.DownloadStringTaskAsync(new Uri(address));
            XmlDocument myXML = new XmlDocument();
            myXML.LoadXml(result);

            return myXML;
        }

        static void Main(string[] args)
        {
            Program test = new Program();

            var task = test.RunMakeXML("http://www.feedforall.com/sample.xml");
            var xml = task.Result;

			using (var stringWriter = new StringWriter())
			{
					using (var xmlTextWriter = XmlWriter.Create(stringWriter))
					{
						xml.WriteTo(xmlTextWriter);
						xmlTextWriter.Flush();
						Console.WriteLine(stringWriter.GetStringBuilder().ToString());
					}
			}
			
        }

    }
}