update pl
set 
	pl.Name = e.Name
from ProposalLines pl
inner join EstimateCategories e
on e.ID = pl.EstimateCategoryID