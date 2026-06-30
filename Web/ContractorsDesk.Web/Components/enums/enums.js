const Enums = {
    StatusReport: 1,
    ClientEmail: 2,
    ScheduleReport: 3,
    ProposalReport: 4,
    EstimateToActualReport: 5,
    RequestDeposit: 6,
    Invoice: 7,
    DepositRequest: 8,
    CompleteActionItem: 9,
    ScheduleRevision: 10,
    CostRevision: 11,
    ChangeOrder: 12,
    ActionItemStatus: {
		NotStarted : 1,
		InProgress : 2,
		ForReview : 3,
        PendingClientResponse : 4,
		ClientApproved : 5,
		Completed : 6,
        Archived: 7,
        PendingClientAcknowledgement : 8
    },
    ProposalStatus: {
        Draft: 1,
        Accepted: 2
    }
}