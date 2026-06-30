using ContractorsDesk.Core.Dto;
using ContractorsDesk.DataStore.Client.Models;
using ContractorsDesk.Services.@base;
using ContractorsDesk.Services.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ContractorsDesk.Core.ApiPayloadModels;
using Microsoft.AspNetCore.Authorization;

namespace ContractorsDesk.Services
{
	public class AudioUploadsService : BaseService, IAudioUploadsService
	{
		public AudioUploadsService(ClientDbContext clientDataDbContext, IMapper mapper)
			: base(mapper, clientDataDbContext) 
		{

		}

		public override async Task<T> CreateAsync<T>(object param)
		{
			if (param is UserUploadDto userUploadDto)
			{
				var dbUserUpload = this.mapper.Map<AudioUpload>(userUploadDto);

				ClientDbContext.AudioUploads.Add(dbUserUpload);
				await ClientDbContext.SaveChangesAsync();

				return this.mapper.Map<T>(userUploadDto);
			}
			else
			{
				throw new Exception("Invalid payload type for UserUploadsService:CreateAsync");
			}
		}

		public async Task<AudioUpload> UpdateAudioFileUploadStatusAsync(AudioUploadResponsePayload payload)
		{
			var audioFileUserUpload = await ClientDbContext.AudioUploads.FirstOrDefaultAsync(u => u.Id == payload.Ref);
			if (audioFileUserUpload != null) {

				if (!string.IsNullOrEmpty(payload.Status))
					audioFileUserUpload.Aistatus = payload.Status;

				audioFileUserUpload.Transcript = payload.Transcript;
				audioFileUserUpload.ProjectId = payload.ProjectId;

				await ClientDbContext.SaveChangesAsync();
				return audioFileUserUpload;
			}

			return new AudioUpload();
		}
	}
}
