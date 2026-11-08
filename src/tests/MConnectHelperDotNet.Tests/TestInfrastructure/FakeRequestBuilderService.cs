using System;
using System.Threading;
using System.Threading.Tasks;
using MConnectHelperDotNet.Abstractions.Services;
using MConnectHelperDotNet.Models.DTO;
using MConnectHelperDotNet.Models.DTO.Request;
using RzR.ResultMessage;
using RzR.ResultMessage.Abstractions;

namespace MConnectHelperDotNet.Tests.TestInfrastructure
{
    public sealed class FakeRequestBuilderService : IRequestBuilderService
    {
        private int _callCount;

        public IResult<RequestBuilderDto> NextResult { get; set; } = Result<RequestBuilderDto>.Success(new RequestBuilderDto
        {
            EndpointUrl = TestValues.BaseUrl,
            CallingEntity = TestValues.CallingEntity,
            CallingUser = TestValues.CallingUser,
            CallBasis = TestValues.CallBasis,
            CallReason = TestValues.CallReason
        });

        public Func<GenerateRequestHeaderDto, IResult<RequestBuilderDto>> Handler { get; set; }

        public int CallCount => Volatile.Read(ref _callCount);

        public GenerateRequestHeaderDto LastRequest { get; private set; }

        public Task<IResult<RequestBuilderDto>> GetHeaderRequestDataAsync(GenerateRequestHeaderDto request,
            CancellationToken cancellationToken = default)
        {
            Interlocked.Increment(ref _callCount);
            LastRequest = request;

            return Task.FromResult(Handler != null ? Handler(request) : NextResult);
        }
    }
}
