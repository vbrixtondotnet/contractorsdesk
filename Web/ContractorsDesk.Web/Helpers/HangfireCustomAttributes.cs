using Hangfire.Common;
using Hangfire.States;

namespace ContractorsDesk.WebPortal.Helpers
{
	public class DisableRetriesAttribute : JobFilterAttribute, IElectStateFilter
	{
		public void OnStateElection(ElectStateContext context)
		{
			// If the job fails, set it to "Deleted" instead of retrying
			if (context.CandidateState is FailedState)
			{
				context.CandidateState = new DeletedState();
			}
		}
	}
}
