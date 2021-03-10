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
            // Arrange
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);
            WusResponse wusResponse = new WusResponse(xmlDSig, logger);

            string responseFile = Path.Combine(TestDataPath, "valid-deliver-response.xml");

            // Act
            aanleverResponse actual = wusResponse.HandleResponse<aanleverResponse>(File.ReadAllText(responseFile));

            // Assert
            Assert.True(actual != null);
        }

        [Fact]
        void HandleResponse_FaultDeliverResponse_Fail()
        {
            // Arrange
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);
            WusResponse wusResponse = new WusResponse(xmlDSig, logger);
            string expected = "ALS100";

            string responseFile = Path.Combine(TestDataPath, "fault-deliver-response.xml");

            // Act

            // Assert
            WusException actual = Assert.Throws<WusException>( () =>
                                wusResponse.HandleResponse<aanleverResponse>(File.ReadAllText(responseFile)));

            Assert.Equal(expected, actual.WusCode);
        }

        [Fact]
        void HandleResponse_InvalidSignature_Fail()
        {
            // Arrange
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);
            WusResponse wusResponse = new WusResponse(xmlDSig, logger);

            string responseFile = Path.Combine(TestDataPath, "tampered-deliver-response.xml");

            // Act

            // Assert
            Assert.Throws<VerificationException>( () =>
                            wusResponse.HandleResponse<aanleverResponse>(File.ReadAllText(responseFile)));
        }

        [Fact]
        void HandleResponse_ValidNewStatusResponse_Pass()
        {
            // Arrange
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);
            WusResponse wusResponse = new WusResponse(xmlDSig, logger);

            string responseFile = Path.Combine(TestDataPath, "valid-newstatus-response.xml");

            // Act
            getNieuweStatussenProcesResponse actual = wusResponse.HandleResponse<getNieuweStatussenProcesResponse>(File.ReadAllText(responseFile));

            // Assert
            Assert.True(actual != null);
        }

        [Fact]
        void HandleResponse_ValidNewStatusReturn_Pass()
        {
            // Arrange
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);
            WusResponse wusResponse = new WusResponse(xmlDSig, logger);

            string responseFile = Path.Combine(TestDataPath, "valid-newstatus-response.xml");

            // Act
           IEnumerable<StatusResultaat> actual = wusResponse.HandleResponse<getNieuweStatussenProcesResponse>(File.ReadAllText(responseFile))
                                                    .getNieuweStatussenProcesReturn;

            // Assert
            Assert.True(actual != null);
        }

        [Fact]
        void HandleResponse_FaultNewStatusResponse_Fail()
        {
            // Arrange
            ILogger logger = null;
            IWusXmlDSig xmlDSig = new WusXmlDSig(logger);
            WusResponse wusResponse = new WusResponse(xmlDSig, logger);
            string expected = "STS100";

            string responseFile = Path.Combine(TestDataPath, "fault-newstatus-response.xml");

            // Act

            // Assert
            WusException actual = Assert.Throws<WusException>( () =>
                                wusResponse.HandleResponse<getNieuweStatussenProcesResponse>(File.ReadAllText(responseFile)));

            Assert.Equal(expected, actual.WusCode);
        }
    }
}