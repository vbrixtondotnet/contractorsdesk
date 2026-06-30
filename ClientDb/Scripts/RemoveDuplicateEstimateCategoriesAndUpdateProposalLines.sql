update pl
set 
	pl.EstimateCategoryID = '9C2C609C-0A88-4437-9466-F938F0FD8264',
	pl.ParentEstimateCategoryID = '851EFB5A-DE4F-4380-A264-AB31BB3ACECD'
from ProposalLines pl
inner join (
	select Id
	from estimatecategories where name = 'plumbing contractor' and ID != '9C2C609C-0A88-4437-9466-F938F0FD8264'
) sq1
on sq1.ID = pl.EstimateCategoryID
exec spCleanUpEstimateCategory '372520B8-E179-4657-A7B4-87961A3AA329'
exec spCleanUpEstimateCategory 'FAC74D2A-9D23-4062-8ACF-9127FB546FAC'
exec spCleanUpEstimateCategory '577F40A1-E175-43A4-B38F-A1884B021943'