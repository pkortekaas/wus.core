# wus.core

**wus.core** is a .NET Core C# example on how to communicate with Digipoort, the Dutch government gateway for exchanging XBRL data. The Digipoort API interface uses SOAP based webservices following the *WUS 2.0* protocol. This example assumes you are familiar with Digipoort and its purpose.

Most of the existing C# WUS 2.0 examples use the Windows Communication Foundation (WCF) framework, but Microsoft will not be porting this full framework to .NET Core as it is too tightly coupled to the Windows operating system.

There are projects like [CoreWCF](https://github.com/CoreWCF/CoreWCF) to enable WCF on .NET Core, but at this time there is no support for **message security**. And without message security it is not possible to have all the WS-* protocol support (Addressing, Security and Digital signatures) required by *WUS 2.0.*

The most likely successor to WCF on .NET Core will become [gRPC](https://grpc.io/), however this framework will no longer support [WS-* protocols](https://docs.microsoft.com/en-us/dotnet/architecture/grpc-for-wcf-developers/ws-protocols)

This C# example has its own WS-* protocol implementation, with the main part located in the **WusXmlDSig.cs** file. The rest of the implementation is pretty straightforward, using LINQ to XML, serializers, extensions and exceptions.

### Requirements
- .NET Core 3.1
- PKIo X.509 (test) certificate for identification and signing

**Note:** All development and testing has been done under Linux.

### Usage
- Get a copy of this example using: `git clone https://github.com/pkortekaas/wus.core.git`
- Do a `dotnet restore` to get all the dependency modules
- Main entrypoint is in WusClient/Program.cs
- Make sure your PKIo certifcate can be found, either through the certificate store or from the local filesystem
- You should be all set to run: `dotnet run -p WusClient` from the solution directory
- The sample code runs again the Digipoort conformance or preprod environment

### Remarks
- This code is at some points what verbose, to make it more readable
- If you run into certificate chain validation issues, make sure all the required intermediate CA certifiates are installed. This is different on the various operating systems.
- Of the various status requests, only the (New)StatusProcess requests are implemented

### Sample output
#### dotnet run -p WusClient
```
[18:02:48 INF] Program [Main] Startup
[18:02:48 INF] Program [Main] -------------------- Deliver --------------------
[18:02:49 DBG] HttpClientFactory [Create] Validate server certificate: 7C46D36D7D8B5B5CB14FCC6DDBCD551BDB8B1DD0 True
[18:02:49 INF] Program [Main] Aanleverkenmerk: Happyflow
[18:02:49 INF] Program [Main] Kenmerk: e5875052-f17b-4520-b7ec-ce41636e9934
[18:02:49 INF] Program [Main] ------------------- New Status ------------------
[18:02:49 INF] Program [Main] Status: 105 - Aanleverproces gestart.
[18:02:49 INF] Program [Main] Status: 100 - Aanleveren gelukt.
[18:02:49 INF] Program [Main] Status: 110 - Aanleverproces wordt aangeboden.
[18:02:49 INF] Program [Main] Status: 200 - Authenticatie gelukt.
[18:02:49 INF] Program [Main] Status: 301 - Validatie gelukt.
[18:02:49 INF] Program [Main] Status: 301 - Validatie gelukt.
[18:02:49 INF] Program [Main] Status: 405 - Afleveren naar uitvragende partij bezig...
[18:02:49 INF] Program [Main] Status: 400 - Afleveren uitvragende partij gelukt.
[18:02:49 INF] Program [Main] Status: 500 - Validatie bij de uitvragende partij gelukt.
[18:02:49 INF] Program [Main] ------------------- All Status ------------------
[18:02:49 INF] Program [Main] Status: 105 - Aanleverproces gestart.
[18:02:49 INF] Program [Main] Status: 100 - Aanleveren gelukt.
[18:02:49 INF] Program [Main] Status: 110 - Aanleverproces wordt aangeboden.
[18:02:49 INF] Program [Main] Status: 200 - Authenticatie gelukt.
[18:02:49 INF] Program [Main] Status: 301 - Validatie gelukt.
[18:02:49 INF] Program [Main] Status: 301 - Validatie gelukt.
[18:02:49 INF] Program [Main] Status: 405 - Afleveren naar uitvragende partij bezig...
[18:02:49 INF] Program [Main] Status: 400 - Afleveren uitvragende partij gelukt.
[18:02:49 INF] Program [Main] Status: 500 - Validatie bij de uitvragende partij gelukt.
[18:02:49 INF] Program [Main] Shutdown
```
### References
[Digipoort](https://www.logius.nl/diensten/digipoort)

[Koppelvlak WUS voor bedrijven](https://www.logius.nl/diensten/digipoort/koppelvlakken/wus-voor-bedrijven)

[Aansluit Suite Digipoort](https://aansluiten.procesinfrastructuur.nl/)
