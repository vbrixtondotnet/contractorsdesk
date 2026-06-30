DELETE FROM ProposalLines WHERE ID IN (
select ID from (
select *, ROW_NUMBER() over (partition by ProposalId, EstimateCategoryId order by Name) as RowNumber from ProposalLines
) sq1
where RowNumber > 1
)