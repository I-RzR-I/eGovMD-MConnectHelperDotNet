using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MConnectHelperDotNet.Abstractions.Services;
using MConnectHelperDotNet.Models.DTO.Request;

namespace MConnectTest.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IMConnectApiService _mConnectApiService;

        public IndexModel(ILogger<IndexModel> logger, IMConnectApiService mConnectApiService)
        {
            _logger = logger;
            _mConnectApiService = mConnectApiService;
        }

        public async Task OnGetAsync()
        {
            var xml = "<mconnect:GetPErsonalInfo xmlns:mconnect=\"https://mconnect.gov.md\">" +
                        $"<mconnect:IDNP>{0000000000000}</mconnect:IDNP>" +
                      "</mconnect:GetPErsonalInfo>";
            var request = await _mConnectApiService.SendRequestAsync(new MConnectRequestDto()
            {
                CallingUserIdentifierCode = "0000000000000",
                TimeOut = 30,
                ValidateSignedMessage = false,
                SignMessage = true,
                SoapAction = "http://localhost:8088/api/v1/temp/GetPErsonalInfo",
                RequestEndPointUrl = "http://localhost:8088/api/v1/temp?wsdl",
                ContentHeaders = string.Empty,
                Content = xml
            });
        }
    }
}
