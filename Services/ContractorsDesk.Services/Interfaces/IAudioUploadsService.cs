using ContractorsDesk.Core.ApiPayloadModels;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.Interfaces.@base;

namespace ContractorsDesk.Services.Interfaces
{
	public interface IAudioUploadsService : IBaseService
	{
		Task<AudioUpload> UpdateAudioFileUploadStatusAsync(AudioUploadResponsePayload payload);
	}
}
