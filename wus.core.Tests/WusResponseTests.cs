using System.Collections.Generic;
using System.IO;
using System.Security;
using CoreWUS;
using Xunit;

namespace wus.core.Tests
{
    public class WusResponseTests : BaseTests
    {
        public WusResponseTests()
            : base()
        {
        }


        [Fact]
        void HandleResponse_ValidDelilverResponse_Pass()
        {
            // Given
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);
            WusResponse wusResponse = new WusResponse(xmlDSig, logger);

            string responseFile = Path.Combine(TestDataPath, "valid-deliver-response.xml");

            // When
            aanleverResponse actual = wusResponse.HandleResponse<aanleverResponse>(File.ReadAllText(responseFile));

            // Then
            Assert.True(actual != null);
        }

        [Fact]
        void HandleResponse_FaultDeliverResponse_Fail()
        {
            // Given
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);
            WusResponse wusResponse = new WusResponse(xmlDSig, logger);
            string expected = "ALS100";

            string responseFile = Path.Combine(TestDataPath, "fault-deliver-response.xml");

            // When

            // Then
            WusException actual = Assert.Throws<WusException>( () =>
                                wusResponse.HandleResponse<aanleverResponse>(File.ReadAllText(responseFile)));

            Assert.Equal(expected, actual.WusCode);
        }

        [Fact]
        void HandleResponse_InvalidSignature_Fail()
        {
            // Given
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);
            WusResponse wusResponse = new WusResponse(xmlDSig, logger);

            string responseFile = Path.Combine(TestDataPath, "tampered-deliver-response.xml");

            // When

            // Then
            Assert.Throws<VerificationException>( () =>
                            wusResponse.HandleResponse<aanleverResponse>(File.ReadAllText(responseFile)));
        }

        [Fact]
        void HandleResponse_ValidNewStatusResponse_Pass()
        {
            // Given
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);
            WusResponse wusResponse = new WusResponse(xmlDSig, logger);

            string responseFile = Path.Combine(TestDataPath, "valid-newstatus-response.xml");

            // When
            getNieuweStatussenProcesResponse actual = wusResponse.HandleResponse<getNieuweStatussenProcesResponse>(File.ReadAllText(responseFile));

            // Then
            Assert.True(actual != null);
        }

        [Fact]
        void HandleResponse_ValidNewStatusReturn_Pass()
        {
            // Given
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);
            WusResponse wusResponse = new WusResponse(xmlDSig, logger);

            string responseFile = Path.Combine(TestDataPath, "valid-newstatus-response.xml");

            // When
           IEnumerable<StatusResultaat> actual = wusResponse.HandleResponse<getNieuweStatussenProcesResponse>(File.ReadAllText(responseFile))
                                                    .getNieuweStatussenProcesReturn;

            // Then
            Assert.True(actual != null);
        }

        [Fact]
        void HandleResponse_FaultNewStatusResponse_Fail()
        {
            // Given
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);
            WusResponse wusResponse = new WusResponse(xmlDSig, logger);
            string expected = "STS100";

            string responseFile = Path.Combine(TestDataPath, "fault-newstatus-response.xml");

            // When

            // Then
            WusException actual = Assert.Throws<WusException>( () =>
                                wusResponse.HandleResponse<getNieuweStatussenProcesResponse>(File.ReadAllText(responseFile)));

            Assert.Equal(expected, actual.WusCode);
        }
    }
}