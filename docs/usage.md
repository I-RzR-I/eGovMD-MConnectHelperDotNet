> :point_up: From the beginning let's make clear one moment. It is a wrapper for interoperability service connection provided by [e-governance agency](https://egov.md/). If you accessed this repository and already installed it or you want or install the package, and you already contacted the agency and discussed that and obtained the necessary information for clean implementation. :muscle:

<br/>

In curret wrapper is available method:
* `SendRequestAsync` -> Send request to external API.

For more details, please check the documentation obtained from the responsible authorities where you can find all the smallest details necessary for the implementation and understanding of the working flow.
<br/>
<hr/>

**Configure the application settings file**

In case you use `netstandard2.0`, `netstandard2.1`, `net5`, `netcoreapp3.1` in your project find a settings file like `appsettings.json` or `appsettings.env.json` and complete it with the following parameters.
```json
  "MConnectOptions": {
    "ClientCertificate": {
      "ClientCertificatePath": "App_Data/mconnect.pfx",
      "ClientCertificatePassword": "password"
    },
    "ServiceCertificate": {
      "Path": "App_Data/mconnect.cer"
    },
    "RequestHeaders": {
      "CallingEntity": "ENTITY",
      "CallingUser": "IDNP",
      "CallBasis": "Integration",
      "CallReason": "Integration"
    }
  }
```

* `CertificatePath` -> provide service certificate, the path with filename (PFX certificate);
* `CertificatePassword` -> provide the password to service certificate specified upper.

<hr/>

**Calling the service**

In case of using the `netstandard2.0+` in your project, after adding configuration data, you must set dependency injection for using functionality. In your project in the file `Startup.cs` add the following part of the code:
```csharp
public void ConfigureServices(IServiceCollection services)
        {
            ...
            
             services.AddMConnect(Configuration);
            
            ...
        }
```

`SendRequestAsync` accept input model of `MConnectRequestDto`
```csharp
    /// <summary>
    ///     Send request DTO
    /// </summary>
    public class MConnectRequestDto
    {
        /// <summary>
        ///     Calling user identifier code (IDNP)
        /// </summary>
        public string CallingUserIdentifierCode { get; set; }

        /// <summary>
        ///     Request endpoint
        /// </summary>
        public string RequestEndPointUrl { get; set; }
        
        /// <summary>
        ///     Method SOAP action
        /// </summary>
        public string SoapAction { get; set; }
        
        /// <summary>
        ///     Content headers
        /// </summary>
        public string ContentHeaders { get; set; }
        
        /// <summary>
        ///     Sign current message on request
        /// </summary>
        public bool SignMessage { get; set; } = true;
        
        /// <summary>
        ///     XML content data
        /// </summary>
        public string Content { get; set; }
        
        /// <summary>
        ///     Validate received response from MConnect
        /// </summary>
        public bool ValidateSignedMessage { get; set; }
        
        /// <summary>
        ///     Calling waiting timeout 
        /// </summary>
        public int TimeOut { get; set; } = 60;
    }
```
