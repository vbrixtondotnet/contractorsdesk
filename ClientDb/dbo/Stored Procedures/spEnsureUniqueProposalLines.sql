CREATE PROCEDURE [dbo].[spEnsureUniqueProposalLines]
	@ProposalId uniqueidentifier
AS
BEGIN
	SET NOCOUNT ON;

	-- Keep one proposal line per Item Name within each parent category.
	-- Prefer lines with QB account mappings, then highest amount, then lowest sequence, then earliest ID.
	IF OBJECT_ID('tempdb..#RankedProposalLines') IS NOT NULL
		DROP TABLE #RankedProposalLines;

	SELECT
		pl.ID,
		pl.EstimateCategoryID,
		ROW_NUMBER() OVER (
			PARTITION BY
				pl.ProposalId,
				pl.ParentEstimateCategoryId,
				LOWER(LTRIM(RTRIM(pl.Name)))
			ORDER BY
				CASE
					WHEN EXISTS (
						SELECT 1
						FROM EstimateMappings em
						WHERE em.EstimateSubCategoryID = pl.EstimateCategoryID
					) THEN 1
					ELSE 0
				END DESC,
				pl.Amount DESC,
				pl.Sequence ASC,
				pl.ID ASC
		) AS RowNumber,
		FIRST_VALUE(pl.EstimateCategoryID) OVER (
			PARTITION BY
				pl.ProposalId,
				pl.ParentEstimateCategoryId,
				LOWER(LTRIM(RTRIM(pl.Name)))
			ORDER BY
				CASE
					WHEN EXISTS (
						SELECT 1
						FROM EstimateMappings em
						WHERE em.EstimateSubCategoryID = pl.EstimateCategoryID
					) THEN 1
					ELSE 0
				END DESC,
				pl.Amount DESC,
				pl.Sequence ASC,
				pl.ID ASC
		) AS KeepEstimateCategoryID
	INTO #RankedProposalLines
	FROM ProposalLines pl
	WHERE pl.ProposalId = @ProposalId
		AND pl.ParentEstimateCategoryId IS NOT NULL
		AND pl.Name IS NOT NULL
		AND LTRIM(RTRIM(pl.Name)) <> '';

	-- Move account mappings from duplicate categories onto the kept category.
	UPDATE em
	SET EstimateSubCategoryID = r.KeepEstimateCategoryID
	FROM EstimateMappings em
	INNER JOIN #RankedProposalLines r
		ON em.EstimateSubCategoryID = r.EstimateCategoryID
	WHERE r.RowNumber > 1
		AND r.EstimateCategoryID <> r.KeepEstimateCategoryID
		AND NOT EXISTS (
			SELECT 1
			FROM EstimateMappings emExisting
			WHERE emExisting.EstimateSubCategoryID = r.KeepEstimateCategoryID
				AND emExisting.QBAccountID = em.QBAccountID
		);

	DELETE em
	FROM EstimateMappings em
	INNER JOIN #RankedProposalLines r
		ON em.EstimateSubCategoryID = r.EstimateCategoryID
	WHERE r.RowNumber > 1;

	UPDATE stm
	SET EstimateCategoryId = r.KeepEstimateCategoryID
	FROM ScheduleTaskMappings stm
	INNER JOIN #RankedProposalLines r
		ON stm.EstimateCategoryId = r.EstimateCategoryID
	WHERE r.RowNumber > 1
		AND r.EstimateCategoryID <> r.KeepEstimateCategoryID
		AND NOT EXISTS (
			SELECT 1
			FROM ScheduleTaskMappings stmExisting
			WHERE stmExisting.EstimateCategoryId = r.KeepEstimateCategoryID
				AND stmExisting.ConstructionTaskId = stm.ConstructionTaskId
		);

	DELETE stm
	FROM ScheduleTaskMappings stm
	INNER JOIN #RankedProposalLines r
		ON stm.EstimateCategoryId = r.EstimateCategoryID
	WHERE r.RowNumber > 1;

	UPDATE ph
	SET EstimateCategoryID = r.KeepEstimateCategoryID
	FROM ProposalLinesHistory ph
	INNER JOIN #RankedProposalLines r
		ON ph.EstimateCategoryID = r.EstimateCategoryID
	WHERE ph.ProposalID = @ProposalId
		AND r.RowNumber > 1
		AND r.EstimateCategoryID <> r.KeepEstimateCategoryID;

	DELETE FROM ProposalLines
	WHERE ID IN (SELECT ID FROM #RankedProposalLines WHERE RowNumber > 1)
		AND ProposalId = @ProposalId;

	DROP TABLE #RankedProposalLines;

	EXEC spCleanUpDuplicateProposalLinesAndEstimateCategories @ProposalId;
END
