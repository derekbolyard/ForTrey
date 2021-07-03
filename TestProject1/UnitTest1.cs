using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace TestProject1
{
    // define type for object in input string
    public class Input
    {
        public string Service { get; set; }
        public int HttpStatus { get; set; }
    }

    public class OutputListObject
    {
        public int HttpStatus { get; set; }
        public int Count { get; set; }
    }

    public class Output
    {
        public string Service { get; set; }
        public List<OutputListObject> Status { get; set; }
    }

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            // Get inputString in a typed format so its easier to work with
            var inputArray = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Input>>("[{\"service\":\"payment\",\"httpStatus\":200},{\"service\":\"order\",\"httpStatus\":200},{\"service\":\"payment\",\"httpStatus\":200},{\"service\":\"payment\",\"httpStatus\":400},{\"service\":\"order\",\"httpStatus\":500}]");
            
            // Group input by service {Key = Service, Value = List<Input>}
            var inputsByService = inputArray.GroupBy(x => x.Service);
            var output = new List<Output>();

            foreach(var item in inputsByService)
            {
                var list = new List<OutputListObject>();

                // group items by httpStatusCode [Key = HttpStatus, Value = List<Input>]
                var inputByStatus = item.GroupBy(x => x.HttpStatus);
                
                // Map items to output list [{HttpStatus, Count}]
                foreach(var input in inputByStatus)
                {
                    list.Add(new OutputListObject
                    {
                        HttpStatus = input.Key,
                        Count = input.Count()
                    });
                }

                // Map to output [{Service, Status = [{HttpStatus, Count}]}]
                output.Add(new Output
                {
                    Service = item.Key,
                    Status = list
                });
            }


            // output variable is solution
            var paymentOutput = output.Single(x => x.Service == "payment");
            var orderOutput = output.Single(x => x.Service == "order");

            Assert.AreEqual(2, output.Count);
            Assert.AreEqual(2, paymentOutput.Status.Count);
            foreach(var item in paymentOutput.Status)
            {
                if (item.HttpStatus == 200)
                {
                    Assert.AreEqual(2, item.Count);
                }
                if (item.HttpStatus == 400)
                {
                    Assert.AreEqual(1, item.Count);
                }
            }

            foreach (var item in orderOutput.Status)
            {
                if (item.HttpStatus == 200)
                {
                    Assert.AreEqual(1, item.Count);
                }
                if (item.HttpStatus == 400)
                {
                    Assert.AreEqual(1, item.Count);
                }
            }

            //Output:[{ "service":"payment","status":[{ "httpStatus":200,"count":2},{ "httpStatus":400,"count":1}]},{ "service":"order","status":[{ "httpStatus":200,"count":1},{ "httpStatus":500,"count":1}]}]
        }
    }
}
