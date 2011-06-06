//using System;
//using System.Collections.Generic;
//using Icodeon.Hotwire.Contracts;
//using NUnit.Framework;

//namespace Icodeon.Hotwire.Tests.AcceptanceTests.Endpoints
//{
//    [TestFixture]
//    public class SeperateFileProcessorPerEndpoint
//    {
//        // the test that is named the same as the class is the most basic "smoke" test possible
//        // all other tests are elaborations of the requirements...
//        [Test]
//        [Ignore("Not yet implemented, not currently planned for this iteration.")]
//        public void Be_able_to_configure_seperate_file_processor_per_endpoint()
//        {
//            // Given a FileProcessorModule configured with :
//            // endpoint A configured to use FileProcessor A
//            // And endpoint B configured to use FileProcessor B
//            // ================================================
            
//            throw new NotImplementedException();

           
            
//            var processorA = new FileProcessorA();
//            var processorB = new FileProcessorB();
//             /*
//            var endpointConfig = new EndPointConfiguration(Active = true, RootServiceName = "test",MethodValidation = MethodValidation.AfterUri)
//                .AddEndpoint(Name = "A", Active = true, UriTemplate = "processorA", FileProcessor = processorA,HttpMethod = HttpMethod.Post, MediaType = MediaType.Json, Security = Security.None)
//                .AddEndpoint(Name = "B", Active = true, UriTemplate = "processorB", FileProcessor = processorB,HttpMethod = HttpMethod.Post, MediaType = MediaType.Json, Security = "None");
//            var moduleConfig = new FileProcessorModuleConfiguration(endpointConfig);
//            var fileProcessorModule = new FileProcessorModule(moduleConfig);

//             */

//            // When endpoint A is triggered 
//            // ============================


//            // Then fileProcessor A is called


//            // When endpoint B is triggered
//            // Then fileProcessor B is called
//        }

//        // classes below marked as internal for this unit test only so that they do 
//        internal class TestFileProcessor : IFileProcessorProvider
//        {
//            private bool _called = false;
//            public bool Called
//            {
//                get
//                {
//                    return _called;
//                }
//            }
//            public void ProcessFile(string resource_file, string transaction_id, System.Collections.Specialized.NameValueCollection requestParams)
//            {
//                _called = true;
//            }
//        }

//        internal class FileProcessorA : TestFileProcessor {}
//        internal class FileProcessorB : TestFileProcessor { }
//    }
//}

// NB! the classes above will cause the tests in AllTheFactoryProvidersShouldBeDetectedAndWiredUpWithoutRequiringXmlConfiguration to fail
//      will need to make them private to the tests or update the ProviderFactory to exclude specific tests classes for autowireup?
