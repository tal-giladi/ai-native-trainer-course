-- review-wait.sql — size a review-queue pain from pull-request data (lesson 01.2).
-- Assumes PR data synced into SQL Server (many shops sync GitHub/Azure DevOps for reporting):
--   dbo.PullRequest(PrId, RepoName, AuthorAlias, CreatedAt, MergedAt)
--   dbo.PullRequestReview(PrId, ReviewerAlias, SubmittedAt, State)
-- Adjust names to your warehouse. Read-only. Aggregates only: never export titles or names.

DECLARE @From datetime2 = '2026-06-01', @To datetime2 = '2026-09-01';

WITH firstReview AS (
    SELECT pr.PrId, pr.CreatedAt, pr.MergedAt,
           MIN(r.SubmittedAt) AS FirstReviewAt,
           COUNT(r.PrId)      AS Reviews
    FROM dbo.PullRequest pr
    LEFT JOIN dbo.PullRequestReview r ON r.PrId = pr.PrId
    WHERE pr.MergedAt >= @From AND pr.MergedAt < @To
    GROUP BY pr.PrId, pr.CreatedAt, pr.MergedAt
), waits AS (
    SELECT PrId, Reviews,
           DATEDIFF(MINUTE, CreatedAt, COALESCE(FirstReviewAt, MergedAt)) / 60.0 AS HoursToFirstReview
    FROM firstReview
)
SELECT DISTINCT
    COUNT(*) OVER ()                                                             AS MergedPrs,
    PERCENTILE_CONT(0.5) WITHIN GROUP (ORDER BY HoursToFirstReview) OVER ()      AS MedianHoursToFirstReview,
    PERCENTILE_CONT(0.9) WITHIN GROUP (ORDER BY HoursToFirstReview) OVER ()      AS P90HoursToFirstReview,
    SUM(CASE WHEN HoursToFirstReview > 48 THEN 1 ELSE 0 END) OVER ()             AS PrsWaitingOver48h,
    AVG(CAST(Reviews AS decimal(9,2))) OVER ()                                   AS AvgReviewsPerPr
FROM waits;

-- Reviewer concentration: is the queue a knowledge bottleneck?
SELECT TOP (5) r.ReviewerAlias,
       COUNT(DISTINCT r.PrId) AS PrsReviewed,
       CAST(100.0 * COUNT(DISTINCT r.PrId) /
            (SELECT COUNT(*) FROM dbo.PullRequest WHERE MergedAt >= @From AND MergedAt < @To) AS decimal(5,1)) AS PctOfPrs
FROM dbo.PullRequestReview r
JOIN dbo.PullRequest pr ON pr.PrId = r.PrId
WHERE pr.MergedAt >= @From AND pr.MergedAt < @To
GROUP BY r.ReviewerAlias
ORDER BY PrsReviewed DESC;

-- No warehouse? The GitHub CLI gives the same raw data (gh has jq built in):
--   gh pr list --state merged --limit 200 --json number,createdAt,mergedAt,reviews \
--     --jq '.[] | [.number, .createdAt, ((.reviews | map(.submittedAt) | min) // ""), .mergedAt, (.reviews | length)] | @tsv' > pr-review-wait.tsv
-- Open the TSV in a spreadsheet: hours to first review = (first review - created) * 24.
